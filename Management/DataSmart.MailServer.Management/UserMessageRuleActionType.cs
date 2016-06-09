using System;

namespace DataSmart.MailServer.Management
{
	public enum UserMessageRuleActionType
	{
		AutoResponse = 1,
		DeleteMessage,
		ForwardToEmail,
		ForwardToHost,
		StoreToDiskFolder,
		ExecuteProgram,
		MoveToIMAPFolder,
		AddHeaderField,
		RemoveHeaderField,
		StoreToFTPFolder = 11,
		PostToNNTPNewsGroup,
		PostToHTTP
	}
}
