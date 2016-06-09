using System.NetworkToolkit;
using System.NetworkToolkit.Mail;
using System.NetworkToolkit.MIME;
using System.NetworkToolkit.SMTP.Relay;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace DataSmart.MailServer.Relay
{
	public class RelayVariablesManager
	{
		private RelayServer m_pRelayServer;

		private Relay_Session m_pRelaySession;

		private string m_ErrorText = "";

		private Stream m_pMessageStream;

		private Mail_Message m_pMime;

		public RelayVariablesManager(RelayServer server, Relay_Session relaySession, string errorText, Mail_Message message)
		{
			this.m_pRelayServer = server;
			this.m_pRelaySession = relaySession;
			this.m_ErrorText = errorText;
			this.m_pMessageStream = relaySession.MessageStream;
			this.m_pMime = message;
		}

		public string Process(string text)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			int num2 = text.IndexOf("<#");
			int num3 = text.IndexOf(">");
			while (num2 > -1 && num3 > num2)
			{
				if (num < num2)
				{
					stringBuilder.Append(text.Substring(num, num2 - num));
				}
				stringBuilder.Append(this.ReplaceVariable(text.Substring(num2, num3 - num2 + 1)));
				num = num3 + 1;
				num2 = text.IndexOf("<#", num);
				num3 = text.IndexOf(">", num);
			}
			if (num < text.Length)
			{
				stringBuilder.Append(text.Substring(num));
			}
			return stringBuilder.ToString();
		}

		private string ReplaceVariable(string variable)
		{
			try
			{
				if (variable.StartsWith("<#sys.datetime"))
				{
					string result = DateTime.Now.ToString(TextUtils.UnQuoteString(variable.Substring(variable.IndexOf("(") + 1, variable.IndexOf(")") - variable.IndexOf("(") - 1)));
					return result;
				}
				if (variable == "<#relay.hostname>")
				{
					string result = Dns.GetHostName();
					return result;
				}
				if (variable == "<#relay.undelivered_after>")
				{
					string result = Convert.ToString(this.m_pRelayServer.UndeliveredAfter / 60);
					return result;
				}
				if (variable == "<#relay.error>")
				{
					string result = this.m_ErrorText;
					return result;
				}
				if (variable == "<#relay.session_id>")
				{
					string result = this.m_pRelaySession.ID;
					return result;
				}
				if (variable == "<#relay.session_messageid>")
				{
					string result = this.m_pRelaySession.MessageID;
					return result;
				}
				if (variable == "<#relay.session_hostname>")
				{
					try
					{
						string result = Dns.GetHostEntry(this.m_pRelaySession.RemoteEndPoint.Address).HostName;
						return result;
					}
					catch
					{
						string result = this.m_pRelaySession.RemoteEndPoint.Address.ToString();
						return result;
					}
				}
				if (variable == "<#relay.to>")
				{
					string result = this.m_pRelaySession.To;
					return result;
				}
				if (variable == "<#relay.from>")
				{
					string result = this.m_pRelaySession.From;
					return result;
				}
				if (variable == "<#message.bodytext>")
				{
					string bodyText = this.m_pMime.BodyText;
					if (bodyText != null)
					{
						string result = bodyText;
						return result;
					}
				}
				else if (variable.StartsWith("<#message.header"))
				{
					string text = TextUtils.UnQuoteString(variable.Substring(variable.IndexOf("[") + 1, variable.IndexOf("]") - variable.IndexOf("[") - 1)).Trim();
					if (text.EndsWith(":"))
					{
						text = text.Substring(0, text.Length - 1);
					}
					MIME_h first = this.m_pMime.Header.GetFirst(text);
					if (first != null)
					{
						string result = first.ToString().Split(new char[]
						{
							':'
						}, 2)[1].Trim();
						return result;
					}
				}
			}
			catch
			{
				string result = variable;
				return result;
			}
			return "";
		}
	}
}
