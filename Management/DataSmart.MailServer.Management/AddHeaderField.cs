using System;

namespace DataSmart.MailServer.Management
{
	public class AddHeaderField : GlobalMessageRuleActionBase
	{
		private string m_HeaderFieldName = "";

		private string m_HeaderFieldValue = "";

		public override GlobalMessageRuleActionType ActionType
		{
			get
			{
				return GlobalMessageRuleActionType.AddHeaderField;
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

		public string HeaderFieldValue
		{
			get
			{
				return this.m_HeaderFieldValue;
			}
			set
			{
				if (this.m_HeaderFieldValue != value)
				{
					this.m_HeaderFieldValue = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		internal AddHeaderField(GlobalMessageRule rule, GlobalMessageRuleActionCollection owner, string id, string description, byte[] actionData) : base(rule, owner, id, description)
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Parse(actionData);
			this.m_HeaderFieldName = xmlTable.GetValue("HeaderFieldName");
			this.m_HeaderFieldValue = xmlTable.GetValue("HeaderFieldValue");
		}

		internal AddHeaderField(GlobalMessageRule rule, GlobalMessageRuleActionCollection owner, string id, string description, string headerFieldName, string headerFieldValue) : base(rule, owner, id, description)
		{
			this.m_HeaderFieldName = headerFieldName;
			this.m_HeaderFieldValue = headerFieldValue;
		}

		internal override byte[] Serialize()
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Add("HeaderFieldName", this.m_HeaderFieldName);
			xmlTable.Add("HeaderFieldValue", this.m_HeaderFieldValue);
			return xmlTable.ToByteData();
		}
	}
}
