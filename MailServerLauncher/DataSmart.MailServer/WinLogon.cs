using System;
using System.Runtime.InteropServices;

namespace DataSmart.MailServer
{
	public class WinLogon
	{
		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool LogonUser(string userName, string domainName, string password, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

		public static bool Logon(string domain, string userName, string password)
		{
			IntPtr intPtr = new IntPtr(0);
			intPtr = IntPtr.Zero;
			return WinLogon.LogonUser(userName, domain, password, 2, 0, ref intPtr);
		}
	}
}
