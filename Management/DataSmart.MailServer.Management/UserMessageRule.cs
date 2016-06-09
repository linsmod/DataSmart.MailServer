using System.NetworkToolkit;
using System;

namespace DataSmart.MailServer.Management
{
	public class UserMessageRule
	{
		private UserMessageRuleCollection m_pOwner;

		private string m_ID = "";

		private long m_Cost;

		private bool m_Enabled;

		private string m_Description = "";

		private string m_MatchExpression = "";

		private GlobalMessageRule_CheckNextRule m_CheckNext;

		private UserMessageRuleActionCollection m_pActions;

		private bool m_ValuesChanged;

		public UserMessageRuleCollection Owner
		{
			get
			{
				return this.m_pOwner;
			}
		}

		public bool HasChanges
		{
			get
			{
				return this.m_ValuesChanged;
			}
		}

		public string ID
		{
			get
			{
				return this.m_ID;
			}
		}

		public long Cost
		{
			get
			{
				return this.m_Cost;
			}
			set
			{
				if (this.m_Cost != value)
				{
					this.m_Cost = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public bool Enabled
		{
			get
			{
				return this.m_Enabled;
			}
			set
			{
				if (this.m_Enabled != value)
				{
					this.m_Enabled = value;
					this.m_ValuesChanged = true;
				}
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

		public string MatchExpression
		{
			get
			{
				return this.m_MatchExpression;
			}
			set
			{
				if (this.m_MatchExpression != value)
				{
					this.m_MatchExpression = value.TrimEnd(new char[0]);
					this.m_ValuesChanged = true;
				}
			}
		}

		public GlobalMessageRule_CheckNextRule CheckNextRule
		{
			get
			{
				return this.m_CheckNext;
			}
			set
			{
				if (this.m_CheckNext != value)
				{
					this.m_CheckNext = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public UserMessageRuleActionCollection Actions
		{
			get
			{
				if (this.m_pActions == null)
				{
					this.m_pActions = new UserMessageRuleActionCollection(this);
				}
				return this.m_pActions;
			}
		}

		internal UserMessageRule(UserMessageRuleCollection owner, string id, long cost, bool enabled, string description, string matchexpression, GlobalMessageRule_CheckNextRule checkNext)
		{
			this.m_pOwner = owner;
			this.m_ID = id;
			this.m_Cost = cost;
			this.m_Enabled = enabled;
			this.m_Description = description;
			this.m_MatchExpression = matchexpression;
			this.m_CheckNext = checkNext;
		}

		public void Commit()
		{
			if (!this.m_ValuesChanged)
			{
				return;
			}
			this.m_pOwner.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"UpdateUserMessageRule ",
				this.m_pOwner.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pOwner.Owner.UserID),
				" ",
				TextUtils.QuoteString(this.m_ID),
				" ",
				this.m_Cost,
				" ",
				this.m_Enabled,
				" ",
				TextUtils.QuoteString(this.m_Description),
				" ",
				TextUtils.QuoteString(this.m_MatchExpression),
				" ",
				(int)this.m_CheckNext
			}));
			string text = this.m_pOwner.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_ValuesChanged = false;
		}
	}
}
