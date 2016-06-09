using System.NetworkToolkit;
using System;

namespace DataSmart.MailServer.Management
{
	public class UserRemoteServer
	{
		private User m_pUser;

		private UserRemoteServerCollection m_pOwner;

		private string m_ID = "";

		private string m_Description = "";

		private string m_Host = "";

		private int m_Port = 110;

		private bool m_SSL;

		private string m_UserName = "";

		private string m_Password = "";

		private bool m_Enabled = true;

		private bool m_ValuesChanged;

		public UserRemoteServerCollection Owner
		{
			get
			{
				return this.m_pOwner;
			}
		}

		public string ID
		{
			get
			{
				return this.m_ID;
			}
		}

		public string Description
		{
			get
			{
				return this.m_Description;
			}
			set
			{
				if (this.m_Description != value)
				{
					this.m_Description = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public string Host
		{
			get
			{
				return this.m_Host;
			}
			set
			{
				if (this.m_Host != value)
				{
					this.m_Host = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public int Port
		{
			get
			{
				return this.m_Port;
			}
			set
			{
				if (this.m_Port != value)
				{
					this.m_Port = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public bool SSL
		{
			get
			{
				return this.m_SSL;
			}
			set
			{
				if (this.m_SSL != value)
				{
					this.m_SSL = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public string UserName
		{
			get
			{
				return this.m_UserName;
			}
			set
			{
				if (this.m_UserName != value)
				{
					this.m_UserName = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public string Password
		{
			get
			{
				return this.m_Password;
			}
			set
			{
				if (this.m_Password != value)
				{
					this.m_Password = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public bool Enabled
		{
			get
			{
				return this.m_Enabled;
			}
			set
			{
				if (this.m_Enabled != value)
				{
					this.m_Enabled = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		internal UserRemoteServer(User ownerUser, UserRemoteServerCollection owner, string id, string description, string host, int port, bool ssl, string userName, string password, bool enabled)
		{
			this.m_pUser = ownerUser;
			this.m_pOwner = owner;
			this.m_ID = id;
			this.m_Description = description;
			this.m_Host = host;
			this.m_Port = port;
			this.m_SSL = ssl;
			this.m_UserName = userName;
			this.m_Password = password;
			this.m_Enabled = enabled;
		}

		public void Commit()
		{
			if (!this.m_ValuesChanged)
			{
				return;
			}
			this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"UpdateUserRemoteServer ",
				this.m_pUser.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_ID),
				" ",
				TextUtils.QuoteString(this.m_pUser.UserName),
				" ",
				TextUtils.QuoteString(this.m_Description),
				" ",
				TextUtils.QuoteString(this.m_Host),
				" ",
				this.m_Port,
				" ",
				TextUtils.QuoteString(this.m_UserName),
				" ",
				TextUtils.QuoteString(this.m_Password),
				" ",
				this.m_SSL,
				" ",
				this.m_Enabled
			}));
			string text = this.m_pUser.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_ValuesChanged = false;
		}
	}
}
