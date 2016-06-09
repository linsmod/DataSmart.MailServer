using System.NetworkToolkit;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;

namespace DataSmart.MailServer.Management
{
	public class Logs
	{
		private VirtualServer m_pVirtualServer;

		internal Logs(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
		}

		public LogSession[] GetSmtpLogSessions(int limit, DateTime date, DateTime startTime, DateTime endTime, string containsText)
		{
			return this.GetLogSessions("SMTP", limit, date, startTime, endTime, containsText);
		}

		public LogSession[] GetPop3LogSessions(int limit, DateTime date, DateTime startTime, DateTime endTime, string containsText)
		{
			return this.GetLogSessions("POP3", limit, date, startTime, endTime, containsText);
		}

		public LogSession[] GetImapLogSessions(int limit, DateTime date, DateTime startTime, DateTime endTime, string containsText)
		{
			return this.GetLogSessions("IMAP", limit, date, startTime, endTime, containsText);
		}

		public LogSession[] GetRelayLogSessions(int limit, DateTime date, DateTime startTime, DateTime endTime, string containsText)
		{
			return this.GetLogSessions("RELAY", limit, date, startTime, endTime, containsText);
		}

		public LogSession[] GetFetchLogSessions(int limit, DateTime date, DateTime startTime, DateTime endTime, string containsText)
		{
			return this.GetLogSessions("FETCH", limit, date, startTime, endTime, containsText);
		}

		private LogSession[] GetLogSessions(string service, int limit, DateTime date, DateTime startTime, DateTime endTime, string containsText)
		{
			LogSession[] result;
			lock (this.m_pVirtualServer.Server.LockSynchronizer)
			{
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
				{
					"GetLogSessions ",
					this.m_pVirtualServer.VirtualServerID,
					" ",
					service,
					" ",
					limit.ToString(),
					" ",
					TextUtils.QuoteString(startTime.ToUniversalTime().ToString("yyyyMMddHHmmss")),
					" ",
					TextUtils.QuoteString(endTime.ToUniversalTime().ToString("yyyyMMddHHmmss")),
					" ",
					TextUtils.QuoteString(containsText)
				}));
				string text = this.m_pVirtualServer.Server.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
				int num = Convert.ToInt32(text.Split(new char[]
				{
					' '
				}, 2)[1]);
				MemoryStream memoryStream = new MemoryStream();
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.ReadFixedCount(memoryStream, (long)num);
				DataSet dataSet = Utils.DecompressDataSet(memoryStream);
				List<LogSession> list = new List<LogSession>();
				if (dataSet.Tables.Contains("LogSessions"))
				{
					foreach (DataRow dataRow in dataSet.Tables["LogSessions"].Rows)
					{
						list.Add(new LogSession(this.m_pVirtualServer, service, dataRow["SessionID"].ToString(), Convert.ToDateTime(dataRow["StartTime"]), ConvertEx.ToIPEndPoint(dataRow["RemoteEndPoint"].ToString(), new IPEndPoint(IPAddress.None, 0)), dataRow["UserName"].ToString()));
					}
				}
				result = list.ToArray();
			}
			return result;
		}
	}
}
