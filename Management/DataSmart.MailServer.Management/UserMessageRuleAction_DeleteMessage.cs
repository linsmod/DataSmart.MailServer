using System;

namespace DataSmart.MailServer.Management
{
	public class UserMessageRuleAction_DeleteMessage : UserMessageRuleActionBase
	{
		internal UserMessageRuleAction_DeleteMessage(UserMessageRule rule, UserMessageRuleActionCollection owner, string id, string description) : base(UserMessageRuleActionType.DeleteMessage, rule, owner, id, description)
		{
		}

		internal override byte[] Serialize()
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			return xmlTable.ToByteData();
		}
	}
}
