using System.NetworkToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class UserCollection : IEnumerable
	{
		private VirtualServer m_pVirtualServer;

		private List<User> m_pUsers;

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
				return this.m_pUsers.Count;
			}
		}

		public User this[int index]
		{
			get
			{
				return this.m_pUsers[index];
			}
		}

		public User this[string userID]
		{
			get
			{
				foreach (User current in this.m_pUsers)
				{
					if (current.UserID.ToLower() == userID.ToLower())
					{
						return current;
					}
				}
				throw new Exception("User with specified ID '" + userID + "' doesn't exist !");
			}
		}

		internal UserCollection(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pUsers = new List<User>();
			this.Bind();
		}

		public User Add(string userName, string fullName, string password, string description, int mailboxSize, bool enabled, UserPermissions permissions)
		{
			string text = Guid.NewGuid().ToString();
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"AddUser ",
				this.m_pVirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(text),
				" ",
				TextUtils.QuoteString(userName),
				" ",
				TextUtils.QuoteString(fullName),
				" ",
				TextUtils.QuoteString(password),
				" ",
				TextUtils.QuoteString(description),
				" ",
				mailboxSize,
				" ",
				enabled,
				" ",
				(int)permissions
			}));
			string text2 = this.m_pVirtualServer.Server.ReadLine();
			if (!text2.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text2);
			}
			User user = new User(this.m_pVirtualServer, this, text, enabled, userName, password, fullName, description, mailboxSize, permissions, DateTime.Now);
			this.m_pUsers.Add(user);
			return user;
		}

		public void Remove(User user)
		{
			Guid.NewGuid().ToString();
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("DeleteUser " + this.m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(user.UserID));
			string text = this.m_pVirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pUsers.Remove(user);
		}

		public bool Contains(string userName)
		{
			foreach (User current in this.m_pUsers)
			{
				if (current.UserName.ToLower() == userName.ToLower())
				{
					return true;
				}
			}
			return false;
		}

		public User GetUserByName(string userName)
		{
			foreach (User current in this.m_pUsers)
			{
				if (current.UserName.ToLower() == userName.ToLower())
				{
					return current;
				}
			}
			throw new Exception("User with specified User Name '" + userName + "' doesn't exist !");
		}

		public void Refresh()
		{
			this.m_pUsers.Clear();
			this.Bind();
		}

		private void Bind()
		{
			lock (this.m_pVirtualServer.Server.LockSynchronizer)
			{
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetUsers " + this.m_pVirtualServer.VirtualServerID);
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
				if (dataSet.Tables.Contains("Users"))
				{
					foreach (DataRow dataRow in dataSet.Tables["Users"].Rows)
					{
						this.m_pUsers.Add(new User(this.m_pVirtualServer, this, dataRow["UserID"].ToString(), Convert.ToBoolean(dataRow["Enabled"]), dataRow["UserName"].ToString(), dataRow["Password"].ToString(), dataRow["FullName"].ToString(), dataRow["Description"].ToString(), Convert.ToInt32(dataRow["Mailbox_Size"]), (UserPermissions)Convert.ToInt32(dataRow["Permissions"]), Convert.ToDateTime(dataRow["CreationTime"])));
					}
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pUsers.GetEnumerator();
		}
	}
}
