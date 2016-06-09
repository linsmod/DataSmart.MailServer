using System.NetworkToolkit;
using System;

namespace DataSmart.MailServer.Management
{
	public class SmtpSettings
	{
		private SystemSettings m_pSysSettings;

		private bool m_Enabled;

		private string m_GreetingText = "";

		private string m_DefaultDomain = "";

		private int m_SessionIdleTimeOut;

		private int m_MaxConnections;

		private int m_MaxConnsPerIP;

		private int m_MaxBadCommnads;

		private int m_MaxRecipientPerMsg;

		private int m_MaxMessageSize;

		private int m_MaxTransactions;

		private bool m_RequireAuth;

		private IPBindInfo[] m_pBinds;

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
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public string GreetingText
		{
			get
			{
				return this.m_GreetingText;
			}
			set
			{
				if (this.m_GreetingText != value)
				{
					this.m_GreetingText = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public string DefaultDomain
		{
			get
			{
				return this.m_DefaultDomain;
			}
			set
			{
				if (this.m_DefaultDomain != value)
				{
					this.m_DefaultDomain = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public int SessionIdleTimeOut
		{
			get
			{
				return this.m_SessionIdleTimeOut;
			}
			set
			{
				if (this.m_SessionIdleTimeOut != value)
				{
					this.m_SessionIdleTimeOut = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public int MaximumConnections
		{
			get
			{
				return this.m_MaxConnections;
			}
			set
			{
				if (this.m_MaxConnections != value)
				{
					this.m_MaxConnections = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public int MaximumConnectionsPerIP
		{
			get
			{
				return this.m_MaxConnsPerIP;
			}
			set
			{
				if (this.m_MaxConnsPerIP != value)
				{
					this.m_MaxConnsPerIP = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public int MaximumBadCommands
		{
			get
			{
				return this.m_MaxBadCommnads;
			}
			set
			{
				if (this.m_MaxBadCommnads != value)
				{
					this.m_MaxBadCommnads = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public int MaximumRecipientsPerMessage
		{
			get
			{
				return this.m_MaxRecipientPerMsg;
			}
			set
			{
				if (this.m_MaxRecipientPerMsg != value)
				{
					this.m_MaxRecipientPerMsg = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public int MaximumMessageSize
		{
			get
			{
				return this.m_MaxMessageSize;
			}
			set
			{
				if (this.m_MaxMessageSize != value)
				{
					this.m_MaxMessageSize = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public int MaximumTransactions
		{
			get
			{
				return this.m_MaxTransactions;
			}
			set
			{
				if (this.m_MaxTransactions != value)
				{
					this.m_MaxTransactions = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public bool RequireAuthentication
		{
			get
			{
				return this.m_RequireAuth;
			}
			set
			{
				if (this.m_RequireAuth != value)
				{
					this.m_RequireAuth = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public IPBindInfo[] Binds
		{
			get
			{
				return this.m_pBinds;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Binds");
				}
				if (!Net_Utils.CompareArray(this.m_pBinds, value))
				{
					this.m_pBinds = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		internal SmtpSettings(SystemSettings sysSettings, bool enabled, string greeting, string defaultDomain, int idleTimeOut, int maxConnections, int maxConnectionsPerIP, int maxBadCommands, int maxRecipients, int maxMessageSize, int maxTransactions, bool requireAuth, IPBindInfo[] bindings)
		{
			this.m_pSysSettings = sysSettings;
			this.m_Enabled = enabled;
			this.m_GreetingText = greeting;
			this.m_DefaultDomain = defaultDomain;
			this.m_SessionIdleTimeOut = idleTimeOut;
			this.m_MaxConnections = maxConnections;
			this.m_MaxConnsPerIP = maxConnectionsPerIP;
			this.m_MaxBadCommnads = maxBadCommands;
			this.m_MaxRecipientPerMsg = maxRecipients;
			this.m_MaxMessageSize = maxMessageSize;
			this.m_MaxTransactions = maxTransactions;
			this.m_RequireAuth = requireAuth;
			this.m_pBinds = bindings;
		}
	}
}
