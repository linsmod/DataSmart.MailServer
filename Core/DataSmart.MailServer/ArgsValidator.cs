using System;

namespace DataSmart.MailServer
{
	public class ArgsValidator
	{
		public static void ValidateFolder(string folder)
		{
			if (folder == null || folder == "")
			{
				throw new ArgumentException("Invalid folder value, user name may not be '' or null !");
			}
			if (ArgsValidator.ContainsChars(folder, new char[]
			{
				':',
				'*',
				'?',
				'"',
				'<',
				'>',
				'|',
				'%'
			}))
			{
				throw new ArgumentException("Invalid folder value, folder may not contain {: * ? \" < > | %} chars ! ");
			}
			if (!ArgsValidator.IsPrintableCharsOnly(folder))
			{
				throw new ArgumentException("Invalid folder value, folder may contain printable chars only !");
			}
		}

		public static void ValidateSharedFolderRoot(string folder)
		{
			if (folder == null || folder == "")
			{
				throw new ArgumentException("Invalid root folder value, user name may not be '' or null !");
			}
			if (ArgsValidator.ContainsChars(folder, new char[]
			{
				':',
				'*',
				'?',
				'"',
				'<',
				'>',
				'|',
				'%',
				'\\',
				'/'
			}))
			{
				throw new ArgumentException("Invalid root folder value, folder may not contain {: * ? \" < > | % \\ /} chars ! ");
			}
			if (!ArgsValidator.IsPrintableCharsOnly(folder))
			{
				throw new ArgumentException("Invalid root folder value, folder may contain printable chars only !");
			}
			if (folder.ToLower() == "inbox")
			{
				throw new ArgumentException("Invalid root folder value, 'Inbox' is reserved folder name !");
			}
			if (folder.ToLower() == "drafts")
			{
				throw new ArgumentException("Invalid root folder value, 'Drafts' is reserved folder name !");
			}
			if (folder.ToLower() == "trash")
			{
				throw new ArgumentException("Invalid root folder value, 'Trash' is reserved folder name !");
			}
			if (folder.ToLower() == "deleted items")
			{
				throw new ArgumentException("Invalid root folder value, 'Deleted Items' is reserved folder name !");
			}
			if (folder.ToLower() == "sent")
			{
				throw new ArgumentException("Invalid root folder value, 'Sent' is reserved folder name !");
			}
			if (folder.ToLower() == "sent items")
			{
				throw new ArgumentException("Invalid root folder value, 'Sent Items' is reserved folder name !");
			}
			if (folder.ToLower() == "outbox")
			{
				throw new ArgumentException("Invalid root folder value, 'Outbox' is reserved folder name !");
			}
		}

		public static void ValidateUserName(string userName)
		{
			if (userName == null || userName == "")
			{
				throw new ArgumentException("Invalid user name value, user name may not be '' or null !");
			}
			if (ArgsValidator.ContainsChars(userName, new char[]
			{
				':',
				'*',
				'?',
				'"',
				'<',
				'>',
				'|',
				'%'
			}))
			{
				throw new ArgumentException("Invalid user name value, user name may not contain {: * ? \" < > | %} chars ! ");
			}
			if (!ArgsValidator.IsPrintableCharsOnly(userName))
			{
				throw new ArgumentException("Invalid user name value, user name may contain printable chars only !");
			}
		}

		public static void ValidateNotNull(object val)
		{
			if (val == null)
			{
				throw new ArgumentNullException("Invalid value, value may not be null !");
			}
		}

		public static void ValidateDomainName(string domainName)
		{
			if (domainName == null)
			{
				throw new ArgumentException("Invalid  domainName value, null values not allowed !");
			}
			if (domainName == "")
			{
				throw new ArgumentException("Invalid  domainName value, please specify domain name !");
			}
			if (!ArgsValidator.IsPrintableCharsOnly(domainName))
			{
				throw new ArgumentException("Invalid  domainName value, domain name may conatin ASCII printable chars only !");
			}
			if (ArgsValidator.ContainsChars(domainName, new char[]
			{
				'@'
			}))
			{
				throw new ArgumentException("Invalid  domainName value, domain name may not conatin '@' char !");
			}
		}

		public static void ValidateEmail(string email)
		{
			if (email == null)
			{
				throw new ArgumentException("Invalid  email value, null values not allowed !");
			}
			if (email == "")
			{
				throw new ArgumentException("Invalid  email value, please specify email !");
			}
		}

		private static bool ContainsChars(string text, char[] chars)
		{
			for (int i = 0; i < chars.Length; i++)
			{
				char value = chars[i];
				if (text.IndexOf(value) > -1)
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsPrintableCharsOnly(string text)
		{
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				if (c <= '\u001f')
				{
					return false;
				}
			}
			return true;
		}
	}
}
