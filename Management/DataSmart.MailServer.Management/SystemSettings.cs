using System.NetworkToolkit;
using System.NetworkToolkit.SIP.Proxy;
using System.NetworkToolkit.SMTP.Relay;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace DataSmart.MailServer.Management
{
	public class SystemSettings
	{
		private VirtualServer m_pVirtualServer;

		private IPAddress[] m_pDnsServers;

		private Auth_Settings m_pAuth;

		private SmtpSettings m_pSMTP;

		private POP3_Settings m_pPOP3;

		private IMAP_Settings m_pIMAP;

		private Relay_Settings m_pRelay;

		private FetchMessages_Settings m_pFetchMessages;

		private SipSettings m_pSIP;

		private Logging_Settings m_pLogging;

		private ServerReturnMessages m_pReturnMessages;

		private bool m_ValuesChanged;

		public bool HasChanges
		{
			get
			{
				return this.m_ValuesChanged;
			}
		}

		public IPAddress[] DnsServers
		{
			get
			{
				return this.m_pDnsServers;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("DnsServers");
				}
				bool flag = false;
				if (this.m_pDnsServers.Length == value.Length)
				{
					for (int i = 0; i < this.m_pDnsServers.Length; i++)
					{
						if (!this.m_pDnsServers[i].Equals(value[i]))
						{
							flag = true;
							break;
						}
					}
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					this.m_pDnsServers = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public Auth_Settings Authentication
		{
			get
			{
				return this.m_pAuth;
			}
		}

		public SmtpSettings SMTP
		{
			get
			{
				return this.m_pSMTP;
			}
		}

		public POP3_Settings POP3
		{
			get
			{
				return this.m_pPOP3;
			}
		}

		public IMAP_Settings IMAP
		{
			get
			{
				return this.m_pIMAP;
			}
		}

		public Relay_Settings Relay
		{
			get
			{
				return this.m_pRelay;
			}
		}

		public FetchMessages_Settings FetchMessages
		{
			get
			{
				return this.m_pFetchMessages;
			}
		}

		public SipSettings SIP
		{
			get
			{
				return this.m_pSIP;
			}
		}

		public Logging_Settings Logging
		{
			get
			{
				return this.m_pLogging;
			}
		}

		public ServerReturnMessages ReturnMessages
		{
			get
			{
				return this.m_pReturnMessages;
			}
		}

		internal SystemSettings(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.Bind();
		}

		public void Commit()
		{
			if (!this.m_ValuesChanged)
			{
				return;
			}
			MemoryStream memoryStream = new MemoryStream();
			this.ToDataSet().WriteXml(memoryStream);
			byte[] array = memoryStream.ToArray();
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("UpdateSettings " + this.m_pVirtualServer.VirtualServerID + " " + array.Length.ToString());
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.Write(array, 0, array.Length);
			string text = this.m_pVirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_ValuesChanged = false;
		}

		internal void LoadSettings(DataSet dsSettings)
		{
			DataSet dataSet = new DataSet();
			this.CreateSettingsSchema(dataSet);
			dataSet.Clear();
			MemoryStream memoryStream = new MemoryStream();
			dsSettings.WriteXml(memoryStream);
			memoryStream.Position = 0L;
			dataSet.ReadXml(memoryStream);
			DataRow dataRow = dataSet.Tables["Settings"].Rows[0];
			List<IPAddress> list = new List<IPAddress>();
			foreach (DataRow dataRow2 in dataSet.Tables["DnsServers"].Rows)
			{
				list.Add(IPAddress.Parse(dataRow2["IP"].ToString()));
			}
			this.m_pDnsServers = list.ToArray();
			this.m_pAuth.AuthenticationType = (ServerAuthenticationType_enum)Convert.ToInt32(dataRow["ServerAuthenticationType"]);
			this.m_pAuth.WinDomain = dataRow["ServerAuthWinDomain"].ToString();
			this.m_pAuth.LdapServer = dataRow["LdapServer"].ToString();
			this.m_pAuth.LdapDn = dataRow["LdapDN"].ToString();
			IPBindInfo[] array = new IPBindInfo[dataSet.Tables["SMTP_Bindings"].Rows.Count];
			for (int i = 0; i < dataSet.Tables["SMTP_Bindings"].Rows.Count; i++)
			{
				DataRow dataRow3 = dataSet.Tables["SMTP_Bindings"].Rows[i];
				byte[] cert = null;
				if (dataSet.Tables["SMTP_Bindings"].Columns.Contains("SSL_Certificate") && dataRow3["SSL_Certificate"] != null && !dataRow3.IsNull("SSL_Certificate") && ((byte[])dataRow3["SSL_Certificate"]).Length > 0)
				{
					cert = (byte[])dataRow3["SSL_Certificate"];
				}
				array[i] = new IPBindInfo(dataRow3["HostName"].ToString(), BindInfoProtocol.TCP, IPAddress.Parse(dataRow3["IP"].ToString()), Convert.ToInt32(dataRow3["Port"]), this.ParseSslMode(dataRow3["SSL"].ToString()), this.PaseCertificate(cert));
			}
			this.m_pSMTP.Enabled = Convert.ToBoolean(dataRow["SMTP_Enabled"]);
			this.m_pSMTP.GreetingText = dataRow["SMTP_GreetingText"].ToString();
			this.m_pSMTP.DefaultDomain = dataRow["SMTP_DefaultDomain"].ToString();
			this.m_pSMTP.SessionIdleTimeOut = Convert.ToInt32(dataRow["SMTP_SessionIdleTimeOut"]);
			this.m_pSMTP.MaximumConnections = Convert.ToInt32(dataRow["SMTP_Threads"]);
			this.m_pSMTP.MaximumConnectionsPerIP = Convert.ToInt32(dataRow["SMTP_MaxConnectionsPerIP"]);
			this.m_pSMTP.MaximumBadCommands = Convert.ToInt32(dataRow["SMTP_MaxBadCommands"]);
			this.m_pSMTP.MaximumRecipientsPerMessage = Convert.ToInt32(dataRow["MaxRecipients"]);
			this.m_pSMTP.MaximumMessageSize = Convert.ToInt32(dataRow["MaxMessageSize"]);
			this.m_pSMTP.RequireAuthentication = Convert.ToBoolean(dataRow["SMTP_RequireAuth"]);
			this.m_pSMTP.Binds = array;
			IPBindInfo[] array2 = new IPBindInfo[dataSet.Tables["POP3_Bindings"].Rows.Count];
			for (int j = 0; j < dataSet.Tables["POP3_Bindings"].Rows.Count; j++)
			{
				DataRow dataRow4 = dataSet.Tables["POP3_Bindings"].Rows[j];
				byte[] cert2 = null;
				if (dataSet.Tables["POP3_Bindings"].Columns.Contains("SSL_Certificate") && dataRow4["SSL_Certificate"] != null && !dataRow4.IsNull("SSL_Certificate") && ((byte[])dataRow4["SSL_Certificate"]).Length > 0)
				{
					cert2 = (byte[])dataRow4["SSL_Certificate"];
				}
				array2[j] = new IPBindInfo(dataRow4["HostName"].ToString(), BindInfoProtocol.TCP, IPAddress.Parse(dataRow4["IP"].ToString()), Convert.ToInt32(dataRow4["Port"]), this.ParseSslMode(dataRow4["SSL"].ToString()), this.PaseCertificate(cert2));
			}
			this.m_pPOP3.Enabled = Convert.ToBoolean(dataRow["POP3_Enabled"]);
			this.m_pPOP3.GreetingText = dataRow["POP3_GreetingText"].ToString();
			this.m_pPOP3.SessionIdleTimeOut = Convert.ToInt32(dataRow["POP3_SessionIdleTimeOut"]);
			this.m_pPOP3.MaximumConnections = Convert.ToInt32(dataRow["POP3_Threads"]);
			this.m_pPOP3.MaximumConnectionsPerIP = Convert.ToInt32(dataRow["POP3_MaxConnectionsPerIP"]);
			this.m_pPOP3.MaximumBadCommands = Convert.ToInt32(dataRow["POP3_MaxBadCommands"]);
			this.m_pPOP3.Binds = array2;
			IPBindInfo[] array3 = new IPBindInfo[dataSet.Tables["IMAP_Bindings"].Rows.Count];
			for (int k = 0; k < dataSet.Tables["IMAP_Bindings"].Rows.Count; k++)
			{
				DataRow dataRow5 = dataSet.Tables["IMAP_Bindings"].Rows[k];
				byte[] cert3 = null;
				if (dataSet.Tables["IMAP_Bindings"].Columns.Contains("SSL_Certificate") && dataRow5["SSL_Certificate"] != null && !dataRow5.IsNull("SSL_Certificate") && ((byte[])dataRow5["SSL_Certificate"]).Length > 0)
				{
					cert3 = (byte[])dataRow5["SSL_Certificate"];
				}
				array3[k] = new IPBindInfo(dataRow5["HostName"].ToString(), BindInfoProtocol.TCP, IPAddress.Parse(dataRow5["IP"].ToString()), Convert.ToInt32(dataRow5["Port"]), this.ParseSslMode(dataRow5["SSL"].ToString()), this.PaseCertificate(cert3));
			}
			this.m_pIMAP.Enabled = Convert.ToBoolean(dataRow["IMAP_Enabled"]);
			this.m_pIMAP.GreetingText = dataRow["IMAP_GreetingText"].ToString();
			this.m_pIMAP.SessionIdleTimeOut = Convert.ToInt32(dataRow["IMAP_SessionIdleTimeOut"]);
			this.m_pIMAP.MaximumConnections = Convert.ToInt32(dataRow["IMAP_Threads"]);
			this.m_pIMAP.MaximumConnectionsPerIP = Convert.ToInt32(dataRow["IMAP_MaxConnectionsPerIP"]);
			this.m_pIMAP.MaximumBadCommands = Convert.ToInt32(dataRow["IMAP_MaxBadCommands"]);
			this.m_pIMAP.Binds = array3;
			Relay_SmartHost[] array4 = new Relay_SmartHost[dataSet.Tables["Relay_SmartHosts"].Rows.Count];
			for (int l = 0; l < array4.Length; l++)
			{
				DataRow dataRow6 = dataSet.Tables["Relay_SmartHosts"].Rows[l];
				array4[l] = new Relay_SmartHost(ConvertEx.ToString(dataRow6["Host"]), ConvertEx.ToInt32(dataRow6["Port"]), (SslMode)Enum.Parse(typeof(SslMode), dataRow6["SslMode"].ToString()), ConvertEx.ToString(dataRow6["UserName"]), ConvertEx.ToString(dataRow6["Password"]));
			}
			IPBindInfo[] array5 = new IPBindInfo[dataSet.Tables["Relay_Bindings"].Rows.Count];
			for (int m = 0; m < dataSet.Tables["Relay_Bindings"].Rows.Count; m++)
			{
				DataRow dataRow7 = dataSet.Tables["Relay_Bindings"].Rows[m];
				byte[] cert4 = null;
				if (dataSet.Tables["Relay_Bindings"].Columns.Contains("SSL_Certificate") && dataRow7["SSL_Certificate"] != null && !dataRow7.IsNull("SSL_Certificate") && ((byte[])dataRow7["SSL_Certificate"]).Length > 0)
				{
					cert4 = (byte[])dataRow7["SSL_Certificate"];
				}
				array5[m] = new IPBindInfo(dataRow7["HostName"].ToString(), BindInfoProtocol.TCP, IPAddress.Parse(dataRow7["IP"].ToString()), Convert.ToInt32(dataRow7["Port"]), this.ParseSslMode(dataRow7["SSL"].ToString()), this.PaseCertificate(cert4));
			}
			this.m_pRelay.RelayMode = (Relay_Mode)Enum.Parse(typeof(Relay_Mode), dataRow["Relay_Mode"].ToString());
			this.m_pRelay.SmartHostsBalanceMode = (BalanceMode)Enum.Parse(typeof(BalanceMode), dataRow["Relay_SmartHostsBalanceMode"].ToString());
			this.m_pRelay.SmartHosts = array4;
			this.m_pRelay.SessionIdleTimeOut = Convert.ToInt32(dataRow["Relay_SessionIdleTimeOut"]);
			this.m_pRelay.MaximumConnections = Convert.ToInt32(dataRow["MaxRelayThreads"]);
			this.m_pRelay.MaximumConnectionsPerIP = Convert.ToInt32(dataRow["Relay_MaxConnectionsPerIP"]);
			this.m_pRelay.RelayInterval = Convert.ToInt32(dataRow["RelayInterval"]);
			this.m_pRelay.RelayRetryInterval = Convert.ToInt32(dataRow["RelayRetryInterval"]);
			this.m_pRelay.SendUndeliveredWarningAfter = Convert.ToInt32(dataRow["RelayUndeliveredWarning"]);
			this.m_pRelay.SendUndeliveredAfter = Convert.ToInt32(dataRow["RelayUndelivered"]);
			this.m_pRelay.StoreUndeliveredMessages = Convert.ToBoolean(dataRow["StoreUndeliveredMessages"]);
			this.m_pRelay.UseTlsIfPossible = Convert.ToBoolean(dataRow["Relay_UseTlsIfPossible"]);
			this.m_pRelay.Binds = array5;
			this.m_pFetchMessages.Enabled = Convert.ToBoolean(dataRow["FetchPOP3_Enabled"]);
			this.m_pFetchMessages.FetchInterval = Convert.ToInt32(dataRow["FetchPOP3_Interval"]);
			this.m_pSIP.Enabled = Convert.ToBoolean(dataRow["SIP_Enabled"]);
			this.m_pSIP.ProxyMode = (SIP_ProxyMode)Enum.Parse(typeof(SIP_ProxyMode), dataRow["SIP_ProxyMode"].ToString());
			this.m_pSIP.MinimumExpires = Convert.ToInt32(dataRow["SIP_MinExpires"]);
			IPBindInfo[] array6 = new IPBindInfo[dataSet.Tables["SIP_Bindings"].Rows.Count];
			for (int n = 0; n < dataSet.Tables["SIP_Bindings"].Rows.Count; n++)
			{
				DataRow dataRow8 = dataSet.Tables["SIP_Bindings"].Rows[n];
				byte[] cert5 = null;
				if (dataSet.Tables["SIP_Bindings"].Columns.Contains("SSL_Certificate") && dataRow8["SSL_Certificate"] != null && !dataRow8.IsNull("SSL_Certificate") && ((byte[])dataRow8["SSL_Certificate"]).Length > 0)
				{
					cert5 = (byte[])dataRow8["SSL_Certificate"];
				}
				array6[n] = new IPBindInfo(dataRow8["HostName"].ToString(), (BindInfoProtocol)Enum.Parse(typeof(BindInfoProtocol), dataRow8["Protocol"].ToString()), IPAddress.Parse(dataRow8["IP"].ToString()), Convert.ToInt32(dataRow8["Port"]), this.ParseSslMode(dataRow8["SSL"].ToString()), this.PaseCertificate(cert5));
			}
			this.m_pSIP.Binds = array6;
			SipGatewayCollection sipGatewayCollection = new SipGatewayCollection(this);
			foreach (DataRow dataRow9 in dataSet.Tables["SIP_Gateways"].Rows)
			{
				sipGatewayCollection.AddInternal(dataRow9["UriScheme"].ToString(), dataRow9["Transport"].ToString(), dataRow9["Host"].ToString(), Convert.ToInt32(dataRow9["Port"]), dataRow9["Realm"].ToString(), dataRow9["UserName"].ToString(), dataRow9["Password"].ToString());
			}
			this.m_pLogging.LogSMTP = ConvertEx.ToBoolean(dataRow["LogSMTPCmds"], false);
			this.m_pLogging.SmtpLogsPath = dataRow["SMTP_LogPath"].ToString();
			this.m_pLogging.LogPOP3 = ConvertEx.ToBoolean(dataRow["LogPOP3Cmds"], false);
			this.m_pLogging.Pop3LogsPath = dataRow["POP3_LogPath"].ToString();
			this.m_pLogging.LogIMAP = ConvertEx.ToBoolean(dataRow["LogIMAPCmds"], false);
			this.m_pLogging.ImapLogsPath = dataRow["IMAP_LogPath"].ToString();
			this.m_pLogging.LogRelay = ConvertEx.ToBoolean(dataRow["LogRelayCmds"], false);
			this.m_pLogging.RelayLogsPath = dataRow["Relay_LogPath"].ToString();
			this.m_pLogging.LogFetchMessages = ConvertEx.ToBoolean(dataRow["LogFetchPOP3Cmds"], false);
			this.m_pLogging.FetchMessagesLogsPath = dataRow["FetchPOP3_LogPath"].ToString();
			string bodyTextRft = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1033{\\fonttbl{\\f0\\fnil\\fcharset0 Verdana;}{\\f1\\fnil\\fcharset186 Verdana;}{\\f2\\fswiss\\fcharset186{\\*\\fname Arial;}Arial Baltic;}{\\f3\\fnil\\fcharset0 Microsoft Sans Serif;}}\r\n{\\colortbl ;\\red0\\green64\\blue128;\\red255\\green0\\blue0;\\red0\\green128\\blue0;}\r\n\\viewkind4\\uc1\\pard\\f0\\fs20 This e-mail is generated by the Server(\\cf1 <#relay.hostname>\\cf0 )  to notify you, \\par\r\n\\lang1061\\f1 that \\lang1033\\f0 your message to \\cf1 <#relay.to>\\cf0  dated \\b\\fs16 <#message.header[\"Date:\"]>\\b0  \\fs20 could not be sent at the first attempt.\\par\r\n\\par\r\nRecipient \\i <#relay.to>'\\i0 s Server (\\cf1 <#relay.session_hostname>\\cf0 ) returned the following response: \\cf2 <#relay.error>\\cf0\\par\r\n\\par\r\n\\par\r\nPlease note Server will attempt to deliver this message for \\b <#relay.undelivered_after>\\b0  hours.\\par\r\n\\par\r\n--------\\par\r\n\\par\r\nYour original message is attached to this e-mail (\\b data.eml\\b0 )\\par\r\n\\par\r\n\\fs16 The tracking number for this message is \\cf3 <#relay.session_messageid>\\cf0\\fs20\\par\r\n\\lang1061\\f2\\par\r\n\\pard\\lang1033\\f3\\fs17\\par\r\n}\r\n";
			ServerReturnMessage delayedDeliveryWarning = new ServerReturnMessage("Delayed delivery notice: <#message.header[\"Subject:\"]>", bodyTextRft);
			string bodyTextRft2 = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1033{\\fonttbl{\\f0\\fswiss\\fprq2\\fcharset0 Verdana;}{\\f1\\fswiss\\fprq2\\fcharset186 Verdana;}{\\f2\\fnil\\fcharset0 Verdana;}{\\f3\\fnil\\fcharset186 Verdana;}{\\f4\\fswiss\\fcharset0 Arial;}{\\f5\\fswiss\\fcharset186{\\*\\fname Arial;}Arial Baltic;}}\r\n{\\colortbl ;\\red0\\green64\\blue128;\\red255\\green0\\blue0;\\red0\\green128\\blue0;}\r\n\\viewkind4\\uc1\\pard\\f0\\fs20 Your message t\\lang1061\\f1 o \\cf1\\lang1033\\f2 <#relay.to>\\cf0\\f0 , dated \\b\\fs16 <#message.header[\"Date:\"]>\\b0\\fs20 , could not be delivered.\\par\r\n\\par\r\nRecipient \\i <#relay.to>'\\i0 s Server (\\cf1 <#relay.session_hostname>\\cf0 ) returned the following response: \\cf2 <#relay.error>\\cf0\\par\r\n\\par\r\n\\par\r\n\\b * Server will not attempt to deliver this message anymore\\b0 .\\par\r\n\\par\r\n--------\\par\r\n\\par\r\nYour original message is attached to this e-mail (\\b data.eml\\b0 )\\par\r\n\\par\r\n\\fs16 The tracking number for this message is \\cf3 <#relay.session_messageid>\\cf0\\fs20\\par\r\n\\lang1061\\f5\\par\r\n\\lang1033\\f2\\par\r\n}\r\n";
			ServerReturnMessage undelivered = new ServerReturnMessage("Undelivered notice: <#message.header[\"Subject:\"]>", bodyTextRft2);
			if (dataSet.Tables.Contains("ServerReturnMessages"))
			{
				foreach (DataRow dataRow10 in dataSet.Tables["ServerReturnMessages"].Rows)
				{
					if (dataRow10["MessageType"].ToString() == "delayed_delivery_warning")
					{
						delayedDeliveryWarning = new ServerReturnMessage(dataRow10["Subject"].ToString(), dataRow10["BodyTextRtf"].ToString());
					}
					else if (dataRow10["MessageType"].ToString() == "undelivered")
					{
						undelivered = new ServerReturnMessage(dataRow10["Subject"].ToString(), dataRow10["BodyTextRtf"].ToString());
					}
				}
			}
			this.m_pReturnMessages = new ServerReturnMessages(this, delayedDeliveryWarning, undelivered);
		}

		internal void SetValuesChanged()
		{
			this.m_ValuesChanged = true;
		}

		private void CreateSettingsSchema(DataSet ds)
		{
			if (!ds.Tables.Contains("Settings"))
			{
				ds.Tables.Add("Settings");
			}
			if (!ds.Tables["Settings"].Columns.Contains("ErrorFile"))
			{
				ds.Tables["Settings"].Columns.Add("ErrorFile").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("SMTP_Threads"))
			{
				ds.Tables["Settings"].Columns.Add("SMTP_Threads").DefaultValue = 100;
			}
			if (!ds.Tables["Settings"].Columns.Contains("POP3_Threads"))
			{
				ds.Tables["Settings"].Columns.Add("POP3_Threads").DefaultValue = 100;
			}
			if (!ds.Tables["Settings"].Columns.Contains("SmartHost"))
			{
				ds.Tables["Settings"].Columns.Add("SmartHost").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("SmartHostPort"))
			{
				ds.Tables["Settings"].Columns.Add("SmartHostPort", typeof(int)).DefaultValue = 25;
			}
			if (!ds.Tables["Settings"].Columns.Contains("UseSmartHost"))
			{
				ds.Tables["Settings"].Columns.Add("UseSmartHost").DefaultValue = false;
			}
			if (!ds.Tables["Settings"].Columns.Contains("Dns1"))
			{
				ds.Tables["Settings"].Columns.Add("Dns1").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("Dns2"))
			{
				ds.Tables["Settings"].Columns.Add("Dns2").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("LogServer"))
			{
				ds.Tables["Settings"].Columns.Add("LogServer").DefaultValue = false;
			}
			if (!ds.Tables["Settings"].Columns.Contains("LogSMTPCmds"))
			{
				ds.Tables["Settings"].Columns.Add("LogSMTPCmds").DefaultValue = false;
			}
			if (!ds.Tables["Settings"].Columns.Contains("LogPOP3Cmds"))
			{
				ds.Tables["Settings"].Columns.Add("LogPOP3Cmds").DefaultValue = false;
			}
			if (!ds.Tables["Settings"].Columns.Contains("SMTP_SessionIdleTimeOut"))
			{
				ds.Tables["Settings"].Columns.Add("SMTP_SessionIdleTimeOut").DefaultValue = 30;
			}
			if (!ds.Tables["Settings"].Columns.Contains("SMTP_CommandIdleTimeOut"))
			{
				ds.Tables["Settings"].Columns.Add("SMTP_CommandIdleTimeOut").DefaultValue = 30;
			}
			if (!ds.Tables["Settings"].Columns.Contains("SMTP_MaxBadCommands"))
			{
				ds.Tables["Settings"].Columns.Add("SMTP_MaxBadCommands").DefaultValue = 8;
			}
			if (!ds.Tables["Settings"].Columns.Contains("POP3_SessionIdleTimeOut"))
			{
				ds.Tables["Settings"].Columns.Add("POP3_SessionIdleTimeOut").DefaultValue = 30;
			}
			if (!ds.Tables["Settings"].Columns.Contains("POP3_CommandIdleTimeOut"))
			{
				ds.Tables["Settings"].Columns.Add("POP3_CommandIdleTimeOut").DefaultValue = 30;
			}
			if (!ds.Tables["Settings"].Columns.Contains("POP3_MaxBadCommands"))
			{
				ds.Tables["Settings"].Columns.Add("POP3_MaxBadCommands").DefaultValue = 8;
			}
			if (!ds.Tables["Settings"].Columns.Contains("MaxMessageSize"))
			{
				ds.Tables["Settings"].Columns.Add("MaxMessageSize").DefaultValue = 1000;
			}
			if (!ds.Tables["Settings"].Columns.Contains("MaxRecipients"))
			{
				ds.Tables["Settings"].Columns.Add("MaxRecipients").DefaultValue = 100;
			}
			if (!ds.Tables["Settings"].Columns.Contains("MaxRelayThreads"))
			{
				ds.Tables["Settings"].Columns.Add("MaxRelayThreads").DefaultValue = 100;
			}
			if (!ds.Tables["Settings"].Columns.Contains("RelayInterval"))
			{
				ds.Tables["Settings"].Columns.Add("RelayInterval").DefaultValue = 15;
			}
			if (!ds.Tables["Settings"].Columns.Contains("RelayRetryInterval"))
			{
				ds.Tables["Settings"].Columns.Add("RelayRetryInterval").DefaultValue = 300;
			}
			if (!ds.Tables["Settings"].Columns.Contains("RelayUndeliveredWarning"))
			{
				ds.Tables["Settings"].Columns.Add("RelayUndeliveredWarning").DefaultValue = 300;
			}
			if (!ds.Tables["Settings"].Columns.Contains("RelayUndelivered"))
			{
				ds.Tables["Settings"].Columns.Add("RelayUndelivered").DefaultValue = 1;
			}
			if (!ds.Tables["Settings"].Columns.Contains("StoreUndeliveredMessages"))
			{
				ds.Tables["Settings"].Columns.Add("StoreUndeliveredMessages").DefaultValue = false;
			}
			if (!ds.Tables["Settings"].Columns.Contains("SMTP_LogPath"))
			{
				ds.Tables["Settings"].Columns.Add("SMTP_LogPath").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("POP3_LogPath"))
			{
				ds.Tables["Settings"].Columns.Add("POP3_LogPath").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("Server_LogPath"))
			{
				ds.Tables["Settings"].Columns.Add("Server_LogPath").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("IMAP_LogPath"))
			{
				ds.Tables["Settings"].Columns.Add("IMAP_LogPath").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("IMAP_SessionIdleTimeOut"))
			{
				ds.Tables["Settings"].Columns.Add("IMAP_SessionIdleTimeOut").DefaultValue = 800;
			}
			if (!ds.Tables["Settings"].Columns.Contains("IMAP_CommandIdleTimeOut"))
			{
				ds.Tables["Settings"].Columns.Add("IMAP_CommandIdleTimeOut").DefaultValue = 60;
			}
			if (!ds.Tables["Settings"].Columns.Contains("IMAP_MaxBadCommands"))
			{
				ds.Tables["Settings"].Columns.Add("IMAP_MaxBadCommands").DefaultValue = 10;
			}
			if (!ds.Tables["Settings"].Columns.Contains("LogIMAPCmds"))
			{
				ds.Tables["Settings"].Columns.Add("LogIMAPCmds").DefaultValue = "false";
			}
			if (!ds.Tables["Settings"].Columns.Contains("IMAP_Threads"))
			{
				ds.Tables["Settings"].Columns.Add("IMAP_Threads").DefaultValue = 100;
			}
			if (!ds.Tables["Settings"].Columns.Contains("SMTP_Enabled"))
			{
				ds.Tables["Settings"].Columns.Add("SMTP_Enabled").DefaultValue = "false";
			}
			if (!ds.Tables["Settings"].Columns.Contains("POP3_Enabled"))
			{
				ds.Tables["Settings"].Columns.Add("POP3_Enabled").DefaultValue = "false";
			}
			if (!ds.Tables["Settings"].Columns.Contains("IMAP_Enabled"))
			{
				ds.Tables["Settings"].Columns.Add("IMAP_Enabled").DefaultValue = "false";
			}
			if (!ds.Tables["Settings"].Columns.Contains("SMTP_DefaultDomain"))
			{
				ds.Tables["Settings"].Columns.Add("SMTP_DefaultDomain").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("RelayStore"))
			{
				ds.Tables["Settings"].Columns.Add("RelayStore").DefaultValue = "c:\\MailStore\\";
			}
			if (!ds.Tables["Settings"].Columns.Contains("SMTP_HostName"))
			{
				ds.Tables["Settings"].Columns.Add("SMTP_HostName").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("POP3_HostName"))
			{
				ds.Tables["Settings"].Columns.Add("POP3_HostName").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("IMAP_HostName"))
			{
				ds.Tables["Settings"].Columns.Add("IMAP_HostName").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("LogRelayCmds"))
			{
				ds.Tables["Settings"].Columns.Add("LogRelayCmds").DefaultValue = false;
			}
			if (!ds.Tables["Settings"].Columns.Contains("Relay_LogPath"))
			{
				ds.Tables["Settings"].Columns.Add("Relay_LogPath").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("SmartHostUserName"))
			{
				ds.Tables["Settings"].Columns.Add("SmartHostUserName").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("SmartHostPassword"))
			{
				ds.Tables["Settings"].Columns.Add("SmartHostPassword").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("RelayLocalIP"))
			{
				ds.Tables["Settings"].Columns.Add("RelayLocalIP").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("LogFetchPOP3Cmds"))
			{
				ds.Tables["Settings"].Columns.Add("LogFetchPOP3Cmds").DefaultValue = "false";
			}
			if (!ds.Tables["Settings"].Columns.Contains("FetchPOP3_LogPath"))
			{
				ds.Tables["Settings"].Columns.Add("FetchPOP3_LogPath").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("SMTP_RequireAuth"))
			{
				ds.Tables["Settings"].Columns.Add("SMTP_RequireAuth").DefaultValue = "false";
			}
			if (!ds.Tables["Settings"].Columns.Contains("Relay_SmartHost_UseSSL"))
			{
				ds.Tables["Settings"].Columns.Add("Relay_SmartHost_UseSSL").DefaultValue = "false";
			}
			if (!ds.Tables["Settings"].Columns.Contains("FetchPOP3_Interval"))
			{
				ds.Tables["Settings"].Columns.Add("FetchPOP3_Interval").DefaultValue = 300;
			}
			if (!ds.Tables["Settings"].Columns.Contains("SMTP_GreetingText"))
			{
				ds.Tables["Settings"].Columns.Add("SMTP_GreetingText").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("POP3_GreetingText"))
			{
				ds.Tables["Settings"].Columns.Add("POP3_GreetingText").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("IMAP_GreetingText"))
			{
				ds.Tables["Settings"].Columns.Add("IMAP_GreetingText").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("ServerAuthenticationType"))
			{
				ds.Tables["Settings"].Columns.Add("ServerAuthenticationType").DefaultValue = 1;
			}
			if (!ds.Tables["Settings"].Columns.Contains("ServerAuthWinDomain"))
			{
				ds.Tables["Settings"].Columns.Add("ServerAuthWinDomain").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("FetchPop3_Enabled"))
			{
				ds.Tables["Settings"].Columns.Add("FetchPop3_Enabled").DefaultValue = "true";
			}
			if (!ds.Tables["Settings"].Columns.Contains("Relay_HostName"))
			{
				ds.Tables["Settings"].Columns.Add("Relay_HostName").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("Relay_SessionIdleTimeOut"))
			{
				ds.Tables["Settings"].Columns.Add("Relay_SessionIdleTimeOut").DefaultValue = "30";
			}
			if (!ds.Tables["Settings"].Columns.Contains("SMTP_MaxConnectionsPerIP"))
			{
				ds.Tables["Settings"].Columns.Add("SMTP_MaxConnectionsPerIP").DefaultValue = "0";
			}
			if (!ds.Tables["Settings"].Columns.Contains("SMTP_MaxTransactions"))
			{
				ds.Tables["Settings"].Columns.Add("SMTP_MaxTransactions").DefaultValue = "0";
			}
			if (!ds.Tables["Settings"].Columns.Contains("POP3_MaxConnectionsPerIP"))
			{
				ds.Tables["Settings"].Columns.Add("POP3_MaxConnectionsPerIP").DefaultValue = "0";
			}
			if (!ds.Tables["Settings"].Columns.Contains("IMAP_MaxConnectionsPerIP"))
			{
				ds.Tables["Settings"].Columns.Add("IMAP_MaxConnectionsPerIP").DefaultValue = "0";
			}
			if (!ds.Tables["Settings"].Columns.Contains("SIP_Enabled"))
			{
				ds.Tables["Settings"].Columns.Add("SIP_Enabled").DefaultValue = false;
			}
			if (!ds.Tables["Settings"].Columns.Contains("SIP_HostName"))
			{
				ds.Tables["Settings"].Columns.Add("SIP_HostName").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("Relay_MaxConnectionsPerIP"))
			{
				ds.Tables["Settings"].Columns.Add("Relay_MaxConnectionsPerIP").DefaultValue = "10";
			}
			if (!ds.Tables["Settings"].Columns.Contains("SIP_MinExpries"))
			{
				ds.Tables["Settings"].Columns.Add("SIP_MinExpires").DefaultValue = 60;
			}
			if (!ds.Tables["Settings"].Columns.Contains("Relay_Mode"))
			{
				ds.Tables["Settings"].Columns.Add("Relay_Mode").DefaultValue = "Dns";
			}
			if (!ds.Tables["Settings"].Columns.Contains("Relay_SmartHostsBalanceMode"))
			{
				ds.Tables["Settings"].Columns.Add("Relay_SmartHostsBalanceMode").DefaultValue = "LoadBalance";
			}
			if (!ds.Tables["Settings"].Columns.Contains("SIP_ProxyMode"))
			{
				ds.Tables["Settings"].Columns.Add("SIP_ProxyMode").DefaultValue = "Registrar,B2BUA";
			}
			if (!ds.Tables["Settings"].Columns.Contains("LdapServer"))
			{
				ds.Tables["Settings"].Columns.Add("LdapServer").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("LdapDN"))
			{
				ds.Tables["Settings"].Columns.Add("LdapDN").DefaultValue = "";
			}
			if (!ds.Tables["Settings"].Columns.Contains("SettingsDate"))
			{
				ds.Tables["Settings"].Columns.Add("SettingsDate", typeof(DateTime)).DefaultValue = DateTime.Now;
			}
			if (!ds.Tables["Settings"].Columns.Contains("Relay_UseTlsIfPossible"))
			{
				ds.Tables["Settings"].Columns.Add("Relay_UseTlsIfPossible").DefaultValue = false;
			}
			if (!ds.Tables.Contains("DnsServers"))
			{
				ds.Tables.Add("DnsServers");
			}
			if (!ds.Tables["DnsServers"].Columns.Contains("IP"))
			{
				ds.Tables["DnsServers"].Columns.Add("IP").DefaultValue = "";
			}
			if (!ds.Tables.Contains("SMTP_Bindings"))
			{
				ds.Tables.Add("SMTP_Bindings");
			}
			if (!ds.Tables["SMTP_Bindings"].Columns.Contains("HostName"))
			{
				ds.Tables["SMTP_Bindings"].Columns.Add("HostName").DefaultValue = "";
			}
			if (!ds.Tables["SMTP_Bindings"].Columns.Contains("IP"))
			{
				ds.Tables["SMTP_Bindings"].Columns.Add("IP").DefaultValue = "0.0.0.0";
			}
			if (!ds.Tables["SMTP_Bindings"].Columns.Contains("Port"))
			{
				ds.Tables["SMTP_Bindings"].Columns.Add("Port").DefaultValue = "25";
			}
			if (!ds.Tables["SMTP_Bindings"].Columns.Contains("SSL"))
			{
				ds.Tables["SMTP_Bindings"].Columns.Add("SSL").DefaultValue = "None";
			}
			if (!ds.Tables["SMTP_Bindings"].Columns.Contains("SSL_Certificate"))
			{
				ds.Tables["SMTP_Bindings"].Columns.Add("SSL_Certificate", typeof(byte[]));
			}
			if (!ds.Tables.Contains("Relay_Bindings"))
			{
				ds.Tables.Add("Relay_Bindings");
			}
			if (!ds.Tables["Relay_Bindings"].Columns.Contains("HostName"))
			{
				ds.Tables["Relay_Bindings"].Columns.Add("HostName").DefaultValue = "";
			}
			if (!ds.Tables["Relay_Bindings"].Columns.Contains("Protocol"))
			{
				ds.Tables["Relay_Bindings"].Columns.Add("Protocol").DefaultValue = "TCP";
			}
			if (!ds.Tables["Relay_Bindings"].Columns.Contains("IP"))
			{
				ds.Tables["Relay_Bindings"].Columns.Add("IP").DefaultValue = "0.0.0.0";
			}
			if (!ds.Tables["Relay_Bindings"].Columns.Contains("Port"))
			{
				ds.Tables["Relay_Bindings"].Columns.Add("Port").DefaultValue = "25";
			}
			if (!ds.Tables["Relay_Bindings"].Columns.Contains("SSL"))
			{
				ds.Tables["Relay_Bindings"].Columns.Add("SSL", typeof(bool)).DefaultValue = false;
			}
			if (!ds.Tables["Relay_Bindings"].Columns.Contains("SSL_Certificate"))
			{
				ds.Tables["Relay_Bindings"].Columns.Add("SSL_Certificate", typeof(byte[]));
			}
			if (!ds.Tables.Contains("Relay_SmartHosts"))
			{
				ds.Tables.Add("Relay_SmartHosts");
			}
			if (!ds.Tables["Relay_SmartHosts"].Columns.Contains("Host"))
			{
				ds.Tables["Relay_SmartHosts"].Columns.Add("Host").DefaultValue = "";
			}
			if (!ds.Tables["Relay_SmartHosts"].Columns.Contains("Port"))
			{
				ds.Tables["Relay_SmartHosts"].Columns.Add("Port").DefaultValue = "25";
			}
			if (!ds.Tables["Relay_SmartHosts"].Columns.Contains("SslMode"))
			{
				ds.Tables["Relay_SmartHosts"].Columns.Add("SslMode").DefaultValue = "None";
			}
			if (!ds.Tables["Relay_SmartHosts"].Columns.Contains("UserName"))
			{
				ds.Tables["Relay_SmartHosts"].Columns.Add("UserName").DefaultValue = "";
			}
			if (!ds.Tables["Relay_SmartHosts"].Columns.Contains("Password"))
			{
				ds.Tables["Relay_SmartHosts"].Columns.Add("Password").DefaultValue = "";
			}
			if (!ds.Tables.Contains("POP3_Bindings"))
			{
				ds.Tables.Add("POP3_Bindings");
			}
			if (!ds.Tables["POP3_Bindings"].Columns.Contains("HostName"))
			{
				ds.Tables["POP3_Bindings"].Columns.Add("HostName").DefaultValue = "";
			}
			if (!ds.Tables["POP3_Bindings"].Columns.Contains("IP"))
			{
				ds.Tables["POP3_Bindings"].Columns.Add("IP").DefaultValue = "0.0.0.0";
			}
			if (!ds.Tables["POP3_Bindings"].Columns.Contains("Port"))
			{
				ds.Tables["POP3_Bindings"].Columns.Add("Port").DefaultValue = "110";
			}
			if (!ds.Tables["POP3_Bindings"].Columns.Contains("SSL"))
			{
				ds.Tables["POP3_Bindings"].Columns.Add("SSL").DefaultValue = "None";
			}
			if (!ds.Tables["POP3_Bindings"].Columns.Contains("SSL_Certificate"))
			{
				ds.Tables["POP3_Bindings"].Columns.Add("SSL_Certificate", typeof(byte[]));
			}
			if (!ds.Tables.Contains("IMAP_Bindings"))
			{
				ds.Tables.Add("IMAP_Bindings");
			}
			if (!ds.Tables["IMAP_Bindings"].Columns.Contains("HostName"))
			{
				ds.Tables["IMAP_Bindings"].Columns.Add("HostName").DefaultValue = "";
			}
			if (!ds.Tables["IMAP_Bindings"].Columns.Contains("IP"))
			{
				ds.Tables["IMAP_Bindings"].Columns.Add("IP").DefaultValue = "0.0.0.0";
			}
			if (!ds.Tables["IMAP_Bindings"].Columns.Contains("Port"))
			{
				ds.Tables["IMAP_Bindings"].Columns.Add("Port").DefaultValue = "143";
			}
			if (!ds.Tables["IMAP_Bindings"].Columns.Contains("SSL"))
			{
				ds.Tables["IMAP_Bindings"].Columns.Add("SSL").DefaultValue = "None";
			}
			if (!ds.Tables["IMAP_Bindings"].Columns.Contains("SSL_Certificate"))
			{
				ds.Tables["IMAP_Bindings"].Columns.Add("SSL_Certificate", typeof(byte[]));
			}
			if (!ds.Tables.Contains("SIP_Bindings"))
			{
				ds.Tables.Add("SIP_Bindings");
			}
			if (!ds.Tables["SIP_Bindings"].Columns.Contains("HostName"))
			{
				ds.Tables["SIP_Bindings"].Columns.Add("HostName").DefaultValue = "";
			}
			if (!ds.Tables["SIP_Bindings"].Columns.Contains("Protocol"))
			{
				ds.Tables["SIP_Bindings"].Columns.Add("Protocol").DefaultValue = "UDP";
			}
			if (!ds.Tables["SIP_Bindings"].Columns.Contains("IP"))
			{
				ds.Tables["SIP_Bindings"].Columns.Add("IP").DefaultValue = "0.0.0.0";
			}
			if (!ds.Tables["SIP_Bindings"].Columns.Contains("Port"))
			{
				ds.Tables["SIP_Bindings"].Columns.Add("Port").DefaultValue = "5060";
			}
			if (!ds.Tables["SIP_Bindings"].Columns.Contains("SSL"))
			{
				ds.Tables["SIP_Bindings"].Columns.Add("SSL").DefaultValue = "None";
			}
			if (!ds.Tables["SIP_Bindings"].Columns.Contains("SSL_Certificate"))
			{
				ds.Tables["SIP_Bindings"].Columns.Add("SSL_Certificate", typeof(byte[]));
			}
			if (!ds.Tables.Contains("SIP_Gateways"))
			{
				ds.Tables.Add("SIP_Gateways");
			}
			if (!ds.Tables["SIP_Gateways"].Columns.Contains("UriScheme"))
			{
				ds.Tables["SIP_Gateways"].Columns.Add("UriScheme");
			}
			if (!ds.Tables["SIP_Gateways"].Columns.Contains("Transport"))
			{
				ds.Tables["SIP_Gateways"].Columns.Add("Transport");
			}
			if (!ds.Tables["SIP_Gateways"].Columns.Contains("Host"))
			{
				ds.Tables["SIP_Gateways"].Columns.Add("Host");
			}
			if (!ds.Tables["SIP_Gateways"].Columns.Contains("Port"))
			{
				ds.Tables["SIP_Gateways"].Columns.Add("Port");
			}
			if (!ds.Tables["SIP_Gateways"].Columns.Contains("Realm"))
			{
				ds.Tables["SIP_Gateways"].Columns.Add("Realm");
			}
			if (!ds.Tables["SIP_Gateways"].Columns.Contains("UserName"))
			{
				ds.Tables["SIP_Gateways"].Columns.Add("UserName");
			}
			if (!ds.Tables["SIP_Gateways"].Columns.Contains("Password"))
			{
				ds.Tables["SIP_Gateways"].Columns.Add("Password");
			}
			if (!ds.Tables.Contains("ServerReturnMessages"))
			{
				ds.Tables.Add("ServerReturnMessages");
			}
			if (!ds.Tables["ServerReturnMessages"].Columns.Contains("MessageType"))
			{
				ds.Tables["ServerReturnMessages"].Columns.Add("MessageType");
			}
			if (!ds.Tables["ServerReturnMessages"].Columns.Contains("Subject"))
			{
				ds.Tables["ServerReturnMessages"].Columns.Add("Subject");
			}
			if (!ds.Tables["ServerReturnMessages"].Columns.Contains("BodyTextRtf"))
			{
				ds.Tables["ServerReturnMessages"].Columns.Add("BodyTextRtf");
			}
			ds.Tables["Settings"].Rows.Add(ds.Tables["Settings"].NewRow());
			foreach (DataRow dataRow in ds.Tables["Settings"].Rows)
			{
				foreach (DataColumn dataColumn in ds.Tables["Settings"].Columns)
				{
					if (dataRow.IsNull(dataColumn.ColumnName))
					{
						dataRow[dataColumn.ColumnName] = dataColumn.DefaultValue;
					}
				}
			}
		}

		internal DataSet ToDataSet()
		{
			DataSet dataSet = new DataSet();
			this.CreateSettingsSchema(dataSet);
			DataRow dataRow = dataSet.Tables["Settings"].Rows[0];
			IPAddress[] pDnsServers = this.m_pDnsServers;
			for (int i = 0; i < pDnsServers.Length; i++)
			{
				IPAddress iPAddress = pDnsServers[i];
				DataRow dataRow2 = dataSet.Tables["DnsServers"].NewRow();
				dataRow2["IP"] = iPAddress.ToString();
				dataSet.Tables["DnsServers"].Rows.Add(dataRow2);
			}
			dataRow["ServerAuthenticationType"] = (int)this.Authentication.AuthenticationType;
			dataRow["ServerAuthWinDomain"] = this.Authentication.WinDomain;
			dataRow["LdapServer"] = this.Authentication.LdapServer;
			dataRow["LdapDN"] = this.Authentication.LdapDn;
			dataRow["SMTP_Enabled"] = this.SMTP.Enabled;
			dataRow["SMTP_GreetingText"] = this.SMTP.GreetingText;
			dataRow["SMTP_DefaultDomain"] = this.SMTP.DefaultDomain;
			dataRow["SMTP_SessionIdleTimeOut"] = this.SMTP.SessionIdleTimeOut;
			dataRow["SMTP_Threads"] = this.SMTP.MaximumConnections;
			dataRow["SMTP_MaxConnectionsPerIP"] = this.SMTP.MaximumConnectionsPerIP;
			dataRow["SMTP_MaxBadCommands"] = this.SMTP.MaximumBadCommands;
			dataRow["MaxRecipients"] = this.SMTP.MaximumRecipientsPerMessage;
			dataRow["MaxMessageSize"] = this.SMTP.MaximumMessageSize;
			dataRow["SMTP_MaxTransactions"] = this.SMTP.MaximumTransactions;
			dataRow["SMTP_RequireAuth"] = this.SMTP.RequireAuthentication;
			IPBindInfo[] binds = this.SMTP.Binds;
			for (int j = 0; j < binds.Length; j++)
			{
				IPBindInfo iPBindInfo = binds[j];
				DataRow dataRow3 = dataSet.Tables["SMTP_Bindings"].NewRow();
				dataRow3["HostName"] = iPBindInfo.HostName;
				dataRow3["IP"] = iPBindInfo.IP.ToString();
				dataRow3["Port"] = iPBindInfo.Port;
				dataRow3["SSL"] = iPBindInfo.SslMode;
				if (iPBindInfo.Certificate != null)
				{
					dataRow3["SSL_Certificate"] = iPBindInfo.Certificate.Export(X509ContentType.Pfx);
				}
				else
				{
					dataRow3["SSL_Certificate"] = DBNull.Value;
				}
				dataSet.Tables["SMTP_Bindings"].Rows.Add(dataRow3);
			}
			dataRow["POP3_Enabled"] = this.POP3.Enabled;
			dataRow["POP3_GreetingText"] = this.POP3.GreetingText;
			dataRow["POP3_SessionIdleTimeOut"] = this.POP3.SessionIdleTimeOut;
			dataRow["POP3_Threads"] = this.POP3.MaximumConnections;
			dataRow["POP3_MaxConnectionsPerIP"] = this.POP3.MaximumConnectionsPerIP;
			dataRow["POP3_MaxBadCommands"] = this.POP3.MaximumBadCommands;
			IPBindInfo[] binds2 = this.POP3.Binds;
			for (int k = 0; k < binds2.Length; k++)
			{
				IPBindInfo iPBindInfo2 = binds2[k];
				DataRow dataRow4 = dataSet.Tables["POP3_Bindings"].NewRow();
				dataRow4["HostName"] = iPBindInfo2.HostName;
				dataRow4["IP"] = iPBindInfo2.IP.ToString();
				dataRow4["Port"] = iPBindInfo2.Port;
				dataRow4["SSL"] = iPBindInfo2.SslMode;
				if (iPBindInfo2.Certificate != null)
				{
					dataRow4["SSL_Certificate"] = iPBindInfo2.Certificate.Export(X509ContentType.Pfx);
				}
				else
				{
					dataRow4["SSL_Certificate"] = DBNull.Value;
				}
				dataSet.Tables["POP3_Bindings"].Rows.Add(dataRow4);
			}
			dataRow["IMAP_Enabled"] = this.IMAP.Enabled;
			dataRow["IMAP_GreetingText"] = this.IMAP.GreetingText;
			dataRow["IMAP_SessionIdleTimeOut"] = this.IMAP.SessionIdleTimeOut;
			dataRow["IMAP_Threads"] = this.IMAP.MaximumConnections;
			dataRow["IMAP_MaxConnectionsPerIP"] = this.IMAP.MaximumConnectionsPerIP;
			dataRow["IMAP_MaxBadCommands"] = this.IMAP.MaximumBadCommands;
			IPBindInfo[] binds3 = this.IMAP.Binds;
			for (int l = 0; l < binds3.Length; l++)
			{
				IPBindInfo iPBindInfo3 = binds3[l];
				DataRow dataRow5 = dataSet.Tables["IMAP_Bindings"].NewRow();
				dataRow5["HostName"] = iPBindInfo3.HostName;
				dataRow5["IP"] = iPBindInfo3.IP.ToString();
				dataRow5["Port"] = iPBindInfo3.Port;
				dataRow5["SSL"] = iPBindInfo3.SslMode;
				if (iPBindInfo3.Certificate != null)
				{
					dataRow5["SSL_Certificate"] = iPBindInfo3.Certificate.Export(X509ContentType.Pfx);
				}
				else
				{
					dataRow5["SSL_Certificate"] = DBNull.Value;
				}
				dataSet.Tables["IMAP_Bindings"].Rows.Add(dataRow5);
			}
			dataRow["Relay_Mode"] = this.Relay.RelayMode.ToString();
			dataRow["Relay_SmartHostsBalanceMode"] = this.Relay.SmartHostsBalanceMode.ToString();
			dataRow["Relay_SessionIdleTimeOut"] = this.Relay.SessionIdleTimeOut;
			dataRow["MaxRelayThreads"] = this.Relay.MaximumConnections;
			dataRow["Relay_MaxConnectionsPerIP"] = this.Relay.MaximumConnectionsPerIP;
			dataRow["RelayInterval"] = this.Relay.RelayInterval;
			dataRow["RelayRetryInterval"] = this.Relay.RelayRetryInterval;
			dataRow["RelayUndeliveredWarning"] = this.Relay.SendUndeliveredWarningAfter;
			dataRow["RelayUndelivered"] = this.Relay.SendUndeliveredAfter;
			dataRow["StoreUndeliveredMessages"] = this.Relay.StoreUndeliveredMessages;
			dataRow["Relay_UseTlsIfPossible"] = this.Relay.UseTlsIfPossible;
			Relay_SmartHost[] smartHosts = this.Relay.SmartHosts;
			for (int m = 0; m < smartHosts.Length; m++)
			{
				Relay_SmartHost relay_SmartHost = smartHosts[m];
				DataRow dataRow6 = dataSet.Tables["Relay_SmartHosts"].NewRow();
				dataRow6["Host"] = relay_SmartHost.Host;
				dataRow6["Port"] = relay_SmartHost.Port.ToString();
				dataRow6["SslMode"] = relay_SmartHost.SslMode.ToString();
				dataRow6["UserName"] = relay_SmartHost.UserName;
				dataRow6["Password"] = relay_SmartHost.Password;
				dataSet.Tables["Relay_SmartHosts"].Rows.Add(dataRow6);
			}
			IPBindInfo[] binds4 = this.Relay.Binds;
			for (int n = 0; n < binds4.Length; n++)
			{
				IPBindInfo iPBindInfo4 = binds4[n];
				DataRow dataRow7 = dataSet.Tables["Relay_Bindings"].NewRow();
				dataRow7["HostName"] = iPBindInfo4.HostName;
				dataRow7["IP"] = iPBindInfo4.IP.ToString();
				dataRow7["Port"] = iPBindInfo4.Port;
				dataRow7["SSL"] = iPBindInfo4.SslMode;
				if (iPBindInfo4.Certificate != null)
				{
					dataRow7["SSL_Certificate"] = iPBindInfo4.Certificate.Export(X509ContentType.Pfx);
				}
				else
				{
					dataRow7["SSL_Certificate"] = DBNull.Value;
				}
				dataSet.Tables["Relay_Bindings"].Rows.Add(dataRow7);
			}
			dataRow["FetchPOP3_Enabled"] = this.FetchMessages.Enabled;
			dataRow["FetchPOP3_Interval"] = this.FetchMessages.FetchInterval;
			dataRow["SIP_Enabled"] = this.SIP.Enabled;
			dataRow["SIP_ProxyMode"] = this.SIP.ProxyMode.ToString();
			dataRow["SIP_MinExpires"] = this.SIP.MinimumExpires;
			IPBindInfo[] binds5 = this.SIP.Binds;
			for (int num = 0; num < binds5.Length; num++)
			{
				IPBindInfo iPBindInfo5 = binds5[num];
				DataRow dataRow8 = dataSet.Tables["SIP_Bindings"].NewRow();
				dataRow8["HostName"] = iPBindInfo5.HostName;
				dataRow8["Protocol"] = iPBindInfo5.Protocol;
				dataRow8["IP"] = iPBindInfo5.IP.ToString();
				dataRow8["Port"] = iPBindInfo5.Port;
				dataRow8["SSL"] = iPBindInfo5.SslMode;
				if (iPBindInfo5.Certificate != null)
				{
					dataRow8["SSL_Certificate"] = iPBindInfo5.Certificate.Export(X509ContentType.Pfx);
				}
				else
				{
					dataRow8["SSL_Certificate"] = DBNull.Value;
				}
				dataSet.Tables["SIP_Bindings"].Rows.Add(dataRow8);
			}
			foreach (SipGateway sipGateway in this.SIP.Gateways)
			{
				DataRow dataRow9 = dataSet.Tables["SIP_Gateways"].NewRow();
				dataRow9["UriScheme"] = sipGateway.UriScheme;
				dataRow9["Transport"] = sipGateway.Transport;
				dataRow9["Host"] = sipGateway.Host;
				dataRow9["Port"] = sipGateway.Port;
				dataRow9["Realm"] = sipGateway.Realm;
				dataRow9["UserName"] = sipGateway.UserName;
				dataRow9["Password"] = sipGateway.Password;
				dataSet.Tables["SIP_Gateways"].Rows.Add(dataRow9);
			}
			dataRow["LogSMTPCmds"] = this.Logging.LogSMTP;
			dataRow["SMTP_LogPath"] = this.Logging.SmtpLogsPath;
			dataRow["LogPOP3Cmds"] = this.Logging.LogPOP3;
			dataRow["POP3_LogPath"] = this.Logging.Pop3LogsPath;
			dataRow["LogIMAPCmds"] = this.Logging.LogIMAP;
			dataRow["IMAP_LogPath"] = this.Logging.ImapLogsPath;
			dataRow["LogRelayCmds"] = this.Logging.LogRelay;
			dataRow["Relay_LogPath"] = this.Logging.RelayLogsPath;
			dataRow["LogFetchPOP3Cmds"] = this.Logging.LogFetchMessages;
			dataRow["FetchPOP3_LogPath"] = this.Logging.FetchMessagesLogsPath;
			DataRow dataRow10 = dataSet.Tables["ServerReturnMessages"].NewRow();
			dataRow10["MessageType"] = "delayed_delivery_warning";
			dataRow10["Subject"] = this.m_pReturnMessages.DelayedDeliveryWarning.Subject;
			dataRow10["BodyTextRtf"] = this.m_pReturnMessages.DelayedDeliveryWarning.BodyTextRtf;
			dataSet.Tables["ServerReturnMessages"].Rows.Add(dataRow10);
			DataRow dataRow11 = dataSet.Tables["ServerReturnMessages"].NewRow();
			dataRow11["MessageType"] = "undelivered";
			dataRow11["Subject"] = this.m_pReturnMessages.Undelivered.Subject;
			dataRow11["BodyTextRtf"] = this.m_pReturnMessages.Undelivered.BodyTextRtf;
			dataSet.Tables["ServerReturnMessages"].Rows.Add(dataRow11);
			return dataSet;
		}

		private void Bind()
		{
			this.m_pVirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetSettings " + this.m_pVirtualServer.VirtualServerID);
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
			memoryStream.Position = 0L;
			DataSet dataSet = new DataSet();
			this.CreateSettingsSchema(dataSet);
			dataSet.Clear();
			dataSet.ReadXml(memoryStream);
			DataRow dataRow = dataSet.Tables["Settings"].Rows[0];
			List<IPAddress> list = new List<IPAddress>();
			foreach (DataRow dataRow2 in dataSet.Tables["DnsServers"].Rows)
			{
				list.Add(IPAddress.Parse(dataRow2["IP"].ToString()));
			}
			this.m_pDnsServers = list.ToArray();
			this.m_pAuth = new Auth_Settings(this, (ServerAuthenticationType_enum)Convert.ToInt32(dataRow["ServerAuthenticationType"]), dataRow["ServerAuthWinDomain"].ToString(), dataRow["LdapServer"].ToString(), dataRow["LdapDN"].ToString());
			IPBindInfo[] array = new IPBindInfo[dataSet.Tables["SMTP_Bindings"].Rows.Count];
			for (int i = 0; i < dataSet.Tables["SMTP_Bindings"].Rows.Count; i++)
			{
				DataRow dataRow3 = dataSet.Tables["SMTP_Bindings"].Rows[i];
				byte[] cert = null;
				if (dataSet.Tables["SMTP_Bindings"].Columns.Contains("SSL_Certificate") && dataRow3["SSL_Certificate"] != null && !dataRow3.IsNull("SSL_Certificate") && ((byte[])dataRow3["SSL_Certificate"]).Length > 0)
				{
					cert = (byte[])dataRow3["SSL_Certificate"];
				}
				array[i] = new IPBindInfo(dataRow3["HostName"].ToString(), BindInfoProtocol.TCP, IPAddress.Parse(dataRow3["IP"].ToString()), Convert.ToInt32(dataRow3["Port"]), this.ParseSslMode(dataRow3["SSL"].ToString()), this.PaseCertificate(cert));
			}
			this.m_pSMTP = new SmtpSettings(this, Convert.ToBoolean(dataRow["SMTP_Enabled"]), dataRow["SMTP_GreetingText"].ToString(), dataRow["SMTP_DefaultDomain"].ToString(), Convert.ToInt32(dataRow["SMTP_SessionIdleTimeOut"]), Convert.ToInt32(dataRow["SMTP_Threads"]), Convert.ToInt32(dataRow["SMTP_MaxConnectionsPerIP"]), Convert.ToInt32(dataRow["SMTP_MaxBadCommands"]), Convert.ToInt32(dataRow["MaxRecipients"]), Convert.ToInt32(dataRow["MaxMessageSize"]), Convert.ToInt32(dataRow["SMTP_MaxTransactions"]), Convert.ToBoolean(dataRow["SMTP_RequireAuth"]), array);
			IPBindInfo[] array2 = new IPBindInfo[dataSet.Tables["POP3_Bindings"].Rows.Count];
			for (int j = 0; j < dataSet.Tables["POP3_Bindings"].Rows.Count; j++)
			{
				DataRow dataRow4 = dataSet.Tables["POP3_Bindings"].Rows[j];
				byte[] cert2 = null;
				if (dataSet.Tables["POP3_Bindings"].Columns.Contains("SSL_Certificate") && dataRow4["SSL_Certificate"] != null && !dataRow4.IsNull("SSL_Certificate") && ((byte[])dataRow4["SSL_Certificate"]).Length > 0)
				{
					cert2 = (byte[])dataRow4["SSL_Certificate"];
				}
				array2[j] = new IPBindInfo(dataRow4["HostName"].ToString(), BindInfoProtocol.TCP, IPAddress.Parse(dataRow4["IP"].ToString()), Convert.ToInt32(dataRow4["Port"]), this.ParseSslMode(dataRow4["SSL"].ToString()), this.PaseCertificate(cert2));
			}
			this.m_pPOP3 = new POP3_Settings(this, Convert.ToBoolean(dataRow["POP3_Enabled"]), dataRow["POP3_GreetingText"].ToString(), Convert.ToInt32(dataRow["POP3_SessionIdleTimeOut"]), Convert.ToInt32(dataRow["POP3_Threads"]), Convert.ToInt32(dataRow["POP3_MaxConnectionsPerIP"]), Convert.ToInt32(dataRow["POP3_MaxBadCommands"]), array2);
			IPBindInfo[] array3 = new IPBindInfo[dataSet.Tables["IMAP_Bindings"].Rows.Count];
			for (int k = 0; k < dataSet.Tables["IMAP_Bindings"].Rows.Count; k++)
			{
				DataRow dataRow5 = dataSet.Tables["IMAP_Bindings"].Rows[k];
				byte[] cert3 = null;
				if (dataSet.Tables["IMAP_Bindings"].Columns.Contains("SSL_Certificate") && dataRow5["SSL_Certificate"] != null && !dataRow5.IsNull("SSL_Certificate") && ((byte[])dataRow5["SSL_Certificate"]).Length > 0)
				{
					cert3 = (byte[])dataRow5["SSL_Certificate"];
				}
				array3[k] = new IPBindInfo(dataRow5["HostName"].ToString(), BindInfoProtocol.TCP, IPAddress.Parse(dataRow5["IP"].ToString()), Convert.ToInt32(dataRow5["Port"]), this.ParseSslMode(dataRow5["SSL"].ToString()), this.PaseCertificate(cert3));
			}
			this.m_pIMAP = new IMAP_Settings(this, Convert.ToBoolean(dataRow["IMAP_Enabled"]), dataRow["IMAP_GreetingText"].ToString(), Convert.ToInt32(dataRow["IMAP_SessionIdleTimeOut"]), Convert.ToInt32(dataRow["IMAP_Threads"]), Convert.ToInt32(dataRow["IMAP_MaxConnectionsPerIP"]), Convert.ToInt32(dataRow["IMAP_MaxBadCommands"]), array3);
			Relay_SmartHost[] array4 = new Relay_SmartHost[dataSet.Tables["Relay_SmartHosts"].Rows.Count];
			for (int l = 0; l < array4.Length; l++)
			{
				DataRow dataRow6 = dataSet.Tables["Relay_SmartHosts"].Rows[l];
				array4[l] = new Relay_SmartHost(ConvertEx.ToString(dataRow6["Host"]), ConvertEx.ToInt32(dataRow6["Port"]), (SslMode)Enum.Parse(typeof(SslMode), dataRow6["SslMode"].ToString()), ConvertEx.ToString(dataRow6["UserName"]), ConvertEx.ToString(dataRow6["Password"]));
			}
			IPBindInfo[] array5 = new IPBindInfo[dataSet.Tables["Relay_Bindings"].Rows.Count];
			for (int m = 0; m < dataSet.Tables["Relay_Bindings"].Rows.Count; m++)
			{
				DataRow dataRow7 = dataSet.Tables["Relay_Bindings"].Rows[m];
				byte[] cert4 = null;
				if (dataSet.Tables["Relay_Bindings"].Columns.Contains("SSL_Certificate") && dataRow7["SSL_Certificate"] != null && !dataRow7.IsNull("SSL_Certificate") && ((byte[])dataRow7["SSL_Certificate"]).Length > 0)
				{
					cert4 = (byte[])dataRow7["SSL_Certificate"];
				}
				array5[m] = new IPBindInfo(dataRow7["HostName"].ToString(), BindInfoProtocol.TCP, IPAddress.Parse(dataRow7["IP"].ToString()), Convert.ToInt32(dataRow7["Port"]), this.ParseSslMode(dataRow7["SSL"].ToString()), this.PaseCertificate(cert4));
			}
			this.m_pRelay = new Relay_Settings(this, (Relay_Mode)Enum.Parse(typeof(Relay_Mode), dataRow["Relay_Mode"].ToString()), (BalanceMode)Enum.Parse(typeof(BalanceMode), dataRow["Relay_SmartHostsBalanceMode"].ToString()), array4, Convert.ToInt32(dataRow["Relay_SessionIdleTimeOut"]), Convert.ToInt32(dataRow["MaxRelayThreads"]), Convert.ToInt32(dataRow["Relay_MaxConnectionsPerIP"]), Convert.ToInt32(dataRow["RelayInterval"]), Convert.ToInt32(dataRow["RelayRetryInterval"]), Convert.ToInt32(dataRow["RelayUndeliveredWarning"]), Convert.ToInt32(dataRow["RelayUndelivered"]), Convert.ToBoolean(dataRow["StoreUndeliveredMessages"]), Convert.ToBoolean(dataRow["Relay_UseTlsIfPossible"]), array5);
			this.m_pFetchMessages = new FetchMessages_Settings(this, Convert.ToBoolean(dataRow["FetchPOP3_Enabled"]), Convert.ToInt32(dataRow["FetchPOP3_Interval"]));
			IPBindInfo[] array6 = new IPBindInfo[dataSet.Tables["SIP_Bindings"].Rows.Count];
			for (int n = 0; n < dataSet.Tables["SIP_Bindings"].Rows.Count; n++)
			{
				DataRow dataRow8 = dataSet.Tables["SIP_Bindings"].Rows[n];
				byte[] cert5 = null;
				if (dataSet.Tables["SIP_Bindings"].Columns.Contains("SSL_Certificate") && dataRow8["SSL_Certificate"] != null && !dataRow8.IsNull("SSL_Certificate") && ((byte[])dataRow8["SSL_Certificate"]).Length > 0)
				{
					cert5 = (byte[])dataRow8["SSL_Certificate"];
				}
				array6[n] = new IPBindInfo(dataRow8["HostName"].ToString(), (BindInfoProtocol)Enum.Parse(typeof(BindInfoProtocol), dataRow8["Protocol"].ToString()), IPAddress.Parse(dataRow8["IP"].ToString()), Convert.ToInt32(dataRow8["Port"]), this.ParseSslMode(dataRow8["SSL"].ToString()), this.PaseCertificate(cert5));
			}
			SipGatewayCollection sipGatewayCollection = new SipGatewayCollection(this);
			foreach (DataRow dataRow9 in dataSet.Tables["SIP_Gateways"].Rows)
			{
				sipGatewayCollection.AddInternal(dataRow9["UriScheme"].ToString(), dataRow9["Transport"].ToString(), dataRow9["Host"].ToString(), Convert.ToInt32(dataRow9["Port"]), dataRow9["Realm"].ToString(), dataRow9["UserName"].ToString(), dataRow9["Password"].ToString());
			}
			this.m_pSIP = new SipSettings(this, Convert.ToBoolean(dataRow["SIP_Enabled"]), (SIP_ProxyMode)Enum.Parse(typeof(SIP_ProxyMode), dataRow["SIP_ProxyMode"].ToString()), Convert.ToInt32(dataRow["SIP_MinExpires"]), array6, sipGatewayCollection);
			this.m_pLogging = new Logging_Settings(this, ConvertEx.ToBoolean(dataRow["LogSMTPCmds"], false), dataRow["SMTP_LogPath"].ToString(), ConvertEx.ToBoolean(dataRow["LogPOP3Cmds"], false), dataRow["POP3_LogPath"].ToString(), ConvertEx.ToBoolean(dataRow["LogIMAPCmds"], false), dataRow["IMAP_LogPath"].ToString(), ConvertEx.ToBoolean(dataRow["LogRelayCmds"], false), dataRow["Relay_LogPath"].ToString(), ConvertEx.ToBoolean(dataRow["LogFetchPOP3Cmds"], false), dataRow["FetchPOP3_LogPath"].ToString());
			string bodyTextRft = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1033{\\fonttbl{\\f0\\fnil\\fcharset0 Verdana;}{\\f1\\fnil\\fcharset186 Verdana;}{\\f2\\fswiss\\fcharset186{\\*\\fname Arial;}Arial Baltic;}{\\f3\\fnil\\fcharset0 Microsoft Sans Serif;}}\r\n{\\colortbl ;\\red0\\green64\\blue128;\\red255\\green0\\blue0;\\red0\\green128\\blue0;}\r\n\\viewkind4\\uc1\\pard\\f0\\fs20 This e-mail is generated by the Server(\\cf1 <#relay.hostname>\\cf0 )  to notify you, \\par\r\n\\lang1061\\f1 that \\lang1033\\f0 your message to \\cf1 <#relay.to>\\cf0  dated \\b\\fs16 <#message.header[\"Date:\"]>\\b0  \\fs20 could not be sent at the first attempt.\\par\r\n\\par\r\nRecipient \\i <#relay.to>'\\i0 s Server (\\cf1 <#relay.session_hostname>\\cf0 ) returned the following response: \\cf2 <#relay.error>\\cf0\\par\r\n\\par\r\n\\par\r\nPlease note Server will attempt to deliver this message for \\b <#relay.undelivered_after>\\b0  hours.\\par\r\n\\par\r\n--------\\par\r\n\\par\r\nYour original message is attached to this e-mail (\\b data.eml\\b0 )\\par\r\n\\par\r\n\\fs16 The tracking number for this message is \\cf3 <#relay.session_messageid>\\cf0\\fs20\\par\r\n\\lang1061\\f2\\par\r\n\\pard\\lang1033\\f3\\fs17\\par\r\n}\r\n";
			ServerReturnMessage delayedDeliveryWarning = new ServerReturnMessage("Delayed delivery notice: <#message.header[\"Subject:\"]>", bodyTextRft);
			string bodyTextRft2 = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1033{\\fonttbl{\\f0\\fswiss\\fprq2\\fcharset0 Verdana;}{\\f1\\fswiss\\fprq2\\fcharset186 Verdana;}{\\f2\\fnil\\fcharset0 Verdana;}{\\f3\\fnil\\fcharset186 Verdana;}{\\f4\\fswiss\\fcharset0 Arial;}{\\f5\\fswiss\\fcharset186{\\*\\fname Arial;}Arial Baltic;}}\r\n{\\colortbl ;\\red0\\green64\\blue128;\\red255\\green0\\blue0;\\red0\\green128\\blue0;}\r\n\\viewkind4\\uc1\\pard\\f0\\fs20 Your message t\\lang1061\\f1 o \\cf1\\lang1033\\f2 <#relay.to>\\cf0\\f0 , dated \\b\\fs16 <#message.header[\"Date:\"]>\\b0\\fs20 , could not be delivered.\\par\r\n\\par\r\nRecipient \\i <#relay.to>'\\i0 s Server (\\cf1 <#relay.session_hostname>\\cf0 ) returned the following response: \\cf2 <#relay.error>\\cf0\\par\r\n\\par\r\n\\par\r\n\\b * Server will not attempt to deliver this message anymore\\b0 .\\par\r\n\\par\r\n--------\\par\r\n\\par\r\nYour original message is attached to this e-mail (\\b data.eml\\b0 )\\par\r\n\\par\r\n\\fs16 The tracking number for this message is \\cf3 <#relay.session_messageid>\\cf0\\fs20\\par\r\n\\lang1061\\f5\\par\r\n\\lang1033\\f2\\par\r\n}\r\n";
			ServerReturnMessage undelivered = new ServerReturnMessage("Undelivered notice: <#message.header[\"Subject:\"]>", bodyTextRft2);
			if (dataSet.Tables.Contains("ServerReturnMessages"))
			{
				foreach (DataRow dataRow10 in dataSet.Tables["ServerReturnMessages"].Rows)
				{
					if (dataRow10["MessageType"].ToString() == "delayed_delivery_warning")
					{
						delayedDeliveryWarning = new ServerReturnMessage(dataRow10["Subject"].ToString(), dataRow10["BodyTextRtf"].ToString());
					}
					else if (dataRow10["MessageType"].ToString() == "undelivered")
					{
						undelivered = new ServerReturnMessage(dataRow10["Subject"].ToString(), dataRow10["BodyTextRtf"].ToString());
					}
				}
			}
			this.m_pReturnMessages = new ServerReturnMessages(this, delayedDeliveryWarning, undelivered);
		}

		private X509Certificate2 PaseCertificate(byte[] cert)
		{
			if (cert == null)
			{
				return null;
			}
			return new X509Certificate2(cert, "", X509KeyStorageFlags.Exportable);
		}

		private SslMode ParseSslMode(string value)
		{
			if (value.ToLower() == "false")
			{
				return SslMode.None;
			}
			if (value.ToLower() == "true")
			{
				return SslMode.SSL;
			}
			return (SslMode)Enum.Parse(typeof(SslMode), value);
		}
	}
}
