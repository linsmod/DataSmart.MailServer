using System.NetworkToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class GroupCollection : IEnumerable
	{
		private VirtualServer m_pVirtualServer;

		private List<Group> m_pGroups;

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
				return this.m_pGroups.Count;
			}
		}

		public Group this[int index]
		{
			get
			{
				return this.m_pGroups[index];
			}
		}

		public Group this[string groupID]
		{
			get
			{
				foreach (Group current in this.m_pGroups)
				{
					if (current.GroupID.ToLower() == groupID.ToLower())
					{
						return current;
					}
				}
				throw new Exception("Group with specified ID '" + groupID + "' doesn't exist !");
			}
		}

		internal GroupCollection(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pGroups = new List<Group>();
			this.Bind();
		}

		public Group Add(string name, string description, bool enabled)
		{
			string text = Guid.NewGuid().ToString();
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"AddGroup ",
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
			Group group = new Group(this.m_pVirtualServer, this, text, name, description, enabled);
			this.m_pGroups.Add(group);
			return group;
		}

		public void Remove(Group group)
		{
			Guid.NewGuid().ToString();
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("DeleteGroup " + this.m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(group.GroupID));
			string text = this.m_pVirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pGroups.Remove(group);
		}

		public bool Contains(string groupName)
		{
			foreach (Group current in this.m_pGroups)
			{
				if (current.GroupName.ToLower() == groupName.ToLower())
				{
					return true;
				}
			}
			return false;
		}

		public Group GetGroupByName(string groupName)
		{
			foreach (Group current in this.m_pGroups)
			{
				if (current.GroupName.ToLower() == groupName.ToLower())
				{
					return current;
				}
			}
			throw new Exception("Group with specified name '" + groupName + "' doesn't exist !");
		}

		public void Refresh()
		{
			this.m_pGroups.Clear();
			this.Bind();
		}

		private void Bind()
		{
			lock (this.m_pVirtualServer.Server.LockSynchronizer)
			{
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetGroups " + this.m_pVirtualServer.VirtualServerID);
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
				if (dataSet.Tables.Contains("Groups"))
				{
					foreach (DataRow dataRow in dataSet.Tables["Groups"].Rows)
					{
						this.m_pGroups.Add(new Group(this.m_pVirtualServer, this, dataRow["GroupID"].ToString(), dataRow["GroupName"].ToString(), dataRow["Description"].ToString(), Convert.ToBoolean(dataRow["Enabled"].ToString())));
					}
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pGroups.GetEnumerator();
		}
	}
}
