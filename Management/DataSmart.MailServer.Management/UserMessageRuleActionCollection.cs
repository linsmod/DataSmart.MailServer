using System.NetworkToolkit;
using System.NetworkToolkit.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DataSmart.MailServer.Management
{
	public class UserMessageRuleActionCollection
	{
		private UserMessageRule m_pRule;

		private List<UserMessageRuleActionBase> m_pActions;

		public UserMessageRule Rule
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

		public UserMessageRuleActionBase this[int index]
		{
			get
			{
				return this.m_pActions[index];
			}
		}

		public UserMessageRuleActionBase this[string userMessageRuleActionID]
		{
			get
			{
				foreach (UserMessageRuleActionBase current in this.m_pActions)
				{
					if (current.ID.ToLower() == userMessageRuleActionID.ToLower())
					{
						return current;
					}
				}
				throw new Exception("UserMessageRule action with specified ID '" + userMessageRuleActionID + "' doesn't exist !");
			}
		}

		internal UserMessageRuleActionCollection(UserMessageRule rule)
		{
			this.m_pRule = rule;
			this.m_pActions = new List<UserMessageRuleActionBase>();
			this.Bind();
		}

		public UserMessageRuleAction_AddHeaderField Add_AddHeaderField(string description, string headerFieldName, string headerFieldValue)
		{
			UserMessageRuleAction_AddHeaderField userMessageRuleAction_AddHeaderField = new UserMessageRuleAction_AddHeaderField(this.m_pRule, this, Guid.NewGuid().ToString(), description, headerFieldName, headerFieldValue);
			this.Add(userMessageRuleAction_AddHeaderField);
			this.m_pActions.Add(userMessageRuleAction_AddHeaderField);
			return userMessageRuleAction_AddHeaderField;
		}

		public UserMessageRuleAction_AutoResponse Add_AutoResponse(string description, string from, byte[] message)
		{
			UserMessageRuleAction_AutoResponse userMessageRuleAction_AutoResponse = new UserMessageRuleAction_AutoResponse(this.m_pRule, this, Guid.NewGuid().ToString(), description, from, message);
			this.Add(userMessageRuleAction_AutoResponse);
			this.m_pActions.Add(userMessageRuleAction_AutoResponse);
			return userMessageRuleAction_AutoResponse;
		}

		public UserMessageRuleAction_DeleteMessage Add_DeleteMessage(string description)
		{
			UserMessageRuleAction_DeleteMessage userMessageRuleAction_DeleteMessage = new UserMessageRuleAction_DeleteMessage(this.m_pRule, this, Guid.NewGuid().ToString(), description);
			this.Add(userMessageRuleAction_DeleteMessage);
			this.m_pActions.Add(userMessageRuleAction_DeleteMessage);
			return userMessageRuleAction_DeleteMessage;
		}

		public UserMessageRuleAction_ExecuteProgram Add_ExecuteProgram(string description, string program, string programArguments)
		{
			UserMessageRuleAction_ExecuteProgram userMessageRuleAction_ExecuteProgram = new UserMessageRuleAction_ExecuteProgram(this.m_pRule, this, Guid.NewGuid().ToString(), description, program, programArguments);
			this.Add(userMessageRuleAction_ExecuteProgram);
			this.m_pActions.Add(userMessageRuleAction_ExecuteProgram);
			return userMessageRuleAction_ExecuteProgram;
		}

		public UserMessageRuleAction_ForwardToEmail Add_ForwardToEmail(string description, string email)
		{
			UserMessageRuleAction_ForwardToEmail userMessageRuleAction_ForwardToEmail = new UserMessageRuleAction_ForwardToEmail(this.m_pRule, this, Guid.NewGuid().ToString(), description, email);
			this.Add(userMessageRuleAction_ForwardToEmail);
			this.m_pActions.Add(userMessageRuleAction_ForwardToEmail);
			return userMessageRuleAction_ForwardToEmail;
		}

		public UserMessageRuleAction_ForwardToHost Add_ForwardToHost(string description, string host, int port)
		{
			UserMessageRuleAction_ForwardToHost userMessageRuleAction_ForwardToHost = new UserMessageRuleAction_ForwardToHost(this.m_pRule, this, Guid.NewGuid().ToString(), description, host, port);
			this.Add(userMessageRuleAction_ForwardToHost);
			this.m_pActions.Add(userMessageRuleAction_ForwardToHost);
			return userMessageRuleAction_ForwardToHost;
		}

		public UserMessageRuleAction_MoveToImapFolder Add_MoveToImapFolder(string description, string folder)
		{
			UserMessageRuleAction_MoveToImapFolder userMessageRuleAction_MoveToImapFolder = new UserMessageRuleAction_MoveToImapFolder(this.m_pRule, this, Guid.NewGuid().ToString(), description, folder);
			this.Add(userMessageRuleAction_MoveToImapFolder);
			this.m_pActions.Add(userMessageRuleAction_MoveToImapFolder);
			return userMessageRuleAction_MoveToImapFolder;
		}

		public UserMessageRuleAction_PostToHttp Add_PostToHttp(string description, string url)
		{
			UserMessageRuleAction_PostToHttp userMessageRuleAction_PostToHttp = new UserMessageRuleAction_PostToHttp(this.m_pRule, this, Guid.NewGuid().ToString(), description, url);
			this.Add(userMessageRuleAction_PostToHttp);
			this.m_pActions.Add(userMessageRuleAction_PostToHttp);
			return userMessageRuleAction_PostToHttp;
		}

		public UserMessageRuleAction_PostToNntpNewsgroup Add_PostToNntp(string description, string host, int port, string newsgroup)
		{
			UserMessageRuleAction_PostToNntpNewsgroup userMessageRuleAction_PostToNntpNewsgroup = new UserMessageRuleAction_PostToNntpNewsgroup(this.m_pRule, this, Guid.NewGuid().ToString(), description, host, port, newsgroup);
			this.Add(userMessageRuleAction_PostToNntpNewsgroup);
			this.m_pActions.Add(userMessageRuleAction_PostToNntpNewsgroup);
			return userMessageRuleAction_PostToNntpNewsgroup;
		}

		public UserMessageRuleAction_RemoveHeaderField Add_RemoveHeaderField(string description, string headerField)
		{
			UserMessageRuleAction_RemoveHeaderField userMessageRuleAction_RemoveHeaderField = new UserMessageRuleAction_RemoveHeaderField(this.m_pRule, this, Guid.NewGuid().ToString(), description, headerField);
			this.Add(userMessageRuleAction_RemoveHeaderField);
			this.m_pActions.Add(userMessageRuleAction_RemoveHeaderField);
			return userMessageRuleAction_RemoveHeaderField;
		}

		public UserMessageRuleAction_StoreToDiskFolder Add_StoreToDisk(string description, string folder)
		{
			UserMessageRuleAction_StoreToDiskFolder userMessageRuleAction_StoreToDiskFolder = new UserMessageRuleAction_StoreToDiskFolder(this.m_pRule, this, Guid.NewGuid().ToString(), description, folder);
			this.Add(userMessageRuleAction_StoreToDiskFolder);
			this.m_pActions.Add(userMessageRuleAction_StoreToDiskFolder);
			return userMessageRuleAction_StoreToDiskFolder;
		}

		public UserMessageRuleAction_StoreToFtp Add_StoreToFtp(string description, string host, int port, string userName, string password, string folder)
		{
			UserMessageRuleAction_StoreToFtp userMessageRuleAction_StoreToFtp = new UserMessageRuleAction_StoreToFtp(this.m_pRule, this, Guid.NewGuid().ToString(), description, host, port, userName, password, folder);
			this.Add(userMessageRuleAction_StoreToFtp);
			this.m_pActions.Add(userMessageRuleAction_StoreToFtp);
			return userMessageRuleAction_StoreToFtp;
		}

		private void Add(UserMessageRuleActionBase action)
		{
			this.Add(action.ID, action.Description, action.ActionType, action.Serialize(), false);
		}

		internal void Add(string actionID, string description, UserMessageRuleActionType type, byte[] actionData, bool addToCollection)
		{
			Guid.NewGuid().ToString();
			SmartStream arg_EE_0 = this.m_pRule.Owner.VirtualServer.Server.TCP_Client.TcpStream;
			string[] array = new string[14];
			array[0] = "AddUserMessageRuleAction ";
			array[1] = this.m_pRule.Owner.VirtualServer.VirtualServerID;
			array[2] = " ";
			array[3] = TextUtils.QuoteString(this.m_pRule.Owner.Owner.UserID);
			array[4] = " ";
			array[5] = TextUtils.QuoteString(this.m_pRule.ID);
			array[6] = " ";
			array[7] = TextUtils.QuoteString(actionID);
			array[8] = " ";
			array[9] = TextUtils.QuoteString(description);
			array[10] = " ";
			string[] arg_D3_0 = array;
			int arg_D3_1 = 11;
			int num = (int)type;
			arg_D3_0[arg_D3_1] = num.ToString();
			array[12] = " ";
			array[13] = Convert.ToBase64String(actionData);
			arg_EE_0.WriteLine(string.Concat(array));
			string text = this.m_pRule.Owner.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			if (addToCollection)
			{
				this.m_pActions.Add(this.GetAction(actionID, description, type, actionData));
			}
		}

		public void Remove(UserMessageRuleActionBase action)
		{
			Guid.NewGuid().ToString();
			this.m_pRule.Owner.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
			{
				"DeleteUserMessageRuleAction ",
				this.m_pRule.Owner.VirtualServer.VirtualServerID,
				" ",
				TextUtils.QuoteString(this.m_pRule.Owner.Owner.UserID),
				" ",
				TextUtils.QuoteString(this.m_pRule.ID),
				" ",
				TextUtils.QuoteString(action.ID)
			}));
			string text = this.m_pRule.Owner.VirtualServer.Server.ReadLine();
			if (!text.ToUpper().StartsWith("+OK"))
			{
				throw new Exception(text);
			}
			this.m_pActions.Remove(action);
		}

		private void Bind()
		{
			lock (this.m_pRule.Owner.VirtualServer.Server.LockSynchronizer)
			{
				this.m_pRule.Owner.VirtualServer.Server.TCP_Client.TcpStream.WriteLine(string.Concat(new string[]
				{
					"GetUserMessageRuleActions ",
					this.m_pRule.Owner.VirtualServer.VirtualServerID,
					" ",
					TextUtils.QuoteString(this.m_pRule.Owner.Owner.UserID),
					" ",
					TextUtils.QuoteString(this.m_pRule.ID)
				}));
				string text = this.m_pRule.Owner.VirtualServer.Server.ReadLine();
				if (!text.ToUpper().StartsWith("+OK"))
				{
					throw new Exception(text);
				}
				int num = Convert.ToInt32(text.Split(new char[]
				{
					' '
				}, 2)[1]);
				MemoryStream memoryStream = new MemoryStream();
				this.m_pRule.Owner.VirtualServer.Server.TCP_Client.TcpStream.ReadFixedCount(memoryStream, (long)num);
				DataSet dataSet = Utils.DecompressDataSet(memoryStream);
				if (dataSet.Tables.Contains("UserMessageRuleActions"))
				{
					foreach (DataRow dataRow in dataSet.Tables["UserMessageRuleActions"].Rows)
					{
						this.m_pActions.Add(this.GetAction(dataRow["ActionID"].ToString(), dataRow["Description"].ToString(), (UserMessageRuleActionType)Convert.ToInt32(dataRow["ActionType"]), Convert.FromBase64String(dataRow["ActionData"].ToString())));
					}
				}
			}
		}

		private UserMessageRuleActionBase GetAction(string actionID, string description, UserMessageRuleActionType actionType, byte[] actionData)
		{
			if (actionType == UserMessageRuleActionType.AddHeaderField)
			{
				return new UserMessageRuleAction_AddHeaderField(this.m_pRule, this, actionID, description, actionData);
			}
			if (actionType == UserMessageRuleActionType.AutoResponse)
			{
				return new UserMessageRuleAction_AutoResponse(this.m_pRule, this, actionID, description, actionData);
			}
			if (actionType == UserMessageRuleActionType.DeleteMessage)
			{
				return new UserMessageRuleAction_DeleteMessage(this.m_pRule, this, actionID, description);
			}
			if (actionType == UserMessageRuleActionType.ExecuteProgram)
			{
				return new UserMessageRuleAction_ExecuteProgram(this.m_pRule, this, actionID, description, actionData);
			}
			if (actionType == UserMessageRuleActionType.ForwardToEmail)
			{
				return new UserMessageRuleAction_ForwardToEmail(this.m_pRule, this, actionID, description, actionData);
			}
			if (actionType == UserMessageRuleActionType.ForwardToHost)
			{
				return new UserMessageRuleAction_ForwardToHost(this.m_pRule, this, actionID, description, actionData);
			}
			if (actionType == UserMessageRuleActionType.MoveToIMAPFolder)
			{
				return new UserMessageRuleAction_MoveToImapFolder(this.m_pRule, this, actionID, description, actionData);
			}
			if (actionType == UserMessageRuleActionType.PostToHTTP)
			{
				return new UserMessageRuleAction_PostToHttp(this.m_pRule, this, actionID, description, actionData);
			}
			if (actionType == UserMessageRuleActionType.PostToNNTPNewsGroup)
			{
				return new UserMessageRuleAction_PostToNntpNewsgroup(this.m_pRule, this, actionID, description, actionData);
			}
			if (actionType == UserMessageRuleActionType.RemoveHeaderField)
			{
				return new UserMessageRuleAction_RemoveHeaderField(this.m_pRule, this, actionID, description, actionData);
			}
			if (actionType == UserMessageRuleActionType.StoreToDiskFolder)
			{
				return new UserMessageRuleAction_StoreToDiskFolder(this.m_pRule, this, actionID, description, actionData);
			}
			if (actionType == UserMessageRuleActionType.StoreToFTPFolder)
			{
				return new UserMessageRuleAction_StoreToFtp(this.m_pRule, this, actionID, description, actionData);
			}
			throw new Exception("Invalid action type !");
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_pActions.GetEnumerator();
		}
	}
}
