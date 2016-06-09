using System;

namespace DataSmart.MailServer.Management
{
	public class RemoveHeaderField : GlobalMessageRuleActionBase
	{
		private string m_HeaderFieldName = "";

		public override GlobalMessageRuleActionType ActionType
		{
			get
			{
				return GlobalMessageRuleActionType.RemoveHeaderField;
			}
		}

		public bool HasChanges
		{
			get
			{
				return this.m_ValuesChanged;
			}
		}

		public string HeaderFieldName
		{
			get
			{
				return this.m_HeaderFieldName;
			}
			set
			{
				if (this.m_HeaderFieldName != value)
				{
					this.m_HeaderFieldName = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		internal RemoveHeaderField(GlobalMessageRule rule, GlobalMessageRuleActionCollection owner, string id, string description, byte[] actionData) : base(rule, owner, id, description)
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Parse(actionData);
			this.m_HeaderFieldName = xmlTable.GetValue("HeaderFieldName");
		}

		internal RemoveHeaderField(GlobalMessageRule rule, GlobalMessageRuleActionCollection owner, string id, string description, string headerField) : base(rule, owner, id, description)
		{
			this.m_HeaderFieldName = headerField;
		}

		internal override byte[] Serialize()
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Add("HeaderFieldName", this.m_HeaderFieldName);
			return xmlTable.ToByteData();
		}
	}
}
