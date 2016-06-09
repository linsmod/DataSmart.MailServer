using System;

namespace DataSmart.MailServer
{
	public class GlobalMessageRuleActionResult
	{
		private bool m_DeleteMessage;

		private string m_StoreFolder;

		private string m_ErrorText;

		public bool DeleteMessage
		{
			get
			{
				return this.m_DeleteMessage;
			}
		}

		public string StoreFolder
		{
			get
			{
				return this.m_StoreFolder;
			}
		}

		public string ErrorText
		{
			get
			{
				return this.m_ErrorText;
			}
		}

		internal GlobalMessageRuleActionResult(bool deleteMessage, string storeFolder, string errorText)
		{
			this.m_DeleteMessage = deleteMessage;
			this.m_StoreFolder = storeFolder;
			this.m_ErrorText = errorText;
		}
	}
}
