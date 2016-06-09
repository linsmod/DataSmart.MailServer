using System.NetworkToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class UserRemoteServerCollection : IEnumerable
	{
		private User m_pUser;

		private List<UserRemoteServer> m_pServers;

		public int Count
		{
			get
			{
				return this.m_pServers.Count;
			}
		}

		public UserRemoteServer this[int index]
		{
			get
			{
				return this.m_pServers[index];
			}
		}

		public UserRemoteServer this[string remoteServerID]
		{
			get
			{
				foreach (UserRemoteServer current in this.m_pServers)
				{
					if (current.ID.ToLower() == remoteServerID.ToLower())
					{
						return current;
					}
				}
				throw new Exception("Remote server '" + remoteServerID + "' doesn't exist !");
			}
		}

		internal UserRemoteServerCollection(User user)
		{
			this.m_pUser = user;
			this.m_pServers = new List<UserRemoteServer>();
			this.Bind();
		}

		public UserRemoteServer Add(string description, string host, int port, bool ssl, string userName, string password, bool enabled)
		{
			string text = Guid.NewGuid().ToString();
			this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"AddUserRemoteServer ",
				this.m_pUser.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(text),
				" ",
				TextUtils.QuoteString(this.m_pUser.UserName),
				" ",
				TextUtils.QuoteString(description),
				" ",
				TextUtils.QuoteString(host),
				" ",
				port,
				" ",
				TextUtils.QuoteString(userName),
				" ",
				TextUtils.QuoteString(password),
				" ",
				ssl,
				" ",
				enabled
			}));
			string text2 = this.m_pUser.VirtualServer.Server.ReadLine();
			if (!text2.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text2);
			}
			UserRemoteServer userRemoteServer = new UserRemoteServer(this.m_pUser, this, text, description, host, port, ssl, userName, password, enabled);
			this.m_pServers.Add(userRemoteServer);
			return userRemoteServer;
		}

		public void Remove(UserRemoteServer remoteServer)
		{
			Guid.NewGuid().ToString();
			this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.WriteLine("DeleteUserRemoteServer " + this.m_pUser.VirtualServer.VirtualServerID + " " + TextUtils.QuoteString(remoteServer.ID));
			string text = this.m_pUser.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pServers.Remove(remoteServer);
		}

		public UserRemoteServer[] ToArray()
		{
			return this.m_pServers.ToArray();
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pServers.GetEnumerator();
		}

		private void Bind()
		{
			lock (this.m_pUser.VirtualServer.Server.LockSynchronizer)
			{
				this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetUserRemoteServers " + this.m_pUser.VirtualServer.VirtualServerID + " " + this.m_pUser.UserID);
				string text = this.m_pUser.VirtualServer.Server.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
				int num = Convert.ToInt32(text.Split(new char[]
				{
					' '
				}, 2)[1]);
				MemoryStream memoryStream = new MemoryStream();
				this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.ReadFixedCount(memoryStream, (long)num);
				DataSet dataSet = Utils.DecompressDataSet(memoryStream);
				if (dataSet.Tables.Contains("UserRemoteServers"))
				{
					foreach (DataRow dataRow in dataSet.Tables["UserRemoteServers"].Rows)
					{
						this.m_pServers.Add(new UserRemoteServer(this.m_pUser, this, dataRow["ServerID"].ToString(), dataRow["Description"].ToString(), dataRow["RemoteServer"].ToString(), Convert.ToInt32(dataRow["RemotePort"]), Convert.ToBoolean(dataRow["UseSSL"]), dataRow["RemoteUserName"].ToString(), dataRow["RemotePassword"].ToString(), Convert.ToBoolean(dataRow["Enabled"])));
					}
				}
			}
		}
	}
}
