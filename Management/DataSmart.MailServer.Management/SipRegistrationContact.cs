using System;

namespace DataSmart.MailServer.Management
{
	public class SipRegistrationContact
	{
		private string m_ContactUri = "";

		private int m_Expires;

		private double m_Priority;

		public string ContactUri
		{
			get
			{
				return this.m_ContactUri;
			}
		}

		public int Expires
		{
			get
			{
				return this.m_Expires;
			}
		}

		public double Priority
		{
			get
			{
				if (this.m_Priority == -1.0)
				{
					return 1.0;
				}
				return this.m_Priority;
			}
		}

		public SipRegistrationContact(string contactUri, int exprires, double priority)
		{
			this.m_ContactUri = contactUri;
			this.m_Expires = exprires;
			this.m_Priority = priority;
		}
	}
}
