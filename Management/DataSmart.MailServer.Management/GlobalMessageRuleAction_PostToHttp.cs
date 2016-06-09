using System;

namespace DataSmart.MailServer.Management
{
	public class GlobalMessageRuleAction_PostToHttp : GlobalMessageRuleActionBase
	{
		private string m_Url = "";

		public override GlobalMessageRuleActionType ActionType
		{
			get
			{
				return GlobalMessageRuleActionType.PostToHTTP;
			}
		}

		public bool HasChanges
		{
			get
			{
				return this.m_ValuesChanged;
			}
		}

		public string Url
		{
			get
			{
				return this.m_Url;
			}
			set
			{
				if (this.m_Url != value)
				{
					this.m_Url = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		internal GlobalMessageRuleAction_PostToHttp(GlobalMessageRule rule, GlobalMessageRuleActionCollection owner, string id, string description, byte[] actionData) : base(rule, owner, id, description)
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Parse(actionData);
			this.m_Url = xmlTable.GetValue("URL");
		}

		internal GlobalMessageRuleAction_PostToHttp(GlobalMessageRule rule, GlobalMessageRuleActionCollection owner, string id, string description, string url) : base(rule, owner, id, description)
		{
			this.m_Url = url;
		}

		internal override byte[] Serialize()
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Add("URL", this.m_Url);
			xmlTable.Add("FileName", "");
			return xmlTable.ToByteData();
		}
	}
}
