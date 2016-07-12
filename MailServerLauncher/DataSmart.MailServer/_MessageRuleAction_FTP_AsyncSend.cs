using System.NetworkToolkit.FTP.Client;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace DataSmart.MailServer
{
	internal class _MessageRuleAction_FTP_AsyncSend
	{
		private string m_Server = "";

		private int m_Port = 21;

		private string m_User = "";

		private string m_Password = "";

		private string m_Folder = "";

		private Stream m_DataStream;

		private string m_FileName = "";

		public _MessageRuleAction_FTP_AsyncSend(string server, int port, string user, string password, string folder, Stream data, string fileName)
		{
			this.m_Server = server;
			this.m_Port = port;
			this.m_User = user;
			this.m_Password = password;
			this.m_Folder = folder;
			this.m_FileName = fileName;
			this.m_DataStream = new MemoryStream();
			SCore.StreamCopy(data, this.m_DataStream);
			this.m_DataStream.Position = 0L;
			Thread thread = new Thread(new ThreadStart(this.Send));
            thread.Name = "FTP Async Send Thread";
			thread.Start();
		}

		private void Send()
		{
			try
			{
				using (FTP_Client fTP_Client = new FTP_Client())
				{
					fTP_Client.Connect(this.m_Server, this.m_Port);
					fTP_Client.Authenticate(this.m_User, this.m_Password);
					fTP_Client.SetCurrentDir(this.m_Folder);
					fTP_Client.StoreFile(this.m_FileName, this.m_DataStream);
				}
			}
			catch (Exception x)
			{
				Error.DumpError(x);
			}
		}
	}
}
