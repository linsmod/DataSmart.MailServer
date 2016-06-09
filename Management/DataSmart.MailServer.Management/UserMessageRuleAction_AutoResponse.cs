using System;
using System.Text;

namespace DataSmart.MailServer.Management
{
	public class UserMessageRuleAction_AutoResponse : UserMessageRuleActionBase
	{
		private string m_From = "";

		private byte[] m_Message;

		public bool HasChanges
		{
			get
			{
				return this.m_ValuesChanged;
			}
		}

		public string From
		{
			get
			{
				return this.m_From;
			}
			set
			{
				if (this.m_From != value)
				{
					this.m_From = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public byte[] Message
		{
			get
			{
				return this.m_Message;
			}
			set
			{
				if (this.m_Message != value)
				{
					this.m_Message = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		internal UserMessageRuleAction_AutoResponse(UserMessageRule rule, UserMessageRuleActionCollection owner, string id, string description, byte[] actionData) : base(UserMessageRuleActionType.AutoResponse, rule, owner, id, description)
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Parse(actionData);
			this.m_From = xmlTable.GetValue("From");
			this.m_Message = Encoding.UTF8.GetBytes(xmlTable.GetValue("Message"));
		}

		internal UserMessageRuleAction_AutoResponse(UserMessageRule rule, UserMessageRuleActionCollection owner, string id, string description, string from, byte[] message) : base(UserMessageRuleActionType.AutoResponse, rule, owner, id, description)
		{
			this.m_From = from;
			this.m_Message = message;
		}

		internal override byte[] Serialize()
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Add("From", this.m_From);
			xmlTable.Add("Message", Encoding.UTF8.GetString(this.m_Message));
			return xmlTable.ToByteData();
		}
	}
}
