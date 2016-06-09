using System;
using System.Threading;
using System.Windows.Forms;

namespace DataSmart.MailServer.AccessManager
{
	public class Program
	{
		[STAThread]
		public static void Main()
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.CurrentDomain_UnhandledException);
			Application.ThreadException += new ThreadExceptionEventHandler(Program.Application_ThreadException);
			Application.EnableVisualStyles();
			Application.Run(new MainForm());
		}

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			MessageBox.Show(null, e.Exception.ToString(), "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			MessageBox.Show(null, ((Exception)e.ExceptionObject).ToString(), "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}
}
