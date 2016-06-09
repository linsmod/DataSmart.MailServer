using System;

namespace DataSmart.MailServer.Management
{
	public class StoreToFtp : GlobalMessageRuleActionBase
	{
		private string m_Server = "";

		private int m_Port = 21;

		private string m_UserName = "";

		private string m_Password = "";

		private string m_Folder = "";

		public override GlobalMessageRuleActionType ActionType
		{
			get
			{
				return GlobalMessageRuleActionType.StoreToFTPFolder;
			}
		}

		public bool HasChanges
		{
			get
			{
				return this.m_ValuesChanged;
			}
		}

		public string Server
		{
			get
			{
				return this.m_Server;
			}
			set
			{
				if (this.m_Server != value)
				{
					this.m_Server = value;
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

		public string Folder
		{
			get
			{
				return this.m_Folder;
			}
			set
			{
				if (this.m_Folder != value)
				{
					this.m_Folder = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		internal StoreToFtp(GlobalMessageRule rule, GlobalMessageRuleActionCollection owner, string id, string description, byte[] actionData) : base(rule, owner, id, description)
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Parse(actionData);
			this.m_Server = xmlTable.GetValue("Server");
			this.m_Port = Convert.ToInt32(xmlTable.GetValue("Port"));
			this.m_UserName = xmlTable.GetValue("User");
			this.m_Password = xmlTable.GetValue("Password");
			this.m_Folder = xmlTable.GetValue("Folder");
		}

		internal StoreToFtp(GlobalMessageRule rule, GlobalMessageRuleActionCollection owner, string id, string description, string host, int port, string userName, string password, string folder) : base(rule, owner, id, description)
		{
			this.m_Server = host;
			this.m_Port = port;
			this.m_UserName = userName;
			this.m_Password = password;
			this.m_Folder = folder;
		}

		internal override byte[] Serialize()
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Add("Server", this.m_Server);
			xmlTable.Add("Port", this.m_Port.ToString());
			xmlTable.Add("User", this.m_UserName);
			xmlTable.Add("Password", this.m_Password);
			xmlTable.Add("Folder", this.m_Folder);
			return xmlTable.ToByteData();
		}
	}
}
