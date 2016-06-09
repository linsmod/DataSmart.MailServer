using System;
using System.Threading;

namespace DataSmart.MailServer
{
	internal class UpdateSync
	{
		private bool m_BlockReads;

		private Thread m_UpdTr;

		private int m_Updates;

		public int m_Reads;

		private object m_UpdSync;

		public UpdateSync()
		{
			this.m_UpdSync = new object();
		}

		public void AddMethod()
		{
			while (this.m_BlockReads && !Thread.CurrentThread.Equals(this.m_UpdTr))
			{
				Thread.Sleep(50);
			}
			lock (this)
			{
				this.m_Reads++;
			}
		}

		public void RemoveMethod()
		{
			lock (this)
			{
				this.m_Reads--;
				if (this.m_Reads < 0)
				{
					throw new Exception("RemoveMethod < 0, RemoveMethod is called more than AddMethod !");
				}
			}
		}

		public void BeginUpdate()
		{
			Monitor.Enter(this.m_UpdSync);
			this.m_Updates++;
			while (true)
			{
				lock (this)
				{
					if (this.m_Reads != 0)
					{
						Thread.Sleep(50);
						continue;
					}
					this.m_BlockReads = true;
				}
				break;
			}
			this.m_UpdTr = Thread.CurrentThread;
		}

		public void EndUpdate()
		{
			this.m_Updates--;
			if (this.m_Updates == 0)
			{
				this.m_BlockReads = false;
				this.m_UpdTr = null;
			}
			Monitor.Exit(this.m_UpdSync);
		}
	}
}
