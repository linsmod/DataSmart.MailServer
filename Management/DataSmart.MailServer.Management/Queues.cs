using System;

namespace DataSmart.MailServer.Management
{
	public class Queues
	{
		private VirtualServer m_pServer;

		private QueueItemCollection m_pSMTP;

		private QueueItemCollection m_pRelay;

		public QueueItemCollection SMTP
		{
			get
			{
				if (this.m_pSMTP == null)
				{
					this.m_pSMTP = new QueueItemCollection(this.m_pServer, true);
				}
				return this.m_pSMTP;
			}
		}

		public QueueItemCollection Relay
		{
			get
			{
				if (this.m_pRelay == null)
				{
					this.m_pRelay = new QueueItemCollection(this.m_pServer, false);
				}
				return this.m_pRelay;
			}
		}

		internal Queues(VirtualServer virtualServer)
		{
			this.m_pServer = virtualServer;
		}
	}
}
