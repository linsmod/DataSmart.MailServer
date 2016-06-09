
using System;
using System.ServiceProcess;
namespace DataSmart.MailServer
{
	public class MailServerService : ServiceBase
	{
		private Server m_pServer;

		public MailServerService()
		{
			this.m_pServer = new Server();
			base.ServiceName = "DataSmart Mail Server";
		}

		protected override void OnStart(string[] args)
		{
			this.m_pServer.Start();
		}

		protected override void OnStop()
		{
			this.m_pServer.Stop();
		}
	}
}
