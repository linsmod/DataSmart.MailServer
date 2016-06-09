using System.NetworkToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class UserMessageRuleCollection
	{
		private User m_pUser;

		private List<UserMessageRule> m_pRules;

		public VirtualServer VirtualServer
		{
			get
			{
				return this.m_pUser.VirtualServer;
			}
		}

		public User Owner
		{
			get
			{
				return this.m_pUser;
			}
		}

		public int Count
		{
			get
			{
				return this.m_pRules.Count;
			}
		}

		public UserMessageRule this[int index]
		{
			get
			{
				return this.m_pRules[index];
			}
		}

		public UserMessageRule this[string ruleID]
		{
			get
			{
				foreach (UserMessageRule current in this.m_pRules)
				{
					if (current.ID.ToLower() == ruleID.ToLower())
					{
						return current;
					}
				}
				throw new Exception("UserMessageRule with specified ID '" + ruleID + "' doesn't exist !");
			}
		}

		internal UserMessageRuleCollection(User user)
		{
			this.m_pUser = user;
			this.m_pRules = new List<UserMessageRule>();
			this.Bind();
		}

		public UserMessageRule Add(bool enabled, string description, string matchExpression, GlobalMessageRule_CheckNextRule checkNext)
		{
			string text = Guid.NewGuid().ToString();
			long ticks = DateTime.Now.Ticks;
			this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"AddUserMessageRule ",
				this.m_pUser.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pUser.UserID),
				" ",
				TextUtils.QuoteString(text),
				" ",
				ticks,
				" ",
				enabled,
				" ",
				TextUtils.QuoteString(description),
				" ",
				TextUtils.QuoteString(matchExpression.TrimEnd(new char[0])),
				" ",
				(int)checkNext
			}));
			string text2 = this.m_pUser.VirtualServer.Server.ReadLine();
			if (!text2.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text2);
			}
			UserMessageRule userMessageRule = new UserMessageRule(this, text, ticks, enabled, description, matchExpression, checkNext);
			this.m_pRules.Add(userMessageRule);
			return userMessageRule;
		}

		public void Remove(UserMessageRule rule)
		{
			Guid.NewGuid().ToString();
			this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
			{
				"DeleteUserMessageRule ",
				this.m_pUser.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pUser.UserID),
				" ",
				TextUtils.QuoteString(rule.ID)
			}));
			string text = this.m_pUser.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pRules.Remove(rule);
		}

		public void Refresh()
		{
			this.m_pRules.Clear();
			this.Bind();
		}

		public UserMessageRule[] ToArray()
		{
			return this.m_pRules.ToArray();
		}

		private void Bind()
		{
			lock (this.m_pUser.VirtualServer.Server.LockSynchronizer)
			{
				this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetUserMessageRules " + this.m_pUser.VirtualServer.VirtualServerID + " " + TextUtils.QuoteString(this.m_pUser.UserID));
				string text = this.m_pUser.VirtualServer.Server.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
				int num = Convert.ToInt32(text.Split(new char[]
				{
					' '
				}, 2)[1]);
				MemoryStream memoryStream = new MemoryStream();
				this.m_pUser.VirtualServer.Server.TCP_Client.TcpStream.ReadFixedCount(memoryStream, (long)num);
				DataSet dataSet = Utils.DecompressDataSet(memoryStream);
				if (dataSet.Tables.Contains("UserMessageRules"))
				{
					foreach (DataRow dataRow in dataSet.Tables["UserMessageRules"].Rows)
					{
						this.m_pRules.Add(new UserMessageRule(this, dataRow["RuleID"].ToString(), Convert.ToInt64(dataRow["Cost"]), Convert.ToBoolean(dataRow["Enabled"]), dataRow["Description"].ToString(), dataRow["MatchExpression"].ToString(), (GlobalMessageRule_CheckNextRule)Convert.ToInt32(dataRow["CheckNextRuleIf"])));
					}
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pRules.GetEnumerator();
		}
	}
}
