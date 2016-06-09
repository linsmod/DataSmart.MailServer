using System.NetworkToolkit;
using System;

namespace DataSmart.MailServer.Management
{
	public class User
	{
		private VirtualServer m_pVirtualServer;

		private UserCollection m_pOwner;

		private string m_UserID = "";

		private bool m_Enabled;

		private string m_UserName = "";

		private string m_Password = "";

		private string m_FullName = "";

		private string m_Description = "";

		private int m_MailboxSize;

		private UserPermissions m_Permissions;

		private UserEmailAddressCollection m_pEmailAddresses;

		private UserFolderCollection m_pFolders;

		private UserMessageRuleCollection m_pMessageRules;

		private UserRemoteServerCollection m_pRemoteServers;

		private DateTime m_CreationTime;

		private bool m_ValuesChanged;

		public VirtualServer VirtualServer
		{
			get
			{
				return this.m_pVirtualServer;
			}
		}

		public UserCollection Owner
		{
			get
			{
				return this.m_pOwner;
			}
		}

		public bool HasChanges
		{
			get
			{
				return this.m_ValuesChanged;
			}
		}

		public string UserID
		{
			get
			{
				return this.m_UserID;
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

		public string FullName
		{
			get
			{
				return this.m_FullName;
			}
			set
			{
				if (this.m_FullName != value)
				{
					this.m_FullName = value;
					this.m_ValuesChanged = true;
				}
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

		public int MaximumMailboxSize
		{
			get
			{
				return this.m_MailboxSize;
			}
			set
			{
				if (this.m_MailboxSize != value)
				{
					this.m_MailboxSize = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public long MailboxSize
		{
			get
			{
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetUserMailboxSize " + this.m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(this.m_UserID));
				string text = this.m_pVirtualServer.Server.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
				return Convert.ToInt64(text.Substring(4).Trim());
			}
		}

		public UserPermissions Permissions
		{
			get
			{
				return this.m_Permissions;
			}
			set
			{
				if (this.m_Permissions != value)
				{
					this.m_Permissions = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public UserEmailAddressCollection EmailAddresses
		{
			get
			{
				if (this.m_pEmailAddresses == null)
				{
					this.m_pEmailAddresses = new UserEmailAddressCollection(this);
				}
				return this.m_pEmailAddresses;
			}
		}

		public UserFolderCollection Folders
		{
			get
			{
				if (this.m_pFolders == null)
				{
					this.m_pFolders = new UserFolderCollection(true, null, this);
				}
				return this.m_pFolders;
			}
		}

		public UserMessageRuleCollection MessageRules
		{
			get
			{
				if (this.m_pMessageRules == null)
				{
					this.m_pMessageRules = new UserMessageRuleCollection(this);
				}
				return this.m_pMessageRules;
			}
		}

		public UserRemoteServerCollection RemoteServers
		{
			get
			{
				if (this.m_pRemoteServers == null)
				{
					this.m_pRemoteServers = new UserRemoteServerCollection(this);
				}
				return this.m_pRemoteServers;
			}
		}

		public DateTime CreationTime
		{
			get
			{
				return this.m_CreationTime;
			}
		}

		public DateTime LastLogin
		{
			get
			{
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetUserLastLoginTime " + this.m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(this.m_UserID));
				string text = this.m_pVirtualServer.Server.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
				return Convert.ToDateTime(text.Substring(4).Trim());
			}
		}

		internal User(VirtualServer virtualServer, UserCollection owner, string id, bool enabled, string userName, string password, string fullName, string description, int mailboxSize, UserPermissions permissions, DateTime creationTime)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pOwner = owner;
			this.m_UserID = id;
			this.m_Enabled = enabled;
			this.m_UserName = userName;
			this.m_Password = password;
			this.m_FullName = fullName;
			this.m_Description = description;
			this.m_MailboxSize = mailboxSize;
			this.m_Permissions = permissions;
			this.m_CreationTime = creationTime;
		}

		public void Commit()
		{
			if (!this.m_ValuesChanged)
			{
				return;
			}
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"UpdateUser ",
				this.m_pVirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_UserID),
				" ",
				TextUtils.QuoteString(this.m_UserName),
				" ",
				TextUtils.QuoteString(this.m_FullName),
				" ",
				TextUtils.QuoteString(this.m_Password),
				" ",
				TextUtils.QuoteString(this.m_Description),
				" ",
				this.m_MailboxSize,
				" ",
				this.m_Enabled,
				" ",
				(int)this.m_Permissions
			}));
			string text = this.m_pVirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
		}
	}
}
