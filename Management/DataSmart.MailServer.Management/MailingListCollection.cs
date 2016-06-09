using System.NetworkToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class MailingListCollection : IEnumerable
	{
		private VirtualServer m_pVirtualServer;

		private List<MailingList> m_pMailingLists;

		public VirtualServer VirtualServer
		{
			get
			{
				return this.m_pVirtualServer;
			}
		}

		public int Count
		{
			get
			{
				return this.m_pMailingLists.Count;
			}
		}

		public MailingList this[int index]
		{
			get
			{
				return this.m_pMailingLists[index];
			}
		}

		public MailingList this[string mailingListID]
		{
			get
			{
				foreach (MailingList current in this.m_pMailingLists)
				{
					if (current.ID.ToLower() == mailingListID.ToLower())
					{
						return current;
					}
				}
				throw new Exception("Mailing list with specified ID '" + mailingListID + "' doesn't exist !");
			}
		}

		internal MailingListCollection(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pMailingLists = new List<MailingList>();
			this.Bind();
		}

		public MailingList Add(string name, string description, bool enabled)
		{
			string text = Guid.NewGuid().ToString();
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"AddMailingList ",
				this.m_pVirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(text),
				" ",
				TextUtils.QuoteString(name),
				" ",
				TextUtils.QuoteString(description),
				" ",
				enabled
			}));
			string text2 = this.m_pVirtualServer.Server.ReadLine();
			if (!text2.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text2);
			}
			MailingList mailingList = new MailingList(this.m_pVirtualServer, this, text, name, description, enabled);
			this.m_pMailingLists.Add(mailingList);
			return mailingList;
		}

		public void Remove(MailingList mailingList)
		{
			Guid.NewGuid().ToString();
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("DeleteMailingList " + this.m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(mailingList.ID));
			string text = this.m_pVirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pMailingLists.Remove(mailingList);
		}

		public bool Contains(string mailingListName)
		{
			foreach (MailingList current in this.m_pMailingLists)
			{
				if (current.Name.ToLower() == mailingListName.ToLower())
				{
					return true;
				}
			}
			return false;
		}

		public MailingList GetMailingListByName(string mailingListName)
		{
			foreach (MailingList current in this.m_pMailingLists)
			{
				if (current.Name.ToLower() == mailingListName.ToLower())
				{
					return current;
				}
			}
			throw new Exception("Mailing list with specified name '" + mailingListName + "' doesn't exist !");
		}

		public void Refresh()
		{
			this.m_pMailingLists.Clear();
			this.Bind();
		}

		private void Bind()
		{
			lock (this.m_pVirtualServer.Server.LockSynchronizer)
			{
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetMailingLists " + this.m_pVirtualServer.VirtualServerID);
				string text = this.m_pVirtualServer.Server.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
				int num = Convert.ToInt32(text.Split(new char[]
				{
					' '
				}, 2)[1]);
				MemoryStream memoryStream = new MemoryStream();
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.ReadFixedCount(memoryStream, (long)num);
				DataSet dataSet = Utils.DecompressDataSet(memoryStream);
				if (dataSet.Tables.Contains("MailingLists"))
				{
					foreach (DataRow dataRow in dataSet.Tables["MailingLists"].Rows)
					{
						this.m_pMailingLists.Add(new MailingList(this.m_pVirtualServer, this, dataRow["MailingListID"].ToString(), dataRow["MailingListName"].ToString(), dataRow["Description"].ToString(), Convert.ToBoolean(dataRow["Enabled"])));
					}
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pMailingLists.GetEnumerator();
		}
	}
}
