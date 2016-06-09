using System.NetworkToolkit;
using System.NetworkToolkit.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class GlobalMessageRuleActionCollection : IEnumerable
	{
		private GlobalMessageRule m_pRule;

		private List<GlobalMessageRuleActionBase> m_pActions;

		public GlobalMessageRule Rule
		{
			get
			{
				return this.m_pRule;
			}
		}

		public int Count
		{
			get
			{
				return this.m_pActions.Count;
			}
		}

		public GlobalMessageRuleActionBase this[int index]
		{
			get
			{
				return this.m_pActions[index];
			}
		}

		public GlobalMessageRuleActionBase this[string globalMessageRuleActionID]
		{
			get
			{
				foreach (GlobalMessageRuleActionBase current in this.m_pActions)
				{
					if (current.ID.ToLower() == globalMessageRuleActionID.ToLower())
					{
						return current;
					}
				}
				throw new Exception("GlobalMessageRule action with specified ID '" + globalMessageRuleActionID + "' doesn't exist !");
			}
		}

		internal GlobalMessageRuleActionCollection(GlobalMessageRule rule)
		{
			this.m_pRule = rule;
			this.m_pActions = new List<GlobalMessageRuleActionBase>();
			this.Bind();
		}

		public AddHeaderField Add_AddHeaderField(string description, string headerFieldName, string headerFieldValue)
		{
			AddHeaderField addHeaderField = new AddHeaderField(this.m_pRule, this, Guid.NewGuid().ToString(), description, headerFieldName, headerFieldValue);
			this.Add(addHeaderField);
			this.m_pActions.Add(addHeaderField);
			return addHeaderField;
		}

		public AutoResponse Add_AutoResponse(string description, string from, byte[] message)
		{
			AutoResponse autoResponse = new AutoResponse(this.m_pRule, this, Guid.NewGuid().ToString(), description, from, message);
			this.Add(autoResponse);
			this.m_pActions.Add(autoResponse);
			return autoResponse;
		}

		public DeleteMessage Add_DeleteMessage(string description)
		{
			DeleteMessage deleteMessage = new DeleteMessage(this.m_pRule, this, Guid.NewGuid().ToString(), description);
			this.Add(deleteMessage);
			this.m_pActions.Add(deleteMessage);
			return deleteMessage;
		}

		public ExecuteProgram Add_ExecuteProgram(string description, string program, string programArguments)
		{
			ExecuteProgram executeProgram = new ExecuteProgram(this.m_pRule, this, Guid.NewGuid().ToString(), description, program, programArguments);
			this.Add(executeProgram);
			this.m_pActions.Add(executeProgram);
			return executeProgram;
		}

		public ForwardToEmail Add_ForwardToEmail(string description, string email)
		{
			ForwardToEmail forwardToEmail = new ForwardToEmail(this.m_pRule, this, Guid.NewGuid().ToString(), description, email);
			this.Add(forwardToEmail);
			this.m_pActions.Add(forwardToEmail);
			return forwardToEmail;
		}

		public ForwardToHost Add_ForwardToHost(string description, string host, int port)
		{
			ForwardToHost forwardToHost = new ForwardToHost(this.m_pRule, this, Guid.NewGuid().ToString(), description, host, port);
			this.Add(forwardToHost);
			this.m_pActions.Add(forwardToHost);
			return forwardToHost;
		}

		public MoveToImapFolder Add_MoveToImapFolder(string description, string folder)
		{
			MoveToImapFolder moveToImapFolder = new MoveToImapFolder(this.m_pRule, this, Guid.NewGuid().ToString(), description, folder);
			this.Add(moveToImapFolder);
			this.m_pActions.Add(moveToImapFolder);
			return moveToImapFolder;
		}

		public GlobalMessageRuleAction_PostToHttp Add_PostToHttp(string description, string url)
		{
			GlobalMessageRuleAction_PostToHttp globalMessageRuleAction_PostToHttp = new GlobalMessageRuleAction_PostToHttp(this.m_pRule, this, Guid.NewGuid().ToString(), description, url);
			this.Add(globalMessageRuleAction_PostToHttp);
			this.m_pActions.Add(globalMessageRuleAction_PostToHttp);
			return globalMessageRuleAction_PostToHttp;
		}

		public PostToNntpNewsgroup Add_PostToNntp(string description, string host, int port, string newsgroup)
		{
			PostToNntpNewsgroup postToNntpNewsgroup = new PostToNntpNewsgroup(this.m_pRule, this, Guid.NewGuid().ToString(), description, host, port, newsgroup);
			this.Add(postToNntpNewsgroup);
			this.m_pActions.Add(postToNntpNewsgroup);
			return postToNntpNewsgroup;
		}

		public RemoveHeaderField Add_RemoveHeaderField(string description, string headerField)
		{
			RemoveHeaderField removeHeaderField = new RemoveHeaderField(this.m_pRule, this, Guid.NewGuid().ToString(), description, headerField);
			this.Add(removeHeaderField);
			this.m_pActions.Add(removeHeaderField);
			return removeHeaderField;
		}

		public SendError Add_SendError(string description, string errorText)
		{
			SendError sendError = new SendError(this.m_pRule, this, Guid.NewGuid().ToString(), description, errorText);
			this.Add(sendError);
			this.m_pActions.Add(sendError);
			return sendError;
		}

		public StoreToDiskFolder Add_StoreToDisk(string description, string folder)
		{
			StoreToDiskFolder storeToDiskFolder = new StoreToDiskFolder(this.m_pRule, this, Guid.NewGuid().ToString(), description, folder);
			this.Add(storeToDiskFolder);
			this.m_pActions.Add(storeToDiskFolder);
			return storeToDiskFolder;
		}

		public StoreToFtp Add_StoreToFtp(string description, string host, int port, string userName, string password, string folder)
		{
			StoreToFtp storeToFtp = new StoreToFtp(this.m_pRule, this, Guid.NewGuid().ToString(), description, host, port, userName, password, folder);
			this.Add(storeToFtp);
			this.m_pActions.Add(storeToFtp);
			return storeToFtp;
		}

		private void Add(GlobalMessageRuleActionBase action)
		{
			this.Add(action.ID, action.Description, action.ActionType, action.Serialize(), false);
		}

		internal void Add(string actionID, string description, GlobalMessageRuleActionType type, byte[] actionData, bool addToCollection)
		{
			Guid.NewGuid().ToString();
			SmartStream arg_BD_0 = this.m_pRule.VirtualServer.Server.TCP_Client.TcpStream;
			string[] array = new string[12];
			array[0] = "AddGlobalMessageRuleAction ";
			array[1] = this.m_pRule.VirtualServer.VirtualServerID;
			array[2] = " ";
			array[3] = TextUtils.QuoteString(this.m_pRule.ID);
			array[4] = " ";
			array[5] = TextUtils.QuoteString(actionID);
			array[6] = " ";
			array[7] = TextUtils.QuoteString(description);
			array[8] = " ";
			string[] arg_A2_0 = array;
			int arg_A2_1 = 9;
			int num = (int)type;
			arg_A2_0[arg_A2_1] = num.ToString();
			array[10] = " ";
			array[11] = Convert.ToBase64String(actionData);
			arg_BD_0.WriteLine(string.Concat(array));
			string text = this.m_pRule.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			if (addToCollection)
			{
				this.m_pActions.Add(this.GetAction(actionID, description, type, actionData));
			}
		}

		public void Remove(GlobalMessageRuleActionBase action)
		{
			Guid.NewGuid().ToString();
			this.m_pRule.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
			{
				"DeleteGlobalMessageRuleAction ",
				this.m_pRule.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pRule.ID),
				" ",
				TextUtils.QuoteString(action.ID)
			}));
			string text = this.m_pRule.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pActions.Remove(action);
		}

		private void Bind()
		{
			lock (this.m_pRule.VirtualServer.Server.LockSynchronizer)
			{
				this.m_pRule.VirtualServer.Server.TCP_Client.TcpStream.WriteLine("GetGlobalMessageRuleActions " + this.m_pRule.VirtualServer.VirtualServerID + " " + TextUtils.QuoteString(this.m_pRule.ID));
				string text = this.m_pRule.VirtualServer.Server.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
				int num = Convert.ToInt32(text.Split(new char[]
				{
					' '
				}, 2)[1]);
				MemoryStream memoryStream = new MemoryStream();
				this.m_pRule.VirtualServer.Server.TCP_Client.TcpStream.ReadFixedCount(memoryStream, (long)num);
				DataSet dataSet = Utils.DecompressDataSet(memoryStream);
				if (dataSet.Tables.Contains("GlobalMessageRuleActions"))
				{
					foreach (DataRow dataRow in dataSet.Tables["GlobalMessageRuleActions"].Rows)
					{
						this.m_pActions.Add(this.GetAction(dataRow["ActionID"].ToString(), dataRow["Description"].ToString(), (GlobalMessageRuleActionType)Convert.ToInt32(dataRow["ActionType"]), Convert.FromBase64String(dataRow["ActionData"].ToString())));
					}
				}
			}
		}

		private GlobalMessageRuleActionBase GetAction(string actionID, string description, GlobalMessageRuleActionType actionType, byte[] actionData)
		{
			if (actionType == GlobalMessageRuleActionType.AddHeaderField)
			{
				return new AddHeaderField(this.m_pRule, this, actionID, description, actionData);
			}
			if (actionType == GlobalMessageRuleActionType.AutoResponse)
			{
				return new AutoResponse(this.m_pRule, this, actionID, description, actionData);
			}
			if (actionType == GlobalMessageRuleActionType.DeleteMessage)
			{
				return new DeleteMessage(this.m_pRule, this, actionID, description);
			}
			if (actionType == GlobalMessageRuleActionType.ExecuteProgram)
			{
				return new ExecuteProgram(this.m_pRule, this, actionID, description, actionData);
			}
			if (actionType == GlobalMessageRuleActionType.ForwardToEmail)
			{
				return new ForwardToEmail(this.m_pRule, this, actionID, description, actionData);
			}
			if (actionType == GlobalMessageRuleActionType.ForwardToHost)
			{
				return new ForwardToHost(this.m_pRule, this, actionID, description, actionData);
			}
			if (actionType == GlobalMessageRuleActionType.MoveToIMAPFolder)
			{
				return new MoveToImapFolder(this.m_pRule, this, actionID, description, actionData);
			}
			if (actionType == GlobalMessageRuleActionType.PostToHTTP)
			{
				return new GlobalMessageRuleAction_PostToHttp(this.m_pRule, this, actionID, description, actionData);
			}
			if (actionType == GlobalMessageRuleActionType.PostToNNTPNewsGroup)
			{
				return new PostToNntpNewsgroup(this.m_pRule, this, actionID, description, actionData);
			}
			if (actionType == GlobalMessageRuleActionType.RemoveHeaderField)
			{
				return new RemoveHeaderField(this.m_pRule, this, actionID, description, actionData);
			}
			if (actionType == GlobalMessageRuleActionType.SendErrorToClient)
			{
				return new SendError(this.m_pRule, this, actionID, description, actionData);
			}
			if (actionType == GlobalMessageRuleActionType.StoreToDiskFolder)
			{
				return new StoreToDiskFolder(this.m_pRule, this, actionID, description, actionData);
			}
			if (actionType == GlobalMessageRuleActionType.StoreToFTPFolder)
			{
				return new StoreToFtp(this.m_pRule, this, actionID, description, actionData);
			}
			throw new Exception("Invalid action type !");
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pActions.GetEnumerator();
		}
	}
}
