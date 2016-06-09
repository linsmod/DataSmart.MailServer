using System.NetworkToolkit;
using System.NetworkToolkit.SMTP.Relay;
using System;

namespace DataSmart.MailServer.Management
{
	public class Relay_Settings
	{
		private SystemSettings m_pSysSettings;

		private Relay_Mode m_RelayMode;

		private BalanceMode m_SmartHostsBalanceMode;

		private Relay_SmartHost[] m_pSmartHosts;

		private int m_IdleTimeout;

		private int m_MaxConnections;

		private int m_MaxConnectionsPerIP;

		private int m_RelayInterval;

		private int m_RelayRetryInterval;

		private int m_SendUndelWaringAfter;

		private int m_SendUndeliveredAfter;

		private bool m_StoreUndeliveredMsgs;

		private bool m_UseTlsIfPossible;

		private IPBindInfo[] m_pBinds;

		public Relay_Mode RelayMode
		{
			get
			{
				return this.m_RelayMode;
			}
			set
			{
				if (this.m_RelayMode != value)
				{
					this.m_RelayMode = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public BalanceMode SmartHostsBalanceMode
		{
			get
			{
				return this.m_SmartHostsBalanceMode;
			}
			set
			{
				if (this.m_SmartHostsBalanceMode != value)
				{
					this.m_SmartHostsBalanceMode = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public Relay_SmartHost[] SmartHosts
		{
			get
			{
				return this.m_pSmartHosts;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("SmartHosts");
				}
				bool flag = false;
				if (value.Length != this.m_pSmartHosts.Length)
				{
					flag = true;
				}
				else
				{
					for (int i = 0; i < this.m_pSmartHosts.Length; i++)
					{
						if (!value[i].Equals(this.m_pSmartHosts[i]))
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					this.m_pSmartHosts = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public int SessionIdleTimeOut
		{
			get
			{
				return this.m_IdleTimeout;
			}
			set
			{
				if (this.m_IdleTimeout != value)
				{
					this.m_IdleTimeout = value;
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
				return this.m_MaxConnectionsPerIP;
			}
			set
			{
				if (this.m_MaxConnectionsPerIP != value)
				{
					this.m_MaxConnectionsPerIP = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public int RelayInterval
		{
			get
			{
				return this.m_RelayInterval;
			}
			set
			{
				if (this.m_RelayInterval != value)
				{
					this.m_RelayInterval = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public int RelayRetryInterval
		{
			get
			{
				return this.m_RelayRetryInterval;
			}
			set
			{
				if (this.m_RelayRetryInterval != value)
				{
					this.m_RelayRetryInterval = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public int SendUndeliveredWarningAfter
		{
			get
			{
				return this.m_SendUndelWaringAfter;
			}
			set
			{
				if (this.m_SendUndelWaringAfter != value)
				{
					this.m_SendUndelWaringAfter = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public int SendUndeliveredAfter
		{
			get
			{
				return this.m_SendUndeliveredAfter;
			}
			set
			{
				if (this.m_SendUndeliveredAfter != value)
				{
					this.m_SendUndeliveredAfter = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public bool StoreUndeliveredMessages
		{
			get
			{
				return this.m_StoreUndeliveredMsgs;
			}
			set
			{
				if (this.m_StoreUndeliveredMsgs != value)
				{
					this.m_StoreUndeliveredMsgs = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public bool UseTlsIfPossible
		{
			get
			{
				return this.m_UseTlsIfPossible;
			}
			set
			{
				if (this.m_UseTlsIfPossible != value)
				{
					this.m_UseTlsIfPossible = value;
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

		internal Relay_Settings(SystemSettings sysSettings, Relay_Mode relayMode, BalanceMode smartHostBalanceMode, Relay_SmartHost[] smartHosts, int idleTimeout, int maxConnections, int maxConnectionsPerIP, int relayInterval, int relayRetryInterval, int undeliveredWarning, int undelivered, bool storeUndelivered, bool useTlsIfPossible, IPBindInfo[] bindings)
		{
			this.m_pSysSettings = sysSettings;
			this.m_RelayMode = relayMode;
			this.m_SmartHostsBalanceMode = smartHostBalanceMode;
			this.m_pSmartHosts = smartHosts;
			this.m_IdleTimeout = idleTimeout;
			this.m_MaxConnections = maxConnections;
			this.m_MaxConnectionsPerIP = maxConnectionsPerIP;
			this.m_RelayInterval = relayInterval;
			this.m_RelayRetryInterval = relayRetryInterval;
			this.m_SendUndelWaringAfter = undeliveredWarning;
			this.m_SendUndeliveredAfter = undelivered;
			this.m_StoreUndeliveredMsgs = storeUndelivered;
			this.m_UseTlsIfPossible = useTlsIfPossible;
			this.m_pBinds = bindings;
		}
	}
}
