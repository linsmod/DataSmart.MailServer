using System.NetworkToolkit;
using System;

namespace DataSmart.MailServer.Management
{
	public class Session
	{
		private SessionCollection m_pOwner;

		private string m_ID = "";

		private string m_Type = "";

		private DateTime m_SartTime;

		private int m_TimeoutSeconds;

		private string m_UserName = "";

		private string m_LocalEndPoint = "";

		private string m_RemoteEndPoint = "";

		private int m_ReadKbSec;

		private int m_WriteKbSec;

		private string m_SessionLog = "";

		public string ID
		{
			get
			{
				return this.m_ID;
			}
		}

		public string Type
		{
			get
			{
				return this.m_Type;
			}
		}

		public DateTime SartTime
		{
			get
			{
				return this.m_SartTime;
			}
		}

		public int IdleTimeOutSeconds
		{
			get
			{
				return this.m_TimeoutSeconds;
			}
		}

		public string UserName
		{
			get
			{
				return this.m_UserName;
			}
		}

		public string LocalEndPoint
		{
			get
			{
				return this.m_LocalEndPoint;
			}
		}

		public string RemoteEndPoint
		{
			get
			{
				return this.m_RemoteEndPoint;
			}
		}

		public int ReadKbInSecond
		{
			get
			{
				return this.m_ReadKbSec;
			}
		}

		public int WriteKbInSecond
		{
			get
			{
				return this.m_WriteKbSec;
			}
		}

		public string SessionLog
		{
			get
			{
				return this.m_SessionLog;
			}
		}

		internal Session(SessionCollection owner, string id, string type, DateTime startTime, int timeoutSec, string userName, string localEndPoint, string remoteEndPoint, int readKbSec, int writeKbSec, string sessionLog)
		{
			this.m_pOwner = owner;
			this.m_ID = id;
			this.m_Type = type;
			this.m_SartTime = startTime;
			this.m_TimeoutSeconds = timeoutSec;
			this.m_UserName = userName;
			this.m_LocalEndPoint = localEndPoint;
			this.m_RemoteEndPoint = remoteEndPoint;
			this.m_ReadKbSec = readKbSec;
			this.m_WriteKbSec = writeKbSec;
			this.m_SessionLog = sessionLog;
		}

		public void Kill()
		{
			this.m_pOwner.Server.TCP_Client.TcpStream.WriteLine("KillSession " + TextUtils.QuoteString(this.m_ID));
			string text = this.m_pOwner.Server.ReadLine();
			if (text != null && !text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pOwner.List.Remove(this);
		}
	}
}
