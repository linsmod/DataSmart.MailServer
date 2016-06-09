using System;

namespace DataSmart.MailServer.Management
{
	public enum UserPermissions
	{
		None,
		All = 65535,
		POP3 = 2,
		IMAP = 4,
		Relay = 8,
		SIP = 16
	}
}
