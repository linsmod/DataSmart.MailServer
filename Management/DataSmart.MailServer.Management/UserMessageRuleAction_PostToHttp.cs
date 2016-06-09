using System;

namespace DataSmart.MailServer.Management
{
	public class UserMessageRuleAction_PostToHttp : UserMessageRuleActionBase
	{
		private string m_Url = "";

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

		internal UserMessageRuleAction_PostToHttp(UserMessageRule rule, UserMessageRuleActionCollection owner, string id, string description, byte[] actionData) : base(UserMessageRuleActionType.PostToHTTP, rule, owner, id, description)
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Parse(actionData);
			this.m_Url = xmlTable.GetValue("URL");
		}

		internal UserMessageRuleAction_PostToHttp(UserMessageRule rule, UserMessageRuleActionCollection owner, string id, string description, string url) : base(UserMessageRuleActionType.PostToHTTP, rule, owner, id, description)
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
