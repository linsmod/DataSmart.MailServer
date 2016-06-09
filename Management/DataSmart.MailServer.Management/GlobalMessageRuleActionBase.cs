using System.NetworkToolkit;
using System;

namespace DataSmart.MailServer.Management
{
	public abstract class GlobalMessageRuleActionBase
	{
		protected bool m_ValuesChanged;

		private GlobalMessageRule m_pRule;

		private GlobalMessageRuleActionCollection m_pOwner;

		private string m_ID = "";

		private string m_Description = "";

		public GlobalMessageRuleActionCollection Owner
		{
			get
			{
				return this.m_pOwner;
			}
		}

		public string ID
		{
			get
			{
				return this.m_ID;
			}
		}

		public virtual GlobalMessageRuleActionType ActionType
		{
			get
			{
				return GlobalMessageRuleActionType.AutoResponse;
			}
		}

		public string Description
		{
			get
			{
				return this.m_Description;
			}
			set
			{
				if (this.m_Description != value)
				{
					this.m_Description = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		internal GlobalMessageRuleActionBase(GlobalMessageRule rule, GlobalMessageRuleActionCollection owner, string id, string description)
		{
			this.m_pRule = rule;
			this.m_pOwner = owner;
			this.m_ID = id;
			this.m_Description = description;
		}

		public void Commit()
		{
			if (!this.m_ValuesChanged)
			{
				return;
			}
			this.m_pRule.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
			{
				"UpdateGlobalMessageRuleAction ",
				this.m_pRule.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pRule.ID),
				" ",
				TextUtils.QuoteString(this.m_ID),
				" ",
				TextUtils.QuoteString(this.m_Description),
				" ",
				((int)this.ActionType).ToString(),
				" ",
				Convert.ToBase64String(this.Serialize())
			}));
			string text = this.m_pRule.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_ValuesChanged = false;
		}

		internal virtual byte[] Serialize()
		{
			return new byte[0];
		}
	}
}
