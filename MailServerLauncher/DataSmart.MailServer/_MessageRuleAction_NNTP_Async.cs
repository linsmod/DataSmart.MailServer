using System.NetworkToolkit.NNTP.Client;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace DataSmart.MailServer
{
	internal class _MessageRuleAction_NNTP_Async
	{
		private string m_Server = "";

		private int m_Port = 119;

		private string m_Newsgroup = "";

		private MemoryStream m_pMessageStream;

		public _MessageRuleAction_NNTP_Async(string server, int port, string newsgroup, MemoryStream message)
		{
			this.m_Server = server;
			this.m_Port = port;
			this.m_Newsgroup = newsgroup;
			this.m_pMessageStream = message;
			Thread thread = new Thread(new ThreadStart(this.Post));
            thread.Name = "Message Rule Action NNTP Thread";
			thread.Start();
		}

		private void Post()
		{
			try
			{
				using (NNTP_Client nNTP_Client = new NNTP_Client())
				{
					nNTP_Client.Connect(this.m_Server, this.m_Port);
					nNTP_Client.PostMessage(this.m_Newsgroup, this.m_pMessageStream);
				}
			}
			catch (Exception x)
			{
				Error.DumpError(x);
			}
		}
	}
}
