using System.NetworkToolkit.SMTP.Server;
using System;

namespace DataSmart.MailServer
{
	public interface ISmtpSenderFilter
	{
		bool Filter(string from, IMailServerManagementApi api, SMTP_Session session, out string errorText);
	}
}
