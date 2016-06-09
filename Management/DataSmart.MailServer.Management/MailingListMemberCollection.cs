using System.NetworkToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class MailingListMemberCollection : IEnumerable
	{
		private MailingList m_pMailingList;

		private List<string> m_pMembers;

		public int Count
		{
			get
			{
				return this.m_pMembers.Count;
			}
		}

		public string this[int index]
		{
			get
			{
				return this.m_pMembers[index];
			}
		}

		public string this[string member]
		{
			get
			{
				foreach (string current in this.m_pMembers)
				{
					if (current.ToLower() == member.ToLower())
					{
						return current;
					}
				}
				throw new Exception("Mailing list member '" + member + "' doesn't exist !");
			}
		}

		internal MailingListMemberCollection(MailingList mailingList)
		{
			this.m_pMailingList = mailingList;
			this.m_pMembers = new List<string>();
			this.Bind();
		}

		public void Add(string member)
		{
			Guid.NewGuid().ToString();
			this.m_pMailingList.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
			{
				"AddMailingListMember ",
				this.m_pMailingList.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pMailingList.ID),
				" ",
				TextUtils.QuoteString(member)
			}));
			string text = this.m_pMailingList.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pMembers.Add(member);
		}

		public void Remove(string member)
		{
			Guid.NewGuid().ToString();
			this.m_pMailingList.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
			{
				"DeleteMailingListMember ",
				this.m_pMailingList.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pMailingList.ID),
				" ",
				TextUtils.QuoteString(member)
			}));
			string text = this.m_pMailingList.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pMembers.Remove(member);
		}

		private void Bind()
		{
			lock (this.m_pMailingList.VirtualServer.Server.LockSynchronizer)
			{
				this.m_pMailingList.VirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetMailingListMembers " + this.m_pMailingList.VirtualServer.VirtualServerID + " " + this.m_pMailingList.ID);
				string text = this.m_pMailingList.VirtualServer.Server.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
				int num = Convert.ToInt32(text.Split(new char[]
				{
					' '
				}, 2)[1]);
				MemoryStream memoryStream = new MemoryStream();
				this.m_pMailingList.VirtualServer.Server.TCP_Client.TcpStream.ReadFixedCount(memoryStream, (long)num);
				DataSet dataSet = Utils.DecompressDataSet(memoryStream);
				if (dataSet.Tables.Contains("MailingListAddresses") && dataSet.Tables.Contains("MailingListAddresses"))
				{
					foreach (DataRow dataRow in dataSet.Tables["MailingListAddresses"].Rows)
					{
						this.m_pMembers.Add(dataRow["Address"].ToString());
					}
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pMembers.GetEnumerator();
		}
	}
}
