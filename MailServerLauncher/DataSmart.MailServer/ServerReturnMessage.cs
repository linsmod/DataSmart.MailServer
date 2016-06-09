using System;

namespace DataSmart.MailServer
{
	public class ServerReturnMessage
	{
		private string m_Subject = "";

		private string m_BodyTextRtf = "";

		public string Subject
		{
			get
			{
				return this.m_Subject;
			}
		}

		public string BodyTextRtf
		{
			get
			{
				return this.m_BodyTextRtf;
			}
		}

		public ServerReturnMessage(string subject, string bodyTextRft)
		{
			this.m_Subject = subject;
			this.m_BodyTextRtf = bodyTextRft;
		}
	}
}
