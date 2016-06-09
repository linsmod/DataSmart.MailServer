using DataSmart.MailServer.Monitoring;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;

namespace DataSmart.MailServer
{
    public class Server : IDisposable
    {
        private string m_StartupPath = "";

        private bool m_Running;

        private Timer m_pSettingsTimer;

        private DateTime m_ServersFileDate = DateTime.MinValue;

        private MonitoringServer m_pManagementServer;

        private DateTime m_StartTime;

        private List<VirtualServer> m_pVirtualServers;
        public void WriteXmlSetting(DataSet dataSet, string fileName)
        {
            if (!fileName.EndsWith("xml", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".xml";
            }
            var path = Path.Combine(StartupPath, "Settings", fileName);
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            dataSet.WriteXml(path);
        }

        public void ReadXmlSetting(DataSet dataSet, string fileName, string tableName)
        {
            if (!fileName.EndsWith("xml", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".xml";
            }
            var path = Path.Combine(StartupPath, "Settings", fileName);
            if (File.Exists(path))
            {
                dataSet.ReadXml(path);
                foreach (DataRow dataRow in dataSet.Tables[tableName].Rows)
                {
                    foreach (DataColumn dataColumn in dataSet.Tables[tableName].Columns)
                    {
                        if (dataRow.IsNull(dataColumn))
                        {
                            dataRow[dataColumn] = dataColumn.DefaultValue;
                        }
                    }
                }
            }
            else
                dataSet.WriteXml(fileName);
        }

        public string StartupPath
        {
            get
            {
                return this.m_StartupPath;
            }
        }

        public bool Running
        {
            get
            {
                return this.m_Running;
            }
        }

        public DateTime StartTime
        {
            get
            {
                if (this.m_Running)
                {
                    return this.m_StartTime;
                }
                return DateTime.MinValue;
            }
        }

        public VirtualServer[] VirtualServers
        {
            get
            {
                return this.m_pVirtualServers.ToArray();
            }
        }

        public object[] Sessions
        {
            get
            {
                ArrayList arrayList = new ArrayList();
                foreach (VirtualServer current in this.m_pVirtualServers)
                {
                    if (current.Enabled)
                    {
                        if (current.SMTP_Server.IsRunning)
                        {
                            arrayList.AddRange(current.SMTP_Server.Sessions.ToArray());
                        }
                        if (current.POP3_Server.IsRunning)
                        {
                            arrayList.AddRange(current.POP3_Server.Sessions.ToArray());
                        }
                        if (current.IMAP_Server.IsRunning)
                        {
                            arrayList.AddRange(current.IMAP_Server.Sessions.ToArray());
                        }
                        if (current.RelayServer.IsRunning)
                        {
                            arrayList.AddRange(current.RelayServer.Sessions.ToArray());
                        }
                    }
                }
                arrayList.AddRange(this.m_pManagementServer.Sessions.ToArray());
                return arrayList.ToArray();
            }
        }

        public Server()
        {
            this.m_pVirtualServers = new List<VirtualServer>();
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);
            this.m_StartupPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar;
            Error.ErrorFilePath = this.m_StartupPath;
            this.m_pSettingsTimer = new Timer(10000.0);
            this.m_pSettingsTimer.AutoReset = true;
            this.m_pSettingsTimer.Elapsed += new ElapsedEventHandler(this.m_pSettingsTimer_Elapsed);
        }

        public void Dispose()
        {
            this.Stop();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Error.DumpError((Exception)e.ExceptionObject, new StackTrace());
        }

        private void m_pSettingsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.LoadVirtualServers();
        }

        public void Start()
        {
            if (this.m_Running)
            {
                return;
            }
            this.m_ServersFileDate = DateTime.MinValue;
            this.m_pSettingsTimer_Elapsed(this, null);
            this.m_pSettingsTimer.Enabled = true;
            this.m_pManagementServer = new MonitoringServer(this);
            this.m_pManagementServer.Start();
            this.m_Running = true;
            this.m_StartTime = DateTime.Now;
        }

        public void Stop()
        {
            if (!this.m_Running)
            {
                return;
            }
            this.m_pSettingsTimer.Enabled = false;
            foreach (VirtualServer current in this.m_pVirtualServers)
            {
                current.Stop();
            }
            this.m_pVirtualServers.Clear();
            this.m_pManagementServer.Dispose();
            this.m_pManagementServer = null;
            this.m_Running = false;
            Logger.WriteLog(this.m_StartupPath + "Logs\\Server\\server.log", "//---- Server stopped " + DateTime.Now);
        }

        internal IMailServerManagementApi LoadApi(string assembly, string typeName, string initString)
        {
            string path;
            if (File.Exists(SCore.PathFix(this.m_StartupPath + "\\" + assembly)))
            {
                path = SCore.PathFix(this.m_StartupPath + "\\" + assembly);
            }
            else
            {
                path = SCore.PathFix(assembly);
            }
            Assembly assembly2 = Assembly.LoadFile(path);
            Type type = assembly2.ExportedTypes.FirstOrDefault((Type x) => x.FullName == typeName && typeof(IMailServerManagementApi).IsAssignableFrom(x));
            if (type == null)
            {
                throw new Exception("cannot load type with name " + typeName);
            }
            return (IMailServerManagementApi)Activator.CreateInstance(type, new object[]
            {
                initString
            });
        }

        public DataSet GetDataSet(string tableName)
        {
            var connStrBuilder = new SQLiteConnectionStringBuilder();
            connStrBuilder.DataSource = "mailserver.db3";
            connStrBuilder.Pooling = false;
            var connString = connStrBuilder.ToString();
            DataSet ds = new DataSet();
            try
            {
                SQLiteConnection conn = new SQLiteConnection(connString);
                conn.Open();
                SQLiteDataAdapter command = new SQLiteDataAdapter("SELECT * FROM " + tableName, conn);
                command.Fill(ds, tableName);
                conn.Close();
                return ds;
            }
            catch (System.Data.SQLite.SQLiteException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        internal void LoadVirtualServers()
        {
            try
            {
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add("Servers");
                dataSet.Tables["Servers"].Columns.Add("ID");
                dataSet.Tables["Servers"].Columns.Add("Enabled");
                dataSet.Tables["Servers"].Columns.Add("Name");
                dataSet.Tables["Servers"].Columns.Add("API_assembly");
                dataSet.Tables["Servers"].Columns.Add("API_class");
                dataSet.Tables["Servers"].Columns.Add("API_initstring");
                this.ReadXmlSetting(dataSet, "LocalServers", "Servers");
                if (dataSet.Tables.Contains("Servers"))
                {
                    for (int i = 0; i < this.m_pVirtualServers.Count; i++)
                    {
                        VirtualServer virtualServer = this.m_pVirtualServers[i];
                        bool flag = false;
                        foreach (DataRow dataRow in dataSet.Tables["Servers"].Rows)
                        {
                            if (virtualServer.ID == dataRow["ID"].ToString())
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            virtualServer.Stop();
                            this.m_pVirtualServers.Remove(virtualServer);
                            i--;
                        }
                    }
                    foreach (DataRow dataRow2 in dataSet.Tables["Servers"].Rows)
                    {
                        bool flag2 = false;
                        foreach (VirtualServer current in this.m_pVirtualServers)
                        {
                            if (current.ID == dataRow2["ID"].ToString())
                            {
                                flag2 = true;
                                current.Enabled = ConvertEx.ToBoolean(dataRow2["Enabled"], true);
                                break;
                            }
                        }
                        if (!flag2)
                        {
                            string id = dataRow2["ID"].ToString();
                            string name = dataRow2["Name"].ToString();
                            string assembly = dataRow2["API_assembly"].ToString();
                            string typeName = dataRow2["API_class"].ToString();
                            string text = dataRow2["API_initstring"].ToString();
                            IMailServerManagementApi api = this.LoadApi(assembly, typeName, text);
                            VirtualServer virtualServer2 = new VirtualServer(this, id, name, text, api);
                            this.m_pVirtualServers.Add(virtualServer2);
                            virtualServer2.Enabled = ConvertEx.ToBoolean(dataRow2["Enabled"], true);
                        }
                    }
                }
            }
            catch (Exception x)
            {
                Error.DumpError(x, new StackTrace());
            }
        }
    }
}
