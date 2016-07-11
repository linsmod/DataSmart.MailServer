using DataSmart.MailServer.UI;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace DataSmart.MailServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.CurrentDomain_UnhandledException);
			if (args.Length <= 0)
			{
				Application.ThreadException += new ThreadExceptionEventHandler(Program.Application_ThreadException);
				Application.EnableVisualStyles();
				Application.Run(new InstallAppForm());
				return;
			}
			if (args[0].ToLower() == "/?" || args[0].ToLower() == "/h")
			{
				string text = "";
				text += "Possible keys:\r\n";
				text += "\r\n";
				text += "\t -daemon, runs server as daemon application.\r\n";
				text += "\t -trayapp, runs server as Windows tray application.\r\n";
				text += "\t -winform, runs server in Windows Forms window.\r\n";
				MessageBox.Show(null, text, "Info:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return;
			}
			if (args[0].ToLower() == "-daemon")
			{
				new Server();
				while (true)
				{
					Thread.Sleep(1);
				}
			}
			else
			{
				if (args[0].ToLower() == "-trayapp")
				{
					Application.ThreadException += new ThreadExceptionEventHandler(Program.Application_ThreadException);
					Application.EnableVisualStyles();
					Application.Run(new TrayAppForm());
					return;
				}
				if (args[0].ToLower() == "-winform")
				{
					Application.ThreadException += new ThreadExceptionEventHandler(Program.Application_ThreadException);
					Application.EnableVisualStyles();
					Application.Run(new WinAppForm());
					return;
				}
				MessageBox.Show("Invalid command line argument was specified ! (try /? or /h for help)", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			MessageBox.Show("Error: " + ((Exception)e.ExceptionObject).ToString(), "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			MessageBox.Show("Error: " + e.Exception.ToString(), "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}
}
