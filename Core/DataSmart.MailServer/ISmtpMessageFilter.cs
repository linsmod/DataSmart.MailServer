using System.NetworkToolkit.SMTP.Server;
using System;
using System.IO;

namespace DataSmart.MailServer
{
	public interface ISmtpMessageFilter
	{
		FilterResult Filter(Stream messageStream, out Stream filteredStream, string sender, string[] recipients, IMailServerManagementApi api, SMTP_Session session, out string errorText);
	}
}
