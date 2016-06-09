using System;

namespace DataSmart.MailServer.Management
{
	public class PostToNntpNewsgroup : GlobalMessageRuleActionBase
	{
		private string m_Server = "";

		private int m_Port = 119;

		private string m_Newsgroup = "";

		public override GlobalMessageRuleActionType ActionType
		{
			get
			{
				return GlobalMessageRuleActionType.PostToNNTPNewsGroup;
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

		public string Newsgroup
		{
			get
			{
				return this.m_Newsgroup;
			}
			set
			{
				if (this.m_Newsgroup != value)
				{
					this.m_Newsgroup = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		internal PostToNntpNewsgroup(GlobalMessageRule rule, GlobalMessageRuleActionCollection owner, string id, string description, byte[] actionData) : base(rule, owner, id, description)
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Parse(actionData);
			this.m_Server = xmlTable.GetValue("Server");
			this.m_Port = Convert.ToInt32(xmlTable.GetValue("Port"));
			this.m_Newsgroup = xmlTable.GetValue("Newsgroup");
		}

		internal PostToNntpNewsgroup(GlobalMessageRule rule, GlobalMessageRuleActionCollection owner, string id, string description, string host, int port, string newsgroup) : base(rule, owner, id, description)
		{
			this.m_Server = host;
			this.m_Port = port;
			this.m_Newsgroup = newsgroup;
		}

		internal override byte[] Serialize()
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Add("Server", this.m_Server);
			xmlTable.Add("Port", this.m_Port.ToString());
			xmlTable.Add("User", "");
			xmlTable.Add("Password", "");
			xmlTable.Add("Newsgroup", this.m_Newsgroup);
			return xmlTable.ToByteData();
		}
	}
}
