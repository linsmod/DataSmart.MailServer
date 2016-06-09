using System.NetworkToolkit;
using System.NetworkToolkit.IO;
using System.NetworkToolkit.TCP;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Security;
using System.Timers;
using DataSmart.MailServer.Extensions;
using System.Windows.Forms;

namespace DataSmart.MailServer.Management
{
    public class Server : IDisposable
    {
        private Server() { }
        private bool m_Connected;

        private string m_Host = "";

        private string m_UserName = "";

        private TCP_Client m_pClient;

        private SessionCollection m_pSessions;

        private EventCollection m_pEvents;

        private VirtualServerCollection m_pVirtualServers;

        private object m_pLockSynchronizer;

        private System.Timers.Timer m_pTimerNoop;

        public Form connectForm;

        public bool Connected
        {
            get
            {
                return this.m_Connected;
            }
        }

        public string Host
        {
            get
            {
                return this.m_Host;
            }
        }

        public string UserName
        {
            get
            {
                return this.m_UserName;
            }
        }

        public IPAddress[] IPAddresses
        {
            get
            {
                if (!this.m_Connected)
                {
                    throw new Exception("You must connect first, before accessing this property !");
                }
                IPAddress[] result;
                lock (this.LockSynchronizer)
                {
                    this.TCP_Client.TcpStream.WriteLine("GetIPAddresses");
                    SmartStream.ReadLineAsyncOP readLineAsyncTask = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
                    this.TCP_Client.TcpStream.ReadLine(readLineAsyncTask, false);
                    string lineUtf = readLineAsyncTask.LineUtf8;
                    if (!lineUtf.ToUpper().StartsWith("+OK"))
                    {
                        throw new Exception(lineUtf);
                    }
                    int num = Convert.ToInt32(lineUtf.Split(new char[]
                    {
                        ' '
                    }, 2)[1]);
                    MemoryStream memoryStream = new MemoryStream();
                    this.TCP_Client.TcpStream.ReadFixedCount(memoryStream, (long)num);
                    DataSet dataSet = Utils.DecompressDataSet(memoryStream);
                    List<IPAddress> list = new List<IPAddress>();
                    if (dataSet.Tables.Contains("dsIPs"))
                    {
                        foreach (DataRow dataRow in dataSet.Tables["dsIPs"].Rows)
                        {
                            list.Add(IPAddress.Parse(dataRow["IP"].ToString()));
                        }
                    }
                    result = list.ToArray();
                }
                return result;
            }
        }

        public ServerInfo ServerInfo
        {
            get
            {
                if (!this.m_Connected)
                {
                    throw new Exception("You must connect first, before accessing this property !");
                }
                ServerInfo result;
                lock (this.LockSynchronizer)
                {
                    this.TCP_Client.TcpStream.WriteLine("GetServerInfo");
                    SmartStream.ReadLineAsyncOP readLineAsyncTask = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
                    this.TCP_Client.TcpStream.ReadLine(readLineAsyncTask, false);
                    string lineUtf = readLineAsyncTask.LineUtf8;
                    if (!lineUtf.ToUpper().StartsWith("+OK"))
                    {
                        throw new Exception(lineUtf);
                    }
                    int num = Convert.ToInt32(lineUtf.Split(new char[]
                    {
                        ' '
                    }, 2)[1]);
                    MemoryStream memoryStream = new MemoryStream();
                    this.TCP_Client.TcpStream.ReadFixedCount(memoryStream, (long)num);
                    DataSet dataSet = Utils.DecompressDataSet(memoryStream);
                    result = new ServerInfo(dataSet.Tables["ServerInfo"].Rows[0]["OS"].ToString(), dataSet.Tables["ServerInfo"].Rows[0]["MailServerVersion"].ToString(), Convert.ToInt32(dataSet.Tables["ServerInfo"].Rows[0]["MemoryUsage"]), Convert.ToInt32(dataSet.Tables["ServerInfo"].Rows[0]["CpuUsage"]), Convert.ToDateTime(dataSet.Tables["ServerStartTime"]), Convert.ToInt32(dataSet.Tables["ServerInfo"].Rows[0]["Read_KB_Sec"]), Convert.ToInt32(dataSet.Tables["ServerInfo"].Rows[0]["Write_KB_Sec"]), Convert.ToInt32(dataSet.Tables["ServerInfo"].Rows[0]["SmtpSessions"]), Convert.ToInt32(dataSet.Tables["ServerInfo"].Rows[0]["Pop3Sessions"]), Convert.ToInt32(dataSet.Tables["ServerInfo"].Rows[0]["ImapSessions"]), Convert.ToInt32(dataSet.Tables["ServerInfo"].Rows[0]["RelaySessions"]));
                }
                return result;
            }
        }

        public SessionCollection Sessions
        {
            get
            {
                if (!this.m_Connected)
                {
                    throw new Exception("You must connect first, before accessing this property !");
                }
                if (this.m_pSessions == null)
                {
                    this.m_pSessions = new SessionCollection(this);
                }
                return this.m_pSessions;
            }
        }

        public EventCollection Events
        {
            get
            {
                if (!this.m_Connected)
                {
                    throw new Exception("You must connect first, before accessing this property !");
                }
                if (this.m_pEvents == null)
                {
                    this.m_pEvents = new EventCollection(this);
                }
                return this.m_pEvents;
            }
        }

        public VirtualServerCollection VirtualServers
        {
            get
            {
                if (!this.m_Connected)
                {
                    throw new Exception("You must connect first, before accessing this property !");
                }
                if (this.m_pVirtualServers == null)
                {
                    this.m_pVirtualServers = new VirtualServerCollection(this);
                }
                return this.m_pVirtualServers;
            }
        }

        internal TCP_Client TCP_Client
        {
            get
            {
                return this.m_pClient;
            }
        }

        internal object LockSynchronizer
        {
            get
            {
                return this.m_pLockSynchronizer;
            }
        }

        public string ID
        {
            get;
            set;
        }

        public Server(string host, string userName, SecureString password)
        {
            this.m_pClient = new TCP_Client();
            this.m_pLockSynchronizer = this.m_pClient;
            this.m_Host = host;
            this.m_UserName = userName;
            this.Password = password;
        }

        public void Dispose()
        {
            this.Disconnect();
        }

        public SecureString Password { get; set; }

        public void Connect()
        {
            this.Connect(this.ID, this.Host, this.UserName, this.Password);
        }

        public void Connect(string id, string host, string userName, SecureString password)
        {
            if (this.m_Connected)
            {
                return;
            }
            this.m_pClient.Connect(host, 5252);
            string text = this.ReadLine();
            if (!text.StartsWith("+OK"))
            {
                this.m_pClient.Disconnect();
                throw new Exception(text);
            }
            this.m_pClient.TcpStream.WriteLine("LOGIN " + TextUtils.QuoteString(userName) + " " + TextUtils.QuoteString(password.ConvertToUnsecureString()));
            text = this.ReadLine();
            if (!text.StartsWith("+OK"))
            {
                this.m_pClient.Disconnect();
                this.m_Connected = false;
                throw new Exception(text);
            }
            this.m_Connected = true;
            this.ID = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;
            this.m_Host = host;
            this.m_UserName = userName;
            this.Password = password;
            this.m_pTimerNoop = new System.Timers.Timer(30000.0);
            this.m_pTimerNoop.AutoReset = true;
            this.m_pTimerNoop.Elapsed += new ElapsedEventHandler(this.m_pTimerNoop_Elapsed);
            this.m_pTimerNoop.Enabled = true;
        }

        public void Disconnect()
        {
            if (this.m_pClient.IsConnected)
            {
                this.m_pClient.Disconnect();
            }
            this.m_Connected = false;
            if (this.m_pTimerNoop != null)
            {
                this.m_pTimerNoop.Dispose();
                this.m_pTimerNoop = null;
            }
        }

        internal string ReadLine()
        {
            if (!this.TCP_Client.IsConnected)
            {
                throw new Exception("You must connect first");
            }
            SmartStream.ReadLineAsyncOP readLineAsyncTask = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
            this.TCP_Client.TcpStream.ReadLine(readLineAsyncTask, false);
            return readLineAsyncTask.LineUtf8;
        }

        private void m_pTimerNoop_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                lock (this.LockSynchronizer)
                {
                    if (this.TCP_Client.IsConnected)
                    {
                        this.m_pClient.TcpStream.WriteLine("NOOP");
                        SmartStream.ReadLineAsyncOP op = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
                        op.Completed += Op_Completed;

                        if (!this.m_pClient.TcpStream.ReadLine(op, false))
                        {
                            Op_Completed(op, new EventArgs<SmartStream.ReadLineAsyncOP>(op));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Disconnect();
                throw new Exception(ex.Message);
            }
        }

        private void Op_Completed(object sender, EventArgs<SmartStream.ReadLineAsyncOP> e)
        {
            if (e.Value.Error != null)
            {
                this.Disconnect();
                throw new Exception(e.Value.Error.Message);
            }
        }
    }
}
