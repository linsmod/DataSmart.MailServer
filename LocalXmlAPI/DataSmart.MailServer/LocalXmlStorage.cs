using System.NetworkToolkit;
using System.NetworkToolkit.AUTH;
using System.NetworkToolkit.IMAP;
using System.NetworkToolkit.IMAP.Server;
using System.NetworkToolkit.Mail;
using System.NetworkToolkit.MIME;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;

namespace DataSmart.MailServer
{
    public class LocalXmlStorage : IMailServerManagementApi
    {
        private class SharedFolderMapInfo
        {
            private string m_OriginalFolder = "";

            private string m_FolderOwner = "";

            private string m_Folder = "";

            private string m_SharedRootName = "";

            public string OriginalFolder
            {
                get
                {
                    return this.m_OriginalFolder;
                }
            }

            public string FolderOnwer
            {
                get
                {
                    return this.m_FolderOwner;
                }
            }

            public string Folder
            {
                get
                {
                    return this.m_Folder;
                }
            }

            public string SharedRootName
            {
                get
                {
                    return this.m_SharedRootName;
                }
            }

            public bool IsSharedFolder
            {
                get
                {
                    return this.m_SharedRootName != "";
                }
            }

            public SharedFolderMapInfo(string originalFolder, string folderOwner, string folder, string sharedRootName)
            {
                this.m_OriginalFolder = originalFolder;
                this.m_FolderOwner = folderOwner;
                this.m_Folder = folder;
                this.m_SharedRootName = sharedRootName;
            }
        }

        private string m_DataPath = "";

        private string m_MailStorePath = "";

        private DataSet dsSettings;

        private DataSet dsUsers;

        private DataSet dsUserAddresses;

        private DataSet dsUserRemoteServers;

        private DataSet dsUserMessageRules;

        private DataSet dsUserMessageRuleActions;

        private DataSet dsUserForwards;

        private DataSet dsGroups;

        private DataSet dsGroupMembers;

        private DataSet dsMailingLists;

        private DataSet dsMailingListAddresses;

        private DataSet dsMailingListACL;

        private DataSet dsDomains;

        private DataSet dsRules;

        private DataSet dsRuleActions;

        private DataSet dsRouting;

        private DataSet dsSecurity;

        private DataSet dsFilters;

        private DataSet dsImapACL;

        private DataSet dsSharedFolderRoots;

        private DataSet dsUsersDefaultFolders;

        private DataSet dsRecycleBinSettings;

        private DateTime m_UsersDate;

        private DateTime m_UserAddressesDate;

        private DateTime m_UsersRemoteServers;

        private DateTime m_UserMessageRules;

        private DateTime m_UserMessageRuleActions;

        private DateTime m_GroupsDate;

        private DateTime m_GroupMembersDate;

        private DateTime m_MailingListsDate;

        private DateTime m_MailingListAddressesDate;

        private DateTime m_MailingListAclDate;

        private DateTime m_RulesDate;

        private DateTime m_RuleActionsDate;

        private DateTime m_RoutingDate;

        private DateTime m_DomainsDate;

        private DateTime m_SecurityDate;

        private DateTime m_FiltersDate;

        private DateTime m_ImapACLDate;

        private DateTime m_SharedFolderRootsDate;

        private DateTime m_UsersDefaultFoldersDate;

        private DateTime m_RecycleBinSettingsDate;

        private System.Timers.Timer timer1;

        private UpdateSync m_UpdSync;

        public LocalXmlStorage()
        {
        }

        public LocalXmlStorage(string intitString)
        {
            this.m_UpdSync = new UpdateSync();
            if (intitString == null || intitString == "")
            {
                throw new Exception("Init string can't be null or \"\" !");
            }
            string[] array = intitString.Replace("\r\n", "\n").Split(new char[]
            {
                '\n'
            });
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string text = array2[i];
                if (text.ToLower().IndexOf("datapath=") > -1)
                {
                    this.m_DataPath = text.Substring(9);
                    this.timer1 = new System.Timers.Timer(15000.0);
                    this.timer1.Elapsed += new ElapsedEventHandler(this.timer1_Elapsed);
                    this.timer1.Enabled = true;
                }
                else if (text.ToLower().IndexOf("mailstorepath=") > -1)
                {
                    this.m_MailStorePath = text.Substring(14);
                }
            }
            if (this.m_DataPath.Length > 0 && !this.m_DataPath.EndsWith("\\"))
            {
                this.m_DataPath += "\\";
            }
            if (this.m_MailStorePath.Length > 0 && !this.m_MailStorePath.EndsWith("\\"))
            {
                this.m_MailStorePath += "\\";
            }
            if (!Path.IsPathRooted(this.m_DataPath))
            {
                this.m_DataPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + Path.DirectorySeparatorChar + this.m_DataPath;
            }
            if (!Path.IsPathRooted(this.m_MailStorePath))
            {
                this.m_MailStorePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + Path.DirectorySeparatorChar + this.m_MailStorePath;
            }
            this.m_DataPath = PathHelper.PathFix(this.m_DataPath);
            this.m_MailStorePath = PathHelper.PathFix(this.m_MailStorePath);
            if (!Directory.Exists(this.m_DataPath))
            {
                Directory.CreateDirectory(this.m_DataPath);
            }
            RecycleBinManager.RecycleBinPath = this.m_MailStorePath + "RecycleBin/";
            if (!Directory.Exists(this.m_MailStorePath + "RecycleBin/"))
            {
                Directory.CreateDirectory(this.m_MailStorePath + "RecycleBin/");
            }
            this.dsSettings = new DataSet();
            this.dsDomains = new DataSet();
            this.dsUsers = new DataSet();
            this.dsUserAddresses = new DataSet();
            this.dsUserRemoteServers = new DataSet();
            this.dsUserMessageRules = new DataSet();
            this.dsUserMessageRuleActions = new DataSet();
            this.dsUserForwards = new DataSet();
            this.dsGroups = new DataSet();
            this.dsGroupMembers = new DataSet();
            this.dsMailingLists = new DataSet();
            this.dsMailingListAddresses = new DataSet();
            this.dsMailingListACL = new DataSet();
            this.dsRules = new DataSet();
            this.dsRuleActions = new DataSet();
            this.dsRouting = new DataSet();
            this.dsSecurity = new DataSet();
            this.dsFilters = new DataSet();
            this.dsImapACL = new DataSet();
            this.dsSharedFolderRoots = new DataSet();
            this.dsUsersDefaultFolders = new DataSet();
            this.dsRecycleBinSettings = new DataSet();
            this.timer1_Elapsed(this, null);
        }

        public DataView GetDomains()
        {
            this.m_UpdSync.AddMethod();
            DataView result;
            try
            {
                result = new DataView(this.dsDomains.Copy().Tables["Domains"]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public void AddDomain(string domainID, string domainName, string description)
        {
            if (domainID.Length == 0)
            {
                throw new Exception("You must specify domainID");
            }
            ArgsValidator.ValidateDomainName(domainName);
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (this.DomainExists(domainName))
                {
                    throw new Exception("Domain '" + domainName + "' already exists !");
                }
                if (this.ContainsID(this.dsDomains.Tables["Domains"], "DomainID", domainID))
                {
                    throw new Exception("Domain with specified ID '" + domainID + "' already exists !");
                }
                DataRow dataRow = this.dsDomains.Tables["Domains"].NewRow();
                dataRow["DomainID"] = domainID;
                dataRow["DomainName"] = domainName;
                dataRow["Description"] = description;
                this.dsDomains.Tables["Domains"].Rows.Add(dataRow);
                this.dsDomains.WriteXml(this.m_DataPath + "Domains.xml", XmlWriteMode.IgnoreSchema);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void DeleteDomain(string domainID)
        {
            this.m_UpdSync.BeginUpdate();
            try
            {
                string text = "";
                using (DataView dataView = new DataView(this.dsDomains.Tables["Domains"]))
                {
                    dataView.RowFilter = "DomainID='" + domainID + "'";
                    if (dataView.Count <= 0)
                    {
                        throw new Exception("Domain with specified ID '" + domainID + "' doesn't exist !");
                    }
                    text = dataView[0]["DomainName"].ToString();
                }
                foreach (DataRowView dataRowView in this.GetUserAddresses(""))
                {
                    if (dataRowView["Address"].ToString().ToLower().EndsWith("@" + text.ToLower()))
                    {
                        this.DeleteUserAddress(dataRowView["Address"].ToString());
                    }
                }
                using (DataView users = this.GetUsers(text))
                {
                    foreach (DataRowView dataRowView2 in users)
                    {
                        this.DeleteUser(dataRowView2["UserID"].ToString());
                    }
                }
                using (DataView mailingLists = this.GetMailingLists(text))
                {
                    foreach (DataRowView dataRowView3 in mailingLists)
                    {
                        this.DeleteMailingList(dataRowView3["MailingListID"].ToString());
                    }
                }
                using (DataView dataView2 = new DataView(this.dsDomains.Tables["Domains"]))
                {
                    dataView2.RowFilter = "DomainID='" + domainID + "'";
                    if (dataView2.Count > 0)
                    {
                        dataView2[0].Delete();
                    }
                    this.dsDomains.WriteXml(this.m_DataPath + "Domains.xml", XmlWriteMode.IgnoreSchema);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void UpdateDomain(string domainID, string domainName, string description)
        {
            ArgsValidator.ValidateDomainName(domainName);
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (!this.ContainsID(this.dsDomains.Tables["Domains"], "DomainID", domainID))
                {
                    throw new Exception("Invalid domainID, specified domainID '" + domainID + "' doesn't exist !");
                }
                foreach (DataRow dataRow in this.dsDomains.Tables["Domains"].Rows)
                {
                    if (dataRow["DomainID"].ToString().ToLower() == domainID)
                    {
                        if (dataRow["DomainName"].ToString().ToLower() != domainName.ToLower())
                        {
                            if (this.DomainExists(domainName))
                            {
                                throw new Exception("Invalid domainName, specified domainName '" + domainName + "' already exists !");
                            }
                            foreach (DataRow dataRow2 in this.dsUserAddresses.Tables["UserAddresses"].Rows)
                            {
                                string[] array = dataRow2["Address"].ToString().Split(new char[]
                                {
                                    '@'
                                });
                                if (array[1].ToLower() == dataRow["DomainName"].ToString().ToLower())
                                {
                                    dataRow2["Address"] = array[0] + "@" + domainName;
                                }
                            }
                            this.dsUserAddresses.WriteXml(this.m_DataPath + "UserAddresses.xml", XmlWriteMode.IgnoreSchema);
                            foreach (DataRow dataRow3 in this.dsMailingLists.Tables["MailingLists"].Rows)
                            {
                                string[] array2 = dataRow3["MailingListName"].ToString().Split(new char[]
                                {
                                    '@'
                                });
                                if (array2[1].ToLower() == dataRow["DomainName"].ToString().ToLower())
                                {
                                    dataRow3["MailingListName"] = array2[0] + "@" + domainName;
                                }
                            }
                            this.dsMailingListAddresses.WriteXml(this.m_DataPath + "MailingListAddresses.xml", XmlWriteMode.IgnoreSchema);
                        }
                        dataRow["DomainName"] = domainName;
                        dataRow["Description"] = description;
                        this.dsDomains.WriteXml(this.m_DataPath + "Domains.xml", XmlWriteMode.IgnoreSchema);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public bool DomainExists(string source)
        {
            this.m_UpdSync.AddMethod();
            bool result;
            try
            {
                if (source.IndexOf("@") > -1)
                {
                    source = source.Substring(source.IndexOf("@") + 1);
                }
                foreach (DataRow dataRow in this.dsDomains.Tables["Domains"].Rows)
                {
                    if (dataRow["DomainName"].ToString().ToLower() == source.ToLower())
                    {
                        result = true;
                        return result;
                    }
                }
                result = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public DataView GetUsers(string domainName)
        {
            this.m_UpdSync.AddMethod();
            DataView result;
            try
            {
                DataView dataView = new DataView(this.dsUsers.Copy().Tables["Users"]);
                if (domainName != "ALL")
                {
                    dataView.RowFilter = "DomainName='" + domainName + "'";
                }
                result = dataView;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public string GetUserID(string userName)
        {
            foreach (DataRow dataRow in this.dsUsers.Tables["Users"].Rows)
            {
                if (dataRow["UserName"].ToString().ToLower() == userName.ToLower())
                {
                    return dataRow["UserID"].ToString();
                }
            }
            return null;
        }

        public void AddUser(string userID, string userName, string fullName, string password, string description, string domainName, int mailboxSize, bool enabled, UserPermissions permissions)
        {
            if (userID.Length == 0)
            {
                throw new Exception("You must specify userID");
            }
            if (userName.Length == 0)
            {
                throw new Exception("You must specify userName");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (this.UserExists(userName))
                {
                    throw new Exception("User '" + userName + "' already exists !");
                }
                if (this.ContainsID(this.dsUsers.Tables["Users"], "UserID", userID))
                {
                    throw new Exception("User with specified ID '" + userID + "' already exists !");
                }
                DataRow dataRow = this.dsUsers.Tables["Users"].NewRow();
                dataRow["UserID"] = userID;
                dataRow["UserName"] = userName;
                dataRow["DomainName"] = domainName;
                dataRow["FullName"] = fullName;
                dataRow["Password"] = password;
                dataRow["Description"] = description;
                dataRow["Mailbox_Size"] = mailboxSize;
                dataRow["Enabled"] = enabled;
                dataRow["Permissions"] = (int)permissions;
                dataRow["CreationTime"] = DateTime.Now;
                this.dsUsers.Tables["Users"].Rows.Add(dataRow);
                this.dsUsers.WriteXml(this.m_DataPath + "Users.xml", XmlWriteMode.IgnoreSchema);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void DeleteUser(string userID)
        {
            this.m_UpdSync.BeginUpdate();
            try
            {
                string userName = "";
                using (DataView dataView = new DataView(this.dsUsers.Tables["Users"]))
                {
                    dataView.RowFilter = "UserID='" + userID + "'";
                    if (dataView.Count <= 0)
                    {
                        throw new Exception("User with specified ID '" + userID + "' doesn't exist !");
                    }
                    userName = dataView[0]["UserName"].ToString();
                }
                using (DataView userAddresses = this.GetUserAddresses(userName))
                {
                    foreach (DataRowView dataRowView in userAddresses)
                    {
                        this.DeleteUserAddress(dataRowView["AddressID"].ToString());
                    }
                }
                using (DataView userRemoteServers = this.GetUserRemoteServers(userName))
                {
                    foreach (DataRowView dataRowView2 in userRemoteServers)
                    {
                        this.DeleteUserRemoteServer(dataRowView2["ServerID"].ToString());
                    }
                }
                using (DataView userMessageRules = this.GetUserMessageRules(userName))
                {
                    foreach (DataRowView dataRowView3 in userMessageRules)
                    {
                        this.DeleteUserMessageRule(userID, dataRowView3["RuleID"].ToString());
                    }
                }
                using (DataView dataView2 = new DataView(this.dsUsers.Tables["Users"]))
                {
                    dataView2.RowFilter = "UserID='" + userID + "'";
                    if (dataView2.Count > 0)
                    {
                        this.dsUsers.Tables["Users"].Rows.Remove(dataView2[0].Row);
                    }
                    this.dsUsers.WriteXml(this.m_DataPath + "Users.xml", XmlWriteMode.IgnoreSchema);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void UpdateUser(string userID, string userName, string fullName, string password, string description, string domainName, int mailboxSize, bool enabled, UserPermissions permissions)
        {
            if (userName.Length == 0)
            {
                throw new Exception("You must specify userName");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                try
                {
                    if (this.ContainsID(this.dsUsers.Tables["Users"], "UserID", userID))
                    {
                        using (DataView dataView = new DataView(this.dsUsers.Tables["Users"]))
                        {
                            dataView.RowFilter = "UserID='" + userID + "'";
                            using (DataView dataView2 = new DataView(this.dsUsers.Tables["Users"]))
                            {
                                dataView2.RowFilter = "UserName='" + userName + "'";
                                if (dataView2.Count > 0 && dataView2[0]["UserID"].ToString() != userID)
                                {
                                    throw new Exception("User '" + userName + "' already exists !");
                                }
                            }
                            if (dataView.Count > 0)
                            {
                                if (dataView[0]["UserName"].ToString().ToLower() != userName.ToLower())
                                {
                                    string text = PathHelper.DirectoryExists(PathHelper.PathFix(this.m_MailStorePath + "Mailboxes\\" + dataView[0]["UserName"]));
                                    if (text != null)
                                    {
                                        Directory.Move(text, PathHelper.PathFix(this.m_MailStorePath + "Mailboxes\\" + userName));
                                    }
                                }
                                dataView[0]["UserName"] = userName;
                                dataView[0]["FullName"] = fullName;
                                dataView[0]["Password"] = password;
                                dataView[0]["Description"] = description;
                                dataView[0]["DomainName"] = domainName;
                                dataView[0]["Mailbox_Size"] = mailboxSize;
                                dataView[0]["Enabled"] = enabled;
                                dataView[0]["Permissions"] = (int)permissions;
                                this.dsUsers.WriteXml(this.m_DataPath + "Users.xml", XmlWriteMode.IgnoreSchema);
                            }
                            goto IL_251;
                        }
                        goto IL_23B;
                        IL_251:
                        goto IL_256;
                    }
                    IL_23B:
                    throw new Exception("User with specified ID '" + userID + "' doesn't exist !");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                IL_256:;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void AddUserAddress(string userName, string emailAddress)
        {
            if (userName.Length == 0)
            {
                throw new Exception("You must specify userName");
            }
            if (emailAddress.Length == 0)
            {
                throw new Exception("You must specify address");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (this.MapUser(emailAddress) != null)
                {
                    throw new Exception("Address '" + emailAddress + "' already exists !");
                }
                string value = "";
                using (DataView dataView = new DataView(this.dsUsers.Tables["Users"]))
                {
                    dataView.RowFilter = "UserName='" + userName + "'";
                    value = dataView[0]["UserID"].ToString();
                }
                DataRow dataRow = this.dsUserAddresses.Tables["UserAddresses"].NewRow();
                dataRow["UserID"] = value;
                dataRow["Address"] = emailAddress;
                this.dsUserAddresses.Tables["UserAddresses"].Rows.Add(dataRow);
                this.dsUserAddresses.WriteXml(this.m_DataPath + "UserAddresses.xml", XmlWriteMode.IgnoreSchema);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void DeleteUserAddress(string emailAddress)
        {
            this.m_UpdSync.BeginUpdate();
            try
            {
                foreach (DataRow dataRow in this.dsUserAddresses.Tables["UserAddresses"].Rows)
                {
                    if (dataRow["Address"].ToString().ToLower() == emailAddress.ToLower())
                    {
                        dataRow.Delete();
                        this.dsUserAddresses.WriteXml(this.m_DataPath + "UserAddresses.xml", XmlWriteMode.IgnoreSchema);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public DataView GetUserAddresses(string userName)
        {
            this.m_UpdSync.AddMethod();
            DataView result;
            try
            {
                string text = "";
                foreach (DataRow dataRow in this.dsUsers.Tables["Users"].Rows)
                {
                    if (userName.ToLower() == dataRow["UserName"].ToString().ToLower())
                    {
                        text = dataRow["UserID"].ToString();
                        break;
                    }
                }
                DataView dataView = new DataView(this.dsUserAddresses.Copy().Tables["UserAddresses"]);
                if (text != "")
                {
                    dataView.RowFilter = "UserID='" + text + "'";
                }
                result = dataView;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public bool UserExists(string userName)
        {
            this.m_UpdSync.AddMethod();
            bool result;
            try
            {
                foreach (DataRow dataRow in this.dsUsers.Tables["Users"].Rows)
                {
                    if (dataRow["UserName"].ToString().ToLower() == userName.ToLower())
                    {
                        result = true;
                        return result;
                    }
                }
                result = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public string MapUser(string emailAddress)
        {
            this.m_UpdSync.AddMethod();
            string result;
            try
            {
                foreach (DataRow dataRow in this.dsUserAddresses.Tables["UserAddresses"].Rows)
                {
                    if (dataRow["Address"].ToString().ToLower() == emailAddress.ToLower())
                    {
                        string a = dataRow["UserID"].ToString();
                        foreach (DataRow dataRow2 in this.dsUsers.Tables["Users"].Rows)
                        {
                            if (a == dataRow2["UserID"].ToString())
                            {
                                result = dataRow2["UserName"].ToString();
                                return result;
                            }
                        }
                    }
                }
                result = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public bool ValidateMailboxSize(string userName)
        {
            this.m_UpdSync.AddMethod();
            bool result;
            try
            {
                foreach (DataRow dataRow in this.dsUsers.Tables["Users"].Rows)
                {
                    if (dataRow["UserName"].ToString().ToLower() == userName.ToLower())
                    {
                        long num = Convert.ToInt64(dataRow["Mailbox_Size"]);
                        if (num < 1L)
                        {
                            result = false;
                            return result;
                        }
                        long mailboxSize = this.GetMailboxSize(userName);
                        if (mailboxSize > num * 1000000L)
                        {
                            result = true;
                            return result;
                        }
                    }
                }
                result = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public UserPermissions GetUserPermissions(string userName)
        {
            foreach (DataRow dataRow in this.dsUsers.Tables["Users"].Rows)
            {
                if (dataRow["UserName"].ToString().ToLower() == userName.ToLower())
                {
                    return (UserPermissions)Convert.ToInt32(dataRow["Permissions"]);
                }
            }
            return UserPermissions.None;
        }

        public DateTime GetUserLastLoginTime(string userName)
        {
            string text = PathHelper.DirectoryExists(this.m_MailStorePath + "Mailboxes/" + userName);
            if (text != null)
            {
                using (FileStream fileStream = this.OpenOrCreateFile(text + "/_LastLogin.txt", 10000))
                {
                    if (fileStream.Length == 0L)
                    {
                        DateTime now = DateTime.Now;
                        byte[] bytes = Encoding.UTF8.GetBytes(now.ToString("yyyyMMdd HH:mm:ss", DateTimeFormatInfo.InvariantInfo));
                        fileStream.Write(bytes, 0, bytes.Length);
                        DateTime result = now;
                        return result;
                    }
                    try
                    {
                        byte[] array = new byte[fileStream.Length];
                        fileStream.Read(array, 0, array.Length);
                        DateTime result = DateTime.ParseExact(Encoding.UTF8.GetString(array), "yyyyMMdd HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
                        return result;
                    }
                    catch
                    {
                        DateTime result = DateTime.Now;
                        return result;
                    }
                }
            }
            if (!this.UserExists(userName))
            {
                throw new Exception("User '" + userName + "' doesn't exist !");
            }
            return DateTime.MinValue;
        }

        public void UpdateUserLastLoginTime(string userName)
        {
            if (!this.UserExists(userName))
            {
                throw new Exception("User '" + userName + "' doesn't exist !");
            }
            string str = PathHelper.EnsureFolder(this.m_MailStorePath + "Mailboxes/" + userName);
            using (FileStream fileStream = this.OpenOrCreateFile(str + "/_LastLogin.txt", 10000))
            {
                fileStream.SetLength(0L);
                byte[] bytes = Encoding.UTF8.GetBytes(DateTime.Now.ToString("yyyyMMdd HH:mm:ss", DateTimeFormatInfo.InvariantInfo));
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }

        public DataView GetUserRemoteServers(string userName)
        {
            this.m_UpdSync.AddMethod();
            DataView result;
            try
            {
                string text = "";
                foreach (DataRow dataRow in this.dsUsers.Tables["Users"].Rows)
                {
                    if (userName.ToLower() == dataRow["UserName"].ToString().ToLower())
                    {
                        text = dataRow["UserID"].ToString();
                        break;
                    }
                }
                DataView dataView = new DataView(this.dsUserRemoteServers.Copy().Tables["UserRemoteServers"]);
                if (text.Length > 0)
                {
                    dataView.RowFilter = "UserID='" + text + "'";
                }
                result = dataView;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public void AddUserRemoteServer(string serverID, string userName, string description, string remoteServer, int remotePort, string remoteUser, string remotePassword, bool useSSL, bool enabled)
        {
            if (serverID.Length == 0)
            {
                throw new Exception("You must specify serverID");
            }
            if (userName.Length == 0)
            {
                throw new Exception("You must specify userName");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (!this.UserExists(userName))
                {
                    throw new Exception("User '" + userName + "' doesn't exist !");
                }
                if (this.ContainsID(this.dsUserRemoteServers.Tables["UserRemoteServers"], "ServerID", serverID))
                {
                    throw new Exception("Address with specified ID '" + serverID + "' already exists !");
                }
                string value = "";
                using (DataView dataView = new DataView(this.dsUsers.Tables["Users"]))
                {
                    dataView.RowFilter = "UserName='" + userName + "'";
                    value = dataView[0]["UserID"].ToString();
                }
                DataRow dataRow = this.dsUserRemoteServers.Tables["UserRemoteServers"].NewRow();
                dataRow["ServerID"] = serverID;
                dataRow["UserID"] = value;
                dataRow["Description"] = description;
                dataRow["RemoteServer"] = remoteServer;
                dataRow["RemotePort"] = remotePort;
                dataRow["RemoteUserName"] = remoteUser;
                dataRow["RemotePassword"] = remotePassword;
                dataRow["UseSSL"] = useSSL;
                dataRow["Enabled"] = enabled;
                this.dsUserRemoteServers.Tables["UserRemoteServers"].Rows.Add(dataRow);
                this.dsUserRemoteServers.WriteXml(this.m_DataPath + "UserRemoteServers.xml", XmlWriteMode.IgnoreSchema);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void DeleteUserRemoteServer(string serverID)
        {
            this.m_UpdSync.BeginUpdate();
            try
            {
                using (DataView dataView = new DataView(this.dsUserRemoteServers.Tables["UserRemoteServers"]))
                {
                    dataView.RowFilter = "ServerID='" + serverID + "'";
                    if (dataView.Count > 0)
                    {
                        dataView[0].Delete();
                    }
                    this.dsUserRemoteServers.WriteXml(this.m_DataPath + "UserRemoteServers.xml", XmlWriteMode.IgnoreSchema);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void UpdateUserRemoteServer(string serverID, string userName, string description, string remoteServer, int remotePort, string remoteUser, string remotePassword, bool useSSL, bool enabled)
        {
            if (serverID.Length == 0)
            {
                throw new Exception("You must specify serverID");
            }
            if (userName.Length == 0)
            {
                throw new Exception("You must specify userName");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                try
                {
                    if (this.UserExists(userName))
                    {
                        if (!this.ContainsID(this.dsUserRemoteServers.Tables["UserRemoteServers"], "ServerID", serverID))
                        {
                            throw new Exception("Address with specified ID '" + serverID + "' doesn't exist !");
                        }
                        IEnumerator enumerator = this.dsUserRemoteServers.Tables["UserRemoteServers"].Rows.GetEnumerator();
                        try
                        {
                            while (enumerator.MoveNext())
                            {
                                DataRow dataRow = (DataRow)enumerator.Current;
                                if (dataRow["ServerID"].ToString().ToLower() == serverID)
                                {
                                    dataRow["Description"] = description;
                                    dataRow["RemoteServer"] = remoteServer;
                                    dataRow["RemotePort"] = remotePort;
                                    dataRow["RemoteUserName"] = remoteUser;
                                    dataRow["RemotePassword"] = remotePassword;
                                    dataRow["UseSSL"] = useSSL;
                                    dataRow["Enabled"] = enabled;
                                    this.dsUserRemoteServers.WriteXml(this.m_DataPath + "UserRemoteServers.xml", XmlWriteMode.IgnoreSchema);
                                    break;
                                }
                            }
                            goto IL_182;
                        }
                        finally
                        {
                            IDisposable disposable = enumerator as IDisposable;
                            if (disposable != null)
                            {
                                disposable.Dispose();
                            }
                        }
                        goto IL_16C;
                        IL_182:
                        goto IL_187;
                    }
                    IL_16C:
                    throw new Exception("User '" + userName + "' doesn't exist !");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                IL_187:;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public DataView GetUserMessageRules(string userName)
        {
            this.m_UpdSync.AddMethod();
            DataView result;
            try
            {
                string str = "";
                foreach (DataRow dataRow in this.dsUsers.Tables["Users"].Rows)
                {
                    if (userName.ToLower() == dataRow["UserName"].ToString().ToLower())
                    {
                        str = dataRow["UserID"].ToString();
                        break;
                    }
                }
                DataView dataView = new DataView(this.dsUserMessageRules.Copy().Tables["UserMessageRules"]);
                if (userName.Length > 0)
                {
                    dataView.RowFilter = "UserID='" + str + "'";
                }
                dataView.Sort = "Cost ASC";
                result = dataView;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public void AddUserMessageRule(string userID, string ruleID, long cost, bool enabled, GlobalMessageRule_CheckNextRule checkNextRule, string description, string matchExpression)
        {
            if (userID == null || userID == "")
            {
                throw new Exception("Invalid userID value, userID can't be '' or null !");
            }
            if (ruleID == null || ruleID == "")
            {
                throw new Exception("Invalid ruleID value, ruleID can't be '' or null !");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (!this.ContainsID(this.dsUsers.Tables["Users"], "UserID", userID))
                {
                    throw new Exception("User with specified id '" + userID + "' doesn't exist !");
                }
                if (this.ContainsID(this.dsUserMessageRules.Tables["UserMessageRules"], "RuleID", ruleID))
                {
                    throw new Exception("Specified ruleID '" + ruleID + "' already exists, choose another ruleID !");
                }
                DataRow dataRow = this.dsUserMessageRules.Tables["UserMessageRules"].NewRow();
                dataRow["UserID"] = userID;
                dataRow["RuleID"] = ruleID;
                dataRow["Cost"] = cost;
                dataRow["Enabled"] = enabled;
                dataRow["CheckNextRuleIf"] = checkNextRule;
                dataRow["Description"] = description;
                dataRow["MatchExpression"] = matchExpression;
                this.dsUserMessageRules.Tables["UserMessageRules"].Rows.Add(dataRow);
                this.dsUserMessageRules.WriteXml(this.m_DataPath + "UserMessageRules.xml", XmlWriteMode.IgnoreSchema);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void DeleteUserMessageRule(string userID, string ruleID)
        {
            if (userID == null || userID == "")
            {
                throw new Exception("Invalid userID value, userID can't be '' or null !");
            }
            if (ruleID == null || ruleID == "")
            {
                throw new Exception("Invalid ruleID value, ruleID can't be '' or null !");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (!this.ContainsID(this.dsUsers.Tables["Users"], "UserID", userID))
                {
                    throw new Exception("User with specified id '" + userID + "' doesn't exist !");
                }
                if (!this.ContainsID(this.dsUserMessageRules.Tables["UserMessageRules"], "RuleID", ruleID))
                {
                    throw new Exception("Invalid ruleID '" + ruleID + "', specified ruleID doesn't exist !");
                }
                foreach (DataRowView dataRowView in this.GetUserMessageRuleActions(userID, ruleID))
                {
                    this.DeleteUserMessageRuleAction(userID, ruleID, dataRowView["ActionID"].ToString());
                }
                foreach (DataRow dataRow in this.dsUserMessageRules.Tables["UserMessageRules"].Rows)
                {
                    if (dataRow["RuleID"].ToString().ToLower() == ruleID)
                    {
                        dataRow.Delete();
                        this.dsUserMessageRules.WriteXml(this.m_DataPath + "UserMessageRules.xml", XmlWriteMode.IgnoreSchema);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void UpdateUserMessageRule(string userID, string ruleID, long cost, bool enabled, GlobalMessageRule_CheckNextRule checkNextRule, string description, string matchExpression)
        {
            if (userID == null || userID == "")
            {
                throw new Exception("Invalid userID value, userID can't be '' or null !");
            }
            if (ruleID == null || ruleID == "")
            {
                throw new Exception("Invalid ruleID value, ruleID can't be '' or null !");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (!this.ContainsID(this.dsUsers.Tables["Users"], "UserID", userID))
                {
                    throw new Exception("User with specified id '" + userID + "' doesn't exist !");
                }
                if (!this.ContainsID(this.dsUserMessageRules.Tables["UserMessageRules"], "RuleID", ruleID))
                {
                    throw new Exception("Invalid ruleID '" + ruleID + "', specified ruleID doesn't exist !");
                }
                foreach (DataRow dataRow in this.dsUserMessageRules.Tables["UserMessageRules"].Rows)
                {
                    if (dataRow["RuleID"].ToString().ToLower() == ruleID)
                    {
                        dataRow["UserID"] = userID;
                        dataRow["RuleID"] = ruleID;
                        dataRow["Cost"] = cost;
                        dataRow["Enabled"] = enabled;
                        dataRow["CheckNextRuleIf"] = checkNextRule;
                        dataRow["Description"] = description;
                        dataRow["MatchExpression"] = matchExpression;
                        this.dsUserMessageRules.WriteXml(this.m_DataPath + "UserMessageRules.xml", XmlWriteMode.IgnoreSchema);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public DataView GetUserMessageRuleActions(string userID, string ruleID)
        {
            this.m_UpdSync.AddMethod();
            DataView result;
            try
            {
                DataView dataView = new DataView(this.dsUserMessageRuleActions.Copy().Tables["UserMessageRuleActions"]);
                if (userID.Length > 0)
                {
                    dataView.RowFilter = string.Concat(new string[]
                    {
                        "UserID='",
                        userID,
                        "' AND RuleID='",
                        ruleID,
                        "'"
                    });
                }
                result = dataView;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public void AddUserMessageRuleAction(string userID, string ruleID, string actionID, string description, GlobalMessageRuleActionType actionType, byte[] actionData)
        {
            if (userID == null || userID == "")
            {
                throw new Exception("Invalid userID value, userID can't be '' or null !");
            }
            if (ruleID == null || ruleID == "")
            {
                throw new Exception("Invalid ruleID value, ruleID can't be '' or null !");
            }
            if (actionID == null || actionID == "")
            {
                throw new Exception("Invalid actionID value, actionID can't be '' or null !");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (!this.ContainsID(this.dsUsers.Tables["Users"], "UserID", userID))
                {
                    throw new Exception("User with specified id '" + userID + "' doesn't exist !");
                }
                if (!this.ContainsID(this.dsUserMessageRules.Tables["UserMessageRules"], "RuleID", ruleID))
                {
                    throw new Exception("Invalid ruleID '" + ruleID + "', specified ruleID doesn't exist !");
                }
                if (this.ContainsID(this.dsUserMessageRuleActions.Tables["UserMessageRuleActions"], "ActionID", ruleID))
                {
                    throw new Exception("Specified actionID '" + actionID + "' already exists, choose another actionID !");
                }
                DataRow dataRow = this.dsUserMessageRuleActions.Tables["UserMessageRuleActions"].NewRow();
                dataRow["UserID"] = userID;
                dataRow["RuleID"] = ruleID;
                dataRow["ActionID"] = actionID;
                dataRow["Description"] = description;
                dataRow["ActionType"] = actionType;
                dataRow["ActionData"] = actionData;
                this.dsUserMessageRuleActions.Tables["UserMessageRuleActions"].Rows.Add(dataRow);
                this.dsUserMessageRuleActions.WriteXml(this.m_DataPath + "UserMessageRuleActions.xml", XmlWriteMode.IgnoreSchema);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void DeleteUserMessageRuleAction(string userID, string ruleID, string actionID)
        {
            if (userID == null || userID == "")
            {
                throw new Exception("Invalid userID value, userID can't be '' or null !");
            }
            if (ruleID == null || ruleID == "")
            {
                throw new Exception("Invalid ruleID value, ruleID can't be '' or null !");
            }
            if (actionID == null || actionID == "")
            {
                throw new Exception("Invalid actionID value, actionID can't be '' or null !");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (!this.ContainsID(this.dsUsers.Tables["Users"], "UserID", userID))
                {
                    throw new Exception("User with specified id '" + userID + "' doesn't exist !");
                }
                if (!this.ContainsID(this.dsUserMessageRules.Tables["UserMessageRules"], "RuleID", ruleID))
                {
                    throw new Exception("Invalid ruleID '" + ruleID + "', specified ruleID doesn't exist !");
                }
                foreach (DataRow dataRow in this.dsUserMessageRuleActions.Tables["UserMessageRuleActions"].Rows)
                {
                    if (dataRow["RuleID"].ToString().ToLower() == ruleID && dataRow["ActionID"].ToString().ToLower() == actionID)
                    {
                        dataRow.Delete();
                        this.dsUserMessageRuleActions.WriteXml(this.m_DataPath + "UserMessageRuleActions.xml", XmlWriteMode.IgnoreSchema);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void UpdateUserMessageRuleAction(string userID, string ruleID, string actionID, string description, GlobalMessageRuleActionType actionType, byte[] actionData)
        {
            if (userID == null || userID == "")
            {
                throw new Exception("Invalid userID value, userID can't be '' or null !");
            }
            if (ruleID == null || ruleID == "")
            {
                throw new Exception("Invalid ruleID value, ruleID can't be '' or null !");
            }
            if (actionID == null || actionID == "")
            {
                throw new Exception("Invalid actionID value, actionID can't be '' or null !");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (!this.ContainsID(this.dsUsers.Tables["Users"], "UserID", userID))
                {
                    throw new Exception("User with specified id '" + userID + "' doesn't exist !");
                }
                if (!this.ContainsID(this.dsUserMessageRules.Tables["UserMessageRules"], "RuleID", ruleID))
                {
                    throw new Exception("Invalid ruleID '" + ruleID + "', specified ruleID doesn't exist !");
                }
                bool flag = false;
                foreach (DataRow dataRow in this.dsUserMessageRuleActions.Tables["UserMessageRuleActions"].Rows)
                {
                    if (dataRow["RuleID"].ToString().ToLower() == ruleID && dataRow["ActionID"].ToString().ToLower() == actionID)
                    {
                        dataRow["RuleID"] = ruleID;
                        dataRow["ActionID"] = actionID;
                        dataRow["Description"] = description;
                        dataRow["ActionType"] = actionType;
                        dataRow["ActionData"] = actionData;
                        this.dsUserMessageRuleActions.WriteXml(this.m_DataPath + "UserMessageRuleActions.xml", XmlWriteMode.IgnoreSchema);
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    throw new Exception("Invalid actionID '" + actionID + "', specified actionID doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public DataSet AuthUser(string userName, string passwData, string authData, AuthType authType)
        {
            this.m_UpdSync.AddMethod();
            DataSet dataSet = new DataSet();
            DataTable dataTable = dataSet.Tables.Add("Result");
            dataTable.Columns.Add("Result");
            dataTable.Columns.Add("ReturnData");
            DataRow dataRow = dataTable.NewRow();
            dataRow["Result"] = "false";
            dataRow["ReturnData"] = "";
            dataTable.Rows.Add(dataRow);
            DataSet result;
            try
            {
                foreach (DataRow dataRow2 in this.dsUsers.Tables["Users"].Rows)
                {
                    if (Convert.ToBoolean(dataRow2["Enabled"]) && dataRow2["USERNAME"].ToString().ToLower() == userName.ToLower())
                    {
                        string text = dataRow2["PASSWORD"].ToString().ToLower();
                        if (authType != AuthType.Plain)
                        {
                            if (authType == AuthType.DIGEST_MD5)
                            {
                                Auth_HttpDigest auth_HttpDigest = new Auth_HttpDigest(authData, "AUTHENTICATE");
                                if (auth_HttpDigest.Authenticate(userName, text))
                                {
                                    dataRow["Result"] = "true";
                                    dataRow["ReturnData"] = auth_HttpDigest.CalculateResponse(userName, text);
                                    result = dataSet;
                                    return result;
                                }
                            }
                        }
                        else if (text == passwData.ToLower())
                        {
                            dataRow["Result"] = "true";
                            result = dataSet;
                            return result;
                        }
                        result = dataSet;
                        return result;
                    }
                }
                result = dataSet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public bool GroupExists(string groupName)
        {
            ArgsValidator.ValidateUserName(groupName);
            return this.GetGroupID(groupName) != null;
        }

        public DataView GetGroups()
        {
            this.m_UpdSync.AddMethod();
            DataView result;
            try
            {
                DataView dataView = new DataView(this.dsGroups.Copy().Tables["Groups"]);
                result = dataView;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public void AddGroup(string groupID, string groupName, string description, bool enabled)
        {
            if (groupID == null || groupID == "")
            {
                throw new Exception("Invalid groupID value, groupID can't be '' or null !");
            }
            ArgsValidator.ValidateUserName(groupName);
            ArgsValidator.ValidateNotNull(description);
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (this.ContainsID(this.dsGroups.Tables["Groups"], "GroupID", groupID))
                {
                    throw new Exception("Invalid group ID, specified group ID '" + groupID + "' already exists !");
                }
                if (this.GroupExists(groupName))
                {
                    throw new Exception("Invalid group name, specified group '" + groupName + "' already exists !");
                }
                if (this.UserExists(groupName))
                {
                    throw new Exception("Invalid group name, user with specified name '" + groupName + "' already exists !");
                }
                DataRow dataRow = this.dsGroups.Tables["Groups"].NewRow();
                dataRow["GroupID"] = groupID;
                dataRow["GroupName"] = groupName;
                dataRow["Description"] = description;
                dataRow["Enabled"] = enabled;
                this.dsGroups.Tables["Groups"].Rows.Add(dataRow);
                this.dsGroups.WriteXml(this.m_DataPath + "Groups.xml", XmlWriteMode.IgnoreSchema);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void DeleteGroup(string groupID)
        {
            if (groupID == null || groupID == "")
            {
                throw new Exception("Invalid groupID value, groupID can't be '' or null !");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (!this.ContainsID(this.dsGroups.Tables["Groups"], "GroupID", groupID))
                {
                    throw new Exception("Invalid group ID, specified group ID '" + groupID + "' doesn't exist !");
                }
                for (int i = 0; i < this.dsGroupMembers.Tables["GroupMembers"].Rows.Count; i++)
                {
                    DataRow dataRow = this.dsGroupMembers.Tables["GroupMembers"].Rows[i];
                    if (dataRow["GroupID"].ToString() == groupID)
                    {
                        dataRow.Delete();
                        i--;
                    }
                }
                this.dsGroupMembers.WriteXml(this.m_DataPath + "GroupMembers.xml", XmlWriteMode.IgnoreSchema);
                foreach (DataRow dataRow2 in this.dsGroups.Tables["Groups"].Rows)
                {
                    if (dataRow2["GroupID"].ToString().ToLower() == groupID)
                    {
                        dataRow2.Delete();
                        this.dsGroups.WriteXml(this.m_DataPath + "Groups.xml", XmlWriteMode.IgnoreSchema);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void UpdateGroup(string groupID, string groupName, string description, bool enabled)
        {
            if (groupID == null || groupID == "")
            {
                throw new Exception("Invalid groupID value, groupID can't be '' or null !");
            }
            ArgsValidator.ValidateUserName(groupName);
            ArgsValidator.ValidateNotNull(description);
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (!this.ContainsID(this.dsGroups.Tables["Groups"], "GroupID", groupID))
                {
                    throw new Exception("Invalid group ID, specified group ID '" + groupID + "' doesn't exist !");
                }
                foreach (DataRow dataRow in this.dsGroups.Tables["Groups"].Rows)
                {
                    if (dataRow["GroupID"].ToString().ToLower() == groupID)
                    {
                        if (dataRow["GroupName"].ToString().ToLower() != groupName.ToLower())
                        {
                            if (this.GroupExists(groupName))
                            {
                                throw new Exception("Invalid group name, specified group '" + groupName + "' already exists !");
                            }
                            if (this.UserExists(groupName))
                            {
                                throw new Exception("Invalid group name, user with specified name '" + groupName + "' already exists !");
                            }
                        }
                        dataRow["GroupName"] = groupName;
                        dataRow["Description"] = description;
                        dataRow["Enabled"] = enabled;
                        this.dsGroups.WriteXml(this.m_DataPath + "Groups.xml", XmlWriteMode.IgnoreSchema);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public bool GroupMemberExists(string groupName, string userOrGroup)
        {
            ArgsValidator.ValidateUserName(groupName);
            ArgsValidator.ValidateUserName(userOrGroup);
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (!this.GroupExists(groupName))
                {
                    throw new Exception("Invalid group name, specified group '" + groupName + "' doesn't exist !");
                }
                string groupID = this.GetGroupID(groupName);
                foreach (DataRow dataRow in this.dsGroupMembers.Tables["GroupMembers"].Rows)
                {
                    if (dataRow["GroupID"].ToString().ToLower() == groupID && dataRow["UserOrGroup"].ToString().ToLower() == userOrGroup.ToLower())
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
            return false;
        }

        public string[] GetGroupMembers(string groupName)
        {
            if (groupName == null || groupName == "")
            {
                throw new Exception("Invalid groupName value, groupName can't be '' or null !");
            }
            this.m_UpdSync.AddMethod();
            string[] result;
            try
            {
                if (!this.GroupExists(groupName))
                {
                    throw new Exception("Invalid group name, specified group name '" + groupName + "' doesn't exist !");
                }
                string groupID = this.GetGroupID(groupName);
                DataTable arg_62_0 = this.dsGroupMembers.Tables["GroupMembers"];
                List<string> list = new List<string>();
                foreach (DataRow dataRow in this.dsGroupMembers.Tables["GroupMembers"].Rows)
                {
                    if (dataRow["GroupID"].ToString() == groupID)
                    {
                        list.Add(dataRow["UserOrGroup"].ToString());
                    }
                }
                result = list.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public void AddGroupMember(string groupName, string userOrGroup)
        {
            ArgsValidator.ValidateUserName(groupName);
            ArgsValidator.ValidateUserName(userOrGroup);
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (!this.GroupExists(groupName))
                {
                    throw new Exception("Invalid group name, specified group '" + groupName + "' doesn't exist !");
                }
                if (groupName.ToLower() == userOrGroup.ToLower())
                {
                    throw new Exception("Invalid group member, can't add goup itself as same group member !");
                }
                if (this.GroupMemberExists(groupName, userOrGroup))
                {
                    throw new Exception("Invalid group member, specified group member '" + userOrGroup + "' already exists !");
                }
                string groupID = this.GetGroupID(groupName);
                DataRow dataRow = this.dsGroupMembers.Tables["GroupMembers"].NewRow();
                dataRow["GroupID"] = groupID;
                dataRow["UserOrGroup"] = userOrGroup;
                this.dsGroupMembers.Tables["GroupMembers"].Rows.Add(dataRow);
                this.dsGroupMembers.WriteXml(this.m_DataPath + "GroupMembers.xml", XmlWriteMode.IgnoreSchema);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void DeleteGroupMember(string groupName, string userOrGroup)
        {
            ArgsValidator.ValidateUserName(groupName);
            ArgsValidator.ValidateUserName(userOrGroup);
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (!this.GroupExists(groupName))
                {
                    throw new Exception("Invalid group name, specified group '" + groupName + "' doesn't exist !");
                }
                if (!this.GroupMemberExists(groupName, userOrGroup))
                {
                    throw new Exception("Invalid group member, specified group member '" + groupName + "' already exists !");
                }
                string groupID = this.GetGroupID(groupName);
                foreach (DataRow dataRow in this.dsGroupMembers.Tables["GroupMembers"].Rows)
                {
                    if (dataRow["GroupID"].ToString().ToLower() == groupID && dataRow["UserOrGroup"].ToString().ToLower() == userOrGroup.ToLower())
                    {
                        dataRow.Delete();
                        this.dsGroupMembers.WriteXml(this.m_DataPath + "GroupMembers.xml", XmlWriteMode.IgnoreSchema);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public string[] GetGroupUsers(string groupName)
        {
            List<string> list = new List<string>();
            List<string> list2 = new List<string>();
            Queue<string> queue = new Queue<string>();
            string[] groupMembers = this.GetGroupMembers(groupName);
            string[] array = groupMembers;
            for (int i = 0; i < array.Length; i++)
            {
                string item = array[i];
                queue.Enqueue(item);
            }
            while (queue.Count > 0)
            {
                string text = queue.Dequeue();
                DataRow group = this.GetGroup(text);
                if (group != null)
                {
                    if (!list2.Contains(text.ToLower()))
                    {
                        if (Convert.ToBoolean(group["Enabled"]))
                        {
                            groupMembers = this.GetGroupMembers(text);
                            string[] array2 = groupMembers;
                            for (int j = 0; j < array2.Length; j++)
                            {
                                string item2 = array2[j];
                                queue.Enqueue(item2);
                            }
                        }
                        list2.Add(text.ToLower());
                    }
                }
                else if (!list.Contains(text))
                {
                    list.Add(text);
                }
            }
            return list.ToArray();
        }

        public string GetGroupID(string groupName)
        {
            foreach (DataRow dataRow in this.dsGroups.Tables["Groups"].Rows)
            {
                if (dataRow["GroupName"].ToString().ToLower() == groupName.ToLower())
                {
                    return dataRow["GroupID"].ToString();
                }
            }
            return null;
        }

        public DataView GetMailingLists(string domainName)
        {
            this.m_UpdSync.AddMethod();
            DataView result;
            try
            {
                DataView dataView = new DataView(this.dsMailingLists.Copy().Tables["MailingLists"]);
                if (domainName != "ALL")
                {
                    dataView.RowFilter = "DomainName='" + domainName + "'";
                }
                result = dataView;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public void AddMailingList(string mailingListID, string mailingListName, string description, string domainName, bool enabled)
        {
            if (mailingListID.Length == 0)
            {
                throw new Exception("You must specify mailingListID");
            }
            if (mailingListName.Length == 0)
            {
                throw new Exception("You must specify mailingListName");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (this.MailingListExists(mailingListName))
                {
                    throw new Exception("Mailing list '" + mailingListName + "' already exists !");
                }
                if (this.ContainsID(this.dsMailingLists.Tables["MailingLists"], "MailingListID", mailingListID))
                {
                    throw new Exception("Mailing list with specified ID '" + mailingListID + "' already exists !");
                }
                DataRow dataRow = this.dsMailingLists.Tables["MailingLists"].NewRow();
                dataRow["MailingListID"] = mailingListID;
                dataRow["MailingListName"] = mailingListName;
                dataRow["Description"] = description;
                dataRow["DomainName"] = domainName;
                dataRow["Enabled"] = enabled;
                this.dsMailingLists.Tables["MailingLists"].Rows.Add(dataRow);
                this.dsMailingLists.WriteXml(this.m_DataPath + "MailingLists.xml", XmlWriteMode.IgnoreSchema);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void DeleteMailingList(string mailingListID)
        {
            this.m_UpdSync.BeginUpdate();
            try
            {
                string mailingListName = "";
                using (DataView dataView = new DataView(this.dsMailingLists.Tables["MailingLists"]))
                {
                    dataView.RowFilter = "MailingListID='" + mailingListID + "'";
                    if (dataView.Count <= 0)
                    {
                        throw new Exception("Mailing list with specified ID '" + mailingListID + "' doesn't exist !");
                    }
                    mailingListName = dataView[0]["MailingListName"].ToString();
                }
                using (DataView mailingListAddresses = this.GetMailingListAddresses(mailingListName))
                {
                    foreach (DataRowView dataRowView in mailingListAddresses)
                    {
                        this.DeleteMailingListAddress(dataRowView["AddressID"].ToString());
                    }
                }
                using (DataView dataView2 = new DataView(this.dsMailingLists.Tables["MailingLists"]))
                {
                    dataView2.RowFilter = "MailingListID='" + mailingListID + "'";
                    if (dataView2.Count > 0)
                    {
                        dataView2[0].Delete();
                    }
                    this.dsMailingLists.WriteXml(this.m_DataPath + "MailingLists.xml", XmlWriteMode.IgnoreSchema);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void UpdateMailingList(string mailingListID, string mailingListName, string description, string domainName, bool enabled)
        {
            if (mailingListName.Length == 0)
            {
                throw new Exception("You must specify mailingListName");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                try
                {
                    if (this.ContainsID(this.dsMailingLists.Tables["MailingLists"], "MailingListID", mailingListID))
                    {
                        using (DataView dataView = new DataView(this.dsMailingLists.Tables["MailingLists"]))
                        {
                            dataView.RowFilter = "MailingListID='" + mailingListID + "'";
                            using (DataView dataView2 = new DataView(this.dsMailingLists.Tables["MailingLists"]))
                            {
                                dataView2.RowFilter = "MailingListName='" + mailingListName + "'";
                                if (dataView2.Count > 0 && dataView2[0]["MailingListID"].ToString() != mailingListID)
                                {
                                    throw new Exception("Mailing list '" + mailingListName + "' already exists !");
                                }
                            }
                            if (dataView.Count > 0)
                            {
                                dataView[0]["MailingListName"] = mailingListName;
                                dataView[0]["Description"] = description;
                                dataView[0]["DomainName"] = domainName;
                                dataView[0]["Enabled"] = enabled;
                                this.dsMailingLists.WriteXml(this.m_DataPath + "MailingLists.xml", XmlWriteMode.IgnoreSchema);
                            }
                            goto IL_185;
                        }
                        goto IL_16F;
                        IL_185:
                        goto IL_18A;
                    }
                    IL_16F:
                    throw new Exception("Mailing list with specified ID '" + mailingListID + "' doesn't exist !");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                IL_18A:;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void AddMailingListAddress(string addressID, string mailingListName, string address)
        {
            if (addressID.Length == 0)
            {
                throw new Exception("You must specify addressID");
            }
            if (mailingListName.Length == 0)
            {
                throw new Exception("You must specify mailingListName");
            }
            if (address.Length == 0)
            {
                throw new Exception("You must specify address");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (!this.MailingListExists(mailingListName))
                {
                    throw new Exception("Mailing list doesn't '" + address + "' exist !");
                }
                if (this.ContainsID(this.dsMailingListAddresses.Tables["MailingListAddresses"], "AddressID", addressID))
                {
                    throw new Exception("Address with specified ID '" + addressID + "' already exists !");
                }
                using (DataView mailingListAddresses = this.GetMailingListAddresses(mailingListName))
                {
                    DataView expr_7F = mailingListAddresses;
                    expr_7F.RowFilter = expr_7F.RowFilter + " AND Address='" + address + "'";
                    if (mailingListAddresses.Count > 0)
                    {
                        throw new Exception("Mailing list address '" + address + "' already exists !");
                    }
                }
                string value = "";
                using (DataView dataView = new DataView(this.dsMailingLists.Tables["MailingLists"]))
                {
                    dataView.RowFilter = "MailingListName='" + mailingListName + "'";
                    value = dataView[0]["MailingListID"].ToString();
                }
                DataRow dataRow = this.dsMailingListAddresses.Tables["MailingListAddresses"].NewRow();
                dataRow["AddressID"] = addressID;
                dataRow["MailingListID"] = value;
                dataRow["Address"] = address;
                this.dsMailingListAddresses.Tables["MailingListAddresses"].Rows.Add(dataRow);
                this.dsMailingListAddresses.WriteXml(this.m_DataPath + "MailingListAddresses.xml", XmlWriteMode.IgnoreSchema);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void DeleteMailingListAddress(string addressID)
        {
            this.m_UpdSync.BeginUpdate();
            try
            {
                using (DataView dataView = new DataView(this.dsMailingListAddresses.Tables["MailingListAddresses"]))
                {
                    dataView.RowFilter = "AddressID='" + addressID + "'";
                    if (dataView.Count > 0)
                    {
                        dataView[0].Delete();
                    }
                    this.dsMailingListAddresses.WriteXml(this.m_DataPath + "MailingListAddresses.xml", XmlWriteMode.IgnoreSchema);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public DataView GetMailingListAddresses(string mailingListName)
        {
            this.m_UpdSync.AddMethod();
            DataView result;
            try
            {
                string str = "";
                using (DataView dataView = new DataView(this.dsMailingLists.Tables["MailingLists"]))
                {
                    dataView.RowFilter = "MailingListName='" + mailingListName + "'";
                    if (dataView.Count > 0)
                    {
                        str = dataView[0]["MailingListID"].ToString();
                    }
                }
                result = new DataView(this.dsMailingListAddresses.Copy().Tables["MailingListAddresses"])
                {
                    RowFilter = "MailingListID='" + str + "'"
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public DataView GetMailingListACL(string mailingListName)
        {
            this.m_UpdSync.AddMethod();
            DataView result;
            try
            {
                DataRow mailingList = this.GetMailingList(mailingListName);
                if (mailingList == null)
                {
                    throw new Exception("Invalid mailing list name, specified mailing list '" + mailingListName + "' doesn't exist !");
                }
                string str = mailingList["MailingListID"].ToString();
                result = new DataView(this.dsMailingListACL.Copy().Tables["ACL"])
                {
                    RowFilter = "MailingListID = '" + str + "'"
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public void AddMailingListACL(string mailingListName, string userOrGroup)
        {
            this.m_UpdSync.AddMethod();
            try
            {
                DataRow mailingList = this.GetMailingList(mailingListName);
                if (mailingList == null)
                {
                    throw new Exception("Invalid mailing list name, specified mailing list '" + mailingListName + "' doesn't exist !");
                }
                string text = mailingList["MailingListID"].ToString();
                foreach (DataRow dataRow in this.dsMailingListACL.Tables["ACL"].Rows)
                {
                    if (dataRow["MailingListID"].ToString() == text && dataRow["UserOrGroup"].ToString().ToLower() == userOrGroup.ToLower())
                    {
                        throw new Exception("Invalid userOrGroup, specified userOrGroup '" + userOrGroup + "' already exists !");
                    }
                }
                DataRow dataRow2 = this.dsMailingListACL.Tables["ACL"].NewRow();
                dataRow2["MailingListID"] = text;
                dataRow2["UserOrGroup"] = userOrGroup;
                this.dsMailingListACL.Tables["ACL"].Rows.Add(dataRow2);
                this.dsMailingListACL.WriteXml(this.m_DataPath + "MailingListACL.xml", XmlWriteMode.IgnoreSchema);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
        }

        public void DeleteMailingListACL(string mailingListName, string userOrGroup)
        {
            this.m_UpdSync.AddMethod();
            try
            {
                DataRow mailingList = this.GetMailingList(mailingListName);
                if (mailingList == null)
                {
                    throw new Exception("Invalid mailing list name, specified mailing list '" + mailingListName + "' doesn't exist !");
                }
                string b = mailingList["MailingListID"].ToString();
                foreach (DataRow dataRow in this.dsMailingListACL.Tables["ACL"].Rows)
                {
                    if (dataRow["MailingListID"].ToString() == b && dataRow["UserOrGroup"].ToString().ToLower() == userOrGroup.ToLower())
                    {
                        dataRow.Delete();
                        break;
                    }
                }
                this.dsMailingListACL.WriteXml(this.m_DataPath + "MailingListACL.xml", XmlWriteMode.IgnoreSchema);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
        }

        public bool CanAccessMailingList(string mailingListName, string user)
        {
            this.m_UpdSync.AddMethod();
            try
            {
                DataRow mailingList = this.GetMailingList(mailingListName);
                if (mailingList == null)
                {
                    throw new Exception("Invalid mailing list name, specified mailing list '" + mailingListName + "' doesn't exist !");
                }
                string b = mailingList["MailingListID"].ToString();
                foreach (DataRow dataRow in this.dsMailingListACL.Tables["ACL"].Rows)
                {
                    if (dataRow["MailingListID"].ToString() == b)
                    {
                        bool result;
                        if (dataRow["UserOrGroup"].ToString().ToLower() == "anyone")
                        {
                            result = true;
                            return result;
                        }
                        if (dataRow["UserOrGroup"].ToString().ToLower() == "authenticated users")
                        {
                            result = this.UserExists(user);
                            return result;
                        }
                        if (this.GroupExists(dataRow["UserOrGroup"].ToString()))
                        {
                            result = this.IsUserGroupMember(dataRow["UserOrGroup"].ToString(), user);
                            return result;
                        }
                        result = this.UserExists(user);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return false;
        }

        public bool MailingListExists(string mailingListName)
        {
            this.m_UpdSync.AddMethod();
            bool result;
            try
            {
                foreach (DataRow dataRow in this.dsMailingLists.Tables["MailingLists"].Rows)
                {
                    if (dataRow["MailingListName"].ToString().ToLower() == mailingListName.ToLower())
                    {
                        result = true;
                        return result;
                    }
                }
                result = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public DataView GetGlobalMessageRules()
        {
            this.m_UpdSync.AddMethod();
            DataView result;
            try
            {
                result = new DataView(this.dsRules.Copy().Tables["GlobalMessageRules"])
                {
                    Sort = "Cost ASC"
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public void AddGlobalMessageRule(string ruleID, long cost, bool enabled, GlobalMessageRule_CheckNextRule checkNextRule, string description, string matchExpression)
        {
            if (ruleID == null || ruleID == "")
            {
                throw new Exception("Invalid ruleID value, ruleID can't be '' or null !");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (this.GlobalMessageRuleExists(ruleID))
                {
                    throw new Exception("Specified ruleID '" + ruleID + "' already exists, choose another ruleID !");
                }
                DataRow dataRow = this.dsRules.Tables["GlobalMessageRules"].NewRow();
                dataRow["RuleID"] = ruleID;
                dataRow["Cost"] = cost;
                dataRow["Enabled"] = enabled;
                dataRow["CheckNextRuleIf"] = checkNextRule;
                dataRow["Description"] = description;
                dataRow["MatchExpression"] = matchExpression;
                this.dsRules.Tables["GlobalMessageRules"].Rows.Add(dataRow);
                this.dsRules.WriteXml(this.m_DataPath + "GlobalMessageRules.xml", XmlWriteMode.IgnoreSchema);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void DeleteGlobalMessageRule(string ruleID)
        {
            if (ruleID == null || ruleID == "")
            {
                throw new Exception("Invalid ruleID value, ruleID can't be '' or null !");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (!this.GlobalMessageRuleExists(ruleID))
                {
                    throw new Exception("Invalid ruleID '" + ruleID + "', specified ruleID doesn't exist !");
                }
                foreach (DataRowView dataRowView in this.GetGlobalMessageRuleActions(ruleID))
                {
                    this.DeleteGlobalMessageRuleAction(ruleID, dataRowView["ActionID"].ToString());
                }
                foreach (DataRow dataRow in this.dsRules.Tables["GlobalMessageRules"].Rows)
                {
                    if (dataRow["RuleID"].ToString().ToLower() == ruleID)
                    {
                        dataRow.Delete();
                        this.dsRules.WriteXml(this.m_DataPath + "GlobalMessageRules.xml", XmlWriteMode.IgnoreSchema);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void UpdateGlobalMessageRule(string ruleID, long cost, bool enabled, GlobalMessageRule_CheckNextRule checkNextRule, string description, string matchExpression)
        {
            if (ruleID == null || ruleID == "")
            {
                throw new Exception("Invalid ruleID value, ruleID can't be '' or null !");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (!this.GlobalMessageRuleExists(ruleID))
                {
                    throw new Exception("Invalid ruleID '" + ruleID + "', specified ruleID doesn't exist !");
                }
                foreach (DataRow dataRow in this.dsRules.Tables["GlobalMessageRules"].Rows)
                {
                    if (dataRow["RuleID"].ToString().ToLower() == ruleID)
                    {
                        dataRow["RuleID"] = ruleID;
                        dataRow["Cost"] = cost;
                        dataRow["Enabled"] = enabled;
                        dataRow["CheckNextRuleIf"] = checkNextRule;
                        dataRow["Description"] = description;
                        dataRow["MatchExpression"] = matchExpression;
                        this.dsRules.WriteXml(this.m_DataPath + "GlobalMessageRules.xml", XmlWriteMode.IgnoreSchema);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public bool GlobalMessageRuleExists(string ruleID)
        {
            return this.ContainsID(this.dsRules.Tables["GlobalMessageRules"], "RuleID", ruleID);
        }

        public DataView GetGlobalMessageRuleActions(string ruleID)
        {
            if (ruleID == null || ruleID == "")
            {
                throw new Exception("Invalid ruleID value, ruleID can't be '' or null !");
            }
            this.m_UpdSync.AddMethod();
            DataView result;
            try
            {
                if (!this.GlobalMessageRuleExists(ruleID))
                {
                    throw new Exception("Invalid ruleID '" + ruleID + "', specified ruleID doesn't exist !");
                }
                result = new DataView(this.dsRuleActions.Copy().Tables["GlobalMessageRuleActions"])
                {
                    RowFilter = "RuleID='" + ruleID + "'"
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public void AddGlobalMessageRuleAction(string ruleID, string actionID, string description, GlobalMessageRuleActionType actionType, byte[] actionData)
        {
            if (ruleID == null || ruleID == "")
            {
                throw new Exception("Invalid ruleID value, ruleID can't be '' or null !");
            }
            if (actionID == null || actionID == "")
            {
                throw new Exception("Invalid actionID value, actionID can't be '' or null !");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (!this.GlobalMessageRuleExists(ruleID))
                {
                    throw new Exception("Invalid ruleID '" + ruleID + "', specified ruleID doesn't exist !");
                }
                if (this.ContainsID(this.dsRuleActions.Tables["GlobalMessageRuleActions"], "ActionID", ruleID))
                {
                    throw new Exception("Specified actionID '" + actionID + "' already exists, choose another actionID !");
                }
                DataRow dataRow = this.dsRuleActions.Tables["GlobalMessageRuleActions"].NewRow();
                dataRow["RuleID"] = ruleID;
                dataRow["ActionID"] = actionID;
                dataRow["Description"] = description;
                dataRow["ActionType"] = actionType;
                dataRow["ActionData"] = actionData;
                this.dsRuleActions.Tables["GlobalMessageRuleActions"].Rows.Add(dataRow);
                this.dsRuleActions.WriteXml(this.m_DataPath + "GlobalMessageRuleActions.xml", XmlWriteMode.IgnoreSchema);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void DeleteGlobalMessageRuleAction(string ruleID, string actionID)
        {
            if (ruleID == null || ruleID == "")
            {
                throw new Exception("Invalid ruleID value, ruleID can't be '' or null !");
            }
            if (actionID == null || actionID == "")
            {
                throw new Exception("Invalid actionID value, actionID can't be '' or null !");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (!this.GlobalMessageRuleExists(ruleID))
                {
                    throw new Exception("Invalid ruleID '" + ruleID + "', specified ruleID doesn't exist !");
                }
                foreach (DataRow dataRow in this.dsRuleActions.Tables["GlobalMessageRuleActions"].Rows)
                {
                    if (dataRow["RuleID"].ToString().ToLower() == ruleID && dataRow["ActionID"].ToString().ToLower() == actionID)
                    {
                        dataRow.Delete();
                        this.dsRuleActions.WriteXml(this.m_DataPath + "GlobalMessageRuleActions.xml", XmlWriteMode.IgnoreSchema);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void UpdateGlobalMessageRuleAction(string ruleID, string actionID, string description, GlobalMessageRuleActionType actionType, byte[] actionData)
        {
            if (ruleID == null || ruleID == "")
            {
                throw new Exception("Invalid ruleID value, ruleID can't be '' or null !");
            }
            if (actionID == null || actionID == "")
            {
                throw new Exception("Invalid actionID value, actionID can't be '' or null !");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (!this.GlobalMessageRuleExists(ruleID))
                {
                    throw new Exception("Invalid ruleID '" + ruleID + "', specified ruleID doesn't exist !");
                }
                bool flag = false;
                foreach (DataRow dataRow in this.dsRuleActions.Tables["GlobalMessageRuleActions"].Rows)
                {
                    if (dataRow["RuleID"].ToString().ToLower() == ruleID && dataRow["ActionID"].ToString().ToLower() == actionID)
                    {
                        dataRow["RuleID"] = ruleID;
                        dataRow["ActionID"] = actionID;
                        dataRow["Description"] = description;
                        dataRow["ActionType"] = actionType;
                        dataRow["ActionData"] = actionData;
                        this.dsRuleActions.WriteXml(this.m_DataPath + "GlobalMessageRuleActions.xml", XmlWriteMode.IgnoreSchema);
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    throw new Exception("Invalid actionID '" + actionID + "', specified actionID doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public DataView GetRoutes()
        {
            this.m_UpdSync.AddMethod();
            DataView result;
            try
            {
                result = new DataView(this.dsRouting.Copy().Tables["Routing"])
                {
                    Sort = "Cost ASC"
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public void AddRoute(string routeID, long cost, bool enabled, string description, string pattern, RouteAction action, byte[] actionData)
        {
            if (routeID.Length == 0)
            {
                throw new Exception("You must specify routeID");
            }
            if (pattern.Length == 0)
            {
                throw new Exception("You must specify pattern");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (this.ContainsID(this.dsRouting.Tables["Routing"], "RouteID", routeID))
                {
                    throw new Exception("Route with specified ID '" + routeID + "' already exists !");
                }
                DataRow dataRow = this.dsRouting.Tables["Routing"].NewRow();
                dataRow["RouteID"] = routeID;
                dataRow["Cost"] = cost;
                dataRow["Enabled"] = enabled;
                dataRow["Description"] = description;
                dataRow["Pattern"] = pattern;
                dataRow["Action"] = action;
                dataRow["ActionData"] = actionData;
                this.dsRouting.Tables["Routing"].Rows.Add(dataRow);
                this.dsRouting.WriteXml(this.m_DataPath + "Routing.xml", XmlWriteMode.IgnoreSchema);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void DeleteRoute(string routeID)
        {
            this.m_UpdSync.BeginUpdate();
            try
            {
                using (DataView dataView = new DataView(this.dsRouting.Tables["Routing"]))
                {
                    dataView.RowFilter = "RouteID='" + routeID + "'";
                    if (dataView.Count > 0)
                    {
                        dataView[0].Delete();
                    }
                    this.dsRouting.WriteXml(this.m_DataPath + "Routing.xml", XmlWriteMode.IgnoreSchema);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void UpdateRoute(string routeID, long cost, bool enabled, string description, string pattern, RouteAction action, byte[] actionData)
        {
            if (pattern.Length == 0)
            {
                throw new Exception("You must specify pattern");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                bool flag = false;
                foreach (DataRow dataRow in this.dsRouting.Tables["Routing"].Rows)
                {
                    if (dataRow["RouteID"].ToString() == routeID)
                    {
                        dataRow["Cost"] = cost;
                        dataRow["Enabled"] = enabled;
                        dataRow["Description"] = description;
                        dataRow["Pattern"] = pattern;
                        dataRow["Action"] = action;
                        dataRow["ActionData"] = actionData;
                        this.dsRouting.WriteXml(this.m_DataPath + "Routing.xml", XmlWriteMode.IgnoreSchema);
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    throw new Exception("Route with specified ID '" + routeID + "' doesn't exist !");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }


        public void GetMessagesInfo(string accessingUser, string folderOwnerUser, string folder, List<IMAP_MessageInfo> messageInfos)
        {
            ArgsValidator.ValidateUserName(folderOwnerUser);
            ArgsValidator.ValidateFolder(folder);
            ArgsValidator.ValidateNotNull(messageInfos);
            if (!this.UserExists(folderOwnerUser))
            {
                throw new Exception("User '" + folderOwnerUser + "' doesn't exist !");
            }
            folder = PathHelper.NormalizeFolder(folder);
            string text = folder;
            LocalXmlStorage.SharedFolderMapInfo sharedFolderMapInfo = this.MapSharedFolder(text);
            if (sharedFolderMapInfo.IsSharedFolder)
            {
                folderOwnerUser = sharedFolderMapInfo.FolderOnwer;
                folder = sharedFolderMapInfo.Folder;
                if (folderOwnerUser == "" || folder == "")
                {
                    throw new ArgumentException("Specified root folder '" + text + "' isn't accessible !");
                }
            }
            if (!this.FolderExists(folderOwnerUser + "/" + folder))
            {
                throw new Exception("Folder '" + folder + "' doesn't exist !");
            }
            if (accessingUser.ToLower() != "system")
            {
                IMAP_ACL_Flags userACL = this.GetUserACL(folderOwnerUser, folder, accessingUser);
                if ((userACL & IMAP_ACL_Flags.r) == IMAP_ACL_Flags.None)
                {
                    throw new InsufficientPermissionsException(string.Concat(new string[]
                    {
                        "Insufficient permissions for folder '",
                        accessingUser,
                        "/",
                        folder,
                        "' !"
                    }));
                }
            }
            using (SQLiteConnection messagesInfoSqlCon = this.GetMessagesInfoSqlCon(folderOwnerUser, folder))
            {
                using (SQLiteCommand sQLiteCommand = messagesInfoSqlCon.CreateCommand())
                {
                    sQLiteCommand.CommandText = "select ID,UID,Size,Flags,InternalDateTime from MessagesInfo order by UID ASC;";
                    DataSet dataSet = new DataSet("dsMessagesInfo");
                    using (SQLiteDataAdapter sQLiteDataAdapter = new SQLiteDataAdapter(sQLiteCommand))
                    {
                        sQLiteDataAdapter.Fill(dataSet);
                    }
                    foreach (DataRow dataRow in dataSet.Tables[0].Rows)
                    {
                        DateTime internalDate = DateTime.Now;
                        try
                        {
                            internalDate = DateTime.ParseExact(dataRow["InternalDateTime"].ToString(), "yyyyMMddHHmmss", DateTimeFormatInfo.InvariantInfo);
                        }
                        catch
                        {
                        }
                        messageInfos.Add(new IMAP_MessageInfo(dataRow["ID"].ToString(), Convert.ToInt64(dataRow["UID"]), (dataRow["Flags"].ToString() == string.Empty) ? new string[0] : dataRow["Flags"].ToString().Split(new char[]
                        {
                            ' '
                        }), Convert.ToInt32(dataRow["Size"]), internalDate));
                    }
                }
            }
        }

        public void StoreMessage(string accessingUser, string folderOwnerUser, string folder, Stream msgStream, DateTime date, string[] flags)
        {
            ArgsValidator.ValidateUserName(folderOwnerUser);
            ArgsValidator.ValidateFolder(folder);
            ArgsValidator.ValidateNotNull(msgStream);
            if (!this.UserExists(folderOwnerUser))
            {
                throw new Exception("User '" + folderOwnerUser + "' doesn't exist !");
            }
            folder = PathHelper.NormalizeFolder(folder);
            string text = folder;
            LocalXmlStorage.SharedFolderMapInfo sharedFolderMapInfo = this.MapSharedFolder(text);
            if (sharedFolderMapInfo.IsSharedFolder)
            {
                folderOwnerUser = sharedFolderMapInfo.FolderOnwer;
                folder = sharedFolderMapInfo.Folder;
                if (folderOwnerUser == "" || folder == "")
                {
                    throw new ArgumentException("Specified root folder '" + text + "' isn't accessible !");
                }
            }
            if (!this.FolderExists(folderOwnerUser + "/" + folder))
            {
                throw new Exception("Folder '" + folder + "' doesn't exist !");
            }
            if (accessingUser.ToLower() != "system")
            {
                IMAP_ACL_Flags userACL = this.GetUserACL(folderOwnerUser, folder, accessingUser);
                if ((userACL & IMAP_ACL_Flags.p) == IMAP_ACL_Flags.None && (userACL & IMAP_ACL_Flags.i) == IMAP_ACL_Flags.None)
                {
                    throw new InsufficientPermissionsException(string.Concat(new string[]
                    {
                        "Insufficient permissions for folder '",
                        accessingUser,
                        "/",
                        folder,
                        "' !"
                    }));
                }
            }
            string str = PathHelper.PathFix(PathHelper.DirectoryExists(PathHelper.PathFix(string.Concat(new string[]
            {
                this.m_MailStorePath,
                "Mailboxes\\",
                folderOwnerUser,
                "\\",
                IMAP_Utils.Encode_IMAP_UTF7_String(folder)
            }))) + "\\");
            int nextUid = this.GetNextUid(folderOwnerUser, folder);
            string str2 = this.CreateMessageFileName(date.ToUniversalTime(), nextUid);
            using (SQLiteConnection messagesInfoSqlCon = this.GetMessagesInfoSqlCon(folderOwnerUser, folder))
            {
                long size = 0L;
                using (FileStream fileStream = File.Create(str + str2 + ".eml"))
                {
                    size = Net_Utils.StreamCopy(msgStream, fileStream, 32000);
                }
                if (File.Exists(str + str2 + ".eml"))
                {
                    this.ChangeMailboxSize(folderOwnerUser, size);
                    this.AddMessageInfo(messagesInfoSqlCon, str + str2 + ".eml", Path.GetFileNameWithoutExtension(str + str2 + ".eml"), (long)nextUid, size, flags, date);
                }
            }
        }

        public void StoreMessageFlags(string accessingUser, string folderOwnerUser, string folder, IMAP_MessageInfo messageInfo, string[] flags)
        {
            ArgsValidator.ValidateUserName(folderOwnerUser);
            ArgsValidator.ValidateFolder(folder);
            if (!this.UserExists(folderOwnerUser))
            {
                throw new Exception("User '" + folderOwnerUser + "' doesn't exist !");
            }
            folder = PathHelper.NormalizeFolder(folder);
            string text = folder;
            LocalXmlStorage.SharedFolderMapInfo sharedFolderMapInfo = this.MapSharedFolder(text);
            if (sharedFolderMapInfo.IsSharedFolder)
            {
                folderOwnerUser = sharedFolderMapInfo.FolderOnwer;
                folder = sharedFolderMapInfo.Folder;
                if (folderOwnerUser == "" || folder == "")
                {
                    throw new ArgumentException("Specified root folder '" + text + "' isn't accessible !");
                }
            }
            if (!this.FolderExists(folderOwnerUser + "/" + folder))
            {
                throw new Exception("Folder '" + folder + "' doesn't exist !");
            }
            if (accessingUser != "system")
            {
                IMAP_ACL_Flags userACL = this.GetUserACL(folderOwnerUser, folder, accessingUser);
                if ((userACL & IMAP_ACL_Flags.s) == IMAP_ACL_Flags.None)
                {
                    flags = IMAP_Utils.MessageFlagsRemove(flags, new string[]
                    {
                        "Seen"
                    });
                }
                else if ((userACL & IMAP_ACL_Flags.d) == IMAP_ACL_Flags.None)
                {
                    flags = IMAP_Utils.MessageFlagsRemove(flags, new string[]
                    {
                        "Deleted"
                    });
                }
                else if ((userACL & IMAP_ACL_Flags.s) == IMAP_ACL_Flags.None)
                {
                    flags = IMAP_Utils.MessageFlagsRemove(flags, new string[]
                    {
                        "Answered",
                        "Draft",
                        "Flagged"
                    });
                }
            }
            using (SQLiteConnection messagesInfoSqlCon = this.GetMessagesInfoSqlCon(folderOwnerUser, folder))
            {
                using (SQLiteCommand sQLiteCommand = messagesInfoSqlCon.CreateCommand())
                {
                    sQLiteCommand.CommandText = string.Concat(new string[]
                    {
                        "update MessagesInfo set flags='",
                        Net_Utils.ArrayToString(flags, " "),
                        "' where ID='",
                        messageInfo.ID,
                        "';"
                    });
                    sQLiteCommand.ExecuteNonQuery();
                }
            }
        }

        public void DeleteMessage(string accessingUser, string folderOwnerUser, string folder, string messageID, int uid)
        {
            ArgsValidator.ValidateUserName(folderOwnerUser);
            ArgsValidator.ValidateFolder(folder);
            ArgsValidator.ValidateNotNull(messageID);
            if (!this.UserExists(folderOwnerUser))
            {
                throw new Exception("User '" + folderOwnerUser + "' doesn't exist !");
            }
            folder = PathHelper.NormalizeFolder(folder);
            string text = folder;
            LocalXmlStorage.SharedFolderMapInfo sharedFolderMapInfo = this.MapSharedFolder(text);
            if (sharedFolderMapInfo.IsSharedFolder)
            {
                folderOwnerUser = sharedFolderMapInfo.FolderOnwer;
                folder = sharedFolderMapInfo.Folder;
                if (folderOwnerUser == "" || folder == "")
                {
                    throw new ArgumentException("Specified root folder '" + text + "' isn't accessible !");
                }
            }
            if (!this.FolderExists(folderOwnerUser + "/" + folder))
            {
                throw new Exception("Folder '" + folder + "' doesn't exist !");
            }
            if (accessingUser.ToLower() != "system")
            {
                IMAP_ACL_Flags userACL = this.GetUserACL(folderOwnerUser, folder, accessingUser);
                if ((userACL & IMAP_ACL_Flags.d) == IMAP_ACL_Flags.None)
                {
                    throw new InsufficientPermissionsException(string.Concat(new string[]
                    {
                        "Insufficient permissions for folder '",
                        accessingUser,
                        "/",
                        folder,
                        "' !"
                    }));
                }
            }
            string str = PathHelper.DirectoryExists(PathHelper.PathFix(string.Concat(new string[]
            {
                this.m_MailStorePath,
                "Mailboxes\\",
                folderOwnerUser,
                "\\",
                IMAP_Utils.Encode_IMAP_UTF7_String(folder),
                "\\"
            })));
            string text2 = PathHelper.FileExists(str + messageID + ".eml");
            string text3 = PathHelper.FileExists(str + messageID + ".info");
            using (SQLiteConnection messagesInfoSqlCon = this.GetMessagesInfoSqlCon(folderOwnerUser, folder))
            {
                using (SQLiteCommand sQLiteCommand = messagesInfoSqlCon.CreateCommand())
                {
                    sQLiteCommand.CommandText = "delete from MessagesInfo where ID='" + messageID + "';";
                    sQLiteCommand.ExecuteNonQuery();
                }
            }
            if (text3 != null)
            {
                File.Delete(text3);
            }
            if (text2 != null)
            {
                if (Convert.ToBoolean(this.GetRecycleBinSettings().Rows[0]["DeleteToRecycleBin"]))
                {
                    RecycleBinManager.StoreToRecycleBin(folderOwnerUser, folder, text2);
                }
                FileInfo fileInfo = new FileInfo(text2);
                if (fileInfo.Exists)
                {
                    long length = fileInfo.Length;
                    fileInfo.Delete();
                    this.ChangeMailboxSize(folderOwnerUser, -length);
                }
            }
        }

        public void GetMessageItems(string accessingUser, string folderOwnerUser, string folder, EmailMessageItems e)
        {
            ArgsValidator.ValidateUserName(folderOwnerUser);
            ArgsValidator.ValidateFolder(folder);
            ArgsValidator.ValidateNotNull(e);
            if (!this.UserExists(folderOwnerUser))
            {
                throw new Exception("User '" + folderOwnerUser + "' doesn't exist !");
            }
            folder = PathHelper.NormalizeFolder(folder);
            string text = folder;
            LocalXmlStorage.SharedFolderMapInfo sharedFolderMapInfo = this.MapSharedFolder(text);
            if (sharedFolderMapInfo.IsSharedFolder)
            {
                folderOwnerUser = sharedFolderMapInfo.FolderOnwer;
                folder = sharedFolderMapInfo.Folder;
                if (folderOwnerUser == "" || folder == "")
                {
                    throw new ArgumentException("Specified root folder '" + text + "' isn't accessible !");
                }
            }
            if (!this.FolderExists(folderOwnerUser + "/" + folder))
            {
                throw new Exception("Folder '" + folder + "' doesn't exist !");
            }
            if (accessingUser.ToLower() != "system")
            {
                IMAP_ACL_Flags userACL = this.GetUserACL(folderOwnerUser, folder, accessingUser);
                if ((userACL & IMAP_ACL_Flags.r) == IMAP_ACL_Flags.None)
                {
                    throw new InsufficientPermissionsException(string.Concat(new string[]
                    {
                        "Insufficient permissions for folder '",
                        accessingUser,
                        "/",
                        folder,
                        "' !"
                    }));
                }
            }
            string str = PathHelper.DirectoryExists(PathHelper.PathFix(string.Concat(new string[]
            {
                this.m_MailStorePath,
                "Mailboxes\\",
                folderOwnerUser,
                "\\",
                IMAP_Utils.Encode_IMAP_UTF7_String(folder)
            })));
            string text2 = PathHelper.FileExists(PathHelper.PathFix(str + "\\" + e.MessageID + ".eml"));
            if (text2 != null)
            {
                if ((e.MessageItems & (IMAP_MessageItems)18) != IMAP_MessageItems.None)
                {
                    bool flag = true;
                    FileStream fileStream = File.Open(text2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    new _InternalHeader(fileStream);
                    long position = fileStream.Position;
                    if ((e.MessageItems & IMAP_MessageItems.Header) != IMAP_MessageItems.None)
                    {
                        fileStream.Position = position;
                        e.Header = this.GetTopLines(fileStream, 0);
                    }
                    if ((e.MessageItems & IMAP_MessageItems.Message) != IMAP_MessageItems.None)
                    {
                        fileStream.Position = position;
                        e.MessageStream = fileStream;
                        flag = false;
                    }
                    if (flag)
                    {
                        fileStream.Dispose();
                        return;
                    }
                }
            }
            else
            {
                e.MessageExists = false;
            }
        }

        public byte[] GetMessageTopLines(string accessingUser, string folderOwnerUser, string folder, string msgID, int nrLines)
        {
            ArgsValidator.ValidateUserName(folderOwnerUser);
            ArgsValidator.ValidateFolder(folder);
            ArgsValidator.ValidateNotNull(msgID);
            if (!this.UserExists(folderOwnerUser))
            {
                throw new Exception("User '" + folderOwnerUser + "' doesn't exist !");
            }
            folder = PathHelper.NormalizeFolder(folder);
            string text = folder;
            LocalXmlStorage.SharedFolderMapInfo sharedFolderMapInfo = this.MapSharedFolder(text);
            if (sharedFolderMapInfo.IsSharedFolder)
            {
                folderOwnerUser = sharedFolderMapInfo.FolderOnwer;
                folder = sharedFolderMapInfo.Folder;
                if (folderOwnerUser == "" || folder == "")
                {
                    throw new ArgumentException("Specified root folder '" + text + "' isn't accessible !");
                }
            }
            if (!this.FolderExists(folderOwnerUser + "/" + folder))
            {
                throw new Exception("Folder '" + folder + "' doesn't exist !");
            }
            if (accessingUser.ToLower() != "system")
            {
                IMAP_ACL_Flags userACL = this.GetUserACL(folderOwnerUser, folder, accessingUser);
                if ((userACL & IMAP_ACL_Flags.r) == IMAP_ACL_Flags.None)
                {
                    throw new InsufficientPermissionsException(string.Concat(new string[]
                    {
                        "Insufficient permissions for folder '",
                        accessingUser,
                        "/",
                        folder,
                        "' !"
                    }));
                }
            }
            string text2 = PathHelper.FileExists(PathHelper.PathFix(string.Concat(new string[]
            {
                this.m_MailStorePath,
                "Mailboxes\\",
                folderOwnerUser,
                "\\",
                IMAP_Utils.Encode_IMAP_UTF7_String(folder),
                "\\",
                msgID,
                ".eml"
            })));
            if (text2 == null)
            {
                return null;
            }
            byte[] topLines;
            using (FileStream fileStream = File.Open(text2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                new _InternalHeader(fileStream);
                topLines = this.GetTopLines(fileStream, nrLines);
            }
            return topLines;
        }

        public void CopyMessage(string accessingUser, string folderOwnerUser, string folder, string destFolderUser, string destFolder, IMAP_MessageInfo messageInfo)
        {
            ArgsValidator.ValidateUserName(folderOwnerUser);
            ArgsValidator.ValidateFolder(folder);
            ArgsValidator.ValidateUserName(destFolderUser);
            ArgsValidator.ValidateFolder(destFolder);
            ArgsValidator.ValidateNotNull(messageInfo);
            EmailMessageItems emailMessageItems = new EmailMessageItems(messageInfo.ID, IMAP_MessageItems.Message);
            this.GetMessageItems(accessingUser, folderOwnerUser, folder, emailMessageItems);
            this.StoreMessage("system", destFolderUser, destFolder, emailMessageItems.MessageStream, messageInfo.InternalDate, messageInfo.Flags);
            emailMessageItems.MessageStream.Dispose();
        }

        public void Search(string accessingUser, string folderOwnerUser, string folder, IMAP_e_Search e)
        {
            ArgsValidator.ValidateUserName(folderOwnerUser);
            ArgsValidator.ValidateFolder(folder);
            ArgsValidator.ValidateNotNull(e);
            if (!this.UserExists(folderOwnerUser))
            {
                throw new Exception("User '" + folderOwnerUser + "' doesn't exist !");
            }
            folder = PathHelper.NormalizeFolder(folder);
            string text = folder;
            LocalXmlStorage.SharedFolderMapInfo sharedFolderMapInfo = this.MapSharedFolder(text);
            if (sharedFolderMapInfo.IsSharedFolder)
            {
                folderOwnerUser = sharedFolderMapInfo.FolderOnwer;
                folder = sharedFolderMapInfo.Folder;
                if (folderOwnerUser == "" || folder == "")
                {
                    throw new ArgumentException("Specified root folder '" + text + "' isn't accessible !");
                }
            }
            if (!this.FolderExists(folderOwnerUser + "/" + folder))
            {
                throw new Exception("Folder '" + folder + "' doesn't exist !");
            }
            if (accessingUser.ToLower() != "system")
            {
                IMAP_ACL_Flags userACL = this.GetUserACL(folderOwnerUser, folder, accessingUser);
                if ((userACL & IMAP_ACL_Flags.r) == IMAP_ACL_Flags.None)
                {
                    throw new InsufficientPermissionsException(string.Concat(new string[]
                    {
                        "Insufficient permissions for folder '",
                        accessingUser,
                        "/",
                        folder,
                        "' !"
                    }));
                }
            }
            using (SQLiteConnection messagesInfoSqlCon = this.GetMessagesInfoSqlCon(folderOwnerUser, folder))
            {
                Dictionary<long, long> dictionary = new Dictionary<long, long>();
                using (SQLiteCommand sQLiteCommand = messagesInfoSqlCon.CreateCommand())
                {
                    sQLiteCommand.CommandText = "select UID from MessagesInfo ORDER by UID ASC;";
                    using (SQLiteDataAdapter sQLiteDataAdapter = new SQLiteDataAdapter(sQLiteCommand))
                    {
                        DataSet dataSet = new DataSet();
                        sQLiteDataAdapter.Fill(dataSet);
                        for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                        {
                            dictionary.Add((long)(i + 1), Convert.ToInt64(dataSet.Tables[0].Rows[i]["UID"]));
                        }
                    }
                    using (SQLiteCommand sQLiteCommand2 = messagesInfoSqlCon.CreateCommand())
                    {
                        sQLiteCommand2.CommandText = "select UID from MessagesInfo where " + this.SearchCriteriaToSql(e.Criteria, dictionary) + ";";
                        DataSet dataSet2 = new DataSet("dsMessagesInfo");
                        using (SQLiteDataAdapter sQLiteDataAdapter2 = new SQLiteDataAdapter(sQLiteCommand2))
                        {
                            sQLiteDataAdapter2.Fill(dataSet2);
                        }
                        foreach (DataRow dataRow in dataSet2.Tables[0].Rows)
                        {
                            e.AddMessage(Convert.ToInt64(dataRow["UID"]));
                        }
                    }
                }
            }
        }

        public string[] GetFolders(string userName, bool includeSharedFolders)
        {
            ArgsValidator.ValidateUserName(userName);
            if (!this.UserExists(userName))
            {
                throw new Exception("User '" + userName + "' desn't exist !");
            }
            string text = PathHelper.DirectoryExists(PathHelper.PathFix(this.m_MailStorePath + "Mailboxes\\" + userName));
            if (text == null)
            {
                Directory.CreateDirectory(PathHelper.PathFix(this.m_MailStorePath + "Mailboxes\\" + userName));
                text = PathHelper.PathFix(this.m_MailStorePath + "Mailboxes\\" + userName);
            }
            string[] fileSytemFolders = this.GetFileSytemFolders(text + "\\", false);
            List<string> list = new List<string>();
            string[] array = fileSytemFolders;
            for (int i = 0; i < array.Length; i++)
            {
                string text2 = array[i];
                if ((this.GetUserACL(userName, text2, userName) & IMAP_ACL_Flags.r) != IMAP_ACL_Flags.None)
                {
                    list.Add(IMAP_Utils.Decode_IMAP_UTF7_String(text2).Replace('\\', '/'));
                }
            }
            if (includeSharedFolders)
            {
                SharedFolderRoot[] sharedFolderRoots = this.GetSharedFolderRoots();
                SharedFolderRoot[] array2 = sharedFolderRoots;
                for (int j = 0; j < array2.Length; j++)
                {
                    SharedFolderRoot sharedFolderRoot = array2[j];
                    if (sharedFolderRoot.Enabled)
                    {
                        if (sharedFolderRoot.RootType == SharedFolderRootType.BoundedRootFolder)
                        {
                            if (!string.IsNullOrEmpty(sharedFolderRoot.BoundedFolder) && (this.GetUserACL(sharedFolderRoot.BoundedUser, sharedFolderRoot.BoundedFolder, userName) & IMAP_ACL_Flags.r) != IMAP_ACL_Flags.None)
                            {
                                list.Add(sharedFolderRoot.FolderName);
                            }
                            string[] folders = this.GetFolders(sharedFolderRoot.BoundedUser, false);
                            string[] array3 = folders;
                            for (int k = 0; k < array3.Length; k++)
                            {
                                string text3 = array3[k];
                                string text4 = this.RemovePathRoot(sharedFolderRoot.BoundedFolder, text3);
                                if (text4 != null && (this.GetUserACL(sharedFolderRoot.BoundedUser, text3, userName) & IMAP_ACL_Flags.r) != IMAP_ACL_Flags.None)
                                {
                                    list.Add(sharedFolderRoot.FolderName + "/" + text4);
                                }
                            }
                        }
                        else
                        {
                            List<string> list2 = new List<string>();
                            foreach (DataRow dataRow in this.dsImapACL.Tables["ACL"].Rows)
                            {
                                string text5 = dataRow["User"].ToString().ToLower();
                                string item = dataRow["Folder"].ToString().Split(new char[]
                                {
                                    '/'
                                }, 2)[0];
                                if (text5 == "anyone")
                                {
                                    if (!list2.Contains(item))
                                    {
                                        list2.Add(item);
                                    }
                                }
                                else if (text5 == userName.ToLower())
                                {
                                    if (!list2.Contains(item))
                                    {
                                        list2.Add(item);
                                    }
                                }
                                else
                                {
                                    DataRow group = this.GetGroup(text5);
                                    if (group != null && this.IsUserGroupMember(group["GroupName"].ToString(), userName) && !list2.Contains(item))
                                    {
                                        list2.Add(item);
                                    }
                                }
                            }
                            foreach (string current in list2)
                            {
                                string[] folders2 = this.GetFolders(current, false);
                                string[] array4 = folders2;
                                for (int l = 0; l < array4.Length; l++)
                                {
                                    string text6 = array4[l];
                                    if ((this.GetUserACL(current, text6, userName) & IMAP_ACL_Flags.r) != IMAP_ACL_Flags.None)
                                    {
                                        list.Add(string.Concat(new string[]
                                        {
                                            sharedFolderRoot.FolderName,
                                            "/",
                                            current,
                                            "/",
                                            text6
                                        }));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return list.ToArray();
        }

        public bool FolderExists(string folderName)
        {
            if (PathHelper.DirectoryExists(PathHelper.PathFix(this.m_MailStorePath + "Mailboxes\\" + IMAP_Utils.Encode_IMAP_UTF7_String(folderName))) != null)
            {
                return true;
            }
            string[] array = folderName.Split(new char[]
            {
                '/'
            }, 2);
            if (array[1].ToLower() == "inbox")
            {
                Directory.CreateDirectory(PathHelper.PathFix(this.m_MailStorePath + "Mailboxes\\" + array[0] + "\\Inbox"));
                return true;
            }
            return false;
        }

        public void CreateFolder(string accessingUser, string folderOwnerUser, string folder)
        {
            this.m_UpdSync.BeginUpdate();
            try
            {
                ArgsValidator.ValidateUserName(folderOwnerUser);
                ArgsValidator.ValidateFolder(folder);
                if (!this.UserExists(folderOwnerUser))
                {
                    throw new Exception("User '" + folderOwnerUser + "' doesn't exist !");
                }
                folder = PathHelper.NormalizeFolder(folder);
                string text = folder;
                LocalXmlStorage.SharedFolderMapInfo sharedFolderMapInfo = this.MapSharedFolder(text);
                if (sharedFolderMapInfo.IsSharedFolder)
                {
                    folderOwnerUser = sharedFolderMapInfo.FolderOnwer;
                    folder = sharedFolderMapInfo.Folder;
                    if (folderOwnerUser == "" || folder == "")
                    {
                        throw new ArgumentException("Specified root folder '" + text + "' isn't accessible !");
                    }
                }
                if (this.FolderExists(folderOwnerUser + "/" + folder))
                {
                    throw new Exception("Folder '" + folder + "' already exist !");
                }
                if (accessingUser.ToLower() != "system")
                {
                    IMAP_ACL_Flags userACL = this.GetUserACL(folderOwnerUser, folder, accessingUser);
                    if ((userACL & IMAP_ACL_Flags.c) == IMAP_ACL_Flags.None)
                    {
                        throw new InsufficientPermissionsException(string.Concat(new string[]
                        {
                            "Insufficient permissions for folder '",
                            accessingUser,
                            "/",
                            folder,
                            "' !"
                        }));
                    }
                }
                string text2 = PathHelper.PathFix(string.Concat(new string[]
                {
                    this.m_MailStorePath,
                    "Mailboxes\\",
                    folderOwnerUser,
                    "\\",
                    IMAP_Utils.Encode_IMAP_UTF7_String(folder)
                }));
                if (PathHelper.DirectoryExists(text2) != null)
                {
                    throw new Exception("Folder(" + folder + ") already exists");
                }
                Directory.CreateDirectory(text2);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void DeleteFolder(string accessingUser, string folderOwnerUser, string folder)
        {
            this.m_UpdSync.BeginUpdate();
            try
            {
                ArgsValidator.ValidateUserName(folderOwnerUser);
                ArgsValidator.ValidateFolder(folder);
                if (!this.UserExists(folderOwnerUser))
                {
                    throw new Exception("User '" + folderOwnerUser + "' doesn't exist !");
                }
                folder = PathHelper.NormalizeFolder(folder);
                foreach (DataRow dataRow in this.dsUsersDefaultFolders.Tables["UsersDefaultFolders"].Rows)
                {
                    if (dataRow["FolderName"].ToString().ToLower() == folder.ToLower() && Convert.ToBoolean(dataRow["Permanent"]))
                    {
                        throw new Exception("Can't delete permanent folder '" + folder + "' !");
                    }
                }
                string text = folder;
                LocalXmlStorage.SharedFolderMapInfo sharedFolderMapInfo = this.MapSharedFolder(text);
                if (sharedFolderMapInfo.IsSharedFolder)
                {
                    folderOwnerUser = sharedFolderMapInfo.FolderOnwer;
                    folder = sharedFolderMapInfo.Folder;
                    if (sharedFolderMapInfo.SharedRootName.ToLower() == text.ToLower())
                    {
                        throw new ArgumentException("Can't delete shared root folder '" + text + "' !");
                    }
                    if (folder == "")
                    {
                        throw new ArgumentException("Can't delete shared root folder '" + text + "' !");
                    }
                    if (folderOwnerUser == "" || folder == "")
                    {
                        throw new ArgumentException("Specified root folder '" + text + "' isn't accessible !");
                    }
                }
                if (!this.FolderExists(folderOwnerUser + "/" + folder))
                {
                    throw new Exception("Folder '" + folder + "' doesn't exist !");
                }
                if (accessingUser.ToLower() != "system")
                {
                    IMAP_ACL_Flags userACL = this.GetUserACL(folderOwnerUser, folder, accessingUser);
                    if ((userACL & IMAP_ACL_Flags.c) == IMAP_ACL_Flags.None)
                    {
                        throw new InsufficientPermissionsException(string.Concat(new string[]
                        {
                            "Insufficient permissions for folder '",
                            accessingUser,
                            "/",
                            folder,
                            "' !"
                        }));
                    }
                }
                string text2 = PathHelper.DirectoryExists(PathHelper.PathFix(string.Concat(new string[]
                {
                    this.m_MailStorePath,
                    "Mailboxes\\",
                    folderOwnerUser,
                    "\\",
                    IMAP_Utils.Encode_IMAP_UTF7_String(folder)
                })));
                if (text2 == null)
                {
                    throw new Exception("Folder(" + folder + ") doesn't exist");
                }
                if (Convert.ToBoolean(this.GetRecycleBinSettings().Rows[0]["DeleteToRecycleBin"]))
                {
                    string[] files = Directory.GetFiles(text2, "*.eml");
                    string[] array = files;
                    for (int i = 0; i < array.Length; i++)
                    {
                        string messageFile = array[i];
                        RecycleBinManager.StoreToRecycleBin(folderOwnerUser, folder, messageFile);
                    }
                }
                Directory.Delete(text2, true);
                bool flag = false;
                for (int j = 0; j < this.dsImapACL.Tables["ACL"].Rows.Count; j++)
                {
                    DataRow dataRow2 = this.dsImapACL.Tables["ACL"].Rows[0];
                    if (dataRow2["Folder"].ToString().ToLower().StartsWith(folderOwnerUser + "/" + folder.ToLower()))
                    {
                        dataRow2.Delete();
                        j--;
                        flag = true;
                    }
                }
                if (flag)
                {
                    this.dsImapACL.WriteXml(this.m_DataPath + "IMAP_ACL.xml", XmlWriteMode.IgnoreSchema);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void RenameFolder(string accessingUser, string folderOwnerUser, string folder, string newFolder)
        {
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (folder.ToLower() == "Inbox")
                {
                    throw new Exception("Can't rename Inbox");
                }
                ArgsValidator.ValidateUserName(folderOwnerUser);
                ArgsValidator.ValidateFolder(folder);
                ArgsValidator.ValidateFolder(newFolder);
                if (!this.UserExists(folderOwnerUser))
                {
                    throw new Exception("User '" + folderOwnerUser + "' doesn't exist !");
                }
                folder = PathHelper.NormalizeFolder(folder);
                string text = folder;
                LocalXmlStorage.SharedFolderMapInfo sharedFolderMapInfo = this.MapSharedFolder(text);
                if (sharedFolderMapInfo.IsSharedFolder)
                {
                    folderOwnerUser = sharedFolderMapInfo.FolderOnwer;
                    folder = sharedFolderMapInfo.Folder;
                    if (text.ToLower() == sharedFolderMapInfo.SharedRootName.ToLower() || folderOwnerUser == "" || folder == "")
                    {
                        throw new Exception("Can't rename shared root folder '" + text + "' !");
                    }
                }
                newFolder = PathHelper.NormalizeFolder(newFolder);
                string text2 = newFolder;
                LocalXmlStorage.SharedFolderMapInfo sharedFolderMapInfo2 = this.MapSharedFolder(text2);
                if (sharedFolderMapInfo2.IsSharedFolder)
                {
                    string folderOnwer = sharedFolderMapInfo2.FolderOnwer;
                    newFolder = sharedFolderMapInfo2.Folder;
                    if (text2.ToLower() == sharedFolderMapInfo2.SharedRootName.ToLower() || folderOnwer == "" || newFolder == "")
                    {
                        throw new Exception("Invalid destination folder value, folder '" + text2 + "' alreay exists !");
                    }
                }
                if (sharedFolderMapInfo.SharedRootName.ToLower() != sharedFolderMapInfo2.SharedRootName.ToLower() || sharedFolderMapInfo.FolderOnwer.ToLower() != sharedFolderMapInfo2.FolderOnwer.ToLower())
                {
                    throw new ArgumentException("Shared folder can't change root folder !");
                }
                if (!this.FolderExists(folderOwnerUser + "/" + folder))
                {
                    throw new Exception("Folder '" + folder + "' doesn't exist !");
                }
                if (this.FolderExists(folderOwnerUser + "/" + newFolder))
                {
                    throw new Exception("Folder '" + newFolder + "' doesn't exist !");
                }
                if (accessingUser.ToLower() != "system")
                {
                    IMAP_ACL_Flags userACL = this.GetUserACL(folderOwnerUser, folder, accessingUser);
                    if ((userACL & IMAP_ACL_Flags.c) == IMAP_ACL_Flags.None)
                    {
                        throw new InsufficientPermissionsException(string.Concat(new string[]
                        {
                            "Insufficient permissions for folder '",
                            accessingUser,
                            "/",
                            folder,
                            "' !"
                        }));
                    }
                }
                string text3 = PathHelper.DirectoryExists(PathHelper.PathFix(string.Concat(new string[]
                {
                    this.m_MailStorePath,
                    "Mailboxes\\",
                    folderOwnerUser,
                    "\\",
                    IMAP_Utils.Encode_IMAP_UTF7_String(folder)
                })));
                string text4 = PathHelper.PathFix(string.Concat(new string[]
                {
                    this.m_MailStorePath,
                    "Mailboxes\\",
                    folderOwnerUser,
                    "\\",
                    IMAP_Utils.Encode_IMAP_UTF7_String(newFolder)
                }));
                if (text3 == null)
                {
                    throw new Exception("Source Folder(" + folder + ") doesn't exist");
                }
                if (PathHelper.DirectoryExists(text4) != null)
                {
                    throw new Exception("Destination Folder(" + newFolder + ") already exists");
                }
                Directory.Move(text3, text4);
                bool flag = false;
                foreach (DataRow dataRow in this.dsImapACL.Tables["ACL"].Rows)
                {
                    if (dataRow["Folder"].ToString().ToLower().StartsWith(folderOwnerUser + "/" + folder.ToLower()))
                    {
                        string text5 = folderOwnerUser + "/" + folder.ToLower();
                        dataRow["Folder"] = folderOwnerUser + "/" + newFolder + dataRow["Folder"].ToString().Substring(text5.Length);
                        flag = true;
                    }
                }
                if (flag)
                {
                    this.dsImapACL.WriteXml(this.m_DataPath + "IMAP_ACL.xml", XmlWriteMode.IgnoreSchema);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public DateTime FolderCreationTime(string folderOwnerUser, string folder)
        {
            string text = PathHelper.DirectoryExists(string.Concat(new string[]
            {
                this.m_MailStorePath,
                "Mailboxes/",
                folderOwnerUser,
                "/",
                IMAP_Utils.Encode_IMAP_UTF7_String(folder)
            }));
            if (text == null)
            {
                throw new Exception(string.Concat(new string[]
                {
                    "Folder '",
                    folderOwnerUser,
                    "/",
                    folder,
                    "' doesn't exist !"
                }));
            }
            if (File.Exists(text + "/_FolderCreationTime.txt"))
            {
                return DateTime.ParseExact(File.ReadAllText(text + "/_FolderCreationTime.txt"), "yyyyMMdd HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
            }
            DateTime creationTime = Directory.GetCreationTime(text);
            File.WriteAllText(text + "/_FolderCreationTime.txt", creationTime.ToString("yyyyMMdd HH:mm:ss"));
            return creationTime;
        }

        public string[] GetSubscribedFolders(string userName)
        {
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add("Subscriptions");
            dataSet.Tables["Subscriptions"].Columns.Add("Name");
            string text = PathHelper.DirectoryExists(PathHelper.PathFix(this.m_MailStorePath + "Mailboxes\\" + userName));
            if (text == null)
            {
                Directory.CreateDirectory(PathHelper.PathFix(this.m_MailStorePath + "Mailboxes\\" + userName));
                text = PathHelper.PathFix(this.m_MailStorePath + "Mailboxes\\" + userName);
            }
            if (!File.Exists(PathHelper.PathFix(text + "\\imap.xml")))
            {
                DataSet dataSet2 = dataSet.Copy();
                dataSet2.Tables["Subscriptions"].Rows.Add(new object[]
                {
                    "Inbox"
                });
                dataSet2.WriteXml(PathHelper.PathFix(text + "\\imap.xml"));
            }
            dataSet.ReadXml(PathHelper.PathFix(text + "\\imap.xml"));
            List<string> list = new List<string>();
            foreach (DataRow dataRow in dataSet.Tables["Subscriptions"].Rows)
            {
                if (!list.Contains(dataRow["Name"].ToString()))
                {
                    list.Add(dataRow["Name"].ToString());
                }
            }
            string[] array = new string[list.Count];
            list.CopyTo(array);
            return array;
        }

        public void SubscribeFolder(string userName, string folder)
        {
            string str = PathHelper.DirectoryExists(PathHelper.PathFix(this.m_MailStorePath + "Mailboxes\\" + userName));
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add("Subscriptions");
            dataSet.Tables["Subscriptions"].Columns.Add("Name");
            if (File.Exists(PathHelper.PathFix(str + "\\imap.xml")))
            {
                dataSet.ReadXml(PathHelper.PathFix(str + "\\imap.xml"));
            }
            Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (DataRow dataRow in dataSet.Tables["Subscriptions"].Rows)
            {
                if (!dictionary.ContainsKey(dataRow["Name"].ToString()))
                {
                    dictionary.Add(dataRow["Name"].ToString(), dataRow["Name"].ToString());
                }
            }
            if (!dictionary.ContainsKey(folder))
            {
                dictionary.Add(folder, folder);
            }
            dataSet.Tables["Subscriptions"].Clear();
            foreach (string current in dictionary.Values)
            {
                DataRow dataRow2 = dataSet.Tables["Subscriptions"].NewRow();
                dataRow2["Name"] = current;
                dataSet.Tables["Subscriptions"].Rows.Add(dataRow2);
            }
            dataSet.WriteXml(PathHelper.PathFix(str + "\\imap.xml"));
        }

        public void UnSubscribeFolder(string userName, string folder)
        {
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(PathHelper.PathFix(this.m_MailStorePath + "Mailboxes\\" + userName + "\\imap.xml"));
            foreach (DataRow dataRow in dataSet.Tables["Subscriptions"].Rows)
            {
                if (dataRow["Name"].ToString().ToLower() == folder.ToLower())
                {
                    dataRow.Delete();
                    break;
                }
            }
            dataSet.WriteXml(PathHelper.PathFix(this.m_MailStorePath + "Mailboxes\\" + userName + "\\imap.xml"));
        }

        public bool SharedFolderRootExists(string rootFolder)
        {
            return this.GetSharedFolderRoot(rootFolder) != null;
        }

        public SharedFolderRoot[] GetSharedFolderRoots()
        {
            this.m_UpdSync.AddMethod();
            SharedFolderRoot[] result;
            try
            {
                List<SharedFolderRoot> list = new List<SharedFolderRoot>();
                foreach (DataRow dataRow in this.dsSharedFolderRoots.Tables["SharedFoldersRoots"].Rows)
                {
                    list.Add(new SharedFolderRoot(dataRow["RootID"].ToString(), Convert.ToBoolean(dataRow["Enabled"]), dataRow["Folder"].ToString(), dataRow["Description"].ToString(), (SharedFolderRootType)Convert.ToInt32(dataRow["RootType"]), dataRow["BoundedUser"].ToString(), dataRow["BoundedFolder"].ToString()));
                }
                result = list.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public void AddSharedFolderRoot(string rootID, bool enabled, string folder, string description, SharedFolderRootType rootType, string boundedUser, string boundedFolder)
        {
            if (rootID == null || rootID == "")
            {
                throw new Exception("Invalid rootID value, rootID can't be '' or null !");
            }
            ArgsValidator.ValidateNotNull(rootID);
            ArgsValidator.ValidateSharedFolderRoot(folder);
            ArgsValidator.ValidateNotNull(description);
            if (rootType == SharedFolderRootType.BoundedRootFolder)
            {
                ArgsValidator.ValidateUserName(boundedUser);
                ArgsValidator.ValidateFolder(boundedFolder);
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (this.ContainsID(this.dsSharedFolderRoots.Tables["SharedFoldersRoots"], "RootID", rootID))
                {
                    throw new Exception("Invalid root ID, specified root ID '" + rootID + "' already exists !");
                }
                if (this.SharedFolderRootExists(folder))
                {
                    throw new ArgumentException("Invalid root folder value, root folder '" + folder + "' already exists !");
                }
                DataRow dataRow = this.dsSharedFolderRoots.Tables["SharedFoldersRoots"].NewRow();
                dataRow["RootID"] = rootID;
                dataRow["Enabled"] = enabled;
                dataRow["Folder"] = folder;
                dataRow["Description"] = description;
                dataRow["RootType"] = (int)rootType;
                dataRow["BoundedUser"] = boundedUser;
                dataRow["BoundedFolder"] = boundedFolder;
                this.dsSharedFolderRoots.Tables["SharedFoldersRoots"].Rows.Add(dataRow);
                this.dsSharedFolderRoots.WriteXml(this.m_DataPath + "SharedFoldersRoots.xml", XmlWriteMode.IgnoreSchema);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void DeleteSharedFolderRoot(string rootID)
        {
            if (rootID == null || rootID == "")
            {
                throw new Exception("Invalid rootID value, rootID can't be '' or null !");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (!this.ContainsID(this.dsSharedFolderRoots.Tables["SharedFoldersRoots"], "RootID", rootID))
                {
                    throw new Exception("Invalid root ID, specified root ID '" + rootID + "' doesn't exist !");
                }
                foreach (DataRow dataRow in this.dsSharedFolderRoots.Tables["SharedFoldersRoots"].Rows)
                {
                    if (dataRow["RootID"].ToString().ToLower() == rootID)
                    {
                        dataRow.Delete();
                        this.dsSharedFolderRoots.WriteXml(this.m_DataPath + "SharedFoldersRoots.xml", XmlWriteMode.IgnoreSchema);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void UpdateSharedFolderRoot(string rootID, bool enabled, string folder, string description, SharedFolderRootType rootType, string boundedUser, string boundedFolder)
        {
            if (rootID == null || rootID == "")
            {
                throw new Exception("Invalid rootID value, rootID can't be '' or null !");
            }
            ArgsValidator.ValidateNotNull(rootID);
            ArgsValidator.ValidateSharedFolderRoot(folder);
            ArgsValidator.ValidateNotNull(description);
            if (rootType == SharedFolderRootType.BoundedRootFolder)
            {
                ArgsValidator.ValidateUserName(boundedUser);
                ArgsValidator.ValidateFolder(boundedFolder);
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (!this.ContainsID(this.dsSharedFolderRoots.Tables["SharedFoldersRoots"], "RootID", rootID))
                {
                    throw new Exception("Invalid root ID, specified root ID '" + rootID + "' doesn't exist !");
                }
                foreach (DataRow dataRow in this.dsSharedFolderRoots.Tables["SharedFoldersRoots"].Rows)
                {
                    if (dataRow["RootID"].ToString().ToLower() == rootID)
                    {
                        if (dataRow["Folder"].ToString().ToLower() != folder.ToLower() && this.SharedFolderRootExists(folder))
                        {
                            throw new Exception("Invalid root folder name, specified root folder '" + folder + "' already exists !");
                        }
                        dataRow["Enabled"] = enabled;
                        dataRow["Folder"] = folder;
                        dataRow["Description"] = description;
                        dataRow["RootType"] = (int)rootType;
                        dataRow["BoundedUser"] = boundedUser;
                        dataRow["BoundedFolder"] = boundedFolder;
                        this.dsSharedFolderRoots.WriteXml(this.m_DataPath + "SharedFoldersRoots.xml", XmlWriteMode.IgnoreSchema);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public DataView GetFolderACL(string accessingUser, string folderOwnerUser, string folder)
        {
            this.m_UpdSync.AddMethod();
            DataView result;
            try
            {
                ArgsValidator.ValidateUserName(folderOwnerUser);
                ArgsValidator.ValidateFolder(folder);
                if (!this.UserExists(folderOwnerUser))
                {
                    throw new Exception("User '" + folderOwnerUser + "' doesn't exist !");
                }
                folder = PathHelper.NormalizeFolder(folder);
                string text = folder;
                LocalXmlStorage.SharedFolderMapInfo sharedFolderMapInfo = this.MapSharedFolder(text);
                if (sharedFolderMapInfo.IsSharedFolder)
                {
                    folderOwnerUser = sharedFolderMapInfo.FolderOnwer;
                    folder = sharedFolderMapInfo.Folder;
                    if (folderOwnerUser == "" || folder == "")
                    {
                        throw new ArgumentException("Specified root folder '" + text + "' isn't accessible !");
                    }
                }
                if (!this.FolderExists(folderOwnerUser + "/" + folder))
                {
                    throw new Exception("Folder '" + folder + "' doesn't exist !");
                }
                if (accessingUser.ToLower() != "system")
                {
                    IMAP_ACL_Flags userACL = this.GetUserACL(folderOwnerUser, folder, accessingUser);
                    if ((userACL & IMAP_ACL_Flags.a) == IMAP_ACL_Flags.None)
                    {
                        throw new InsufficientPermissionsException(string.Concat(new string[]
                        {
                            "Insufficient permissions for folder '",
                            accessingUser,
                            "/",
                            folder,
                            "' !"
                        }));
                    }
                }
                DataView dataView = new DataView(this.dsImapACL.Copy().Tables["ACL"]);
                if (folder != "")
                {
                    dataView.RowFilter = string.Concat(new string[]
                    {
                        "Folder='",
                        folderOwnerUser,
                        "/",
                        folder,
                        "'"
                    });
                }
                result = dataView;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public void DeleteFolderACL(string accessingUser, string folderOwnerUser, string folder, string userOrGroup)
        {
            this.m_UpdSync.BeginUpdate();
            try
            {
                ArgsValidator.ValidateUserName(folderOwnerUser);
                ArgsValidator.ValidateFolder(folder);
                ArgsValidator.ValidateUserName(userOrGroup);
                if (!this.UserExists(folderOwnerUser))
                {
                    throw new Exception("User '" + folderOwnerUser + "' doesn't exist !");
                }
                if (userOrGroup.ToLower() != "anyone" && !this.GroupExists(userOrGroup) && !this.UserExists(userOrGroup))
                {
                    throw new Exception("Invalid userOrGroup value, there is no such user or group '" + userOrGroup + "' !");
                }
                folder = PathHelper.NormalizeFolder(folder);
                string text = folder;
                LocalXmlStorage.SharedFolderMapInfo sharedFolderMapInfo = this.MapSharedFolder(text);
                if (sharedFolderMapInfo.IsSharedFolder)
                {
                    folderOwnerUser = sharedFolderMapInfo.FolderOnwer;
                    folder = sharedFolderMapInfo.Folder;
                    if (folderOwnerUser == "" || folder == "")
                    {
                        throw new ArgumentException("Specified root folder '" + text + "' isn't accessible !");
                    }
                }
                if (!this.FolderExists(folderOwnerUser + "/" + folder))
                {
                    throw new Exception("Folder '" + folder + "' doesn't exist !");
                }
                if (accessingUser.ToLower() != "system")
                {
                    IMAP_ACL_Flags userACL = this.GetUserACL(folderOwnerUser, folder, accessingUser);
                    if ((userACL & IMAP_ACL_Flags.a) == IMAP_ACL_Flags.None)
                    {
                        throw new InsufficientPermissionsException(string.Concat(new string[]
                        {
                            "Insufficient permissions for folder '",
                            accessingUser,
                            "/",
                            folder,
                            "' !"
                        }));
                    }
                }
                using (DataView dataView = new DataView(this.dsImapACL.Tables["ACL"]))
                {
                    dataView.RowFilter = string.Concat(new string[]
                    {
                        "Folder='",
                        folderOwnerUser,
                        "/",
                        folder,
                        "' AND User='",
                        userOrGroup,
                        "'"
                    });
                    if (dataView.Count > 0)
                    {
                        dataView[0].Delete();
                    }
                    this.dsImapACL.WriteXml(this.m_DataPath + "IMAP_ACL.xml", XmlWriteMode.IgnoreSchema);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void SetFolderACL(string accessingUser, string folderOwnerUser, string folder, string userOrGroup, IMAP_Flags_SetType setType, IMAP_ACL_Flags aclFlags)
        {
            this.m_UpdSync.BeginUpdate();
            try
            {
                ArgsValidator.ValidateUserName(folderOwnerUser);
                ArgsValidator.ValidateFolder(folder);
                ArgsValidator.ValidateUserName(userOrGroup);
                if (!this.UserExists(folderOwnerUser))
                {
                    throw new Exception("User '" + folderOwnerUser + "' doesn't exist !");
                }
                if (userOrGroup.ToLower() != "anyone" && !this.GroupExists(userOrGroup) && !this.UserExists(userOrGroup))
                {
                    throw new Exception("Invalid userOrGroup value, there is no such user or group '" + userOrGroup + "' !");
                }
                folder = PathHelper.NormalizeFolder(folder);
                string text = folder;
                LocalXmlStorage.SharedFolderMapInfo sharedFolderMapInfo = this.MapSharedFolder(text);
                if (sharedFolderMapInfo.IsSharedFolder)
                {
                    folderOwnerUser = sharedFolderMapInfo.FolderOnwer;
                    folder = sharedFolderMapInfo.Folder;
                    if (folderOwnerUser == "" || folder == "")
                    {
                        throw new ArgumentException("Specified root folder '" + text + "' isn't accessible !");
                    }
                }
                if (!this.FolderExists(folderOwnerUser + "/" + folder))
                {
                    throw new Exception("Folder '" + folder + "' doesn't exist !");
                }
                if (accessingUser.ToLower() != "system")
                {
                    IMAP_ACL_Flags userACL = this.GetUserACL(folderOwnerUser, folder, accessingUser);
                    if ((userACL & IMAP_ACL_Flags.a) == IMAP_ACL_Flags.None)
                    {
                        throw new InsufficientPermissionsException(string.Concat(new string[]
                        {
                            "Insufficient permissions for folder '",
                            accessingUser,
                            "/",
                            folder,
                            "' !"
                        }));
                    }
                }
                using (DataView dataView = new DataView(this.dsImapACL.Tables["ACL"]))
                {
                    dataView.RowFilter = string.Concat(new string[]
                    {
                        "Folder='",
                        folderOwnerUser,
                        "/",
                        folder,
                        "' AND User='",
                        userOrGroup,
                        "'"
                    });
                    if (dataView.Count == 0)
                    {
                        DataRow dataRow = dataView.Table.NewRow();
                        dataRow["Folder"] = folderOwnerUser + "/" + folder;
                        dataRow["User"] = userOrGroup;
                        dataRow["Permissions"] = IMAP_Utils.ACL_to_String(aclFlags);
                        dataView.Table.Rows.Add(dataRow);
                    }
                    else
                    {
                        IMAP_ACL_Flags iMAP_ACL_Flags = IMAP_Utils.ACL_From_String(dataView[0]["Permissions"].ToString());
                        if (setType == IMAP_Flags_SetType.Replace)
                        {
                            dataView[0]["Permissions"] = IMAP_Utils.ACL_to_String(aclFlags);
                        }
                        else if (setType == IMAP_Flags_SetType.Add)
                        {
                            iMAP_ACL_Flags |= aclFlags;
                            dataView[0]["Permissions"] = IMAP_Utils.ACL_to_String(iMAP_ACL_Flags);
                        }
                        else if (setType == IMAP_Flags_SetType.Remove)
                        {
                            iMAP_ACL_Flags &= ~aclFlags;
                            dataView[0]["Permissions"] = IMAP_Utils.ACL_to_String(iMAP_ACL_Flags);
                        }
                    }
                    this.dsImapACL.WriteXml(this.m_DataPath + "IMAP_ACL.xml", XmlWriteMode.IgnoreSchema);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public IMAP_ACL_Flags GetUserACL(string folderOwnerUser, string folder, string user)
        {
            ArgsValidator.ValidateUserName(folderOwnerUser);
            ArgsValidator.ValidateFolder(folder);
            ArgsValidator.ValidateUserName(user);
            this.m_UpdSync.AddMethod();
            IMAP_ACL_Flags result;
            try
            {
                if (!this.UserExists(folderOwnerUser))
                {
                    throw new Exception("User '" + folderOwnerUser + "' doesn't exist !");
                }
                if (!this.UserExists(user))
                {
                    throw new Exception("User '" + user + "' doesn't exist !");
                }
                folder = PathHelper.NormalizeFolder(folder);
                string text = folder;
                LocalXmlStorage.SharedFolderMapInfo sharedFolderMapInfo = this.MapSharedFolder(text);
                if (sharedFolderMapInfo.IsSharedFolder)
                {
                    folderOwnerUser = sharedFolderMapInfo.FolderOnwer;
                    folder = sharedFolderMapInfo.Folder;
                    if (folderOwnerUser == "" || folder == "")
                    {
                        throw new ArgumentException("Specified root folder '" + text + "' isn't accessible !");
                    }
                }
                IMAP_ACL_Flags iMAP_ACL_Flags = IMAP_ACL_Flags.None;
                DataView dataView = null;
                try
                {
                    dataView = this.GetFolderACL("system", folderOwnerUser, folder);
                }
                catch
                {
                }
                if (dataView != null && dataView.Count > 0)
                {
                    bool flag = false;
                    foreach (DataRowView dataRowView in dataView)
                    {
                        if (this.GroupExists(dataRowView["User"].ToString()) && this.IsUserGroupMember(dataRowView["User"].ToString(), user))
                        {
                            iMAP_ACL_Flags |= IMAP_Utils.ACL_From_String(dataRowView["Permissions"].ToString());
                        }
                        else if (dataRowView["User"].ToString().ToLower() == user.ToLower())
                        {
                            iMAP_ACL_Flags = IMAP_Utils.ACL_From_String(dataRowView["Permissions"].ToString());
                            flag = true;
                        }
                        else if (dataRowView["User"].ToString().ToLower() == "anyone")
                        {
                            iMAP_ACL_Flags = IMAP_Utils.ACL_From_String(dataRowView["Permissions"].ToString());
                        }
                    }
                    if (!flag && user.ToLower() == folderOwnerUser.ToLower())
                    {
                        iMAP_ACL_Flags = IMAP_ACL_Flags.All;
                    }
                }
                else if (user.ToLower() == folderOwnerUser.ToLower())
                {
                    iMAP_ACL_Flags = IMAP_ACL_Flags.All;
                }
                else
                {
                    while (folder.LastIndexOf('/') > -1)
                    {
                        folder = folder.Substring(0, folder.LastIndexOf('/'));
                        dataView = this.GetFolderACL("system", folderOwnerUser, folder);
                        if (dataView.Count > 0)
                        {
                            IEnumerator enumerator2 = dataView.GetEnumerator();
                            try
                            {
                                while (enumerator2.MoveNext())
                                {
                                    DataRowView dataRowView2 = (DataRowView)enumerator2.Current;
                                    string groupName = dataRowView2["User"].ToString();
                                    if (this.GroupExists(groupName) && this.IsUserGroupMember(dataRowView2["User"].ToString(), user))
                                    {
                                        iMAP_ACL_Flags |= IMAP_Utils.ACL_From_String(dataRowView2["Permissions"].ToString());
                                    }
                                    else if (dataRowView2["User"].ToString().ToLower() == user.ToLower())
                                    {
                                        iMAP_ACL_Flags |= IMAP_Utils.ACL_From_String(dataRowView2["Permissions"].ToString());
                                    }
                                    else if (dataRowView2["User"].ToString().ToLower() == "anyone")
                                    {
                                        iMAP_ACL_Flags |= IMAP_Utils.ACL_From_String(dataRowView2["Permissions"].ToString());
                                    }
                                }
                                break;
                            }
                            finally
                            {
                                IDisposable disposable2 = enumerator2 as IDisposable;
                                if (disposable2 != null)
                                {
                                    disposable2.Dispose();
                                }
                            }
                        }
                    }
                }
                result = iMAP_ACL_Flags;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public void CreateUserDefaultFolders(string userName)
        {
            foreach (DataRowView dataRowView in this.GetUsersDefaultFolders())
            {
                if (!this.FolderExists(userName + "/" + dataRowView["FolderName"].ToString()))
                {
                    this.CreateFolder("system", userName, dataRowView["FolderName"].ToString());
                    this.SubscribeFolder(userName, dataRowView["FolderName"].ToString());
                }
            }
        }

        public DataView GetUsersDefaultFolders()
        {
            this.m_UpdSync.AddMethod();
            DataView result;
            try
            {
                if (this.dsUsersDefaultFolders.Tables["UsersDefaultFolders"].Rows.Count == 0)
                {
                    DataRow dataRow = this.dsUsersDefaultFolders.Tables["UsersDefaultFolders"].NewRow();
                    dataRow["FolderName"] = "Inbox";
                    dataRow["Permanent"] = true;
                    this.dsUsersDefaultFolders.Tables["UsersDefaultFolders"].Rows.Add(dataRow);
                }
                result = new DataView(this.dsUsersDefaultFolders.Copy().Tables["UsersDefaultFolders"]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public void AddUsersDefaultFolder(string folderName, bool permanent)
        {
            ArgsValidator.ValidateFolder(folderName);
            this.m_UpdSync.BeginUpdate();
            try
            {
                foreach (DataRow dataRow in this.dsUsersDefaultFolders.Tables["UsersDefaultFolders"].Rows)
                {
                    if (dataRow["FolderName"].ToString().ToLower() == folderName.ToLower())
                    {
                        throw new Exception("Users default folder with specified name '" + folderName + "' already exists !");
                    }
                }
                DataRow dataRow2 = this.dsUsersDefaultFolders.Tables["UsersDefaultFolders"].NewRow();
                dataRow2["FolderName"] = folderName;
                dataRow2["Permanent"] = permanent;
                this.dsUsersDefaultFolders.Tables["UsersDefaultFolders"].Rows.Add(dataRow2);
                this.dsUsersDefaultFolders.WriteXml(this.m_DataPath + "UsersDefaultFolders.xml", XmlWriteMode.IgnoreSchema);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void DeleteUsersDefaultFolder(string folderName)
        {
            ArgsValidator.ValidateFolder(folderName);
            this.m_UpdSync.BeginUpdate();
            try
            {
                foreach (DataRow dataRow in this.dsUsersDefaultFolders.Tables["UsersDefaultFolders"].Rows)
                {
                    if (dataRow["FolderName"].ToString().ToLower() == folderName.ToLower())
                    {
                        dataRow.Delete();
                        this.dsUsersDefaultFolders.WriteXml(this.m_DataPath + "UsersDefaultFolders.xml", XmlWriteMode.IgnoreSchema);
                        return;
                    }
                }
                throw new Exception("Users default folder with specified name '" + folderName + "' doesn't exists !");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public long GetMailboxSize(string userName)
        {
            try
            {
                using (FileStream fileStream = File.OpenRead(PathHelper.PathFix(this.m_MailStorePath + "Mailboxes\\" + userName + "\\_mailbox_size")))
                {
                    byte[] array = new byte[fileStream.Length];
                    fileStream.Read(array, 0, array.Length);
                    long num = Convert.ToInt64(Encoding.ASCII.GetString(array).Trim());
                    if (num < 0L)
                    {
                        num = 0L;
                    }
                    return num;
                }
            }
            catch (IOException ex)
            {
                string arg_6E_0 = ex.Message;
                this.ChangeMailboxSize(userName, 0L);
            }
            return 0L;
        }

        public DataTable GetRecycleBinSettings()
        {
            return this.dsRecycleBinSettings.Tables["RecycleBinSettings"];
        }

        public void UpdateRecycleBinSettings(bool deleteToRecycleBin, int deleteMessagesAfter)
        {
            this.dsRecycleBinSettings.Tables["RecycleBinSettings"].Rows[0]["DeleteToRecycleBin"] = deleteToRecycleBin;
            this.dsRecycleBinSettings.Tables["RecycleBinSettings"].Rows[0]["DeleteMessagesAfter"] = deleteMessagesAfter;
            this.dsRecycleBinSettings.WriteXml(this.m_DataPath + "RecycleBinSettings.xml");
        }

        public DataView GetRecycleBinMessagesInfo(string user, DateTime startDate, DateTime endDate)
        {
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add("MessagesInfo");
            dataSet.Tables["MessagesInfo"].Columns.Add("MessageID");
            dataSet.Tables["MessagesInfo"].Columns.Add("DeleteTime", typeof(DateTime));
            dataSet.Tables["MessagesInfo"].Columns.Add("User");
            dataSet.Tables["MessagesInfo"].Columns.Add("Folder");
            dataSet.Tables["MessagesInfo"].Columns.Add("Size");
            dataSet.Tables["MessagesInfo"].Columns.Add("Envelope");
            foreach (RecycleBinMessageInfo current in RecycleBinManager.GetMessagesInfo(user, startDate, endDate))
            {
                DataRow dataRow = dataSet.Tables["MessagesInfo"].NewRow();
                dataRow["MessageID"] = current.MessageID;
                dataRow["DeleteTime"] = current.DeleteTime;
                dataRow["User"] = current.User;
                dataRow["Folder"] = current.Folder;
                dataRow["Size"] = current.Size;
                dataRow["Envelope"] = current.Envelope;
                dataSet.Tables["MessagesInfo"].Rows.Add(dataRow);
            }
            return dataSet.Tables["MessagesInfo"].DefaultView;
        }

        public Stream GetRecycleBinMessage(string messageID)
        {
            return RecycleBinManager.GetRecycleBinMessage(messageID);
        }

        public void DeleteRecycleBinMessage(string messageID)
        {
            RecycleBinManager.DeleteRecycleBinMessage(messageID);
        }

        public void RestoreRecycleBinMessage(string messageID)
        {
            RecycleBinManager.RestoreFromRecycleBin(messageID, this);
        }

        public DataView GetSecurityList()
        {
            this.m_UpdSync.AddMethod();
            DataView result;
            try
            {
                result = new DataView(this.dsSecurity.Copy().Tables["IPSecurity"]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public void AddSecurityEntry(string id, bool enabled, string description, ServiceType service, IPSecurityAction action, IPAddress startIP, IPAddress endIP)
        {
            if (id.Length == 0)
            {
                throw new Exception("You must specify id");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (this.ContainsID(this.dsSecurity.Tables["IPSecurity"], "ID", id))
                {
                    throw new Exception("Security entry with specified ID '" + id + "' already exists !");
                }
                DataRow dataRow = this.dsSecurity.Tables["IPSecurity"].NewRow();
                dataRow["ID"] = id;
                dataRow["Enabled"] = enabled;
                dataRow["Description"] = description;
                dataRow["Service"] = service;
                dataRow["Action"] = action;
                dataRow["StartIP"] = startIP.ToString();
                dataRow["EndIP"] = endIP.ToString();
                this.dsSecurity.Tables["IPSecurity"].Rows.Add(dataRow);
                this.dsSecurity.WriteXml(this.m_DataPath + "IPSecurity.xml", XmlWriteMode.IgnoreSchema);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void DeleteSecurityEntry(string id)
        {
            this.m_UpdSync.BeginUpdate();
            try
            {
                foreach (DataRow dataRow in this.dsSecurity.Tables["IPSecurity"].Rows)
                {
                    if (dataRow["ID"].ToString().ToLower() == id)
                    {
                        dataRow.Delete();
                        this.dsSecurity.WriteXml(this.m_DataPath + "IPSecurity.xml", XmlWriteMode.IgnoreSchema);
                        return;
                    }
                }
                throw new Exception("Security entry with specified ID '" + id + "' doesn't exists !");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void UpdateSecurityEntry(string id, bool enabled, string description, ServiceType service, IPSecurityAction action, IPAddress startIP, IPAddress endIP)
        {
            this.m_UpdSync.BeginUpdate();
            try
            {
                foreach (DataRow dataRow in this.dsSecurity.Tables["IPSecurity"].Rows)
                {
                    if (dataRow["ID"].ToString().ToLower() == id)
                    {
                        dataRow["ID"] = id;
                        dataRow["Enabled"] = enabled;
                        dataRow["Description"] = description;
                        dataRow["Service"] = service;
                        dataRow["Action"] = action;
                        dataRow["StartIP"] = startIP.ToString();
                        dataRow["EndIP"] = endIP.ToString();
                        this.dsSecurity.WriteXml(this.m_DataPath + "IPSecurity.xml", XmlWriteMode.IgnoreSchema);
                        return;
                    }
                }
                throw new Exception("Security entry with specified ID '" + id + "' doesn't exist !");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public DataView GetFilters()
        {
            this.m_UpdSync.AddMethod();
            DataView result;
            try
            {
                result = new DataView(this.dsFilters.Copy().Tables["SmtpFilters"]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        public void AddFilter(string filterID, string description, string type, string assembly, string className, long cost, bool enabled)
        {
            if (filterID.Length == 0)
            {
                throw new Exception("You must specify filterID");
            }
            this.m_UpdSync.BeginUpdate();
            try
            {
                if (this.ContainsID(this.dsFilters.Tables["SmtpFilters"], "FilterID", filterID))
                {
                    throw new Exception("Filter with specified ID '" + filterID + "' already exists !");
                }
                DataRow dataRow = this.dsFilters.Tables["SmtpFilters"].NewRow();
                dataRow["FilterID"] = filterID;
                dataRow["Description"] = description;
                dataRow["Type"] = type;
                dataRow["Assembly"] = assembly;
                dataRow["ClassName"] = className;
                dataRow["Cost"] = cost;
                dataRow["Enabled"] = enabled;
                this.dsFilters.Tables["SmtpFilters"].Rows.Add(dataRow);
                this.dsFilters.WriteXml(this.m_DataPath + "Filters.xml", XmlWriteMode.IgnoreSchema);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void DeleteFilter(string filterID)
        {
            this.m_UpdSync.BeginUpdate();
            try
            {
                using (DataView dataView = new DataView(this.dsFilters.Tables["SmtpFilters"]))
                {
                    dataView.RowFilter = "FilterID='" + filterID + "'";
                    if (dataView.Count > 0)
                    {
                        dataView[0].Delete();
                    }
                    this.dsFilters.WriteXml(this.m_DataPath + "Filters.xml", XmlWriteMode.IgnoreSchema);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public void UpdateFilter(string filterID, string description, string type, string assembly, string className, long cost, bool enabled)
        {
            this.m_UpdSync.BeginUpdate();
            try
            {
                try
                {
                    if (this.ContainsID(this.dsFilters.Tables["SmtpFilters"], "FilterID", filterID))
                    {
                        using (DataView dataView = new DataView(this.dsFilters.Tables["SmtpFilters"]))
                        {
                            dataView.RowFilter = "FilterID='" + filterID + "'";
                            if (dataView.Count > 0)
                            {
                                dataView[0]["Description"] = description;
                                dataView[0]["Type"] = type;
                                dataView[0]["Assembly"] = assembly;
                                dataView[0]["ClassName"] = className;
                                dataView[0]["Cost"] = cost;
                                dataView[0]["Enabled"] = enabled;
                            }
                            this.dsFilters.WriteXml(this.m_DataPath + "Filters.xml", XmlWriteMode.IgnoreSchema);
                            goto IL_123;
                        }
                        goto IL_10D;
                        IL_123:
                        goto IL_128;
                    }
                    IL_10D:
                    throw new Exception("Filter with specified ID '" + filterID + "' doesn't exist !");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                IL_128:;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        public DataRow GetSettings()
        {
            DataSet dataSet = new DataSet();
            PathHelper.CreateSettingsSchema(dataSet);
            if (File.Exists(this.m_DataPath + "Settings.xml"))
            {
                dataSet.ReadXml(this.m_DataPath + "Settings.xml");
            }
            else if (dataSet.Tables["Settings"].Rows.Count == 0)
            {
                dataSet.Tables["Settings"].Rows.Add(dataSet.Tables["Settings"].NewRow());
            }
            foreach (DataRow dataRow in dataSet.Tables["Settings"].Rows)
            {
                foreach (DataColumn dataColumn in dataSet.Tables["Settings"].Columns)
                {
                    if (dataRow.IsNull(dataColumn.ColumnName))
                    {
                        dataRow[dataColumn.ColumnName] = dataColumn.DefaultValue;
                    }
                }
            }
            if (dataSet.Tables["SMTP_Bindings"].Rows.Count == 0)
            {
                dataSet.Tables["SMTP_Bindings"].Rows.Add(dataSet.Tables["SMTP_Bindings"].NewRow());
            }
            foreach (DataRow dataRow2 in dataSet.Tables["SMTP_Bindings"].Rows)
            {
                foreach (DataColumn dataColumn2 in dataSet.Tables["SMTP_Bindings"].Columns)
                {
                    if (dataRow2.IsNull(dataColumn2.ColumnName))
                    {
                        dataRow2[dataColumn2.ColumnName] = dataColumn2.DefaultValue;
                    }
                }
            }
            if (dataSet.Tables["POP3_Bindings"].Rows.Count == 0)
            {
                dataSet.Tables["POP3_Bindings"].Rows.Add(dataSet.Tables["POP3_Bindings"].NewRow());
            }
            foreach (DataRow dataRow3 in dataSet.Tables["POP3_Bindings"].Rows)
            {
                foreach (DataColumn dataColumn3 in dataSet.Tables["POP3_Bindings"].Columns)
                {
                    if (dataRow3.IsNull(dataColumn3.ColumnName))
                    {
                        dataRow3[dataColumn3.ColumnName] = dataColumn3.DefaultValue;
                    }
                }
            }
            if (dataSet.Tables["IMAP_Bindings"].Rows.Count == 0)
            {
                dataSet.Tables["IMAP_Bindings"].Rows.Add(dataSet.Tables["IMAP_Bindings"].NewRow());
            }
            foreach (DataRow dataRow4 in dataSet.Tables["IMAP_Bindings"].Rows)
            {
                foreach (DataColumn dataColumn4 in dataSet.Tables["IMAP_Bindings"].Columns)
                {
                    if (dataRow4.IsNull(dataColumn4.ColumnName))
                    {
                        dataRow4[dataColumn4.ColumnName] = dataColumn4.DefaultValue;
                    }
                }
            }
            return dataSet.Tables["Settings"].Rows[0];
        }

        public void UpdateSettings(DataRow settings)
        {
            settings["SettingsDate"] = DateTime.Now;
            settings.Table.DataSet.WriteXml(this.m_DataPath + "Settings.xml", XmlWriteMode.IgnoreSchema);
        }

        private void LoadDomains()
        {
            DateTime lastWriteTime = File.GetLastWriteTime(this.m_DataPath + "Domains.xml");
            if (DateTime.Compare(lastWriteTime, this.m_DomainsDate) == 0)
            {
                return;
            }
            this.m_DomainsDate = lastWriteTime;
            this.dsDomains.Clear();
            if (!this.dsDomains.Tables.Contains("Domains"))
            {
                this.dsDomains.Tables.Add("Domains");
            }
            if (!this.dsDomains.Tables["Domains"].Columns.Contains("DomainID"))
            {
                this.dsDomains.Tables["Domains"].Columns.Add("DomainID", Type.GetType("System.String"));
            }
            if (!this.dsDomains.Tables["Domains"].Columns.Contains("DomainName"))
            {
                this.dsDomains.Tables["Domains"].Columns.Add("DomainName", Type.GetType("System.String"));
            }
            if (!this.dsDomains.Tables["Domains"].Columns.Contains("Description"))
            {
                this.dsDomains.Tables["Domains"].Columns.Add("Description", Type.GetType("System.String"));
            }
            if (File.Exists(this.m_DataPath + "Domains.xml"))
            {
                this.dsDomains.ReadXml(this.m_DataPath + "Domains.xml");
            }
        }

        private void LoadUsers()
        {
            DateTime lastWriteTime = File.GetLastWriteTime(this.m_DataPath + "Users.xml");
            if (DateTime.Compare(lastWriteTime, this.m_UsersDate) == 0)
            {
                return;
            }
            this.m_UsersDate = lastWriteTime;
            this.dsUsers.Clear();
            if (!this.dsUsers.Tables.Contains("Users"))
            {
                this.dsUsers.Tables.Add("Users");
            }
            if (!this.dsUsers.Tables["Users"].Columns.Contains("UserID"))
            {
                this.dsUsers.Tables["Users"].Columns.Add("UserID", Type.GetType("System.String"));
            }
            if (!this.dsUsers.Tables["Users"].Columns.Contains("FullName"))
            {
                this.dsUsers.Tables["Users"].Columns.Add("FullName", Type.GetType("System.String"));
            }
            if (!this.dsUsers.Tables["Users"].Columns.Contains("UserName"))
            {
                this.dsUsers.Tables["Users"].Columns.Add("UserName", Type.GetType("System.String"));
            }
            if (!this.dsUsers.Tables["Users"].Columns.Contains("Password"))
            {
                this.dsUsers.Tables["Users"].Columns.Add("Password", Type.GetType("System.String"));
            }
            if (!this.dsUsers.Tables["Users"].Columns.Contains("Description"))
            {
                this.dsUsers.Tables["Users"].Columns.Add("Description", Type.GetType("System.String"));
            }
            if (!this.dsUsers.Tables["Users"].Columns.Contains("DomainName"))
            {
                this.dsUsers.Tables["Users"].Columns.Add("DomainName", Type.GetType("System.String"));
            }
            if (!this.dsUsers.Tables["Users"].Columns.Contains("Mailbox_Size"))
            {
                this.dsUsers.Tables["Users"].Columns.Add("Mailbox_Size", Type.GetType("System.Int64"));
                this.dsUsers.Tables["Users"].Columns["Mailbox_Size"].DefaultValue = 20;
                this.dsUsers.Tables["Users"].Columns["Mailbox_Size"].AllowDBNull = false;
            }
            if (!this.dsUsers.Tables["Users"].Columns.Contains("Enabled"))
            {
                this.dsUsers.Tables["Users"].Columns.Add("Enabled", Type.GetType("System.Boolean"));
                this.dsUsers.Tables["Users"].Columns["Enabled"].DefaultValue = true;
                this.dsUsers.Tables["Users"].Columns["Enabled"].AllowDBNull = false;
            }
            if (!this.dsUsers.Tables["Users"].Columns.Contains("Permissions"))
            {
                this.dsUsers.Tables["Users"].Columns.Add("Permissions", Type.GetType("System.Int32"));
                this.dsUsers.Tables["Users"].Columns["Permissions"].DefaultValue = 65535;
            }
            if (!this.dsUsers.Tables["Users"].Columns.Contains("CreationTime"))
            {
                this.dsUsers.Tables["Users"].Columns.Add("CreationTime", Type.GetType("System.DateTime"));
                this.dsUsers.Tables["Users"].Columns["CreationTime"].DefaultValue = DateTime.Now;
            }
            if (File.Exists(this.m_DataPath + "Users.xml"))
            {
                this.dsUsers.ReadXml(this.m_DataPath + "Users.xml");
            }
            this.LoadDataSetDefaults(this.dsUsers);
        }

        private void LoadUserAddresses()
        {
            DateTime lastWriteTime = File.GetLastWriteTime(this.m_DataPath + "UserAddresses.xml");
            if (DateTime.Compare(lastWriteTime, this.m_UserAddressesDate) == 0)
            {
                return;
            }
            this.m_UserAddressesDate = lastWriteTime;
            this.dsUserAddresses.Clear();
            if (!this.dsUserAddresses.Tables.Contains("UserAddresses"))
            {
                this.dsUserAddresses.Tables.Add("UserAddresses");
            }
            if (!this.dsUserAddresses.Tables["UserAddresses"].Columns.Contains("AddressID"))
            {
                this.dsUserAddresses.Tables["UserAddresses"].Columns.Add("AddressID", Type.GetType("System.String"));
            }
            if (!this.dsUserAddresses.Tables["UserAddresses"].Columns.Contains("UserID"))
            {
                this.dsUserAddresses.Tables["UserAddresses"].Columns.Add("UserID", Type.GetType("System.String"));
            }
            if (!this.dsUserAddresses.Tables["UserAddresses"].Columns.Contains("Address"))
            {
                this.dsUserAddresses.Tables["UserAddresses"].Columns.Add("Address", Type.GetType("System.String"));
            }
            if (File.Exists(this.m_DataPath + "UserAddresses.xml"))
            {
                this.dsUserAddresses.ReadXml(this.m_DataPath + "UserAddresses.xml");
            }
        }

        private void LoadUserRemoteServers()
        {
            DateTime lastWriteTime = File.GetLastWriteTime(this.m_DataPath + "UserRemoteServers.xml");
            if (DateTime.Compare(lastWriteTime, this.m_UsersRemoteServers) == 0)
            {
                return;
            }
            this.m_UsersRemoteServers = lastWriteTime;
            this.dsUserRemoteServers.Clear();
            if (!this.dsUserRemoteServers.Tables.Contains("UserRemoteServers"))
            {
                this.dsUserRemoteServers.Tables.Add("UserRemoteServers");
            }
            if (!this.dsUserRemoteServers.Tables["UserRemoteServers"].Columns.Contains("ServerID"))
            {
                this.dsUserRemoteServers.Tables["UserRemoteServers"].Columns.Add("ServerID", Type.GetType("System.String"));
            }
            if (!this.dsUserRemoteServers.Tables["UserRemoteServers"].Columns.Contains("UserID"))
            {
                this.dsUserRemoteServers.Tables["UserRemoteServers"].Columns.Add("UserID", Type.GetType("System.String"));
            }
            if (!this.dsUserRemoteServers.Tables["UserRemoteServers"].Columns.Contains("Description"))
            {
                this.dsUserRemoteServers.Tables["UserRemoteServers"].Columns.Add("Description", Type.GetType("System.String"));
            }
            if (!this.dsUserRemoteServers.Tables["UserRemoteServers"].Columns.Contains("RemoteServer"))
            {
                this.dsUserRemoteServers.Tables["UserRemoteServers"].Columns.Add("RemoteServer", Type.GetType("System.String"));
            }
            if (!this.dsUserRemoteServers.Tables["UserRemoteServers"].Columns.Contains("RemotePort"))
            {
                this.dsUserRemoteServers.Tables["UserRemoteServers"].Columns.Add("RemotePort", Type.GetType("System.String"));
            }
            if (!this.dsUserRemoteServers.Tables["UserRemoteServers"].Columns.Contains("RemoteUserName"))
            {
                this.dsUserRemoteServers.Tables["UserRemoteServers"].Columns.Add("RemoteUserName", Type.GetType("System.String"));
            }
            if (!this.dsUserRemoteServers.Tables["UserRemoteServers"].Columns.Contains("RemotePassword"))
            {
                this.dsUserRemoteServers.Tables["UserRemoteServers"].Columns.Add("RemotePassword", Type.GetType("System.String"));
            }
            if (!this.dsUserRemoteServers.Tables["UserRemoteServers"].Columns.Contains("UseSSL"))
            {
                this.dsUserRemoteServers.Tables["UserRemoteServers"].Columns.Add("UseSSL", Type.GetType("System.Boolean")).DefaultValue = false;
            }
            if (!this.dsUserRemoteServers.Tables["UserRemoteServers"].Columns.Contains("Enabled"))
            {
                this.dsUserRemoteServers.Tables["UserRemoteServers"].Columns.Add("Enabled", Type.GetType("System.Boolean")).DefaultValue = true;
            }
            if (File.Exists(this.m_DataPath + "UserRemoteServers.xml"))
            {
                this.dsUserRemoteServers.ReadXml(this.m_DataPath + "UserRemoteServers.xml");
            }
            this.LoadDataSetDefaults(this.dsUserRemoteServers);
        }

        private void LoadUserMessageRules()
        {
            DateTime lastWriteTime = File.GetLastWriteTime(this.m_DataPath + "UserMessageRules.xml");
            if (DateTime.Compare(lastWriteTime, this.m_UserMessageRules) == 0)
            {
                return;
            }
            this.m_UserMessageRules = lastWriteTime;
            this.dsUserMessageRules.Clear();
            if (!this.dsUserMessageRules.Tables.Contains("UserMessageRules"))
            {
                this.dsUserMessageRules.Tables.Add("UserMessageRules");
            }
            if (!this.dsUserMessageRules.Tables["UserMessageRules"].Columns.Contains("UserID"))
            {
                this.dsUserMessageRules.Tables["UserMessageRules"].Columns.Add("UserID", Type.GetType("System.String"));
            }
            if (!this.dsUserMessageRules.Tables["UserMessageRules"].Columns.Contains("RuleID"))
            {
                this.dsUserMessageRules.Tables["UserMessageRules"].Columns.Add("RuleID", Type.GetType("System.String"));
            }
            if (!this.dsUserMessageRules.Tables["UserMessageRules"].Columns.Contains("Cost"))
            {
                this.dsUserMessageRules.Tables["UserMessageRules"].Columns.Add("Cost", Type.GetType("System.Int64"));
            }
            if (!this.dsUserMessageRules.Tables["UserMessageRules"].Columns.Contains("Enabled"))
            {
                this.dsUserMessageRules.Tables["UserMessageRules"].Columns.Add("Enabled", Type.GetType("System.Boolean"));
            }
            if (!this.dsUserMessageRules.Tables["UserMessageRules"].Columns.Contains("CheckNextRuleIf"))
            {
                this.dsUserMessageRules.Tables["UserMessageRules"].Columns.Add("CheckNextRuleIf", Type.GetType("System.Int32"));
            }
            if (!this.dsUserMessageRules.Tables["UserMessageRules"].Columns.Contains("Description"))
            {
                this.dsUserMessageRules.Tables["UserMessageRules"].Columns.Add("Description", Type.GetType("System.String"));
            }
            if (!this.dsUserMessageRules.Tables["UserMessageRules"].Columns.Contains("MatchExpression"))
            {
                this.dsUserMessageRules.Tables["UserMessageRules"].Columns.Add("MatchExpression", Type.GetType("System.String"));
            }
            if (File.Exists(this.m_DataPath + "UserMessageRules.xml"))
            {
                this.dsUserMessageRules.ReadXml(this.m_DataPath + "UserMessageRules.xml");
            }
        }

        private void LoadUserMessageRuleActions()
        {
            DateTime lastWriteTime = File.GetLastWriteTime(this.m_DataPath + "UserMessageRuleActions.xml");
            if (DateTime.Compare(lastWriteTime, this.m_UserMessageRuleActions) == 0)
            {
                return;
            }
            this.m_UserMessageRuleActions = lastWriteTime;
            this.dsUserMessageRuleActions.Clear();
            if (!this.dsUserMessageRuleActions.Tables.Contains("UserMessageRuleActions"))
            {
                this.dsUserMessageRuleActions.Tables.Add("UserMessageRuleActions");
            }
            if (!this.dsUserMessageRuleActions.Tables["UserMessageRuleActions"].Columns.Contains("UserID"))
            {
                this.dsUserMessageRuleActions.Tables["UserMessageRuleActions"].Columns.Add("UserID", Type.GetType("System.String"));
            }
            if (!this.dsUserMessageRuleActions.Tables["UserMessageRuleActions"].Columns.Contains("RuleID"))
            {
                this.dsUserMessageRuleActions.Tables["UserMessageRuleActions"].Columns.Add("RuleID", Type.GetType("System.String"));
            }
            if (!this.dsUserMessageRuleActions.Tables["UserMessageRuleActions"].Columns.Contains("ActionID"))
            {
                this.dsUserMessageRuleActions.Tables["UserMessageRuleActions"].Columns.Add("ActionID", Type.GetType("System.String"));
            }
            if (!this.dsUserMessageRuleActions.Tables["UserMessageRuleActions"].Columns.Contains("Description"))
            {
                this.dsUserMessageRuleActions.Tables["UserMessageRuleActions"].Columns.Add("Description", Type.GetType("System.String"));
            }
            if (!this.dsUserMessageRuleActions.Tables["UserMessageRuleActions"].Columns.Contains("ActionType"))
            {
                this.dsUserMessageRuleActions.Tables["UserMessageRuleActions"].Columns.Add("ActionType", Type.GetType("System.Int32"));
            }
            if (!this.dsUserMessageRuleActions.Tables["UserMessageRuleActions"].Columns.Contains("ActionData"))
            {
                this.dsUserMessageRuleActions.Tables["UserMessageRuleActions"].Columns.Add("ActionData", typeof(byte[]));
            }
            if (File.Exists(this.m_DataPath + "UserMessageRuleActions.xml"))
            {
                this.dsUserMessageRuleActions.ReadXml(this.m_DataPath + "UserMessageRuleActions.xml");
            }
        }

        private void LoadGroups()
        {
            DateTime lastWriteTime = File.GetLastWriteTime(this.m_DataPath + "Groups.xml");
            if (DateTime.Compare(lastWriteTime, this.m_GroupsDate) == 0)
            {
                return;
            }
            this.m_GroupsDate = lastWriteTime;
            this.dsGroups.Clear();
            if (!this.dsGroups.Tables.Contains("Groups"))
            {
                this.dsGroups.Tables.Add("Groups");
            }
            if (!this.dsGroups.Tables["Groups"].Columns.Contains("GroupID"))
            {
                this.dsGroups.Tables["Groups"].Columns.Add("GroupID");
            }
            if (!this.dsGroups.Tables["Groups"].Columns.Contains("GroupName"))
            {
                this.dsGroups.Tables["Groups"].Columns.Add("GroupName");
            }
            if (!this.dsGroups.Tables["Groups"].Columns.Contains("Description"))
            {
                this.dsGroups.Tables["Groups"].Columns.Add("Description");
            }
            if (!this.dsGroups.Tables["Groups"].Columns.Contains("Enabled"))
            {
                this.dsGroups.Tables["Groups"].Columns.Add("Enabled");
            }
            if (!File.Exists(this.m_DataPath + "Groups.xml"))
            {
                this.dsGroups.WriteXml(this.m_DataPath + "Groups.xml");
                return;
            }
            this.dsGroups.ReadXml(this.m_DataPath + "Groups.xml");
        }

        private void LoadGroupMembers()
        {
            DateTime lastWriteTime = File.GetLastWriteTime(this.m_DataPath + "GroupMembers.xml");
            if (DateTime.Compare(lastWriteTime, this.m_GroupMembersDate) == 0)
            {
                return;
            }
            this.m_GroupMembersDate = lastWriteTime;
            this.dsGroupMembers.Clear();
            if (!this.dsGroupMembers.Tables.Contains("GroupMembers"))
            {
                this.dsGroupMembers.Tables.Add("GroupMembers");
            }
            if (!this.dsGroupMembers.Tables["GroupMembers"].Columns.Contains("GroupID"))
            {
                this.dsGroupMembers.Tables["GroupMembers"].Columns.Add("GroupID");
            }
            if (!this.dsGroupMembers.Tables["GroupMembers"].Columns.Contains("UserOrGroup"))
            {
                this.dsGroupMembers.Tables["GroupMembers"].Columns.Add("UserOrGroup");
            }
            if (!File.Exists(this.m_DataPath + "GroupMembers.xml"))
            {
                this.dsGroupMembers.WriteXml(this.m_DataPath + "GroupMembers.xml");
                return;
            }
            this.dsGroupMembers.ReadXml(this.m_DataPath + "GroupMembers.xml");
        }

        private void LoadMailingLists()
        {
            DateTime lastWriteTime = File.GetLastWriteTime(this.m_DataPath + "MailingLists.xml");
            if (DateTime.Compare(lastWriteTime, this.m_MailingListsDate) == 0)
            {
                return;
            }
            this.m_MailingListsDate = lastWriteTime;
            this.dsMailingLists.Clear();
            if (!this.dsMailingLists.Tables.Contains("MailingLists"))
            {
                this.dsMailingLists.Tables.Add("MailingLists");
            }
            if (!this.dsMailingLists.Tables["MailingLists"].Columns.Contains("MailingListID"))
            {
                this.dsMailingLists.Tables["MailingLists"].Columns.Add("MailingListID", Type.GetType("System.String"));
            }
            if (!this.dsMailingLists.Tables["MailingLists"].Columns.Contains("DomainName"))
            {
                this.dsMailingLists.Tables["MailingLists"].Columns.Add("DomainName", Type.GetType("System.String"));
            }
            if (!this.dsMailingLists.Tables["MailingLists"].Columns.Contains("MailingListName"))
            {
                this.dsMailingLists.Tables["MailingLists"].Columns.Add("MailingListName", Type.GetType("System.String"));
            }
            if (!this.dsMailingLists.Tables["MailingLists"].Columns.Contains("Description"))
            {
                this.dsMailingLists.Tables["MailingLists"].Columns.Add("Description", Type.GetType("System.String"));
            }
            if (!this.dsMailingLists.Tables["MailingLists"].Columns.Contains("Enabled"))
            {
                this.dsMailingLists.Tables["MailingLists"].Columns.Add("Enabled", Type.GetType("System.String")).DefaultValue = true;
            }
            if (File.Exists(this.m_DataPath + "MailingLists.xml"))
            {
                this.dsMailingLists.ReadXml(this.m_DataPath + "MailingLists.xml");
            }
            foreach (DataRow dataRow in this.dsMailingLists.Tables["MailingLists"].Rows)
            {
                foreach (DataColumn dataColumn in this.dsMailingLists.Tables["MailingLists"].Columns)
                {
                    if (dataRow.IsNull(dataColumn.ColumnName))
                    {
                        dataRow[dataColumn.ColumnName] = dataColumn.DefaultValue;
                    }
                }
            }
        }

        private void LoadMailingListAddresses()
        {
            DateTime lastWriteTime = File.GetLastWriteTime(this.m_DataPath + "MailingListAddresses.xml");
            if (DateTime.Compare(lastWriteTime, this.m_MailingListAddressesDate) == 0)
            {
                return;
            }
            this.m_MailingListAddressesDate = lastWriteTime;
            this.dsMailingListAddresses.Clear();
            if (!this.dsMailingListAddresses.Tables.Contains("MailingListAddresses"))
            {
                this.dsMailingListAddresses.Tables.Add("MailingListAddresses");
            }
            if (!this.dsMailingListAddresses.Tables["MailingListAddresses"].Columns.Contains("AddressID"))
            {
                this.dsMailingListAddresses.Tables["MailingListAddresses"].Columns.Add("AddressID", Type.GetType("System.String"));
            }
            if (!this.dsMailingListAddresses.Tables["MailingListAddresses"].Columns.Contains("MailingListID"))
            {
                this.dsMailingListAddresses.Tables["MailingListAddresses"].Columns.Add("MailingListID", Type.GetType("System.String"));
            }
            if (!this.dsMailingListAddresses.Tables["MailingListAddresses"].Columns.Contains("Address"))
            {
                this.dsMailingListAddresses.Tables["MailingListAddresses"].Columns.Add("Address", Type.GetType("System.String"));
            }
            if (File.Exists(this.m_DataPath + "MailingListAddresses.xml"))
            {
                this.dsMailingListAddresses.ReadXml(this.m_DataPath + "MailingListAddresses.xml");
            }
        }

        private void LoadMailingListACL()
        {
            DateTime lastWriteTime = File.GetLastWriteTime(this.m_DataPath + "MailingListACL.xml");
            if (DateTime.Compare(lastWriteTime, this.m_MailingListAclDate) == 0)
            {
                return;
            }
            this.m_MailingListAclDate = lastWriteTime;
            this.dsMailingListACL.Clear();
            if (!this.dsMailingListACL.Tables.Contains("ACL"))
            {
                this.dsMailingListACL.Tables.Add("ACL");
            }
            if (!this.dsMailingListACL.Tables["ACL"].Columns.Contains("MailingListID"))
            {
                this.dsMailingListACL.Tables["ACL"].Columns.Add("MailingListID");
            }
            if (!this.dsMailingListACL.Tables["ACL"].Columns.Contains("UserOrGroup"))
            {
                this.dsMailingListACL.Tables["ACL"].Columns.Add("UserOrGroup");
            }
            if (!File.Exists(this.m_DataPath + "MailingListACL.xml"))
            {
                this.dsMailingListACL.WriteXml(this.m_DataPath + "MailingListACL.xml");
                return;
            }
            this.dsMailingListACL.ReadXml(this.m_DataPath + "MailingListACL.xml");
        }

        private void LoadGlobalMessageRules()
        {
            DateTime lastWriteTime = File.GetLastWriteTime(this.m_DataPath + "GlobalMessageRules.xml");
            if (DateTime.Compare(lastWriteTime, this.m_RulesDate) == 0)
            {
                return;
            }
            this.m_RulesDate = lastWriteTime;
            this.dsRules.Clear();
            if (!this.dsRules.Tables.Contains("GlobalMessageRules"))
            {
                this.dsRules.Tables.Add("GlobalMessageRules");
            }
            if (!this.dsRules.Tables["GlobalMessageRules"].Columns.Contains("RuleID"))
            {
                this.dsRules.Tables["GlobalMessageRules"].Columns.Add("RuleID", Type.GetType("System.String"));
            }
            if (!this.dsRules.Tables["GlobalMessageRules"].Columns.Contains("Cost"))
            {
                this.dsRules.Tables["GlobalMessageRules"].Columns.Add("Cost", Type.GetType("System.Int64"));
            }
            if (!this.dsRules.Tables["GlobalMessageRules"].Columns.Contains("Enabled"))
            {
                this.dsRules.Tables["GlobalMessageRules"].Columns.Add("Enabled", Type.GetType("System.Boolean"));
            }
            if (!this.dsRules.Tables["GlobalMessageRules"].Columns.Contains("CheckNextRuleIf"))
            {
                this.dsRules.Tables["GlobalMessageRules"].Columns.Add("CheckNextRuleIf", Type.GetType("System.Int32"));
            }
            if (!this.dsRules.Tables["GlobalMessageRules"].Columns.Contains("Description"))
            {
                this.dsRules.Tables["GlobalMessageRules"].Columns.Add("Description", Type.GetType("System.String"));
            }
            if (!this.dsRules.Tables["GlobalMessageRules"].Columns.Contains("MatchExpression"))
            {
                this.dsRules.Tables["GlobalMessageRules"].Columns.Add("MatchExpression", Type.GetType("System.String"));
            }
            if (!File.Exists(this.m_DataPath + "GlobalMessageRules.xml"))
            {
                this.dsRules.WriteXml(this.m_DataPath + "GlobalMessageRules.xml");
                return;
            }
            this.dsRules.ReadXml(this.m_DataPath + "GlobalMessageRules.xml");
        }

        private void LoadGlobalMessageRuleActions()
        {
            DateTime lastWriteTime = File.GetLastWriteTime(this.m_DataPath + "GlobalMessageRuleActions.xml");
            if (DateTime.Compare(lastWriteTime, this.m_RuleActionsDate) == 0)
            {
                return;
            }
            this.m_RuleActionsDate = lastWriteTime;
            this.dsRuleActions.Clear();
            if (!this.dsRuleActions.Tables.Contains("GlobalMessageRuleActions"))
            {
                this.dsRuleActions.Tables.Add("GlobalMessageRuleActions");
            }
            if (!this.dsRuleActions.Tables["GlobalMessageRuleActions"].Columns.Contains("RuleID"))
            {
                this.dsRuleActions.Tables["GlobalMessageRuleActions"].Columns.Add("RuleID", Type.GetType("System.String"));
            }
            if (!this.dsRuleActions.Tables["GlobalMessageRuleActions"].Columns.Contains("ActionID"))
            {
                this.dsRuleActions.Tables["GlobalMessageRuleActions"].Columns.Add("ActionID", Type.GetType("System.String"));
            }
            if (!this.dsRuleActions.Tables["GlobalMessageRuleActions"].Columns.Contains("Description"))
            {
                this.dsRuleActions.Tables["GlobalMessageRuleActions"].Columns.Add("Description", Type.GetType("System.String"));
            }
            if (!this.dsRuleActions.Tables["GlobalMessageRuleActions"].Columns.Contains("ActionType"))
            {
                this.dsRuleActions.Tables["GlobalMessageRuleActions"].Columns.Add("ActionType", Type.GetType("System.Int32"));
            }
            if (!this.dsRuleActions.Tables["GlobalMessageRuleActions"].Columns.Contains("ActionData"))
            {
                this.dsRuleActions.Tables["GlobalMessageRuleActions"].Columns.Add("ActionData", typeof(byte[]));
            }
            if (!File.Exists(this.m_DataPath + "GlobalMessageRuleActions.xml"))
            {
                this.dsRuleActions.WriteXml(this.m_DataPath + "GlobalMessageRuleActions.xml");
                return;
            }
            this.dsRuleActions.ReadXml(this.m_DataPath + "GlobalMessageRuleActions.xml");
        }

        private void LoadRouting()
        {
            DateTime lastWriteTime = File.GetLastWriteTime(this.m_DataPath + "Routing.xml");
            if (DateTime.Compare(lastWriteTime, this.m_RoutingDate) == 0)
            {
                return;
            }
            this.m_RoutingDate = lastWriteTime;
            this.dsRouting.Clear();
            if (!this.dsRouting.Tables.Contains("Routing"))
            {
                this.dsRouting.Tables.Add("Routing");
            }
            if (!this.dsRouting.Tables["Routing"].Columns.Contains("RouteID"))
            {
                this.dsRouting.Tables["Routing"].Columns.Add("RouteID", Type.GetType("System.String"));
            }
            if (!this.dsRouting.Tables["Routing"].Columns.Contains("Cost"))
            {
                this.dsRouting.Tables["Routing"].Columns.Add("Cost", Type.GetType("System.Int64"));
            }
            if (!this.dsRouting.Tables["Routing"].Columns.Contains("Enabled"))
            {
                this.dsRouting.Tables["Routing"].Columns.Add("Enabled", Type.GetType("System.Boolean"));
            }
            if (!this.dsRouting.Tables["Routing"].Columns.Contains("Description"))
            {
                this.dsRouting.Tables["Routing"].Columns.Add("Description", Type.GetType("System.String"));
            }
            if (!this.dsRouting.Tables["Routing"].Columns.Contains("Pattern"))
            {
                this.dsRouting.Tables["Routing"].Columns.Add("Pattern", Type.GetType("System.String"));
            }
            if (!this.dsRouting.Tables["Routing"].Columns.Contains("Action"))
            {
                this.dsRouting.Tables["Routing"].Columns.Add("Action", Type.GetType("System.Int32"));
            }
            if (!this.dsRouting.Tables["Routing"].Columns.Contains("ActionData"))
            {
                this.dsRouting.Tables["Routing"].Columns.Add("ActionData", Type.GetType("System.Byte[]"));
            }
            if (File.Exists(this.m_DataPath + "Routing.xml"))
            {
                this.dsRouting.ReadXml(this.m_DataPath + "Routing.xml");
            }
        }

        private void LoadSecurity()
        {
            DateTime lastWriteTime = File.GetLastWriteTime(this.m_DataPath + "Security.xml");
            if (DateTime.Compare(lastWriteTime, this.m_SecurityDate) == 0)
            {
                return;
            }
            this.m_SecurityDate = lastWriteTime;
            this.dsSecurity.Clear();
            if (!this.dsSecurity.Tables.Contains("IPSecurity"))
            {
                this.dsSecurity.Tables.Add("IPSecurity");
            }
            if (!this.dsSecurity.Tables["IPSecurity"].Columns.Contains("ID"))
            {
                this.dsSecurity.Tables["IPSecurity"].Columns.Add("ID", Type.GetType("System.String"));
            }
            if (!this.dsSecurity.Tables["IPSecurity"].Columns.Contains("Enabled"))
            {
                this.dsSecurity.Tables["IPSecurity"].Columns.Add("Enabled", Type.GetType("System.Boolean"));
            }
            if (!this.dsSecurity.Tables["IPSecurity"].Columns.Contains("Description"))
            {
                this.dsSecurity.Tables["IPSecurity"].Columns.Add("Description", Type.GetType("System.String"));
            }
            if (!this.dsSecurity.Tables["IPSecurity"].Columns.Contains("Service"))
            {
                this.dsSecurity.Tables["IPSecurity"].Columns.Add("Service", Type.GetType("System.Int32"));
            }
            if (!this.dsSecurity.Tables["IPSecurity"].Columns.Contains("Action"))
            {
                this.dsSecurity.Tables["IPSecurity"].Columns.Add("Action", Type.GetType("System.Int32"));
            }
            if (!this.dsSecurity.Tables["IPSecurity"].Columns.Contains("StartIP"))
            {
                this.dsSecurity.Tables["IPSecurity"].Columns.Add("StartIP", Type.GetType("System.String"));
            }
            if (!this.dsSecurity.Tables["IPSecurity"].Columns.Contains("EndIP"))
            {
                this.dsSecurity.Tables["IPSecurity"].Columns.Add("EndIP", Type.GetType("System.String"));
            }
            if (File.Exists(this.m_DataPath + "IPSecurity.xml"))
            {
                this.dsSecurity.ReadXml(this.m_DataPath + "IPSecurity.xml");
            }
        }

        private void LoadFilters()
        {
            DateTime lastWriteTime = File.GetLastWriteTime(this.m_DataPath + "Filters.xml");
            if (DateTime.Compare(lastWriteTime, this.m_FiltersDate) == 0)
            {
                return;
            }
            this.m_FiltersDate = lastWriteTime;
            this.dsFilters.Clear();
            if (!this.dsFilters.Tables.Contains("SmtpFilters"))
            {
                this.dsFilters.Tables.Add("SmtpFilters");
            }
            if (!this.dsFilters.Tables["SmtpFilters"].Columns.Contains("FilterID"))
            {
                this.dsFilters.Tables["SmtpFilters"].Columns.Add("FilterID", Type.GetType("System.String"));
            }
            if (!this.dsFilters.Tables["SmtpFilters"].Columns.Contains("Cost"))
            {
                this.dsFilters.Tables["SmtpFilters"].Columns.Add("Cost", Type.GetType("System.Int64"));
            }
            if (!this.dsFilters.Tables["SmtpFilters"].Columns.Contains("Assembly"))
            {
                this.dsFilters.Tables["SmtpFilters"].Columns.Add("Assembly", Type.GetType("System.String"));
            }
            if (!this.dsFilters.Tables["SmtpFilters"].Columns.Contains("ClassName"))
            {
                this.dsFilters.Tables["SmtpFilters"].Columns.Add("ClassName", Type.GetType("System.String"));
            }
            if (!this.dsFilters.Tables["SmtpFilters"].Columns.Contains("Enabled"))
            {
                this.dsFilters.Tables["SmtpFilters"].Columns.Add("Enabled", Type.GetType("System.Boolean"));
                this.dsFilters.Tables["SmtpFilters"].Columns["Enabled"].DefaultValue = true;
            }
            if (!this.dsFilters.Tables["SmtpFilters"].Columns.Contains("Description"))
            {
                this.dsFilters.Tables["SmtpFilters"].Columns.Add("Description", Type.GetType("System.String"));
            }
            if (!this.dsFilters.Tables["SmtpFilters"].Columns.Contains("Type"))
            {
                this.dsFilters.Tables["SmtpFilters"].Columns.Add("Type", Type.GetType("System.String"));
            }
            if (File.Exists(this.m_DataPath + "Filters.xml"))
            {
                this.dsFilters.ReadXml(this.m_DataPath + "Filters.xml");
            }
        }

        private void Load_IMAP_ACL()
        {
            DateTime lastWriteTime = File.GetLastWriteTime(this.m_DataPath + "IMAP_ACL.xml");
            if (DateTime.Compare(lastWriteTime, this.m_ImapACLDate) == 0)
            {
                return;
            }
            this.m_ImapACLDate = lastWriteTime;
            this.dsImapACL.Clear();
            if (!this.dsImapACL.Tables.Contains("ACL"))
            {
                this.dsImapACL.Tables.Add("ACL");
            }
            if (!this.dsImapACL.Tables["ACL"].Columns.Contains("Folder"))
            {
                this.dsImapACL.Tables["ACL"].Columns.Add("Folder", Type.GetType("System.String"));
            }
            if (!this.dsImapACL.Tables["ACL"].Columns.Contains("User"))
            {
                this.dsImapACL.Tables["ACL"].Columns.Add("User", Type.GetType("System.String"));
            }
            if (!this.dsImapACL.Tables["ACL"].Columns.Contains("Permissions"))
            {
                this.dsImapACL.Tables["ACL"].Columns.Add("Permissions", Type.GetType("System.String"));
            }
            if (File.Exists(this.m_DataPath + "IMAP_ACL.xml"))
            {
                this.dsImapACL.ReadXml(this.m_DataPath + "IMAP_ACL.xml");
            }
        }

        private void Load_SharedFolders_Roots()
        {
            DateTime lastWriteTime = File.GetLastWriteTime(this.m_DataPath + "IMAP_ACL.xml");
            if (DateTime.Compare(lastWriteTime, this.m_SharedFolderRootsDate) == 0)
            {
                return;
            }
            this.m_SharedFolderRootsDate = lastWriteTime;
            this.dsSharedFolderRoots.Clear();
            if (!this.dsSharedFolderRoots.Tables.Contains("SharedFoldersRoots"))
            {
                this.dsSharedFolderRoots.Tables.Add("SharedFoldersRoots");
            }
            if (!this.dsSharedFolderRoots.Tables["SharedFoldersRoots"].Columns.Contains("RootID"))
            {
                this.dsSharedFolderRoots.Tables["SharedFoldersRoots"].Columns.Add("RootID", Type.GetType("System.String"));
            }
            if (!this.dsSharedFolderRoots.Tables["SharedFoldersRoots"].Columns.Contains("Enabled"))
            {
                this.dsSharedFolderRoots.Tables["SharedFoldersRoots"].Columns.Add("Enabled", Type.GetType("System.Boolean"));
            }
            if (!this.dsSharedFolderRoots.Tables["SharedFoldersRoots"].Columns.Contains("Folder"))
            {
                this.dsSharedFolderRoots.Tables["SharedFoldersRoots"].Columns.Add("Folder", Type.GetType("System.String"));
            }
            if (!this.dsSharedFolderRoots.Tables["SharedFoldersRoots"].Columns.Contains("Description"))
            {
                this.dsSharedFolderRoots.Tables["SharedFoldersRoots"].Columns.Add("Description", Type.GetType("System.String"));
            }
            if (!this.dsSharedFolderRoots.Tables["SharedFoldersRoots"].Columns.Contains("RootType"))
            {
                this.dsSharedFolderRoots.Tables["SharedFoldersRoots"].Columns.Add("RootType", Type.GetType("System.Int32"));
            }
            if (!this.dsSharedFolderRoots.Tables["SharedFoldersRoots"].Columns.Contains("BoundedUser"))
            {
                this.dsSharedFolderRoots.Tables["SharedFoldersRoots"].Columns.Add("BoundedUser", Type.GetType("System.String"));
            }
            if (!this.dsSharedFolderRoots.Tables["SharedFoldersRoots"].Columns.Contains("BoundedFolder"))
            {
                this.dsSharedFolderRoots.Tables["SharedFoldersRoots"].Columns.Add("BoundedFolder", Type.GetType("System.String"));
            }
            if (!File.Exists(this.m_DataPath + "SharedFoldersRoots.xml"))
            {
                this.dsSharedFolderRoots.WriteXml(this.m_DataPath + "SharedFoldersRoots.xml");
                return;
            }
            this.dsSharedFolderRoots.ReadXml(this.m_DataPath + "SharedFoldersRoots.xml");
        }

        private void LoadUsersDefaultFolders()
        {
            DateTime lastWriteTime = File.GetLastWriteTime(this.m_DataPath + "UsersDefaultFolders.xml");
            if (DateTime.Compare(lastWriteTime, this.m_UsersDefaultFoldersDate) == 0)
            {
                return;
            }
            this.m_UsersDefaultFoldersDate = lastWriteTime;
            this.dsUsersDefaultFolders.Clear();
            if (!this.dsUsersDefaultFolders.Tables.Contains("UsersDefaultFolders"))
            {
                this.dsUsersDefaultFolders.Tables.Add("UsersDefaultFolders");
            }
            if (!this.dsUsersDefaultFolders.Tables["UsersDefaultFolders"].Columns.Contains("FolderName"))
            {
                this.dsUsersDefaultFolders.Tables["UsersDefaultFolders"].Columns.Add("FolderName", Type.GetType("System.String"));
            }
            if (!this.dsUsersDefaultFolders.Tables["UsersDefaultFolders"].Columns.Contains("Permanent"))
            {
                this.dsUsersDefaultFolders.Tables["UsersDefaultFolders"].Columns.Add("Permanent", Type.GetType("System.Boolean"));
            }
            if (File.Exists(this.m_DataPath + "UsersDefaultFolders.xml"))
            {
                this.dsUsersDefaultFolders.ReadXml(this.m_DataPath + "UsersDefaultFolders.xml");
            }
        }

        private void LoadRecycleBinSettings()
        {
            DateTime lastWriteTime = File.GetLastWriteTime(this.m_DataPath + "RecycleBinSettings.xml");
            if (DateTime.Compare(lastWriteTime, this.m_RecycleBinSettingsDate) == 0)
            {
                return;
            }
            this.m_RecycleBinSettingsDate = lastWriteTime;
            this.dsRecycleBinSettings.Clear();
            if (!this.dsRecycleBinSettings.Tables.Contains("RecycleBinSettings"))
            {
                this.dsRecycleBinSettings.Tables.Add("RecycleBinSettings");
            }
            if (!this.dsRecycleBinSettings.Tables["RecycleBinSettings"].Columns.Contains("DeleteToRecycleBin"))
            {
                this.dsRecycleBinSettings.Tables["RecycleBinSettings"].Columns.Add("DeleteToRecycleBin", typeof(bool)).DefaultValue = false;
            }
            if (!this.dsRecycleBinSettings.Tables["RecycleBinSettings"].Columns.Contains("DeleteMessagesAfter"))
            {
                this.dsRecycleBinSettings.Tables["RecycleBinSettings"].Columns.Add("DeleteMessagesAfter", typeof(int)).DefaultValue = 1;
            }
            if (File.Exists(this.m_DataPath + "RecycleBinSettings.xml"))
            {
                this.dsRecycleBinSettings.ReadXml(this.m_DataPath + "RecycleBinSettings.xml");
            }
            if (this.dsRecycleBinSettings.Tables["RecycleBinSettings"].Rows.Count == 0)
            {
                DataRow dataRow = this.dsRecycleBinSettings.Tables["RecycleBinSettings"].NewRow();
                dataRow["DeleteToRecycleBin"] = false;
                dataRow["DeleteMessagesAfter"] = 1;
                this.dsRecycleBinSettings.Tables["RecycleBinSettings"].Rows.Add(dataRow);
            }
        }

        private void timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.m_UpdSync.BeginUpdate();
            try
            {
                this.LoadDomains();
                this.LoadUsers();
                this.LoadUserAddresses();
                this.LoadUserRemoteServers();
                this.LoadUserMessageRules();
                this.LoadUserMessageRuleActions();
                this.LoadGroups();
                this.LoadGroupMembers();
                this.LoadMailingLists();
                this.LoadMailingListAddresses();
                this.LoadMailingListACL();
                this.LoadGlobalMessageRules();
                this.LoadGlobalMessageRuleActions();
                this.LoadRouting();
                this.LoadSecurity();
                this.LoadFilters();
                this.Load_IMAP_ACL();
                this.Load_SharedFolders_Roots();
                this.LoadUsersDefaultFolders();
                this.LoadRecycleBinSettings();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.EndUpdate();
            }
        }

        private SharedFolderRoot GetSharedFolderRoot(string rootFolder)
        {
            SharedFolderRoot[] sharedFolderRoots = this.GetSharedFolderRoots();
            for (int i = 0; i < sharedFolderRoots.Length; i++)
            {
                SharedFolderRoot sharedFolderRoot = sharedFolderRoots[i];
                if (sharedFolderRoot.FolderName.ToLower() == rootFolder.ToLower())
                {
                    return sharedFolderRoot;
                }
            }
            return null;
        }

        private bool IsUserGroupMember(string group, string user)
        {
            List<string> list = new List<string>();
            Queue<string> queue = new Queue<string>();
            string[] groupMembers = this.GetGroupMembers(group);
            string[] array = groupMembers;
            for (int i = 0; i < array.Length; i++)
            {
                string item = array[i];
                queue.Enqueue(item);
            }
            while (queue.Count > 0)
            {
                string text = queue.Dequeue();
                DataRow group2 = this.GetGroup(text);
                if (group2 != null)
                {
                    if (!list.Contains(text.ToLower()))
                    {
                        if (Convert.ToBoolean(group2["Enabled"]))
                        {
                            groupMembers = this.GetGroupMembers(text);
                            string[] array2 = groupMembers;
                            for (int j = 0; j < array2.Length; j++)
                            {
                                string item2 = array2[j];
                                queue.Enqueue(item2);
                            }
                        }
                        list.Add(text.ToLower());
                    }
                }
                else if (text.ToLower() == user.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        private DataRow GetGroup(string group)
        {
            this.m_UpdSync.AddMethod();
            DataRow result;
            try
            {
                foreach (DataRow dataRow in this.dsGroups.Tables["Groups"].Rows)
                {
                    if (group.ToLower() == dataRow["GroupName"].ToString().ToLower())
                    {
                        result = dataRow;
                        return result;
                    }
                }
                result = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.m_UpdSync.RemoveMethod();
            }
            return result;
        }

        private DataRow GetMailingList(string mailingListName)
        {
            foreach (DataRow dataRow in this.dsMailingLists.Tables["MailingLists"].Rows)
            {
                if (dataRow["MailingListName"].ToString().ToLower() == mailingListName.ToLower())
                {
                    return dataRow;
                }
            }
            return null;
        }

        private LocalXmlStorage.SharedFolderMapInfo MapSharedFolder(string folder)
        {
            string text = folder.Split(new char[]
            {
                '/'
            }, 2)[0];
            SharedFolderRoot[] sharedFolderRoots = this.GetSharedFolderRoots();
            SharedFolderRoot[] array = sharedFolderRoots;
            for (int i = 0; i < array.Length; i++)
            {
                SharedFolderRoot sharedFolderRoot = array[i];
                if (sharedFolderRoot.RootType == SharedFolderRootType.BoundedRootFolder)
                {
                    if (text.ToLower() == sharedFolderRoot.FolderName.ToLower())
                    {
                        LocalXmlStorage.SharedFolderMapInfo result;
                        if (folder.Split(new char[]
                        {
                            '/'
                        }, 2).Length == 2)
                        {
                            result = new LocalXmlStorage.SharedFolderMapInfo(folder, sharedFolderRoot.BoundedUser, sharedFolderRoot.BoundedFolder + "/" + folder.Split(new char[]
                            {
                                '/'
                            }, 2)[1], sharedFolderRoot.FolderName);
                            return result;
                        }
                        result = new LocalXmlStorage.SharedFolderMapInfo(folder, sharedFolderRoot.BoundedUser, sharedFolderRoot.BoundedFolder, sharedFolderRoot.FolderName);
                        return result;
                    }
                }
                else if (sharedFolderRoot.RootType == SharedFolderRootType.UsersSharedFolder && text.ToLower() == sharedFolderRoot.FolderName.ToLower())
                {
                    string[] array2 = folder.Split(new char[]
                    {
                        '/'
                    }, 3);
                    LocalXmlStorage.SharedFolderMapInfo result;
                    if (array2.Length == 3)
                    {
                        result = new LocalXmlStorage.SharedFolderMapInfo(folder, array2[1], array2[2], sharedFolderRoot.FolderName);
                        return result;
                    }
                    if (array2.Length == 3)
                    {
                        result = new LocalXmlStorage.SharedFolderMapInfo(folder, array2[1], "", sharedFolderRoot.FolderName);
                        return result;
                    }
                    result = new LocalXmlStorage.SharedFolderMapInfo(folder, "", "", sharedFolderRoot.FolderName);
                    return result;
                }
            }
            return new LocalXmlStorage.SharedFolderMapInfo(folder, "", "", "");
        }

        private object[] CreateMessageInfo(string file)
        {
            string[] array = Path.GetFileNameWithoutExtension(file).Split(new char[]
            {
                '_'
            });
            if (array.Length == 3)
            {
                int num = (int)new FileInfo(file).Length;
                DateTime dateTime = DateTime.ParseExact(array[0], "yyyyMMddHHmmss", DateTimeFormatInfo.InvariantInfo);
                int num2 = Convert.ToInt32(array[1]);
                int num3 = Convert.ToInt32(array[2]);
                return new object[]
                {
                    num2,
                    num,
                    num3,
                    dateTime
                };
            }
            DateTime dateTime2 = DateTime.ParseExact(array[0], "yyyyMMddHHmmss", DateTimeFormatInfo.InvariantInfo);
            int num4 = Convert.ToInt32(array[1]);
            int num5 = 0;
            int num6 = 0;
            using (FileStream fileStream = File.OpenRead(file))
            {
                _InternalHeader internalHeader = new _InternalHeader(fileStream);
                num5 = (int)(fileStream.Length - fileStream.Position);
                num6 = (int)internalHeader.MessageFlags;
            }
            if (num5 == 0)
            {
                throw new Exception("CreateMessageInfo if(size == 0){, this should never happen !");
            }
            return new object[]
            {
                num4,
                num5,
                num6,
                dateTime2
            };
        }

        private string NormalizeFolder(string folderPath)
        {
            folderPath = folderPath.Replace("\\", "/");
            if (folderPath.EndsWith("/"))
            {
                folderPath = folderPath.Substring(0, folderPath.Length - 1);
            }
            if (folderPath.StartsWith("/"))
            {
                folderPath = folderPath.Substring(1);
            }
            return folderPath;
        }

        private bool ContainsID(DataTable dt, string column, string idValue)
        {
            using (DataView dataView = new DataView(dt))
            {
                dataView.RowFilter = column + "='" + idValue + "'";
                if (dataView.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private byte[] GetTopLines(Stream strm, int nrLines)
        {
            TextReader textReader = new StreamReader(strm);
            int num = 0;
            int num2 = -1;
            bool flag = false;
            StringBuilder stringBuilder = new StringBuilder();
            while (true)
            {
                string text = textReader.ReadLine();
                if (text == null)
                {
                    break;
                }
                if (!flag && text.Length == 0)
                {
                    flag = true;
                }
                if (flag)
                {
                    if (num2 > nrLines)
                    {
                        break;
                    }
                    num2++;
                }
                stringBuilder.Append(text + "\r\n");
                num++;
            }
            return Encoding.ASCII.GetBytes(stringBuilder.ToString());
        }

        private long CalculateMailboxSize(string userName)
        {
            string path = PathHelper.DirectoryExists(PathHelper.PathFix(this.m_MailStorePath + "Mailboxes\\" + userName));
            long num = 0L;
            string[] fileSytemFolders = this.GetFileSytemFolders(path, true);
            for (int i = 0; i < fileSytemFolders.Length; i++)
            {
                string path2 = fileSytemFolders[i];
                DirectoryInfo directoryInfo = new DirectoryInfo(path2);
                FileInfo[] files = directoryInfo.GetFiles();
                for (int j = 0; j < files.Length; j++)
                {
                    FileInfo fileInfo = files[j];
                    num += fileInfo.Length;
                }
            }
            return num;
        }

        private long ChangeMailboxSize(string userName, long size)
        {
            string text = PathHelper.DirectoryExists(PathHelper.PathFix(this.m_MailStorePath + "Mailboxes\\" + userName));
            string path = PathHelper.PathFix(text + "\\_mailbox_size");
            for (int i = 0; i < 1000; i++)
            {
                try
                {
                    long result;
                    if (text == null)
                    {
                        result = 0L;
                        return result;
                    }
                    long num = 0L;
                    using (FileStream fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                    {
                        if (fileStream.Length == 0L)
                        {
                            long num2 = this.CalculateMailboxSize(userName);
                            byte[] bytes = Encoding.ASCII.GetBytes(num2.ToString());
                            fileStream.Write(bytes, 0, bytes.Length);
                            num = num2;
                        }
                        else
                        {
                            byte[] array = new byte[fileStream.Length];
                            fileStream.Read(array, 0, array.Length);
                            long num3 = Convert.ToInt64(Encoding.ASCII.GetString(array).Trim());
                            fileStream.SetLength(0L);
                            array = Encoding.ASCII.GetBytes(Convert.ToString(num3 + size));
                            fileStream.Write(array, 0, array.Length);
                            num = num3 + size;
                        }
                    }
                    result = num;
                    return result;
                }
                catch (IOException ex)
                {
                    string arg_10B_0 = ex.Message;
                    Thread.Sleep(10);
                }
            }
            throw new Exception("Error changing mailbox size, ChangeMailboxSize(long size) failed !");
        }

        private FileStream OpenOrCreateFile(string fileName, int lockTimeOut)
        {
            return this.OpenOrCreateFile(fileName, lockTimeOut, false);
        }

        private FileStream OpenOrCreateFile(string fileName, int lockTimeOut, bool lockOnlyIfCreated)
        {
            DateTime now = DateTime.Now;
            DateTime t = DateTime.Now.AddMilliseconds((double)lockTimeOut);
            string text = "";
            while (t > DateTime.Now)
            {
                try
                {
                    FileStream result;
                    if (lockOnlyIfCreated)
                    {
                        result = File.Open(PathHelper.PathFix(fileName), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                        return result;
                    }
                    result = File.Open(PathHelper.PathFix(fileName), FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    return result;
                }
                catch (FileNotFoundException ex)
                {
                    string arg_53_0 = ex.Message;
                    try
                    {
                        FileStream result = File.Open(PathHelper.PathFix(fileName), FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
                        return result;
                    }
                    catch
                    {
                    }
                }
                catch (IOException ex2)
                {
                    text = ex2.Message;
                    Thread.Sleep(100);
                }
            }
            throw new Exception(string.Concat(new object[]
            {
                "Can't open file '",
                fileName,
                "', lock wait timed out (aquireTime='",
                now.ToString(),
                "' currentTime='",
                DateTime.Now.ToString(),
                " ' lockOnlyIfCreated=",
                lockOnlyIfCreated,
                " error message='",
                text,
                "')!"
            }));
        }

        private string[] GetFileSytemFolders(string path, bool fullPath)
        {
            path = PathHelper.PathFix(path);
            List<string> list = new List<string>();
            Queue<string> queue = new Queue<string>();
            string[] directories = Directory.GetDirectories(path);
            for (int i = 0; i < directories.Length; i++)
            {
                string item = directories[i];
                queue.Enqueue(item);
            }
            while (queue.Count > 0)
            {
                string text = queue.Dequeue();
                string[] directories2 = Directory.GetDirectories(text);
                for (int j = 0; j < directories2.Length; j++)
                {
                    string item2 = directories2[j];
                    queue.Enqueue(item2);
                }
                if (fullPath)
                {
                    list.Add(text);
                }
                else
                {
                    list.Add(text.Substring(path.Length));
                }
            }
            return list.ToArray();
        }

        private string RemovePathRoot(string root, string path)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (path.Replace('/', '\\').StartsWith(root.Replace('/', '\\'), StringComparison.InvariantCultureIgnoreCase))
            {
                string text = path.Substring(root.Length);
                if (text.StartsWith("/") || text.StartsWith("\\"))
                {
                    return text.Substring(1);
                }
            }
            return null;
        }

        private int GetNextUid(string userName, string folder)
        {
            string text = PathHelper.DirectoryExists(PathHelper.PathFix(string.Concat(new string[]
            {
                this.m_MailStorePath,
                "Mailboxes\\",
                userName,
                "\\",
                IMAP_Utils.Encode_IMAP_UTF7_String(folder)
            })));
            string path = PathHelper.PathFix(text + "\\_UID_holder");
            DateTime now = DateTime.Now;
            string str = "";
            while (now.AddSeconds(20.0) > DateTime.Now)
            {
                try
                {
                    int num = 1;
                    using (FileStream fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                    {
                        byte[] array;
                        if (fileStream.Length > 0L)
                        {
                            array = new byte[fileStream.Length];
                            fileStream.Read(array, 0, array.Length);
                            num = Convert.ToInt32(Encoding.ASCII.GetString(array));
                        }
                        else
                        {
                            string[] files = Directory.GetFiles(text, "*.eml");
                            string[] array2 = files;
                            for (int i = 0; i < array2.Length; i++)
                            {
                                string path2 = array2[i];
                                int num2 = Convert.ToInt32(Path.GetFileNameWithoutExtension(path2).Split(new char[]
                                {
                                    '_'
                                })[1]);
                                if (num2 > num)
                                {
                                    num = num2;
                                }
                            }
                        }
                        num++;
                        fileStream.Position = 0L;
                        array = Encoding.ASCII.GetBytes(num.ToString("d10"));
                        fileStream.Write(array, 0, array.Length);
                    }
                    return num;
                }
                catch (Exception ex)
                {
                    str = ex.Message;
                    Thread.Sleep(5);
                }
            }
            throw new Exception("Getting next message UID value timed-out, failed with error: " + str);
        }

        private string CreateMessageFileName(DateTime internalDate, int uid)
        {
            return internalDate.ToString("yyyyMMddHHmmss") + "_" + uid.ToString("D10");
        }

        private void LoadDataSetDefaults(DataSet ds)
        {
            foreach (DataTable dataTable in ds.Tables)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    foreach (DataColumn dataColumn in dataTable.Columns)
                    {
                        if (dataRow.IsNull(dataColumn.ColumnName))
                        {
                            dataRow[dataColumn.ColumnName] = dataColumn.DefaultValue;
                        }
                    }
                }
            }
        }

        private SQLiteConnection GetMessagesInfoSqlCon(string user, string folder)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (user == string.Empty)
            {
                throw new ArgumentException("Argument 'user' value must be specified.", "user");
            }
            if (folder == null)
            {
                throw new ArgumentNullException("folder");
            }
            if (folder == string.Empty)
            {
                throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
            }
            string text = string.Concat(new string[]
            {
                this.m_MailStorePath,
                "Mailboxes\\",
                user,
                "\\",
                IMAP_Utils.Encode_IMAP_UTF7_String(folder)
            });
            SQLiteConnection sQLiteConnection = new SQLiteConnection("Data Source=\"" + text + "\\_MessagesInfo.db3\";Pooling=false");
            sQLiteConnection.Open();
            if (sQLiteConnection.GetSchema("Tables").Select("Table_Name = 'MessagesInfo'").Length > 0)
            {
                return sQLiteConnection;
            }
            SQLiteCommand sQLiteCommand = sQLiteConnection.CreateCommand();
            sQLiteCommand.CommandText = "BEGIN EXCLUSIVE TRANSACTION;";
            sQLiteCommand.ExecuteNonQuery();
            try
            {
                sQLiteCommand.CommandText = "create table MessagesInfo(ID,UID INTEGER,Size INTEGER,Flags,InternalDateTime INTEGER,InternalDate INTEGER,Header,HeaderDecoded,Structure,StructureDecoded,Header_Bcc,Header_Cc,Header_From,Header_Date INTEGER,Header_Subject,Header_To,TextParts);";
                sQLiteCommand.ExecuteNonQuery();
                Dictionary<long, string[]> dictionary = new Dictionary<long, string[]>();
                try
                {
                    if (File.Exists(text + "\\_flags.txt"))
                    {
                        using (FileStream fileStream = File.Open(text + "\\_flags.txt", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            TextReader textReader = new StreamReader(fileStream);
                            for (string text2 = textReader.ReadLine(); text2 != null; text2 = textReader.ReadLine())
                            {
                                if (!text2.StartsWith("#") && !text2.StartsWith("\0"))
                                {
                                    string[] array = text2.Split(new char[]
                                    {
                                        ' '
                                    });
                                    if (!dictionary.ContainsKey((long)Convert.ToInt32(array[1])))
                                    {
                                        List<string> list = new List<string>();
                                        IMAP_MessageFlags iMAP_MessageFlags = (IMAP_MessageFlags)Convert.ToInt32(array[2]);
                                        if ((iMAP_MessageFlags & IMAP_MessageFlags.Answered) != IMAP_MessageFlags.None)
                                        {
                                            list.Add("Answered");
                                        }
                                        if ((iMAP_MessageFlags & IMAP_MessageFlags.Deleted) != IMAP_MessageFlags.None)
                                        {
                                            list.Add("Deleted");
                                        }
                                        if ((iMAP_MessageFlags & IMAP_MessageFlags.Draft) != IMAP_MessageFlags.None)
                                        {
                                            list.Add("Draft");
                                        }
                                        if ((iMAP_MessageFlags & IMAP_MessageFlags.Flagged) != IMAP_MessageFlags.None)
                                        {
                                            list.Add("Flagged");
                                        }
                                        if ((iMAP_MessageFlags & IMAP_MessageFlags.Recent) != IMAP_MessageFlags.None)
                                        {
                                            list.Add("Recent");
                                        }
                                        if ((iMAP_MessageFlags & IMAP_MessageFlags.Seen) != IMAP_MessageFlags.None)
                                        {
                                            list.Add("Seen");
                                        }
                                        dictionary.Add((long)Convert.ToInt32(array[1]), list.ToArray());
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
                string[] files = Directory.GetFiles(text, "*.eml");
                for (int i = 0; i < files.Length; i++)
                {
                    string text3 = files[i];
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text3);
                    long length = new FileInfo(text3).Length;
                    DateTime internalDate = DateTime.ParseExact(Path.GetFileNameWithoutExtension(text3).Split(new char[]
                    {
                        '_'
                    })[0], "yyyyMMddHHmmss", DateTimeFormatInfo.InvariantInfo);
                    long num = Convert.ToInt64(Path.GetFileNameWithoutExtension(text3).Split(new char[]
                    {
                        '_'
                    })[1]);
                    string[] flags = dictionary.ContainsKey(num) ? dictionary[num] : new string[0];
                    this.AddMessageInfo(sQLiteConnection, text3, fileNameWithoutExtension, num, length, flags, internalDate);
                }
            }
            finally
            {
                sQLiteCommand.CommandText = "END TRANSACTION;";
                sQLiteCommand.ExecuteNonQuery();
                sQLiteCommand.Dispose();
            }
            return sQLiteConnection;
        }

        [SQLiteFunction(Name = "REGEXP", Arguments = 2, FuncType = FunctionType.Scalar)]
        class REGEXP : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return Regex.IsMatch(Convert.ToString(args[1]), Convert.ToString(args[0]));
            }
        }

        private void AddMessageInfo(SQLiteConnection sqlCon, string msgFile, string id, long uid, long size, string[] flags, DateTime internalDate)
        {
            if (sqlCon == null)
            {
                throw new ArgumentNullException("sqlCon");
            }
            if (msgFile == null)
            {
                throw new ArgumentNullException("msgFile");
            }
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            StringBuilder stringBuilder = new StringBuilder();
            Mail_Message mail_Message = null;
            try
            {
                mail_Message = Mail_Message.ParseFromFile(msgFile);
            }
            catch
            {
                mail_Message = PathHelper.GenerateBadMessage(new MemoryStream());
            }
            MIME_Entity[] allEntities = mail_Message.AllEntities;
            for (int i = 0; i < allEntities.Length; i++)
            {
                MIME_Entity mIME_Entity = allEntities[i];
                try
                {
                    if (mIME_Entity.Body is MIME_b_SinglepartBase)
                    {
                        if ((mIME_Entity.ContentType == null || string.Equals(mIME_Entity.ContentType.TypeWithSubtype, MIME_MediaTypes.Text.plain, StringComparison.InvariantCultureIgnoreCase)) && mIME_Entity.ContentDisposition == null)
                        {
                            try
                            {
                                stringBuilder.Append(((MIME_b_Text)mIME_Entity.Body).Text);
                            }
                            catch
                            {
                            }
                        }
                        string contentTransferEncoding = MIME_TransferEncodings.SevenBit;
                        if (!string.IsNullOrEmpty(mIME_Entity.ContentTransferEncoding))
                        {
                            contentTransferEncoding = mIME_Entity.ContentTransferEncoding;
                        }
                        ((MIME_b_SinglepartBase)mIME_Entity.Body).SetEncodedData(contentTransferEncoding, new MemoryStream());
                    }
                }
                catch
                {
                }
            }
            using (SQLiteCommand sQLiteCommand = sqlCon.CreateCommand())
            {
                sQLiteCommand.CommandText = "insert into MessagesInfo (ID,UID,Size,Flags,InternalDateTime,InternalDate,Header,HeaderDecoded,Structure,StructureDecoded,Header_Bcc,Header_Cc,Header_From,Header_Date,Header_Subject,Header_To,TextParts) values (@id,@uid,@size,@flags,@internalDateTime,@internalDate,@header,@headerDecoded,@structure,@structureDecoded,@header_Bcc,@header_Cc,@header_From,@header_Date,@header_Subject,@header_To,@textParts);";
                sQLiteCommand.Parameters.Add(new SQLiteParameter("id", id));
                sQLiteCommand.Parameters.Add(new SQLiteParameter("@uid", uid));
                sQLiteCommand.Parameters.Add(new SQLiteParameter("@size", size));
                sQLiteCommand.Parameters.Add(new SQLiteParameter("@flags", Net_Utils.ArrayToString(flags, " ")));
                sQLiteCommand.Parameters.Add(new SQLiteParameter("@internalDateTime", internalDate.ToString("yyyyMMddHHmmss")));
                sQLiteCommand.Parameters.Add(new SQLiteParameter("@internalDate", internalDate.ToString("yyyyMMdd")));
                sQLiteCommand.Parameters.Add(new SQLiteParameter("@header", mail_Message.Header.ToString()));
                sQLiteCommand.Parameters.Add(new SQLiteParameter("@headerDecoded", mail_Message.Header.ToString(null, null, true)));
                sQLiteCommand.Parameters.Add(new SQLiteParameter("@structure", mail_Message.ToString()));
                sQLiteCommand.Parameters.Add(new SQLiteParameter("@structureDecoded", mail_Message.ToString(null, null, true)));
                try
                {
                    sQLiteCommand.Parameters.Add(new SQLiteParameter("@header_Bcc", (mail_Message.Bcc == null) ? "" : mail_Message.Bcc.ToString()));
                }
                catch
                {
                    sQLiteCommand.Parameters.Add(new SQLiteParameter("@header_Bcc", ""));
                }
                try
                {
                    sQLiteCommand.Parameters.Add(new SQLiteParameter("@header_Cc", (mail_Message.Cc == null) ? "" : mail_Message.Cc.ToString()));
                }
                catch
                {
                    sQLiteCommand.Parameters.Add(new SQLiteParameter("@header_Cc", ""));
                }
                try
                {
                    sQLiteCommand.Parameters.Add(new SQLiteParameter("@header_From", (mail_Message.From == null) ? "" : mail_Message.From.ToString()));
                }
                catch
                {
                    sQLiteCommand.Parameters.Add(new SQLiteParameter("@header_From", ""));
                }
                try
                {
                    sQLiteCommand.Parameters.Add(new SQLiteParameter("@header_Date", ((mail_Message.Date == DateTime.MinValue) ? DateTime.Now : mail_Message.Date).ToString("yyyyMMdd")));
                }
                catch
                {
                    sQLiteCommand.Parameters.Add(new SQLiteParameter("@header_Date", DateTime.Now.ToString("yyyyMMdd")));
                }
                sQLiteCommand.Parameters.Add(new SQLiteParameter("@header_Subject", (mail_Message.Subject == null) ? "" : mail_Message.Subject));
                try
                {
                    sQLiteCommand.Parameters.Add(new SQLiteParameter("@header_To", (mail_Message.To == null) ? "" : mail_Message.To.ToString()));
                }
                catch
                {
                    sQLiteCommand.Parameters.Add(new SQLiteParameter("@header_To", ""));
                }
                sQLiteCommand.Parameters.Add(new SQLiteParameter("@textParts", stringBuilder.ToString()));
                sQLiteCommand.ExecuteNonQuery();
            }
        }

        private string SearchCriteriaToSql(IMAP_Search_Key key, Dictionary<long, long> seqNo_to_UID)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            StringBuilder stringBuilder = new StringBuilder();
            if (key is IMAP_Search_Key_Group)
            {
                stringBuilder.Append("(");
                IMAP_Search_Key_Group iMAP_Search_Key_Group = (IMAP_Search_Key_Group)key;
                for (int i = 0; i < iMAP_Search_Key_Group.Keys.Count; i++)
                {
                    if (i > 0)
                    {
                        stringBuilder.Append(" AND ");
                    }
                    stringBuilder.Append(this.SearchCriteriaToSql(iMAP_Search_Key_Group.Keys[i], seqNo_to_UID));
                }
                stringBuilder.Append(")");
            }
            else if (key is IMAP_Search_Key_All)
            {
                stringBuilder.Append("UID > -1");
            }
            else if (key is IMAP_Search_Key_Answered)
            {
                stringBuilder.Append("Flags like '%Answered%'");
            }
            else if (key is IMAP_Search_Key_Before)
            {
                stringBuilder.Append("InternalDate < " + ((IMAP_Search_Key_Before)key).Date.ToString("yyyyMMdd"));
            }
            else if (key is IMAP_Search_Key_Bcc)
            {
                stringBuilder.Append("Header_Bcc like '%" + ((IMAP_Search_Key_Bcc)key).Value + "%'");
            }
            else if (key is IMAP_Search_Key_Body)
            {
                stringBuilder.Append("TextParts like '%" + ((IMAP_Search_Key_Body)key).Value + "%'");
            }
            else if (key is IMAP_Search_Key_Cc)
            {
                stringBuilder.Append("Header_Cc like '%" + ((IMAP_Search_Key_Cc)key).Value + "%'");
            }
            else if (key is IMAP_Search_Key_Deleted)
            {
                stringBuilder.Append("Flags like '%Deleted%'");
            }
            else if (key is IMAP_Search_Key_Draft)
            {
                stringBuilder.Append("Flags like '%Draft%'");
            }
            else if (key is IMAP_Search_Key_Flagged)
            {
                stringBuilder.Append("Flags like '%Flagged%'");
            }
            else if (key is IMAP_Search_Key_From)
            {
                stringBuilder.Append("Header_From like '%" + ((IMAP_Search_Key_From)key).Value + "%'");
            }
            else if (key is IMAP_Search_Key_Header)
            {
                IMAP_Search_Key_Header iMAP_Search_Key_Header = (IMAP_Search_Key_Header)key;
                if (string.IsNullOrEmpty(iMAP_Search_Key_Header.Value))
                {
                    stringBuilder.Append("HeaderDecoded REGEXP '(\\n)*" + Regex.Escape(iMAP_Search_Key_Header.FieldName) + "\\s*:{1}.*'");
                }
                else
                {
                    stringBuilder.Append(string.Concat(new string[]
                    {
                        "HeaderDecoded REGEXP '(\\n)*",
                        Regex.Escape(iMAP_Search_Key_Header.FieldName),
                        "\\s*:{1}.*(\\r\\n\\s|\\n\\s)*.*",
                        Regex.Escape(iMAP_Search_Key_Header.Value),
                        ".*'"
                    }));
                }
            }
            else if (key is IMAP_Search_Key_Keyword)
            {
                stringBuilder.Append("Flags like '%" + ((IMAP_Search_Key_Keyword)key).Value + "%'");
            }
            else if (key is IMAP_Search_Key_Larger)
            {
                stringBuilder.Append("Size > " + ((IMAP_Search_Key_Larger)key).Value);
            }
            else if (key is IMAP_Search_Key_New)
            {
                stringBuilder.Append("(Flags like '%Recent%' and Flags not like '%Seen%')");
            }
            else if (key is IMAP_Search_Key_Not)
            {
                stringBuilder.Append("not " + this.SearchCriteriaToSql(((IMAP_Search_Key_Not)key).SearchKey, seqNo_to_UID));
            }
            else if (key is IMAP_Search_Key_Old)
            {
                stringBuilder.Append("Flags not like '%Recent%'");
            }
            else if (key is IMAP_Search_Key_On)
            {
                stringBuilder.Append("InternalDate = " + ((IMAP_Search_Key_On)key).Date.ToString("yyyyMMdd"));
            }
            else if (key is IMAP_Search_Key_Or)
            {
                stringBuilder.Append(string.Concat(new string[]
                {
                    "(",
                    this.SearchCriteriaToSql(((IMAP_Search_Key_Or)key).SearchKey1, seqNo_to_UID),
                    " or ",
                    this.SearchCriteriaToSql(((IMAP_Search_Key_Or)key).SearchKey2, seqNo_to_UID),
                    ")"
                }));
            }
            else if (key is IMAP_Search_Key_Recent)
            {
                stringBuilder.Append("Flags like '%Recent%'");
            }
            else if (key is IMAP_Search_Key_Seen)
            {
                stringBuilder.Append("Flags like '%Seen%'");
            }
            else if (key is IMAP_Search_Key_SentBefore)
            {
                stringBuilder.Append("Header_Date < " + ((IMAP_Search_Key_SentBefore)key).Date.ToString("yyyyMMdd"));
            }
            else if (key is IMAP_Search_Key_SentOn)
            {
                stringBuilder.Append("Header_Date = " + ((IMAP_Search_Key_SentOn)key).Date.ToString("yyyyMMdd"));
            }
            else if (key is IMAP_Search_Key_SentSince)
            {
                stringBuilder.Append("Header_Date >= " + ((IMAP_Search_Key_SentSince)key).Date.ToString("yyyyMMdd"));
            }
            else if (key is IMAP_Search_Key_Since)
            {
                stringBuilder.Append("InternalDate >= " + ((IMAP_Search_Key_Since)key).Date.ToString("yyyyMMdd"));
            }
            else if (key is IMAP_Search_Key_Smaller)
            {
                stringBuilder.Append("Size < " + ((IMAP_Search_Key_Smaller)key).Value);
            }
            else if (key is IMAP_Search_Key_Subject)
            {
                stringBuilder.Append("Header_Subject like '%" + ((IMAP_Search_Key_Subject)key).Value + "%'");
            }
            else if (key is IMAP_Search_Key_Text)
            {
                stringBuilder.Append(string.Concat(new string[]
                {
                    "(StructureDecoded like '%",
                    ((IMAP_Search_Key_Text)key).Value,
                    "%' OR TextParts like '%",
                    ((IMAP_Search_Key_Text)key).Value,
                    "%')"
                }));
            }
            else if (key is IMAP_Search_Key_To)
            {
                stringBuilder.Append("Header_To like '%" + ((IMAP_Search_Key_To)key).Value + "%'");
            }
            else if (key is IMAP_Search_Key_SeqSet)
            {
                stringBuilder.Append("(");
                IMAP_t_SeqSet value = ((IMAP_Search_Key_SeqSet)key).Value;
                for (int j = 0; j < value.Items.Length; j++)
                {
                    Range_long range_long = value.Items[j];
                    if (j > 0)
                    {
                        stringBuilder.Append(" OR ");
                    }
                    long num = 0L;
                    long num2 = 0L;
                    if (!seqNo_to_UID.TryGetValue(range_long.Start, out num))
                    {
                        num = 2147483647L;
                    }
                    if (!seqNo_to_UID.TryGetValue(range_long.End, out num2))
                    {
                        num2 = 2147483647L;
                    }
                    if (num == num2)
                    {
                        stringBuilder.Append("UID = " + num);
                    }
                    else
                    {
                        stringBuilder.Append(string.Concat(new object[]
                        {
                            "UID >= ",
                            num,
                            " AND UID <= ",
                            num2
                        }));
                    }
                }
                stringBuilder.Append(")");
            }
            else if (key is IMAP_Search_Key_Uid)
            {
                stringBuilder.Append("(");
                IMAP_t_SeqSet value2 = ((IMAP_Search_Key_Uid)key).Value;
                for (int k = 0; k < value2.Items.Length; k++)
                {
                    Range_long range_long2 = value2.Items[k];
                    if (k > 0)
                    {
                        stringBuilder.Append(" OR ");
                    }
                    if (range_long2.Start == range_long2.End)
                    {
                        stringBuilder.Append("UID = " + range_long2.Start);
                    }
                    else
                    {
                        stringBuilder.Append(string.Concat(new object[]
                        {
                            "UID >= ",
                            range_long2.Start,
                            " AND UID <= ",
                            range_long2.End
                        }));
                    }
                }
                stringBuilder.Append(")");
            }
            else if (key is IMAP_Search_Key_Unanswered)
            {
                stringBuilder.Append("Flags not like '%Answered%'");
            }
            else if (key is IMAP_Search_Key_Undeleted)
            {
                stringBuilder.Append("Flags not like '%Deleted%'");
            }
            else if (key is IMAP_Search_Key_Undraft)
            {
                stringBuilder.Append("Flags not like '%Draft%'");
            }
            else if (key is IMAP_Search_Key_Unflagged)
            {
                stringBuilder.Append("Flags not like '%Flagged%'");
            }
            else if (key is IMAP_Search_Key_Keyword)
            {
                stringBuilder.Append("Flags not like '%" + ((IMAP_Search_Key_Unkeyword)key).Value + "%'");
            }
            else if (key is IMAP_Search_Key_Unseen)
            {
                stringBuilder.Append("Flags not like '%Seen%'");
            }
            else
            {
                stringBuilder.Append("'' = ''");
            }
            return stringBuilder.ToString();
        }
    }
}
