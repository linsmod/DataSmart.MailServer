using System;

namespace DataSmart.MailServer.Management
{
	public class UserMessageRuleAction_ForwardToHost : UserMessageRuleActionBase
	{
		private string m_Host = "";

		private int m_Port = 25;

		public bool HasChanges
		{
			get
			{
				return this.m_ValuesChanged;
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

		internal UserMessageRuleAction_ForwardToHost(UserMessageRule rule, UserMessageRuleActionCollection owner, string id, string description, byte[] actionData) : base(UserMessageRuleActionType.ForwardToHost, rule, owner, id, description)
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Parse(actionData);
			this.m_Host = xmlTable.GetValue("Host");
			this.m_Port = Convert.ToInt32(xmlTable.GetValue("Port"));
		}

		internal UserMessageRuleAction_ForwardToHost(UserMessageRule rule, UserMessageRuleActionCollection owner, string id, string description, string host, int port) : base(UserMessageRuleActionType.ForwardToHost, rule, owner, id, description)
		{
			this.m_Host = host;
			this.m_Port = port;
		}

		internal override byte[] Serialize()
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Add("Host", this.m_Host);
			xmlTable.Add("Port", this.m_Port.ToString());
			return xmlTable.ToByteData();
		}
	}
}
