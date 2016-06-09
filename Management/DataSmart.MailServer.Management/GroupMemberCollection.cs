using System.NetworkToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class GroupMemberCollection : IEnumerable
	{
		private Group m_pGroup;

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
				throw new Exception("Member '" + member + "' doesn't exist !");
			}
		}

		internal GroupMemberCollection(Group group)
		{
			this.m_pGroup = group;
			this.m_pMembers = new List<string>();
			this.Bind();
		}

		public void Add(string userOrGroup)
		{
			Guid.NewGuid().ToString();
			this.m_pGroup.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
			{
				"AddGroupMember ",
				this.m_pGroup.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pGroup.GroupID),
				" ",
				TextUtils.QuoteString(userOrGroup)
			}));
			string text = this.m_pGroup.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pMembers.Add(userOrGroup);
		}

		public void Remove(string userOrGroup)
		{
			Guid.NewGuid().ToString();
			this.m_pGroup.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
			{
				"DeleteGroupMember ",
				this.m_pGroup.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pGroup.GroupID),
				" ",
				TextUtils.QuoteString(userOrGroup)
			}));
			string text = this.m_pGroup.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pMembers.Remove(userOrGroup);
		}

		public bool Contains(string member)
		{
			foreach (string current in this.m_pMembers)
			{
				if (current.ToLower() == member.ToLower())
				{
					return true;
				}
			}
			return false;
		}

		private void Bind()
		{
			lock (this.m_pGroup.VirtualServer.Server.LockSynchronizer)
			{
				this.m_pGroup.VirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetGroupMembers " + this.m_pGroup.VirtualServer.VirtualServerID + " " + this.m_pGroup.GroupID);
				string text = this.m_pGroup.VirtualServer.Server.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
				int num = Convert.ToInt32(text.Split(new char[]
				{
					' '
				}, 2)[1]);
				MemoryStream memoryStream = new MemoryStream();
				this.m_pGroup.VirtualServer.Server.TCP_Client.TcpStream.ReadFixedCount(memoryStream, (long)num);
				DataSet dataSet = Utils.DecompressDataSet(memoryStream);
				if (dataSet.Tables.Contains("Members"))
				{
					foreach (DataRow dataRow in dataSet.Tables["Members"].Rows)
					{
						this.m_pMembers.Add(dataRow["Member"].ToString());
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
