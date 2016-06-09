using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

namespace DataSmart.MailServer
{
	internal class Error
	{
		private static string m_Path = "";

		public static string ErrorFilePath
		{
			get
			{
				return Error.m_Path;
			}
			set
			{
				Error.m_Path = value;
			}
		}

		public static void DumpError(Exception x)
		{
			Error.DumpError("", x, new StackTrace());
		}

		public static void DumpError(string virtualServer, Exception x)
		{
			Error.DumpError("", x, new StackTrace());
		}

		public static void DumpError(Exception x, StackTrace stackTrace)
		{
			Error.DumpError("", x, stackTrace);
		}

		public static void DumpError(string virtualServer, Exception x, StackTrace stackTrace)
		{
			try
			{
				string text = stackTrace.GetFrame(0).GetMethod().DeclaringType.FullName + "." + stackTrace.GetFrame(0).GetMethod().Name + "()";
				string text2 = x.Message + "\r\n";
				string text3 = text2;
				text2 = string.Concat(new string[]
				{
					text3,
					"//------------- function: ",
					text,
					"  ",
					DateTime.Now.ToString(),
					"------------//\r\n"
				});
				text2 = text2 + new StackTrace().ToString() + "\r\n";
				text2 += "//--- Excetption info: -------------------------------------------------\r\n";
				text2 = text2 + x.ToString() + "\r\n";
				if (x is SqlException)
				{
					SqlException ex = (SqlException)x;
					text2 += "\r\n\r\nSql errors:\r\n";
					foreach (SqlError sqlError in ex.Errors)
					{
						text2 += "\n";
						string text4 = text2;
						text2 = string.Concat(new string[]
						{
							text4,
							"Procedure: '",
							sqlError.Procedure,
							"'  line: ",
							sqlError.LineNumber.ToString(),
							"  error: ",
							sqlError.Number.ToString(),
							"\r\n"
						});
						text2 = text2 + "Message: " + sqlError.Message + "\r\n";
					}
				}
				if (x.InnerException != null)
				{
					text2 = text2 + "\r\n\r\n//------------- Innner Exception ----------\r\n" + x.Message + "\r\n";
					text2 += x.InnerException.ToString();
				}
				Error.DumpError(virtualServer, text2);
			}
			catch
			{
			}
		}

		public static void DumpError(string virtualServer, string errorText)
		{
			try
			{
				DataSet dataSet = new DataSet("dsEvents");
				dataSet.Tables.Add("Events");
				dataSet.Tables["Events"].Columns.Add("ID");
				dataSet.Tables["Events"].Columns.Add("VirtualServer");
				dataSet.Tables["Events"].Columns.Add("CreateDate", typeof(DateTime));
				dataSet.Tables["Events"].Columns.Add("Type");
				dataSet.Tables["Events"].Columns.Add("Text");
				if (File.Exists(SCore.PathFix(Error.m_Path + "Settings\\Events.xml")))
				{
					dataSet.ReadXml(SCore.PathFix(Error.m_Path + "Settings\\Events.xml"));
				}
				DataRow dataRow = dataSet.Tables["Events"].NewRow();
				dataRow["ID"] = Guid.NewGuid().ToString();
				dataRow["VirtualServer"] = virtualServer;
				dataRow["CreateDate"] = DateTime.Now;
				dataRow["Type"] = 0;
				dataRow["Text"] = errorText;
				dataSet.Tables["Events"].Rows.Add(dataRow);
				dataSet.WriteXml(SCore.PathFix(Error.m_Path + "Settings\\Events.xml"));
			}
			catch
			{
			}
		}
	}
}
