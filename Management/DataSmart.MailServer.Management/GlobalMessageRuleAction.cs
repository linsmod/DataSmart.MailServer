using System;

namespace DataSmart.MailServer.Management
{
	public enum GlobalMessageRuleActionType
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
		SendErrorToClient,
		StoreToFTPFolder,
		PostToNNTPNewsGroup,
		PostToHTTP
	}
}
