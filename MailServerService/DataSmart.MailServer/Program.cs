using System;
using System.Configuration.Install;
using System.Security;
using System.ServiceProcess;
using System.Windows.Forms;

namespace DataSmart.MailServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				if (args.Length > 0 && args[0].ToLower() == "-install")
				{
					ManagedInstallerClass.InstallHelper(new string[]
					{
						"MailServerService.exe"
					});
					ServiceController serviceController = new ServiceController("DataSmart Mail Server");
					serviceController.Start();
				}
				else if (args.Length > 0 && args[0].ToLower() == "-uninstall")
				{
					ManagedInstallerClass.InstallHelper(new string[]
					{
						"/u",
						"MailServerService.exe"
					});
				}
				else
				{
					ServiceBase[] services = new ServiceBase[]
					{
						new MailServerService()
					};
					ServiceBase.Run(services);
				}
			}
			catch (Exception ex)
			{
				if (ex.InnerException is SecurityException)
				{
					MessageBox.Show("You need administrator rights to run this application, run this application 'Run as Administrator'.", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
				else
				{
					MessageBox.Show("Error: " + ex.ToString(), "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
				Environment.Exit(1);
			}
		}
	}
}
