using System;

namespace DataSmart.MailServer.Management
{
	public class DeleteMessage : GlobalMessageRuleActionBase
	{
		public override GlobalMessageRuleActionType ActionType
		{
			get
			{
				return GlobalMessageRuleActionType.DeleteMessage;
			}
		}

		internal DeleteMessage(GlobalMessageRule rule, GlobalMessageRuleActionCollection owner, string id, string description) : base(rule, owner, id, description)
		{
		}

		internal override byte[] Serialize()
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			return xmlTable.ToByteData();
		}
	}
}
