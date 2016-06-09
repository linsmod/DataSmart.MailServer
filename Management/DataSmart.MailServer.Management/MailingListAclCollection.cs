using System.NetworkToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class MailingListAclCollection
	{
		private MailingList m_pMailingList;

		private List<string> m_pAcl;

		public int Count
		{
			get
			{
				return this.m_pAcl.Count;
			}
		}

		public string this[int index]
		{
			get
			{
				return this.m_pAcl[index];
			}
		}

		public string this[string aclEntry]
		{
			get
			{
				foreach (string current in this.m_pAcl)
				{
					if (current.ToLower() == aclEntry.ToLower())
					{
						return current;
					}
				}
				throw new Exception("Mailing list ACL entry '" + aclEntry + "' doesn't exist !");
			}
		}

		internal MailingListAclCollection(MailingList mailingList)
		{
			this.m_pMailingList = mailingList;
			this.m_pAcl = new List<string>();
			this.Bind();
		}

		public void Add(string userOrGroup)
		{
			Guid.NewGuid().ToString();
			this.m_pMailingList.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
			{
				"AddMailingListAcl ",
				this.m_pMailingList.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pMailingList.ID),
				" ",
				TextUtils.QuoteString(userOrGroup)
			}));
			string text = this.m_pMailingList.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pAcl.Add(userOrGroup);
		}

		public void Remove(string userOrGroup)
		{
			Guid.NewGuid().ToString();
			this.m_pMailingList.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
			{
				"DeleteMailingListAcl ",
				this.m_pMailingList.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pMailingList.ID),
				" ",
				TextUtils.QuoteString(userOrGroup)
			}));
			string text = this.m_pMailingList.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pAcl.Remove(userOrGroup);
		}

		private void Bind()
		{
			lock (this.m_pMailingList.VirtualServer.Server.LockSynchronizer)
			{
				this.m_pMailingList.VirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetMailingListAcl " + this.m_pMailingList.VirtualServer.VirtualServerID + " " + this.m_pMailingList.ID);
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
				if (dataSet.Tables.Contains("ACL") && dataSet.Tables.Contains("ACL"))
				{
					foreach (DataRow dataRow in dataSet.Tables["ACL"].Rows)
					{
						this.m_pAcl.Add(dataRow["UserOrGroup"].ToString());
					}
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pAcl.GetEnumerator();
		}
	}
}
