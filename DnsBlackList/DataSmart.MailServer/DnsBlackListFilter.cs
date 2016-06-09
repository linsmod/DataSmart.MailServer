using DataSmart.MailServer.Filters;
using System.NetworkToolkit.DNS;
using System.NetworkToolkit.DNS.Client;
using System.NetworkToolkit.SMTP.Server;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace DataSmart.MailServer
{
	public class DnsBlackListFilter : ISmtpSenderFilter, ISettingsUI
	{
		public bool Filter(string from, IMailServerManagementApi api, SMTP_Session session, out string errorText)
		{
			errorText = null;
			bool result = true;
			if (session.IsAuthenticated || this.IsPrivateIP(session.RemoteEndPoint.Address))
			{
				return true;
			}
			try
			{
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add("General");
				dataSet.Tables["General"].Columns.Add("CheckHelo");
				dataSet.Tables["General"].Columns.Add("LogRejections");
				dataSet.Tables.Add("BlackListSettings");
				dataSet.Tables["BlackListSettings"].Columns.Add("ErrorText");
				dataSet.Tables.Add("BlackList");
				dataSet.Tables["BlackList"].Columns.Add("IP");
				dataSet.Tables.Add("Servers");
				dataSet.Tables["Servers"].Columns.Add("Cost");
				dataSet.Tables["Servers"].Columns.Add("Server");
				dataSet.Tables["Servers"].Columns.Add("DefaultRejectionText");
				dataSet.ReadXml(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\DnsBlackList.xml");
				bool flag = false;
				if (dataSet.Tables["General"].Rows.Count == 1)
				{
					if (Convert.ToBoolean(dataSet.Tables["General"].Rows[0]["CheckHelo"]))
					{
						DnsServerResponse dnsServerResponse = Dns_Client.Static.Query(session.EhloHost, DNS_QType.A);
						if (dnsServerResponse.ConnectionOk && dnsServerResponse.ResponseCode != DNS_RCode.SERVER_FAILURE)
						{
							bool flag2 = false;
							DNS_rr_A[] aRecords = dnsServerResponse.GetARecords();
							for (int i = 0; i < aRecords.Length; i++)
							{
								DNS_rr_A dNS_rr_A = aRecords[i];
								if (session.RemoteEndPoint.Address.Equals(dNS_rr_A.IP))
								{
									flag2 = true;
									break;
								}
							}
							if (!flag2)
							{
								errorText = "Not valid DNS EHLO/HELO name for your IP '" + session.EhloHost + "' !";
								bool result2 = false;
								return result2;
							}
						}
					}
					flag = ConvertEx.ToBoolean(dataSet.Tables["General"].Rows[0]["LogRejections"]);
				}
				foreach (DataRow dataRow in dataSet.Tables["BlackList"].Rows)
				{
					if (this.IsAstericMatch(dataRow["IP"].ToString(), session.RemoteEndPoint.Address.ToString()))
					{
						errorText = dataSet.Tables["BlackListSettings"].Rows[0]["ErrorText"].ToString();
						bool result2 = false;
						return result2;
					}
				}
				foreach (DataRow dataRow2 in dataSet.Tables["Servers"].Rows)
				{
					DnsServerResponse dnsServerResponse2 = Dns_Client.Static.Query(this.ReverseIP(session.RemoteEndPoint.Address) + "." + dataRow2["Server"].ToString(), DNS_QType.ANY);
					DNS_rr_A[] aRecords2 = dnsServerResponse2.GetARecords();
					if (aRecords2.Length > 0)
					{
						if (flag)
						{
							this.WriteFilterLog(string.Concat(new string[]
							{
								"Sender:",
								from,
								" IP:",
								session.RemoteEndPoint.Address.ToString(),
								" blocked\r\n"
							}));
						}
						errorText = dataRow2["DefaultRejectionText"].ToString();
						if (dnsServerResponse2.GetTXTRecords().Length > 0)
						{
							errorText = dnsServerResponse2.GetTXTRecords()[0].Text;
						}
						if (errorText == "")
						{
							errorText = "You are in '" + dataRow2["Server"].ToString() + "' rejection list !";
						}
						bool result2 = false;
						return result2;
					}
				}
			}
			catch
			{
			}
			return result;
		}

		public Form GetUI()
		{
			return new MainForm();
		}

		private bool IsAstericMatch(string pattern, string text)
		{
			pattern = pattern.ToLower();
			text = text.ToLower();
			if (pattern == "")
			{
				pattern = "*";
			}
			while (pattern.Length > 0)
			{
				if (pattern.StartsWith("*"))
				{
					if (pattern.IndexOf("*", 1) <= -1)
					{
						return text.EndsWith(pattern.Substring(1));
					}
					string text2 = pattern.Substring(1, pattern.IndexOf("*", 1) - 1);
					if (text.IndexOf(text2) == -1)
					{
						return false;
					}
					text = text.Substring(text.IndexOf(text2) + text2.Length);
					pattern = pattern.Substring(pattern.IndexOf("*", 1));
				}
				else
				{
					if (pattern.IndexOfAny(new char[]
					{
						'*'
					}) <= -1)
					{
						return text == pattern;
					}
					string text3 = pattern.Substring(0, pattern.IndexOfAny(new char[]
					{
						'*'
					}));
					if (!text.StartsWith(text3))
					{
						return false;
					}
					text = text.Substring(text.IndexOf(text3) + text3.Length);
					pattern = pattern.Substring(pattern.IndexOfAny(new char[]
					{
						'*'
					}));
				}
			}
			return true;
		}

		private string ReverseIP(IPAddress ip)
		{
			byte[] addressBytes = ip.GetAddressBytes();
			return string.Concat(new string[]
			{
				addressBytes[3].ToString(),
				".",
				addressBytes[2].ToString(),
				".",
				addressBytes[1].ToString(),
				".",
				addressBytes[0].ToString()
			});
		}

		private bool IsPrivateIP(IPAddress ip)
		{
			if (ip.AddressFamily == AddressFamily.InterNetwork)
			{
				byte[] addressBytes = ip.GetAddressBytes();
				if (addressBytes[0] == 192 && addressBytes[1] == 168)
				{
					return true;
				}
				if (addressBytes[0] == 172 && addressBytes[1] >= 16 && addressBytes[1] <= 31)
				{
					return true;
				}
				if (addressBytes[0] == 10)
				{
					return true;
				}
				if (addressBytes[0] == 169 && addressBytes[1] == 254)
				{
					return true;
				}
			}
			return false;
		}

		private void WriteFilterLog(string text)
		{
			try
			{
				using (FileStream fileStream = new FileStream(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\lsDNSBL_Filter_block.log", FileMode.OpenOrCreate))
				{
					fileStream.Seek(0L, SeekOrigin.End);
					byte[] bytes = Encoding.ASCII.GetBytes(text);
					fileStream.Write(bytes, 0, bytes.Length);
				}
			}
			catch
			{
			}
		}
	}
}
