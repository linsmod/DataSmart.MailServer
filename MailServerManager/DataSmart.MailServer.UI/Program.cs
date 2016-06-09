using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class Program
	{
		[STAThread]
		public static void Main()
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.CurrentDomain_UnhandledException);
			Application.ThreadException += new ThreadExceptionEventHandler(Program.Application_ThreadException);
			Application.EnableVisualStyles();
			Application.CurrentCulture = CultureInfo.InvariantCulture;
			Application.Run(new MainForm());
		}

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			ErrorForm errorForm = new ErrorForm(e.Exception, new StackTrace());
			errorForm.ShowDialog(null);
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			ErrorForm errorForm = new ErrorForm((Exception)e.ExceptionObject, new StackTrace());
			errorForm.ShowDialog(null);
		}
	}
}
