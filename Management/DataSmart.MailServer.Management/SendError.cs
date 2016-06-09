using System;

namespace DataSmart.MailServer.Management
{
	public class SendError : GlobalMessageRuleActionBase
	{
		private string m_ErrorText = "";

		public override GlobalMessageRuleActionType ActionType
		{
			get
			{
				return GlobalMessageRuleActionType.SendErrorToClient;
			}
		}

		public bool HasChanges
		{
			get
			{
				return this.m_ValuesChanged;
			}
		}

		public string SmtpErrorText
		{
			get
			{
				return this.m_ErrorText;
			}
			set
			{
				if (this.m_ErrorText != value)
				{
					this.m_ErrorText = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		internal SendError(GlobalMessageRule rule, GlobalMessageRuleActionCollection owner, string id, string description, byte[] actionData) : base(rule, owner, id, description)
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Parse(actionData);
			this.m_ErrorText = xmlTable.GetValue("ErrorText");
		}

		internal SendError(GlobalMessageRule rule, GlobalMessageRuleActionCollection owner, string id, string description, string errorText) : base(rule, owner, id, description)
		{
			this.m_ErrorText = errorText;
		}

		internal override byte[] Serialize()
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Add("ErrorText", this.m_ErrorText);
			return xmlTable.ToByteData();
		}
	}
}
