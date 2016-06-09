using System.NetworkToolkit.Mail;
using System.NetworkToolkit.MIME;
using System;
using System.Data;
using System.IO;
using System.Text;

namespace DataSmart.MailServer
{
	public class PathHelper
	{
		public static string NormalizeFolder(string folder)
		{
			folder = folder.Replace("\\", "/");
			while (folder.IndexOf("//") > -1)
			{
				folder = folder.Replace("//", "/");
			}
			if (folder.StartsWith("/"))
			{
				folder = folder.Substring(1);
			}
			if (folder.EndsWith("/"))
			{
				folder = folder.Substring(0, folder.Length - 1);
			}
			return folder;
		}

		public static string PathFix(string path)
		{
			return path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
		}

		public static string DirectoryExists(string dirName)
		{
			if (Environment.OSVersion.Platform.ToString().ToLower().IndexOf("win") > -1)
			{
				if (Directory.Exists(dirName))
				{
					return dirName;
				}
				return null;
			}
			else
			{
				if (Directory.Exists(dirName))
				{
					return dirName;
				}
				if (dirName.StartsWith("/"))
				{
					dirName = dirName.Substring(1);
				}
				string[] array = dirName.Split(new char[]
				{
					'/'
				});
				string text = "/";
				for (int i = 0; i < array.Length; i++)
				{
					bool flag = false;
					string[] directories = Directory.GetDirectories(text);
					string[] array2 = directories;
					for (int j = 0; j < array2.Length; j++)
					{
						string text2 = array2[j];
						string[] array3 = text2.Split(new char[]
						{
							'/'
						});
						if (array[i].ToLower() == array3[array3.Length - 1].ToLower())
						{
							text = text2;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						return null;
					}
				}
				return text;
			}
		}

		public static string EnsureFolder(string folder)
		{
			string text = PathHelper.DirectoryExists(folder);
			if (text == null)
			{
				Directory.CreateDirectory(folder);
				return folder;
			}
			return text;
		}

		public static string FileExists(string fileName)
		{
			if (Environment.OSVersion.Platform.ToString().ToLower().IndexOf("win") > -1)
			{
				if (File.Exists(fileName))
				{
					return fileName;
				}
			}
			else
			{
				if (File.Exists(fileName))
				{
					return fileName;
				}
				if (fileName.StartsWith("/"))
				{
					fileName = fileName.Substring(1);
				}
				string[] array = fileName.Split(new char[]
				{
					'/'
				});
				string path = "/";
				for (int i = 0; i < array.Length - 1; i++)
				{
					bool flag = false;
					string[] directories = Directory.GetDirectories(path);
					string[] array2 = directories;
					for (int j = 0; j < array2.Length; j++)
					{
						string text = array2[j];
						string[] array3 = text.Split(new char[]
						{
							'/'
						});
						if (array[i].ToLower() == array3[array3.Length - 1].ToLower())
						{
							path = text;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						return null;
					}
				}
				string[] files = Directory.GetFiles(path);
				string[] array4 = files;
				for (int k = 0; k < array4.Length; k++)
				{
					string text2 = array4[k];
					if (array[array.Length - 1].ToLower() == Path.GetFileName(text2).ToLower())
					{
						return text2;
					}
				}
			}
			return null;
		}

		[Obsolete("Use Net_utils.StreamCopy instead.")]
		public static long StreamCopy(Stream source, Stream destination)
		{
			byte[] array = new byte[16000];
			long num = 0L;
			while (true)
			{
				int num2 = source.Read(array, 0, array.Length);
				if (num2 == 0)
				{
					break;
				}
				num += (long)num2;
				destination.Write(array, 0, num2);
			}
			return num;
		}

		public static void CreateSettingsSchema(DataSet ds)
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
			if (!ds.Tables["Settings"].Columns.Contains("SMTP_MaxTransactions"))
			{
				ds.Tables["Settings"].Columns.Add("SMTP_MaxTransactions").DefaultValue = "0";
			}
			if (!ds.Tables["Settings"].Columns.Contains("RelayStore"))
			{
				ds.Tables["Settings"].Columns.Add("RelayStore").DefaultValue = "c:\\MailStore\\";
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
			if (!ds.Tables["Settings"].Columns.Contains("POP3_MaxConnectionsPerIP"))
			{
				ds.Tables["Settings"].Columns.Add("POP3_MaxConnectionsPerIP").DefaultValue = "0";
			}
			if (!ds.Tables["Settings"].Columns.Contains("IMAP_MaxConnectionsPerIP"))
			{
				ds.Tables["Settings"].Columns.Add("IMAP_MaxConnectionsPerIP").DefaultValue = "0";
			}
			if (!ds.Tables["Settings"].Columns.Contains("SmartHostUseSSL"))
			{
				ds.Tables["Settings"].Columns.Add("SmartHostUseSSL").DefaultValue = false;
			}
			if (!ds.Tables["Settings"].Columns.Contains("SIP_Enabled"))
			{
				ds.Tables["Settings"].Columns.Add("SIP_Enabled").DefaultValue = false;
			}
			if (!ds.Tables["Settings"].Columns.Contains("Relay_MaxConnectionsPerIP"))
			{
				ds.Tables["Settings"].Columns.Add("Relay_MaxConnectionsPerIP").DefaultValue = "10";
			}
			if (!ds.Tables["Settings"].Columns.Contains("SIP_MinExpires"))
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
			if (!ds.Tables["SMTP_Bindings"].Columns.Contains("Protocol"))
			{
				ds.Tables["SMTP_Bindings"].Columns.Add("Protocol").DefaultValue = "TCP";
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
			if (!ds.Tables["POP3_Bindings"].Columns.Contains("Protocol"))
			{
				ds.Tables["POP3_Bindings"].Columns.Add("Protocol").DefaultValue = "TCP";
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
			if (!ds.Tables["IMAP_Bindings"].Columns.Contains("Protocol"))
			{
				ds.Tables["IMAP_Bindings"].Columns.Add("Protocol").DefaultValue = "TCP";
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
		}

		public static Mail_Message GenerateBadMessage(Stream message)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			Mail_Message mail_Message = new Mail_Message();
			mail_Message.MimeVersion = "1.0";
			mail_Message.MessageID = MIME_Utils.CreateMessageID();
			mail_Message.Date = DateTime.Now;
			mail_Message.From = new Mail_t_MailboxList();
			mail_Message.From.Add(new Mail_t_Mailbox("system", "system"));
			mail_Message.To = new Mail_t_AddressList();
			mail_Message.To.Add(new Mail_t_Mailbox("system", "system"));
			mail_Message.Subject = "[BAD MESSAGE] Bad message, message parsing failed !";
			MIME_b_MultipartMixed mIME_b_MultipartMixed = new MIME_b_MultipartMixed(new MIME_h_ContentType(MIME_MediaTypes.Multipart.mixed)
			{
				Param_Boundary = Guid.NewGuid().ToString().Replace('-', '.')
			});
			mail_Message.Body = mIME_b_MultipartMixed;
			MIME_Entity mIME_Entity = new MIME_Entity();
			MIME_b_Text mIME_b_Text = new MIME_b_Text(MIME_MediaTypes.Text.plain);
			mIME_Entity.Body = mIME_b_Text;
			mIME_b_Text.SetText(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, "NOTE: Bad message, message parsing failed.\r\n\r\nOriginal message attached as 'data.eml'\r\n");
			mIME_b_MultipartMixed.BodyParts.Add(mIME_Entity);
			MIME_Entity mIME_Entity2 = new MIME_Entity();
			mIME_Entity2.ContentDisposition = new MIME_h_ContentDisposition("attachment");
			mIME_Entity2.ContentDisposition.Param_FileName = "data.eml";
			MIME_b_Application mIME_b_Application = new MIME_b_Application(MIME_MediaTypes.Application.octet_stream);
			mIME_Entity2.Body = mIME_b_Application;
			mIME_b_Application.SetData(message, "base64");
			mIME_b_MultipartMixed.BodyParts.Add(mIME_Entity2);
			return mail_Message;
		}
	}
}
