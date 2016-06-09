using System.NetworkToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class GlobalMessageRuleCollection : IEnumerable
	{
		private VirtualServer m_pVirtualServer;

		private List<GlobalMessageRule> m_pGlobalMessageRules;

		public VirtualServer VirtualServer
		{
			get
			{
				return this.m_pVirtualServer;
			}
		}

		public int Count
		{
			get
			{
				return this.m_pGlobalMessageRules.Count;
			}
		}

		public GlobalMessageRule this[int index]
		{
			get
			{
				return this.m_pGlobalMessageRules[index];
			}
		}

		public GlobalMessageRule this[string globalMessageRuleID]
		{
			get
			{
				foreach (GlobalMessageRule current in this.m_pGlobalMessageRules)
				{
					if (current.ID.ToLower() == globalMessageRuleID.ToLower())
					{
						return current;
					}
				}
				throw new Exception("GlobalMessageRule with specified ID '" + globalMessageRuleID + "' doesn't exist !");
			}
		}

		internal GlobalMessageRuleCollection(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pGlobalMessageRules = new List<GlobalMessageRule>();
			this.Bind();
		}

		public GlobalMessageRule Add(bool enabled, string description, string matchExpression, GlobalMessageRule_CheckNextRule checkNext)
		{
			string text = Guid.NewGuid().ToString();
			long ticks = DateTime.Now.Ticks;
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"AddGlobalMessageRule ",
				this.m_pVirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(text),
				" ",
				ticks,
				" ",
				enabled,
				" ",
				TextUtils.QuoteString(description),
				" ",
				TextUtils.QuoteString(matchExpression),
				" ",
				(int)checkNext
			}));
			string text2 = this.m_pVirtualServer.Server.ReadLine();
			if (!text2.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text2);
			}
			GlobalMessageRule globalMessageRule = new GlobalMessageRule(this.m_pVirtualServer, this, text, ticks, enabled, description, matchExpression, checkNext);
			this.m_pGlobalMessageRules.Add(globalMessageRule);
			return globalMessageRule;
		}

		public void Remove(GlobalMessageRule rule)
		{
			Guid.NewGuid().ToString();
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("DeleteGlobalMessageRule " + this.m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(rule.ID));
			string text = this.m_pVirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pGlobalMessageRules.Remove(rule);
		}

		public void Refresh()
		{
			this.m_pGlobalMessageRules.Clear();
			this.Bind();
		}

		private void Bind()
		{
			lock (this.m_pVirtualServer.Server.LockSynchronizer)
			{
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetGlobalMessageRules " + this.m_pVirtualServer.VirtualServerID);
				string text = this.m_pVirtualServer.Server.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
				int num = Convert.ToInt32(text.Split(new char[]
				{
					' '
				}, 2)[1]);
				MemoryStream memoryStream = new MemoryStream();
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.ReadFixedCount(memoryStream, (long)num);
				DataSet dataSet = Utils.DecompressDataSet(memoryStream);
				if (dataSet.Tables.Contains("GlobalMessageRules"))
				{
					foreach (DataRow dataRow in dataSet.Tables["GlobalMessageRules"].Rows)
					{
						this.m_pGlobalMessageRules.Add(new GlobalMessageRule(this.m_pVirtualServer, this, dataRow["RuleID"].ToString(), Convert.ToInt64(dataRow["Cost"]), Convert.ToBoolean(dataRow["Enabled"]), dataRow["Description"].ToString(), dataRow["MatchExpression"].ToString(), (GlobalMessageRule_CheckNextRule)Convert.ToInt32(dataRow["CheckNextRuleIf"])));
					}
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pGlobalMessageRules.GetEnumerator();
		}
	}
}
