using System;

namespace DataSmart.MailServer.Management
{
	public class Logging_Settings
	{
		private SystemSettings m_pSysSettings;

		private bool m_LogSMTP;

		private string m_SmtpLogsPath = "";

		private bool m_LogPOP3;

		private string m_Pop3LogsPath = "";

		private bool m_LogIMAP;

		private string m_ImapLogsPath = "";

		private bool m_LogRelay;

		private string m_RelayLogsPath = "";

		private bool m_LogFetchMessages;

		private string m_FetchMessagesLogsPath = "";

		public bool LogSMTP
		{
			get
			{
				return this.m_LogSMTP;
			}
			set
			{
				if (this.m_LogSMTP != value)
				{
					this.m_LogSMTP = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public string SmtpLogsPath
		{
			get
			{
				return this.m_SmtpLogsPath;
			}
			set
			{
				if (this.m_SmtpLogsPath != value)
				{
					this.m_SmtpLogsPath = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public bool LogPOP3
		{
			get
			{
				return this.m_LogPOP3;
			}
			set
			{
				if (this.m_LogPOP3 != value)
				{
					this.m_LogPOP3 = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public string Pop3LogsPath
		{
			get
			{
				return this.m_Pop3LogsPath;
			}
			set
			{
				if (this.m_Pop3LogsPath != value)
				{
					this.m_Pop3LogsPath = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public bool LogIMAP
		{
			get
			{
				return this.m_LogIMAP;
			}
			set
			{
				if (this.m_LogIMAP != value)
				{
					this.m_LogIMAP = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public string ImapLogsPath
		{
			get
			{
				return this.m_ImapLogsPath;
			}
			set
			{
				if (this.m_ImapLogsPath != value)
				{
					this.m_ImapLogsPath = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public bool LogRelay
		{
			get
			{
				return this.m_LogRelay;
			}
			set
			{
				if (this.m_LogRelay != value)
				{
					this.m_LogRelay = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public string RelayLogsPath
		{
			get
			{
				return this.m_RelayLogsPath;
			}
			set
			{
				if (this.m_RelayLogsPath != value)
				{
					this.m_RelayLogsPath = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public bool LogFetchMessages
		{
			get
			{
				return this.m_LogFetchMessages;
			}
			set
			{
				if (this.m_LogFetchMessages != value)
				{
					this.m_LogFetchMessages = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		public string FetchMessagesLogsPath
		{
			get
			{
				return this.m_FetchMessagesLogsPath;
			}
			set
			{
				if (this.m_FetchMessagesLogsPath != value)
				{
					this.m_FetchMessagesLogsPath = value;
					this.m_pSysSettings.SetValuesChanged();
				}
			}
		}

		internal Logging_Settings(SystemSettings sysSettings, bool logSMTP, string smtpLogsPath, bool logPOP3, string pop3LogsPath, bool logIMAP, string imapLogsPath, bool logRelay, string relayLogsPath, bool logFetchMessages, string fetchMessagesLogsPath)
		{
			this.m_pSysSettings = sysSettings;
			this.m_LogSMTP = logSMTP;
			this.m_SmtpLogsPath = smtpLogsPath;
			this.m_LogPOP3 = logPOP3;
			this.m_Pop3LogsPath = pop3LogsPath;
			this.m_LogIMAP = logIMAP;
			this.m_ImapLogsPath = imapLogsPath;
			this.m_LogRelay = logRelay;
			this.m_RelayLogsPath = relayLogsPath;
			this.m_LogFetchMessages = logFetchMessages;
			this.m_FetchMessagesLogsPath = fetchMessagesLogsPath;
		}
	}
}
