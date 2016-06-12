using System.NetworkToolkit;
using System.NetworkToolkit.IMAP;
using System.NetworkToolkit.IMAP.Server;
using System.NetworkToolkit.IO;
using System.NetworkToolkit.Mail;
using System.NetworkToolkit.MIME;
using System.NetworkToolkit.POP3.Server;
using System.NetworkToolkit.SIP.Message;
using System.NetworkToolkit.SIP.Proxy;
using System.NetworkToolkit.SMTP.Relay;
using System.NetworkToolkit.SMTP.Server;
using System.NetworkToolkit.TCP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Linq;
namespace DataSmart.MailServer.Monitoring
{
    public class MonitoringServerSession : TCP_ServerSession
    {
        private int m_BadCmdCount;

        private GenericIdentity m_pUser;

        public new MonitoringServer Server
        {
            get
            {
                if (base.IsDisposed)
                {
                    throw new ObjectDisposedException(base.GetType().Name);
                }
                return (MonitoringServer)base.Server;
            }
        }

        public override GenericIdentity AuthenticatedUserIdentity
        {
            get
            {
                if (base.IsDisposed)
                {
                    throw new ObjectDisposedException(base.GetType().Name);
                }
                return this.m_pUser;
            }
        }

        protected override void Start()
        {
            base.Start();
            try
            {
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add("IP_Access");
                dataSet.Tables["IP_Access"].Columns.Add("StartIP");
                dataSet.Tables["IP_Access"].Columns.Add("EndIP");
                if (File.Exists(SCore.PathFix(this.Server.MailServer.StartupPath + "Settings\\AdminAccess.xml")))
                {
                    dataSet.ReadXml(SCore.PathFix(this.Server.MailServer.StartupPath + "Settings\\AdminAccess.xml"));
                }
                else
                {
                    DataRow dataRow = dataSet.Tables["IP_Access"].NewRow();
                    dataRow["StartIP"] = "127.0.0.1";
                    dataRow["EndIP"] = "127.0.0.1";
                    dataSet.Tables["IP_Access"].Rows.Add(dataRow);
                }
                bool flag = false;
                IPAddress address = this.RemoteEndPoint.Address;
                if (IPAddress.Loopback.Equals(address) || IPAddress.IPv6Loopback.Equals(address))
                {
                    flag = true;
                }
                else
                {
                    foreach (DataRow dataRow2 in dataSet.Tables["IP_Access"].Rows)
                    {
                        if (Net_Utils.CompareIP(IPAddress.Parse(dataRow2["StartIP"].ToString()), address) >= 0 && Net_Utils.CompareIP(IPAddress.Parse(dataRow2["EndIP"].ToString()), address) <= 0)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag)
                {
                    base.Disconnect("-ERR Access denied.");
                }
                else
                {
                    this.WriteLine("+OK [" + base.LocalHostName + "] Monitoring Server ready");
                    this.BeginReadCmd();
                }
            }
            catch (Exception x)
            {
                this.OnError(x);
            }
        }

        protected override void OnError(Exception x)
        {
            if (base.IsDisposed)
            {
                return;
            }
            if (x == null)
            {
                return;
            }
            try
            {
                if (x is IOException || x is SocketException)
                {
                    this.Dispose();
                }
                else
                {
                    base.OnError(x);
                    try
                    {
                        this.WriteLine("-ERR Internal server error.");
                    }
                    catch
                    {
                        this.Dispose();
                    }
                }
            }
            catch
            {
            }
        }

        protected override void OnTimeout()
        {
            try
            {
                this.WriteLine("-ERR Idle timeout, closing connection.");
            }
            catch
            {
            }
        }

        private void BeginReadCmd()
        {
            if (base.IsDisposed)
            {
                return;
            }
            try
            {
                SmartStream.ReadLineAsyncOP readLineOP = new SmartStream.ReadLineAsyncOP(new byte[32000], SizeExceededAction.JunkAndThrowException);
                readLineOP.Completed += delegate (object sender, EventArgs<SmartStream.ReadLineAsyncOP> e)
                {
                    if (this.ProcessCmd(readLineOP))
                    {
                        this.BeginReadCmd();
                    }
                };
                while (this.TcpStream.ReadLine(readLineOP, true) && this.ProcessCmd(readLineOP))
                {
                }
            }
            catch (Exception x)
            {
                this.OnError(x);
            }
        }

        private bool ProcessCmd(SmartStream.ReadLineAsyncOP op)
        {
            if (op.Error != null)
            {
                this.OnError(op.Error);
            }
            bool result = true;
            try
            {
                if (base.IsDisposed)
                {
                    bool result2 = false;
                    return result2;
                }
                string[] array = Encoding.UTF8.GetString(op.Buffer, 0, op.LineBytesInBuffer).Split(new char[]
                {
                    ' '
                }, 2);
                string text = array[0].ToUpperInvariant();
                string text2 = (array.Length == 2) ? array[1] : "";
                switch (text)
                {
                    case "NOOP":
                        this.Noop();
                        goto IL_D8C;
                    case "LOGIN":
                        this.Login(text2);
                        goto IL_D8C;
                    case "GETSERVERINFO":
                        this.GetServerInfo();
                        goto IL_D8C;
                    case "GETIPADDRESSES":
                        this.GetIPAddresses();
                        goto IL_D8C;
                    case "KILLSESSION":
                        this.KillSession(text2);
                        goto IL_D8C;
                    case "GETSESSIONS":
                        this.GetSessions();
                        goto IL_D8C;
                    case "GETEVENTS":
                        this.GetEvents();
                        goto IL_D8C;
                    case "GETSIPREGISTRATIONS":
                        this.GetSipRegistrations(text2);
                        goto IL_D8C;
                    case "GETSIPREGISTRATION":
                        this.GetSipRegistration(text2);
                        goto IL_D8C;
                    case "SETSIPREGISTRATION":
                        this.SetSipRegistration(text2);
                        goto IL_D8C;
                    case "DELETESIPREGISTRATION":
                        this.DeleteSipRegistration(text2);
                        goto IL_D8C;
                    case "GETSIPCALLS":
                        this.GetSipCalls(text2);
                        goto IL_D8C;
                    case "TERMINATESIPCALL":
                        this.TerminateSipCall(text2);
                        goto IL_D8C;
                    case "CLEAREVENTS":
                        this.ClearEvents();
                        goto IL_D8C;
                    case "GETLOGSESSIONS":
                        this.GetLogSessions(text2);
                        goto IL_D8C;
                    case "GETSESSIONLOG":
                        this.GetSessionLog(text2);
                        goto IL_D8C;
                    case "GETVIRTUALSERVERS":
                        this.GetVirtualServers();
                        goto IL_D8C;
                    case "GETVIRTUALSERVERAPIS":
                        this.GetVirtualServerAPIs();
                        goto IL_D8C;
                    case "ADDVIRTUALSERVER":
                        this.AddVirtualServer(text2);
                        goto IL_D8C;
                    case "UPDATEVIRTUALSERVER":
                        this.UpdateVirtualServer(text2);
                        goto IL_D8C;
                    case "DELETEVIRTUALSERVER":
                        this.DeleteVirtualServers(text2);
                        goto IL_D8C;
                    case "GETSETTINGS":
                        this.GetSettings(text2);
                        goto IL_D8C;
                    case "UPDATESETTINGS":
                        this.UpdateSettings(text2);
                        goto IL_D8C;
                    case "GETDOMAINS":
                        this.GetDomains(text2);
                        goto IL_D8C;
                    case "ADDDOMAIN":
                        this.AddDomain(text2);
                        goto IL_D8C;
                    case "UPDATEDOMAIN":
                        this.UpdateDomain(text2);
                        goto IL_D8C;
                    case "DELETEDOMAIN":
                        this.DeleteDomain(text2);
                        goto IL_D8C;
                    case "GETUSERS":
                        this.GetUsers(text2);
                        goto IL_D8C;
                    case "ADDUSER":
                        this.AddUser(text2);
                        goto IL_D8C;
                    case "UPDATEUSER":
                        this.UpdateUser(text2);
                        goto IL_D8C;
                    case "DELETEUSER":
                        this.DeleteUser(text2);
                        goto IL_D8C;
                    case "GETUSEREMAILADDRESSES":
                        this.GetUserEmailAddresses(text2);
                        goto IL_D8C;
                    case "ADDUSEREMAILADDRESS":
                        this.AddUserEmailAddress(text2);
                        goto IL_D8C;
                    case "DELETEUSEREMAILADDRESS":
                        this.DeleteUserEmailAddress(text2);
                        goto IL_D8C;
                    case "GETUSERMESSAGERULES":
                        this.GetUserMessageRules(text2);
                        goto IL_D8C;
                    case "ADDUSERMESSAGERULE":
                        this.AddUserMessageRule(text2);
                        goto IL_D8C;
                    case "UPDATEUSERMESSAGERULE":
                        this.UpdateUserMessageRule(text2);
                        goto IL_D8C;
                    case "DELETEUSERMESSAGERULE":
                        this.DeleteUserMessageRule(text2);
                        goto IL_D8C;
                    case "GETUSERMESSAGERULEACTIONS":
                        this.GetUserMessageRuleActions(text2);
                        goto IL_D8C;
                    case "ADDUSERMESSAGERULEACTION":
                        this.AddUserMessageRuleAction(text2);
                        goto IL_D8C;
                    case "UPDATEUSERMESSAGERULEACTION":
                        this.UpdateUserMessageRuleAction(text2);
                        goto IL_D8C;
                    case "DELETEUSERMESSAGERULEACTION":
                        this.DeleteUserMessageRuleAction(text2);
                        goto IL_D8C;
                    case "GETUSERMAILBOXSIZE":
                        this.GetUserMailboxSize(text2);
                        goto IL_D8C;
                    case "GETUSERLASTLOGINTIME":
                        this.GetUserLastLoginTime(text2);
                        goto IL_D8C;
                    case "GETUSERFOLDERS":
                        this.GetUserFolders(text2);
                        goto IL_D8C;
                    case "ADDUSERFOLDER":
                        this.AddUserFolder(text2);
                        goto IL_D8C;
                    case "DELETEUSERFOLDER":
                        this.DeleteUserFolder(text2);
                        goto IL_D8C;
                    case "RENAMEUSERFOLDER":
                        this.RenameUserFolder(text2);
                        goto IL_D8C;
                    case "GETUSERFOLDERINFO":
                        this.GetUserFolderInfo(text2);
                        goto IL_D8C;
                    case "GETUSERFOLDERMESSAGESINFO":
                        this.GetUserFolderMessagesInfo(text2);
                        goto IL_D8C;
                    case "GETUSERFOLDERMESSAGE":
                        this.GetUserFolderMessage(text2);
                        goto IL_D8C;
                    case "STOREUSERFOLDERMESSAGE":
                        this.StoreUserFolderMessage(text2);
                        goto IL_D8C;
                    case "DELETEUSERFOLDERMESSAGE":
                        this.DeleteUserFolderMessage(text2);
                        goto IL_D8C;
                    case "GETUSERREMOTESERVERS":
                        this.GetUserRemoteServers(text2);
                        goto IL_D8C;
                    case "ADDUSERREMOTESERVER":
                        this.AddUserRemoteServer(text2);
                        goto IL_D8C;
                    case "UPDATEUSERREMOTESERVER":
                        this.UpdateUserRemoteServer(text2);
                        goto IL_D8C;
                    case "DELETEUSERREMOTESERVER":
                        this.DeleteUserRemoteServer(text2);
                        goto IL_D8C;
                    case "GETUSERFOLDERACL":
                        this.GetUserFolderAcl(text2);
                        goto IL_D8C;
                    case "SETUSERFOLDERACL":
                        this.SetUserFolderAcl(text2);
                        goto IL_D8C;
                    case "DELETEUSERFOLDERACL":
                        this.DeleteUserFolderAcl(text2);
                        goto IL_D8C;
                    case "GETUSERSDEFAULTFOLDERS":
                        this.GetUsersDefaultFolders(text2);
                        goto IL_D8C;
                    case "ADDUSERSDEFAULTFOLDER":
                        this.AddUsersDefaultFolder(text2);
                        goto IL_D8C;
                    case "DELETEUSERSDEFAULTFOLDER":
                        this.DeleteUsersDefaultFolder(text2);
                        goto IL_D8C;
                    case "GETGROUPS":
                        this.GetGroups(text2);
                        goto IL_D8C;
                    case "GETGROUPMEMBERS":
                        this.GetGroupMembers(text2);
                        goto IL_D8C;
                    case "ADDGROUP":
                        this.AddGroup(text2);
                        goto IL_D8C;
                    case "UPDATEGROUP":
                        this.UpdateGroup(text2);
                        goto IL_D8C;
                    case "DELETEGROUP":
                        this.DeleteGroup(text2);
                        goto IL_D8C;
                    case "ADDGROUPMEMBER":
                        this.AddGroupMember(text2);
                        goto IL_D8C;
                    case "DELETEGROUPMEMBER":
                        this.DeleteGroupMember(text2);
                        goto IL_D8C;
                    case "GETMAILINGLISTS":
                        this.GetMailingLists(text2);
                        goto IL_D8C;
                    case "ADDMAILINGLIST":
                        this.AddMailingList(text2);
                        goto IL_D8C;
                    case "UPDATEMAILINGLIST":
                        this.UpdateMailingList(text2);
                        goto IL_D8C;
                    case "DELETEMAILINGLIST":
                        this.DeleteMailingList(text2);
                        goto IL_D8C;
                    case "GETMAILINGLISTMEMBERS":
                        this.GetMailingListMembers(text2);
                        goto IL_D8C;
                    case "ADDMAILINGLISTMEMBER":
                        this.AddMailingListMember(text2);
                        goto IL_D8C;
                    case "DELETEMAILINGLISTMEMBER":
                        this.DeleteMailingListMember(text2);
                        goto IL_D8C;
                    case "GETMAILINGLISTACL":
                        this.GetMailingListAcl(text2);
                        goto IL_D8C;
                    case "ADDMAILINGLISTACL":
                        this.AddMailingListAcl(text2);
                        goto IL_D8C;
                    case "DELETEMAILINGLISTACL":
                        this.DeleteMailingListAcl(text2);
                        goto IL_D8C;
                    case "GETGLOBALMESSAGERULES":
                        this.GetGlobalMessageRules(text2);
                        goto IL_D8C;
                    case "ADDGLOBALMESSAGERULE":
                        this.AddGlobalMessageRule(text2);
                        goto IL_D8C;
                    case "UPDATEGLOBALMESSAGERULE":
                        this.UpdateGlobalMessageRule(text2);
                        goto IL_D8C;
                    case "DELETEGLOBALMESSAGERULE":
                        this.DeleteGlobalMessageRule(text2);
                        goto IL_D8C;
                    case "GETGLOBALMESSAGERULEACTIONS":
                        this.GetGlobalMessageRuleActions(text2);
                        goto IL_D8C;
                    case "ADDGLOBALMESSAGERULEACTION":
                        this.AddGlobalMessageRuleAction(text2);
                        goto IL_D8C;
                    case "UPDATEGLOBALMESSAGERULEACTION":
                        this.UpdateGlobalMessageRuleAction(text2);
                        goto IL_D8C;
                    case "DELETEGLOBALMESSAGERULEACTION":
                        this.DeleteGlobalMessageRuleAction(text2);
                        goto IL_D8C;
                    case "GETROUTES":
                        this.GetRoutes(text2);
                        goto IL_D8C;
                    case "ADDROUTE":
                        this.AddRoute(text2);
                        goto IL_D8C;
                    case "UPDATEROUTE":
                        this.UpdateRoute(text2);
                        goto IL_D8C;
                    case "DELETEROUTE":
                        this.DeleteRoute(text2);
                        goto IL_D8C;
                    case "GETSHAREDROOTFOLDERS":
                        this.GetSharedRootFolders(text2);
                        goto IL_D8C;
                    case "ADDSHAREDROOTFOLDER":
                        this.AddSharedRootFolder(text2);
                        goto IL_D8C;
                    case "UPDATESHAREDROOTFOLDER":
                        this.UpdateSharedRootFolder(text2);
                        goto IL_D8C;
                    case "DELETESHAREDROOTFOLDER":
                        this.DeleteSharedRootFolder(text2);
                        goto IL_D8C;
                    case "GETFILTERTYPES":
                        this.GetFilterTypes(text2);
                        goto IL_D8C;
                    case "GETFILTERS":
                        this.GetFilters(text2);
                        goto IL_D8C;
                    case "ADDFILTER":
                        this.AddFilter(text2);
                        goto IL_D8C;
                    case "UPDATEFILTER":
                        this.UpdateFilter(text2);
                        goto IL_D8C;
                    case "DELETEFILTER":
                        this.DeleteFilter(text2);
                        goto IL_D8C;
                    case "GETQUEUE":
                        this.GetQueue(text2);
                        goto IL_D8C;
                    case "GETIPSECURITY":
                        this.GetIPSecurity(text2);
                        goto IL_D8C;
                    case "ADDIPSECURITYENTRY":
                        this.AddIPSecurityEntry(text2);
                        goto IL_D8C;
                    case "UPDATEIPSECURITYENTRY":
                        this.UpdateIPSecurityEntry(text2);
                        goto IL_D8C;
                    case "DELETEIPSECURITYENTRY":
                        this.DeleteIPSecurityEntry(text2);
                        goto IL_D8C;
                    case "GETRECYCLEBINSETTINGS":
                        this.GetRecycleBinSettings(text2);
                        goto IL_D8C;
                    case "UPDATERECYCLEBINSETTINGS":
                        this.UpdateRecycleBinSettings(text2);
                        goto IL_D8C;
                    case "GETRECYCLEBINMESSAGESINFO":
                        this.GetRecycleBinMessagesInfo(text2);
                        goto IL_D8C;
                    case "GETRECYCLEBINMESSAGE":
                        this.GetRecycleBinMessage(text2);
                        goto IL_D8C;
                    case "RESTORERECYCLEBINMESSAGE":
                        this.RestoreRecycleBinMessage(text2);
                        goto IL_D8C;
                    case "QUIT":
                        {
                            this.QUIT();
                            result = false;
                            bool result2 = true;
                            return result2;
                        }
                }
                this.WriteLine("-ERR command '" + text + "' unrecognized");
                if (this.m_BadCmdCount > this.Server.MaxBadCommands - 1)
                {
                    this.WriteLine("-ERR Too many bad commands, closing transmission channel");
                    bool result2 = true;
                    return result2;
                }
                this.m_BadCmdCount++;
                IL_D8C:;
            }
            catch (Exception x)
            {
                this.OnError(x);
            }
            return result;
        }

        private void Noop()
        {
            this.WriteLine("+OK");
        }

        private void Login(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: Login \"<UserName>\" \"<password>\"");
                }
                else
                {
                    DataSet dataSet = new DataSet();
                    dataSet.Tables.Add("Users");
                    dataSet.Tables["Users"].Columns.Add("UserName");
                    dataSet.Tables["Users"].Columns.Add("Password");
                    if (File.Exists(SCore.PathFix(this.Server.MailServer.StartupPath + "Settings\\AdminAccess.xml")))
                    {
                        dataSet.ReadXml(SCore.PathFix(this.Server.MailServer.StartupPath + "Settings\\AdminAccess.xml"));
                    }
                    else
                    {
                        DataRow dataRow = dataSet.Tables["Users"].NewRow();
                        dataRow["UserName"] = "Administrator";
                        dataRow["Password"] = "";
                        dataSet.Tables["Users"].Rows.Add(dataRow);
                    }
                    foreach (DataRow dataRow2 in dataSet.Tables["Users"].Rows)
                    {
                        if (dataRow2["UserName"].ToString() == TextUtils.UnQuoteString(array[0]) && dataRow2["Password"].ToString() == TextUtils.UnQuoteString(array[1]))
                        {
                            this.m_pUser = new GenericIdentity(dataRow2["UserName"].ToString());
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR LOGIN failed, invalid user name or password !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetServerInfo()
        {
            try
            {
                DataSet dataSet = new DataSet("dsServerInfo");
                dataSet.Tables.Add("ServerInfo");
                dataSet.Tables["ServerInfo"].Columns.Add("OS");
                dataSet.Tables["ServerInfo"].Columns.Add("MailServerVersion");
                dataSet.Tables["ServerInfo"].Columns.Add("MemoryUsage");
                dataSet.Tables["ServerInfo"].Columns.Add("CpuUsage");
                dataSet.Tables["ServerInfo"].Columns.Add("ServerStartTime", typeof(DateTime));
                dataSet.Tables["ServerInfo"].Columns.Add("Read_KB_Sec");
                dataSet.Tables["ServerInfo"].Columns.Add("Write_KB_Sec");
                dataSet.Tables["ServerInfo"].Columns.Add("SmtpSessions");
                dataSet.Tables["ServerInfo"].Columns.Add("Pop3Sessions");
                dataSet.Tables["ServerInfo"].Columns.Add("ImapSessions");
                dataSet.Tables["ServerInfo"].Columns.Add("RelaySessions");
                TimeSpan totalProcessorTime = Process.GetCurrentProcess().TotalProcessorTime;
                Thread.Sleep(100);
                TimeSpan totalProcessorTime2 = Process.GetCurrentProcess().TotalProcessorTime;
                ArrayList arrayList = new ArrayList(this.Server.MailServer.Sessions);
                int num = 0;
                int num2 = 0;
                int num3 = 0;
                int num4 = 0;
                long num5 = 0L;
                long num6 = 0L;
                foreach (object current in arrayList)
                {
                    try
                    {
                        if (current is SMTP_Session)
                        {
                            num5 += ((SMTP_Session)current).TcpStream.BytesReaded;
                            num6 += ((SMTP_Session)current).TcpStream.BytesWritten;
                            num++;
                        }
                        else if (current is POP3_Session)
                        {
                            num5 += ((POP3_Session)current).TcpStream.BytesReaded;
                            num6 += ((POP3_Session)current).TcpStream.BytesWritten;
                            num2++;
                        }
                        else if (current is IMAP_Session)
                        {
                            num5 += ((IMAP_Session)current).TcpStream.BytesReaded;
                            num6 += ((IMAP_Session)current).TcpStream.BytesWritten;
                            num3++;
                        }
                        else if (current is Relay_Session)
                        {
                            num5 += ((Relay_Session)current).TcpStream.BytesReaded;
                            num6 += ((Relay_Session)current).TcpStream.BytesWritten;
                            num4++;
                        }
                    }
                    catch
                    {
                    }
                }
                Thread.Sleep(200);
                long num7 = 0L;
                long num8 = 0L;
                foreach (object current2 in arrayList)
                {
                    try
                    {
                        if (current2 is SMTP_Session)
                        {
                            num7 += ((SMTP_Session)current2).TcpStream.BytesReaded;
                            num8 += ((SMTP_Session)current2).TcpStream.BytesWritten;
                        }
                        else if (current2 is POP3_Session)
                        {
                            num7 += ((POP3_Session)current2).TcpStream.BytesReaded;
                            num8 += ((POP3_Session)current2).TcpStream.BytesWritten;
                        }
                        else if (current2 is IMAP_Session)
                        {
                            num7 += ((IMAP_Session)current2).TcpStream.BytesReaded;
                            num8 += ((IMAP_Session)current2).TcpStream.BytesWritten;
                        }
                        else if (current2 is Relay_Session)
                        {
                            num7 += ((Relay_Session)current2).TcpStream.BytesReaded;
                            num8 += ((Relay_Session)current2).TcpStream.BytesWritten;
                        }
                    }
                    catch
                    {
                    }
                }
                DataRow dataRow = dataSet.Tables["ServerInfo"].NewRow();
                dataRow["OS"] = Environment.OSVersion.Platform.ToString();
                dataRow["MailServerVersion"] = "0.93";
                dataRow["MemoryUsage"] = Process.GetCurrentProcess().WorkingSet64 / 1000000L;
                dataRow["CpuUsage"] = Math.Min((int)((totalProcessorTime2 - totalProcessorTime).Milliseconds / 100m * 100m), 100);
                dataRow["ServerStartTime"] = this.Server.MailServer.StartTime;
                dataRow["Read_KB_Sec"] = Math.Max((num7 - num5) * 5L, 0L) / 1000L;
                dataRow["Write_KB_Sec"] = Math.Max((num8 - num6) * 5L, 0L) / 1000L;
                dataRow["SmtpSessions"] = num;
                dataRow["Pop3Sessions"] = num2;
                dataRow["ImapSessions"] = num3;
                dataRow["RelaySessions"] = num4;
                dataSet.Tables["ServerInfo"].Rows.Add(dataRow);
                byte[] array = this.CompressDataSet(dataSet);
                this.WriteLine("+OK " + array.Length);
                this.Write(array);
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetIPAddresses()
        {
            try
            {
                DataSet dataSet = new DataSet("dsIPs");
                dataSet.Tables.Add("dsIPs");
                dataSet.Tables["dsIPs"].Columns.Add("IP");
                DataRow dataRow = dataSet.Tables["dsIPs"].NewRow();
                dataRow["IP"] = IPAddress.Any.ToString();
                dataSet.Tables["dsIPs"].Rows.Add(dataRow);
                dataRow = dataSet.Tables["dsIPs"].NewRow();
                dataRow["IP"] = IPAddress.Loopback.ToString();
                dataSet.Tables["dsIPs"].Rows.Add(dataRow);
                dataRow = dataSet.Tables["dsIPs"].NewRow();
                dataRow["IP"] = IPAddress.IPv6Any.ToString();
                dataSet.Tables["dsIPs"].Rows.Add(dataRow);
                dataRow = dataSet.Tables["dsIPs"].NewRow();
                dataRow["IP"] = IPAddress.IPv6Loopback.ToString();
                dataSet.Tables["dsIPs"].Rows.Add(dataRow);
                IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress[] addressList = hostEntry.AddressList;
                for (int i = 0; i < addressList.Length; i++)
                {
                    IPAddress iPAddress = addressList[i];
                    dataRow = dataSet.Tables["dsIPs"].NewRow();
                    dataRow["IP"] = iPAddress.ToString();
                    dataSet.Tables["dsIPs"].Rows.Add(dataRow);
                }
                byte[] array = this.CompressDataSet(dataSet);
                this.WriteLine("+OK " + array.Length);
                this.Write(array);
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void KillSession(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ', true);
                if (array.Length != 1)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: KillSession \"<sessionID>\"");
                }
                else
                {
                    object[] sessions = this.Server.MailServer.Sessions;
                    for (int i = 0; i < sessions.Length; i++)
                    {
                        object obj = sessions[i];
                        if (obj is SMTP_Session)
                        {
                            SMTP_Session sMTP_Session = (SMTP_Session)obj;
                            if (sMTP_Session.ID == array[0])
                            {
                                sMTP_Session.Disconnect();
                                this.WriteLine("+OK");
                                return;
                            }
                        }
                        else if (obj is POP3_Session)
                        {
                            POP3_Session pOP3_Session = (POP3_Session)obj;
                            if (pOP3_Session.ID == array[0])
                            {
                                pOP3_Session.Disconnect();
                                this.WriteLine("+OK");
                                return;
                            }
                        }
                        else if (obj is IMAP_Session)
                        {
                            IMAP_Session iMAP_Session = (IMAP_Session)obj;
                            if (iMAP_Session.ID == array[0])
                            {
                                iMAP_Session.Disconnect();
                                this.WriteLine("+OK");
                                return;
                            }
                        }
                        else if (obj is Relay_Session)
                        {
                            Relay_Session relay_Session = (Relay_Session)obj;
                            if (relay_Session.ID == array[0])
                            {
                                relay_Session.Disconnect("500 Session killed by administrator.");
                                this.WriteLine("+OK");
                                return;
                            }
                        }
                        else if (obj is MonitoringServerSession)
                        {
                            MonitoringServerSession monitoringServerSession = (MonitoringServerSession)obj;
                            if (monitoringServerSession.ID == array[0])
                            {
                                monitoringServerSession.Disconnect();
                                this.WriteLine("+OK");
                                return;
                            }
                        }
                    }
                    this.WriteLine("+OK");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetSessions()
        {
            try
            {
                DataSet dataSet = new DataSet();
                DataTable dataTable = dataSet.Tables.Add("Sessions");
                dataTable.Columns.Add("SessionType");
                dataTable.Columns.Add("SessionID");
                dataTable.Columns.Add("UserName");
                dataTable.Columns.Add("LocalEndPoint");
                dataTable.Columns.Add("RemoteEndPoint");
                dataTable.Columns.Add("SessionStartTime", typeof(DateTime));
                dataTable.Columns.Add("ExpectedTimeout").DefaultValue = 0;
                dataTable.Columns.Add("SessionLog");
                dataTable.Columns.Add("ReadedCount").DefaultValue = 0;
                dataTable.Columns.Add("WrittenCount").DefaultValue = 0;
                dataTable.Columns.Add("ReadTransferRate").DefaultValue = 0;
                dataTable.Columns.Add("WriteTransferRate").DefaultValue = 0;
                dataTable.Columns.Add("IsSecureConnection").DefaultValue = 0;
                ArrayList arrayList = new ArrayList(this.Server.MailServer.Sessions);
                Hashtable hashtable = new Hashtable();
                foreach (object current in arrayList)
                {
                    if (current is SMTP_Session)
                    {
                        hashtable.Add(current, new object[]
                        {
                            ((SMTP_Session)current).TcpStream.BytesReaded,
                            ((SMTP_Session)current).TcpStream.BytesWritten
                        });
                    }
                    else if (current is POP3_Session)
                    {
                        hashtable.Add(current, new object[]
                        {
                            ((POP3_Session)current).TcpStream.BytesReaded,
                            ((POP3_Session)current).TcpStream.BytesWritten
                        });
                    }
                    else if (current is IMAP_Session)
                    {
                        hashtable.Add(current, new object[]
                        {
                            ((IMAP_Session)current).TcpStream.BytesReaded,
                            ((IMAP_Session)current).TcpStream.BytesWritten
                        });
                    }
                    else if (current is Relay_Session)
                    {
                        hashtable.Add(current, new object[]
                        {
                            ((Relay_Session)current).TcpStream.BytesReaded,
                            ((Relay_Session)current).TcpStream.BytesWritten
                        });
                    }
                    else if (current is MonitoringServerSession)
                    {
                        hashtable.Add(current, new object[]
                        {
                            ((MonitoringServerSession)current).TcpStream.BytesReaded,
                            ((MonitoringServerSession)current).TcpStream.BytesWritten
                        });
                    }
                }
                Thread.Sleep(1000);
                foreach (object current2 in arrayList)
                {
                    try
                    {
                        if (current2 is SMTP_Session)
                        {
                            SMTP_Session sMTP_Session = (SMTP_Session)current2;
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["SessionType"] = "SMTP";
                            dataRow["SessionID"] = sMTP_Session.ID;
                            dataRow["UserName"] = ((sMTP_Session.AuthenticatedUserIdentity == null) ? "" : sMTP_Session.AuthenticatedUserIdentity.Name);
                            dataRow["LocalEndPoint"] = this.ObjectToString(sMTP_Session.LocalEndPoint);
                            dataRow["RemoteEndPoint"] = this.ObjectToString(sMTP_Session.RemoteEndPoint);
                            dataRow["SessionStartTime"] = sMTP_Session.ConnectTime;
                            dataRow["ExpectedTimeout"] = (double)sMTP_Session.Server.SessionIdleTimeout - (DateTime.Now - sMTP_Session.TcpStream.LastActivity).TotalSeconds;
                            dataRow["SessionLog"] = "<obsolete: switch to log file in UI>";
                            dataRow["ReadedCount"] = sMTP_Session.TcpStream.BytesReaded;
                            dataRow["WrittenCount"] = sMTP_Session.TcpStream.BytesWritten;
                            dataRow["ReadTransferRate"] = sMTP_Session.TcpStream.BytesReaded - (long)((object[])hashtable[current2])[0];
                            dataRow["WriteTransferRate"] = sMTP_Session.TcpStream.BytesWritten - (long)((object[])hashtable[current2])[1];
                            dataRow["IsSecureConnection"] = sMTP_Session.IsSecureConnection;
                            dataTable.Rows.Add(dataRow);
                        }
                        else if (current2 is POP3_Session)
                        {
                            POP3_Session pOP3_Session = (POP3_Session)current2;
                            DataRow dataRow2 = dataTable.NewRow();
                            dataRow2["SessionType"] = "POP3";
                            dataRow2["SessionID"] = pOP3_Session.ID;
                            dataRow2["UserName"] = ((pOP3_Session.AuthenticatedUserIdentity == null) ? "" : pOP3_Session.AuthenticatedUserIdentity.Name);
                            dataRow2["LocalEndPoint"] = this.ObjectToString(pOP3_Session.LocalEndPoint);
                            dataRow2["RemoteEndPoint"] = this.ObjectToString(pOP3_Session.RemoteEndPoint);
                            dataRow2["SessionStartTime"] = pOP3_Session.ConnectTime;
                            dataRow2["ExpectedTimeout"] = Convert.ToInt32((double)pOP3_Session.Server.SessionIdleTimeout - (DateTime.Now - pOP3_Session.TcpStream.LastActivity).TotalSeconds);
                            dataRow2["SessionLog"] = "<obsolete: switch to log file in UI>";
                            dataRow2["ReadedCount"] = pOP3_Session.TcpStream.BytesReaded;
                            dataRow2["WrittenCount"] = pOP3_Session.TcpStream.BytesWritten;
                            dataRow2["ReadTransferRate"] = pOP3_Session.TcpStream.BytesReaded - (long)((object[])hashtable[current2])[0];
                            dataRow2["WriteTransferRate"] = pOP3_Session.TcpStream.BytesWritten - (long)((object[])hashtable[current2])[1];
                            dataRow2["IsSecureConnection"] = pOP3_Session.IsSecureConnection;
                            dataTable.Rows.Add(dataRow2);
                        }
                        else if (current2 is IMAP_Session)
                        {
                            IMAP_Session iMAP_Session = (IMAP_Session)current2;
                            DataRow dataRow3 = dataTable.NewRow();
                            dataRow3["SessionType"] = "IMAP";
                            dataRow3["SessionID"] = iMAP_Session.ID;
                            dataRow3["UserName"] = ((iMAP_Session.AuthenticatedUserIdentity == null) ? "" : iMAP_Session.AuthenticatedUserIdentity.Name);
                            dataRow3["LocalEndPoint"] = this.ObjectToString(iMAP_Session.LocalEndPoint);
                            dataRow3["RemoteEndPoint"] = this.ObjectToString(iMAP_Session.RemoteEndPoint);
                            dataRow3["SessionStartTime"] = iMAP_Session.ConnectTime;
                            dataRow3["ExpectedTimeout"] = Convert.ToInt32((double)iMAP_Session.Server.SessionIdleTimeout - (DateTime.Now - iMAP_Session.TcpStream.LastActivity).TotalSeconds);
                            dataRow3["ReadedCount"] = iMAP_Session.TcpStream.BytesReaded;
                            dataRow3["WrittenCount"] = iMAP_Session.TcpStream.BytesWritten;
                            dataRow3["ReadTransferRate"] = iMAP_Session.TcpStream.BytesReaded - (long)((object[])hashtable[current2])[0];
                            dataRow3["WriteTransferRate"] = iMAP_Session.TcpStream.BytesWritten - (long)((object[])hashtable[current2])[1];
                            dataRow3["IsSecureConnection"] = iMAP_Session.IsSecureConnection;
                            dataTable.Rows.Add(dataRow3);
                        }
                        else if (current2 is Relay_Session)
                        {
                            Relay_Session relay_Session = (Relay_Session)current2;
                            DataRow dataRow4 = dataTable.NewRow();
                            dataRow4["SessionType"] = "RELAY";
                            dataRow4["SessionID"] = relay_Session.ID;
                            dataRow4["UserName"] = "";
                            dataRow4["LocalEndPoint"] = this.ObjectToString(relay_Session.LocalEndPoint);
                            dataRow4["RemoteEndPoint"] = this.ObjectToString(relay_Session.RemoteEndPoint);
                            dataRow4["SessionStartTime"] = relay_Session.SessionCreateTime;
                            dataRow4["ExpectedTimeout"] = relay_Session.ExpectedTimeout;
                            dataRow4["SessionLog"] = "Not supported";
                            dataRow4["ReadedCount"] = relay_Session.TcpStream.BytesReaded;
                            dataRow4["WrittenCount"] = relay_Session.TcpStream.BytesWritten;
                            dataRow4["ReadTransferRate"] = relay_Session.TcpStream.BytesReaded - (long)((object[])hashtable[current2])[0];
                            dataRow4["WriteTransferRate"] = relay_Session.TcpStream.BytesWritten - (long)((object[])hashtable[current2])[1];
                            dataRow4["IsSecureConnection"] = relay_Session.IsSecureConnection;
                            dataTable.Rows.Add(dataRow4);
                        }
                        else if (current2 is MonitoringServerSession)
                        {
                            MonitoringServerSession monitoringServerSession = (MonitoringServerSession)current2;
                            DataRow dataRow5 = dataTable.NewRow();
                            dataRow5["SessionType"] = "ADMIN";
                            dataRow5["SessionID"] = monitoringServerSession.ID;
                            dataRow5["UserName"] = ((monitoringServerSession.AuthenticatedUserIdentity == null) ? "" : monitoringServerSession.AuthenticatedUserIdentity.Name);
                            dataRow5["LocalEndPoint"] = this.ObjectToString(monitoringServerSession.LocalEndPoint);
                            dataRow5["RemoteEndPoint"] = this.ObjectToString(monitoringServerSession.RemoteEndPoint);
                            dataRow5["SessionStartTime"] = monitoringServerSession.ConnectTime;
                            dataRow5["ExpectedTimeout"] = Convert.ToInt32((double)monitoringServerSession.Server.SessionIdleTimeout - (DateTime.Now - monitoringServerSession.TcpStream.LastActivity).TotalSeconds);
                            dataRow5["SessionLog"] = "Not supported";
                            dataRow5["ReadedCount"] = monitoringServerSession.TcpStream.BytesReaded;
                            dataRow5["WrittenCount"] = monitoringServerSession.TcpStream.BytesWritten;
                            dataRow5["ReadTransferRate"] = monitoringServerSession.TcpStream.BytesReaded - (long)((object[])hashtable[current2])[0];
                            dataRow5["WriteTransferRate"] = monitoringServerSession.TcpStream.BytesWritten - (long)((object[])hashtable[current2])[1];
                            dataRow5["IsSecureConnection"] = monitoringServerSession.IsSecureConnection;
                            dataTable.Rows.Add(dataRow5);
                        }
                    }
                    catch
                    {
                    }
                }
                byte[] array = this.CompressDataSet(dataSet);
                this.WriteLine("+OK " + array.Length);
                this.Write(array);
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetSipRegistrations(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ', true);
                if (array.Length != 1)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetSipRegistrations \"<virtualServerID>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            DataSet dataSet = new DataSet("dsSipRegistrations");
                            dataSet.Tables.Add("SipRegistrations");
                            dataSet.Tables["SipRegistrations"].Columns.Add("UserName");
                            dataSet.Tables["SipRegistrations"].Columns.Add("AddressOfRecord");
                            dataSet.Tables["SipRegistrations"].Columns.Add("Contacts");
                            SIP_Registration[] registrations = virtualServer.SipServer.Registrar.Registrations;
                            for (int j = 0; j < registrations.Length; j++)
                            {
                                SIP_Registration sIP_Registration = registrations[j];
                                DataRow dataRow = dataSet.Tables["SipRegistrations"].NewRow();
                                dataRow["UserName"] = sIP_Registration.UserName;
                                dataRow["AddressOfRecord"] = sIP_Registration.AOR;
                                string text = "";
                                SIP_RegistrationBinding[] bindings = sIP_Registration.Bindings;
                                for (int k = 0; k < bindings.Length; k++)
                                {
                                    SIP_RegistrationBinding sIP_RegistrationBinding = bindings[k];
                                    text = text + sIP_RegistrationBinding.ToContactValue() + "\t";
                                }
                                dataRow["Contacts"] = text;
                                dataSet.Tables["SipRegistrations"].Rows.Add(dataRow);
                            }
                            byte[] array2 = this.CompressDataSet(dataSet);
                            this.WriteLine("+OK " + array2.Length);
                            this.Write(array2);
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetSipRegistration(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ', true);
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetSipRegistration \"<virtualServerID>\" \"<addressOfRecord>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            DataSet dataSet = new DataSet("dsSipRegistration");
                            dataSet.Tables.Add("Contacts");
                            dataSet.Tables["Contacts"].Columns.Add("Value");
                            SIP_Registration[] registrations = virtualServer.SipServer.Registrar.Registrations;
                            for (int j = 0; j < registrations.Length; j++)
                            {
                                SIP_Registration sIP_Registration = registrations[j];
                                if (array[1].ToLower() == sIP_Registration.AOR.ToLower())
                                {
                                    SIP_RegistrationBinding[] bindings = sIP_Registration.Bindings;
                                    for (int k = 0; k < bindings.Length; k++)
                                    {
                                        SIP_RegistrationBinding sIP_RegistrationBinding = bindings[k];
                                        DataRow dataRow = dataSet.Tables["Contacts"].NewRow();
                                        dataRow["Value"] = sIP_RegistrationBinding.ToContactValue();
                                        dataSet.Tables["Contacts"].Rows.Add(dataRow);
                                    }
                                    break;
                                }
                            }
                            byte[] array2 = this.CompressDataSet(dataSet);
                            this.WriteLine("+OK " + array2.Length);
                            this.Write(array2);
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void SetSipRegistration(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ', true);
                if (array.Length != 3)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: SetSipRegistration \"<virtualServerID>\" \"<addressOfRecord>\" \"<contacts>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            List<SIP_t_ContactParam> list = new List<SIP_t_ContactParam>();
                            string[] array2 = array[2].Split(new char[]
                            {
                                '\t'
                            });
                            for (int j = 0; j < array2.Length; j++)
                            {
                                string text = array2[j];
                                if (text.Length > 0)
                                {
                                    SIP_t_ContactParam sIP_t_ContactParam = new SIP_t_ContactParam();
                                    sIP_t_ContactParam.Parse(new System.NetworkToolkit.StringReader(text));
                                    list.Add(sIP_t_ContactParam);
                                }
                            }
                            virtualServer.SipServer.Registrar.SetRegistration(array[1], list.ToArray());
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void DeleteSipRegistration(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ', true);
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeleteSipRegistration \"<virtualServerID>\" \"<addressOfRecord>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            SIP_Registration[] registrations = virtualServer.SipServer.Registrar.Registrations;
                            for (int j = 0; j < registrations.Length; j++)
                            {
                                SIP_Registration sIP_Registration = registrations[j];
                                if (array[0].ToLower() == sIP_Registration.AOR.ToLower())
                                {
                                    virtualServer.SipServer.Registrar.DeleteRegistration(sIP_Registration.AOR);
                                    break;
                                }
                            }
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetSipCalls(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ', true);
                if (array.Length != 1)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetSipCalls \"<virtualServerID>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            DataSet dataSet = new DataSet("dsSipCalls");
                            dataSet.Tables.Add("SipCalls");
                            dataSet.Tables["SipCalls"].Columns.Add("CallID");
                            dataSet.Tables["SipCalls"].Columns.Add("Caller");
                            dataSet.Tables["SipCalls"].Columns.Add("Callee");
                            dataSet.Tables["SipCalls"].Columns.Add("StartTime");
                            SIP_B2BUA_Call[] calls = virtualServer.SipServer.B2BUA.Calls;
                            for (int j = 0; j < calls.Length; j++)
                            {
                                SIP_B2BUA_Call sIP_B2BUA_Call = calls[j];
                                DataRow dataRow = dataSet.Tables["SipCalls"].NewRow();
                                dataRow["CallID"] = sIP_B2BUA_Call.CallID;
                                dataRow["Caller"] = sIP_B2BUA_Call.CallerDialog.RemoteUri;
                                dataRow["Callee"] = sIP_B2BUA_Call.CallerDialog.LocalUri;
                                dataRow["StartTime"] = sIP_B2BUA_Call.StartTime;
                                dataSet.Tables["SipCalls"].Rows.Add(dataRow);
                            }
                            byte[] array2 = this.CompressDataSet(dataSet);
                            this.WriteLine("+OK " + array2.Length);
                            this.Write(array2);
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void TerminateSipCall(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ', true);
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: TerminateSipCall \"<virtualServerID>\" \"<callID>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.SipServer.B2BUA.GetCallByID(array[1]).Terminate();
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetEvents()
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
                if (File.Exists(SCore.PathFix(this.Server.MailServer.StartupPath + "\\Settings\\Events.xml")))
                {
                    dataSet.ReadXml(SCore.PathFix(this.Server.MailServer.StartupPath + "\\Settings\\Events.xml"));
                }
                byte[] array = this.CompressDataSet(dataSet);
                this.WriteLine("+OK " + array.Length);
                this.Write(array);
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void ClearEvents()
        {
            try
            {
                if (File.Exists(SCore.PathFix(this.Server.MailServer.StartupPath + "\\Settings\\Events.xml")))
                {
                    File.Delete(SCore.PathFix(this.Server.MailServer.StartupPath + "\\Settings\\Events.xml"));
                }
                this.WriteLine("+OK");
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetLogSessions(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 6)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetLogSessions <virtualServerID> <service> <limit> \"<startTime>\" \"<endTime>\" \"containsText\"");
                }
                else
                {
                    VirtualServer virtualServer = null;
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer2 = virtualServers[i];
                        if (virtualServer2.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer = virtualServer2;
                            break;
                        }
                    }
                    if (virtualServer == null)
                    {
                        this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                    }
                    DataSet dataSet = new DataSet("dsLogsSessions");
                    dataSet.Tables.Add("LogSessions");
                    dataSet.Tables["LogSessions"].Columns.Add("SessionID");
                    dataSet.Tables["LogSessions"].Columns.Add("StartTime", typeof(DateTime));
                    dataSet.Tables["LogSessions"].Columns.Add("RemoteEndPoint");
                    dataSet.Tables["LogSessions"].Columns.Add("UserName");
                    int num = ConvertEx.ToInt32(array[2]);
                    DateTime t = TimeHelper.Parse(TextUtils.UnQuoteString(array[3]));
                    DateTime t2 = TimeHelper.Parse(TextUtils.UnQuoteString(array[4]));
                    string text = TextUtils.UnQuoteString(array[5]);
                    string text2;
                    if (array[1] == "SMTP")
                    {
                        text2 = virtualServer.SMTP_LogsPath + "smtp";
                    }
                    else if (array[1] == "POP3")
                    {
                        text2 = virtualServer.POP3_LogsPath + "pop3";
                    }
                    else if (array[1] == "IMAP")
                    {
                        text2 = virtualServer.IMAP_LogsPath + "imap";
                    }
                    else if (array[1] == "RELAY")
                    {
                        text2 = virtualServer.RELAY_LogsPath + "relay";
                    }
                    else
                    {
                        if (!(array[1] == "FETCH"))
                        {
                            throw new Exception("Invalid <service value> !");
                        }
                        text2 = virtualServer.FETCH_LogsPath + "fetch";
                    }
                    text2 = text2 + "-" + t.ToString("yyyyMMdd") + ".log";
                    if (File.Exists(text2))
                    {
                        using (TextDb textDb = new TextDb('\t'))
                        {
                            textDb.OpenRead(text2);
                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                            while (textDb.MoveNext())
                            {
                                string[] currentRow = textDb.CurrentRow;
                                if (currentRow.Length == 6)
                                {
                                    string text3 = currentRow[0];
                                    DateTime dateTime = DateTime.ParseExact(TextUtils.UnQuoteString(currentRow[1]), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                                    string value = currentRow[2];
                                    string value2 = currentRow[3];
                                    string text4 = currentRow[5];
                                    if (!dictionary.ContainsKey(text3))
                                    {
                                        bool flag = true;
                                        if (t > dateTime || t2 < dateTime)
                                        {
                                            flag = false;
                                        }
                                        if (text.Length > 0 && text4.ToLower().IndexOf(text.ToLower()) == -1)
                                        {
                                            flag = false;
                                        }
                                        if (dictionary.Count > num)
                                        {
                                            break;
                                        }
                                        if (flag)
                                        {
                                            DataRow dataRow = dataSet.Tables["LogSessions"].NewRow();
                                            dataRow["SessionID"] = text3;
                                            dataRow["StartTime"] = dateTime;
                                            dataRow["RemoteEndPoint"] = value;
                                            dataRow["UserName"] = value2;
                                            dataSet.Tables["LogSessions"].Rows.Add(dataRow);
                                            dictionary.Add(text3, "");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    byte[] array2 = this.CompressDataSet(dataSet);
                    this.WriteLine("+OK " + array2.Length);
                    this.Write(array2);
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetSessionLog(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ', true);
                if (array.Length != 4)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetSessionLog <virtualServerID> <service> \"<sessionID>\" \"<sessionStartDate>\"");
                }
                else
                {
                    VirtualServer virtualServer = null;
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer2 = virtualServers[i];
                        if (virtualServer2.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer = virtualServer2;
                            break;
                        }
                    }
                    if (virtualServer == null)
                    {
                        this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                    }
                    DateTime dateTime = TimeHelper.Parse(TextUtils.UnQuoteString(array[3]));
                    DataSet dataSet = new DataSet("dsSessionLog");
                    dataSet.Tables.Add("SessionLog");
                    dataSet.Tables["SessionLog"].Columns.Add("LogText");
                    string text = "";
                    if (array[1] == "SMTP")
                    {
                        text = text + virtualServer.SMTP_LogsPath + "smtp";
                    }
                    else if (array[1] == "POP3")
                    {
                        text = text + virtualServer.POP3_LogsPath + "pop3";
                    }
                    else if (array[1] == "IMAP")
                    {
                        text = text + virtualServer.IMAP_LogsPath + "imap";
                    }
                    else if (array[1] == "RELAY")
                    {
                        text = text + virtualServer.RELAY_LogsPath + "relay";
                    }
                    else
                    {
                        if (!(array[1] == "FETCH"))
                        {
                            throw new Exception("Invalid <service value> !");
                        }
                        text = text + virtualServer.FETCH_LogsPath + "fetch";
                    }
                    text = text + "-" + dateTime.ToString("yyyyMMdd") + ".log";
                    if (File.Exists(text))
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        using (TextDb textDb = new TextDb('\t'))
                        {
                            textDb.OpenRead(text);
                            while (textDb.MoveNext())
                            {
                                string[] currentRow = textDb.CurrentRow;
                                if (currentRow.Length == 6)
                                {
                                    string text2 = currentRow[0];
                                    string arg_209_0 = currentRow[5];
                                    if (text2.ToLower() == array[2].ToLower())
                                    {
                                        stringBuilder.Append(textDb.CurrentRowString + "\r\n");
                                    }
                                }
                            }
                        }
                        DataRow dataRow = dataSet.Tables["SessionLog"].NewRow();
                        dataRow["LogText"] = stringBuilder.ToString();
                        dataSet.Tables["SessionLog"].Rows.Add(dataRow);
                    }
                    byte[] array2 = this.CompressDataSet(dataSet);
                    this.WriteLine("+OK " + array2.Length);
                    this.Write(array2);
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetVirtualServerAPIs()
        {
            try
            {
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add("API");
                dataSet.Tables["API"].Columns.Add("AssemblyName");
                dataSet.Tables["API"].Columns.Add("TypeName");
                string[] files = Directory.GetFiles(SCore.PathFix(this.Server.MailServer.StartupPath), "*.dll");
                string[] array = files;
                var apiType = typeof(IMailServerManagementApi);
                for (int i = 0; i < array.Length; i++)
                {
                    string path = array[i];
                    try
                    {
                        Assembly assembly = Assembly.LoadFile(path);
                        var types = assembly.ExportedTypes.Where(x => !x.IsAbstract && apiType.IsAssignableFrom(x));
                        foreach (var type in types)
                        {
                            DataRow dataRow = dataSet.Tables["API"].NewRow();
                            dataRow["AssemblyName"] = Path.GetFileName(path);
                            dataRow["TypeName"] = type.ToString();
                            dataSet.Tables["API"].Rows.Add(dataRow);
                        }
                    }
                    catch
                    {
                    }
                }
                byte[] array3 = this.CompressDataSet(dataSet);
                this.WriteLine("+OK " + array3.Length);
                this.Write(array3);
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetVirtualServers()
        {
            try
            {
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add("Servers");
                dataSet.Tables["Servers"].Columns.Add("ID");
                dataSet.Tables["Servers"].Columns.Add("Enabled").DefaultValue = true;
                dataSet.Tables["Servers"].Columns.Add("Name");
                dataSet.Tables["Servers"].Columns.Add("API_assembly");
                dataSet.Tables["Servers"].Columns.Add("API_class");
                dataSet.Tables["Servers"].Columns.Add("API_initstring");
                this.Server.MailServer.ReadXmlSetting(dataSet, "LocalServers", "Servers");
                byte[] array = this.CompressDataSet(dataSet);
                this.WriteLine("+OK " + array.Length);
                this.Write(array);
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void AddVirtualServer(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 6)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: AddVirtualServer <ID> <enabled> \"<name>\" \"<assembly>\" \"<type>\" \"<initString>:base64\"");
                }
                else
                {
                    this.Server.MailServer.LoadApi(TextUtils.UnQuoteString(array[3]), TextUtils.UnQuoteString(array[4]), Encoding.UTF8.GetString(Convert.FromBase64String(TextUtils.UnQuoteString(array[5]))));
                    DataSet dataSet = new DataSet();
                    dataSet.Tables.Add("Servers");
                    dataSet.Tables["Servers"].Columns.Add("ID");
                    dataSet.Tables["Servers"].Columns.Add("Enabled");
                    dataSet.Tables["Servers"].Columns.Add("Name");
                    dataSet.Tables["Servers"].Columns.Add("API_assembly");
                    dataSet.Tables["Servers"].Columns.Add("API_class");
                    dataSet.Tables["Servers"].Columns.Add("API_initstring");
                    this.Server.MailServer.ReadXmlSetting(dataSet, "localServers", "Servers");
                    DataRow dataRow = dataSet.Tables["Servers"].NewRow();
                    dataRow["ID"] = array[0];
                    dataRow["Enabled"] = array[1];
                    dataRow["Name"] = TextUtils.UnQuoteString(array[2]);
                    dataRow["API_assembly"] = TextUtils.UnQuoteString(array[3]);
                    dataRow["API_class"] = TextUtils.UnQuoteString(array[4]);
                    dataRow["API_initstring"] = Encoding.UTF8.GetString(Convert.FromBase64String(TextUtils.UnQuoteString(array[5])));
                    dataSet.Tables["Servers"].Rows.Add(dataRow);
                    this.Server.MailServer.WriteXmlSetting(dataSet, "localServers");
                    this.Server.MailServer.LoadVirtualServers();
                    this.WriteLine("+OK");
                }
            }
            catch (TargetInvocationException ex)
            {
                this.WriteLine("-ERR " + ex.InnerException.Message);
            }
            catch (Exception ex2)
            {
                this.WriteLine("-ERR " + ex2.Message + "\r\n" + ex2.StackTrace);
            }
        }

        private void UpdateVirtualServer(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 4)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: UpdateVirtualServer <ID> <enabled> \"<name>\" \"<initString>:base64\"");
                }
                else
                {
                    DataSet dataSet = new DataSet();
                    dataSet.Tables.Add("Servers");
                    dataSet.Tables["Servers"].Columns.Add("ID");
                    dataSet.Tables["Servers"].Columns.Add("Enabled");
                    dataSet.Tables["Servers"].Columns.Add("Name");
                    dataSet.Tables["Servers"].Columns.Add("API_assembly");
                    dataSet.Tables["Servers"].Columns.Add("API_class");
                    dataSet.Tables["Servers"].Columns.Add("API_initstring");
                    dataSet.ReadXml(SCore.PathFix(this.Server.MailServer.StartupPath + "Settings\\localServers.xml"));
                    if (dataSet.Tables.Contains("Servers"))
                    {
                        foreach (DataRow dataRow in dataSet.Tables["Servers"].Rows)
                        {
                            if (dataRow["ID"].ToString() == TextUtils.UnQuoteString(array[0]))
                            {
                                dataRow["Enabled"] = array[1];
                                dataRow["Name"] = TextUtils.UnQuoteString(array[2]);
                                dataRow["API_initstring"] = Encoding.UTF8.GetString(Convert.FromBase64String(TextUtils.UnQuoteString(array[3])));
                                dataSet.WriteXml(SCore.PathFix(this.Server.MailServer.StartupPath + "Settings\\localServers.xml"));
                                this.WriteLine("+OK");
                                return;
                            }
                        }
                        this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                    }
                }
            }
            catch (TargetInvocationException ex)
            {
                this.WriteLine("-ERR " + ex.InnerException.Message);
            }
            catch (Exception ex2)
            {
                this.WriteLine("-ERR " + ex2.Message + "\r\n" + ex2.StackTrace);
            }
        }

        private void DeleteVirtualServers(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 1)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeleteVirtualServer <virtualServerID>");
                }
                else
                {
                    DataSet dataSet = new DataSet();
                    dataSet.ReadXml(SCore.PathFix(this.Server.MailServer.StartupPath + "Settings\\localServers.xml"));
                    if (dataSet.Tables.Contains("Servers"))
                    {
                        foreach (DataRow dataRow in dataSet.Tables["Servers"].Rows)
                        {
                            if (dataRow["ID"].ToString() == TextUtils.UnQuoteString(array[0]))
                            {
                                dataRow.Delete();
                                dataSet.WriteXml(SCore.PathFix(this.Server.MailServer.StartupPath + "Settings\\localServers.xml"));
                                this.Server.MailServer.LoadVirtualServers();
                                this.WriteLine("+OK");
                                return;
                            }
                        }
                        this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                    }
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetSettings(string argsText)
        {
            try
            {
                VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                for (int i = 0; i < virtualServers.Length; i++)
                {
                    VirtualServer virtualServer = virtualServers[i];
                    if (virtualServer.ID.ToLower() == argsText.ToLower())
                    {
                        MemoryStream memoryStream = new MemoryStream();
                        virtualServer.InternalService.GetSettings().Table.DataSet.WriteXml(memoryStream);
                        memoryStream.Position = 0L;
                        this.WriteLine("+OK " + memoryStream.Length);
                        this.Write(memoryStream);
                        return;
                    }
                }
                this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void UpdateSettings(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: UpdateSettings <virtualServerID> <dataLength><CRLF><data>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            MemoryStream memoryStream = new MemoryStream();
                            this.TcpStream.ReadFixedCount(memoryStream, (long)Convert.ToInt32(array[1]));
                            memoryStream.Position = 0L;
                            DataSet dataSet = new DataSet();
                            PathHelper.CreateSettingsSchema(dataSet);
                            dataSet.ReadXml(memoryStream);
                            virtualServer.InternalService.UpdateSettings(dataSet.Tables["Settings"].Rows[0]);
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetDomains(string argsText)
        {
            try
            {
                VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                for (int i = 0; i < virtualServers.Length; i++)
                {
                    VirtualServer virtualServer = virtualServers[i];
                    if (virtualServer.ID.ToLower() == argsText.ToLower())
                    {
                        new MemoryStream();
                        DataSet dataSet = virtualServer.InternalService.GetDomains().Table.DataSet;
                        byte[] array = this.CompressDataSet(dataSet);
                        this.WriteLine("+OK " + array.Length);
                        this.Write(array);
                        return;
                    }
                }
                this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void AddDomain(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 4)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: AddDomain <virtualServerID> \"<domainID>\" \"<domainName>\" \"<description>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.AddDomain(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]), TextUtils.UnQuoteString(array[3]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void UpdateDomain(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 4)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: UpdateDomain <virtualServerID> \"<domainID>\" \"<domainName>\" \"<description>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.UpdateDomain(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]), TextUtils.UnQuoteString(array[3]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void DeleteDomain(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeleteDomain <virtualServerID> \"<domainID>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.DeleteDomain(TextUtils.UnQuoteString(array[1]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetUsers(string argsText)
        {
            try
            {
                VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                for (int i = 0; i < virtualServers.Length; i++)
                {
                    VirtualServer virtualServer = virtualServers[i];
                    if (virtualServer.ID.ToLower() == argsText.ToLower())
                    {
                        DataSet dataSet = virtualServer.InternalService.GetUsers("ALL").Table.DataSet;
                        byte[] array = this.CompressDataSet(dataSet);
                        this.WriteLine("+OK " + array.Length);
                        this.Write(array);
                        return;
                    }
                }
                this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void AddUser(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 9)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: AddUser <virtualServerID> \"<userID>\" \"<userName>\" \"<fullName>\" \"<password>\" \"<description>\" <mailboxSize> <enabled> <allowRelay>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.AddUser(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]), TextUtils.UnQuoteString(array[3]), TextUtils.UnQuoteString(array[4]), TextUtils.UnQuoteString(array[5]), "", Convert.ToInt32(array[6]), Convert.ToBoolean(array[7]), (UserPermissions)Convert.ToInt32(array[8]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void UpdateUser(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 9)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: UpdateUser <virtualServerID> \"<userID>\" \"<userName>\" \"<fullName>\" \"<password>\" \"<description>\" <mailboxSize> <enabled> <allowRelay>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.UpdateUser(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]), TextUtils.UnQuoteString(array[3]), TextUtils.UnQuoteString(array[4]), TextUtils.UnQuoteString(array[5]), "", Convert.ToInt32(array[6]), Convert.ToBoolean(array[7]), (UserPermissions)Convert.ToInt32(array[8]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void DeleteUser(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeleteUser <virtualServerID> \"<userID>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.DeleteUser(TextUtils.UnQuoteString(array[1]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetUserEmailAddresses(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetUserEmailAddresses <virtualServerID> <userID>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            DataSet ds = this.DataView_To_DataSet(virtualServer.InternalService.GetUserAddresses(this.UserName_From_UserID(virtualServer.InternalService.GetUsers("ALL"), array[1])));
                            byte[] array2 = this.CompressDataSet(ds);
                            this.WriteLine("+OK " + array2.Length);
                            this.Write(array2);
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void AddUserEmailAddress(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 3)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: AddUserEmailAddress <virtualServerID> \"<userID>\" \"<emailAddress>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            new MemoryStream();
                            virtualServer.InternalService.AddUserAddress(this.UserName_From_UserID(virtualServer.InternalService.GetUsers("ALL"), TextUtils.UnQuoteString(array[1])), TextUtils.UnQuoteString(array[2]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void DeleteUserEmailAddress(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 3)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeleteUserEmailAddress <virtualServerID> \"<userID>\" \"<emailAddress>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            new MemoryStream();
                            virtualServer.InternalService.DeleteUserAddress(TextUtils.UnQuoteString(array[2]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetUserMessageRules(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetUserMessageRules <virtualServerID> \"<userID>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            DataSet ds = this.DataView_To_DataSet(virtualServer.InternalService.GetUserMessageRules(this.UserName_From_UserID(virtualServer.InternalService.GetUsers("ALL"), TextUtils.UnQuoteString(array[1]))));
                            byte[] array2 = this.CompressDataSet(ds);
                            this.WriteLine("+OK " + array2.Length);
                            this.Write(array2);
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void AddUserMessageRule(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 8)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: AddGlobalMessageRule <virtualServerID> \"<userID>\" \"<ruleID>\" <cost> <enabled> \"<description>\" \"<matchExpression>\" <checkNext>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.AddUserMessageRule(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]), Convert.ToInt64(array[3]), Convert.ToBoolean(array[4]), (GlobalMessageRule_CheckNextRule)Convert.ToInt32(array[7]), TextUtils.UnQuoteString(array[5]), TextUtils.UnQuoteString(array[6]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void UpdateUserMessageRule(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 8)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: UpdateUserMessageRule <virtualServerID> \"<userID>\" \"<ruleID>\" <cost> <enabled> \"<description>\" \"<matchExpression>\" <checkNext>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.UpdateUserMessageRule(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]), Convert.ToInt64(array[3]), Convert.ToBoolean(array[4]), (GlobalMessageRule_CheckNextRule)Convert.ToInt32(array[7]), TextUtils.UnQuoteString(array[5]), TextUtils.UnQuoteString(array[6]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void DeleteUserMessageRule(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 3)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeleteUserMessageRule <virtualServerID> \"<userID>\" \"<ruleID>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.DeleteUserMessageRule(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetUserMessageRuleActions(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 3)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetUserMessageRuleActions <virtualServerID> \"<userID>\" \"<messageRuleID>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            DataSet ds = this.DataView_To_DataSet(virtualServer.InternalService.GetUserMessageRuleActions(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2])));
                            byte[] array2 = this.CompressDataSet(ds);
                            this.WriteLine("+OK " + array2.Length);
                            this.Write(array2);
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void AddUserMessageRuleAction(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 7)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: AddUserMessageRuleAction <virtualServerID> \"<userID>\" \"<messageRuleID>\" \"<messageRuleActionID>\" \"<description>\" <actionType> \"<actionData>:base64\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.AddUserMessageRuleAction(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]), TextUtils.UnQuoteString(array[3]), TextUtils.UnQuoteString(array[4]), (GlobalMessageRuleActionType)Convert.ToInt32(array[5]), Convert.FromBase64String(TextUtils.UnQuoteString(array[6])));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void UpdateUserMessageRuleAction(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 7)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: UpdateUserMessageRuleAction <virtualServerID> \"<messageRuleID>\" \"<messageRuleActionID>\" \"<description>\" <actionType> \"<actionData>:base64\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.UpdateUserMessageRuleAction(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]), TextUtils.UnQuoteString(array[3]), TextUtils.UnQuoteString(array[4]), (GlobalMessageRuleActionType)Convert.ToInt32(array[5]), Convert.FromBase64String(TextUtils.UnQuoteString(array[6])));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void DeleteUserMessageRuleAction(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 4)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeleteUserMessageRuleAction <virtualServerID> \"<userID>\" \"<messageRuleID>\" \"<messageRuleActionID>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.DeleteUserMessageRuleAction(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]), TextUtils.UnQuoteString(array[3]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetUserMailboxSize(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetUserMailboxSize <virtualServerID> <userID>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            this.WriteLine("+OK " + virtualServer.InternalService.GetMailboxSize(this.UserName_From_UserID(virtualServer.InternalService.GetUsers("ALL"), TextUtils.UnQuoteString(array[1]))).ToString());
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetUserLastLoginTime(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetUserLastLoginTime <virtualServerID> <userID>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            this.WriteLine("+OK " + virtualServer.InternalService.GetUserLastLoginTime(this.UserName_From_UserID(virtualServer.InternalService.GetUsers("ALL"), TextUtils.UnQuoteString(array[1]))).ToString("u"));
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetUserFolders(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetUserFolders <virtualServerID> <userID>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.CreateUserDefaultFolders(this.UserName_From_UserID(virtualServer.InternalService.GetUsers("ALL"), array[1]));
                            string[] folders = virtualServer.InternalService.GetFolders(this.UserName_From_UserID(virtualServer.InternalService.GetUsers("ALL"), array[1]), false);
                            DataSet dataSet = new DataSet();
                            DataTable dataTable = dataSet.Tables.Add("Folders");
                            dataTable.Columns.Add("Folder");
                            string[] array2 = folders;
                            for (int j = 0; j < array2.Length; j++)
                            {
                                string value = array2[j];
                                DataRow dataRow = dataTable.NewRow();
                                dataRow["Folder"] = value;
                                dataTable.Rows.Add(dataRow);
                            }
                            byte[] array3 = this.CompressDataSet(dataSet);
                            this.WriteLine("+OK " + array3.Length);
                            this.Write(array3);
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void AddUserFolder(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 3)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: AddUserFolder <virtualServerID> \"<folderOwnerUser>\" \"<folder>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.CreateFolder("system", TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void DeleteUserFolder(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 3)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeleteUserFolder <virtualServerID> \"<folderOwnerUser>\" \"<folder>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.DeleteFolder("system", TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void RenameUserFolder(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 4)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: RenameUserFolder <virtualServerID> \"<folderOwnerUser>\" \"<folder>\" \"<newFolder>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.RenameFolder("system", TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]), TextUtils.UnQuoteString(array[3]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetUserFolderInfo(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 3)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetUserFolderInfo <virtualServerID> \"<folderOwnerUser>\" \"<folder>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            List<IMAP_MessageInfo> list = new List<IMAP_MessageInfo>();
                            virtualServer.InternalService.GetMessagesInfo("system", TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]), list);
                            long num = 0L;
                            foreach (IMAP_MessageInfo current in list)
                            {
                                num += (long)current.Size;
                            }
                            this.WriteLine(string.Concat(new object[]
                            {
                                "+OK \"",
                                virtualServer.InternalService.FolderCreationTime(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2])).ToString("yyyyMMdd HH:mm:ss"),
                                "\" ",
                                list.Count,
                                " ",
                                num
                            }));
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetUserFolderMessagesInfo(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ', true);
                if (array.Length != 3)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetUserFolderMessagesInfo <virtualServerID> \"<user>\" \"<folder>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            DataSet dataSet = new DataSet();
                            dataSet.Tables.Add("MessagesInfo");
                            dataSet.Tables["MessagesInfo"].Columns.Add("ID");
                            dataSet.Tables["MessagesInfo"].Columns.Add("UID");
                            dataSet.Tables["MessagesInfo"].Columns.Add("Size", typeof(long));
                            dataSet.Tables["MessagesInfo"].Columns.Add("Flags");
                            dataSet.Tables["MessagesInfo"].Columns.Add("Envelope");
                            List<IMAP_MessageInfo> list = new List<IMAP_MessageInfo>();
                            virtualServer.InternalService.GetMessagesInfo("system", array[1], array[2], list);
                            foreach (IMAP_MessageInfo current in list)
                            {
                                try
                                {
                                    DataRow dataRow = dataSet.Tables["MessagesInfo"].NewRow();
                                    dataRow["ID"] = current.ID;
                                    dataRow["UID"] = current.UID;
                                    dataRow["Size"] = current.Size;
                                    dataRow["Flags"] = Net_Utils.ArrayToString(current.Flags, " ");
                                    EmailMessageItems emailMessageItems = new EmailMessageItems(current.ID, IMAP_MessageItems.Header);
                                    virtualServer.InternalService.GetMessageItems("system", array[1], array[2], emailMessageItems);
                                    dataRow["Envelope"] = IMAP_t_Fetch_r_i_Envelope.ConstructEnvelope(Mail_Message.ParseFromByte(emailMessageItems.Header));
                                    dataSet.Tables["MessagesInfo"].Rows.Add(dataRow);
                                }
                                catch
                                {
                                }
                            }
                            byte[] array2 = this.CompressDataSet(dataSet);
                            this.WriteLine("+OK " + array2.Length);
                            this.Write(array2);
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetUserFolderMessage(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ', true);
                if (array.Length != 4)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetUserFolderMessage <virtualServerID> \"<folderOwnerUser>\" \"<folder>\" \"<messageID>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            EmailMessageItems emailMessageItems = new EmailMessageItems(array[3], IMAP_MessageItems.Message);
                            virtualServer.InternalService.GetMessageItems("system", array[1], array[2], emailMessageItems);
                            if (emailMessageItems.MessageExists)
                            {
                                try
                                {
                                    this.WriteLine("+OK " + ConvertEx.ToString(emailMessageItems.MessageStream.Length - emailMessageItems.MessageStream.Position));
                                    this.Write(emailMessageItems.MessageStream);
                                    goto IL_DD;
                                }
                                finally
                                {
                                    emailMessageItems.MessageStream.Close();
                                }
                            }
                            this.WriteLine("-ERR Specified message doesn't exist !");
                            IL_DD:
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void StoreUserFolderMessage(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ', true);
                if (array.Length != 4)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: StoreUserFolderMessage <virtualServerID> \"<folderOwnerUser>\" \"<folder>\" <sizeInBytes>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            this.WriteLine("+OK");
                            using (SwapableStream swapableStream = new SwapableStream(32000))
                            {
                                this.TcpStream.ReadFixedCount(swapableStream, (long)Convert.ToInt32(array[3]));
                                swapableStream.Position = 0L;
                                virtualServer.InternalService.StoreMessage("system", array[1], array[2], swapableStream, DateTime.Now, new string[]
                                {
                                    "Recent"
                                });
                            }
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void DeleteUserFolderMessage(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ', true);
                if (array.Length != 5)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeleteUserFolderMessage <virtualServerID> \"<folderOwnerUser>\" \"<folder>\" \"<ID>\" \"<UID>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.DeleteMessage("system", array[1], array[2], array[3], ConvertEx.ToInt32(array[4]));
                            this.WriteLine("+OK ");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetUserRemoteServers(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetUserRemoteServers <virtualServerID> <userID>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            DataSet ds = this.DataView_To_DataSet(virtualServer.InternalService.GetUserRemoteServers(this.UserName_From_UserID(virtualServer.InternalService.GetUsers("ALL"), array[1])));
                            byte[] array2 = this.CompressDataSet(ds);
                            this.WriteLine("+OK " + array2.Length);
                            this.Write(array2);
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void AddUserRemoteServer(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 10)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: AddUserRemoteServer <virtualServerID> \"<remoteServerID>\" \"<userName>\" \"<description>\" \"<remoteHost>\" <remoteHostPort> \"<remoteHostUserName>\" \"<remoteHostPassword>\" <ssl> <enabled>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.AddUserRemoteServer(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]), TextUtils.UnQuoteString(array[3]), TextUtils.UnQuoteString(array[4]), Convert.ToInt32(array[5]), TextUtils.UnQuoteString(array[6]), TextUtils.UnQuoteString(array[7]), Convert.ToBoolean(array[8]), Convert.ToBoolean(array[9]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void DeleteUserRemoteServer(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeleteUserRemoteServer <virtualServerID> \"<remoteServerID>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.DeleteUserRemoteServer(TextUtils.UnQuoteString(array[1]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void UpdateUserRemoteServer(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 10)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: UpdateUserRemoteServer <virtualServerID> <userID>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.UpdateUserRemoteServer(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]), TextUtils.UnQuoteString(array[3]), TextUtils.UnQuoteString(array[4]), Convert.ToInt32(array[5]), TextUtils.UnQuoteString(array[6]), TextUtils.UnQuoteString(array[7]), Convert.ToBoolean(array[8]), Convert.ToBoolean(array[9]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetUserFolderAcl(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 3)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetUserFolders <virtualServerID> <userID> \"<FolderName>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            DataSet ds = this.DataView_To_DataSet(virtualServer.InternalService.GetFolderACL("system", this.UserName_From_UserID(virtualServer.InternalService.GetUsers("ALL"), array[1]), TextUtils.UnQuoteString(array[2])));
                            byte[] array2 = this.CompressDataSet(ds);
                            this.WriteLine("+OK " + array2.Length);
                            this.Write(array2);
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void SetUserFolderAcl(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 5)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetUserFolders <virtualServerID> \"<folderOwnerUser>\" \"<folder>\" \"<userOrGroup>\" <flags:int32>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            new MemoryStream();
                            virtualServer.InternalService.SetFolderACL("system", TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]), TextUtils.UnQuoteString(array[3]), IMAP_Flags_SetType.Replace, (IMAP_ACL_Flags)Convert.ToInt32(array[4]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void DeleteUserFolderAcl(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 4)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeleteUserFolderAcl <virtualServerID> \"<folderOwnerUser>\" \"<folder>\" \"<userOrGroup>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.DeleteFolderACL("system", TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]), TextUtils.UnQuoteString(array[3]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetUsersDefaultFolders(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 1)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetUsersDefaultFolders <virtualServerID>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            DataSet ds = this.DataView_To_DataSet(virtualServer.InternalService.GetUsersDefaultFolders());
                            byte[] array2 = this.CompressDataSet(ds);
                            this.WriteLine("+OK " + array2.Length);
                            this.Write(array2);
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void AddUsersDefaultFolder(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 3)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: AddUsersDefaultFolder <virtualServerID> \"<folderName>\" <permanent>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.AddUsersDefaultFolder(TextUtils.UnQuoteString(array[1]), ConvertEx.ToBoolean(array[2]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void DeleteUsersDefaultFolder(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeleteUsersDefaultFolder <virtualServerID> \"<<folderName>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.DeleteUsersDefaultFolder(TextUtils.UnQuoteString(array[1]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetGroups(string argsText)
        {
            try
            {
                VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                for (int i = 0; i < virtualServers.Length; i++)
                {
                    VirtualServer virtualServer = virtualServers[i];
                    if (virtualServer.ID.ToLower() == argsText.ToLower())
                    {
                        DataSet dataSet = virtualServer.InternalService.GetGroups().Table.DataSet;
                        byte[] array = this.CompressDataSet(dataSet);
                        this.WriteLine("+OK " + array.Length);
                        this.Write(array);
                        return;
                    }
                }
                this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void AddGroup(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 5)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: UpdateGroup <virtualServerID> \"<groupID>\" \"<groupName>\" \"<description>\" <enabled>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.AddGroup(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]), TextUtils.UnQuoteString(array[3]), Convert.ToBoolean(array[4]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void UpdateGroup(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 5)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: UpdateGroup <virtualServerID> \"<groupID>\" \"<groupName>\" \"<description>\" <enabled>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.UpdateGroup(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]), TextUtils.UnQuoteString(array[3]), Convert.ToBoolean(array[4]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void DeleteGroup(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeleteGroup <virtualServerID> \"<groupID>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.DeleteGroup(TextUtils.UnQuoteString(array[1]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetGroupMembers(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetGroupMembers <virtualServerID> <groupID>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            string[] groupMembers = virtualServer.InternalService.GetGroupMembers(this.GroupName_From_GroupID(virtualServer.InternalService.GetGroups(), array[1]));
                            DataSet dataSet = new DataSet();
                            DataTable dataTable = dataSet.Tables.Add("Members");
                            dataTable.Columns.Add("Member");
                            string[] array2 = groupMembers;
                            for (int j = 0; j < array2.Length; j++)
                            {
                                string value = array2[j];
                                DataRow dataRow = dataTable.NewRow();
                                dataRow["Member"] = value;
                                dataTable.Rows.Add(dataRow);
                            }
                            byte[] array3 = this.CompressDataSet(dataSet);
                            this.WriteLine("+OK " + array3.Length);
                            this.Write(array3);
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void AddGroupMember(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 3)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: AddGroupMember <virtualServerID> \"<groupID>\" \"<member>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.AddGroupMember(this.GroupName_From_GroupID(virtualServer.InternalService.GetGroups(), TextUtils.UnQuoteString(array[1])), TextUtils.UnQuoteString(array[2]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void DeleteGroupMember(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 3)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeletGroupMember <virtualServerID> \"<groupID>\" \"<member>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.DeleteGroupMember(this.GroupName_From_GroupID(virtualServer.InternalService.GetGroups(), TextUtils.UnQuoteString(array[1])), TextUtils.UnQuoteString(array[2]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetMailingLists(string argsText)
        {
            try
            {
                VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                for (int i = 0; i < virtualServers.Length; i++)
                {
                    VirtualServer virtualServer = virtualServers[i];
                    if (virtualServer.ID.ToLower() == argsText.ToLower())
                    {
                        DataSet dataSet = virtualServer.InternalService.GetMailingLists("ALL").Table.DataSet;
                        byte[] array = this.CompressDataSet(dataSet);
                        this.WriteLine("+OK " + array.Length);
                        this.Write(array);
                        return;
                    }
                }
                this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void AddMailingList(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 5)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: AddMailingList <virtualServerID> \"<mailingListID>\" \"<mailingListName>\" \"<description>\" <enabled>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.AddMailingList(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]), TextUtils.UnQuoteString(array[3]), "", Convert.ToBoolean(array[4]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void UpdateMailingList(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 5)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: UpdateGroup <virtualServerID> \"<mailingListID>\" \"<mailingListName>\" \"<description>\" <enabled>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.UpdateMailingList(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]), TextUtils.UnQuoteString(array[3]), "", Convert.ToBoolean(array[4]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void DeleteMailingList(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeleteMailingList <virtualServerID> \"<mailingListID>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.DeleteMailingList(TextUtils.UnQuoteString(array[1]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetMailingListMembers(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetMailingListMembers <virtualServerID> <mailignListID>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            DataSet ds = this.DataView_To_DataSet(virtualServer.InternalService.GetMailingListAddresses(this.MailingListName_From_ID(virtualServer.InternalService.GetMailingLists("ALL"), array[1])));
                            byte[] array2 = this.CompressDataSet(ds);
                            this.WriteLine("+OK " + array2.Length);
                            this.Write(array2);
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void AddMailingListMember(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 3)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: AddMailingListMember <virtualServerID> \"<mailingListID>\" \"<member>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.AddMailingListAddress(Guid.NewGuid().ToString(), this.MailingListName_From_ID(virtualServer.InternalService.GetMailingLists("ALL"), TextUtils.UnQuoteString(array[1])), TextUtils.UnQuoteString(array[2]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void DeleteMailingListMember(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 3)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeleteMailingListMember <virtualServerID> \"<mailingListID>\" \"<member>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.DeleteMailingListAddress(this.AddressID_From_MailingListMember(virtualServer.InternalService.GetMailingListAddresses(this.MailingListName_From_ID(virtualServer.InternalService.GetMailingLists("ALL"), TextUtils.UnQuoteString(array[1]))), TextUtils.UnQuoteString(array[2])));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetMailingListAcl(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetMailingListMembers <virtualServerID> <mailignListID>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            DataSet ds = this.DataView_To_DataSet(virtualServer.InternalService.GetMailingListACL(this.MailingListName_From_ID(virtualServer.InternalService.GetMailingLists("ALL"), array[1])));
                            byte[] array2 = this.CompressDataSet(ds);
                            this.WriteLine("+OK " + array2.Length);
                            this.Write(array2);
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void AddMailingListAcl(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 3)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: AddMailingListAcl <virtualServerID> \"<mailingListID>\" \"<userOrGroup>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.AddMailingListACL(this.MailingListName_From_ID(virtualServer.InternalService.GetMailingLists("ALL"), TextUtils.UnQuoteString(array[1])), TextUtils.UnQuoteString(array[2]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void DeleteMailingListAcl(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 3)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeleteMailingListAcl <virtualServerID> \"<mailingListID>\" \"<userOrGroup>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.DeleteMailingListACL(this.MailingListName_From_ID(virtualServer.InternalService.GetMailingLists("ALL"), TextUtils.UnQuoteString(array[1])), TextUtils.UnQuoteString(array[2]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetGlobalMessageRules(string argsText)
        {
            try
            {
                VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                for (int i = 0; i < virtualServers.Length; i++)
                {
                    VirtualServer virtualServer = virtualServers[i];
                    if (virtualServer.ID.ToLower() == argsText.ToLower())
                    {
                        DataSet ds = this.DataView_To_DataSet(virtualServer.InternalService.GetGlobalMessageRules());
                        byte[] array = this.CompressDataSet(ds);
                        this.WriteLine("+OK " + array.Length);
                        this.Write(array);
                        return;
                    }
                }
                this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void AddGlobalMessageRule(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 7)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: AddGlobalMessageRule <virtualServerID> \"<ruleID>\" <cost> <enabled> \"<description>\" \"<matchExpression>\" <checkNext>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.AddGlobalMessageRule(TextUtils.UnQuoteString(array[1]), Convert.ToInt64(array[2]), Convert.ToBoolean(array[3]), (GlobalMessageRule_CheckNextRule)Convert.ToInt32(array[6]), TextUtils.UnQuoteString(array[4]), TextUtils.UnQuoteString(array[5]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void UpdateGlobalMessageRule(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 7)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: UpdateGlopbalMessageRule <virtualServerID> \"<ruleID>\" <cost> <enabled> \"<description>\" \"<matchExpression>\" <checkNext>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.UpdateGlobalMessageRule(TextUtils.UnQuoteString(array[1]), Convert.ToInt64(array[2]), Convert.ToBoolean(array[3]), (GlobalMessageRule_CheckNextRule)Convert.ToInt32(array[6]), TextUtils.UnQuoteString(array[4]), TextUtils.UnQuoteString(array[5]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void DeleteGlobalMessageRule(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeleteGlobalMessageRule <virtualServerID> \"<ruleID>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.DeleteGlobalMessageRule(TextUtils.UnQuoteString(array[1]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetGlobalMessageRuleActions(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetGlobalMessageRuleActions <virtualServerID> \"<messageRuleID>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            DataSet ds = this.DataView_To_DataSet(virtualServer.InternalService.GetGlobalMessageRuleActions(TextUtils.UnQuoteString(array[1])));
                            byte[] array2 = this.CompressDataSet(ds);
                            this.WriteLine("+OK " + array2.Length);
                            this.Write(array2);
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void AddGlobalMessageRuleAction(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 6)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: AddGlobalMessageRuleAction <virtualServerID> \"<messageRuleID>\" \"<messageRuleActionID>\" \"<description>\" <actionType> \"<actionData>:base64\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.AddGlobalMessageRuleAction(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]), TextUtils.UnQuoteString(array[3]), (GlobalMessageRuleActionType)Convert.ToInt32(array[4]), Convert.FromBase64String(TextUtils.UnQuoteString(array[5])));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void UpdateGlobalMessageRuleAction(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 6)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: UpdateGlobalMessageRuleAction <virtualServerID> \"<messageRuleID>\" \"<messageRuleActionID>\" \"<description>\" <actionType> \"<actionData>:base64\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.UpdateGlobalMessageRuleAction(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]), TextUtils.UnQuoteString(array[3]), (GlobalMessageRuleActionType)Convert.ToInt32(array[4]), Convert.FromBase64String(TextUtils.UnQuoteString(array[5])));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void DeleteGlobalMessageRuleAction(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 3)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeleteGlobalMessageRuleAction <virtualServerID> \"<messageRuleID>\" \"<messageRuleActionID>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.DeleteGlobalMessageRuleAction(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[2]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetRoutes(string argsText)
        {
            try
            {
                VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                for (int i = 0; i < virtualServers.Length; i++)
                {
                    VirtualServer virtualServer = virtualServers[i];
                    if (virtualServer.ID.ToLower() == argsText.ToLower())
                    {
                        DataSet dataSet = virtualServer.InternalService.GetRoutes().Table.DataSet;
                        byte[] array = this.CompressDataSet(dataSet);
                        this.WriteLine("+OK " + array.Length);
                        this.Write(array);
                        return;
                    }
                }
                this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void AddRoute(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 8)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: AddRoute <virtualServerID> \"<routeID>\" <cost> \"<description>\" \"<pattern>\" <enabled> <actionType> \"<actionData>:base64\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.AddRoute(TextUtils.UnQuoteString(array[1]), Convert.ToInt64(array[2]), Convert.ToBoolean(array[5]), TextUtils.UnQuoteString(array[3]), TextUtils.UnQuoteString(array[4]), (RouteAction)Convert.ToInt32(array[6]), Convert.FromBase64String(TextUtils.UnQuoteString(array[7])));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void UpdateRoute(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 8)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: UpdateRoute <virtualServerID> \"<routeID>\" <cost> \"<description>\" \"<pattern>\" <enabled> <actionType> \"<actionData>:base64\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.UpdateRoute(TextUtils.UnQuoteString(array[1]), Convert.ToInt64(array[2]), Convert.ToBoolean(array[5]), TextUtils.UnQuoteString(array[3]), TextUtils.UnQuoteString(array[4]), (RouteAction)Convert.ToInt32(array[6]), Convert.FromBase64String(TextUtils.UnQuoteString(array[7])));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void DeleteRoute(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeleteRoute <virtualServerID> \"<routeID>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.DeleteRoute(TextUtils.UnQuoteString(array[1]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetSharedRootFolders(string argsText)
        {
            try
            {
                VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                for (int i = 0; i < virtualServers.Length; i++)
                {
                    VirtualServer virtualServer = virtualServers[i];
                    if (virtualServer.ID.ToLower() == argsText.ToLower())
                    {
                        DataSet dataSet = new DataSet();
                        dataSet.Tables.Add("SharedFoldersRoots");
                        dataSet.Tables["SharedFoldersRoots"].Columns.Add("RootID");
                        dataSet.Tables["SharedFoldersRoots"].Columns.Add("Enabled");
                        dataSet.Tables["SharedFoldersRoots"].Columns.Add("Folder");
                        dataSet.Tables["SharedFoldersRoots"].Columns.Add("Description");
                        dataSet.Tables["SharedFoldersRoots"].Columns.Add("RootType");
                        dataSet.Tables["SharedFoldersRoots"].Columns.Add("BoundedUser");
                        dataSet.Tables["SharedFoldersRoots"].Columns.Add("BoundedFolder");
                        SharedFolderRoot[] sharedFolderRoots = virtualServer.InternalService.GetSharedFolderRoots();
                        for (int j = 0; j < sharedFolderRoots.Length; j++)
                        {
                            SharedFolderRoot sharedFolderRoot = sharedFolderRoots[j];
                            DataRow dataRow = dataSet.Tables["SharedFoldersRoots"].NewRow();
                            dataRow["RootID"] = sharedFolderRoot.RootID;
                            dataRow["Enabled"] = sharedFolderRoot.Enabled;
                            dataRow["Folder"] = sharedFolderRoot.FolderName;
                            dataRow["Description"] = sharedFolderRoot.Description;
                            dataRow["RootType"] = (int)sharedFolderRoot.RootType;
                            dataRow["BoundedUser"] = sharedFolderRoot.BoundedUser;
                            dataRow["BoundedFolder"] = sharedFolderRoot.BoundedFolder;
                            dataSet.Tables["SharedFoldersRoots"].Rows.Add(dataRow);
                        }
                        byte[] array = this.CompressDataSet(dataSet);
                        this.WriteLine("+OK " + array.Length);
                        this.Write(array);
                        return;
                    }
                }
                this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void AddSharedRootFolder(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 8)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: AddSharedRootFolder <virtualServerID> \"<rootFolderID>\" \"<rootFolderName>\" \"<description>\" <type> \"<boundedUser>\" \"boundedFolder\" <enabled>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.AddSharedFolderRoot(TextUtils.UnQuoteString(array[1]), Convert.ToBoolean(array[7]), TextUtils.UnQuoteString(array[2]), TextUtils.UnQuoteString(array[3]), (SharedFolderRootType)Convert.ToInt32(array[4]), TextUtils.UnQuoteString(array[5]), TextUtils.UnQuoteString(array[6]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void UpdateSharedRootFolder(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 8)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: UpdateSharedRootFolder <virtualServerID> \"<rootFolderID>\" \"<rootFolderName>\" \"<description>\" <type> \"<boundedUser>\" \"boundedFolder\" <enabled>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.UpdateSharedFolderRoot(TextUtils.UnQuoteString(array[1]), Convert.ToBoolean(array[7]), TextUtils.UnQuoteString(array[2]), TextUtils.UnQuoteString(array[3]), (SharedFolderRootType)Convert.ToInt32(array[4]), TextUtils.UnQuoteString(array[5]), TextUtils.UnQuoteString(array[6]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void DeleteSharedRootFolder(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeleteSharedRootFolder <virtualServerID> \"<rootFolderID>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.DeleteSharedFolderRoot(TextUtils.UnQuoteString(array[1]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetFilterTypes(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 1)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetFilterTypes <virtualServerID>");
                }
                else
                {
                    DataSet dataSet = new DataSet();
                    dataSet.Tables.Add("Filters");
                    dataSet.Tables["Filters"].Columns.Add("AssemblyName");
                    dataSet.Tables["Filters"].Columns.Add("TypeName");
                    string[] files = Directory.GetFiles(this.Server.MailServer.StartupPath);
                    var interfaces = new Type[] { typeof(ISmtpMessageFilter), typeof(ISmtpSenderFilter), typeof(ISmtpUserMessageFilter) };
                    for (int i = 0; i < files.Length; i++)
                    {
                        string path = files[i];
                        try
                        {
                            if (Path.GetExtension(path) == ".exe" || Path.GetExtension(path) == ".dll")
                            {
                                Assembly assembly = Assembly.LoadFile(path);
                                var types = assembly.ExportedTypes.Where(x => interfaces.Any(itf => itf.IsAssignableFrom(x)) && !x.IsAbstract);
                                foreach (var type in types)
                                {
                                    DataRow dataRow = dataSet.Tables["Filters"].NewRow();
                                    dataRow["AssemblyName"] = Path.GetFileName(path);
                                    dataRow["TypeName"] = type.ToString();
                                    dataSet.Tables["Filters"].Rows.Add(dataRow);
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                    byte[] array4 = this.CompressDataSet(dataSet);
                    this.WriteLine("+OK " + array4.Length);
                    this.Write(array4);
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetFilters(string argsText)
        {
            try
            {
                VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                for (int i = 0; i < virtualServers.Length; i++)
                {
                    VirtualServer virtualServer = virtualServers[i];
                    if (virtualServer.ID.ToLower() == argsText.ToLower())
                    {
                        DataSet dataSet = virtualServer.InternalService.GetFilters().Table.DataSet;
                        byte[] array = this.CompressDataSet(dataSet);
                        this.WriteLine("+OK " + array.Length);
                        this.Write(array);
                        return;
                    }
                }
                this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void AddFilter(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 7)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: AddFilter <virtualServerID> \"<filterID>\" <cost> \"<description>\" \"<assembly>\" \"<filterClass>\" <enabled>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            string type = "";
                            try
                            {
                                string text = TextUtils.UnQuoteString(array[4]);
                                if (text.IndexOf(":") == -1)
                                {
                                    text = Path.Combine(this.Server.MailServer.StartupPath, text);
                                }
                                Assembly assembly = Assembly.LoadFile(text);
                                Type type2 = assembly.GetType(TextUtils.UnQuoteString(array[5]));
                                if (type2 == null)
                                {
                                    this.WriteLine("-ERR filterClass contains invalid value !");
                                    return;
                                }
                                if (type2.GetInterface("DataSmart.MailServer.ISmtpMessageFilter") != null)
                                {
                                    type = "ISmtpMessageFilter";
                                }
                                else if (type2.GetInterface("DataSmart.MailServer.ISmtpSenderFilter") != null)
                                {
                                    type = "ISmtpSenderFilter";
                                }
                                else if (type2.GetInterface("DataSmart.MailServer.ISmtpUserMessageFilter") != null)
                                {
                                    type = "ISmtpUserMessageFilter";
                                }
                            }
                            catch (Exception ex)
                            {
                                this.WriteLine("-ERR " + ex.Message);
                                return;
                            }
                            virtualServer.InternalService.AddFilter(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[3]), type, TextUtils.UnQuoteString(array[4]), TextUtils.UnQuoteString(array[5]), Convert.ToInt64(array[2]), Convert.ToBoolean(TextUtils.UnQuoteString(array[6])));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex2)
            {
                this.WriteLine("-ERR " + ex2.Message + "\r\n" + ex2.StackTrace);
            }
        }

        private void UpdateFilter(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 7)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: AddFilter <virtualServerID> \"<filterID>\" <cost> \"<description>\" \"<assembly>\" \"<filterClass>\" <enabled>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            string type = "";
                            try
                            {
                                string text = TextUtils.UnQuoteString(array[4]);
                                if (text.IndexOf(":") == -1)
                                {
                                    text = Path.Combine(this.Server.MailServer.StartupPath, text);
                                }
                                Assembly assembly = Assembly.LoadFile(text);
                                Type type2 = assembly.GetType(TextUtils.UnQuoteString(array[5]));
                                if (type2 == null)
                                {
                                    this.WriteLine("-ERR filterClass contains invalid value !");
                                    return;
                                }
                                if (type2.GetInterface("DataSmart.MailServer.ISmtpMessageFilter") != null)
                                {
                                    type = "ISmtpMessageFilter";
                                }
                                else if (type2.GetInterface("DataSmart.MailServer.ISmtpSenderFilter") != null)
                                {
                                    type = "ISmtpSenderFilter";
                                }
                                else if (type2.GetInterface("DataSmart.MailServer.ISmtpUserMessageFilter") != null)
                                {
                                    type = "ISmtpUserMessageFilter";
                                }
                            }
                            catch (Exception ex)
                            {
                                this.WriteLine("-ERR " + ex.Message);
                                return;
                            }
                            virtualServer.InternalService.UpdateFilter(TextUtils.UnQuoteString(array[1]), TextUtils.UnQuoteString(array[3]), type, TextUtils.UnQuoteString(array[4]), TextUtils.UnQuoteString(array[5]), Convert.ToInt64(array[2]), Convert.ToBoolean(TextUtils.UnQuoteString(array[6])));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex2)
            {
                this.WriteLine("-ERR " + ex2.Message + "\r\n" + ex2.StackTrace);
            }
        }

        private void DeleteFilter(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeleteFilter <virtualServerID> \"<filterID>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.DeleteFilter(TextUtils.UnQuoteString(array[1]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetIPSecurity(string argsText)
        {
            try
            {
                VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                for (int i = 0; i < virtualServers.Length; i++)
                {
                    VirtualServer virtualServer = virtualServers[i];
                    if (virtualServer.ID.ToLower() == argsText.ToLower())
                    {
                        DataSet dataSet = virtualServer.InternalService.GetSecurityList().Table.DataSet;
                        byte[] array = this.CompressDataSet(dataSet);
                        this.WriteLine("+OK " + array.Length);
                        this.Write(array);
                        return;
                    }
                }
                this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void AddIPSecurityEntry(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 8)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: AddIPSecurityEntry <virtualServerID> \"<securityEntryID>\" enabled \"<description>\" <service> <action> \"<startIP>\" \"<endIP>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.AddSecurityEntry(TextUtils.UnQuoteString(array[1]), Convert.ToBoolean(array[2]), TextUtils.UnQuoteString(array[3]), (ServiceType)Convert.ToInt32(array[4]), (IPSecurityAction)Convert.ToInt32(array[5]), IPAddress.Parse(TextUtils.UnQuoteString(array[6])), IPAddress.Parse(TextUtils.UnQuoteString(array[7])));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void UpdateIPSecurityEntry(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 8)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: UpdateIPSecurityEntry <virtualServerID> \"<securityEntryID>\" enabled \"<description>\" <service> <action> \"<startIP>\" \"<endIP>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.UpdateSecurityEntry(TextUtils.UnQuoteString(array[1]), Convert.ToBoolean(array[2]), TextUtils.UnQuoteString(array[3]), (ServiceType)Convert.ToInt32(array[4]), (IPSecurityAction)Convert.ToInt32(array[5]), IPAddress.Parse(TextUtils.UnQuoteString(array[6])), IPAddress.Parse(TextUtils.UnQuoteString(array[7])));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void DeleteIPSecurityEntry(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: DeleteIPSecurityEntry <virtualServerID> \"<securityEntryID>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.DeleteSecurityEntry(TextUtils.UnQuoteString(array[1]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetQueue(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetQueue <virtualServerID> <queueType>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            DataSet dataSet = new DataSet();
                            dataSet.Tables.Add("Queue");
                            dataSet.Tables["Queue"].Columns.Add("CreateTime", typeof(DateTime));
                            dataSet.Tables["Queue"].Columns.Add("Header");
                            string path;
                            if (Convert.ToInt32(array[1]) == 1)
                            {
                                path = virtualServer.MailStorePath + "IncomingSMTP";
                            }
                            else
                            {
                                path = virtualServer.MailStorePath + "Retry";
                            }
                            if (Directory.Exists(path))
                            {
                                string[] files = Directory.GetFiles(path, "*.eml");
                                for (int j = 0; j < files.Length; j++)
                                {
                                    string path2 = files[j];
                                    try
                                    {
                                        string text = "";
                                        using (FileStream fileStream = new FileStream(path2, FileMode.Open, FileAccess.Read, FileShare.Read))
                                        {
                                            MIME_h_Collection mIME_h_Collection = new MIME_h_Collection(new MIME_h_Provider());
                                            mIME_h_Collection.Parse(new SmartStream(fileStream, false));
                                            text = mIME_h_Collection.ToString();
                                        }
                                        if (text.Length > 2000)
                                        {
                                            text = text.Substring(0, 2000);
                                        }
                                        DataRow dataRow = dataSet.Tables["Queue"].NewRow();
                                        dataRow["CreateTime"] = File.GetCreationTime(path2);
                                        dataRow["Header"] = text;
                                        dataSet.Tables["Queue"].Rows.Add(dataRow);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                            byte[] array3 = this.CompressDataSet(dataSet);
                            this.WriteLine("+OK " + array3.Length);
                            this.Write(array3);
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetRecycleBinSettings(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 1)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetRecycleBinSettings <virtualServerID>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            MemoryStream memoryStream = new MemoryStream();
                            virtualServer.InternalService.GetRecycleBinSettings().DataSet.WriteXml(memoryStream);
                            memoryStream.Position = 0L;
                            this.WriteLine("+OK " + memoryStream.Length);
                            this.Write(memoryStream);
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void UpdateRecycleBinSettings(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 3)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: UpdateRecycleBinSettings <virtualServerID> <deleteToRecycleBin> <deleteMessagesAfter>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.UpdateRecycleBinSettings(Convert.ToBoolean(array[1]), Convert.ToInt32(array[2]));
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetRecycleBinMessagesInfo(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ', true);
                if (array.Length != 4)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetRecycleBinMessagesInfo <virtualServerID> \"<user>\" \"<startDate>\" \"<endDate>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            DataSet dataSet = virtualServer.InternalService.GetRecycleBinMessagesInfo(array[1], DateTime.ParseExact(array[2], "yyyyMMddHHmmss", CultureInfo.InvariantCulture), DateTime.ParseExact(array[3], "yyyyMMddHHmmss", CultureInfo.InvariantCulture)).Table.DataSet;
                            byte[] array2 = this.CompressDataSet(dataSet);
                            this.WriteLine("+OK " + array2.Length);
                            this.Write(array2);
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void GetRecycleBinMessage(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ', true);
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: GetRecycleBinMessage <virtualServerID> \"<messageID>\"");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            Stream recycleBinMessage = virtualServer.InternalService.GetRecycleBinMessage(array[1]);
                            try
                            {
                                this.WriteLine("+OK " + ConvertEx.ToString(recycleBinMessage.Length - recycleBinMessage.Position));
                                this.Write(recycleBinMessage);
                            }
                            finally
                            {
                                recycleBinMessage.Close();
                            }
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + array[0] + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void RestoreRecycleBinMessage(string argsText)
        {
            try
            {
                string[] array = TextUtils.SplitQuotedString(argsText, ' ');
                if (array.Length != 2)
                {
                    this.WriteLine("-ERR Invalid arguments. Syntax: RestoreRecycleBinMessage <virtualServerID> <messageID>");
                }
                else
                {
                    VirtualServer[] virtualServers = this.Server.MailServer.VirtualServers;
                    for (int i = 0; i < virtualServers.Length; i++)
                    {
                        VirtualServer virtualServer = virtualServers[i];
                        if (virtualServer.ID.ToLower() == array[0].ToLower())
                        {
                            virtualServer.InternalService.RestoreRecycleBinMessage(array[1]);
                            this.WriteLine("+OK");
                            return;
                        }
                    }
                    this.WriteLine("-ERR Specified virtual server with ID '" + argsText + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                this.WriteLine("-ERR " + ex.Message);
            }
        }

        private void QUIT()
        {
            this.WriteLine("+OK Service closing transmission channel");
        }

        private void WriteLine(string line)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }
            this.TcpStream.WriteLine(line);
        }

        private void Write(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            this.TcpStream.Write(data, 0, data.Length);
        }

        private void Write(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            Net_Utils.StreamCopy(stream, this.TcpStream, 1024);
        }

        private string ObjectToString(object val)
        {
            if (val != null)
            {
                return val.ToString();
            }
            return "";
        }

        private byte[] CompressDataSet(DataSet ds)
        {
            MemoryStream memoryStream = new MemoryStream();
            ds.WriteXml(memoryStream);
            MemoryStream memoryStream2 = new MemoryStream();
            GZipStream gZipStream = new GZipStream(memoryStream2, CompressionMode.Compress);
            byte[] array = memoryStream.ToArray();
            gZipStream.Write(array, 0, array.Length);
            gZipStream.Flush();
            gZipStream.Dispose();
            return memoryStream2.ToArray();
        }

        private string UserName_From_UserID(DataView users, string userID)
        {
            foreach (DataRowView dataRowView in users)
            {
                if (string.Equals(dataRowView["UserID"].ToString(), userID, StringComparison.InvariantCultureIgnoreCase))
                {
                    return dataRowView["UserName"].ToString();
                }
            }
            throw new Exception("Specified userID '" + userID + "' doesn't exist !");
        }

        private string GroupName_From_GroupID(DataView groups, string groupID)
        {
            foreach (DataRowView dataRowView in groups)
            {
                if (string.Equals(dataRowView["GroupID"].ToString(), groupID))
                {
                    return dataRowView["GroupName"].ToString();
                }
            }
            throw new Exception("Specified groupID '" + groupID + "' doesn't exist !");
        }

        private string MailingListName_From_ID(DataView mailingLists, string mailingListID)
        {
            foreach (DataRowView dataRowView in mailingLists)
            {
                if (string.Equals(dataRowView["MailingListID"].ToString(), mailingListID, StringComparison.InvariantCultureIgnoreCase))
                {
                    return dataRowView["MailingListName"].ToString();
                }
            }
            throw new Exception("Specified mailingList ID '" + mailingListID + "' doesn't exist !");
        }

        private string AddressID_From_MailingListMember(DataView mailingListMembers, string member)
        {
            foreach (DataRowView dataRowView in mailingListMembers)
            {
                if (string.Equals(dataRowView["Address"].ToString(), member, StringComparison.InvariantCultureIgnoreCase))
                {
                    return dataRowView["AddressID"].ToString();
                }
            }
            throw new Exception("Specified mailing list member '" + member + "' doesn't exist !");
        }

        private string AddressID_From_UserEmail(DataView userEmails, string emailAddress)
        {
            foreach (DataRowView dataRowView in userEmails)
            {
                if (string.Equals(dataRowView["Address"].ToString(), emailAddress, StringComparison.InvariantCultureIgnoreCase))
                {
                    return dataRowView["AddressID"].ToString();
                }
            }
            throw new Exception("Specified emailaddress '" + emailAddress + "' doesn't exist !");
        }

        private DataSet DataView_To_DataSet(DataView dv)
        {
            DataSet dataSet = new DataSet();
            DataTable dataTable = dv.Table.Clone();
            dataSet.Tables.Add(dataTable);
            foreach (DataRowView dataRowView in dv)
            {
                dataTable.ImportRow(dataRowView.Row);
            }
            return dataSet;
        }
    }
}
