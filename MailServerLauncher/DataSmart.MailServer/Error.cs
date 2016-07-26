using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

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
            Error.DumpError("", x);
        }

        public static void DumpError(string virtualServer, Exception ex)
        {
            DumpError(virtualServer, GetExceptionMessage(ex));
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

        public static string GetExceptionMessage(Exception ex, int deep = 1)
        {
            var whiteSpaces = string.Join("----", Enumerable.Repeat("", deep - 1));
            var sb = new StringBuilder();
            sb.AppendLine(whiteSpaces + ex.Message);
            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                sb.AppendLine(whiteSpaces + ex.StackTrace);
            }
            if (ex.InnerException != null)
            {
                sb.AppendFormat(GetExceptionMessage(ex.InnerException, deep + 1));
            }
            return sb.ToString();
        }
    }
}
