using System.NetworkToolkit;
using System;

namespace DataSmart.MailServer.Management
{
	public class POP3_Settings
	{
		private SystemSettings m_pSysSettings;

		private bool m_Enabled;

		private string m_GreetingText = "";

		private int m_IdleTimeout;

		private int m_MaxConnections;

		private int m_MaxConnsPerIP;

		private int m_MaxBadCommands;

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
				return this.m_MaxBadCommands;
			}
			set
			{
				if (this.m_MaxBadCommands != value)
				{
					this.m_MaxBadCommands = value;
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

		internal POP3_Settings(SystemSettings sysSettings, bool enabled, string greeting, int idleTimeout, int maxConnections, int maxConnectionsPerIP, int maxBadCommands, IPBindInfo[] bindings)
		{
			this.m_pSysSettings = sysSettings;
			this.m_Enabled = enabled;
			this.m_GreetingText = greeting;
			this.m_IdleTimeout = idleTimeout;
			this.m_MaxConnections = maxConnections;
			this.m_MaxConnsPerIP = maxConnectionsPerIP;
			this.m_MaxBadCommands = maxBadCommands;
			this.m_pBinds = bindings;
		}
	}
}
