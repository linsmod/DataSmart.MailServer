using System;

namespace DataSmart.MailServer.Management
{
	public class UserMessageRuleAction_ExecuteProgram : UserMessageRuleActionBase
	{
		private string m_Program = "";

		private string m_ProgramArgs = "";

		public bool HasChanges
		{
			get
			{
				return this.m_ValuesChanged;
			}
		}

		public string Program
		{
			get
			{
				return this.m_Program;
			}
			set
			{
				if (this.m_Program != value)
				{
					this.m_Program = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public string ProgramArguments
		{
			get
			{
				return this.m_ProgramArgs;
			}
			set
			{
				if (this.m_ProgramArgs != value)
				{
					this.m_ProgramArgs = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		internal UserMessageRuleAction_ExecuteProgram(UserMessageRule rule, UserMessageRuleActionCollection owner, string id, string description, byte[] actionData) : base(UserMessageRuleActionType.ExecuteProgram, rule, owner, id, description)
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Parse(actionData);
			this.m_Program = xmlTable.GetValue("Program");
			this.m_ProgramArgs = xmlTable.GetValue("Arguments");
		}

		internal UserMessageRuleAction_ExecuteProgram(UserMessageRule rule, UserMessageRuleActionCollection owner, string id, string description, string program, string programArgs) : base(UserMessageRuleActionType.ExecuteProgram, rule, owner, id, description)
		{
			this.m_Program = program;
			this.m_ProgramArgs = programArgs;
		}

		internal override byte[] Serialize()
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Add("Program", this.m_Program);
			xmlTable.Add("Arguments", this.m_ProgramArgs);
			return xmlTable.ToByteData();
		}
	}
}
