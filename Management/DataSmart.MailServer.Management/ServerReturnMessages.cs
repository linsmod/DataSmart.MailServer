using System;

namespace DataSmart.MailServer.Management
{
	public class ServerReturnMessages
	{
		private SystemSettings m_pSysSettings;

		private ServerReturnMessage m_pDelayedDeliveryWarning;

		private ServerReturnMessage m_pUndelivered;

		public ServerReturnMessage DelayedDeliveryWarning
		{
			get
			{
				return this.m_pDelayedDeliveryWarning;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("DelayedDeliveryWarning");
				}
				if (!this.m_pDelayedDeliveryWarning.Equals(value))
				{
					this.m_pDelayedDeliveryWarning = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public ServerReturnMessage Undelivered
		{
			get
			{
				return this.m_pUndelivered;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Undelivered");
				}
				if (!this.m_pUndelivered.Equals(value))
				{
					this.m_pUndelivered = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		internal ServerReturnMessages(SystemSettings sysSettings, ServerReturnMessage delayedDeliveryWarning, ServerReturnMessage undelivered)
		{
			this.m_pSysSettings = sysSettings;
			this.m_pDelayedDeliveryWarning = delayedDeliveryWarning;
			this.m_pUndelivered = undelivered;
		}
	}
}
