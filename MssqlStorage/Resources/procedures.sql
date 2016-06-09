

CREATE PROCEDURE [dbo].[lspr_AddDomain] 
	@DomainID    nvarchar(100) = NULL,
	@DomainName  nvarchar(100) = NULL,
	@Description nvarchar(100) = NULL
AS

set nocount on

if(not exists(select * from lsDomains where (DomainID=@DomainID)))
begin
	if(not exists(select * from lsDomains where (DomainName=@DomainName)))
	begin
		insert lsDomains (DomainID,DomainName,Description) values (@DomainID,@DomainName,@Description)

		select null as ErrorText
	end
	else
	begin
		select 'Domain with specified name "' + @DomainName + '" already exists !' as ErrorText
	end
end
else
begin
	select 'Domain with specified ID "' + @DomainID + '" already exists !' as ErrorText
end


GO


CREATE PROCEDURE [dbo].[lspr_AddFilter]
	@FilterID    nvarchar(100) = NULL,
	@Description nvarchar(100) = NULL,
	@Type        nvarchar(100) = NULL,
	@Assembly    nvarchar(100) = NULL,
	@ClassName   nvarchar(100) = NULL,
	@Cost        bigint        = 0,
	@Enabled     bit           = true
AS

set nocount on

if(not exists(select * from lsFilters where (FilterID=@FilterID)))
begin
	insert lsFilters (FilterID,Description,Type,Assembly,ClassName,Cost,Enabled) 
	values (@FilterID,@Description,@Type,@Assembly,@ClassName,@Cost,@Enabled)

	select null as ErrorText
end
else
begin
	select 'Filter with specified ID "' + @FilterID + '" already exists !' as ErrorText
end


GO


CREATE PROCEDURE [dbo].[lspr_AddGlobalMessageRule]
	@ruleID          nvarchar(100) = NULL,
	@cost            bigint        = NULL,
	@enabled         bit           = NULL,
	@checkNextRule   int           = NULL,
	@description     nvarchar(400) = NULL,
	@matchExpression image         = NULL
AS
BEGIN
	if(not exists(select * from lsGlobalMessageRules where (RuleID = @ruleID)))
	begin
		insert lsGlobalMessageRules (RuleID,Cost,Enabled,CheckNextRuleIf,Description,MatchExpression) 
			values (@ruleID,@cost,@enabled,@checkNextRule,@description,@matchExpression)

		select null as ErrorText
	end
	else
	begin
		select 'Rule with specified ID "' + @ruleID + '" already exists !' as ErrorText
	end
END


GO


CREATE PROCEDURE [dbo].[lspr_AddGlobalMessageRuleAction]
	@ruleID          nvarchar(100) = NULL,
	@actionID        nvarchar(100) = NULL,
	@description     nvarchar(400) = NULL,
	@actionType      int           = NULL,
	@actionData      image         = NULL
AS
BEGIN
	if(not exists(select * from lsGlobalMessageRuleActions where (RuleID = @ruleID AND ActionID = @actionID)))
	begin
		insert lsGlobalMessageRuleActions (RuleID,ActionID,Description,ActionType,ActionData) 
			values (@ruleID,@actionID,@description,@actionType,@actionData)

		select null as ErrorText
	end
	else
	begin
		select 'Action with specified ID "' + @actionID + '" already exists !' as ErrorText
	end
END


GO


/*   Implementation notes:
      Decsription:
	    Adds new user group
	  Returns:
		If successful returns nothing, otherwise returns 1 row with error text in column 'ErrorText'.

	  Implementation:
		*) Ensure that group ID won't exist already. Return error text.
        *) Ensure that group or user with specified name doesn't exist. Return error text.
        *) Add group.
		 
*/

CREATE PROCEDURE [dbo].[lspr_AddGroup]
	@groupID     nvarchar(100) = NULL,
	@groupName   nvarchar(100) = NULL,
	@description nvarchar(400) = NULL,
	@enabled     bit           = NULL
AS
BEGIN
	-- Ensure that group ID won't exist already. 
	if(exists(select * from lsGroups where (GroupID = @groupID)))
	begin
		select 'Invalid group ID, specified group ID ''' + @groupID + ''' already exists !' as ErrorText
		return;
	end

	-- Ensure that group name won't exist already.
	if(exists(select * from lsGroups where (GroupName = @groupName)))
	begin
		select 'Invalid group name, specified group ''' + @groupName + ''' already exists !' as ErrorText
		return;
	end
	-- Ensure that user name with groupName doen't exist.
	else if exists(select * from lsUsers where (UserName = @groupName))
	begin
		select 'Invalid group name, user with specified name ''' + @groupName + ''' already exists !' as ErrorText
		return;
	end
	
	-- Insert group
	insert lsGroups (GroupID,GroupName,Description,Enabled) 
		values (@groupID,@groupName,@description,@enabled)

	select null as ErrorText
END


GO


/*  Implementation notes:
      Decsription:
	    Adds new user group member.
	  Returns:
		If successful returns nothing, otherwise returns 1 row with error text in column 'ErrorText'.

	  Implementation:
		*) Ensure that group exists. Return error text.
        *) Don't allow to add same group as group member. Return error text.
        *) Ensure that group member doesn't exist. Return error text.
        *) Add group member.
		 
*/

CREATE PROCEDURE [dbo].[lspr_AddGroupMember]
	@groupName   nvarchar(100) = NULL,
	@userOrGroup nvarchar(100) = NULL
AS
BEGIN	
	-- Ensure that group exists.
	if(not exists(select * from lsGroups where (GroupName = @groupName)))
	begin
		select 'Invalid group name, specified group ''' + @groupName + ''' doesn''t exist !' as ErrorText
		return;
	end

	-- Don't allow to add same group as group member.
	if(@groupName = @userOrGroup)
	begin
		select 'Invalid group member, can''t add goup itself as same group member !' as ErrorText
		return;
	end

	-- Get groupID
	declare @groupID as nvarchar(100)
	select @groupID = (select GroupID from lsGroups where (GroupName = @groupName))

	-- Ensure that group member doesn't exist.
	if(exists(select * from lsGroupMembers where (GroupID = @groupID AND UserOrGroup = @userOrGroup)))
	begin
		select 'Invalid group member, specified group member ''' + @userOrGroup + ''' already exists !' as ErrorText
		return;
	end
	
	-- Insert group member
	insert lsGroupMembers (GroupID,UserOrGroup) 
		values (@groupID,@userOrGroup)
	
	select null as ErrorText
END


GO


CREATE PROCEDURE [dbo].[lspr_AddMailingList]
	@MailingListID	 varchar(100) = NULL,
	@MailingListName varchar(100) = NULL,
	@Description     varchar(100) = NULL,
	@DomainName      varchar(100) = NULL,
	@enabled         bit          = false
AS

set nocount on

if(not exists(select * from lsMailingLists where (MailingListID=@MailingListID)))
begin
	if(not exists(select * from lsMailingLists where (MailingListName=@MailingListName)))
	begin
		insert lsMailingLists (MailingListID,MailingListName,Description,DomainName,Enabled) 
		values (@MailingListID,@MailingListName,@Description,@DomainName,@enabled)

		select null as ErrorText
	end
	else
	begin
		select 'Mailing list with specified name "' + @MailingListName + '" already exists !' as ErrorText
	end
end
else
begin
	select 'Mailing list with specified ID "' + @MailingListID + '" already exists !' as ErrorText
end


GO


/*  Implementation notes:
      Decsription:
	    Mailing list ACL entry
	  Returns:
		If successful returns nothing, otherwise returns 1 row with error text in column 'ErrorText'.

	  Implementation:
		*) Ensure that mailing list exists.
        *) Ensure that user or group already doesn't exist in list.
        *) Add ACL entry.
		 
*/

CREATE PROCEDURE [dbo].[lspr_AddMailingListACL]
	@mailingListName nvarchar(100) = NULL,
	@userOrGroup     nvarchar(100) = NULL
AS
BEGIN
	-- Ensure that mailing list exists.
	if(not exists(select * from lsMailingLists where (MailingListName = @mailingListName)))
	begin
		select 'Invalid mailing list name, specified mailing list ''' + @mailingListName + ''' doesn''t exist !' as ErrorText
		return;
	end

	-- Get mailing list ID
	declare @mailingListID as nvarchar(100)
	select @mailingListID = (select MailingListID from lsMailingLists where (MailingListName = @mailingListName))
	
	-- Ensure that user or group already doesn't exist in list.
	if(exists(select * from lsMailingListACL where (MailingListID = @mailingListID AND UserOrGroup = @userOrGroup)))
	begin
		select 'Invalid userOrGroup, specified userOrGroup ''' + @userOrGroup + '''already exist !' as ErrorText
		return;
	end

	
	-- Insert group
	insert lsMailingListACL (MailingListID,UserOrGroup) 
		values (@mailingListID,@userOrGroup)

	select null as ErrorText
END


GO


CREATE PROCEDURE [dbo].[lspr_AddMailingListAddress] 
	@AddressID       nvarchar(100) = NULL,
	@MailingListName nvarchar(100) = NULL,
	@Address         nvarchar(100) = NULL
AS

set nocount on

if(not exists(select * from lsMailingListAddresses where (AddressID=@AddressID)))
begin
	declare @MailingListID nvarchar(100)
	select @MailingListID = (select MailingListID from lsMailingLists where MailingListName=@MailingListName)

	if(not exists(select * from lsMailingListAddresses where (MailingListID=@MailingListID AND Address=@Address)))
	begin		
		insert lsMailingListAddresses (AddressID,MailingListID,Address) values (@AddressID,@MailingListID,@Address)

		select null as ErrorText
	end
	else
	begin
		select 'Mailing list address with specified name "' + @Address + '" already exists !' as ErrorText
	end
end
else
begin
	select 'Mailing list address with specified ID "' + @AddressID + '" already exists !' as ErrorText
end


GO


CREATE PROCEDURE [dbo].[lspr_AddRoute]
	@routeID     varchar(100) = NULL,
	@cost        bigint       = NULL,
	@enabled     bit          = NULL,
	@description varchar(100) = NULL,
	@pattern     varchar(100) = NULL,
	@action      int          = NULL,
	@actionData  image        = NULL
AS

set nocount on

if(not exists(select * from lsRouting where (RouteID=@routeID)))
begin
	if(not exists(select * from lsRouting where (Pattern=@pattern)))
	begin
		insert lsRouting (RouteID,Cost,Enabled,Description,Pattern,Action,ActionData) 
		values (@routeID,@cost,@enabled,@description,@pattern,@action,@actionData)

		select null as ErrorText
	end
	else
	begin
		select 'Route with specified pattern "' + @Pattern + '" already exists !' as ErrorText
	end
end
else
begin
	select 'Route with specified ID "' + @RouteID + '" already exists !' as ErrorText
end


GO


CREATE PROCEDURE [dbo].[lspr_AddSecurityEntry]
	@id          varchar(100) = NULL,
	@enabled     bit          = 1,
	@description varchar(100) = NULL,
	@service     varchar(100) = NULL,
	@action      varchar(100) = NULL,
	@startIP     varchar(100) = NULL,
	@endIP       varchar(100) = NULL
AS

set nocount on

if(not exists(select * from lsIPSecurity where (ID=@id)))
begin
	insert lsIPSecurity (ID,Enabled,Description,Service,Action,StartIP,EndIP) 
	values (@id,@enabled,@Description,@service,@action,@startIP,@endIP)

	select null as ErrorText
end
else
begin
	select 'Security entry with specified ID "' + @id + '" already exists !' as ErrorText
end


GO


/*  Implementation notes:
      Decsription:
	    Adds new shared folder root.
	  Returns:
		If successful returns nothing, otherwise returns 1 row with error text in column 'ErrorText'.

	  Implementation:
		*) Ensure that root doesn't exists.
        *) Add root folder.
		 
*/

CREATE PROCEDURE [dbo].[lspr_AddSharedFolderRoot]
	@rootID        nvarchar(100) = NULL,
	@enabled       bit           = NULL,
	@folder        nvarchar(400) = NULL,
	@description   nvarchar(400) = NULL,
	@rootType      int           = NULL,
	@boundedUser   nvarchar(100) = NULL,
	@boundedFolder nvarchar(400) = NULL
AS
BEGIN
	-- Ensure that root ID won't exist already. 
	if(exists(select * from lsSharedFoldersRoots where (RootID = @rootID)))
	begin
		select 'Invalid root ID, specified root ID ''' + @rootID + ''' already exists !' as ErrorText
		return;
	end

	-- Ensure that root folder name won't exist already.
	if(exists(select * from lsSharedFoldersRoots where (Folder = @folder)))
	begin
		select 'Invalid root folder name, specified folder ''' + @folder + ''' already exists !' as ErrorText
		return;
	end
	
	-- Insert root folder
	insert lsSharedFoldersRoots (RootID,Enabled,Folder,Description,RootType,BoundedUser,BoundedFolder) 
		values (@rootID,@enabled,@folder,@description,@rootType,@boundedUser,@boundedFolder)

	select null as ErrorText
END


GO


CREATE PROCEDURE [dbo].[lspr_AddUser]
	@UserID	     varchar(100) = NULL,
	@FullName    varchar(100) = NULL,
	@UserName    varchar(100) = NULL,
	@Password    varchar(100) = NULL,
	@Description varchar(100) = NULL,
	@DomainName  varchar(100) = NULL,
	@MailboxSize bigint	  = 0,
	@Enabled     bit          = true,
	@permissions int          = 255
AS

set nocount on

if(not exists(select * from lsUsers where (UserID=@UserID)))
begin
	if(not exists(select * from lsUsers where (UserName=@UserName)))
	begin
		insert lsUsers (UserID,FullName,UserName,Password,Description,Mailbox_Size,DomainName,Enabled,[Permissions],CreationTime) 
		values (@UserID,@FullName,@UserName,@Password,@Description,@MailboxSize,@DomainName,@Enabled,@permissions,getdate())

		select null as ErrorText
	end
	else
	begin
		select 'User with specified name "' + @UserName + '" already exists !' as ErrorText
	end
end
else
begin
	select 'User with specified ID "' + @UserID + '" already exists !' as ErrorText
end


GO


CREATE PROCEDURE [dbo].[lspr_AddUserAddress]
	@UserName  nvarchar(100) = NULL,
	@Address   nvarchar(100) = NULL
AS

set nocount on
BEGIN
	declare @UserID nvarchar(100)
	select @UserID = (select UserID from lsUsers where UserName=@UserName)

	if(not exists(select * from lsUserAddresses where (UserID=@UserID AND Address=@Address)))
	begin
		insert lsUserAddresses (UserID,Address) values (@UserID,@Address)

		select null as ErrorText
	end
	else
	begin
		select 'User address with specified name "' + @Address + '" already exists !' as ErrorText
	end
END


GO


CREATE PROCEDURE [dbo].[lspr_AddUserMessageRule]
	@userID          nvarchar(100) = NULL,
	@ruleID          nvarchar(100) = NULL,
	@cost            bigint        = NULL,
	@enabled         bit           = NULL,
	@checkNextRule   int           = NULL,
	@description     nvarchar(400) = NULL,
	@matchExpression image         = NULL
AS
BEGIN
	if(not exists(select * from lsUserMessageRules where (RuleID = @ruleID)))
	begin
		insert lsUserMessageRules (UserID,RuleID,Cost,Enabled,CheckNextRuleIf,Description,MatchExpression) 
			values (@userID,@ruleID,@cost,@enabled,@checkNextRule,@description,@matchExpression)

		select null as ErrorText
	end
	else
	begin
		select 'Rule with specified ID "' + @ruleID + '" already exists !' as ErrorText
	end
END


GO


CREATE PROCEDURE [dbo].[lspr_AddUserMessageRuleAction]
	@userID          nvarchar(100) = NULL,
	@ruleID          nvarchar(100) = NULL,
	@actionID        nvarchar(100) = NULL,
	@description     nvarchar(400) = NULL,
	@actionType      int           = NULL,
	@actionData      image         = NULL
AS
BEGIN
	if(not exists(select * from lsUserMessageRuleActions where (RuleID = @ruleID AND ActionID = @actionID)))
	begin
		insert lsUserMessageRuleActions (UserID,RuleID,ActionID,Description,ActionType,ActionData) 
			values (@userID,@ruleID,@actionID,@description,@actionType,@actionData)

		select null as ErrorText
	end
	else
	begin
		select 'Action with specified ID "' + @actionID + '" already exists !' as ErrorText
	end
END


GO


CREATE PROCEDURE [dbo].[lspr_AddUserRemoteServer] 
	@ServerID       nvarchar(100) = NULL,
	@UserName       nvarchar(100) = NULL,
	@Description    nvarchar(100) = NULL,
	@RemoteServer   nvarchar(100) = NULL,
	@RemotePort     int           = NULL,
	@RemoteUserName nvarchar(100) = NULL,
	@RemotePassword nvarchar(100) = NULL,
	@UseSSL         bit           = NULL,
	@Enabled        bit           = NULL
AS

set nocount on

if(not exists(select * from lsUserRemoteServers where (ServerID=@ServerID)))
begin
	-- Get userID
	declare @UserID nvarchar(100)
	select @UserID = (select UserID from lsUsers where UserName=@UserName)

	insert lsUserRemoteServers (
		ServerID,
		UserID,
		Description,
		RemoteServer,
		RemotePort,
		RemoteUserName,
		RemotePassword,
		UseSSL,
		Enabled
	) 
	values (
		@ServerID,
		@UserID,
		@Description,
		@RemoteServer,
		@RemotePort,
		@RemoteUserName,
		@RemotePassword,
		@UseSSL,
		@Enabled
	)

	select null as ErrorText
end
else
begin
	select 'User remote server with specified ID "' + @ServerID + '" already exists !' as ErrorText
end


GO


/* Adds users default folder.
    @folderName - Users default folder name.
    @permanent  - Specifies if folder is permanent, users can't delete it.
*/
CREATE PROCEDURE [dbo].[lspr_AddUsersDefaultFolder] 
    @folderName nvarchar(200),
    @permanent  bit
AS

IF(exists(select * from lsUsersDefaultFolders where (FolderName = @folderName)))
BEGIN
    select 'Users default folder with specified name ''' + @folderName + ''' already exists !' as ErrorText
    return;
END

insert into lsUsersDefaultFolders (FolderName,Permanent)
    values (@folderName,@permanent)

select null as ErrorText


GO


CREATE PROCEDURE [dbo].[lspr_CreateFolder] 
	@UserName nvarchar(100),
	@Folder   nvarchar(100)
AS


declare @UserID as uniqueidentifier
select @UserID = (select UserID from lsUsers where UserName = @UserName)

if exists(select * from  lsIMAPFolders where UserID = @UserID AND FolderName = @Folder)
begin
	select 'Folder(' + @Folder  + ') already exists' as ErrorText
end
else
begin
	insert into lsIMAPFolders (UserID,FolderName,CreationTime) values (@UserID,@Folder,getdate())
end


GO


CREATE PROCEDURE [dbo].[lspr_DeleteDomain]
	@DomainID nvarchar(100) = NULL
AS

declare @DomainName nvarchar(100)
select @DomainName = (select DomainName from lsDomains where DomainID = @DomainID)

--- Delete domain users ---------------------------------------------------------
declare rsUsers cursor for select UserID from lsUsers where DomainName=@DomainName
open rsUsers

declare @UserID nvarchar(100)
fetch next from  rsUsers into @UserID
while(@@FETCH_STATUS = 0)
begin
	exec lspr_DeleteUser @UserID=@UserID
	-- Get next data row
	fetch next from  rsUsers into @UserID
end
close rsUsers
deallocate rsUsers
---------------------------------------------------------------------------------

--- Delete domain mailing lists --------------------------------------------------
declare rsMailingLists cursor for select MailingListID from lsMailingLists where DomainName=@DomainName
open rsMailingLists

declare @MailingListID nvarchar(100)
fetch next from  rsMailingLists into @MailingListID
while(@@FETCH_STATUS = 0)
begin
	exec lspr_DeleteMailingList @MailingListID=@MailingListID
	-- Get next data row
	fetch next from  rsMailingLists into @MailingListID
end
close rsMailingLists
deallocate rsMailingLists
---------------------------------------------------------------------------------

delete from lsDomains where DomainID=@DomainID


GO


CREATE PROCEDURE [dbo].[lspr_DeleteFilter]
	@FilterID nvarchar(100) = NULL
AS

delete from lsFilters where (FilterID=@FilterID)


GO


CREATE PROCEDURE [dbo].[lspr_DeleteFolder] 
	@UserName nvarchar(100),
	@Folder   nvarchar(100)
AS

declare @UserID as uniqueidentifier
select @UserID = (select UserID from lsUsers where UserName = @UserName)

if exists(select * from  lsIMAPFolders where UserID = @UserID AND FolderName = @Folder)
begin
	-- Delete specified folder and it's subfolders messages
	delete lsMailStore where (Mailbox = @UserName AND Folder LIKE (@Folder + '%'))

	-- Delete folder and it's sub folders
	delete lsIMAPFolders where (UserID = @UserID AND FolderName LIKE (@Folder + '%'))

	-- Delete specified folder and it's subfolders ACL, if any
	delete lsIMAP_ACL where (Folder LIKE (@UserName + '/' + @Folder + '%'))
end
else
begin
	select 'Folder(' + @Folder  + ') doesn''t exist' as ErrorText	
end


GO


CREATE PROCEDURE [dbo].[lspr_DeleteFolderACL]
	@FolderName nvarchar(500) = NULL,
	@UserName   nvarchar(500) = NULL
AS

delete from lsIMAP_ACL where (Folder = @FolderName AND [User] = @UserName)


GO


CREATE PROCEDURE [dbo].[lspr_DeleteGlobalMessageRule]
	@ruleID nvarchar(100) = NULL
AS
BEGIN
	-- Delete all specified rule Actions
	delete from lsGlobalMessageRuleActions where (RuleID = @ruleID)

	delete from lsGlobalMessageRules where (RuleID = @ruleID)
END


GO


CREATE PROCEDURE [dbo].[lspr_DeleteGlobalMessageRuleAction]
	@ruleID   nvarchar(100) = NULL,
	@actionID nvarchar(100) = NULL
AS
BEGIN
	delete from lsGlobalMessageRuleActions where (RuleID = @ruleID AND ActionID = @actionID)
END


GO


/*  Implementation notes:
      Decsription:
	    Deletes user group
	  Returns:
		If successful returns nothing, otherwise returns 1 row with error text in column 'ErrorText'.

	  Implementation:
		*) Ensure that group ID exist. Return error text.
        *) Delete group members.
        *) Delete group.
		 
*/

CREATE PROCEDURE [dbo].[lspr_DeleteGroup]
	@groupID     nvarchar(100) = NULL
AS
BEGIN
	-- Ensure that group ID exist.
	if(not exists(select * from lsGroups where (GroupID = @groupID)))
	begin
		select 'Invalid group ID, specified group ID ''' + @groupID + ''' doesn''t exist !' as ErrorText
		return;
	end

	-- Delete group members.
	delete from lsGroupMembers where (GroupID = @groupID)

	-- Delete group.
	delete from lsGroups where (GroupID = @groupID)

	select null as ErrorText
END


GO


/*  Implementation notes:
      Decsription:
	    Deletes user group member
	  Returns:
		If successful returns nothing, otherwise returns 1 row with error text in column 'ErrorText'.

	  Implementation:
		*) Ensure that group exists. Return error text.
        *) Ensure that group member does exist. Return error text.
        *) Delete group member.
		 
*/

CREATE PROCEDURE [dbo].[lspr_DeleteGroupMember]
	@groupName   nvarchar(100) = NULL,
	@userOrGroup nvarchar(100) = NULL
AS
BEGIN
	-- Ensure that group exists.
	if(not exists(select * from lsGroups where (GroupName = @groupName)))
	begin
		select 'Invalid group name, specified group ''' + @groupName + ''' doesn''t exist !' as ErrorText
		return;
	end

	-- Ensure that group member does exist.
	if(not exists(select * from lsGroupMembers where (UserOrGroup = @userOrGroup)))
	begin
		select 'Invalid group member, specified group member ''' + @userOrGroup + ''' already exists !' as ErrorText
		return;
	end

	-- Delete group members.
	delete from lsGroupMembers where (UserOrGroup = @userOrGroup)

	select null as ErrorText
END


GO


CREATE PROCEDURE [dbo].[lspr_DeleteMailingList]
	@MailingListID	nvarchar(100) = NULL
AS

delete from lsMailingListAcl where (MailingListID=@MailingListID)
delete from lsMailingListAddresses where (MailingListID=@MailingListID)
delete from lsMailingLists where (MailingListID=@MailingListID)


GO


/*  Implementation notes:
      Decsription:
	    Deletes specified mailing list ACL entry.
	  Returns:
		If successful returns nothing, otherwise returns 1 row with error text in column 'ErrorText'.

	  Implementation:
		*) Ensure that mailing list exists.
        *) Delete ACL entry.
		 
*/

CREATE PROCEDURE [dbo].[lspr_DeleteMailingListACL]
	@mailingListName nvarchar(100) = NULL,
	@userOrGroup     nvarchar(100) = NULL
AS
BEGIN
	-- Ensure that mailing list exists.
	if(not exists(select * from lsMailingLists where (MailingListName = @mailingListName)))
	begin
		select 'Invalid mailing list name, specified mailing list ''' + @mailingListName + ''' doesn''t exist !' as ErrorText
		return;
	end

	-- Get mailing list ID
	declare @mailingListID as nvarchar(100)
	select @mailingListID = (select MailingListID from lsMailingLists where (MailingListName = @mailingListName))

	-- Delete ACL entry.
	delete from lsMailingListACL where (MailingListID = @mailingListID AND UserOrGroup = @userOrGroup)

	select null as ErrorText
END


GO


CREATE PROCEDURE [dbo].[lspr_DeleteMailingListAddress]
	@AddressID nvarchar(100) = NULL
AS

delete from lsMailingListAddresses where (AddressID=@AddressID)


GO


CREATE PROCEDURE [dbo].[lspr_DeleteMessage] 
    @MessageID uniqueidentifier = NULL,
    @Mailbox   nvarchar(100)    = NULL,
    @Folder    nvarchar(100)    = NULL
AS
BEGIN
    delete from lsMailStore 
        where MessageID = @MessageID AND Mailbox = @Mailbox AND Folder = @Folder
END


GO


/* Deletes specified recycle bin message.
    @messageID - Message ID which to delete.
*/
CREATE PROCEDURE [dbo].[lspr_DeleteRecycleBinMessage]
	@messageID nvarchar(100) = NULL
AS
BEGIN
	delete from lsRecycleBin where(MessageID = @messageID)
END


GO


CREATE PROCEDURE [dbo].[lspr_DeleteRoute]
	@RouteID nvarchar(100) = NULL
AS

delete from lsRouting where (RouteID=@RouteID)


GO


CREATE PROCEDURE [dbo].[lspr_DeleteSecurityEntry]
	@SecurityID nvarchar(100) = NULL
AS

delete from lsIPSecurity where (ID=@SecurityID)


GO


/*	Implementation notes:
      Decsription:
	    Deletes shared folder root.
	  Returns:
		If successful returns nothing, otherwise returns 1 row with error text in column 'ErrorText'.

	  Implementation:
		*) Ensure that root ID exist. Return error text.
        *) Delete root folder.
		 
*/

CREATE PROCEDURE [dbo].[lspr_DeleteSharedFolderRoot]
	@rootID nvarchar(100) = NULL
AS
BEGIN
	-- Ensure that root ID exist.
	if(not exists(select * from lsSharedFoldersRoots where (RootID = @rootID)))
	begin
		select 'Invalid root ID, specified root ID ''' + @rootID + ''' doesn''t exist !' as ErrorText
		return;
	end

	-- Delete group.
	delete from lsSharedFoldersRoots where (RootID = @rootID)

	select null as ErrorText
END


GO


CREATE PROCEDURE [dbo].[lspr_DeleteUser]
	@UserID	nvarchar(100) = NULL
AS

delete from lsUserAddresses where (UserID=@UserID)
delete from lsUserRemoteServers where (UserID=@UserID)
delete from lsUserMessageRules where (UserID=@UserID)
delete from lsIMAPSubscribedFolders where (UserID=@UserID)
delete from lsUsers where (UserID=@UserID)


GO


CREATE PROCEDURE [dbo].[lspr_DeleteUserAddress]
	@emailAddress nvarchar(100) = NULL
AS

delete from lsUserAddresses where (Address = @emailAddress)


GO


CREATE PROCEDURE [dbo].[lspr_DeleteUserMessageRule]
	@userID nvarchar(100) = NULL,
	@ruleID nvarchar(100) = NULL
AS
BEGIN
	-- Delete all specified rule Actions
	delete from lsUserMessageRuleActions where (UserID = @userID AND RuleID = @ruleID)

	delete from lsUserMessageRules where (UserID = @userID AND RuleID = @ruleID)
END


GO


CREATE PROCEDURE [dbo].[lspr_DeleteUserMessageRuleAction]
	@userID   nvarchar(100) = NULL,
	@ruleID   nvarchar(100) = NULL,
	@actionID nvarchar(100) = NULL
AS
BEGIN
	delete from lsUserMessageRuleActions where (UserID = @userID AND RuleID = @ruleID AND ActionID = @actionID)
END


GO


CREATE PROCEDURE [dbo].[lspr_DeleteUserRemoteServer]
	@ServerID nvarchar(100) = NULL
AS

delete from lsUserRemoteServers where (ServerID=@ServerID)


GO


/* Deletes users default folder.
    @folderName - Users default folder name which to delete.
*/
CREATE PROCEDURE [dbo].[lspr_DeleteUsersDefaultFolder]
	@folderName nvarchar(200)
AS
BEGIN
	-- Ensure that folder exist.
	if(not exists(select * from lsUsersDefaultFolders where (FolderName = @folderName)))
	begin
		select 'Users default folder with specified name ''' + @folderName + ''' doesn''t exists !' as ErrorText
		return;
	end

	-- Delete folder.
	delete from lsUsersDefaultFolders where (FolderName = @folderName)

	select null as ErrorText
END


GO


CREATE PROCEDURE [dbo].[lspr_DomainExists]
	@DomainName nvarchar(100) = NULL
AS

select * from lsDomains where (DomainName=@DomainName)


GO


CREATE PROCEDURE [dbo].[lspr_FolderExists]
	@FolderName nvarchar(500) = NULL,
	@UserName   nvarchar(100) = NULL
AS

if(exists (select * from lsIMAPFolders where (UserID=(select UserID from lsUsers where UserName = @UserName)  AND FolderName = @FolderName)))
begin 
	select * from lsIMAPFolders where (UserID=(select UserID from lsUsers where UserName = @UserName)  AND FolderName = @FolderName)
end
else
begin
	if(lower(@FolderName) = 'inbox')
	begin
		-- Create inbox, it's missing
		exec lspr_CreateFolder @UserName,'Inbox'

		select * from lsIMAPFolders where (UserID=(select UserID from lsUsers where UserName = @UserName)  AND FolderName = @FolderName)
	end
end


GO

CREATE PROCEDURE [dbo].[lspr_GetDomains]  
AS

select * from lsDomains


GO


CREATE PROCEDURE [dbo].[lspr_GetFilters]
AS

select * from lsFilters


GO


CREATE PROCEDURE [dbo].[lspr_GetFolderACL] 
	@FolderName nvarchar(500) = NULL
AS

if(@FolderName is not null)
begin
	select * from lsIMAP_ACL where (Folder = @FolderName)
end
else
begin
	select * from lsIMAP_ACL
end


GO


CREATE PROCEDURE [dbo].[lspr_GetFolders] 
	@UserName as nvarchar(100)
AS

if(exists (select * from lsIMAPFolders where UserID=(select UserID from lsUsers where UserName = @UserName)))
begin 
	select * from lsIMAPFolders where UserID=(select UserID from lsUsers where UserName = @UserName)
end
else
begin
	-- Create inbox, it's missing
	exec lspr_CreateFolder @UserName,'Inbox'

	select 'Inbox' as FolderName
end


GO


CREATE PROCEDURE [dbo].[lspr_GetGlobalMessageRuleActions]
	@ruleID nvarchar(100) = NULL
AS
BEGIN
	select * from lsGlobalMessageRuleActions where (RuleID = @ruleID)
END


GO


CREATE PROCEDURE [dbo].[lspr_GetGlobalMessageRules]
AS
BEGIN
	select * from lsGlobalMessageRules order by Cost ASC
END


GO


/*  Implementation notes:
      Decsription:
	     Gets user group members.
	  Returns:
		 Retruns user group members.
*/

CREATE PROCEDURE [dbo].[lspr_GetGroupMembers]
	@groupName   nvarchar(100) = NULL	
AS
BEGIN
	-- Get groupID
	declare @groupID as nvarchar(100)
	select @groupID = (select GroupID from lsGroups where (GroupName = @groupName))

	select * from lsGroupMembers where (GroupID = @groupID)
END


GO


/*  Implementation notes:
      Decsription:
	     Gets user groups.
	  Returns:
		 Retruns user groups.
*/

CREATE PROCEDURE [dbo].[lspr_GetGroups]	
AS
BEGIN
	select * from lsGroups
END


GO


CREATE PROCEDURE [dbo].[lspr_GetMailboxSize]
	@UserName nvarchar(100) = NULL
AS

set nocount on

declare @Size int
select  @Size = 0

-- Count mailbox size
if(exists(select MailBox from lsMailStore where Mailbox=@UserName))
begin
    select @Size = (select sum(Size) from lsMailStore where Mailbox=@UserName)
end

select @Size as MailboxSize


GO


/*  Implementation notes:
      Decsription:
	     Gets mailing list ACL list.
	  Returns:
		 Retruns mailing list ACL list.
*/

CREATE PROCEDURE [dbo].[lspr_GetMailingListACL]	
	@mailingListName as nvarchar(100)
AS
BEGIN
	select * from lsMailingListACL 
		where (MailingListID = (select MailingListID from lsMailingLists where (MailingListName = @mailingListName)))
END


GO


CREATE PROCEDURE [dbo].[lspr_GetMailingListAddresses]
	@MailingListName nvarchar(100) = NULL
AS

if(@MailingListName is not null)
begin
	declare @MailingListID nvarchar(100)
	select @MailingListID = (select MailingListID from lsMailingLists where MailingListName=@MailingListName)

	select * from lsMailingListAddresses where (MailingListID=@MailingListID)
end
else
begin
       select * from lsMailingListAddresses
end


GO


CREATE PROCEDURE [dbo].[lspr_GetMailingListProperties]
	@MailingListName nvarchar(100)	= NULL
AS

select * from lsMailingLists where (MailingListName = @MailingListName)


GO


CREATE PROCEDURE [dbo].[lspr_GetMailingLists]
	@DomainName nvarchar(100) = NULL
AS

if(@DomainName is not null)
begin
      select * from lsMailingLists where (DomainName=@DomainName)
end
else
begin
       select * from lsMailingLists
end


GO


CREATE PROCEDURE [dbo].[lspr_GetMessage]
	@MessageID uniqueidentifier = NULL,
	@Mailbox   nvarchar(100)    = NULL,
	@Folder    nvarchar(100)    = NULL
AS

select Data from lsMailStore where MessageID = @MessageID AND Mailbox = @Mailbox AND Folder = @Folder


GO


CREATE PROCEDURE [dbo].[lspr_GetMessageInfo]
	@Mailbox nvarchar(100)= NULL,
	@Folder	 nvarchar(100)= NULL
AS

select MessageID,Size,Date,MessageFlags,UID  from lsMailStore where MAILBOX = @Mailbox AND Folder = @Folder


GO


CREATE PROCEDURE [dbo].[lspr_GetMessageTopLines]
	@MessageID uniqueidentifier = NULL,
	@Mailbox   nvarchar(100)    = NULL,
	@Folder    nvarchar(100)    = NULL
AS
BEGIN
	select TopLines from lsMailStore where MessageID = @MessageID AND  Mailbox = @Mailbox AND Folder = @Folder
END


GO


/* Gets recycle bin message.
    @messageID - Recycle bin message ID.
*/
CREATE PROCEDURE [dbo].[lspr_GetRecycleBinMessage]
    @messageID uniqueidentifier = NULL
AS
BEGIN
	select * from lsRecycleBin where (MessageID = @messageID)
END


GO


/* Gets reycle bin messages info.
    @userName  - User who's recyclebin messages to get or null if all users messages.
    @startDate - Messages from specified date.
    @endDate   - Messages to specified date.
*/
CREATE PROCEDURE [dbo].[lspr_GetRecycleBinMessagesInfo]
    @userName  nvarchar(200) = NULL,
    @startDate datetime,
    @endDate   datetime
AS
BEGIN
    IF @userName is null
    BEGIN
        select MessageID,DeleteTime,[User],Folder,[Size],Envelope from lsRecycleBin where(@startDate <= CONVERT(varchar(8),DeleteTime,112) AND @endDate >= CONVERT(varchar(8),DeleteTime,112))
    END
    ELSE
    BEGIN
        select MessageID,DeleteTime,[User],Folder,[Size],Envelope from lsRecycleBin where([User] = @userName AND @startDate <= CONVERT(varchar(8),DeleteTime,112) AND @endDate >= CONVERT(varchar(8),DeleteTime,112))
    END
END


GO


/* Gets recycle bin settings.
*/
CREATE PROCEDURE [dbo].[lspr_GetRecycleBinSettings]	
AS
BEGIN
	select * from lsRecycleBinSettings
END


GO


CREATE PROCEDURE [dbo].[lspr_GetRoutes]
AS
begin

select * from lsRouting

end


GO


CREATE PROCEDURE [dbo].[lspr_GetSecurityList]
AS

select * from lsIPSecurity


GO


CREATE PROCEDURE [dbo].[lspr_GetSettings]  
AS

select * from lsSettings


GO


/*  Implementation notes:
      Decsription:
	     Gets shared root folders.
	  Returns:
		 Retruns shared root folders.
*/

CREATE PROCEDURE [dbo].[lspr_GetSharedFolderRoots]	
AS
BEGIN
	select * from lsSharedFoldersRoots
END


GO


CREATE PROCEDURE [dbo].[lspr_GetSubscribedFolders] 
	@UserName as nvarchar(100)
AS

select * from lsIMAPSubscribedFolders where UserID=(select UserID from lsUsers where UserName = @UserName)


GO


CREATE PROCEDURE [dbo].[lspr_GetUserAddresses]
	@UserName nvarchar(100) = NULL
AS

if(@UserName is not null)
begin
	declare @UserID nvarchar(100)
	select @UserID = (select UserID from lsUsers where UserName=@UserName)

	select * from lsUserAddresses where (UserID=@UserID)
end
else
begin
       select * from lsUserAddresses
end


GO


CREATE PROCEDURE [dbo].[lspr_GetUserMessageRuleActions]
	@userID nvarchar(100) = NULL,
	@ruleID nvarchar(100) = NULL
AS
BEGIN
	select * from lsUserMessageRuleActions where (UserID = @userID AND RuleID = @ruleID)
END


GO


CREATE PROCEDURE [dbo].[lspr_GetUserMessageRules]
	@UserName nvarchar(100) = NULL
AS

if(@UserName is not null)
begin
	declare @UserID nvarchar(100)
	select @UserID = (select UserID from lsUsers where UserName=@UserName)

	select * from lsUserMessageRules where (UserID=@UserID)
end
else
begin
	select * from lsUserMessageRules
end


GO


CREATE PROCEDURE [dbo].[lspr_GetUserProperties]
	@UserName nvarchar(100)	= NULL
AS

select * from lsUsers where (UserName = @UserName)


GO


CREATE PROCEDURE [dbo].[lspr_GetUserRemoteServers]
	@UserName nvarchar(100) = NULL
AS

if(@UserName is not null)
begin
	declare @UserID nvarchar(100)
	select @UserID = (select UserID from lsUsers where UserName=@UserName)

	select * from lsUserRemoteServers where (UserID=@UserID)
end
else
begin
	select * from lsUserRemoteServers
end


GO


CREATE PROCEDURE [dbo].[lspr_GetUsers]
	@DomainName nvarchar(100) = NULL
AS

if(@DomainName is not null)
begin
      select * from lsUsers where (DomainName=@DomainName)
end
else
begin
       select * from lsUsers
end


GO


/*  Implementation notes:
      Decsription:
	     Gets users default folders.
	  Returns:
		 Retruns users default folders.
*/

CREATE PROCEDURE [dbo].[lspr_GetUsersDefaultFolders]	
AS
BEGIN
	select * from lsUsersDefaultFolders
END


GO


/*  Implementation notes:
      Decsription:
	     Checks if specified user group exists.
	  Returns:
		 Retruns specified group if it exists.
*/

CREATE PROCEDURE [dbo].[lspr_GroupExists]
	@groupName nvarchar(100) = NULL
AS
BEGIN
	select * from lsGroups where (GroupName = @groupName)
END


GO


/*	Implementation notes:
      Decsription:
	     Checks if specified user group member exists.
	  Returns:
		 Retruns specified group member if it exists.
*/

CREATE PROCEDURE [dbo].[lspr_GroupMemberExists]
	@groupName   nvarchar(100) = NULL,
	@userOrGroup nvarchar(100) = NULL
AS
BEGIN
	-- Get groupID
	declare @groupID as nvarchar(100)
	select @groupID = (select GroupID from Groups where (GroupName = @groupName))

	select * from lsGroupMembers where (GroupID = @groupID AND UserOrGroup = @userOrGroup)
END


GO


CREATE PROCEDURE [dbo].[lspr_MapUser]
	@EmailAddress nvarchar(100) = NULL
AS

declare @UserID nvarchar(100)
select @UserID = (select UserID from lsUserAddresses where Address=@EmailAddress)

select UserName from lsUsers where (UserID=@UserID)


GO


CREATE PROCEDURE [dbo].[lspr_RenameFolder] 
	@UserName  nvarchar(100),
	@Folder    nvarchar(100),
	@NewFolder nvarchar(100)
AS

declare @UserID as uniqueidentifier
select @UserID = (select UserID from lsUsers where UserName = @UserName)

-- Check if destination folder exists
if exists(select * from  lsIMAPFolders where UserID = @UserID AND FolderName = @NewFolder)
begin
	select 'Destination Folder(' + @Folder  + ') already exists' as ErrorText
	return
end

if exists(select * from  lsIMAPFolders where UserID = @UserID AND FolderName = @Folder)
begin
	-- Rename mail store folder and it's subfolders
	update lsMailStore  set 
		Folder = (@NewFolder + substring(Folder,len(@Folder) + 1,len(Folder) - len(@Folder)))
	where (Mailbox = @UserName AND Folder LIKE (@Folder + '%'))

	-- Rename folder and it's subfolders
	update lsIMAPFolders  set 
		FolderName = (@NewFolder + substring(FolderName,len(@Folder) + 1,len(FolderName) - len(@Folder)))
	where (UserID = @UserID AND FolderName LIKE (@Folder + '%'))

	-- Rename folder and it's subfolders ACL
	update lsIMAP_ACL  set 
		Folder = (@UserName + '/' + @NewFolder + substring(Folder,len(@UserName + '/' + @Folder) + 1,len(Folder) - len(@UserName + '/' + @NewFolder) + 1)) 
	where (Folder LIKE (@UserName + '/' + @Folder + '%'))
end
else
begin
	select 'Source Folder(' + @Folder  + ') doesn''t exists' as ErrorText	
end


GO


CREATE PROCEDURE [dbo].[lspr_SetFolderACL]
	@FolderName  nvarchar(500) = NULL,
	@UserName    nvarchar(500) = NULL,
	@Permissions nvarchar(20)  = ''
AS

if(exists(select * from lsIMAP_ACL where (Folder = @FolderName AND [User] = @UserName)))
begin
	update lsIMAP_ACL set 
		[Permissions] = @Permissions
	where  (Folder = @FolderName AND [User] = @UserName)
end
else
begin
	insert lsIMAP_ACL (Folder,[User],[Permissions]) 
	values (@FolderName,@UserName,@Permissions)
end


GO


CREATE PROCEDURE [dbo].[lspr_StoreMessage]
	@Mailbox       nvarchar(100) = NULL,
	@Folder        nvarchar(100) = NULL,
	@Data          image         = NULL,
	@Size          bigint        = 0,
	@TopLines      image         = NULL,
	@Date          DateTime	     = NULL,
	@MessageFlags int            = 0
AS

if(not exists (select * from lsIMAPFolders where UserID=(select UserID from lsUsers where UserName = @Mailbox)))
begin 
	if(lower(@Folder) = 'inbox')
	begin
		declare @UserID as nvarchar(100)
		select @UserID = (select UserID from lsUsers where UserName = @Mailbox);

		insert into lsIMAPFolders (UserID,FolderName) values (@UserID,'Inbox')
	end
	else
	begin
		select ('Folder ' + @Folder + ' doesn''t exist') as ErrorText
		return;
	end
end

insert lsMailStore (MessageID,Mailbox,Folder,Data,Size,TopLines,Date,MessageFlags) values (newid(),@Mailbox,@Folder,@Data,@Size,@TopLines,@Date,@MessageFlags)


GO


CREATE PROCEDURE [dbo].[lspr_StoreMessageFlags]
	@MessageID    uniqueidentifier = NULL,
	@Mailbox      nvarchar(100)    = NULL,
	@Folder       nvarchar(100)    = NULL,
	@MessageFalgs int              = NULL
AS

Update lsMailStore set MessageFlags = @MessageFalgs where MessageID = @MessageID AND Mailbox = @Mailbox AND Folder = @Folder


GO


/* Stores specified message to recycel bin.
    @messageID - Recycle bin message ID.
    @user      - User whos messge it is.
    @folder    - Original folder that contained message.
    @size      - Message size in bytes.
    @envelope  - Message IMAP Envelop string.
    @data      - Message data.
*/
CREATE PROCEDURE [dbo].[lspr_StoreRecycleBinMessage]
    @messageID nvarchar(100)  = NULL,
    @user      nvarchar(200)  = NULL,
    @folder    nvarchar(500)  = NULL,
	@size      bigint         = 0,
    @envelope  nvarchar(2000) = NULL,
    @data      image          = NULL
AS
BEGIN
    insert into lsRecycleBin (MessageID,DeleteTime,[User],Folder,[Size],Envelope,Data)
        values(@messageID,getdate(),@user,@folder,@size,@envelope,@data)
END


GO


CREATE PROCEDURE [dbo].[lspr_SubscribeFolder] 
	@UserName nvarchar(100),
	@Folder   nvarchar(100)
AS

-- ToDo: check if exist, delete or just skip ???

declare @UserID as uniqueidentifier
select @UserID = (select UserID from lsUsers where UserName = @UserName)

insert into lsIMAPSubscribedFolders (UserID,FolderName) values (@UserID,@Folder)


GO


CREATE PROCEDURE [dbo].[lspr_UnSubscribeFolder] 
	@UserName nvarchar(100),
	@Folder   nvarchar(100)
AS

declare @UserID as uniqueidentifier
select @UserID = (select UserID from lsUsers where UserName = @UserName)

delete  lsIMAPSubscribedFolders where UserID =  @UserID AND FolderName = @Folder


GO


CREATE PROCEDURE [dbo].[lspr_UpdateDomain]
    @DomainID    nvarchar(100) = NULL,
    @DomainName  nvarchar(100) = NULL,
    @Description nvarchar(100) = NULL
AS

set nocount on

-- Ensure that domain with specified ID exists
IF(not exists(select * from lsDomains where (DomainID = @DomainID)))
BEGIN
    select 'Specified @DomainID "' + @DomainID + '" doesn''t exists !' as ErrorText
    return;
END

-- Ensure that another domain haven't same domain name
IF(exists(select * from lsDomains where (DomainID != @DomainID AND DomainName = @DomainName)))
BEGIN
    select 'Domain with specified name "' + @DomainName + '" already exists !' as ErrorText
    return;
END

-- If domain name changed, rename user addresses and mailing lists
declare @oldDomainName varchar(200)
select @oldDomainName = (select DomainName from lsDomains where (DomainID = @DomainID))

IF(lower(@oldDomainName) != lower(@DomainName))
BEGIN
    -- Rename user addresses
    update lsUserAddresses set
        Address = substring(Address,0,len(Address) - len(@oldDomainName) + 1) + @DomainName
    where(Address LIKE ('%@' + @oldDomainName))

    -- Rename mailing lists
    update lsMailingLists set
        MailingListName = substring(MailingListName,0,len(MailingListName) - len(@oldDomainName) + 1) + @DomainName
    where(MailingListName LIKE ('%@' + @oldDomainName)) 
END

update lsDomains set 
    DomainName  = @DomainName,
    Description = @Description
where (DomainiD = @DomainID)

select null as ErrorText


GO


CREATE PROCEDURE [dbo].[lspr_UpdateFilter]
	@FilterID    nvarchar(100) = NULL,
	@Description nvarchar(100) = NULL,
	@Type        nvarchar(100) = NULL,
	@Assembly    nvarchar(100) = NULL,
	@ClassName   nvarchar(100) = NULL,
	@Cost        bigint        = 0,
	@Enabled     bit           = true
AS

if(exists(select * from lsFilters where (FilterID=@FilterID)))
begin
	update lsFilters set 
		Description = @Description,
		Type        = @Type,
		Assembly    = @Assembly,
		ClassName   = @ClassName,
		Cost        = @Cost,
		Enabled     = @Enabled
	where  (FilterID=@FilterID)

	select null as ErrorText
end
else
begin
	select 'Filter with specified ID "' + @FilterID + '" doesn''t exist !' as ErrorText
end


GO


CREATE PROCEDURE [dbo].[lspr_UpdateGlobalMessageRule]
	@ruleID          nvarchar(100) = NULL,
	@cost            bigint        = NULL,
	@enabled         bit           = NULL,
	@checkNextRule   int           = NULL,
	@description     nvarchar(400) = NULL,
	@matchExpression image         = NULL
AS
BEGIN
	if(exists(select * from lsGlobalMessageRules where (RuleID = @ruleID)))
    begin
		update lsGlobalMessageRules set
			RuleID          = @ruleID,
			Cost            = @cost,
			Enabled         = @enabled,
			CheckNextRuleIf = @checkNextRule,
			Description     = @description,
			MatchExpression = @matchExpression
		where  (RuleID = @ruleID)

		select null as ErrorText
    end
    else
	begin
		select 'Rule with specified ID "' + @ruleID + '" doesn''t exist !' as ErrorText
	end
END


GO


CREATE PROCEDURE [dbo].[lspr_UpdateGlobalMessageRuleAction]
	@ruleID          nvarchar(100) = NULL,
	@actionID        nvarchar(100) = NULL,
	@description     nvarchar(400) = NULL,
	@actionType      int           = NULL,
	@actionData      image         = NULL
AS
BEGIN
	if(exists(select * from lsGlobalMessageRuleActions where (RuleID = @ruleID AND ActionID = @ActionID)))
    begin
		update lsGlobalMessageRuleActions set
			RuleID      = @ruleID,
			ActionID    = @actionID,
			Description = @description,
			ActionType  = @actionType,
			ActionData  = @actionData
		where  (RuleID = @ruleID AND ActionID = @ActionID)

		select null as ErrorText
    end
    else
	begin
		select 'Action with specified ID "' + @actionID + '" doesn''t exist !' as ErrorText
	end
END


GO


/*  Implementation notes:
      Decsription:
	    Updates user group.
	  Returns:
		If successful returns nothing, otherwise returns 1 row with error text in column 'ErrorText'.

	  Implementation:
		*) Ensure that group with specified ID does exist. Return error text.
        *) If group name is changed, ensure that new group name won't conflict 
           any other group or user name. Return error text.                    
        *) Udpate group.
		 
*/

CREATE PROCEDURE [dbo].[lspr_UpdateGroup]
	@groupID     nvarchar(100) = NULL,
	@groupName   nvarchar(100) = NULL,
	@description nvarchar(400) = NULL,
	@enabled     bit           = NULL
AS
BEGIN
	-- Ensure that group with specified ID does exist.
	if(not exists(select * from lsGroups where (GroupID = @groupID)))
	begin
		select 'Invalid group ID, specified group ID ''' + @groupID + ''' already exists !' as ErrorText
		return;
	end

	-- If group name is changed, ensure that new group name won't conflict 
	-- any other group or user name. Throw Exception if does.
	declare @currentGroupName as nvarchar(100)
	select @currentGroupName = (select GroupName from lsGroups where (GroupID = @groupID))
	if(@currentGroupName != @groupName) 
	begin		
		-- Ensure that group name won't exist already.
		if(exists(select * from lsGroups where (GroupName = @groupName)))
		begin
			select 'Invalid group name, specified group ''' + @groupName + ''' already exists !' as ErrorText
			return;
		end
		-- Ensure that user name with groupName doen't exist.
		else if exists(select * from lsUsers where (UserName = @groupName))
		begin
			select 'Invalid group name, user with specified name ''' + @groupName + ''' already exists !' as ErrorText
			return;
		end
	end

	-- Insert group
	update lsGroups set
		GroupID     = @groupID,
		GroupName   = @groupName,
		Description = @description,
		Enabled     = @enabled	
	where (GroupID = @groupID)
		
	select null as ErrorText
END


GO


CREATE PROCEDURE [dbo].[lspr_UpdateMailingList]
	@MailingListID	 varchar(100) = NULL,
	@MailingListName varchar(100) = NULL,
	@Description     varchar(100) = NULL,
	@DomainName      varchar(100) = NULL,
	@enabled         bit          = false
AS

if(exists(select * from lsMailingLists where (MailingListID=@MailingListID)))
begin
	-- If changeing mailing list name, ensure that anyone already haven't got it
	if(exists(select * from lsMailingLists where (MailingListName=@MailingListName)))
	begin
		declare @MailingListOwnerID as nvarchar(100)

		select @MailingListOwnerID = (select MailingListID from lsMailingLists where MailingListName=@MailingListName)
		if(@MailingListOwnerID != @MailingListID)
		begin
			select 'Mailing list with name "' + @MailingListName + '" already exists !' as ErrorText
			return
		end
	end

	update lsMailingLists set 
		MailingListName = @MailingListName,
		Description     = @Description,
		DomainName      = @DomainName,
		Enabled         = @enabled
	where  (MailingListID=@MailingListID)

	select null as ErrorText
end
else
begin
	select 'Mailing list with specified ID "' + @MailingListID + '" doesn''t exist !' as ErrorText
end


GO


/* Updates recycle bin settings.
    @deleteToRecycleBin  - Specifies if messages are deleted to recycle bin. 
    @deleteMessagesAfter - Specifies after what days messages will be deleted.
*/
CREATE PROCEDURE [dbo].[lspr_UpdateRecycleBinSettings]
	@deleteToRecycleBin  bit = 0,
	@deleteMessagesAfter int = 1
AS
BEGIN
    IF(exists(select * from lsRecycleBinSettings))
    BEGIN
	    update lsRecycleBinSettings set
            DeleteToRecycleBin  = @deleteToRecycleBin,
            DeleteMessagesAfter = @deleteMessagesAfter
    END
    ELSE
    BEGIN
        insert into lsRecycleBinSettings (DeleteToRecycleBin,DeleteMessagesAfter)
            values (@deleteToRecycleBin,@deleteMessagesAfter)
    END
END


GO


CREATE PROCEDURE [dbo].[lspr_UpdateRoute]
	@routeID     varchar(100) = NULL,
	@cost        bigint       = NULL,
	@enabled     bit          = NULL,
	@description varchar(100) = NULL,
	@pattern     varchar(100) = NULL,
	@action      int          = NULL,
	@actionData  image        = NULL
AS

if(exists(select * from lsRouting where (RouteID=@routeID)))
begin
	-- If changeing route pattern, ensure that it  doesn't exist already
	if(exists(select * from lsRouting where (Pattern=@pattern)))
	begin
		declare @RouteOwnerID as nvarchar(100)

		select @RouteOwnerID = (select RouteID from lsRouting where Pattern=@pattern)
		if(@RouteOwnerID != @RouteID)
		begin
			select 'Route with pattern "' + @pattern + '" already exists !' as ErrorText
			return
		end
	end

	update lsRouting set 
		Cost        = @cost,
		Enabled     = @enabled,
		Description = @Description,
		Pattern     = @pattern,
		Action      = @action,
		ActionData  = @actionData
	where  (RouteID=@RouteID)

	select null as ErrorText
end
else
begin
	select 'Route with specified ID "' + @RouteID + '" doesn''t exist !' as ErrorText
end


GO


CREATE PROCEDURE [dbo].[lspr_UpdateSecurityEntry]
	@id          varchar(100) = NULL,
	@enabled     bit          = 1,
	@description varchar(100) = NULL,
	@service     varchar(100) = NULL,
	@action      varchar(100) = NULL,
	@startIP     varchar(100) = 0,
	@endIP       varchar(100) = 0
AS

if(exists(select * from lsIPSecurity where (ID=@id)))
begin
	update lsIPSecurity set 
		Enabled     = @enabled,
		Description = @description,
		Service     = @service,
		Action      = @action,
		StartIP     = @startIP,
		EndIP       = @endIP
	where (ID=@id)

	select null as ErrorText
end
else
begin
	select 'Security entry with specified ID "' + @id + '" doesn''t exist !' as ErrorText
end


GO


CREATE PROCEDURE [dbo].[lspr_UpdateSettings]
	@Settings image = NULL
AS

if(exists(select * from lsSettings))
begin
	update lsSettings set 
		Settings = @Settings
end
else
begin
	insert into lsSettings (Settings) values (@Settings)
end


GO


/*  Implementation notes:
      Decsription:
	    Updates shared folder root.
	  Returns:
		If successful returns nothing, otherwise returns 1 row with error text in column 'ErrorText'.

	  Implementation:
		*) Ensure that root with specified ID does exist. Return error text.
        *) If root name is changed, ensure that new root name won't conflict 
           any other root name. Return error text.                    
        *) Udpate root folder.
		 
*/

CREATE PROCEDURE [dbo].[lspr_UpdateSharedFolderRoot]
	@rootID        nvarchar(100) = NULL,
	@enabled       bit           = NULL,
	@folder        nvarchar(400) = NULL,
	@description   nvarchar(400) = NULL,
	@rootType      int           = NULL,
	@boundedUser   nvarchar(100) = NULL,
	@boundedFolder nvarchar(400) = NULL
AS
BEGIN
	-- Ensure that root with specified ID does exist.
	if(not exists(select * from lsSharedFoldersRoots where (RootID = @rootID)))
	begin
		select 'Invalid root ID, specified root ID ''' + @rootID + ''' already exists !' as ErrorText
		return;
	end

	-- If root name is changed, ensure that new root name won't conflict 
    -- any other root name. Throw Exception if does.
	declare @currentRootName as nvarchar(100)
	select @currentRootName = (select Folder from lsSharedFoldersRoots where (RootID = @rootID))
	if(@currentRootName != @folder) 
	begin		
		-- Ensure that root name won't exist already.
		if(exists(select * from lsSharedFoldersRoots where (Folder = @folder)))
		begin
			select 'Invalid root name, specified root ''' + @folder + ''' already exists !' as ErrorText
			return;
		end
	end

	-- Insert group
	update lsSharedFoldersRoots set
		Enabled       = @enabled,
		Folder        = @folder,
		Description   = @description,
		RootType      = @rootType,
		BoundedUser   = @boundedUser,
		BoundedFolder = @boundedFolder
	where (RootID = @rootID)
		
	select null as ErrorText
END


GO


CREATE PROCEDURE [dbo].[lspr_UpdateUser]
	@UserID	     varchar(100) = NULL,
	@FullName    varchar(100) = NULL,
	@UserName    varchar(100) = NULL,
	@Password    varchar(100) = NULL,
	@Description varchar(100) = NULL,
	@DomainName  varchar(100) = NULL,
	@MailboxSize bigint	      = 0,
	@Enabled     bit          = true,
	@permissions int          = 255
AS

if(exists(select * from lsUsers where (UserID=@UserID)))
begin
	-- If changeing username, ensure that anyone already haven't got it
	if(exists(select * from lsUsers where (UserName = @UserName)))
	begin
		declare @UserNameOwnerID as nvarchar(100)

		select @UserNameOwnerID = (select UserID from lsUsers where UserName=@UserName)
		if(@UserNameOwnerID != @UserID)
		begin
			select 'User with user name "' + @UserName + '" already exists !' as ErrorText
			return
		end
	end

	update lsUsers set 
		FullName      = @FullName,
		UserName      = @UserName,
		Password      = @Password,
		Description   = @Description,
		Mailbox_Size  = @MailboxSize,
		DomainName    = @DomainName,
		Enabled       = @Enabled,
		[Permissions] = @permissions
	where  (UserID=@UserID)

	select null as ErrorText
end
else
begin
	select 'User with specified ID "' + @UserID + '" doesn''t exist !' as ErrorText
end


GO


/* Updates user last login time.
    @userName - User name whos last login time to update.
*/
CREATE PROCEDURE [dbo].[lspr_UpdateUserLastLoginTime]
    @userName nvarchar(100) = NULL
AS
BEGIN
    update lsUsers set
        LastLoginTime = getdate()
    where (UserName = @userName)
END


GO


CREATE PROCEDURE [dbo].[lspr_UpdateUserMessageRule]
	@userID          nvarchar(100) = NULL,
	@ruleID          nvarchar(100) = NULL,
	@cost            bigint        = NULL,
	@enabled         bit           = NULL,
	@checkNextRule   int           = NULL,
	@description     nvarchar(400) = NULL,
	@matchExpression image         = NULL
AS
BEGIN
	if(exists(select * from lsUserMessageRules where (UserID = @userID AND RuleID = @ruleID)))
    begin
		update lsUserMessageRules set
			UserID          = @userID,
			RuleID          = @ruleID,
			Cost            = @cost,
			Enabled         = @enabled,
			CheckNextRuleIf = @checkNextRule,
			Description     = @description,
			MatchExpression = @matchExpression
		where  (UserID = @userID AND RuleID = @ruleID)

		select null as ErrorText
    end
    else
	begin
		select 'Rule with specified ID "' + @ruleID + '" doesn''t exist !' as ErrorText
	end
END


GO


CREATE PROCEDURE [dbo].[lspr_UpdateUserMessageRuleAction]
	@userID          nvarchar(100) = NULL,
	@ruleID          nvarchar(100) = NULL,
	@actionID        nvarchar(100) = NULL,
	@description     nvarchar(400) = NULL,
	@actionType      int           = NULL,
	@actionData      image         = NULL
AS
BEGIN
	if(exists(select * from lsUserMessageRuleActions where (UserID = @userID AND RuleID = @ruleID AND ActionID = @ActionID)))
    begin
		update lsUserMessageRuleActions set
			UserID      = @userID,
			RuleID      = @ruleID,
			ActionID    = @actionID,
			Description = @description,
			ActionType  = @actionType,
			ActionData  = @actionData
		where (UserID = @userID AND RuleID = @ruleID AND ActionID = @ActionID)

		select null as ErrorText
    end
    else
	begin
		select 'Action with specified ID "' + @actionID + '" doesn''t exist !' as ErrorText
	end
END


GO


CREATE PROCEDURE [dbo].[lspr_UpdateUserRemoteServer] 
	@ServerID       nvarchar(100) = NULL,
	@UserName       nvarchar(100) = NULL,
	@Description    nvarchar(100) = NULL,
	@RemoteServer   nvarchar(100) = NULL,
	@RemotePort     int           = NULL,
	@RemoteUserName nvarchar(100) = NULL,
	@RemotePassword nvarchar(100) = NULL,
	@UseSSL         bit           = NULL,
	@Enabled        bit           = NULL
AS

set nocount on

if(exists(select * from lsUserRemoteServers where (ServerID = @ServerID)))
begin
	-- Get userID
	declare @UserID nvarchar(100)
	select @UserID = (select UserID from lsUsers where UserName=@UserName)

	update lsUserRemoteServers set
		ServerID       = @ServerID,
		UserID         = @UserID,
		Description    = @Description,
		RemoteServer   = @RemoteServer,
		RemotePort     = @RemotePort,
		RemoteUserName = @RemoteUserName,
		RemotePassword = @RemotePassword,
		UseSSL         = @UseSSL,
		Enabled        = @Enabled  
	where (ServerID = @ServerID)

	select null as ErrorText
end
else
begin
	select 'User remote server with specified ID "' + @ServerID + '" doesn''t exist !' as ErrorText
end


GO


CREATE PROCEDURE [dbo].[lspr_ValidateMailboxSize]
	@UserName nvarchar(100) = NULL
AS
BEGIN
    set nocount on

    declare @Size bigint , @AllowedSize bigint
    select  @Size = 0
    select  @AllowedSize = -1

    -- Get mailbox size
    if(exists(select Mailbox_Size from lsUsers where UserName=@UserName))
    begin
        select @AllowedSize = (select Mailbox_Size from lsUsers where UserName=@UserName)
    end


    -- Count mailbox size
    if(exists(select MailBox from lsMailStore where Mailbox=@UserName))
    begin
        select   @Size = (select sum(Size) from lsMailStore where Mailbox=@UserName)
    end


    if(@Size < @AllowedSize*1000000)  -- Allowed size in mb, size is bytes
        select cast(1 as Bit) as Validated
    else
	begin
      select cast(0 as Bit) as Validated
    end
END
