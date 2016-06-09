using System;

namespace DataSmart.MailServer.Management
{
	public class UserMessageRuleAction_ForwardToEmail : UserMessageRuleActionBase
	{
		private string m_EmailAddress = "";

		public bool HasChanges
		{
			get
			{
				return this.m_ValuesChanged;
			}
		}

		public string EmailAddress
		{
			get
			{
				return this.m_EmailAddress;
			}
			set
			{
				if (this.m_EmailAddress != value)
				{
					this.m_EmailAddress = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		internal UserMessageRuleAction_ForwardToEmail(UserMessageRule rule, UserMessageRuleActionCollection owner, string id, string description, byte[] actionData) : base(UserMessageRuleActionType.ForwardToEmail, rule, owner, id, description)
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Parse(actionData);
			this.m_EmailAddress = xmlTable.GetValue("Email");
		}

		internal UserMessageRuleAction_ForwardToEmail(UserMessageRule rule, UserMessageRuleActionCollection owner, string id, string description, string email) : base(UserMessageRuleActionType.ForwardToEmail, rule, owner, id, description)
		{
			this.m_EmailAddress = email;
		}

		internal override byte[] Serialize()
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Add("Email", this.m_EmailAddress);
			return xmlTable.ToByteData();
		}
	}
}
