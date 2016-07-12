using System;
using System.Data;
using System.Diagnostics;
using System.Timers;

namespace DataSmart.MailServer
{
	internal class RecycleBinManager : IDisposable
	{
		private IMailServerManagementApi m_pApi;

		private Timer m_pTimer;

		private DateTime m_LastCleanTime;

		public DateTime LastCleanUpTime
		{
			get
			{
				return this.m_LastCleanTime;
			}
		}

		public RecycleBinManager(IMailServerManagementApi api)
		{
			this.m_pApi = api;
			this.m_pTimer = new Timer();
			this.m_pTimer.Interval = 3600000.0;
			this.m_pTimer.Elapsed += new ElapsedEventHandler(this.m_pTimer_Elapsed);
			this.m_pTimer.Enabled = true;
			this.m_LastCleanTime = DateTime.MinValue;
		}

		public void Dispose()
		{
			if (this.m_pTimer != null)
			{
				this.m_pTimer.Dispose();
				this.m_pTimer = null;
			}
		}

		private void m_pTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			this.DoCleanUp();
		}

		public void DoCleanUp()
		{
			try
			{
				int num = Convert.ToInt32(this.m_pApi.GetRecycleBinSettings().Rows[0]["DeleteMessagesAfter"]);
				foreach (DataRowView dataRowView in this.m_pApi.GetRecycleBinMessagesInfo(null, DateTime.MinValue, DateTime.Today.AddDays((double)(-(double)num))))
				{
					if (DateTime.Now > Convert.ToDateTime(dataRowView["DeleteTime"]).AddDays((double)num))
					{
						try
						{
							this.m_pApi.DeleteRecycleBinMessage(dataRowView["MessageID"].ToString());
						}
						catch
						{
						}
					}
				}
				this.m_LastCleanTime = DateTime.Now;
			}
			catch (Exception x)
			{
				Error.DumpError(x);
			}
		}
	}
}
