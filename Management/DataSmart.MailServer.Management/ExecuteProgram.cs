using System;

namespace DataSmart.MailServer.Management
{
	public class ExecuteProgram : GlobalMessageRuleActionBase
	{
		private string m_Program = "";

		private string m_ProgramArgs = "";

		public override GlobalMessageRuleActionType ActionType
		{
			get
			{
				return GlobalMessageRuleActionType.ExecuteProgram;
			}
		}

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

		internal ExecuteProgram(GlobalMessageRule rule, GlobalMessageRuleActionCollection owner, string id, string description, byte[] actionData) : base(rule, owner, id, description)
		{
			XmlTable xmlTable = new XmlTable("ActionData");
			xmlTable.Parse(actionData);
			this.m_Program = xmlTable.GetValue("Program");
			this.m_ProgramArgs = xmlTable.GetValue("Arguments");
		}

		internal ExecuteProgram(GlobalMessageRule rule, GlobalMessageRuleActionCollection owner, string id, string description, string program, string programArgs) : base(rule, owner, id, description)
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
