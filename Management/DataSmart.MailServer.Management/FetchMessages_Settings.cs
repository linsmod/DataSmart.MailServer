using System;

namespace DataSmart.MailServer.Management
{
	public class FetchMessages_Settings
	{
		private SystemSettings m_pSysSettings;

		private bool m_Enabled;

		private int m_FetchInterval;

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

		public int FetchInterval
		{
			get
			{
				return this.m_FetchInterval;
			}
			set
			{
				if (this.m_FetchInterval != value)
				{
					this.m_FetchInterval = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		internal FetchMessages_Settings(SystemSettings sysSettings, bool enabled, int fetchInterval)
		{
			this.m_pSysSettings = sysSettings;
			this.m_Enabled = enabled;
			this.m_FetchInterval = fetchInterval;
		}
	}
}
