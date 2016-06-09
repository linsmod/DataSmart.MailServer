using System;

namespace DataSmart.MailServer
{
	public class InsufficientPermissionsException : Exception
	{
		public InsufficientPermissionsException(string errorText) : base(errorText)
		{
		}
	}
}
