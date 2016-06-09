using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace DataSmart.MailServer.MailServer
{
	[RunInstaller(true)]
	public class ServiceInstaller : Installer
	{
		private Container components;

		private System.ServiceProcess.ServiceInstaller serviceInstaller;

		private ServiceProcessInstaller processInstaller;

		public ServiceInstaller()
		{
			this.InitializeComponent();
			this.processInstaller = new ServiceProcessInstaller();
			this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();
			this.processInstaller.Account = ServiceAccount.LocalSystem;
			this.serviceInstaller.StartType = ServiceStartMode.Automatic;
			this.serviceInstaller.ServiceName = "DataSmart Mail Server";
			base.Installers.Add(this.serviceInstaller);
			base.Installers.Add(this.processInstaller);
		}

		private void InitializeComponent()
		{
			this.components = new Container();
		}
	}
}
