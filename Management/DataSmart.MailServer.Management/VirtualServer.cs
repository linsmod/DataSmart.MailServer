using System.NetworkToolkit;
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Net;
using System.Text;

namespace DataSmart.MailServer.Management
{
	public class VirtualServer
	{
		private Server m_pServer;

		private VirtualServerCollection m_pOwner;

		private string m_VirtualServerID = "";

		private bool m_Enabled;

		private string m_Name = "";

		private string m_Assembly = "";

		private string m_Type = "";

		private string m_InitString = "";

		private SystemSettings m_pSystemSettings;

		private DomainCollection m_pDomains;

		private UserCollection m_pUsers;

		private GroupCollection m_pGroups;

		private MailingListCollection m_pMailingLists;

		private GlobalMessageRuleCollection m_pGlobalMsgRules;

		private RouteCollection m_pRoutes;

		private SharedRootFolderCollection m_pRootFolders;

		private UsersDefaultFolderCollection m_UsersDefaultFolders;

		private FilterCollection m_pFilters;

		private IPSecurityCollection m_pIpSecurity;

		private Queues m_pQueues;

		private RecycleBin m_pRecycleBin;

		private Logs m_pLogs;

		private SipRegistrationCollection m_pSipRegistrations;

		private SipCallCollection m_pSipCalls;

		private bool m_ValuesChanged;

		public Server Server
		{
			get
			{
				return this.m_pServer;
			}
		}

		public VirtualServerCollection Owner
		{
			get
			{
				return this.m_pOwner;
			}
		}

		public bool HasChanges
		{
			get
			{
				return this.m_ValuesChanged;
			}
		}

		public string VirtualServerID
		{
			get
			{
				return this.m_VirtualServerID;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.m_Enabled;
			}
			set
			{
				if (this.m_Enabled != value)
				{
					this.m_Enabled = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public string Name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				if (this.m_Name != value)
				{
					this.m_Name = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public string AssemblyName
		{
			get
			{
				return this.m_Assembly;
			}
		}

		public string TypeName
		{
			get
			{
				return this.m_Type;
			}
		}

		public string InitString
		{
			get
			{
				return this.m_InitString;
			}
			set
			{
				if (this.m_InitString != value)
				{
					this.m_InitString = value;
					this.m_ValuesChanged = true;
				}
			}
		}

		public SystemSettings SystemSettings
		{
			get
			{
				if (this.m_pSystemSettings == null)
				{
					this.m_pSystemSettings = new SystemSettings(this);
				}
				return this.m_pSystemSettings;
			}
		}

		public DomainCollection Domains
		{
			get
			{
				if (this.m_pDomains == null)
				{
					this.m_pDomains = new DomainCollection(this);
				}
				return this.m_pDomains;
			}
		}

		public UserCollection Users
		{
			get
			{
				if (this.m_pUsers == null)
				{
					this.m_pUsers = new UserCollection(this);
				}
				return this.m_pUsers;
			}
		}

		public GroupCollection Groups
		{
			get
			{
				if (this.m_pGroups == null)
				{
					this.m_pGroups = new GroupCollection(this);
				}
				return this.m_pGroups;
			}
		}

		public MailingListCollection MailingLists
		{
			get
			{
				if (this.m_pMailingLists == null)
				{
					this.m_pMailingLists = new MailingListCollection(this);
				}
				return this.m_pMailingLists;
			}
		}

		public GlobalMessageRuleCollection GlobalMessageRules
		{
			get
			{
				if (this.m_pGlobalMsgRules == null)
				{
					this.m_pGlobalMsgRules = new GlobalMessageRuleCollection(this);
				}
				return this.m_pGlobalMsgRules;
			}
		}

		public RouteCollection Routes
		{
			get
			{
				if (this.m_pRoutes == null)
				{
					this.m_pRoutes = new RouteCollection(this);
				}
				return this.m_pRoutes;
			}
		}

		public SharedRootFolderCollection RootFolders
		{
			get
			{
				if (this.m_pRootFolders == null)
				{
					this.m_pRootFolders = new SharedRootFolderCollection(this);
				}
				return this.m_pRootFolders;
			}
		}

		public UsersDefaultFolderCollection UsersDefaultFolders
		{
			get
			{
				if (this.m_UsersDefaultFolders == null)
				{
					this.m_UsersDefaultFolders = new UsersDefaultFolderCollection(this);
				}
				return this.m_UsersDefaultFolders;
			}
		}

		public FilterCollection Filters
		{
			get
			{
				if (this.m_pFilters == null)
				{
					this.m_pFilters = new FilterCollection(this);
				}
				return this.m_pFilters;
			}
		}

		public IPSecurityCollection IpSecurity
		{
			get
			{
				if (this.m_pIpSecurity == null)
				{
					this.m_pIpSecurity = new IPSecurityCollection(this);
				}
				return this.m_pIpSecurity;
			}
		}

		public Queues Queues
		{
			get
			{
				if (this.m_pQueues == null)
				{
					this.m_pQueues = new Queues(this);
				}
				return this.m_pQueues;
			}
		}

		public RecycleBin RecycleBin
		{
			get
			{
				if (this.m_pRecycleBin == null)
				{
					this.m_pRecycleBin = new RecycleBin(this);
				}
				return this.m_pRecycleBin;
			}
		}

		public Logs Logs
		{
			get
			{
				if (this.m_pLogs == null)
				{
					this.m_pLogs = new Logs(this);
				}
				return this.m_pLogs;
			}
		}

		public SipRegistrationCollection SipRegistrations
		{
			get
			{
				if (this.m_pSipRegistrations == null)
				{
					this.m_pSipRegistrations = new SipRegistrationCollection(this);
				}
				return this.m_pSipRegistrations;
			}
		}

		public SipCallCollection SipCalls
		{
			get
			{
				if (this.m_pSipCalls == null)
				{
					this.m_pSipCalls = new SipCallCollection(this);
				}
				return this.m_pSipCalls;
			}
		}

		internal VirtualServer(Server server, VirtualServerCollection owner, string id, bool enabled, string name, string assembly, string type, string initString)
		{
			this.m_pServer = server;
			this.m_pOwner = owner;
			this.m_VirtualServerID = id;
			this.m_Enabled = enabled;
			this.m_Name = name;
			this.m_Assembly = assembly;
			this.m_Type = type;
			this.m_InitString = initString;
		}

		public void Backup(string fileName)
		{
			using (FileStream fileStream = File.Create(fileName))
			{
				this.Backup(fileStream);
			}
		}

		public void Backup(Stream stream)
		{
			DataSet dataSet = new DataSet("dsVirtualServerBackup");
			dataSet.Merge(this.SystemSettings.ToDataSet());
			dataSet.Tables.Add("Domains");
			dataSet.Tables["Domains"].Columns.Add("DomainID");
			dataSet.Tables["Domains"].Columns.Add("DomainName");
			dataSet.Tables["Domains"].Columns.Add("Description");
			foreach (Domain domain in this.Domains)
			{
				DataRow dataRow = dataSet.Tables["Domains"].NewRow();
				dataRow["DomainID"] = domain.DomainID;
				dataRow["DomainName"] = domain.DomainName;
				dataRow["Description"] = domain.Description;
				dataSet.Tables["Domains"].Rows.Add(dataRow);
			}
			dataSet.Tables.Add("Users");
			dataSet.Tables["Users"].Columns.Add("UserID");
			dataSet.Tables["Users"].Columns.Add("FullName");
			dataSet.Tables["Users"].Columns.Add("UserName");
			dataSet.Tables["Users"].Columns.Add("Password");
			dataSet.Tables["Users"].Columns.Add("Description");
			dataSet.Tables["Users"].Columns.Add("Mailbox_Size");
			dataSet.Tables["Users"].Columns.Add("Enabled");
			dataSet.Tables["Users"].Columns.Add("Permissions");
			foreach (User user in this.Users)
			{
				DataRow dataRow2 = dataSet.Tables["Users"].NewRow();
				dataRow2["UserID"] = user.UserID;
				dataRow2["FullName"] = user.FullName;
				dataRow2["UserName"] = user.UserName;
				dataRow2["Password"] = user.Password;
				dataRow2["Description"] = user.Description;
				dataRow2["Mailbox_Size"] = user.MaximumMailboxSize;
				dataRow2["Enabled"] = user.Enabled;
				dataRow2["Permissions"] = (int)user.Permissions;
				dataSet.Tables["Users"].Rows.Add(dataRow2);
			}
			dataSet.Tables.Add("User_EmailAddresses");
			dataSet.Tables["User_EmailAddresses"].Columns.Add("UserID");
			dataSet.Tables["User_EmailAddresses"].Columns.Add("EmailAddress");
			foreach (User user2 in this.Users)
			{
				foreach (string value in user2.EmailAddresses)
				{
					DataRow dataRow3 = dataSet.Tables["User_EmailAddresses"].NewRow();
					dataRow3["UserID"] = user2.UserID;
					dataRow3["EmailAddress"] = value;
					dataSet.Tables["User_EmailAddresses"].Rows.Add(dataRow3);
				}
			}
			dataSet.Tables.Add("User_RemoteServers");
			dataSet.Tables["User_RemoteServers"].Columns.Add("UserID");
			dataSet.Tables["User_RemoteServers"].Columns.Add("ServerID");
			dataSet.Tables["User_RemoteServers"].Columns.Add("Description");
			dataSet.Tables["User_RemoteServers"].Columns.Add("RemoteServer");
			dataSet.Tables["User_RemoteServers"].Columns.Add("RemotePort");
			dataSet.Tables["User_RemoteServers"].Columns.Add("RemoteUserName");
			dataSet.Tables["User_RemoteServers"].Columns.Add("RemotePassword");
			dataSet.Tables["User_RemoteServers"].Columns.Add("UseSSL");
			dataSet.Tables["User_RemoteServers"].Columns.Add("Enabled");
			foreach (User user3 in this.Users)
			{
				foreach (UserRemoteServer userRemoteServer in user3.RemoteServers)
				{
					DataRow dataRow4 = dataSet.Tables["User_RemoteServers"].NewRow();
					dataRow4["UserID"] = user3.UserID;
					dataRow4["ServerID"] = userRemoteServer.ID;
					dataRow4["Description"] = userRemoteServer.Description;
					dataRow4["RemoteServer"] = userRemoteServer.Host;
					dataRow4["RemotePort"] = userRemoteServer.Port;
					dataRow4["RemoteUserName"] = userRemoteServer.UserName;
					dataRow4["RemotePassword"] = userRemoteServer.Password;
					dataRow4["UseSSL"] = userRemoteServer.SSL;
					dataRow4["Enabled"] = userRemoteServer.Enabled;
					dataSet.Tables["User_RemoteServers"].Rows.Add(dataRow4);
				}
			}
			dataSet.Tables.Add("User_MessageRules");
			dataSet.Tables["User_MessageRules"].Columns.Add("UserID");
			dataSet.Tables["User_MessageRules"].Columns.Add("RuleID");
			dataSet.Tables["User_MessageRules"].Columns.Add("Enabled");
			dataSet.Tables["User_MessageRules"].Columns.Add("CheckNextRuleIf");
			dataSet.Tables["User_MessageRules"].Columns.Add("Description");
			dataSet.Tables["User_MessageRules"].Columns.Add("MatchExpression");
			foreach (User user4 in this.Users)
			{
				foreach (UserMessageRule userMessageRule in user4.MessageRules)
				{
					DataRow dataRow5 = dataSet.Tables["User_MessageRules"].NewRow();
					dataRow5["UserID"] = user4.UserID;
					dataRow5["RuleID"] = userMessageRule.ID;
					dataRow5["Enabled"] = userMessageRule.Enabled;
					dataRow5["CheckNextRuleIf"] = (int)userMessageRule.CheckNextRule;
					dataRow5["Description"] = userMessageRule.Description;
					dataRow5["MatchExpression"] = userMessageRule.MatchExpression;
					dataSet.Tables["User_MessageRules"].Rows.Add(dataRow5);
				}
			}
			dataSet.Tables.Add("User_MessageRuleActions");
			dataSet.Tables["User_MessageRuleActions"].Columns.Add("UserID");
			dataSet.Tables["User_MessageRuleActions"].Columns.Add("RuleID");
			dataSet.Tables["User_MessageRuleActions"].Columns.Add("ActionID");
			dataSet.Tables["User_MessageRuleActions"].Columns.Add("Description");
			dataSet.Tables["User_MessageRuleActions"].Columns.Add("ActionType");
			dataSet.Tables["User_MessageRuleActions"].Columns.Add("ActionData", typeof(byte[]));
			IEnumerator enumerator;
			foreach (User user5 in this.Users)
			{
				enumerator = user5.MessageRules.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						UserMessageRule userMessageRule2 = (UserMessageRule)enumerator.Current;
						foreach (UserMessageRuleActionBase userMessageRuleActionBase in userMessageRule2.Actions)
						{
							DataRow dataRow6 = dataSet.Tables["User_MessageRuleActions"].NewRow();
							dataRow6["UserID"] = user5.UserID;
							dataRow6["RuleID"] = userMessageRule2.ID;
							dataRow6["ActionID"] = userMessageRuleActionBase.ID;
							dataRow6["Description"] = userMessageRuleActionBase.Description;
							dataRow6["ActionType"] = (int)userMessageRuleActionBase.ActionType;
							dataRow6["ActionData"] = userMessageRuleActionBase.Serialize();
							dataSet.Tables["User_MessageRuleActions"].Rows.Add(dataRow6);
						}
					}
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
			dataSet.Tables.Add("Groups");
			dataSet.Tables["Groups"].Columns.Add("GroupID");
			dataSet.Tables["Groups"].Columns.Add("GroupName");
			dataSet.Tables["Groups"].Columns.Add("Description");
			dataSet.Tables["Groups"].Columns.Add("Enabled");
			enumerator = this.Groups.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Group group = (Group)enumerator.Current;
					DataRow dataRow7 = dataSet.Tables["Groups"].NewRow();
					dataRow7["GroupID"] = group.GroupID;
					dataRow7["GroupName"] = group.GroupName;
					dataRow7["Description"] = group.Description;
					dataRow7["Enabled"] = group.Enabled;
					dataSet.Tables["Groups"].Rows.Add(dataRow7);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			dataSet.Tables.Add("Group_Members");
			dataSet.Tables["Group_Members"].Columns.Add("GroupID");
			dataSet.Tables["Group_Members"].Columns.Add("UserOrGroup");
			enumerator = this.Groups.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Group group2 = (Group)enumerator.Current;
					foreach (string value2 in group2.Members)
					{
						DataRow dataRow8 = dataSet.Tables["Group_Members"].NewRow();
						dataRow8["GroupID"] = group2.GroupID;
						dataRow8["UserOrGroup"] = value2;
						dataSet.Tables["Group_Members"].Rows.Add(dataRow8);
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			dataSet.Tables.Add("MailingLists");
			dataSet.Tables["MailingLists"].Columns.Add("MailingListID");
			dataSet.Tables["MailingLists"].Columns.Add("MailingListName");
			dataSet.Tables["MailingLists"].Columns.Add("Description");
			dataSet.Tables["MailingLists"].Columns.Add("Enabled");
			enumerator = this.MailingLists.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					MailingList mailingList = (MailingList)enumerator.Current;
					DataRow dataRow9 = dataSet.Tables["MailingLists"].NewRow();
					dataRow9["MailingListID"] = mailingList.ID;
					dataRow9["MailingListName"] = mailingList.Name;
					dataRow9["Description"] = mailingList.Description;
					dataRow9["Enabled"] = mailingList.Enabled;
					dataSet.Tables["MailingLists"].Rows.Add(dataRow9);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			dataSet.Tables.Add("MailingList_Members");
			dataSet.Tables["MailingList_Members"].Columns.Add("MailingListID");
			dataSet.Tables["MailingList_Members"].Columns.Add("Address");
			enumerator = this.MailingLists.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					MailingList mailingList2 = (MailingList)enumerator.Current;
					foreach (string value3 in mailingList2.Members)
					{
						DataRow dataRow10 = dataSet.Tables["MailingList_Members"].NewRow();
						dataRow10["MailingListID"] = mailingList2.ID;
						dataRow10["Address"] = value3;
						dataSet.Tables["MailingList_Members"].Rows.Add(dataRow10);
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			dataSet.Tables.Add("MailingList_ACL");
			dataSet.Tables["MailingList_ACL"].Columns.Add("MailingListID");
			dataSet.Tables["MailingList_ACL"].Columns.Add("UserOrGroup");
			enumerator = this.MailingLists.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					MailingList mailingList3 = (MailingList)enumerator.Current;
					foreach (string value4 in mailingList3.ACL)
					{
						DataRow dataRow11 = dataSet.Tables["MailingList_ACL"].NewRow();
						dataRow11["MailingListID"] = mailingList3.ID;
						dataRow11["UserOrGroup"] = value4;
						dataSet.Tables["MailingList_ACL"].Rows.Add(dataRow11);
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			dataSet.Tables.Add("Routing");
			dataSet.Tables["Routing"].Columns.Add("RouteID");
			dataSet.Tables["Routing"].Columns.Add("Enabled");
			dataSet.Tables["Routing"].Columns.Add("Description");
			dataSet.Tables["Routing"].Columns.Add("Pattern");
			dataSet.Tables["Routing"].Columns.Add("Action");
			dataSet.Tables["Routing"].Columns.Add("ActionData", typeof(byte[]));
			enumerator = this.Routes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Route route = (Route)enumerator.Current;
					DataRow dataRow12 = dataSet.Tables["Routing"].NewRow();
					dataRow12["RouteID"] = route.ID;
					dataRow12["Enabled"] = route.Enabled;
					dataRow12["Description"] = route.Description;
					dataRow12["Pattern"] = route.Pattern;
					dataRow12["Action"] = (int)route.Action.ActionType;
					dataRow12["ActionData"] = route.Action.Serialize();
					dataSet.Tables["Routing"].Rows.Add(dataRow12);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			dataSet.Tables.Add("GlobalMessageRules");
			dataSet.Tables["GlobalMessageRules"].Columns.Add("RuleID");
			dataSet.Tables["GlobalMessageRules"].Columns.Add("Enabled");
			dataSet.Tables["GlobalMessageRules"].Columns.Add("CheckNextRuleIf");
			dataSet.Tables["GlobalMessageRules"].Columns.Add("Description");
			dataSet.Tables["GlobalMessageRules"].Columns.Add("MatchExpression");
			enumerator = this.GlobalMessageRules.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					GlobalMessageRule globalMessageRule = (GlobalMessageRule)enumerator.Current;
					DataRow dataRow13 = dataSet.Tables["GlobalMessageRules"].NewRow();
					dataRow13["RuleID"] = globalMessageRule.ID;
					dataRow13["Enabled"] = globalMessageRule.Enabled;
					dataRow13["CheckNextRuleIf"] = (int)globalMessageRule.CheckNextRule;
					dataRow13["Description"] = globalMessageRule.Description;
					dataRow13["MatchExpression"] = globalMessageRule.MatchExpression;
					dataSet.Tables["GlobalMessageRules"].Rows.Add(dataRow13);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			dataSet.Tables.Add("GlobalMessageRuleActions");
			dataSet.Tables["GlobalMessageRuleActions"].Columns.Add("RuleID");
			dataSet.Tables["GlobalMessageRuleActions"].Columns.Add("ActionID");
			dataSet.Tables["GlobalMessageRuleActions"].Columns.Add("Description");
			dataSet.Tables["GlobalMessageRuleActions"].Columns.Add("ActionType");
			dataSet.Tables["GlobalMessageRuleActions"].Columns.Add("ActionData", typeof(byte[]));
			enumerator = this.GlobalMessageRules.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					GlobalMessageRule globalMessageRule2 = (GlobalMessageRule)enumerator.Current;
					foreach (GlobalMessageRuleActionBase globalMessageRuleActionBase in globalMessageRule2.Actions)
					{
						DataRow dataRow14 = dataSet.Tables["GlobalMessageRuleActions"].NewRow();
						dataRow14["RuleID"] = globalMessageRule2.ID;
						dataRow14["ActionID"] = globalMessageRuleActionBase.ID;
						dataRow14["Description"] = globalMessageRuleActionBase.Description;
						dataRow14["ActionType"] = (int)globalMessageRuleActionBase.ActionType;
						dataRow14["ActionData"] = globalMessageRuleActionBase.Serialize();
						dataSet.Tables["GlobalMessageRuleActions"].Rows.Add(dataRow14);
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			dataSet.Tables.Add("IP_Security");
			dataSet.Tables["IP_Security"].Columns.Add("ID");
			dataSet.Tables["IP_Security"].Columns.Add("Enabled");
			dataSet.Tables["IP_Security"].Columns.Add("Description");
			dataSet.Tables["IP_Security"].Columns.Add("Service");
			dataSet.Tables["IP_Security"].Columns.Add("Action");
			dataSet.Tables["IP_Security"].Columns.Add("StartIP");
			dataSet.Tables["IP_Security"].Columns.Add("EndIP");
			enumerator = this.IpSecurity.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					IPSecurity iPSecurity = (IPSecurity)enumerator.Current;
					DataRow dataRow15 = dataSet.Tables["IP_Security"].NewRow();
					dataRow15["ID"] = iPSecurity.ID;
					dataRow15["Enabled"] = iPSecurity.Enabled;
					dataRow15["Description"] = iPSecurity.Description;
					dataRow15["Service"] = (int)iPSecurity.Service;
					dataRow15["Action"] = (int)iPSecurity.Action;
					dataRow15["StartIP"] = iPSecurity.StartIP;
					dataRow15["EndIP"] = iPSecurity.EndIP;
					dataSet.Tables["IP_Security"].Rows.Add(dataRow15);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			dataSet.Tables.Add("Filters");
			dataSet.Tables["Filters"].Columns.Add("FilterID");
			dataSet.Tables["Filters"].Columns.Add("Assembly");
			dataSet.Tables["Filters"].Columns.Add("ClassName");
			dataSet.Tables["Filters"].Columns.Add("Enabled");
			dataSet.Tables["Filters"].Columns.Add("Description");
			enumerator = this.Filters.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Filter filter = (Filter)enumerator.Current;
					DataRow dataRow16 = dataSet.Tables["Filters"].NewRow();
					dataRow16["FilterID"] = filter.ID;
					dataRow16["Assembly"] = filter.AssemblyName;
					dataRow16["ClassName"] = filter.Class;
					dataRow16["Enabled"] = filter.Enabled;
					dataRow16["Description"] = filter.Description;
					dataSet.Tables["Filters"].Rows.Add(dataRow16);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			dataSet.Tables.Add("UsersDefaultFolders");
			dataSet.Tables["UsersDefaultFolders"].Columns.Add("FolderName");
			dataSet.Tables["UsersDefaultFolders"].Columns.Add("Permanent");
			enumerator = this.UsersDefaultFolders.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					UsersDefaultFolder usersDefaultFolder = (UsersDefaultFolder)enumerator.Current;
					DataRow dataRow17 = dataSet.Tables["UsersDefaultFolders"].NewRow();
					dataRow17["FolderName"] = usersDefaultFolder.FolderName;
					dataRow17["Permanent"] = usersDefaultFolder.Permanent;
					dataSet.Tables["UsersDefaultFolders"].Rows.Add(dataRow17);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			dataSet.WriteXml(stream);
		}

		public void Restore(string fileName, RestoreFlags_enum restoreFlags)
		{
			using (FileStream fileStream = File.OpenRead(fileName))
			{
				this.Restore(fileStream, restoreFlags);
			}
		}

		public void Restore(Stream stream, RestoreFlags_enum restoreFlags)
		{
			DataSet dataSet = new DataSet();
			dataSet.ReadXml(stream);
			if (dataSet.Tables.Contains("Settings"))
			{
				this.SystemSettings.LoadSettings(dataSet);
				this.SystemSettings.Commit();
			}
			if (dataSet.Tables.Contains("Domains"))
			{
				foreach (DataRow dataRow in dataSet.Tables["Domains"].Rows)
				{
					if (this.Domains.Contains(dataRow["DomainName"].ToString()))
					{
						if ((restoreFlags & RestoreFlags_enum.Replace) != (RestoreFlags_enum)0)
						{
						}
					}
					else if ((restoreFlags & RestoreFlags_enum.Add) != (RestoreFlags_enum)0)
					{
						this.Domains.Add(dataRow["DomainName"].ToString(), dataRow["Description"].ToString());
					}
				}
			}
			if (dataSet.Tables.Contains("Users"))
			{
				foreach (DataRow dataRow2 in dataSet.Tables["Users"].Rows)
				{
					User user = null;
					bool flag = false;
					if (this.Users.Contains(dataRow2["UserName"].ToString()))
					{
						if ((restoreFlags & RestoreFlags_enum.Replace) != (RestoreFlags_enum)0)
						{
							user = this.Users.GetUserByName(dataRow2["UserName"].ToString());
							user.UserName = dataRow2["UserName"].ToString();
							user.FullName = dataRow2["FullName"].ToString();
							user.Password = dataRow2["Password"].ToString();
							user.Description = dataRow2["Description"].ToString();
							user.MaximumMailboxSize = ConvertEx.ToInt32(dataRow2["Mailbox_Size"]);
							user.Enabled = ConvertEx.ToBoolean(dataRow2["Enabled"]);
							user.Permissions = (UserPermissions)ConvertEx.ToInt32(dataRow2["Permissions"]);
							user.Commit();
							string[] array = user.EmailAddresses.ToArray();
							for (int i = 0; i < array.Length; i++)
							{
								string emailAddress = array[i];
								user.EmailAddresses.Remove(emailAddress);
							}
							UserRemoteServer[] array2 = user.RemoteServers.ToArray();
							for (int j = 0; j < array2.Length; j++)
							{
								UserRemoteServer remoteServer = array2[j];
								user.RemoteServers.Remove(remoteServer);
							}
							UserMessageRule[] array3 = user.MessageRules.ToArray();
							for (int k = 0; k < array3.Length; k++)
							{
								UserMessageRule rule = array3[k];
								user.MessageRules.Remove(rule);
							}
							flag = true;
						}
					}
					else if ((restoreFlags & RestoreFlags_enum.Add) != (RestoreFlags_enum)0)
					{
						user = this.Users.Add(dataRow2["UserName"].ToString(), dataRow2["FullName"].ToString(), dataRow2["Password"].ToString(), dataRow2["Description"].ToString(), ConvertEx.ToInt32(dataRow2["Mailbox_Size"]), ConvertEx.ToBoolean(dataRow2["Enabled"]), (UserPermissions)ConvertEx.ToInt32(dataRow2["Permissions"]));
						flag = true;
					}
					if (flag)
					{
						if (dataSet.Tables.Contains("User_EmailAddresses"))
						{
							foreach (DataRowView dataRowView in new DataView(dataSet.Tables["User_EmailAddresses"])
							{
								RowFilter = "UserID='" + dataRow2["UserID"].ToString() + "'"
							})
							{
								user.EmailAddresses.Add(dataRowView["EmailAddress"].ToString());
							}
						}
						if (dataSet.Tables.Contains("User_RemoteServers"))
						{
							foreach (DataRowView dataRowView2 in new DataView(dataSet.Tables["User_RemoteServers"])
							{
								RowFilter = "UserID='" + dataRow2["UserID"].ToString() + "'"
							})
							{
								user.RemoteServers.Add(dataRowView2["Description"].ToString(), dataRowView2["RemoteServer"].ToString(), ConvertEx.ToInt32(dataRowView2["RemotePort"]), ConvertEx.ToBoolean(dataRowView2["UseSSL"]), dataRowView2["RemoteUserName"].ToString(), dataRowView2["RemotePassword"].ToString(), ConvertEx.ToBoolean(dataRowView2["Enabled"]));
							}
						}
						if (dataSet.Tables.Contains("User_MessageRules"))
						{
							foreach (DataRowView dataRowView3 in new DataView(dataSet.Tables["User_MessageRules"])
							{
								RowFilter = "UserID='" + dataRow2["UserID"].ToString() + "'"
							})
							{
								UserMessageRule userMessageRule = user.MessageRules.Add(ConvertEx.ToBoolean(dataRowView3["Enabled"]), dataRowView3["Description"].ToString(), dataRowView3["MatchExpression"].ToString(), (GlobalMessageRule_CheckNextRule)ConvertEx.ToInt32(dataRowView3["CheckNextRuleIf"]));
								if (dataSet.Tables.Contains("User_MessageRuleActions"))
								{
									foreach (DataRowView dataRowView4 in new DataView(dataSet.Tables["User_MessageRuleActions"])
									{
										RowFilter = string.Concat(new string[]
										{
											"UserID='",
											dataRow2["UserID"].ToString(),
											"' AND RuleID='",
											dataRowView3["RuleID"].ToString(),
											"'"
										})
									})
									{
										userMessageRule.Actions.Add(dataRowView4["ActionID"].ToString(), dataRowView4["Description"].ToString(), (UserMessageRuleActionType)Convert.ToInt32(dataRowView4["ActionType"]), Convert.FromBase64String(dataRowView4["ActionData"].ToString()), true);
									}
								}
							}
						}
					}
				}
			}
			if (dataSet.Tables.Contains("Groups"))
			{
				foreach (DataRow dataRow3 in dataSet.Tables["Groups"].Rows)
				{
					bool flag2 = false;
					if (this.Groups.Contains(dataRow3["GroupName"].ToString()) && (restoreFlags & RestoreFlags_enum.Replace) != (RestoreFlags_enum)0)
					{
						this.Groups.Remove(this.Groups.GetGroupByName(dataRow3["GroupName"].ToString()));
						flag2 = true;
					}
					if (!this.Groups.Contains(dataRow3["GroupName"].ToString()) && (flag2 || (restoreFlags & RestoreFlags_enum.Add) != (RestoreFlags_enum)0))
					{
						Group group = this.Groups.Add(dataRow3["GroupName"].ToString(), dataRow3["Description"].ToString(), ConvertEx.ToBoolean(dataRow3["Enabled"]));
						if (dataSet.Tables.Contains("Group_Members"))
						{
							foreach (DataRowView dataRowView5 in new DataView(dataSet.Tables["Group_Members"])
							{
								RowFilter = "GroupID='" + dataRow3["GroupID"].ToString() + "'"
							})
							{
								group.Members.Add(dataRowView5["UserOrGroup"].ToString());
							}
						}
					}
				}
			}
			if (dataSet.Tables.Contains("MailingLists"))
			{
				foreach (DataRow dataRow4 in dataSet.Tables["MailingLists"].Rows)
				{
					bool flag3 = false;
					if (this.MailingLists.Contains(dataRow4["MailingListName"].ToString()) && (restoreFlags & RestoreFlags_enum.Replace) != (RestoreFlags_enum)0)
					{
						this.MailingLists.Remove(this.MailingLists.GetMailingListByName(dataRow4["MailingListName"].ToString()));
						flag3 = true;
					}
					if (!this.MailingLists.Contains(dataRow4["MailingListName"].ToString()) && (flag3 || (restoreFlags & RestoreFlags_enum.Add) != (RestoreFlags_enum)0))
					{
						MailingList mailingList = this.MailingLists.Add(dataRow4["MailingListName"].ToString(), dataRow4["Description"].ToString(), ConvertEx.ToBoolean(dataRow4["Enabled"]));
						if (dataSet.Tables.Contains("MailingList_Members"))
						{
							foreach (DataRowView dataRowView6 in new DataView(dataSet.Tables["MailingList_Members"])
							{
								RowFilter = "MailingListID='" + dataRow4["MailingListID"].ToString() + "'"
							})
							{
								mailingList.Members.Add(dataRowView6["Address"].ToString());
							}
						}
						if (dataSet.Tables.Contains("MailingList_ACL"))
						{
							foreach (DataRowView dataRowView7 in new DataView(dataSet.Tables["MailingList_ACL"])
							{
								RowFilter = "MailingListID='" + dataRow4["MailingListID"].ToString() + "'"
							})
							{
								mailingList.ACL.Add(dataRowView7["UserOrGroup"].ToString());
							}
						}
					}
				}
			}
			if (dataSet.Tables.Contains("Routing"))
			{
				foreach (DataRow dataRow5 in dataSet.Tables["Routing"].Rows)
				{
					bool flag4 = false;
					if (this.Routes.ContainsPattern(dataRow5["Pattern"].ToString()) && (restoreFlags & RestoreFlags_enum.Replace) != (RestoreFlags_enum)0)
					{
						this.Routes.Remove(this.Routes.GetRouteByPattern(dataRow5["Pattern"].ToString()));
						flag4 = true;
					}
					if (!this.Routes.ContainsPattern(dataRow5["Pattern"].ToString()) && (flag4 || (restoreFlags & RestoreFlags_enum.Add) != (RestoreFlags_enum)0))
					{
						RouteAction routeAction_enum = (RouteAction)Convert.ToInt32(dataRow5["Action"]);
						RouteActionBase action = null;
						if (routeAction_enum == RouteAction.RouteToEmail)
						{
							action = new RouteAction_RouteToEmail(Convert.FromBase64String(dataRow5["ActionData"].ToString()));
						}
						else if (routeAction_enum == RouteAction.RouteToHost)
						{
							action = new RouteAction_RouteToHost(Convert.FromBase64String(dataRow5["ActionData"].ToString()));
						}
						else if (routeAction_enum == RouteAction.RouteToMailbox)
						{
							action = new RouteAction_RouteToMailbox(Convert.FromBase64String(dataRow5["ActionData"].ToString()));
						}
						this.Routes.Add(dataRow5["Description"].ToString(), dataRow5["Pattern"].ToString(), ConvertEx.ToBoolean(dataRow5["Enabled"]), action);
					}
				}
			}
			if (dataSet.Tables.Contains("GlobalMessageRules"))
			{
				IEnumerator enumerator;
				if ((restoreFlags & RestoreFlags_enum.Replace) != (RestoreFlags_enum)0)
				{
					enumerator = this.GlobalMessageRules.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							GlobalMessageRule rule2 = (GlobalMessageRule)enumerator.Current;
							this.GlobalMessageRules.Remove(rule2);
						}
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
				DataView dataView = new DataView(dataSet.Tables["GlobalMessageRules"]);
				enumerator = dataView.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DataRowView dataRowView8 = (DataRowView)enumerator.Current;
						GlobalMessageRule globalMessageRule = this.GlobalMessageRules.Add(ConvertEx.ToBoolean(dataRowView8["Enabled"]), dataRowView8["Description"].ToString(), dataRowView8["MatchExpression"].ToString(), (GlobalMessageRule_CheckNextRule)ConvertEx.ToInt32(dataRowView8["CheckNextRuleIf"]));
						if (dataSet.Tables.Contains("GlobalMessageRuleActions"))
						{
							foreach (DataRowView dataRowView9 in new DataView(dataSet.Tables["GlobalMessageRuleActions"])
							{
								RowFilter = "RuleID='" + dataRowView8["RuleID"].ToString() + "'"
							})
							{
								globalMessageRule.Actions.Add(dataRowView9["ActionID"].ToString(), dataRowView9["Description"].ToString(), (GlobalMessageRuleActionType)Convert.ToInt32(dataRowView9["ActionType"]), Convert.FromBase64String(dataRowView9["ActionData"].ToString()), true);
							}
						}
					}
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
			if (dataSet.Tables.Contains("IP_Security"))
			{
				IPSecurity[] array4 = this.IpSecurity.ToArray();
				for (int i = 0; i < array4.Length; i++)
				{
					IPSecurity entry = array4[i];
					this.IpSecurity.Remove(entry);
				}
				foreach (DataRow dataRow6 in dataSet.Tables["IP_Security"].Rows)
				{
					this.IpSecurity.Add(ConvertEx.ToBoolean(dataRow6["Enabled"]), dataRow6["Description"].ToString(), (ServiceKind)ConvertEx.ToInt32(dataRow6["Service"]), (IPSecurityAction)ConvertEx.ToInt32(dataRow6["Action"]), IPAddress.Parse(dataRow6["StartIP"].ToString()), IPAddress.Parse(dataRow6["EndIP"].ToString()));
				}
			}
			if (dataSet.Tables.Contains("Filters"))
			{
				Filter[] array5 = this.Filters.ToArray();
				for (int i = 0; i < array5.Length; i++)
				{
					Filter filter = array5[i];
					this.Filters.Remove(filter);
				}
				foreach (DataRow dataRow7 in dataSet.Tables["Filters"].Rows)
				{
					this.Filters.Add(ConvertEx.ToBoolean(dataRow7["Enabled"]), dataRow7["Description"].ToString(), dataRow7["Assembly"].ToString(), dataRow7["ClassName"].ToString());
				}
			}
			if (dataSet.Tables.Contains("UsersDefaultFolders"))
			{
				foreach (DataRow dataRow8 in dataSet.Tables["UsersDefaultFolders"].Rows)
				{
					if (this.UsersDefaultFolders.Contains(dataRow8["FolderName"].ToString()))
					{
						if ((restoreFlags & RestoreFlags_enum.Replace) != (RestoreFlags_enum)0)
						{
							this.UsersDefaultFolders.Remove(this.UsersDefaultFolders.GetFolderByName(dataRow8["FolderName"].ToString()));
							this.UsersDefaultFolders.Add(dataRow8["FolderName"].ToString(), ConvertEx.ToBoolean(dataRow8["Permanent"]));
						}
					}
					else if ((restoreFlags & RestoreFlags_enum.Add) != (RestoreFlags_enum)0)
					{
						this.UsersDefaultFolders.Add(dataRow8["FolderName"].ToString(), ConvertEx.ToBoolean(dataRow8["Permanent"]));
					}
				}
			}
		}

		public void Commit()
		{
			if (!this.m_ValuesChanged)
			{
				return;
			}
			this.m_pServer.TCP_Client.TcpStream.WriteLine(string.Concat(new object[]
			{
				"UpdateVirtualServer ",
				this.m_VirtualServerID,
				" ",
				this.m_Enabled,
				" ",
				TextUtils.QuoteString(this.m_Name),
				" ",
				TextUtils.QuoteString(Convert.ToBase64String(Encoding.UTF8.GetBytes(this.m_InitString)))
			}));
			string text = this.m_pServer.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
		}

		internal void DomainChanged()
		{
			this.m_pUsers = null;
			this.m_pMailingLists = null;
		}
	}
}
