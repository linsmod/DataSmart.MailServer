using System;

namespace DataSmart.MailServer.Management
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

		public override bool Equals(object obj)
		{
			if (obj != null && obj is ServerReturnMessage)
			{
				ServerReturnMessage serverReturnMessage = (ServerReturnMessage)obj;
				return !(serverReturnMessage.Subject != this.Subject) && !(serverReturnMessage.BodyTextRtf != this.BodyTextRtf);
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
