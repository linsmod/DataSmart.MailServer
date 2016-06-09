using System.NetworkToolkit;
using System;
using System.Data;
using System.IO;
using System.Net;

namespace DataSmart.MailServer.Management
{
	public class LogSession
	{
		private VirtualServer m_pVirtualServer;

		private string m_Service = "";

		private string m_SessionID = "";

		private DateTime m_StartTime;

		private IPEndPoint m_pRemoteEndPoint;

		private string m_UserName = "";

		public string SessionID
		{
			get
			{
				return this.m_SessionID;
			}
		}

		public DateTime StartTime
		{
			get
			{
				return this.m_StartTime;
			}
		}

		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return this.m_pRemoteEndPoint;
			}
		}

		public string UserName
		{
			get
			{
				return this.m_UserName;
			}
		}

		public string LogText
		{
			get
			{
				return this.GetLogText();
			}
		}

		internal LogSession(VirtualServer virtualServer, string service, string sessionID, DateTime startTime, IPEndPoint remoteEndPoint, string userName)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_Service = service;
			this.m_SessionID = sessionID;
			this.m_StartTime = startTime;
			this.m_pRemoteEndPoint = remoteEndPoint;
			this.m_UserName = userName;
		}

		private string GetLogText()
		{
			string result;
			lock (this.m_pVirtualServer.Server)
			{
				this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
				{
					"GetSessionLog ",
					this.m_pVirtualServer.VirtualServerID,
					" ",
					this.m_Service,
					" ",
					TextUtils.QuoteString(this.m_SessionID),
					" ",
					TextUtils.QuoteString(this.m_StartTime.ToUniversalTime().ToString("u"))
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
				if (dataSet.Tables.Contains("SessionLog") && dataSet.Tables["SessionLog"].Rows.Count > 0)
				{
					result = dataSet.Tables["SessionLog"].Rows[0]["LogText"].ToString();
				}
				else
				{
					result = "";
				}
			}
			return result;
		}
	}
}
