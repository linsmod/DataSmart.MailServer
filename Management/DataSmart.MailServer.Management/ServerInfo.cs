using System;

namespace DataSmart.MailServer.Management
{
	public class ServerInfo
	{
		private string m_OS = "";

		private string m_ServerVersion = "";

		private int m_MemoryUsage;

		private int m_CpuUsage;

		private DateTime m_pServerStartTime;

		private int m_ReadsSec;

		private int m_WitesSec;

		private int m_SmtpSessions;

		private int m_Pop3Sessions;

		private int m_ImapSessions;

		private int m_RelaySessions;

		public string OS
		{
			get
			{
				return this.m_OS;
			}
		}

		public string MailServerVersion
		{
			get
			{
				return this.m_ServerVersion;
			}
		}

		public int MemoryUsage
		{
			get
			{
				return this.m_MemoryUsage;
			}
		}

		public int CpuUsage
		{
			get
			{
				return this.m_CpuUsage;
			}
		}

		public DateTime ServerStartTime
		{
			get
			{
				return this.m_pServerStartTime;
			}
		}

		public int ReadsInSecond
		{
			get
			{
				return this.m_ReadsSec;
			}
		}

		public int WritesInSecond
		{
			get
			{
				return this.m_WitesSec;
			}
		}

		public int TotalSmtpSessions
		{
			get
			{
				return this.m_SmtpSessions;
			}
		}

		public int TotalPop3Sessions
		{
			get
			{
				return this.m_Pop3Sessions;
			}
		}

		public int TotalImapSessions
		{
			get
			{
				return this.m_ImapSessions;
			}
		}

		public int TotalRelaySessions
		{
			get
			{
				return this.m_RelaySessions;
			}
		}

		internal ServerInfo(string os, string mailserverVersion, int memUsage, int cpuUsage, DateTime serverDateTime, int readsSec, int writesSec, int smtpSessions, int pop3Sessions, int imapSessions, int relaySessions)
		{
			this.m_OS = os;
			this.m_ServerVersion = mailserverVersion;
			this.m_MemoryUsage = memUsage;
			this.m_CpuUsage = cpuUsage;
			this.m_pServerStartTime = serverDateTime;
			this.m_ReadsSec = readsSec;
			this.m_WitesSec = writesSec;
			this.m_SmtpSessions = smtpSessions;
			this.m_Pop3Sessions = pop3Sessions;
			this.m_ImapSessions = imapSessions;
			this.m_RelaySessions = relaySessions;
		}
	}
}
