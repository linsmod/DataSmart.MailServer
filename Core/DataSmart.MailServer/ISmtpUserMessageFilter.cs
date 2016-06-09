using System.NetworkToolkit.SMTP.Server;
using System;
using System.IO;

namespace DataSmart.MailServer
{
	public interface ISmtpUserMessageFilter
	{
		void Filter(MemoryStream messageStream, out MemoryStream filteredStream, string userName, string to, IMailServerManagementApi api, SMTP_Session session, out string storeFolder, out string errorText);
	}
}
