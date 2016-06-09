using System.NetworkToolkit.Log;
using System.NetworkToolkit.POP3.Client;
using System;
using System.Data;
using System.IO;
using System.Timers;

namespace DataSmart.MailServer
{
	internal class FetchPop3 : IDisposable
	{
		private VirtualServer m_pServer;

		private IMailServerManagementApi m_pApi;

		private bool m_Enabled = true;

		private bool m_Fetching;

		private DateTime m_LastFetch;

		private string m_LogPath = "";

		private bool m_LogCommands;

		private Timer m_pTimer;

		private int m_FetchInterval = 300;

		public bool Enabled
		{
			get
			{
				return this.m_Enabled;
			}
			set
			{
				this.m_Enabled = value;
				this.m_pTimer.Enabled = value;
			}
		}

		public bool IsFetching
		{
			get
			{
				return this.m_Fetching;
			}
		}

		public bool FetchTime
		{
			get
			{
				return this.m_LastFetch.AddSeconds((double)this.m_FetchInterval) < DateTime.Now;
			}
		}

		public string LogPath
		{
			get
			{
				return this.m_LogPath;
			}
			set
			{
				this.m_LogPath = value;
			}
		}

		public bool LogCommands
		{
			get
			{
				return this.m_LogCommands;
			}
			set
			{
				this.m_LogCommands = value;
			}
		}

		public int FetchInterval
		{
			get
			{
				return this.m_FetchInterval;
			}
			set
			{
				this.m_FetchInterval = value;
			}
		}

		public FetchPop3(VirtualServer server, IMailServerManagementApi api)
		{
			this.m_pServer = server;
			this.m_pApi = api;
			this.m_LastFetch = DateTime.Now.AddMinutes(-5.0);
			this.m_pTimer = new Timer();
			this.m_pTimer.Interval = 15000.0;
			this.m_pTimer.Elapsed += new ElapsedEventHandler(this.m_pTimer_Elapsed);
		}

		public void Dispose()
		{
			if (this.m_pTimer != null)
			{
				this.m_pTimer.Dispose();
				this.m_pTimer = null;
			}
		}

		private void m_pTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (this.Enabled && this.FetchTime)
			{
				this.StartFetching();
			}
		}

		private void Pop3_WriteLog(object sender, WriteLogEventArgs e)
		{
			Logger.WriteLog(this.m_LogPath + "fetch-" + DateTime.Today.ToString("yyyyMMdd") + ".log", e.LogEntry);
		}

		public void StartFetching()
		{
			if (this.m_Fetching)
			{
				return;
			}
			this.m_Fetching = true;
			try
			{
				DataView users = this.m_pApi.GetUsers("ALL");
				using (DataView userRemoteServers = this.m_pApi.GetUserRemoteServers(""))
				{
					foreach (DataRowView dataRowView in userRemoteServers)
					{
						try
						{
							if (ConvertEx.ToBoolean(dataRowView["Enabled"]))
							{
								users.RowFilter = "UserID='" + dataRowView["UserID"] + "'";
								if (users.Count > 0)
								{
									string userName = users[0]["UserName"].ToString();
									string host = dataRowView.Row["RemoteServer"].ToString();
									int port = Convert.ToInt32(dataRowView.Row["RemotePort"]);
									string user = dataRowView.Row["RemoteUserName"].ToString();
									string password = dataRowView.Row["RemotePassword"].ToString();
									bool ssl = ConvertEx.ToBoolean(dataRowView["UseSSL"]);
									using (POP3_Client pOP3_Client = new POP3_Client())
									{
										pOP3_Client.Logger = new System.NetworkToolkit.Log.Logger();
										pOP3_Client.Logger.WriteLog += new EventHandler<WriteLogEventArgs>(this.Pop3_WriteLog);
										pOP3_Client.Connect(host, port, ssl);
										pOP3_Client.Login(user, password);
										foreach (POP3_ClientMessage pOP3_ClientMessage in pOP3_Client.Messages)
										{
											this.m_pServer.ProcessUserMsg("", "", userName, "Inbox", new MemoryStream(pOP3_ClientMessage.MessageToByte()), null);
											pOP3_ClientMessage.MarkForDeletion();
										}
									}
								}
							}
						}
						catch
						{
						}
					}
				}
				this.m_LastFetch = DateTime.Now;
			}
			catch (Exception x)
			{
				Error.DumpError(this.m_pServer.Name, x);
			}
			this.m_Fetching = false;
		}
	}
}
