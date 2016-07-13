using DataSmart.MailServer.Relay;
using System.NetworkToolkit;
using System.NetworkToolkit.AUTH;
using System.NetworkToolkit.DNS.Client;
using System.NetworkToolkit.IMAP;
using System.NetworkToolkit.IMAP.Server;
using System.NetworkToolkit.IO;
using System.NetworkToolkit.Log;
using System.NetworkToolkit.Mail;
using System.NetworkToolkit.MIME;
using System.NetworkToolkit.POP3.Server;
using System.NetworkToolkit.SIP.Proxy;
using System.NetworkToolkit.SIP.Stack;
using System.NetworkToolkit.SMTP;
using System.NetworkToolkit.SMTP.Relay;
using System.NetworkToolkit.SMTP.Server;
using System.NetworkToolkit.TCP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices.Protocols;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Timers;

namespace DataSmart.MailServer
{
    public class VirtualServer
    {
        private Server m_pOwnerServer;

        private string m_ID = "";

        private string m_Name = "";

        private string m_ApiInitString = "";

        private IMailServerManagementApi m_pApi;

        private bool m_Running;

        private Dns_Client m_pDnsClient;

        private SMTP_Server m_pSMTP_Server;

        private POP3_Server m_pPop3Server;

        private IMAP_Server m_pImapServer;

        private RelayServer m_pRelayServer;

        private SIP_Proxy m_pSipServer;

        private FetchPop3 m_pFetchServer;

        private RecycleBinManager m_pRecycleBinManager;

        private BadLoginManager m_pBadLoginManager;

        private Timer m_pTimer;

        private DateTime m_SettingsDate;

        private string m_MailStorePath = "";

        private MailServerAuthType m_AuthType = MailServerAuthType.Integrated;

        private string m_Auth_Win_Domain = "";

        private string m_Auth_LDAP_Server = "";

        private string m_Auth_LDAP_DN = "";

        private bool m_SMTP_RequireAuth;

        private string m_SMTP_DefaultDomain = "";

        private string m_Server_LogPath = "";

        private string m_SMTP_LogPath = "";

        private string m_POP3_LogPath = "";

        private string m_IMAP_LogPath = "";

        private string m_Relay_LogPath = "";

        private string m_Fetch_LogPath = "";

        public string ID
        {
            get
            {
                return this.m_ID;
            }
        }

        public bool Enabled
        {
            get
            {
                return this.m_Running;
            }
            set
            {
                if (value)
                {
                    this.Start();
                    return;
                }
                this.Stop();
            }
        }

        public string Name
        {
            get
            {
                return this.m_Name;
            }
        }

        public IMailServerManagementApi InternalService
        {
            get
            {
                return this.m_pApi;
            }
        }

        public string MailStorePath
        {
            get
            {
                return this.m_MailStorePath;
            }
        }

        public Dns_Client DnsClient
        {
            get
            {
                return this.m_pDnsClient;
            }
        }

        public SMTP_Server SMTP_Server
        {
            get
            {
                return this.m_pSMTP_Server;
            }
        }

        public POP3_Server POP3_Server
        {
            get
            {
                return this.m_pPop3Server;
            }
        }

        public IMAP_Server IMAP_Server
        {
            get
            {
                return this.m_pImapServer;
            }
        }

        public RelayServer RelayServer
        {
            get
            {
                return this.m_pRelayServer;
            }
        }

        public SIP_Proxy SipServer
        {
            get
            {
                return this.m_pSipServer;
            }
        }

        internal string SMTP_LogsPath
        {
            get
            {
                return this.m_SMTP_LogPath;
            }
        }

        internal string POP3_LogsPath
        {
            get
            {
                return this.m_POP3_LogPath;
            }
        }

        internal string IMAP_LogsPath
        {
            get
            {
                return this.m_IMAP_LogPath;
            }
        }

        internal string RELAY_LogsPath
        {
            get
            {
                return this.m_Relay_LogPath;
            }
        }

        internal string FETCH_LogsPath
        {
            get
            {
                return this.m_Fetch_LogPath;
            }
        }

        public VirtualServer(Server server, string id, string name, string apiInitString, IMailServerManagementApi api)
        {
            this.m_pOwnerServer = server;
            this.m_ID = id;
            this.m_Name = name;
            this.m_ApiInitString = apiInitString;
            this.m_pApi = api;
        }

        private void m_pSMTP_Server_SessionCreated(object sender, TCP_ServerSessionEventArgs<SMTP_Session> e)
        {
            e.Session.Started += new EventHandler<SMTP_e_Started>(this.m_pSMTP_Server_Session_Started);
            e.Session.Ehlo += new EventHandler<SMTP_e_Ehlo>(this.m_pSMTP_Server_Session_Ehlo);
            e.Session.MailFrom += new EventHandler<SMTP_e_MailFrom>(this.m_pSMTP_Server_Session_MailFrom);
            e.Session.RcptTo += new EventHandler<SMTP_e_RcptTo>(this.m_pSMTP_Server_Session_RcptTo);
            e.Session.GetMessageStream += new EventHandler<SMTP_e_Message>(this.m_pSMTP_Server_Session_GetMessageStream);
            e.Session.MessageStoringCanceled += new EventHandler(this.m_pSMTP_Server_Session_MessageStoringCanceled);
            e.Session.MessageStoringCompleted += new EventHandler<SMTP_e_MessageStored>(this.m_pSMTP_Server_Session_MessageStoringCompleted);
            if (this.m_AuthType == MailServerAuthType.Windows || this.m_AuthType == MailServerAuthType.Ldap)
            {
                AUTH_SASL_ServerMechanism_Plain aUTH_SASL_ServerMechanism_Plain = new AUTH_SASL_ServerMechanism_Plain(false);
                aUTH_SASL_ServerMechanism_Plain.Authenticate += delegate (object s, AUTH_e_Authenticate e1)
                {
                    try
                    {
                        e1.IsAuthenticated = this.Authenticate(e.Session.RemoteEndPoint.Address, e1.UserName, e1.Password);
                    }
                    catch (Exception x)
                    {
                        this.OnError(x);
                        e1.IsAuthenticated = false;
                    }
                };
                e.Session.Authentications.Add(aUTH_SASL_ServerMechanism_Plain.Name, aUTH_SASL_ServerMechanism_Plain);
                AUTH_SASL_ServerMechanism_Login aUTH_SASL_ServerMechanism_Login = new AUTH_SASL_ServerMechanism_Login(false);
                aUTH_SASL_ServerMechanism_Login.Authenticate += delegate (object s, AUTH_e_Authenticate e1)
                {
                    try
                    {
                        e1.IsAuthenticated = this.Authenticate(e.Session.RemoteEndPoint.Address, e1.UserName, e1.Password);
                    }
                    catch (Exception x)
                    {
                        this.OnError(x);
                        e1.IsAuthenticated = false;
                    }
                };
                e.Session.Authentications.Add(aUTH_SASL_ServerMechanism_Login.Name, aUTH_SASL_ServerMechanism_Login);
                return;
            }
            AUTH_SASL_ServerMechanism_DigestMd5 aUTH_SASL_ServerMechanism_DigestMd = new AUTH_SASL_ServerMechanism_DigestMd5(false);
            aUTH_SASL_ServerMechanism_DigestMd.Realm = e.Session.LocalHostName;
            aUTH_SASL_ServerMechanism_DigestMd.GetUserInfo += delegate (object s, AUTH_e_UserInfo e1)
            {
                this.FillUserInfo(e1);
            };
            e.Session.Authentications.Add(aUTH_SASL_ServerMechanism_DigestMd.Name, aUTH_SASL_ServerMechanism_DigestMd);
            AUTH_SASL_ServerMechanism_CramMd5 aUTH_SASL_ServerMechanism_CramMd = new AUTH_SASL_ServerMechanism_CramMd5(false);
            aUTH_SASL_ServerMechanism_CramMd.GetUserInfo += delegate (object s, AUTH_e_UserInfo e1)
            {
                this.FillUserInfo(e1);
            };
            e.Session.Authentications.Add(aUTH_SASL_ServerMechanism_CramMd.Name, aUTH_SASL_ServerMechanism_CramMd);
            AUTH_SASL_ServerMechanism_Plain aUTH_SASL_ServerMechanism_Plain2 = new AUTH_SASL_ServerMechanism_Plain(false);
            aUTH_SASL_ServerMechanism_Plain2.Authenticate += delegate (object s, AUTH_e_Authenticate e1)
            {
                try
                {
                    e1.IsAuthenticated = this.Authenticate(e.Session.RemoteEndPoint.Address, e1.UserName, e1.Password);
                }
                catch (Exception x)
                {
                    this.OnError(x);
                    e1.IsAuthenticated = false;
                }
            };
            e.Session.Authentications.Add(aUTH_SASL_ServerMechanism_Plain2.Name, aUTH_SASL_ServerMechanism_Plain2);
            AUTH_SASL_ServerMechanism_Login aUTH_SASL_ServerMechanism_Login2 = new AUTH_SASL_ServerMechanism_Login(false);
            aUTH_SASL_ServerMechanism_Login2.Authenticate += delegate (object s, AUTH_e_Authenticate e1)
            {
                try
                {
                    e1.IsAuthenticated = this.Authenticate(e.Session.RemoteEndPoint.Address, e1.UserName, e1.Password);
                }
                catch (Exception x)
                {
                    this.OnError(x);
                    e1.IsAuthenticated = false;
                }
            };
            e.Session.Authentications.Add(aUTH_SASL_ServerMechanism_Login2.Name, aUTH_SASL_ServerMechanism_Login2);
        }

        private void m_pSMTP_Server_Session_Started(object sender, SMTP_e_Started e)
        {
            if (!this.IsAccessAllowed(ServiceType.SMTP, e.Session.RemoteEndPoint.Address))
            {
                e.Reply = new SMTP_Reply(554, "Your IP address is blocked.");
            }
        }

        private void m_pSMTP_Server_Session_Ehlo(object sender, SMTP_e_Ehlo e)
        {
        }

        private void m_pSMTP_Server_Session_MailFrom(object sender, SMTP_e_MailFrom e)
        {
            if (this.m_SMTP_RequireAuth && !e.Session.IsAuthenticated)
            {
                e.Reply = new SMTP_Reply(530, "5.7.0  Authentication required.");
                return;
            }
            if (e.MailFrom.Mailbox.IndexOf('@') != -1 && e.MailFrom.Mailbox.Substring(e.MailFrom.Mailbox.IndexOf('@') + 1).Length < 1)
            {
                e.Reply = new SMTP_Reply(501, "MAIL FROM: address(" + e.MailFrom + ") domain name must be specified.");
                return;
            }
            try
            {
                DataView filters = this.m_pApi.GetFilters();
                filters.RowFilter = "Enabled=true AND Type='ISmtpSenderFilter'";
                filters.Sort = "Cost";
                foreach (DataRowView dataRowView in filters)
                {
                    string text = dataRowView.Row["Assembly"].ToString();
                    if (!File.Exists(text))
                    {
                        text = Path.Combine(this.m_pOwnerServer.StartupPath, text);
                    }
                    Assembly assembly = Assembly.LoadFrom(text);
                    Type type = assembly.ExportedTypes.FirstOrDefault((Type x) => typeof(ISmtpSenderFilter).IsAssignableFrom(x) && x.FullName == dataRowView["ClassName"].ToString());
                    object obj = Activator.CreateInstance(type);
                    ISmtpSenderFilter smtpSenderFilter = (ISmtpSenderFilter)obj;
                    string text2 = null;
                    if (!smtpSenderFilter.Filter(e.MailFrom.Mailbox, this.m_pApi, e.Session, out text2))
                    {
                        if (text2 != null)
                        {
                            e.Reply = new SMTP_Reply(550, text2);
                        }
                        else
                        {
                            e.Reply = new SMTP_Reply(550, "Sender rejected.");
                        }
                        break;
                    }
                }
            }
            catch (Exception x)
            {
                e.Reply = new SMTP_Reply(500, "Internal server error.");
                Error.DumpError(this.Name, x);
            }
        }

        private void m_pSMTP_Server_Session_RcptTo(object sender, SMTP_e_RcptTo e)
        {
            try
            {
                string text = e.RcptTo.Mailbox;
                if (text.IndexOf("@") == -1)
                {
                    text = text + "@" + this.m_SMTP_DefaultDomain;
                }
                if (this.m_pApi.DomainExists(text))
                {
                    string text2 = this.m_pApi.MapUser(text);
                    if (text2 == null)
                    {
                        e.Reply = new SMTP_Reply(550, "No such user here.");
                        if (this.m_pApi.MailingListExists(text))
                        {
                            if (!e.Session.IsAuthenticated)
                            {
                                if (this.m_pApi.CanAccessMailingList(text, "anyone"))
                                {
                                    e.Reply = new SMTP_Reply(250, "OK.");
                                    goto IL_25C;
                                }
                                goto IL_25C;
                            }
                            else
                            {
                                if (this.m_pApi.CanAccessMailingList(text, e.Session.AuthenticatedUserIdentity.Name))
                                {
                                    e.Reply = new SMTP_Reply(250, "OK.");
                                    goto IL_25C;
                                }
                                goto IL_25C;
                            }
                        }
                        else
                        {
                            DataView routes = this.m_pApi.GetRoutes();
                            IEnumerator enumerator = routes.GetEnumerator();
                            try
                            {
                                while (enumerator.MoveNext())
                                {
                                    DataRowView dataRowView = (DataRowView)enumerator.Current;
                                    if (Convert.ToBoolean(dataRowView["Enabled"]) && SCore.IsAstericMatch(dataRowView["Pattern"].ToString(), text))
                                    {
                                        e.Reply = new SMTP_Reply(250, "OK.");
                                        break;
                                    }
                                }
                                goto IL_25C;
                            }
                            finally
                            {
                                IDisposable disposable = enumerator as IDisposable;
                                if (disposable != null)
                                {
                                    disposable.Dispose();
                                }
                            }
                        }
                    }
                    if (this.m_pApi.ValidateMailboxSize(text2))
                    {
                        e.Reply = new SMTP_Reply(552, "Requested mail action aborted: Mailbox <" + e.RcptTo.Mailbox + "> is full.");
                    }
                    else
                    {
                        e.Reply = new SMTP_Reply(250, "OK.");
                    }
                }
                else
                {
                    e.Reply = new SMTP_Reply(550, "Relay not allowed.");
                    if (e.Session.IsAuthenticated)
                    {
                        if (this.IsRelayAllowed(e.Session.AuthenticatedUserIdentity.Name, e.Session.RemoteEndPoint.Address))
                        {
                            e.Reply = new SMTP_Reply(250, "User not local will relay.");
                        }
                    }
                    else if (this.IsRelayAllowed("", e.Session.RemoteEndPoint.Address))
                    {
                        e.Reply = new SMTP_Reply(250, "User not local will relay.");
                    }
                }
                IL_25C:;
            }
            catch (Exception x)
            {
                e.Reply = new SMTP_Reply(500, "Internal server error.");
                Error.DumpError(this.Name, x);
            }
        }

        private void m_pSMTP_Server_Session_GetMessageStream(object sender, SMTP_e_Message e)
        {
            if (!Directory.Exists(this.m_MailStorePath + "IncomingSMTP"))
            {
                Directory.CreateDirectory(this.m_MailStorePath + "IncomingSMTP");
            }
            e.Stream = new FileStream(PathHelper.PathFix(this.m_MailStorePath + "IncomingSMTP\\" + Guid.NewGuid().ToString().Replace("-", "") + ".eml"), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 32000, FileOptions.DeleteOnClose);
            e.Session.Tags["MessageStream"] = e.Stream;
        }

        private void m_pSMTP_Server_Session_MessageStoringCanceled(object sender, EventArgs e)
        {
            try
            {
                ((IDisposable)((SMTP_Session)sender).Tags["MessageStream"]).Dispose();
            }
            catch
            {
            }
        }

        private void m_pSMTP_Server_Session_MessageStoringCompleted(object sender, SMTP_e_MessageStored e)
        {
            try
            {
                e.Stream.Position = 0L;
                this.ProcessAndStoreMessage(e.Session.From.ENVID, e.Session.From.Mailbox, e.Session.From.RET, e.Session.To, e.Stream, e);
            }
            catch (Exception x)
            {
                Error.DumpError(this.Name, x);
                e.Reply = new SMTP_Reply(552, "Requested mail action aborted: Internal server error.");
            }
            finally
            {
                ((FileStream)e.Stream).Dispose();
            }
        }

        private void SMTP_Server_SessionLog(object sender, WriteLogEventArgs e)
        {
            Logger.WriteLog(this.m_SMTP_LogPath + "smtp-" + DateTime.Today.ToString("yyyyMMdd") + ".log", e.LogEntry);
        }

        private void m_pRelayServer_WriteLog(object sender, WriteLogEventArgs e)
        {
            Logger.WriteLog(this.m_Relay_LogPath + "relay-" + DateTime.Today.ToString("yyyyMMdd") + ".log", e.LogEntry);
        }

        private void m_pPop3Server_SessionCreated(object sender, TCP_ServerSessionEventArgs<POP3_Session> e)
        {
            e.Session.Started += new EventHandler<POP3_e_Started>(this.m_pPop3Server_Session_Started);
            e.Session.Authenticate += new EventHandler<POP3_e_Authenticate>(this.m_pPop3Server_Session_Authenticate);
            e.Session.GetMessagesInfo += new EventHandler<POP3_e_GetMessagesInfo>(this.m_pPop3Server_Session_GetMessagesInfo);
            e.Session.GetTopOfMessage += new EventHandler<POP3_e_GetTopOfMessage>(this.m_pPop3Server_Session_GetTopOfMessage);
            e.Session.GetMessageStream += new EventHandler<POP3_e_GetMessageStream>(this.m_pPop3Server_Session_GetMessageStream);
            e.Session.DeleteMessage += new EventHandler<POP3_e_DeleteMessage>(this.m_pPop3Server_Session_DeleteMessage);
            if (this.m_AuthType == MailServerAuthType.Windows || this.m_AuthType == MailServerAuthType.Ldap)
            {
                AUTH_SASL_ServerMechanism_Plain aUTH_SASL_ServerMechanism_Plain = new AUTH_SASL_ServerMechanism_Plain(false);
                aUTH_SASL_ServerMechanism_Plain.Authenticate += delegate (object s, AUTH_e_Authenticate e1)
                {
                    try
                    {
                        if ((this.m_pApi.GetUserPermissions(e1.UserName) & UserPermissions.POP3) == UserPermissions.None)
                        {
                            e1.IsAuthenticated = false;
                        }
                        else
                        {
                            e1.IsAuthenticated = this.Authenticate(e.Session.RemoteEndPoint.Address, e1.UserName, e1.Password);
                        }
                    }
                    catch (Exception x)
                    {
                        this.OnError(x);
                        e1.IsAuthenticated = false;
                    }
                };
                e.Session.Authentications.Add(aUTH_SASL_ServerMechanism_Plain.Name, aUTH_SASL_ServerMechanism_Plain);
                AUTH_SASL_ServerMechanism_Login aUTH_SASL_ServerMechanism_Login = new AUTH_SASL_ServerMechanism_Login(false);
                aUTH_SASL_ServerMechanism_Login.Authenticate += delegate (object s, AUTH_e_Authenticate e1)
                {
                    try
                    {
                        if ((this.m_pApi.GetUserPermissions(e1.UserName) & UserPermissions.POP3) == UserPermissions.None)
                        {
                            e1.IsAuthenticated = false;
                        }
                        else
                        {
                            e1.IsAuthenticated = this.Authenticate(e.Session.RemoteEndPoint.Address, e1.UserName, e1.Password);
                        }
                    }
                    catch (Exception x)
                    {
                        this.OnError(x);
                        e1.IsAuthenticated = false;
                    }
                };
                e.Session.Authentications.Add(aUTH_SASL_ServerMechanism_Login.Name, aUTH_SASL_ServerMechanism_Login);
                return;
            }
            AUTH_SASL_ServerMechanism_DigestMd5 aUTH_SASL_ServerMechanism_DigestMd = new AUTH_SASL_ServerMechanism_DigestMd5(false);
            aUTH_SASL_ServerMechanism_DigestMd.Realm = e.Session.LocalHostName;
            aUTH_SASL_ServerMechanism_DigestMd.GetUserInfo += delegate (object s, AUTH_e_UserInfo e1)
            {
                if ((this.m_pApi.GetUserPermissions(e1.UserName) & UserPermissions.POP3) == UserPermissions.None)
                {
                    e1.UserExists = false;
                    return;
                }
                this.FillUserInfo(e1);
            };
            e.Session.Authentications.Add(aUTH_SASL_ServerMechanism_DigestMd.Name, aUTH_SASL_ServerMechanism_DigestMd);
            AUTH_SASL_ServerMechanism_CramMd5 aUTH_SASL_ServerMechanism_CramMd = new AUTH_SASL_ServerMechanism_CramMd5(false);
            aUTH_SASL_ServerMechanism_CramMd.GetUserInfo += delegate (object s, AUTH_e_UserInfo e1)
            {
                if ((this.m_pApi.GetUserPermissions(e1.UserName) & UserPermissions.POP3) == UserPermissions.None)
                {
                    e1.UserExists = false;
                    return;
                }
                this.FillUserInfo(e1);
            };
            e.Session.Authentications.Add(aUTH_SASL_ServerMechanism_CramMd.Name, aUTH_SASL_ServerMechanism_CramMd);
            AUTH_SASL_ServerMechanism_Plain aUTH_SASL_ServerMechanism_Plain2 = new AUTH_SASL_ServerMechanism_Plain(false);
            aUTH_SASL_ServerMechanism_Plain2.Authenticate += delegate (object s, AUTH_e_Authenticate e1)
            {
                try
                {
                    if ((this.m_pApi.GetUserPermissions(e1.UserName) & UserPermissions.POP3) == UserPermissions.None)
                    {
                        e1.IsAuthenticated = false;
                    }
                    else
                    {
                        e1.IsAuthenticated = this.Authenticate(e.Session.RemoteEndPoint.Address, e1.UserName, e1.Password);
                    }
                }
                catch (Exception x)
                {
                    this.OnError(x);
                    e1.IsAuthenticated = false;
                }
            };
            e.Session.Authentications.Add(aUTH_SASL_ServerMechanism_Plain2.Name, aUTH_SASL_ServerMechanism_Plain2);
            AUTH_SASL_ServerMechanism_Login aUTH_SASL_ServerMechanism_Login2 = new AUTH_SASL_ServerMechanism_Login(false);
            aUTH_SASL_ServerMechanism_Login2.Authenticate += delegate (object s, AUTH_e_Authenticate e1)
            {
                try
                {
                    if ((this.m_pApi.GetUserPermissions(e1.UserName) & UserPermissions.POP3) == UserPermissions.None)
                    {
                        e1.IsAuthenticated = false;
                    }
                    else
                    {
                        e1.IsAuthenticated = this.Authenticate(e.Session.RemoteEndPoint.Address, e1.UserName, e1.Password);
                    }
                }
                catch (Exception x)
                {
                    this.OnError(x);
                    e1.IsAuthenticated = false;
                }
            };
            e.Session.Authentications.Add(aUTH_SASL_ServerMechanism_Login2.Name, aUTH_SASL_ServerMechanism_Login2);
        }

        private void m_pPop3Server_Session_Started(object sender, POP3_e_Started e)
        {
            if (!this.IsAccessAllowed(ServiceType.POP3, ((POP3_Session)sender).RemoteEndPoint.Address))
            {
                e.Response = "-ERR Your IP address is blocked.";
            }
        }

        private void m_pPop3Server_Session_Authenticate(object sender, POP3_e_Authenticate e)
        {
            if ((this.m_pApi.GetUserPermissions(e.User) & UserPermissions.POP3) == UserPermissions.None)
            {
                e.IsAuthenticated = false;
                return;
            }
            e.IsAuthenticated = this.Authenticate(((POP3_Session)sender).RemoteEndPoint.Address, e.User, e.Password);
        }

        private void m_pPop3Server_Session_GetMessagesInfo(object sender, POP3_e_GetMessagesInfo e)
        {
            try
            {
                string name = ((POP3_Session)sender).AuthenticatedUserIdentity.Name;
                List<IMAP_MessageInfo> list = new List<IMAP_MessageInfo>();
                this.m_pApi.GetMessagesInfo(name, name, "Inbox", list);
                foreach (IMAP_MessageInfo current in list)
                {
                    e.Messages.Add(new POP3_ServerMessage(current.UID.ToString(), current.Size, current.ID));
                }
            }
            catch (Exception x)
            {
                Error.DumpError(this.Name, x);
            }
        }

        private void m_pPop3Server_Session_GetTopOfMessage(object sender, POP3_e_GetTopOfMessage e)
        {
            try
            {
                string name = ((POP3_Session)sender).AuthenticatedUserIdentity.Name;
                e.Data = this.m_pApi.GetMessageTopLines(name, name, "Inbox", e.Message.Tag.ToString(), e.LineCount);
            }
            catch (Exception x)
            {
                Error.DumpError(x);
            }
        }

        private void m_pPop3Server_Session_GetMessageStream(object sender, POP3_e_GetMessageStream e)
        {
            try
            {
                string name = ((POP3_Session)sender).AuthenticatedUserIdentity.Name;
                EmailMessageItems emailMessageItems = new EmailMessageItems(e.Message.Tag.ToString(), IMAP_MessageItems.Message);
                this.m_pApi.GetMessageItems(name, name, "Inbox", emailMessageItems);
                if (emailMessageItems.MessageStream != null)
                {
                    e.MessageStream = emailMessageItems.MessageStream;
                }
            }
            catch (Exception x)
            {
                Error.DumpError(this.Name, x);
            }
        }

        private void m_pPop3Server_Session_DeleteMessage(object sender, POP3_e_DeleteMessage e)
        {
            try
            {
                string name = ((POP3_Session)sender).AuthenticatedUserIdentity.Name;
                this.m_pApi.DeleteMessage(name, name, "Inbox", e.Message.Tag.ToString(), Convert.ToInt32(e.Message.UID));
            }
            catch (Exception x)
            {
                Error.DumpError(this.Name, x);
            }
        }

        private void POP3_Server_SessionLog(object sender, WriteLogEventArgs e)
        {
            Logger.WriteLog(this.m_POP3_LogPath + "pop3-" + DateTime.Today.ToString("yyyyMMdd") + ".log", e.LogEntry);
        }

        private void m_pImapServer_SessionCreated(object sender, TCP_ServerSessionEventArgs<IMAP_Session> e)
        {
            e.Session.Started += new EventHandler<IMAP_e_Started>(this.m_pImapServer_Session_Started);
            e.Session.Login += new EventHandler<IMAP_e_Login>(this.m_pImapServer_Session_Login);
            e.Session.Namespace += new EventHandler<IMAP_e_Namespace>(this.m_pImapServer_Session_Namespace);
            e.Session.LSub += new EventHandler<IMAP_e_LSub>(this.m_pImapServer_Session_LSub);
            e.Session.Subscribe += new EventHandler<IMAP_e_Folder>(this.m_pImapServer_Session_Subscribe);
            e.Session.Unsubscribe += new EventHandler<IMAP_e_Folder>(this.m_pImapServer_Session_Unsubscribe);
            e.Session.List += new EventHandler<IMAP_e_List>(this.m_pImapServer_Session_List);
            e.Session.Create += new EventHandler<IMAP_e_Folder>(this.m_pImapServer_Session_Create);
            e.Session.Delete += new EventHandler<IMAP_e_Folder>(this.m_pImapServer_Session_Delete);
            e.Session.Rename += new EventHandler<IMAP_e_Rename>(this.m_pImapServer_Session_Rename);
            e.Session.GetQuotaRoot += new EventHandler<IMAP_e_GetQuotaRoot>(this.m_pImapServer_Session_GetQuotaRoot);
            e.Session.GetQuota += new EventHandler<IMAP_e_GetQuota>(this.m_pImapServer_Session_GetQuota);
            e.Session.GetAcl += new EventHandler<IMAP_e_GetAcl>(this.m_pImapServer_Session_GetAcl);
            e.Session.SetAcl += new EventHandler<IMAP_e_SetAcl>(this.m_pImapServer_Session_SetAcl);
            e.Session.DeleteAcl += new EventHandler<IMAP_e_DeleteAcl>(this.m_pImapServer_Session_DeleteAcl);
            e.Session.ListRights += new EventHandler<IMAP_e_ListRights>(this.m_pImapServer_Session_ListRights);
            e.Session.MyRights += new EventHandler<IMAP_e_MyRights>(this.m_pImapServer_Session_MyRights);
            e.Session.Select += new EventHandler<IMAP_e_Select>(this.m_pImapServer_Session_Select);
            e.Session.Append += new EventHandler<IMAP_e_Append>(this.m_pImapServer_Session_Append);
            e.Session.GetMessagesInfo += new EventHandler<IMAP_e_MessagesInfo>(this.m_pImapServer_Session_GetMessagesInfo);
            e.Session.Search += new EventHandler<IMAP_e_Search>(this.m_pImapServer_Session_Search);
            e.Session.Fetch += new EventHandler<IMAP_e_Fetch>(this.m_pImapServer_Session_Fetch);
            e.Session.Expunge += new EventHandler<IMAP_e_Expunge>(this.m_pImapServer_Session_Expunge);
            e.Session.Store += new EventHandler<IMAP_e_Store>(this.m_pImapServer_Session_Store);
            e.Session.Copy += new EventHandler<IMAP_e_Copy>(this.m_pImapServer_Session_Copy);
            if (this.m_AuthType == MailServerAuthType.Windows || this.m_AuthType == MailServerAuthType.Ldap)
            {
                AUTH_SASL_ServerMechanism_Plain aUTH_SASL_ServerMechanism_Plain = new AUTH_SASL_ServerMechanism_Plain(false);
                aUTH_SASL_ServerMechanism_Plain.Authenticate += delegate (object s, AUTH_e_Authenticate e1)
                {
                    try
                    {
                        if ((this.m_pApi.GetUserPermissions(e1.UserName) & UserPermissions.IMAP) == UserPermissions.None)
                        {
                            e1.IsAuthenticated = false;
                        }
                        else
                        {
                            e1.IsAuthenticated = this.Authenticate(e.Session.RemoteEndPoint.Address, e1.UserName, e1.Password);
                        }
                    }
                    catch (Exception x)
                    {
                        this.OnError(x);
                        e1.IsAuthenticated = false;
                    }
                };
                e.Session.Authentications.Add(aUTH_SASL_ServerMechanism_Plain.Name, aUTH_SASL_ServerMechanism_Plain);
                AUTH_SASL_ServerMechanism_Login aUTH_SASL_ServerMechanism_Login = new AUTH_SASL_ServerMechanism_Login(false);
                aUTH_SASL_ServerMechanism_Login.Authenticate += delegate (object s, AUTH_e_Authenticate e1)
                {
                    try
                    {
                        if ((this.m_pApi.GetUserPermissions(e1.UserName) & UserPermissions.IMAP) == UserPermissions.None)
                        {
                            e1.IsAuthenticated = false;
                        }
                        else
                        {
                            e1.IsAuthenticated = this.Authenticate(e.Session.RemoteEndPoint.Address, e1.UserName, e1.Password);
                        }
                    }
                    catch (Exception x)
                    {
                        this.OnError(x);
                        e1.IsAuthenticated = false;
                    }
                };
                e.Session.Authentications.Add(aUTH_SASL_ServerMechanism_Login.Name, aUTH_SASL_ServerMechanism_Login);
                return;
            }
            AUTH_SASL_ServerMechanism_DigestMd5 aUTH_SASL_ServerMechanism_DigestMd = new AUTH_SASL_ServerMechanism_DigestMd5(false);
            aUTH_SASL_ServerMechanism_DigestMd.Realm = e.Session.LocalHostName;
            aUTH_SASL_ServerMechanism_DigestMd.GetUserInfo += delegate (object s, AUTH_e_UserInfo e1)
            {
                if ((this.m_pApi.GetUserPermissions(e1.UserName) & UserPermissions.IMAP) == UserPermissions.None)
                {
                    e1.UserExists = false;
                    return;
                }
                this.FillUserInfo(e1);
            };
            e.Session.Authentications.Add(aUTH_SASL_ServerMechanism_DigestMd.Name, aUTH_SASL_ServerMechanism_DigestMd);
            AUTH_SASL_ServerMechanism_CramMd5 aUTH_SASL_ServerMechanism_CramMd = new AUTH_SASL_ServerMechanism_CramMd5(false);
            aUTH_SASL_ServerMechanism_CramMd.GetUserInfo += delegate (object s, AUTH_e_UserInfo e1)
            {
                if ((this.m_pApi.GetUserPermissions(e1.UserName) & UserPermissions.IMAP) == UserPermissions.None)
                {
                    e1.UserExists = false;
                    return;
                }
                this.FillUserInfo(e1);
            };
            e.Session.Authentications.Add(aUTH_SASL_ServerMechanism_CramMd.Name, aUTH_SASL_ServerMechanism_CramMd);
            AUTH_SASL_ServerMechanism_Plain aUTH_SASL_ServerMechanism_Plain2 = new AUTH_SASL_ServerMechanism_Plain(false);
            aUTH_SASL_ServerMechanism_Plain2.Authenticate += delegate (object s, AUTH_e_Authenticate e1)
            {
                try
                {
                    if ((this.m_pApi.GetUserPermissions(e1.UserName) & UserPermissions.IMAP) == UserPermissions.None)
                    {
                        e1.IsAuthenticated = false;
                    }
                    else
                    {
                        e1.IsAuthenticated = this.Authenticate(e.Session.RemoteEndPoint.Address, e1.UserName, e1.Password);
                    }
                }
                catch (Exception x)
                {
                    this.OnError(x);
                    e1.IsAuthenticated = false;
                }
            };
            e.Session.Authentications.Add(aUTH_SASL_ServerMechanism_Plain2.Name, aUTH_SASL_ServerMechanism_Plain2);
            AUTH_SASL_ServerMechanism_Login aUTH_SASL_ServerMechanism_Login2 = new AUTH_SASL_ServerMechanism_Login(false);
            aUTH_SASL_ServerMechanism_Login2.Authenticate += delegate (object s, AUTH_e_Authenticate e1)
            {
                try
                {
                    if ((this.m_pApi.GetUserPermissions(e1.UserName) & UserPermissions.IMAP) == UserPermissions.None)
                    {
                        e1.IsAuthenticated = false;
                    }
                    else
                    {
                        e1.IsAuthenticated = this.Authenticate(e.Session.RemoteEndPoint.Address, e1.UserName, e1.Password);
                    }
                }
                catch (Exception x)
                {
                    this.OnError(x);
                    e1.IsAuthenticated = false;
                }
            };
            e.Session.Authentications.Add(aUTH_SASL_ServerMechanism_Login2.Name, aUTH_SASL_ServerMechanism_Login2);
        }

        private void m_pImapServer_Session_Started(object sender, IMAP_e_Started e)
        {
            if (!this.IsAccessAllowed(ServiceType.IMAP, ((IMAP_Session)sender).RemoteEndPoint.Address))
            {
                e.Response = new IMAP_r_u_ServerStatus("NO", "Your IP address is blocked.");
            }
        }

        private void m_pImapServer_Session_Login(object sender, IMAP_e_Login e)
        {
            if ((this.m_pApi.GetUserPermissions(e.UserName) & UserPermissions.IMAP) == UserPermissions.None)
            {
                e.IsAuthenticated = false;
                return;
            }
            e.IsAuthenticated = this.Authenticate(((IMAP_Session)sender).RemoteEndPoint.Address, e.UserName, e.Password);
        }

        private void m_pImapServer_Session_Namespace(object sender, IMAP_e_Namespace e)
        {
            SharedFolderRoot[] sharedFolderRoots = this.m_pApi.GetSharedFolderRoots();
            List<IMAP_Namespace_Entry> list = new List<IMAP_Namespace_Entry>();
            List<IMAP_Namespace_Entry> list2 = new List<IMAP_Namespace_Entry>();
            SharedFolderRoot[] array = sharedFolderRoots;
            for (int i = 0; i < array.Length; i++)
            {
                SharedFolderRoot sharedFolderRoot = array[i];
                if (sharedFolderRoot.Enabled)
                {
                    if (sharedFolderRoot.RootType == SharedFolderRootType.BoundedRootFolder)
                    {
                        list.Add(new IMAP_Namespace_Entry(sharedFolderRoot.FolderName, '/'));
                    }
                    else
                    {
                        list2.Add(new IMAP_Namespace_Entry(sharedFolderRoot.FolderName, '/'));
                    }
                }
            }
            e.NamespaceResponse = new IMAP_r_u_Namespace(new IMAP_Namespace_Entry[]
            {
                new IMAP_Namespace_Entry("", '/')
            }, list2.ToArray(), list.ToArray());
        }

        private void m_pImapServer_Session_LSub(object sender, IMAP_e_LSub e)
        {
            IMAP_Session iMAP_Session = (IMAP_Session)sender;
            string[] subscribedFolders = this.m_pApi.GetSubscribedFolders(iMAP_Session.AuthenticatedUserIdentity.Name);
            string[] array = subscribedFolders;
            for (int i = 0; i < array.Length; i++)
            {
                string text = array[i];
                if ((string.IsNullOrEmpty(e.FolderReferenceName) || text.StartsWith(e.FolderReferenceName, StringComparison.InvariantCultureIgnoreCase)) && !string.IsNullOrEmpty(text) && this.FolderMatches(e.FolderFilter, text))
                {
                    e.Folders.Add(new IMAP_r_u_LSub(text, '/', null));
                }
            }
        }

        private void m_pImapServer_Session_Subscribe(object sender, IMAP_e_Folder e)
        {
            IMAP_Session iMAP_Session = (IMAP_Session)sender;
            this.m_pApi.SubscribeFolder(iMAP_Session.AuthenticatedUserIdentity.Name, e.Folder);
        }

        private void m_pImapServer_Session_Unsubscribe(object sender, IMAP_e_Folder e)
        {
            IMAP_Session iMAP_Session = (IMAP_Session)sender;
            this.m_pApi.UnSubscribeFolder(iMAP_Session.AuthenticatedUserIdentity.Name, e.Folder);
        }

        private void m_pImapServer_Session_List(object sender, IMAP_e_List e)
        {
            IMAP_Session iMAP_Session = (IMAP_Session)sender;
            string[] folders = this.m_pApi.GetFolders(iMAP_Session.AuthenticatedUserIdentity.Name, true);
            string[] array = folders;
            for (int i = 0; i < array.Length; i++)
            {
                string text = array[i];
                if ((string.IsNullOrEmpty(e.FolderReferenceName) || text.StartsWith(e.FolderReferenceName, StringComparison.InvariantCultureIgnoreCase)) && this.FolderMatches(e.FolderFilter, text))
                {
                    e.Folders.Add(new IMAP_r_u_List(text, '/', null));
                }
            }
        }

        private void m_pImapServer_Session_Create(object sender, IMAP_e_Folder e)
        {
            try
            {
                IMAP_Session iMAP_Session = (IMAP_Session)sender;
                this.m_pApi.CreateFolder(iMAP_Session.AuthenticatedUserIdentity.Name, iMAP_Session.AuthenticatedUserIdentity.Name, e.Folder);
            }
            catch (Exception ex)
            {
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag, "NO", "Error: " + ex.Message);
            }
        }

        private void m_pImapServer_Session_Delete(object sender, IMAP_e_Folder e)
        {
            try
            {
                IMAP_Session iMAP_Session = (IMAP_Session)sender;
                this.m_pApi.DeleteFolder(iMAP_Session.AuthenticatedUserIdentity.Name, iMAP_Session.AuthenticatedUserIdentity.Name, e.Folder);
            }
            catch (Exception ex)
            {
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag, "NO", "Error: " + ex.Message);
            }
        }

        private void m_pImapServer_Session_Rename(object sender, IMAP_e_Rename e)
        {
            try
            {
                IMAP_Session iMAP_Session = (IMAP_Session)sender;
                this.m_pApi.RenameFolder(iMAP_Session.AuthenticatedUserIdentity.Name, iMAP_Session.AuthenticatedUserIdentity.Name, e.CurrentFolder, e.NewFolder);
                e.Response = new IMAP_r_ServerStatus(e.CmdTag, "OK", "RENAME command completed.");
            }
            catch (Exception ex)
            {
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag, "NO", "Error: " + ex.Message);
            }
        }

        private void m_pImapServer_Session_GetQuotaRoot(object sender, IMAP_e_GetQuotaRoot e)
        {
            try
            {
                IMAP_Session iMAP_Session = (IMAP_Session)sender;
                e.QuotaRootResponses.Add(new IMAP_r_u_QuotaRoot(e.Folder, new string[]
                {
                    "root"
                }));
                foreach (DataRowView dataRowView in this.m_pApi.GetUsers("ALL"))
                {
                    if (dataRowView["UserName"].ToString().ToLower() == iMAP_Session.AuthenticatedUserIdentity.Name.ToLower())
                    {
                        e.QuotaResponses.Add(new IMAP_r_u_Quota("root", new IMAP_Quota_Entry[]
                        {
                            new IMAP_Quota_Entry("STORAGE", this.m_pApi.GetMailboxSize(iMAP_Session.AuthenticatedUserIdentity.Name), (long)(ConvertEx.ToInt32(dataRowView["Mailbox_Size"]) * 1000 * 1000))
                        }));
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag, "NO", "Error: " + ex.Message);
            }
        }

        private void m_pImapServer_Session_GetQuota(object sender, IMAP_e_GetQuota e)
        {
            try
            {
                IMAP_Session iMAP_Session = (IMAP_Session)sender;
                foreach (DataRowView dataRowView in this.m_pApi.GetUsers("ALL"))
                {
                    if (dataRowView["UserName"].ToString().ToLower() == iMAP_Session.AuthenticatedUserIdentity.Name.ToLower())
                    {
                        e.QuotaResponses.Add(new IMAP_r_u_Quota(e.QuotaRoot, new IMAP_Quota_Entry[]
                        {
                            new IMAP_Quota_Entry("STORAGE", this.m_pApi.GetMailboxSize(iMAP_Session.AuthenticatedUserIdentity.Name), (long)(ConvertEx.ToInt32(dataRowView["Mailbox_Size"]) * 1000 * 1000))
                        }));
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag, "NO", "Error: " + ex.Message);
            }
        }

        private void m_pImapServer_Session_GetAcl(object sender, IMAP_e_GetAcl e)
        {
            try
            {
                IMAP_Session iMAP_Session = (IMAP_Session)sender;
                DataView folderACL = this.m_pApi.GetFolderACL(iMAP_Session.AuthenticatedUserIdentity.Name, iMAP_Session.AuthenticatedUserIdentity.Name, e.Folder);
                List<IMAP_Acl_Entry> list = new List<IMAP_Acl_Entry>();
                foreach (DataRowView dataRowView in folderACL)
                {
                    list.Add(new IMAP_Acl_Entry(dataRowView["User"].ToString(), dataRowView["Permissions"].ToString()));
                }
                e.AclResponses.Add(new IMAP_r_u_Acl(e.Folder, list.ToArray()));
            }
            catch (Exception ex)
            {
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag, "NO", "Error: " + ex.Message);
            }
        }

        private void m_pImapServer_Session_SetAcl(object sender, IMAP_e_SetAcl e)
        {
            try
            {
                IMAP_Session iMAP_Session = (IMAP_Session)sender;
                this.m_pApi.SetFolderACL(iMAP_Session.AuthenticatedUserIdentity.Name, iMAP_Session.AuthenticatedUserIdentity.Name, e.Folder, e.Identifier, e.FlagsSetType, IMAP_Utils.ACL_From_String(e.Rights));
            }
            catch (Exception ex)
            {
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag, "NO", "Error: " + ex.Message);
            }
        }

        private void m_pImapServer_Session_DeleteAcl(object sender, IMAP_e_DeleteAcl e)
        {
            try
            {
                IMAP_Session iMAP_Session = (IMAP_Session)sender;
                this.m_pApi.DeleteFolderACL(iMAP_Session.AuthenticatedUserIdentity.Name, iMAP_Session.AuthenticatedUserIdentity.Name, e.Folder, e.Identifier);
            }
            catch (Exception ex)
            {
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag, "NO", "Error: " + ex.Message);
            }
        }

        private void m_pImapServer_Session_ListRights(object sender, IMAP_e_ListRights e)
        {
            try
            {
                e.ListRightsResponse = new IMAP_r_u_ListRights(e.Folder, e.Identifier, "", "l r s w i p c d a");
            }
            catch (Exception ex)
            {
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag, "NO", "Error: " + ex.Message);
            }
        }

        private void m_pImapServer_Session_MyRights(object sender, IMAP_e_MyRights e)
        {
            try
            {
                IMAP_Session iMAP_Session = (IMAP_Session)sender;
                e.MyRightsResponse = new IMAP_r_u_MyRights(e.Folder, IMAP_Utils.ACL_to_String(this.m_pApi.GetUserACL(iMAP_Session.AuthenticatedUserIdentity.Name, e.Folder, iMAP_Session.AuthenticatedUserIdentity.Name)));
            }
            catch (Exception ex)
            {
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag, "NO", "Error: " + ex.Message);
            }
        }

        private void m_pImapServer_Session_Select(object sender, IMAP_e_Select e)
        {
            try
            {
                IMAP_Session arg_06_0 = (IMAP_Session)sender;
                e.FolderUID = 1237333;
            }
            catch (Exception ex)
            {
                e.ErrorResponse = new IMAP_r_ServerStatus(e.CmdTag, "NO", "Error: " + ex.Message);
            }
        }

        private void m_pImapServer_Session_Append(object sender, IMAP_e_Append e)
        {
            try
            {
                IMAP_Session ses = (IMAP_Session)sender;
                e.Stream = new SwapableStream(32000);
                e.Completed += delegate (object s1, EventArgs e1)
                {
                    e.Stream.Position = 0L;
                    this.m_pApi.StoreMessage(ses.AuthenticatedUserIdentity.Name, ses.AuthenticatedUserIdentity.Name, e.Folder, e.Stream, (e.InternalDate == DateTime.MinValue) ? DateTime.Now : e.InternalDate, e.Flags);
                };
            }
            catch (Exception ex)
            {
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag, "NO", "Error: " + ex.Message);
            }
        }

        private void m_pImapServer_Session_GetMessagesInfo(object sender, IMAP_e_MessagesInfo e)
        {
            IMAP_Session iMAP_Session = (IMAP_Session)sender;
            List<IMAP_MessageInfo> list = new List<IMAP_MessageInfo>();
            this.m_pApi.GetMessagesInfo(iMAP_Session.AuthenticatedUserIdentity.Name, iMAP_Session.AuthenticatedUserIdentity.Name, e.Folder, list);
            e.MessagesInfo.AddRange(list);
        }

        private void m_pImapServer_Session_Search(object sender, IMAP_e_Search e)
        {
            try
            {
                IMAP_Session iMAP_Session = (IMAP_Session)sender;
                this.m_pApi.Search(iMAP_Session.AuthenticatedUserIdentity.Name, iMAP_Session.AuthenticatedUserIdentity.Name, iMAP_Session.SelectedFolderName, e);
            }
            catch (Exception ex)
            {
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag, "NO", "Error: " + ex.Message);
            }
        }

        private void m_pImapServer_Session_Fetch(object sender, IMAP_e_Fetch e)
        {
            try
            {
                IMAP_Session iMAP_Session = (IMAP_Session)sender;
                IMAP_MessageInfo[] messagesInfo = e.MessagesInfo;
                for (int i = 0; i < messagesInfo.Length; i++)
                {
                    IMAP_MessageInfo iMAP_MessageInfo = messagesInfo[i];
                    if (e.FetchDataType == IMAP_Fetch_DataType.MessageHeader)
                    {
                        EmailMessageItems emailMessageItems = new EmailMessageItems(iMAP_MessageInfo.ID, IMAP_MessageItems.Header);
                        this.m_pApi.GetMessageItems(iMAP_Session.AuthenticatedUserIdentity.Name, iMAP_Session.AuthenticatedUserIdentity.Name, iMAP_Session.SelectedFolderName, emailMessageItems);
                        Mail_Message msgData = null;
                        try
                        {
                            if (emailMessageItems.MessageExists)
                            {
                                msgData = Mail_Message.ParseFromByte(emailMessageItems.Header);
                            }
                            else
                            {
                                msgData = VirtualServer.GenerateMessageMissing();
                            }
                        }
                        catch
                        {
                            msgData = PathHelper.GenerateBadMessage(new MemoryStream(emailMessageItems.Header));
                        }
                        e.AddData(iMAP_MessageInfo, msgData);
                    }
                    else if (e.FetchDataType == IMAP_Fetch_DataType.MessageStructure)
                    {
                        EmailMessageItems emailMessageItems2 = new EmailMessageItems(iMAP_MessageInfo.ID, IMAP_MessageItems.Message);
                        this.m_pApi.GetMessageItems(iMAP_Session.AuthenticatedUserIdentity.Name, iMAP_Session.AuthenticatedUserIdentity.Name, iMAP_Session.SelectedFolderName, emailMessageItems2);
                        Mail_Message mail_Message = null;
                        try
                        {
                            if (emailMessageItems2.MessageExists)
                            {
                                mail_Message = Mail_Message.ParseFromStream(emailMessageItems2.MessageStream);
                            }
                            else
                            {
                                mail_Message = VirtualServer.GenerateMessageMissing();
                                MemoryStream messageStream = new MemoryStream(mail_Message.ToByte(new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q, Encoding.UTF8), Encoding.UTF8));
                                emailMessageItems2.MessageStream = messageStream;
                            }
                        }
                        catch
                        {
                            mail_Message = PathHelper.GenerateBadMessage(emailMessageItems2.MessageStream);
                        }
                        e.AddData(iMAP_MessageInfo, mail_Message);
                        emailMessageItems2.MessageStream.Close();
                    }
                    else
                    {
                        EmailMessageItems emailMessageItems3 = new EmailMessageItems(iMAP_MessageInfo.ID, IMAP_MessageItems.Message);
                        this.m_pApi.GetMessageItems(iMAP_Session.AuthenticatedUserIdentity.Name, iMAP_Session.AuthenticatedUserIdentity.Name, iMAP_Session.SelectedFolderName, emailMessageItems3);
                        Mail_Message mail_Message2 = null;
                        try
                        {
                            if (emailMessageItems3.MessageExists)
                            {
                                mail_Message2 = Mail_Message.ParseFromStream(emailMessageItems3.MessageStream);
                            }
                            else
                            {
                                mail_Message2 = VirtualServer.GenerateMessageMissing();
                                MemoryStream messageStream2 = new MemoryStream(mail_Message2.ToByte(new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q, Encoding.UTF8), Encoding.UTF8));
                                emailMessageItems3.MessageStream = messageStream2;
                            }
                        }
                        catch
                        {
                            mail_Message2 = PathHelper.GenerateBadMessage(emailMessageItems3.MessageStream);
                        }
                        e.AddData(iMAP_MessageInfo, mail_Message2);
                        emailMessageItems3.MessageStream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag, "NO", "Error: " + ex.Message);
            }
        }

        private void m_pImapServer_Session_Expunge(object sender, IMAP_e_Expunge e)
        {
            try
            {
                IMAP_Session iMAP_Session = (IMAP_Session)sender;
                this.m_pApi.DeleteMessage(iMAP_Session.AuthenticatedUserIdentity.Name, iMAP_Session.AuthenticatedUserIdentity.Name, iMAP_Session.SelectedFolderName, e.MessageInfo.ID, (int)e.MessageInfo.UID);
            }
            catch (Exception ex)
            {
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag, "NO", "Error: " + ex.Message);
            }
        }

        private void m_pImapServer_Session_Store(object sender, IMAP_e_Store e)
        {
            try
            {
                IMAP_Session iMAP_Session = (IMAP_Session)sender;
                List<string> list = new List<string>();
                if (e.FlagsSetType == IMAP_Flags_SetType.Add)
                {
                    list.AddRange(IMAP_Utils.MessageFlagsAdd(e.MessageInfo.Flags, e.Flags));
                }
                else if (e.FlagsSetType == IMAP_Flags_SetType.Remove)
                {
                    list.AddRange(IMAP_Utils.MessageFlagsRemove(e.MessageInfo.Flags, e.Flags));
                }
                else
                {
                    list.AddRange(e.Flags);
                }
                this.m_pApi.StoreMessageFlags(iMAP_Session.AuthenticatedUserIdentity.Name, iMAP_Session.AuthenticatedUserIdentity.Name, iMAP_Session.SelectedFolderName, e.MessageInfo, list.ToArray());
            }
            catch (Exception ex)
            {
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag, "NO", "Error: " + ex.Message);
            }
        }

        private void m_pImapServer_Session_Copy(object sender, IMAP_e_Copy e)
        {
            try
            {
                IMAP_Session iMAP_Session = (IMAP_Session)sender;
                List<IMAP_MessageInfo> list = new List<IMAP_MessageInfo>();
                try
                {
                    IMAP_MessageInfo[] messagesInfo = e.MessagesInfo;
                    for (int i = 0; i < messagesInfo.Length; i++)
                    {
                        IMAP_MessageInfo iMAP_MessageInfo = messagesInfo[i];
                        this.m_pApi.CopyMessage(iMAP_Session.AuthenticatedUserIdentity.Name, iMAP_Session.AuthenticatedUserIdentity.Name, e.SourceFolder, iMAP_Session.AuthenticatedUserIdentity.Name, e.TargetFolder, iMAP_MessageInfo);
                        list.Add(iMAP_MessageInfo);
                    }
                }
                catch (Exception ex)
                {
                    foreach (IMAP_MessageInfo current in list)
                    {
                        try
                        {
                            this.m_pApi.DeleteMessage(iMAP_Session.AuthenticatedUserIdentity.Name, iMAP_Session.AuthenticatedUserIdentity.Name, e.TargetFolder, current.ID, (int)current.UID);
                        }
                        catch
                        {
                        }
                    }
                    throw ex;
                }
            }
            catch (Exception ex2)
            {
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag, "NO", "Error: " + ex2.Message);
            }
        }

        private void IMAP_Server_SessionLog(object sender, WriteLogEventArgs e)
        {
            Logger.WriteLog(this.m_IMAP_LogPath + "imap-" + DateTime.Today.ToString("yyyyMMdd") + ".log", e.LogEntry);
        }

        private void m_pSipServer_Authenticate(SIP_AuthenticateEventArgs e)
        {
            foreach (DataRowView dataRowView in this.m_pApi.GetUsers("ALL"))
            {
                if (e.AuthContext.UserName.ToLower() == dataRowView["UserName"].ToString().ToLower())
                {
                    if ((Convert.ToInt32(dataRowView["Permissions"]) & 16) == 0)
                    {
                        return;
                    }
                    e.Authenticated = e.AuthContext.Authenticate(dataRowView["UserName"].ToString(), dataRowView["Password"].ToString());
                    return;
                }
            }
            e.Authenticated = false;
        }

        private bool m_pSipServer_IsLocalUri(string uri)
        {
            return this.m_pApi.DomainExists(uri);
        }

        private bool m_pSipServer_AddressExists(string address)
        {
            return this.m_pApi.MapUser(address) != null;
        }

        private bool m_pSipServer_CanRegister(string userName, string address)
        {
            foreach (DataRowView dataRowView in this.m_pApi.GetUserAddresses(userName))
            {
                if (dataRowView["Address"].ToString().ToLower() == address.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        private void m_pSipServer_Error(object sender, ExceptionEventArgs e)
        {
            this.OnError(e.Exception);
        }

        private void OnServer_SysError(object sender, Error_EventArgs e)
        {
            this.OnError(e.Exception);
        }

        private void m_pTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                this.LoadSettings();
            }
            catch (Exception x)
            {
                this.OnError(x);
            }
        }

        public void Start()
        {
            if (this.m_Running)
            {
                return;
            }
            this.m_Running = true;
            this.m_pDnsClient = new Dns_Client();
            this.m_pSMTP_Server = new SMTP_Server();
            this.m_pSMTP_Server.Error += new System.NetworkToolkit.ErrorEventHandler(this.OnServer_SysError);
            this.m_pSMTP_Server.SessionCreated += new EventHandler<TCP_ServerSessionEventArgs<SMTP_Session>>(this.m_pSMTP_Server_SessionCreated);
            this.m_pPop3Server = new POP3_Server();
            this.m_pPop3Server.Error += new System.NetworkToolkit.ErrorEventHandler(this.OnServer_SysError);
            this.m_pPop3Server.SessionCreated += new EventHandler<TCP_ServerSessionEventArgs<POP3_Session>>(this.m_pPop3Server_SessionCreated);
            this.m_pImapServer = new IMAP_Server();
            this.m_pImapServer.Error += new System.NetworkToolkit.ErrorEventHandler(this.OnServer_SysError);
            this.m_pImapServer.SessionCreated += new EventHandler<TCP_ServerSessionEventArgs<IMAP_Session>>(this.m_pImapServer_SessionCreated);
            this.m_pRelayServer = new RelayServer(this);
            this.m_pRelayServer.DnsClient = this.m_pDnsClient;
            this.m_pFetchServer = new FetchPop3(this, this.m_pApi);
            this.m_pSipServer = new SIP_Proxy(new SIP_Stack());
            this.m_pSipServer.Authenticate += new SIP_AuthenticateEventHandler(this.m_pSipServer_Authenticate);
            this.m_pSipServer.IsLocalUri += new SIP_IsLocalUriEventHandler(this.m_pSipServer_IsLocalUri);
            this.m_pSipServer.AddressExists += new SIP_AddressExistsEventHandler(this.m_pSipServer_AddressExists);
            this.m_pSipServer.Registrar.CanRegister += new SIP_CanRegisterEventHandler(this.m_pSipServer_CanRegister);
            this.m_pSipServer.Stack.Error += new EventHandler<ExceptionEventArgs>(this.m_pSipServer_Error);
            this.m_pRecycleBinManager = new RecycleBinManager(this.m_pApi);
            this.m_pBadLoginManager = new BadLoginManager();
            this.m_pTimer = new Timer();
            this.m_pTimer.Interval = 15000.0;
            this.m_pTimer.Elapsed += new ElapsedEventHandler(this.m_pTimer_Elapsed);
            this.m_pTimer.Enabled = true;
            this.LoadSettings();
        }

        public void Stop()
        {
            this.m_Running = false;
            if (this.m_pDnsClient != null)
            {
                this.m_pDnsClient.Dispose();
                this.m_pDnsClient = null;
            }
            if (this.m_pSMTP_Server != null)
            {
                try
                {
                    this.m_pSMTP_Server.Dispose();
                }
                catch
                {
                }
                this.m_pSMTP_Server = null;
            }
            if (this.m_pPop3Server != null)
            {
                try
                {
                    this.m_pPop3Server.Dispose();
                }
                catch
                {
                }
                this.m_pPop3Server = null;
            }
            if (this.m_pImapServer != null)
            {
                try
                {
                    this.m_pImapServer.Dispose();
                }
                catch
                {
                }
                this.m_pImapServer = null;
            }
            if (this.m_pRelayServer != null)
            {
                try
                {
                    this.m_pRelayServer.Dispose();
                }
                catch
                {
                }
                this.m_pRelayServer = null;
            }
            if (this.m_pFetchServer != null)
            {
                try
                {
                    this.m_pFetchServer.Dispose();
                }
                catch
                {
                }
                this.m_pFetchServer = null;
            }
            if (this.m_pSipServer != null)
            {
                try
                {
                    this.m_pSipServer.Stack.Stop();
                }
                catch
                {
                }
                this.m_pSipServer = null;
            }
            if (this.m_pTimer != null)
            {
                try
                {
                    this.m_pTimer.Dispose();
                }
                catch
                {
                }
                this.m_pTimer = null;
            }
            if (this.m_pRecycleBinManager != null)
            {
                try
                {
                    this.m_pRecycleBinManager.Dispose();
                }
                catch
                {
                }
                this.m_pRecycleBinManager = null;
            }
            if (this.m_pBadLoginManager != null)
            {
                try
                {
                    this.m_pBadLoginManager.Dispose();
                }
                catch
                {
                }
                this.m_pBadLoginManager = null;
            }
        }

        public bool Authenticate(IPAddress ip, string userName, string password)
        {
            if (ip == null)
            {
                throw new ArgumentNullException("ip");
            }
            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }
            bool result;
            try
            {
                if (this.m_pBadLoginManager.IsExceeded(ip.ToString(), userName))
                {
                    result = false;
                }
                else
                {
                    bool flag = false;
                    if (this.m_AuthType == MailServerAuthType.Integrated)
                    {
                        IEnumerator enumerator = this.m_pApi.GetUsers("ALL").GetEnumerator();
                        try
                        {
                            while (enumerator.MoveNext())
                            {
                                DataRowView dataRowView = (DataRowView)enumerator.Current;
                                if (userName.ToLowerInvariant() == dataRowView["UserName"].ToString().ToLowerInvariant())
                                {
                                    if (password == dataRowView["Password"].ToString())
                                    {
                                        flag = true;
                                        break;
                                    }
                                    break;
                                }
                            }
                            goto IL_14D;
                        }
                        finally
                        {
                            IDisposable disposable = enumerator as IDisposable;
                            if (disposable != null)
                            {
                                disposable.Dispose();
                            }
                        }
                    }
                    if (this.m_AuthType == MailServerAuthType.Windows)
                    {
                        if (this.m_pApi.UserExists(userName))
                        {
                            flag = WinLogon.Logon(this.m_Auth_Win_Domain, userName, password);
                        }
                    }
                    else if (this.m_AuthType == MailServerAuthType.Ldap)
                    {
                        try
                        {
                            string userName2 = this.m_Auth_LDAP_DN.Replace("%user", userName);
                            using (LdapConnection ldapConnection = new LdapConnection(new LdapDirectoryIdentifier(this.m_Auth_LDAP_Server), new NetworkCredential(userName2, password), System.DirectoryServices.Protocols.AuthType.Basic))
                            {
                                ldapConnection.SessionOptions.ProtocolVersion = 3;
                                ldapConnection.Bind();
                            }
                            flag = true;
                        }
                        catch
                        {
                        }
                    }
                    IL_14D:
                    if (!flag)
                    {
                        this.m_pBadLoginManager.Put(ip.ToString(), userName);
                    }
                    if (flag)
                    {
                        this.m_pApi.UpdateUserLastLoginTime(userName);
                    }
                    result = flag;
                }
            }
            catch (Exception x)
            {
                this.OnError(x);
                result = false;
            }
            return result;
        }

        private void LoadSettings()
        {
            try
            {
                lock (this)
                {
                    DataRow settings = this.m_pApi.GetSettings();
                    if (!Convert.ToDateTime(settings["SettingsDate"]).Equals(this.m_SettingsDate))
                    {
                        this.m_SettingsDate = Convert.ToDateTime(settings["SettingsDate"]);
                        this.m_MailStorePath = "Settings\\MailStore";
                        string[] array = this.m_ApiInitString.Replace("\r\n", "\n").Split(new char[]
                        {
                            '\n'
                        });
                        string[] array2 = array;
                        for (int i = 0; i < array2.Length; i++)
                        {
                            string text = array2[i];
                            if (text.ToLower().IndexOf("mailstorepath=") > -1)
                            {
                                this.m_MailStorePath = text.Substring(14);
                            }
                        }
                        if (this.m_MailStorePath.Length > 0 && !this.m_MailStorePath.EndsWith("\\"))
                        {
                            this.m_MailStorePath += "\\";
                        }
                        if (!Path.IsPathRooted(this.m_MailStorePath))
                        {
                            this.m_MailStorePath = this.m_pOwnerServer.StartupPath + this.m_MailStorePath;
                        }
                        this.m_MailStorePath = PathHelper.PathFix(this.m_MailStorePath);
                        this.m_AuthType = (MailServerAuthType)ConvertEx.ToInt32(settings["ServerAuthenticationType"]);
                        this.m_Auth_Win_Domain = ConvertEx.ToString(settings["ServerAuthWinDomain"]);
                        this.m_Auth_LDAP_Server = ConvertEx.ToString(settings["LdapServer"]);
                        this.m_Auth_LDAP_DN = ConvertEx.ToString(settings["LdapDN"]);
                        List<string> list = new List<string>();
                        foreach (DataRow dataRow in settings.Table.DataSet.Tables["DnsServers"].Rows)
                        {
                            list.Add(dataRow["IP"].ToString());
                        }
                        Dns_Client.DnsServers = list.ToArray();
                        try
                        {
                            List<IPBindInfo> list2 = new List<IPBindInfo>();
                            foreach (DataRow dataRow2 in settings.Table.DataSet.Tables["SMTP_Bindings"].Rows)
                            {
                                list2.Add(new IPBindInfo(ConvertEx.ToString(dataRow2["HostName"]), IPAddress.Parse(ConvertEx.ToString(dataRow2["IP"])), ConvertEx.ToInt32(dataRow2["Port"]), this.ParseSslMode(dataRow2["SSL"].ToString()), this.PaseCertificate(dataRow2["SSL_Certificate"])));
                            }
                            this.m_pSMTP_Server.Bindings = list2.ToArray();
                            this.m_pSMTP_Server.MaxConnections = (long)ConvertEx.ToInt32(settings["SMTP_Threads"]);
                            this.m_pSMTP_Server.MaxConnectionsPerIP = (long)ConvertEx.ToInt32(settings["SMTP_MaxConnectionsPerIP"]);
                            this.m_pSMTP_Server.SessionIdleTimeout = ConvertEx.ToInt32(settings["SMTP_SessionIdleTimeOut"]);
                            this.m_pSMTP_Server.MaxMessageSize = ConvertEx.ToInt32(settings["MaxMessageSize"]) * 1000000;
                            this.m_pSMTP_Server.MaxRecipients = ConvertEx.ToInt32(settings["MaxRecipients"]);
                            this.m_pSMTP_Server.MaxBadCommands = ConvertEx.ToInt32(settings["SMTP_MaxBadCommands"]);
                            this.m_pSMTP_Server.MaxTransactions = ConvertEx.ToInt32(settings["SMTP_MaxTransactions"]);
                            this.m_pSMTP_Server.GreetingText = ConvertEx.ToString(settings["SMTP_GreetingText"]);
                            this.m_pSMTP_Server.ServiceExtentions = new string[]
                            {
                                SMTP_ServiceExtensions.PIPELINING,
                                SMTP_ServiceExtensions.SIZE,
                                SMTP_ServiceExtensions.STARTTLS,
                                SMTP_ServiceExtensions._8BITMIME,
                                SMTP_ServiceExtensions.BINARYMIME,
                                SMTP_ServiceExtensions.CHUNKING,
                                SMTP_ServiceExtensions.DSN
                            };
                            this.m_SMTP_RequireAuth = ConvertEx.ToBoolean(settings["SMTP_RequireAuth"]);
                            this.m_SMTP_DefaultDomain = ConvertEx.ToString(settings["SMTP_DefaultDomain"]);
                            if (ConvertEx.ToBoolean(settings["SMTP_Enabled"]))
                            {
                                this.m_pSMTP_Server.Start();
                            }
                            else
                            {
                                this.m_pSMTP_Server.Stop();
                            }
                        }
                        catch (Exception x)
                        {
                            this.OnError(x);
                        }
                        try
                        {
                            List<IPBindInfo> list3 = new List<IPBindInfo>();
                            foreach (DataRow dataRow3 in settings.Table.DataSet.Tables["POP3_Bindings"].Rows)
                            {
                                list3.Add(new IPBindInfo(ConvertEx.ToString(dataRow3["HostName"]), IPAddress.Parse(ConvertEx.ToString(dataRow3["IP"])), ConvertEx.ToInt32(dataRow3["Port"]), this.ParseSslMode(dataRow3["SSL"].ToString()), this.PaseCertificate(dataRow3["SSL_Certificate"])));
                            }
                            this.m_pPop3Server.Bindings = list3.ToArray();
                            this.m_pPop3Server.MaxConnections = (long)ConvertEx.ToInt32(settings["POP3_Threads"]);
                            this.m_pPop3Server.MaxConnectionsPerIP = (long)ConvertEx.ToInt32(settings["POP3_MaxConnectionsPerIP"]);
                            this.m_pPop3Server.SessionIdleTimeout = ConvertEx.ToInt32(settings["POP3_SessionIdleTimeOut"]);
                            this.m_pPop3Server.MaxBadCommands = ConvertEx.ToInt32(settings["POP3_MaxBadCommands"]);
                            this.m_pPop3Server.GreetingText = ConvertEx.ToString(settings["POP3_GreetingText"]);
                            if (ConvertEx.ToBoolean(settings["POP3_Enabled"]))
                            {
                                this.m_pPop3Server.Start();
                            }
                            else
                            {
                                this.m_pPop3Server.Stop();
                            }
                        }
                        catch (Exception x2)
                        {
                            this.OnError(x2);
                        }
                        try
                        {
                            List<IPBindInfo> list4 = new List<IPBindInfo>();
                            foreach (DataRow dataRow4 in settings.Table.DataSet.Tables["IMAP_Bindings"].Rows)
                            {
                                list4.Add(new IPBindInfo(ConvertEx.ToString(dataRow4["HostName"]), IPAddress.Parse(ConvertEx.ToString(dataRow4["IP"])), ConvertEx.ToInt32(dataRow4["Port"]), this.ParseSslMode(dataRow4["SSL"].ToString()), this.PaseCertificate(dataRow4["SSL_Certificate"])));
                            }
                            this.m_pImapServer.Bindings = list4.ToArray();
                            this.m_pImapServer.MaxConnections = (long)ConvertEx.ToInt32(settings["IMAP_Threads"]);
                            this.m_pImapServer.MaxConnectionsPerIP = (long)ConvertEx.ToInt32(settings["IMAP_Threads"]);
                            this.m_pImapServer.SessionIdleTimeout = ConvertEx.ToInt32(settings["IMAP_SessionIdleTimeOut"]);
                            this.m_pImapServer.MaxBadCommands = ConvertEx.ToInt32(settings["IMAP_MaxBadCommands"]);
                            this.m_pImapServer.GreetingText = ConvertEx.ToString(settings["IMAP_GreetingText"]);
                            if (ConvertEx.ToBoolean(settings["IMAP_Enabled"]))
                            {
                                this.m_pImapServer.Start();
                            }
                            else
                            {
                                this.m_pImapServer.Stop();
                            }
                        }
                        catch (Exception x3)
                        {
                            this.OnError(x3);
                        }
                        try
                        {
                            List<IPBindInfo> list5 = new List<IPBindInfo>();
                            foreach (DataRow dataRow5 in settings.Table.DataSet.Tables["Relay_Bindings"].Rows)
                            {
                                list5.Add(new IPBindInfo(ConvertEx.ToString(dataRow5["HostName"]), IPAddress.Parse(ConvertEx.ToString(dataRow5["IP"])), 0, SslMode.None, null));
                            }
                            List<Relay_SmartHost> list6 = new List<Relay_SmartHost>();
                            foreach (DataRow dataRow6 in settings.Table.DataSet.Tables["Relay_SmartHosts"].Rows)
                            {
                                list6.Add(new Relay_SmartHost(ConvertEx.ToString(dataRow6["Host"]), ConvertEx.ToInt32(dataRow6["Port"]), (SslMode)Enum.Parse(typeof(SslMode), dataRow6["SslMode"].ToString()), ConvertEx.ToString(dataRow6["UserName"]), ConvertEx.ToString(dataRow6["Password"])));
                            }
                            this.m_pRelayServer.RelayMode = (Relay_Mode)Enum.Parse(typeof(Relay_Mode), settings["Relay_Mode"].ToString());
                            this.m_pRelayServer.SmartHostsBalanceMode = (BalanceMode)Enum.Parse(typeof(BalanceMode), settings["Relay_SmartHostsBalanceMode"].ToString());
                            this.m_pRelayServer.SmartHosts = list6.ToArray();
                            this.m_pRelayServer.SessionIdleTimeout = ConvertEx.ToInt32(settings["Relay_SessionIdleTimeOut"]);
                            this.m_pRelayServer.MaxConnections = (long)ConvertEx.ToInt32(settings["MaxRelayThreads"]);
                            this.m_pRelayServer.MaxConnectionsPerIP = (long)ConvertEx.ToInt32(settings["Relay_MaxConnectionsPerIP"]);
                            this.m_pRelayServer.RelayInterval = ConvertEx.ToInt32(settings["RelayInterval"]);
                            this.m_pRelayServer.RelayRetryInterval = ConvertEx.ToInt32(settings["RelayRetryInterval"]);
                            this.m_pRelayServer.DelayedDeliveryNotifyAfter = ConvertEx.ToInt32(settings["RelayUndeliveredWarning"]);
                            this.m_pRelayServer.UndeliveredAfter = ConvertEx.ToInt32(settings["RelayUndelivered"]) * 60;
                            this.m_pRelayServer.DelayedDeliveryMessage = null;
                            this.m_pRelayServer.UndeliveredMessage = null;
                            foreach (DataRow dataRow7 in settings.Table.DataSet.Tables["ServerReturnMessages"].Rows)
                            {
                                if (dataRow7["MessageType"].ToString() == "delayed_delivery_warning")
                                {
                                    this.m_pRelayServer.DelayedDeliveryMessage = new ServerReturnMessage(dataRow7["Subject"].ToString(), dataRow7["BodyTextRtf"].ToString());
                                }
                                else if (dataRow7["MessageType"].ToString() == "undelivered")
                                {
                                    this.m_pRelayServer.UndeliveredMessage = new ServerReturnMessage(dataRow7["Subject"].ToString(), dataRow7["BodyTextRtf"].ToString());
                                }
                            }
                            this.m_pRelayServer.Bindings = list5.ToArray();
                            if (ConvertEx.ToBoolean(settings["LogRelayCmds"]))
                            {
                                this.m_pRelayServer.Logger = new System.NetworkToolkit.Log.Logger();
                                this.m_pRelayServer.Logger.WriteLog += new EventHandler<WriteLogEventArgs>(this.m_pRelayServer_WriteLog);
                            }
                            else if (this.m_pRelayServer.Logger != null)
                            {
                                this.m_pRelayServer.Logger.Dispose();
                                this.m_pRelayServer.Logger = null;
                            }
                            if (settings["Relay_LogPath"].ToString().Length == 0)
                            {
                                this.m_Relay_LogPath = this.m_pOwnerServer.StartupPath + "Logs\\Relay\\";
                            }
                            else
                            {
                                this.m_Relay_LogPath = settings["Relay_LogPath"].ToString() + "\\";
                            }
                            this.m_pRelayServer.StoreUndeliveredMessages = ConvertEx.ToBoolean(settings["StoreUndeliveredMessages"]);
                            this.m_pRelayServer.UseTlsIfPossible = ConvertEx.ToBoolean(settings["Relay_UseTlsIfPossible"]);
                            if (!this.m_pRelayServer.IsRunning)
                            {
                                this.m_pRelayServer.Start();
                            }
                        }
                        catch (Exception x4)
                        {
                            this.OnError(x4);
                        }
                        try
                        {
                            this.m_pFetchServer.Enabled = ConvertEx.ToBoolean(settings["FetchPop3_Enabled"]);
                            this.m_pFetchServer.FetchInterval = ConvertEx.ToInt32(settings["FetchPOP3_Interval"]);
                        }
                        catch (Exception x5)
                        {
                            this.OnError(x5);
                        }
                        List<IPBindInfo> list7 = new List<IPBindInfo>();
                        foreach (DataRow dataRow8 in settings.Table.DataSet.Tables["SIP_Bindings"].Rows)
                        {
                            if (dataRow8["Protocol"].ToString().ToUpper() == "TCP")
                            {
                                list7.Add(new IPBindInfo(ConvertEx.ToString(dataRow8["HostName"]), IPAddress.Parse(ConvertEx.ToString(dataRow8["IP"])), ConvertEx.ToInt32(dataRow8["Port"]), this.ParseSslMode(dataRow8["SSL"].ToString()), this.PaseCertificate(dataRow8["SSL_Certificate"])));
                            }
                            else
                            {
                                list7.Add(new IPBindInfo(ConvertEx.ToString(dataRow8["HostName"]), BindInfoProtocol.UDP, IPAddress.Parse(ConvertEx.ToString(dataRow8["IP"])), ConvertEx.ToInt32(dataRow8["Port"])));
                            }
                        }
                        this.m_pSipServer.Stack.BindInfo = list7.ToArray();
                        if (ConvertEx.ToBoolean(settings["SIP_Enabled"]))
                        {
                            this.m_pSipServer.Stack.Start();
                        }
                        else
                        {
                            this.m_pSipServer.Stack.Stop();
                        }
                        this.m_pSipServer.Stack.MinimumExpireTime = ConvertEx.ToInt32(settings["SIP_MinExpires"]);
                        this.m_pSipServer.ProxyMode = (SIP_ProxyMode)Enum.Parse(typeof(SIP_ProxyMode), settings["SIP_ProxyMode"].ToString());
                        try
                        {
                            if (ConvertEx.ToBoolean(settings["LogSMTPCmds"], false))
                            {
                                this.m_pSMTP_Server.Logger = new System.NetworkToolkit.Log.Logger();
                                this.m_pSMTP_Server.Logger.WriteLog += new EventHandler<WriteLogEventArgs>(this.SMTP_Server_SessionLog);
                            }
                            else
                            {
                                this.m_pSMTP_Server.Logger = null;
                            }
                            if (ConvertEx.ToBoolean(settings["LogPOP3Cmds"], false))
                            {
                                this.m_pPop3Server.Logger = new System.NetworkToolkit.Log.Logger();
                                this.m_pPop3Server.Logger.WriteLog += new EventHandler<WriteLogEventArgs>(this.POP3_Server_SessionLog);
                            }
                            else
                            {
                                this.m_pPop3Server.Logger = null;
                            }
                            if (ConvertEx.ToBoolean(settings["LogIMAPCmds"], false))
                            {
                                this.m_pImapServer.Logger = new System.NetworkToolkit.Log.Logger();
                                this.m_pImapServer.Logger.WriteLog += new EventHandler<WriteLogEventArgs>(this.IMAP_Server_SessionLog);
                            }
                            else
                            {
                                this.m_pImapServer.Logger = null;
                            }
                            this.m_pFetchServer.LogCommands = ConvertEx.ToBoolean(settings["LogFetchPOP3Cmds"], false);
                            this.m_SMTP_LogPath = PathHelper.PathFix(ConvertEx.ToString(settings["SMTP_LogPath"]) + "\\");
                            this.m_POP3_LogPath = PathHelper.PathFix(ConvertEx.ToString(settings["POP3_LogPath"]) + "\\");
                            this.m_IMAP_LogPath = PathHelper.PathFix(ConvertEx.ToString(settings["IMAP_LogPath"]) + "\\");
                            this.m_Server_LogPath = PathHelper.PathFix(ConvertEx.ToString(settings["Server_LogPath"]) + "\\");
                            this.m_Fetch_LogPath = PathHelper.PathFix(ConvertEx.ToString(settings["FetchPOP3_LogPath"]) + "\\");
                            if (settings["SMTP_LogPath"].ToString().Trim().Length == 0)
                            {
                                this.m_SMTP_LogPath = "Logs\\SMTP\\";
                            }
                            if (settings["POP3_LogPath"].ToString().Trim().Length == 0)
                            {
                                this.m_POP3_LogPath = "Logs\\POP3\\";
                            }
                            if (settings["IMAP_LogPath"].ToString().Trim().Length == 0)
                            {
                                this.m_IMAP_LogPath = "Logs\\IMAP\\";
                            }
                            if (settings["Server_LogPath"].ToString().Trim().Length == 0)
                            {
                                this.m_Server_LogPath = "Logs\\Server\\";
                            }
                            if (settings["FetchPOP3_LogPath"].ToString().Trim().Length == 0)
                            {
                                this.m_Fetch_LogPath = "Logs\\FetchPOP3\\";
                            }

                            var basePath = new DirectoryInfo(this.MailStorePath).Parent.FullName;
                            this.m_SMTP_LogPath = Path.Combine(basePath, m_SMTP_LogPath);
                            this.m_POP3_LogPath = Path.Combine(basePath, m_POP3_LogPath);
                            this.m_IMAP_LogPath = Path.Combine(basePath, m_IMAP_LogPath);
                            this.m_Fetch_LogPath = Path.Combine(basePath, m_Fetch_LogPath);

                            this.m_pFetchServer.LogPath = this.m_Fetch_LogPath;
                        }
                        catch (Exception x6)
                        {
                            this.OnError(x6);
                        }
                    }
                }
            }
            catch (Exception x7)
            {
                Error.DumpError(x7);
            }
        }

        private bool IsRelayAllowed(string userName, IPAddress ip)
        {
            if (userName != null && userName.Length > 0 && (this.m_pApi.GetUserPermissions(userName) & UserPermissions.Relay) != UserPermissions.None)
            {
                return true;
            }
            using (DataView securityList = this.m_pApi.GetSecurityList())
            {
                foreach (DataRowView dataRowView in securityList)
                {
                    if (Convert.ToBoolean(dataRowView["Enabled"]) && Convert.ToInt32(dataRowView["Service"]) == 4 && Convert.ToInt32(dataRowView["Action"]) == 2 && Net_Utils.CompareIP(IPAddress.Parse(dataRowView["StartIP"].ToString()), ip) >= 0 && Net_Utils.CompareIP(IPAddress.Parse(dataRowView["EndIP"].ToString()), ip) <= 0)
                    {
                        bool result = false;
                        return result;
                    }
                }
                foreach (DataRowView dataRowView2 in securityList)
                {
                    if (Convert.ToBoolean(dataRowView2["Enabled"]) && Convert.ToInt32(dataRowView2["Service"]) == 4 && Convert.ToInt32(dataRowView2["Action"]) == 1 && Net_Utils.CompareIP(IPAddress.Parse(dataRowView2["StartIP"].ToString()), ip) >= 0 && Net_Utils.CompareIP(IPAddress.Parse(dataRowView2["EndIP"].ToString()), ip) <= 0)
                    {
                        bool result = true;
                        return result;
                    }
                }
            }
            return false;
        }

        public bool IsAccessAllowed(ServiceType service, IPAddress ip)
        {
            using (DataView securityList = this.m_pApi.GetSecurityList())
            {
                foreach (DataRowView dataRowView in securityList)
                {
                    if (Convert.ToBoolean(dataRowView["Enabled"]) && Convert.ToInt32(dataRowView["Service"]) == (int)service && Convert.ToInt32(dataRowView["Action"]) == 2 && Net_Utils.CompareIP(IPAddress.Parse(dataRowView["StartIP"].ToString()), ip) >= 0 && Net_Utils.CompareIP(IPAddress.Parse(dataRowView["EndIP"].ToString()), ip) <= 0)
                    {
                        bool result = false;
                        return result;
                    }
                }
                foreach (DataRowView dataRowView2 in securityList)
                {
                    if (Convert.ToBoolean(dataRowView2["Enabled"]) && Convert.ToInt32(dataRowView2["Service"]) == (int)service && Convert.ToInt32(dataRowView2["Action"]) == 1 && Net_Utils.CompareIP(IPAddress.Parse(dataRowView2["StartIP"].ToString()), ip) >= 0 && Net_Utils.CompareIP(IPAddress.Parse(dataRowView2["EndIP"].ToString()), ip) <= 0)
                    {
                        bool result = true;
                        return result;
                    }
                }
            }
            return false;
        }

        private X509Certificate2 PaseCertificate(object cert)
        {
            if (cert == null)
            {
                return null;
            }
            if (cert == DBNull.Value)
            {
                return null;
            }
            string tempFileName = Path.GetTempFileName();
            X509Certificate2 result;
            try
            {
                using (FileStream fileStream = File.Open(tempFileName, FileMode.Open))
                {
                    fileStream.Write((byte[])cert, 0, ((byte[])cert).Length);
                }
                X509Certificate2 x509Certificate = new X509Certificate2(tempFileName);
                result = x509Certificate;
            }
            finally
            {
                File.Delete(tempFileName);
            }
            return result;
        }

        private SslMode ParseSslMode(string value)
        {
            if (value.ToLower() == "false")
            {
                return SslMode.None;
            }
            if (value.ToLower() == "true")
            {
                return SslMode.SSL;
            }
            return (SslMode)Enum.Parse(typeof(SslMode), value);
        }

        private void FillUserInfo(AUTH_e_UserInfo userInfo)
        {
            if (userInfo == null)
            {
                throw new ArgumentNullException("userInfo");
            }
            try
            {
                foreach (DataRowView dataRowView in this.m_pApi.GetUsers(""))
                {
                    if (userInfo.UserName.ToLowerInvariant() == dataRowView["UserName"].ToString().ToLowerInvariant())
                    {
                        userInfo.UserExists = true;
                        userInfo.Password = dataRowView["Password"].ToString();
                        break;
                    }
                }
            }
            catch (Exception x)
            {
                this.OnError(x);
            }
        }

        public void ProcessAndStoreMessage(string sender, string[] recipient, Stream msgStream, SMTP_e_MessageStored e)
        {
            List<SMTP_RcptTo> list = new List<SMTP_RcptTo>();
            for (int i = 0; i < recipient.Length; i++)
            {
                string mailbox = recipient[i];
                list.Add(new SMTP_RcptTo(mailbox, SMTP_DSN_Notify.NotSpecified, null));
            }
            this.ProcessAndStoreMessage(null, sender, SMTP_DSN_Ret.NotSpecified, list.ToArray(), msgStream, e);
        }

        public void ProcessAndStoreMessage(string envelopeID, string sender, SMTP_DSN_Ret ret, SMTP_RcptTo[] recipients, Stream msgStream, SMTP_e_MessageStored e)
        {
            if (recipients == null)
            {
                throw new ArgumentNullException("recipients");
            }
            if (msgStream == null)
            {
                throw new ArgumentNullException("msgStream");
            }
            List<SMTP_RcptTo> list = new List<SMTP_RcptTo>();
            string[] array = new string[recipients.Length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = recipients[i].Mailbox;
            }
            Stream stream = msgStream;
            DataView filters = this.m_pApi.GetFilters();
            filters.RowFilter = "Enabled=true AND Type='ISmtpMessageFilter'";
            filters.Sort = "Cost";
            foreach (DataRowView dataRowView in filters)
            {
                try
                {
                    stream.Position = 0L;
                    string text = PathHelper.PathFix(dataRowView.Row["Assembly"].ToString());
                    if (!File.Exists(text))
                    {
                        text = Path.Combine(this.m_pOwnerServer.StartupPath, text);
                    }
                    Assembly assembly = Assembly.LoadFrom(text);
                    Type type = assembly.ExportedTypes.FirstOrDefault((Type x) => typeof(ISmtpMessageFilter).IsAssignableFrom(x) && x.FullName == dataRowView["ClassName"].ToString());
                    object obj = Activator.CreateInstance(type);
                    ISmtpMessageFilter smtpMessageFilter = (ISmtpMessageFilter)obj;
                    string str = "";
                    SMTP_Session session = null;
                    if (e != null)
                    {
                        session = e.Session;
                    }
                    FilterResult filterResult = smtpMessageFilter.Filter(stream, out stream, sender, array, this.m_pApi, session, out str);
                    if (filterResult == FilterResult.DontStore)
                    {
                        e.Reply = new SMTP_Reply(552, "Requested mail action aborted: Message discarded by server filter.");
                        return;
                    }
                    if (filterResult == FilterResult.Error)
                    {
                        if (e != null)
                        {
                            e.Reply = new SMTP_Reply(552, "Requested mail action aborted: " + str);
                        }
                        return;
                    }
                    if (stream == null)
                    {
                        e.Reply = new SMTP_Reply(552, "Requested mail action aborted: Message discarded by server filter.");
                        return;
                    }
                }
                catch (Exception x)
                {
                    this.OnError(x);
                }
            }
            stream.Position = 0L;
            Mail_Message mail_Message = null;
            try
            {
                mail_Message = Mail_Message.ParseFromStream(stream);
            }
            catch
            {
                e.Reply = new SMTP_Reply(552, "Requested mail action aborted: Message has invalid structure/syntax.");
                try
                {
                    if (!Directory.Exists(this.MailStorePath + "Unparseable"))
                    {
                        Directory.CreateDirectory(this.MailStorePath + "Unparseable");
                    }
                    using (FileStream fileStream = File.Create(this.MailStorePath + "Unparseable\\" + Guid.NewGuid().ToString().Replace("-", "") + ".eml"))
                    {
                        stream.Position = 0L;
                        Net_Utils.StreamCopy(stream, fileStream, 32000);
                    }
                }
                catch
                {
                }
                return;
            }
            bool flag = false;
            string storeFolder = "Inbox";
            string text2 = null;
            foreach (DataRowView dataRowView2 in this.m_pApi.GetGlobalMessageRules())
            {
                stream.Position = 0L;
                if (Convert.ToBoolean(dataRowView2["Enabled"]))
                {
                    string ruleID = dataRowView2["RuleID"].ToString();
                    GlobalMessageRule_CheckNextRule globalMessageRule_CheckNextRule_enum = (GlobalMessageRule_CheckNextRule)(int)dataRowView2["CheckNextRuleIf"];
                    string matchExpression = dataRowView2["MatchExpression"].ToString();
                    SMTP_Session smtpSession = null;
                    if (e != null)
                    {
                        smtpSession = e.Session;
                    }
                    GlobalMessageRuleProcessor globalMessageRuleProcessor = new GlobalMessageRuleProcessor();
                    bool flag2 = globalMessageRuleProcessor.Match(matchExpression, sender, array, smtpSession, mail_Message, (int)stream.Length);
                    if (flag2)
                    {
                        GlobalMessageRuleActionResult globalMessageRuleActionResult = globalMessageRuleProcessor.DoActions(this.m_pApi.GetGlobalMessageRuleActions(ruleID), this, stream, sender, array);
                        if (globalMessageRuleActionResult.DeleteMessage)
                        {
                            flag = true;
                        }
                        if (globalMessageRuleActionResult.StoreFolder != null)
                        {
                            storeFolder = globalMessageRuleActionResult.StoreFolder;
                        }
                        if (globalMessageRuleActionResult.ErrorText != null)
                        {
                            text2 = globalMessageRuleActionResult.ErrorText;
                        }
                    }
                    if (globalMessageRule_CheckNextRule_enum != GlobalMessageRule_CheckNextRule.Always && ((globalMessageRule_CheckNextRule_enum == (GlobalMessageRule_CheckNextRule)1 && !flag2) || (globalMessageRule_CheckNextRule_enum == (GlobalMessageRule_CheckNextRule)2 && flag2)))
                    {
                        break;
                    }
                }
            }
            if (text2 != null)
            {
                e.Reply = new SMTP_Reply(552, "Requested mail action aborted: " + text2);
            }
            else
            {
                if (flag)
                {
                    return;
                }
                stream.Position = 0L;
                HashSet<string> hashSet = new HashSet<string>();
                Queue<SMTP_RcptTo> queue = new Queue<SMTP_RcptTo>();
                for (int j = 0; j < recipients.Length; j++)
                {
                    SMTP_RcptTo item = recipients[j];
                    queue.Enqueue(item);
                }
                while (queue.Count > 0)
                {
                    SMTP_RcptTo sMTP_RcptTo = queue.Dequeue();
                    if (!hashSet.Contains(sMTP_RcptTo.Mailbox))
                    {
                        hashSet.Add(sMTP_RcptTo.Mailbox);
                        if (sMTP_RcptTo.Mailbox.IndexOf('@') == -1 && this.m_pApi.UserExists(sMTP_RcptTo.Mailbox))
                        {
                            hashSet.Add(sMTP_RcptTo.Mailbox);
                            if ((sMTP_RcptTo.Notify & SMTP_DSN_Notify.Success) != SMTP_DSN_Notify.NotSpecified)
                            {
                                list.Add(sMTP_RcptTo);
                            }
                            this.ProcessUserMsg(sender, sMTP_RcptTo.Mailbox, sMTP_RcptTo.Mailbox, storeFolder, stream, e);
                        }
                        else
                        {
                            string text3 = this.m_pApi.MapUser(sMTP_RcptTo.Mailbox);
                            if (text3 != null)
                            {
                                hashSet.Add(text3);
                                if ((sMTP_RcptTo.Notify & SMTP_DSN_Notify.Success) != SMTP_DSN_Notify.NotSpecified)
                                {
                                    list.Add(sMTP_RcptTo);
                                }
                                this.ProcessUserMsg(sender, sMTP_RcptTo.Mailbox, text3, storeFolder, stream, e);
                            }
                            else if (this.m_pApi.MailingListExists(sMTP_RcptTo.Mailbox))
                            {
                                if ((sMTP_RcptTo.Notify & SMTP_DSN_Notify.Success) != SMTP_DSN_Notify.NotSpecified)
                                {
                                    list.Add(sMTP_RcptTo);
                                }
                                Queue<string> queue2 = new Queue<string>();
                                queue2.Enqueue(sMTP_RcptTo.Mailbox);
                                while (queue2.Count > 0)
                                {
                                    string mailingListName = queue2.Dequeue();
                                    foreach (DataRowView dataRowView3 in this.m_pApi.GetMailingListAddresses(mailingListName))
                                    {
                                        string text4 = dataRowView3["Address"].ToString();
                                        if (text4.IndexOf('*') > -1)
                                        {
                                            DataView userAddresses = this.m_pApi.GetUserAddresses("");
                                            IEnumerator enumerator2 = userAddresses.GetEnumerator();
                                            try
                                            {
                                                while (enumerator2.MoveNext())
                                                {
                                                    DataRowView dataRowView4 = (DataRowView)enumerator2.Current;
                                                    string text5 = dataRowView4["Address"].ToString();
                                                    if (SCore.IsAstericMatch(text4, text5))
                                                    {
                                                        queue.Enqueue(new SMTP_RcptTo(text5, SMTP_DSN_Notify.NotSpecified, null));
                                                    }
                                                }
                                                continue;
                                            }
                                            finally
                                            {
                                                IDisposable disposable = enumerator2 as IDisposable;
                                                if (disposable != null)
                                                {
                                                    disposable.Dispose();
                                                }
                                            }
                                        }
                                        if (text4.IndexOf('@') == -1)
                                        {
                                            if (this.m_pApi.GroupExists(text4))
                                            {
                                                string[] groupUsers = this.m_pApi.GetGroupUsers(text4);
                                                for (int j = 0; j < groupUsers.Length; j++)
                                                {
                                                    string mailbox = groupUsers[j];
                                                    queue.Enqueue(new SMTP_RcptTo(mailbox, SMTP_DSN_Notify.NotSpecified, null));
                                                }
                                            }
                                            else if (this.m_pApi.UserExists(text4))
                                            {
                                                queue.Enqueue(new SMTP_RcptTo(text4, SMTP_DSN_Notify.NotSpecified, null));
                                            }
                                        }
                                        else if (this.m_pApi.MailingListExists(text4))
                                        {
                                            queue2.Enqueue(text4);
                                        }
                                        else
                                        {
                                            queue.Enqueue(new SMTP_RcptTo(text4, SMTP_DSN_Notify.NotSpecified, null));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                bool flag3 = false;
                                foreach (DataRowView dataRowView5 in this.m_pApi.GetRoutes())
                                {
                                    if (Convert.ToBoolean(dataRowView5["Enabled"]) && SCore.IsAstericMatch(dataRowView5["Pattern"].ToString(), sMTP_RcptTo.Mailbox))
                                    {
                                        string text6 = dataRowView5["Action"].ToString();
                                        RouteAction routeAction_enum = (RouteAction)Convert.ToInt32(dataRowView5["Action"]);
                                        byte[] data = (byte[])dataRowView5["ActionData"];
                                        if (routeAction_enum == RouteAction.RouteToEmail)
                                        {
                                            XmlTable xmlTable = new XmlTable("ActionData");
                                            xmlTable.Parse(data);
                                            queue.Enqueue(new SMTP_RcptTo(xmlTable.GetValue("EmailAddress"), SMTP_DSN_Notify.NotSpecified, null));
                                            if (e != null)
                                            {
                                                e.Session.LogAddText(string.Concat(new string[]
                                                {
                                                    "Route '[",
                                                    text6,
                                                    "]: ",
                                                    dataRowView5["Pattern"].ToString(),
                                                    "' routed to email '",
                                                    xmlTable.GetValue("EmailAddress"),
                                                    "'."
                                                }));
                                            }
                                        }
                                        else if (routeAction_enum == RouteAction.RouteToHost)
                                        {
                                            XmlTable xmlTable2 = new XmlTable("ActionData");
                                            xmlTable2.Parse(data);
                                            msgStream.Position = 0L;
                                            this.RelayServer.StoreRelayMessage(Guid.NewGuid().ToString(), envelopeID, msgStream, HostEndPoint.Parse(xmlTable2.GetValue("Host") + ":" + xmlTable2.GetValue("Port")), sender, sMTP_RcptTo.Mailbox, sMTP_RcptTo.ORCPT, sMTP_RcptTo.Notify, ret);
                                            if (e != null)
                                            {
                                                e.Session.LogAddText(string.Concat(new string[]
                                                {
                                                    "Route '[",
                                                    text6,
                                                    "]: ",
                                                    dataRowView5["Pattern"].ToString(),
                                                    "' routed to host '",
                                                    xmlTable2.GetValue("Host"),
                                                    ":",
                                                    xmlTable2.GetValue("Port"),
                                                    "'."
                                                }));
                                            }
                                        }
                                        else if (routeAction_enum == RouteAction.RouteToMailbox)
                                        {
                                            XmlTable xmlTable3 = new XmlTable("ActionData");
                                            xmlTable3.Parse(data);
                                            this.ProcessUserMsg(sender, sMTP_RcptTo.Mailbox, xmlTable3.GetValue("Mailbox"), storeFolder, stream, e);
                                            if (e != null)
                                            {
                                                e.Session.LogAddText(string.Concat(new string[]
                                                {
                                                    "Route '[",
                                                    text6,
                                                    "]: ",
                                                    dataRowView5["Pattern"].ToString(),
                                                    "' routed to user '",
                                                    xmlTable3.GetValue("Mailbox"),
                                                    "'."
                                                }));
                                            }
                                        }
                                        flag3 = true;
                                        break;
                                    }
                                }
                                if (!flag3)
                                {
                                    stream.Position = 0L;
                                    this.RelayServer.StoreRelayMessage(Guid.NewGuid().ToString(), envelopeID, stream, null, sender, sMTP_RcptTo.Mailbox, sMTP_RcptTo.ORCPT, sMTP_RcptTo.Notify, ret);
                                }
                            }
                        }
                    }
                }
                if (list.Count > 0 && !string.IsNullOrEmpty(sender))
                {
                    try
                    {
                        string text7 = "";
                        for (int k = 0; k < list.Count; k++)
                        {
                            if (k == list.Count - 1)
                            {
                                text7 += list[k].Mailbox;
                            }
                            else
                            {
                                text7 = text7 + list[k].Mailbox + "; ";
                            }
                        }
                        string str2;
                        if (e != null && !string.IsNullOrEmpty(e.Session.LocalHostName))
                        {
                            str2 = e.Session.LocalHostName;
                        }
                        else
                        {
                            str2 = Dns.GetHostName();
                        }
                        ServerReturnMessage serverReturnMessage = null;
                        if (serverReturnMessage == null)
                        {
                            string bodyTextRft = "{\\rtf1\\ansi\\ansicpg1257\\deff0\\deflang1061{\\fonttbl{\\f0\\froman\\fcharset0 Times New Roman;}{\\f1\froman\\fcharset186{\\*\\fname Times New Roman;}Times New Roman Baltic;}{\\f2\fswiss\\fcharset186{\\*\\fname Arial;}Arial Baltic;}}\r\n{\\colortbl ;\\red0\\green128\\blue0;\\red128\\green128\\blue128;}\r\n{\\*\\generator Msftedit 5.41.21.2508;}\\viewkind4\\uc1\\pard\\sb100\\sa100\\lang1033\\f0\\fs24\\par\r\nYour message WAS SUCCESSFULLY DELIVERED to:\\line\\lang1061\\f1\\tab\\cf1\\lang1033\\b\\f0 " + text7 + "\\line\\cf0\\b0 and you explicitly requested a delivery status notification on success.\\par\\par\r\n\\cf2 Your original message\\lang1061\\f1 /header\\lang1033\\f0  is attached to this e-mail\\lang1061\\f1 .\\lang1033\\f0\\par\\r\\n\\cf0\\line\\par\r\n\\pard\\lang1061\\f2\\fs20\\par\r\n}\r\n";
                            serverReturnMessage = new ServerReturnMessage("DSN SUCCESSFULLY DELIVERED: " + mail_Message.Subject, bodyTextRft);
                        }
                        string bodyTextRtf = serverReturnMessage.BodyTextRtf;
                        Mail_Message mail_Message2 = new Mail_Message();
                        mail_Message2.MimeVersion = "1.0";
                        mail_Message2.Date = DateTime.Now;
                        mail_Message2.From = new Mail_t_MailboxList();
                        mail_Message2.From.Add(new Mail_t_Mailbox("Mail Delivery Subsystem", "postmaster@local"));
                        mail_Message2.To = new Mail_t_AddressList();
                        mail_Message2.To.Add(new Mail_t_Mailbox(null, sender));
                        mail_Message2.Subject = serverReturnMessage.Subject;
                        MIME_h_ContentType mIME_h_ContentType = new MIME_h_ContentType(MIME_MediaTypes.Multipart.report);
                        mIME_h_ContentType.Parameters["report-type"] = "delivery-status";
                        mIME_h_ContentType.Param_Boundary = Guid.NewGuid().ToString().Replace('-', '.');
                        MIME_b_MultipartReport mIME_b_MultipartReport = new MIME_b_MultipartReport(mIME_h_ContentType);
                        mail_Message2.Body = mIME_b_MultipartReport;
                        MIME_Entity mIME_Entity = new MIME_Entity();
                        MIME_b_MultipartAlternative mIME_b_MultipartAlternative = new MIME_b_MultipartAlternative(new MIME_h_ContentType(MIME_MediaTypes.Multipart.alternative)
                        {
                            Param_Boundary = Guid.NewGuid().ToString().Replace('-', '.')
                        });
                        mIME_Entity.Body = mIME_b_MultipartAlternative;
                        mIME_b_MultipartReport.BodyParts.Add(mIME_Entity);
                        MIME_Entity mIME_Entity2 = new MIME_Entity();
                        MIME_b_Text mIME_b_Text = new MIME_b_Text(MIME_MediaTypes.Text.plain);
                        mIME_Entity2.Body = mIME_b_Text;
                        mIME_b_Text.SetText(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, SCore.RtfToText(bodyTextRtf));
                        mIME_b_MultipartAlternative.BodyParts.Add(mIME_Entity2);
                        MIME_Entity mIME_Entity3 = new MIME_Entity();
                        MIME_b_Text mIME_b_Text2 = new MIME_b_Text(MIME_MediaTypes.Text.html);
                        mIME_Entity3.Body = mIME_b_Text2;
                        mIME_b_Text2.SetText(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, SCore.RtfToHtml(bodyTextRtf));
                        mIME_b_MultipartAlternative.BodyParts.Add(mIME_Entity3);
                        MIME_Entity mIME_Entity4 = new MIME_Entity();
                        MIME_b_MessageDeliveryStatus mIME_b_MessageDeliveryStatus = new MIME_b_MessageDeliveryStatus();
                        mIME_Entity4.Body = mIME_b_MessageDeliveryStatus;
                        mIME_b_MultipartReport.BodyParts.Add(mIME_Entity4);
                        MIME_h_Collection messageFields = mIME_b_MessageDeliveryStatus.MessageFields;
                        if (!string.IsNullOrEmpty(envelopeID))
                        {
                            messageFields.Add(new MIME_h_Unstructured("Original-Envelope-Id", envelopeID));
                        }
                        messageFields.Add(new MIME_h_Unstructured("Arrival-Date", MIME_Utils.DateTimeToRfc2822(DateTime.Now)));
                        if (e != null && !string.IsNullOrEmpty(e.Session.EhloHost))
                        {
                            messageFields.Add(new MIME_h_Unstructured("Received-From-MTA", "dns;" + e.Session.EhloHost));
                        }
                        messageFields.Add(new MIME_h_Unstructured("Reporting-MTA", "dns;" + str2));
                        foreach (SMTP_RcptTo current in list)
                        {
                            MIME_h_Collection mIME_h_Collection = new MIME_h_Collection(new MIME_h_Provider());
                            if (current.ORCPT != null)
                            {
                                mIME_h_Collection.Add(new MIME_h_Unstructured("Original-Recipient", current.ORCPT));
                            }
                            mIME_h_Collection.Add(new MIME_h_Unstructured("Final-Recipient", "rfc822;" + current.Mailbox));
                            mIME_h_Collection.Add(new MIME_h_Unstructured("Action", "delivered"));
                            mIME_h_Collection.Add(new MIME_h_Unstructured("Status", "2.0.0"));
                            mIME_b_MessageDeliveryStatus.RecipientBlocks.Add(mIME_h_Collection);
                        }
                        if (mail_Message != null)
                        {
                            MIME_Entity mIME_Entity5 = new MIME_Entity();
                            MIME_b_MessageRfc822 mIME_b_MessageRfc = new MIME_b_MessageRfc822();
                            mIME_Entity5.Body = mIME_b_MessageRfc;
                            if (ret == SMTP_DSN_Ret.FullMessage)
                            {
                                mIME_b_MessageRfc.Message = mail_Message;
                            }
                            else
                            {
                                MemoryStream memoryStream = new MemoryStream();
                                mail_Message.Header.ToStream(memoryStream, null, null);
                                memoryStream.Position = 0L;
                                mIME_b_MessageRfc.Message = Mail_Message.ParseFromStream(memoryStream);
                            }
                            mIME_b_MultipartReport.BodyParts.Add(mIME_Entity5);
                        }
                        using (MemoryStream memoryStream2 = new MemoryStream())
                        {
                            mail_Message2.ToStream(memoryStream2, new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q, Encoding.UTF8), Encoding.UTF8);
                            this.ProcessAndStoreMessage("", new string[]
                            {
                                sender
                            }, memoryStream2, null);
                        }
                    }
                    catch (Exception x2)
                    {
                        Error.DumpError(this.Name, x2);
                    }
                }
                return;
            }
        }

        internal void ProcessUserMsg(string sender, string recipient, string userName, string storeFolder, Stream msgStream, SMTP_e_MessageStored e)
        {
            string userID = this.m_pApi.GetUserID(userName);
            if (userID == null)
            {
                return;
            }
            msgStream.Position = 0L;
            Mail_Message mime = null;
            try
            {
                mime = Mail_Message.ParseFromStream(msgStream);
            }
            catch
            {
                e.Reply = new SMTP_Reply(552, "Requested mail action aborted: Message has invalid structure/syntax.");
                return;
            }
            string[] array = new string[]
            {
                recipient
            };
            bool flag = false;
            string text = null;
            foreach (DataRowView dataRowView in this.m_pApi.GetUserMessageRules(userName))
            {
                msgStream.Position = 0L;
                if (Convert.ToBoolean(dataRowView["Enabled"]))
                {
                    string ruleID = dataRowView["RuleID"].ToString();
                    GlobalMessageRule_CheckNextRule globalMessageRule_CheckNextRule_enum = (GlobalMessageRule_CheckNextRule)(int)dataRowView["CheckNextRuleIf"];
                    string matchExpression = dataRowView["MatchExpression"].ToString();
                    SMTP_Session smtpSession = null;
                    if (e != null)
                    {
                        smtpSession = e.Session;
                    }
                    GlobalMessageRuleProcessor globalMessageRuleProcessor = new GlobalMessageRuleProcessor();
                    bool flag2 = globalMessageRuleProcessor.Match(matchExpression, sender, array, smtpSession, mime, (int)msgStream.Length);
                    if (flag2)
                    {
                        GlobalMessageRuleActionResult globalMessageRuleActionResult = globalMessageRuleProcessor.DoActions(this.m_pApi.GetUserMessageRuleActions(userID, ruleID), this, msgStream, sender, array);
                        if (globalMessageRuleActionResult.DeleteMessage)
                        {
                            flag = true;
                        }
                        if (globalMessageRuleActionResult.StoreFolder != null)
                        {
                            storeFolder = globalMessageRuleActionResult.StoreFolder;
                        }
                        if (globalMessageRuleActionResult.ErrorText != null)
                        {
                            text = globalMessageRuleActionResult.ErrorText;
                        }
                    }
                    if (((globalMessageRule_CheckNextRule_enum == (GlobalMessageRule_CheckNextRule)1 && !flag2) || (globalMessageRule_CheckNextRule_enum == (GlobalMessageRule_CheckNextRule)2 && flag2)))
                    {
                        break;
                    }
                }
            }
            if (text != null)
            {
                e.Reply = new SMTP_Reply(552, "Requested mail action aborted: " + text);
            }
            else
            {
                if (flag)
                {
                    return;
                }
                msgStream.Position = 0L;
                msgStream.Position = 0L;
                AppendableStream appendableStream = new AppendableStream();
                if (e != null)
                {
                    appendableStream.AppendStream(new MemoryStream(Encoding.UTF8.GetBytes("Return-Path: <" + e.Session.From.Mailbox + ">\r\n")));
                }
                appendableStream.AppendStream(msgStream);
                try
                {
                    this.m_pApi.StoreMessage("system", userName, storeFolder, appendableStream, DateTime.Now, new string[]
                    {
                        "Recent"
                    });
                }
                catch
                {
                    this.m_pApi.StoreMessage("system", userName, "Inbox", appendableStream, DateTime.Now, new string[]
                    {
                        "Recent"
                    });
                }
                return;
            }
        }

        public bool AstericMatch(string pattern, string text)
        {
            pattern = pattern.ToLower();
            text = text.ToLower();
            if (pattern == "")
            {
                pattern = "*";
            }
            while (pattern.Length > 0)
            {
                if (pattern.StartsWith("*"))
                {
                    if (pattern.IndexOf("*", 1) <= -1)
                    {
                        return text.EndsWith(pattern.Substring(1));
                    }
                    string text2 = pattern.Substring(1, pattern.IndexOf("*", 1) - 1);
                    if (text.IndexOf(text2) == -1)
                    {
                        return false;
                    }
                    text = text.Substring(text.IndexOf(text2) + text2.Length + 1);
                    pattern = pattern.Substring(pattern.IndexOf("*", 1) + 1);
                }
                else
                {
                    if (pattern.IndexOfAny(new char[]
                    {
                        '*'
                    }) <= -1)
                    {
                        return text == pattern;
                    }
                    string text3 = pattern.Substring(0, pattern.IndexOfAny(new char[]
                    {
                        '*'
                    }));
                    if (!text.StartsWith(text3))
                    {
                        return false;
                    }
                    text = text.Substring(text.IndexOf(text3) + text3.Length);
                    pattern = pattern.Substring(pattern.IndexOfAny(new char[]
                    {
                        '*'
                    }));
                }
            }
            return true;
        }

        private bool FolderMatches(string folderPattern, string folder)
        {
            folderPattern = folderPattern.ToLower();
            folder = folder.ToLower();
            string[] array = folder.Split(new char[]
            {
                '/'
            });
            string[] array2 = folderPattern.Split(new char[]
            {
                '/'
            });
            if (array.Length < array2.Length)
            {
                return false;
            }
            if (array.Length > array2.Length && !folderPattern.EndsWith("*"))
            {
                return false;
            }
            for (int i = 0; i < array2.Length; i++)
            {
                string text = array2[i].Replace("%", "*");
                if (text.IndexOf('*') > -1)
                {
                    if (!this.AstericMatch(text, array[i]))
                    {
                        return false;
                    }
                }
                else if (array[i] != text)
                {
                    return false;
                }
            }
            return true;
        }

        public static Mail_Message GenerateMessageMissing()
        {
            Mail_Message mail_Message = new Mail_Message();
            mail_Message.MimeVersion = "1.0";
            mail_Message.MessageID = MIME_Utils.CreateMessageID();
            mail_Message.Date = DateTime.Now;
            mail_Message.From = new Mail_t_MailboxList();
            mail_Message.From.Add(new Mail_t_Mailbox("system", "system"));
            mail_Message.To = new Mail_t_AddressList();
            mail_Message.To.Add(new Mail_t_Mailbox("system", "system"));
            mail_Message.Subject = "[MESSAGE MISSING] Message no longer exists on server !";
            MIME_b_MultipartMixed mIME_b_MultipartMixed = new MIME_b_MultipartMixed(new MIME_h_ContentType(MIME_MediaTypes.Multipart.mixed)
            {
                Param_Boundary = Guid.NewGuid().ToString().Replace('-', '.')
            });
            mail_Message.Body = mIME_b_MultipartMixed;
            MIME_Entity mIME_Entity = new MIME_Entity();
            MIME_b_Text mIME_b_Text = new MIME_b_Text(MIME_MediaTypes.Text.plain);
            mIME_Entity.Body = mIME_b_Text;
            mIME_b_Text.SetText(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, "NOTE: Message no longer exists on server.\r\n\r\nMessage is deleted by Administrator or anti-virus software.\r\n");
            mIME_b_MultipartMixed.BodyParts.Add(mIME_Entity);
            return mail_Message;
        }

        private void OnError(Exception x)
        {
            Error.DumpError(this.Name, x);
        }
    }
}
