using System.NetworkToolkit;
using System.NetworkToolkit.SIP.Proxy;
using System;

namespace DataSmart.MailServer.Management
{
	public class SipSettings
	{
		private SystemSettings m_pSysSettings;

		private bool m_Enabled;

		private SIP_ProxyMode m_ProxyMode = SIP_ProxyMode.Registrar | SIP_ProxyMode.B2BUA;

		private int m_MinExpires = 60;

		private IPBindInfo[] m_pBinds;

		private SipGatewayCollection m_pGateways;

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

		public SIP_ProxyMode ProxyMode
		{
			get
			{
				return this.m_ProxyMode;
			}
			set
			{
				if (this.m_ProxyMode != value)
				{
					this.m_ProxyMode = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public int MinimumExpires
		{
			get
			{
				return this.m_MinExpires;
			}
			set
			{
				if (value < 60)
				{
					throw new ArgumentException("Argument MinimumExpires value must be >= 60 !");
				}
				if (this.m_MinExpires != value)
				{
					this.m_MinExpires = value;
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

		public SipGatewayCollection Gateways
		{
			get
			{
				return this.m_pGateways;
			}
		}

		internal SipSettings(SystemSettings sysSettings, bool enabled, SIP_ProxyMode proxyMode, int minExpires, IPBindInfo[] bindings, SipGatewayCollection gateways)
		{
			this.m_pSysSettings = sysSettings;
			this.m_Enabled = enabled;
			this.m_ProxyMode = proxyMode;
			this.m_MinExpires = minExpires;
			this.m_pBinds = bindings;
			this.m_pGateways = gateways;
		}
	}
}
