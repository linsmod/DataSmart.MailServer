using System;

namespace DataSmart.MailServer
{
	public enum GlobalMessageRuleActionType
	{
		AutoResponse = 1,
		DeleteMessage,
		ForwardToEmail,
		ForwardToHost,
		StoreToDiskFolder,
		ExecuteProgram,
		StoreToIMAPFolder,
		AddHeaderField,
		RemoveHeaderField,
		SendErrorToClient,
		StoreToFTPFolder,
		PostToNNTPNewsGroup,
		PostToHTTP
	}
}
