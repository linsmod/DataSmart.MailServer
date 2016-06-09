using System.NetworkToolkit;
using System.NetworkToolkit.IMAP;
using System.NetworkToolkit.IMAP.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;

namespace DataSmart.MailServer
{
	public interface IMailServerManagementApi
	{

		DataView GetDomains();

		void AddDomain(string domainID, string domainName, string description);

		void DeleteDomain(string domainID);

		void UpdateDomain(string domainID, string domainName, string description);

		bool DomainExists(string source);

		DataView GetUsers(string domainName);

		string GetUserID(string userName);

		void AddUser(string userID, string userName, string fullName, string password, string description, string domainName, int mailboxSize, bool enabled, UserPermissions permissions);

		void DeleteUser(string userID);

		void UpdateUser(string userID, string userName, string fullName, string password, string description, string domainName, int mailboxSize, bool enabled, UserPermissions permissions);

		void AddUserAddress(string userName, string emailAddress);

		void DeleteUserAddress(string emailAddress);

		DataView GetUserAddresses(string userName);

		bool UserExists(string userName);

		string MapUser(string emailAddress);

		bool ValidateMailboxSize(string userName);

		UserPermissions GetUserPermissions(string userName);

		DateTime GetUserLastLoginTime(string userName);

		void UpdateUserLastLoginTime(string userName);

		DataView GetUserRemoteServers(string userName);

		void AddUserRemoteServer(string serverID, string userName, string description, string remoteServer, int remotePort, string remoteUser, string remotePassword, bool useSSL, bool enabled);

		void DeleteUserRemoteServer(string serverID);

		void UpdateUserRemoteServer(string serverID, string userName, string description, string remoteServer, int remotePort, string remoteUser, string remotePassword, bool useSSL, bool enabled);

		DataView GetUserMessageRules(string userName);

		void AddUserMessageRule(string userID, string ruleID, long cost, bool enabled, GlobalMessageRule_CheckNextRule checkNextRule, string description, string matchExpression);

		void DeleteUserMessageRule(string userID, string ruleID);

		void UpdateUserMessageRule(string userID, string ruleID, long cost, bool enabled, GlobalMessageRule_CheckNextRule checkNextRule, string description, string matchExpression);

		DataView GetUserMessageRuleActions(string userID, string ruleID);

		void AddUserMessageRuleAction(string userID, string ruleID, string actionID, string description, GlobalMessageRuleActionType actionType, byte[] actionData);

		void DeleteUserMessageRuleAction(string userID, string ruleID, string actionID);

		void UpdateUserMessageRuleAction(string userID, string ruleID, string actionID, string description, GlobalMessageRuleActionType actionType, byte[] actionData);

		DataSet AuthUser(string userName, string passwData, string authData, AuthType authType);

		bool GroupExists(string groupName);

		DataView GetGroups();

		void AddGroup(string groupID, string groupName, string description, bool enabled);

		void DeleteGroup(string groupID);

		void UpdateGroup(string groupID, string groupName, string description, bool enabled);

		bool GroupMemberExists(string groupName, string userOrGroup);

		string[] GetGroupMembers(string groupName);

		void AddGroupMember(string groupName, string userOrGroup);

		void DeleteGroupMember(string groupName, string userOrGroup);

		string[] GetGroupUsers(string groupName);

		DataView GetMailingLists(string domainName);

		void AddMailingList(string mailingListID, string mailingListName, string description, string domainName, bool enabled);

		void DeleteMailingList(string mailingListID);

		void UpdateMailingList(string mailingListID, string mailingListName, string description, string domainName, bool enabled);

		void AddMailingListAddress(string addressID, string mailingListName, string address);

		void DeleteMailingListAddress(string addressID);

		DataView GetMailingListAddresses(string mailingListName);

		DataView GetMailingListACL(string mailingListName);

		void AddMailingListACL(string mailingListName, string userOrGroup);

		void DeleteMailingListACL(string mailingListName, string userOrGroup);

		bool CanAccessMailingList(string mailingListName, string user);

		bool MailingListExists(string mailingListName);

		DataView GetGlobalMessageRules();

		void AddGlobalMessageRule(string ruleID, long cost, bool enabled, GlobalMessageRule_CheckNextRule checkNextRule, string description, string matchExpression);

		void DeleteGlobalMessageRule(string ruleID);

		void UpdateGlobalMessageRule(string ruleID, long cost, bool enabled, GlobalMessageRule_CheckNextRule checkNextRule, string description, string matchExpression);

		DataView GetGlobalMessageRuleActions(string ruleID);

		void AddGlobalMessageRuleAction(string ruleID, string actionID, string description, GlobalMessageRuleActionType actionType, byte[] actionData);

		void DeleteGlobalMessageRuleAction(string ruleID, string actionID);

		void UpdateGlobalMessageRuleAction(string ruleID, string actionID, string description, GlobalMessageRuleActionType actionType, byte[] actionData);

		DataView GetRoutes();

		void AddRoute(string routeID, long cost, bool enabled, string description, string pattern, RouteAction action, byte[] actionData);

		void DeleteRoute(string routeID);

		void UpdateRoute(string routeID, long cost, bool enabled, string description, string pattern, RouteAction action, byte[] actionData);

		void GetMessagesInfo(string accessingUser, string folderOwnerUser, string folder, List<IMAP_MessageInfo> messageInfos);

		void StoreMessage(string accessingUser, string folderOwnerUser, string folder, Stream msgStream, DateTime date, string[] flags);

		void StoreMessageFlags(string accessingUser, string folderOwnerUser, string folder, IMAP_MessageInfo messageInfo, string[] flags);

		void DeleteMessage(string accessingUser, string folderOwnerUser, string folder, string messageID, int uid);

		void GetMessageItems(string accessingUser, string folderOwnerUser, string folder, EmailMessageItems e);

		byte[] GetMessageTopLines(string accessingUser, string folderOwnerUser, string folder, string msgID, int nrLines);

		void CopyMessage(string accessingUser, string folderOwnerUser, string folder, string destFolderUser, string destFolder, IMAP_MessageInfo messageInfo);

		void Search(string accessingUser, string folderOwnerUser, string folder, IMAP_e_Search e);

		string[] GetFolders(string userName, bool includeSharedFolders);

		string[] GetSubscribedFolders(string userName);

		void SubscribeFolder(string userName, string folder);

		void UnSubscribeFolder(string userName, string folder);

		void CreateFolder(string accessingUser, string folderOwnerUser, string folder);

		void DeleteFolder(string accessingUser, string folderOwnerUser, string folder);

		void RenameFolder(string accessingUser, string folderOwnerUser, string folder, string newFolder);

		bool FolderExists(string folderName);

		DateTime FolderCreationTime(string folderOwnerUser, string folder);

		SharedFolderRoot[] GetSharedFolderRoots();

		void AddSharedFolderRoot(string rootID, bool enabled, string folder, string description, SharedFolderRootType rootType, string boundedUser, string boundedFolder);

		void DeleteSharedFolderRoot(string rootID);

		void UpdateSharedFolderRoot(string rootID, bool enabled, string folder, string description, SharedFolderRootType rootType, string boundedUser, string boundedFolder);

		DataView GetFolderACL(string accessingUser, string folderOwnerUser, string folder);

		void DeleteFolderACL(string accessingUser, string folderOwnerUser, string folder, string userOrGroup);

		void SetFolderACL(string accessingUser, string folderOwnerUser, string folder, string userOrGroup, IMAP_Flags_SetType setType, IMAP_ACL_Flags aclFlags);

		IMAP_ACL_Flags GetUserACL(string folderOwnerUser, string folder, string user);

		void CreateUserDefaultFolders(string userName);

		DataView GetUsersDefaultFolders();

		void AddUsersDefaultFolder(string folderName, bool permanent);

		void DeleteUsersDefaultFolder(string folderName);

		long GetMailboxSize(string userName);

		DataTable GetRecycleBinSettings();

		void UpdateRecycleBinSettings(bool deleteToRecycleBin, int deleteMessagesAfter);

		DataView GetRecycleBinMessagesInfo(string user, DateTime startDate, DateTime endDate);

		Stream GetRecycleBinMessage(string messageID);

		void DeleteRecycleBinMessage(string messageID);

		void RestoreRecycleBinMessage(string messageID);

		DataView GetSecurityList();

		void AddSecurityEntry(string id, bool enabled, string description, ServiceType service, IPSecurityAction action, IPAddress startIP, IPAddress endIP);

		void DeleteSecurityEntry(string id);

		void UpdateSecurityEntry(string id, bool enabled, string description, ServiceType service, IPSecurityAction action, IPAddress startIP, IPAddress endIP);

		DataView GetFilters();

		void AddFilter(string filterID, string description, string type, string assembly, string className, long cost, bool enabled);

		void DeleteFilter(string filterID);

		void UpdateFilter(string filterID, string description, string type, string assembly, string className, long cost, bool enabled);

		DataRow GetSettings();

		void UpdateSettings(DataRow settings);
	}
}
