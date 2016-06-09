using System;

namespace DataSmart.MailServer.Management
{
	public class QueueItem
	{
		private DateTime m_CreateTime;

		private string m_Header = "";

		public DateTime CreateTime
		{
			get
			{
				return this.m_CreateTime;
			}
		}

		public string Header
		{
			get
			{
				return this.m_Header;
			}
		}

		internal QueueItem(DateTime createTime, string header)
		{
			this.m_CreateTime = createTime;
			this.m_Header = header;
		}
	}
}
