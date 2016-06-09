using System;

namespace DataSmart.MailServer.Management
{
	public class StoreToDiskFolder : GlobalMessageRuleActionBase
	{
		private string m_Folder = "";

		public override GlobalMessageRuleActionType ActionType
		{
			get
			{
				return GlobalMessageRuleActionType.StoreToDiskFolder;
			}
		}

		public bool HasChanges
		{
			get
			{
				return this.m_ValuesChanged;
			}
		}

		public string Folder
		{
			get
			{
				return this.m_Folder;
			}
			set
			{
				if (this.m_Folder != value)
				{
					this.m_Folder = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		internal StoreToDiskFolder(GlobalMessageRule rule, GlobalMessageRuleActionCollection owner, string id, string description, byte[] actionData) : base(rule, owner, id, description)
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Parse(actionData);
			this.m_Folder = xmlTable.GetValue("Folder");
		}

		internal StoreToDiskFolder(GlobalMessageRule rule, GlobalMessageRuleActionCollection owner, string id, string description, string folder) : base(rule, owner, id, description)
		{
			this.m_Folder = folder;
		}

		internal override byte[] Serialize()
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Add("Folder", this.m_Folder);
			return xmlTable.ToByteData();
		}
	}
}
