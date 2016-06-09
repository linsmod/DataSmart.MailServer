using DataSmart.MailServer.Filters;
using System.NetworkToolkit.SMTP.Server;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace DataSmart.MailServer
{
	public class VirusFilter : ISmtpMessageFilter, ISettingsUI
	{
		public FilterResult Filter(Stream messageStream, out Stream filteredStream, string sender, string[] recipients, IMailServerManagementApi api, SMTP_Session session, out string errorText)
		{
			errorText = null;
			filteredStream = null;
			string text = PathHelper.PathFix(Path.GetTempPath() + "\\" + Guid.NewGuid().ToString() + ".eml");
			try
			{
				using (FileStream fileStream = File.Create(text))
				{
					byte[] array = new byte[messageStream.Length];
					messageStream.Read(array, 0, array.Length);
					fileStream.Write(array, 0, array.Length);
				}
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add("Settings");
				dataSet.Tables["Settings"].Columns.Add("Program");
				dataSet.Tables["Settings"].Columns.Add("Arguments");
				dataSet.Tables["Settings"].Columns.Add("VirusExitCode");
				dataSet.ReadXml(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\VirusScan.xml");
				string fileName = dataSet.Tables["Settings"].Rows[0]["Program"].ToString();
				string arguments = dataSet.Tables["Settings"].Rows[0]["Arguments"].ToString().Replace("#FileName", text);
				int num = ConvertEx.ToInt32(dataSet.Tables["Settings"].Rows[0]["Program"], 1);
				int num2 = 0;
				Process process = Process.Start(new ProcessStartInfo(fileName, arguments)
				{
					CreateNoWindow = true,
					UseShellExecute = false
				});
				if (process != null)
				{
					process.WaitForExit(60000);
					num2 = process.ExitCode;
				}
				if (File.Exists(text))
				{
					using (FileStream fileStream2 = File.OpenRead(text))
					{
						byte[] array2 = new byte[fileStream2.Length];
						fileStream2.Read(array2, 0, array2.Length);
						filteredStream = new MemoryStream(array2);
					}
					File.Delete(text);
				}
				else
				{
					num = num2;
				}
				if (num == num2)
				{
					errorText = "Message is blocked, contains virus !";
					return FilterResult.Error;
				}
			}
			catch (Exception ex)
			{
				string arg_243_0 = ex.Message;
				filteredStream = messageStream;
			}
			finally
			{
				if (File.Exists(text))
				{
					File.Delete(text);
				}
			}
			return FilterResult.Store;
		}

		public Form GetUI()
		{
			return new MainForm();
		}
	}
}
