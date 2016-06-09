using System.NetworkToolkit;
using System;

namespace DataSmart.MailServer.Management
{
	public class Route
	{
		private RouteCollection m_pOwner;

		private string m_ID = "";

		private long m_Cost;

		private string m_Description = "";

		private string m_Pattern = "";

		private bool m_Enabled;

		private RouteActionBase m_pAction;

		private bool m_ValuesChanged;

		public RouteCollection Owner
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

		public string Pattern
		{
			get
			{
				return this.m_Pattern;
			}
			set
			{
				if (this.m_Pattern != value)
				{
					this.m_Pattern = value;
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

		public RouteActionBase Action
		{
			get
			{
				return this.m_pAction;
			}
			set
			{
				if (value == null)
				{
					throw new NullReferenceException("Action value can't be null !");
				}
				bool flag = false;
				if (this.m_pAction.ActionType != value.ActionType)
				{
					flag = true;
				}
				else if (value.ActionType == RouteAction.RouteToEmail)
				{
					if (((RouteAction_RouteToEmail)this.m_pAction).EmailAddress != ((RouteAction_RouteToEmail)value).EmailAddress)
					{
						flag = true;
					}
				}
				else if (value.ActionType == RouteAction.RouteToHost)
				{
					if (((RouteAction_RouteToHost)this.m_pAction).Host != ((RouteAction_RouteToHost)value).Host)
					{
						flag = true;
					}
					if (((RouteAction_RouteToHost)this.m_pAction).Port != ((RouteAction_RouteToHost)value).Port)
					{
						flag = true;
					}
				}
				else if (value.ActionType == RouteAction.RouteToMailbox && ((RouteAction_RouteToMailbox)this.m_pAction).Mailbox != ((RouteAction_RouteToMailbox)value).Mailbox)
				{
					flag = true;
				}
				if (flag)
				{
					this.m_pAction = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		internal Route(RouteCollection owner, string id, long cost, string descritpion, string pattern, bool enabled, RouteActionBase action)
		{
			this.m_pOwner = owner;
			this.m_ID = id;
			this.m_Cost = cost;
			this.m_Description = descritpion;
			this.m_Pattern = pattern;
			this.m_Enabled = enabled;
			this.m_pAction = action;
		}

		public void Commit()
		{
			if (!this.m_ValuesChanged)
			{
				return;
			}
			this.m_pOwner.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"UpdateRoute ",
				this.m_pOwner.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_ID),
				" ",
				this.m_Cost,
				" ",
				TextUtils.QuoteString(this.m_Description),
				" ",
				TextUtils.QuoteString(this.m_Pattern),
				" ",
				this.m_Enabled,
				" ",
				(int)this.m_pAction.ActionType,
				" ",
				Convert.ToBase64String(this.m_pAction.Serialize())
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
