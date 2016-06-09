using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class AddEditUserMessageRuleActionForm : Form
	{
		private PictureBox m_pIcon;

		private Label mt_Info;

		private GroupBox m_pSeparator1;

		private Label mt_Description;

		private TextBox m_pDescription;

		private Label mt_Action;

		private ComboBox m_pAction;

		private GroupBox m_pSeparator2;

		private GroupBox m_pSeparator3;

		private Button m_pHelp;

		private Button m_pCancel;

		private Button m_Ok;

		private Label mt_AutoResponse_From;

		private TextBox m_pAutoResponse_From;

		private Label mt_AutoResponse_FullMEssage;

		private TextBox m_pAutoResponse_FullMEssage;

		private Button m_pAutoResponse_Compose;

		private Button mt_AutoResponse_Load;

		private Label mt_ExecuteProgram_ProgramToExecute;

		private TextBox m_pExecuteProgram_ProgramToExecute;

		private Button m_pExecuteProgram_BrowseProgramToExecute;

		private Label mt_ExecuteProgram_ProgramArguments;

		private TextBox m_pExecuteProgram_ProgramArguments;

		private Label mt_ForwardToEmail_Email;

		private TextBox m_pForwardToEmail_Email;

		private Label mt_ForwardToHost_Host;

		private TextBox m_pForwardToHost_Host;

		private Label mt_ForwardToHost_HostPort;

		private NumericUpDown m_pForwardToHost_HostPort;

		private Label mt_StoreToDiskFolder_Folder;

		private TextBox m_pStoreToDiskFolder_Folder;

		private Button m_pStoreToDiskFolder_BrowseFolder;

		private Label mt_MoveToIMAPFolder_Folder;

		private TextBox m_pMoveToIMAPFolder_Folder;

		private Label mt_AddHeaderField_FieldName;

		private TextBox m_pAddHeaderField_FieldName;

		private Label mt_AddHeaderField_FieldValue;

		private TextBox m_pAddHeaderField_FieldValue;

		private Label mt_RemoveHeaderField_FieldName;

		private TextBox m_pRemoveHeaderField_FieldName;

		private Label mt_SendErrorToClient_ErrorText;

		private TextBox m_pSendErrorToClient_ErrorText;

		private Label mt_StoreToFTPFolder_Server;

		private TextBox m_pStoreToFTPFolder_Server;

		private NumericUpDown m_pStoreToFTPFolder_Port;

		private Label mt_StoreToFTPFolder_User;

		private TextBox m_pStoreToFTPFolder_User;

		private Label mt_StoreToFTPFolder_Password;

		private TextBox m_pStoreToFTPFolder_Password;

		private Label mt_StoreToFTPFolder_Folder;

		private TextBox m_pStoreToFTPFolder_Folder;

		private Label mt_PostToNNTPNewsgroup_Server;

		private TextBox m_pPostToNNTPNewsgroup_Server;

		private NumericUpDown m_pPostToNNTPNewsgroup_Port;

		private Label mt_PostToNNTPNewsgroup_Newsgroup;

		private TextBox m_pPostToNNTPNewsgroup_Newsgroup;

		private Label mt_PostToHTTP_URL;

		private TextBox m_pPostToHTTP_URL;

		private UserMessageRule m_pRule;

		private UserMessageRuleActionBase m_pActionData;

		public string ActionID
		{
			get
			{
				if (this.m_pActionData != null)
				{
					return this.m_pActionData.ID;
				}
				return "";
			}
		}

		public string Description
		{
			get
			{
				return this.m_pDescription.Text;
			}
		}

		public GlobalMessageRuleActionType ActionType
		{
			get
			{
				return (GlobalMessageRuleActionType)((WComboBoxItem)this.m_pAction.SelectedItem).Tag;
			}
		}

		public AddEditUserMessageRuleActionForm(UserMessageRule rule)
		{
			this.m_pRule = rule;
			this.InitializeComponent();
			this.m_pAction.SelectedIndex = 0;
		}

		public AddEditUserMessageRuleActionForm(UserMessageRule rule, UserMessageRuleActionBase action)
		{
			this.m_pRule = rule;
			this.m_pActionData = action;
			this.InitializeComponent();
			this.m_pDescription.Text = action.Description;
			this.m_pAction.Enabled = false;
			if (action.ActionType == UserMessageRuleActionType.AutoResponse)
			{
				UserMessageRuleAction_AutoResponse userMessageRuleAction_AutoResponse = (UserMessageRuleAction_AutoResponse)action;
				this.m_pAutoResponse_From.Text = userMessageRuleAction_AutoResponse.From;
				this.m_pAutoResponse_FullMEssage.Text = Encoding.UTF8.GetString(userMessageRuleAction_AutoResponse.Message);
				this.m_pAction.SelectedIndex = 0;
				return;
			}
			if (action.ActionType == UserMessageRuleActionType.DeleteMessage)
			{
				UserMessageRuleAction_DeleteMessage arg_8F_0 = (UserMessageRuleAction_DeleteMessage)action;
				this.m_pAction.SelectedIndex = 1;
				return;
			}
			if (action.ActionType == UserMessageRuleActionType.ExecuteProgram)
			{
				UserMessageRuleAction_ExecuteProgram userMessageRuleAction_ExecuteProgram = (UserMessageRuleAction_ExecuteProgram)action;
				this.m_pExecuteProgram_ProgramToExecute.Text = userMessageRuleAction_ExecuteProgram.Program;
				this.m_pExecuteProgram_ProgramArguments.Text = userMessageRuleAction_ExecuteProgram.ProgramArguments;
				this.m_pAction.SelectedIndex = 2;
				return;
			}
			if (action.ActionType == UserMessageRuleActionType.ForwardToEmail)
			{
				UserMessageRuleAction_ForwardToEmail userMessageRuleAction_ForwardToEmail = (UserMessageRuleAction_ForwardToEmail)action;
				this.m_pForwardToEmail_Email.Text = userMessageRuleAction_ForwardToEmail.EmailAddress;
				this.m_pAction.SelectedIndex = 3;
				return;
			}
			if (action.ActionType == UserMessageRuleActionType.ForwardToHost)
			{
				UserMessageRuleAction_ForwardToHost userMessageRuleAction_ForwardToHost = (UserMessageRuleAction_ForwardToHost)action;
				this.m_pForwardToHost_Host.Text = userMessageRuleAction_ForwardToHost.Host;
				this.m_pForwardToHost_HostPort.Value = userMessageRuleAction_ForwardToHost.Port;
				this.m_pAction.SelectedIndex = 4;
				return;
			}
			if (action.ActionType == UserMessageRuleActionType.StoreToDiskFolder)
			{
				UserMessageRuleAction_StoreToDiskFolder userMessageRuleAction_StoreToDiskFolder = (UserMessageRuleAction_StoreToDiskFolder)action;
				this.m_pStoreToDiskFolder_Folder.Text = userMessageRuleAction_StoreToDiskFolder.Folder;
				this.m_pAction.SelectedIndex = 5;
				return;
			}
			if (action.ActionType == UserMessageRuleActionType.MoveToIMAPFolder)
			{
				UserMessageRuleAction_MoveToImapFolder userMessageRuleAction_MoveToImapFolder = (UserMessageRuleAction_MoveToImapFolder)action;
				this.m_pMoveToIMAPFolder_Folder.Text = userMessageRuleAction_MoveToImapFolder.Folder;
				this.m_pAction.SelectedIndex = 6;
				return;
			}
			if (action.ActionType == UserMessageRuleActionType.AddHeaderField)
			{
				UserMessageRuleAction_AddHeaderField userMessageRuleAction_AddHeaderField = (UserMessageRuleAction_AddHeaderField)action;
				this.m_pAddHeaderField_FieldName.Text = userMessageRuleAction_AddHeaderField.HeaderFieldName;
				this.m_pAddHeaderField_FieldValue.Text = userMessageRuleAction_AddHeaderField.HeaderFieldValue;
				this.m_pAction.SelectedIndex = 7;
				return;
			}
			if (action.ActionType == UserMessageRuleActionType.RemoveHeaderField)
			{
				UserMessageRuleAction_RemoveHeaderField userMessageRuleAction_RemoveHeaderField = (UserMessageRuleAction_RemoveHeaderField)action;
				this.m_pRemoveHeaderField_FieldName.Text = userMessageRuleAction_RemoveHeaderField.HeaderFieldName;
				this.m_pAction.SelectedIndex = 8;
				return;
			}
			if (action.ActionType == UserMessageRuleActionType.StoreToFTPFolder)
			{
				UserMessageRuleAction_StoreToFtp userMessageRuleAction_StoreToFtp = (UserMessageRuleAction_StoreToFtp)action;
				this.m_pStoreToFTPFolder_Server.Text = userMessageRuleAction_StoreToFtp.Server;
				this.m_pStoreToFTPFolder_Port.Value = userMessageRuleAction_StoreToFtp.Port;
				this.m_pStoreToFTPFolder_User.Text = userMessageRuleAction_StoreToFtp.UserName;
				this.m_pStoreToFTPFolder_Password.Text = userMessageRuleAction_StoreToFtp.Password;
				this.m_pStoreToFTPFolder_Folder.Text = userMessageRuleAction_StoreToFtp.Folder;
				this.m_pAction.SelectedIndex = 10;
				return;
			}
			if (action.ActionType == UserMessageRuleActionType.PostToNNTPNewsGroup)
			{
				UserMessageRuleAction_PostToNntpNewsgroup userMessageRuleAction_PostToNntpNewsgroup = (UserMessageRuleAction_PostToNntpNewsgroup)action;
				this.m_pPostToNNTPNewsgroup_Server.Text = userMessageRuleAction_PostToNntpNewsgroup.Server;
				this.m_pPostToNNTPNewsgroup_Port.Value = userMessageRuleAction_PostToNntpNewsgroup.Port;
				this.m_pPostToNNTPNewsgroup_Newsgroup.Text = userMessageRuleAction_PostToNntpNewsgroup.Newsgroup;
				this.m_pAction.SelectedIndex = 11;
				return;
			}
			if (action.ActionType == UserMessageRuleActionType.PostToHTTP)
			{
				UserMessageRuleAction_PostToHttp userMessageRuleAction_PostToHttp = (UserMessageRuleAction_PostToHttp)action;
				this.m_pPostToHTTP_URL.Text = userMessageRuleAction_PostToHttp.Url;
				this.m_pAction.SelectedIndex = 12;
			}
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(492, 373);
			base.StartPosition = FormStartPosition.CenterParent;
			this.MinimumSize = new Size(500, 400);
			base.MinimizeBox = false;
			this.Text = "User Message Rule Add/Edit Action";
			base.Icon = ResManager.GetIcon("ruleaction.ico");
			this.m_pIcon = new PictureBox();
			this.m_pIcon.Size = new Size(32, 32);
			this.m_pIcon.Location = new Point(10, 10);
			this.m_pIcon.Image = ResManager.GetIcon("ruleaction.ico").ToBitmap();
			this.mt_Info = new Label();
			this.mt_Info.Size = new Size(200, 32);
			this.mt_Info.Location = new Point(50, 10);
			this.mt_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Info.Text = "Specify action information.";
			this.m_pSeparator1 = new GroupBox();
			this.m_pSeparator1.Size = new Size(483, 3);
			this.m_pSeparator1.Location = new Point(7, 50);
			this.m_pSeparator1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.mt_Description = new Label();
			this.mt_Description.Size = new Size(100, 20);
			this.mt_Description.Location = new Point(0, 60);
			this.mt_Description.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Description.Text = "Description:";
			this.m_pDescription = new TextBox();
			this.m_pDescription.Size = new Size(380, 20);
			this.m_pDescription.Location = new Point(105, 60);
			this.mt_Action = new Label();
			this.mt_Action.Size = new Size(100, 20);
			this.mt_Action.Location = new Point(0, 85);
			this.mt_Action.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Action.Text = "Action:";
			this.m_pAction = new ComboBox();
			this.m_pAction.Size = new Size(160, 21);
			this.m_pAction.Location = new Point(105, 85);
			this.m_pAction.DropDownStyle = ComboBoxStyle.DropDownList;
			try
			{
				this.m_pAction.GetType().GetProperty("DropDownHeight").GetSetMethod(true).Invoke(this.m_pAction, new object[]
				{
					200
				});
			}
			catch
			{
			}
			this.m_pAction.SelectedIndexChanged += new EventHandler(this.m_pAction_SelectedIndexChanged);
			this.m_pAction.Items.Add(new WComboBoxItem("Auto Response", GlobalMessageRuleActionType.AutoResponse));
			this.m_pAction.Items.Add(new WComboBoxItem("Delete Message", GlobalMessageRuleActionType.DeleteMessage));
			this.m_pAction.Items.Add(new WComboBoxItem("Execute Program", GlobalMessageRuleActionType.ExecuteProgram));
			this.m_pAction.Items.Add(new WComboBoxItem("Forward To Email", GlobalMessageRuleActionType.ForwardToEmail));
			this.m_pAction.Items.Add(new WComboBoxItem("Forward To Host", GlobalMessageRuleActionType.ForwardToHost));
			this.m_pAction.Items.Add(new WComboBoxItem("Store To Disk Folder", GlobalMessageRuleActionType.StoreToDiskFolder));
			this.m_pAction.Items.Add(new WComboBoxItem("Move To IMAP Folder", GlobalMessageRuleActionType.MoveToIMAPFolder));
			this.m_pAction.Items.Add(new WComboBoxItem("Add Header Field", GlobalMessageRuleActionType.AddHeaderField));
			this.m_pAction.Items.Add(new WComboBoxItem("Remove Header Field", GlobalMessageRuleActionType.RemoveHeaderField));
			this.m_pAction.Items.Add(new WComboBoxItem("Store To FTP Folder", GlobalMessageRuleActionType.StoreToFTPFolder));
			this.m_pAction.Items.Add(new WComboBoxItem("Post To NNTP Newsgroup", GlobalMessageRuleActionType.PostToNNTPNewsGroup));
			this.m_pAction.Items.Add(new WComboBoxItem("Post To HTTP", GlobalMessageRuleActionType.PostToHTTP));
			this.m_pSeparator2 = new GroupBox();
			this.m_pSeparator2.Size = new Size(483, 3);
			this.m_pSeparator2.Location = new Point(7, 115);
			this.m_pSeparator2.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pSeparator3 = new GroupBox();
			this.m_pSeparator3.Size = new Size(483, 3);
			this.m_pSeparator3.Location = new Point(7, 335);
			this.m_pSeparator3.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pHelp = new Button();
			this.m_pHelp.Size = new Size(70, 20);
			this.m_pHelp.Location = new Point(10, 350);
			this.m_pHelp.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.m_pHelp.Text = "Help";
			this.m_pHelp.Click += new EventHandler(this.m_pHelp_Click);
			this.m_pCancel = new Button();
			this.m_pCancel.Size = new Size(70, 20);
			this.m_pCancel.Location = new Point(340, 350);
			this.m_pCancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.m_pCancel.Text = "Cancel";
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			this.m_Ok = new Button();
			this.m_Ok.Size = new Size(70, 20);
			this.m_Ok.Location = new Point(415, 350);
			this.m_Ok.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.m_Ok.Text = "Ok";
			this.m_Ok.Click += new EventHandler(this.m_Ok_Click);
			this.mt_AutoResponse_From = new Label();
			this.mt_AutoResponse_From.Size = new Size(100, 20);
			this.mt_AutoResponse_From.Location = new Point(0, 125);
			this.mt_AutoResponse_From.TextAlign = ContentAlignment.MiddleRight;
			this.mt_AutoResponse_From.Text = "From:";
			this.mt_AutoResponse_From.Visible = false;
			this.m_pAutoResponse_From = new TextBox();
			this.m_pAutoResponse_From.Size = new Size(380, 20);
			this.m_pAutoResponse_From.Location = new Point(105, 125);
			this.m_pAutoResponse_From.Visible = false;
			this.mt_AutoResponse_FullMEssage = new Label();
			this.mt_AutoResponse_FullMEssage.Size = new Size(100, 13);
			this.mt_AutoResponse_FullMEssage.Location = new Point(10, 155);
			this.mt_AutoResponse_FullMEssage.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_AutoResponse_FullMEssage.Text = "Full Message:";
			this.mt_AutoResponse_FullMEssage.Visible = false;
			this.m_pAutoResponse_FullMEssage = new TextBox();
			this.m_pAutoResponse_FullMEssage.Size = new Size(395, 145);
			this.m_pAutoResponse_FullMEssage.Location = new Point(12, 175);
			this.m_pAutoResponse_FullMEssage.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pAutoResponse_FullMEssage.AcceptsReturn = true;
			this.m_pAutoResponse_FullMEssage.AcceptsTab = true;
			this.m_pAutoResponse_FullMEssage.Multiline = true;
			this.m_pAutoResponse_FullMEssage.ScrollBars = ScrollBars.Both;
			this.m_pAutoResponse_FullMEssage.Visible = false;
			this.m_pAutoResponse_Compose = new Button();
			this.m_pAutoResponse_Compose.Size = new Size(70, 21);
			this.m_pAutoResponse_Compose.Location = new Point(415, 175);
			this.m_pAutoResponse_Compose.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.m_pAutoResponse_Compose.Text = "Compose";
			this.m_pAutoResponse_Compose.Visible = false;
			this.m_pAutoResponse_Compose.Click += new EventHandler(this.m_pAutoResponse_Compose_Click);
			this.mt_AutoResponse_Load = new Button();
			this.mt_AutoResponse_Load.Size = new Size(70, 40);
			this.mt_AutoResponse_Load.Location = new Point(415, 200);
			this.mt_AutoResponse_Load.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.mt_AutoResponse_Load.Text = "Load Form File";
			this.mt_AutoResponse_Load.Visible = false;
			this.mt_AutoResponse_Load.Click += new EventHandler(this.mt_AutoResponse_Load_Click);
			this.mt_ExecuteProgram_ProgramToExecute = new Label();
			this.mt_ExecuteProgram_ProgramToExecute.Size = new Size(100, 20);
			this.mt_ExecuteProgram_ProgramToExecute.Location = new Point(0, 125);
			this.mt_ExecuteProgram_ProgramToExecute.Visible = false;
			this.mt_ExecuteProgram_ProgramToExecute.TextAlign = ContentAlignment.MiddleRight;
			this.mt_ExecuteProgram_ProgramToExecute.Text = "Program:";
			this.m_pExecuteProgram_ProgramToExecute = new TextBox();
			this.m_pExecuteProgram_ProgramToExecute.Size = new Size(350, 20);
			this.m_pExecuteProgram_ProgramToExecute.Location = new Point(105, 125);
			this.m_pExecuteProgram_ProgramToExecute.Visible = false;
			this.m_pExecuteProgram_BrowseProgramToExecute = new Button();
			this.m_pExecuteProgram_BrowseProgramToExecute.Size = new Size(25, 20);
			this.m_pExecuteProgram_BrowseProgramToExecute.Location = new Point(460, 125);
			this.m_pExecuteProgram_BrowseProgramToExecute.Visible = false;
			this.m_pExecuteProgram_BrowseProgramToExecute.Click += new EventHandler(this.m_pExecuteProgram_BrowseProgramToExecute_Click);
			this.m_pExecuteProgram_BrowseProgramToExecute.Text = "...";
			this.mt_ExecuteProgram_ProgramArguments = new Label();
			this.mt_ExecuteProgram_ProgramArguments.Size = new Size(100, 20);
			this.mt_ExecuteProgram_ProgramArguments.Location = new Point(0, 150);
			this.mt_ExecuteProgram_ProgramArguments.Visible = false;
			this.mt_ExecuteProgram_ProgramArguments.TextAlign = ContentAlignment.MiddleRight;
			this.mt_ExecuteProgram_ProgramArguments.Text = "Arguments:";
			this.m_pExecuteProgram_ProgramArguments = new TextBox();
			this.m_pExecuteProgram_ProgramArguments.Size = new Size(380, 20);
			this.m_pExecuteProgram_ProgramArguments.Location = new Point(105, 150);
			this.m_pExecuteProgram_ProgramArguments.Visible = false;
			this.mt_ForwardToEmail_Email = new Label();
			this.mt_ForwardToEmail_Email.Size = new Size(100, 20);
			this.mt_ForwardToEmail_Email.Location = new Point(0, 125);
			this.mt_ForwardToEmail_Email.Visible = false;
			this.mt_ForwardToEmail_Email.TextAlign = ContentAlignment.MiddleRight;
			this.mt_ForwardToEmail_Email.Text = "Email:";
			this.m_pForwardToEmail_Email = new TextBox();
			this.m_pForwardToEmail_Email.Size = new Size(380, 20);
			this.m_pForwardToEmail_Email.Location = new Point(105, 125);
			this.m_pForwardToEmail_Email.Visible = false;
			this.mt_ForwardToHost_Host = new Label();
			this.mt_ForwardToHost_Host.Size = new Size(100, 20);
			this.mt_ForwardToHost_Host.Location = new Point(0, 125);
			this.mt_ForwardToHost_Host.Visible = false;
			this.mt_ForwardToHost_Host.TextAlign = ContentAlignment.MiddleRight;
			this.mt_ForwardToHost_Host.Text = "Host:";
			this.m_pForwardToHost_Host = new TextBox();
			this.m_pForwardToHost_Host.Size = new Size(380, 20);
			this.m_pForwardToHost_Host.Location = new Point(105, 125);
			this.m_pForwardToHost_Host.Visible = false;
			this.mt_ForwardToHost_HostPort = new Label();
			this.mt_ForwardToHost_HostPort.Size = new Size(100, 20);
			this.mt_ForwardToHost_HostPort.Location = new Point(0, 150);
			this.mt_ForwardToHost_HostPort.Visible = false;
			this.mt_ForwardToHost_HostPort.TextAlign = ContentAlignment.MiddleRight;
			this.mt_ForwardToHost_HostPort.Text = "Port:";
			this.m_pForwardToHost_HostPort = new NumericUpDown();
			this.m_pForwardToHost_HostPort.Size = new Size(60, 20);
			this.m_pForwardToHost_HostPort.Location = new Point(105, 150);
			this.m_pForwardToHost_HostPort.Maximum = 99999m;
			this.m_pForwardToHost_HostPort.Minimum = 1m;
			this.m_pForwardToHost_HostPort.Value = 25m;
			this.m_pForwardToHost_HostPort.Visible = false;
			this.mt_StoreToDiskFolder_Folder = new Label();
			this.mt_StoreToDiskFolder_Folder.Size = new Size(100, 20);
			this.mt_StoreToDiskFolder_Folder.Location = new Point(0, 125);
			this.mt_StoreToDiskFolder_Folder.Visible = false;
			this.mt_StoreToDiskFolder_Folder.TextAlign = ContentAlignment.MiddleRight;
			this.mt_StoreToDiskFolder_Folder.Text = "Disk Folder:";
			this.m_pStoreToDiskFolder_Folder = new TextBox();
			this.m_pStoreToDiskFolder_Folder.Size = new Size(350, 120);
			this.m_pStoreToDiskFolder_Folder.Location = new Point(105, 125);
			this.m_pStoreToDiskFolder_Folder.Visible = false;
			this.m_pStoreToDiskFolder_BrowseFolder = new Button();
			this.m_pStoreToDiskFolder_BrowseFolder.Size = new Size(25, 20);
			this.m_pStoreToDiskFolder_BrowseFolder.Location = new Point(460, 125);
			this.m_pStoreToDiskFolder_BrowseFolder.Visible = false;
			this.m_pStoreToDiskFolder_BrowseFolder.Click += new EventHandler(this.m_pStoreToDiskFolder_BrowseFolder_Click);
			this.m_pStoreToDiskFolder_BrowseFolder.Text = "...";
			this.mt_MoveToIMAPFolder_Folder = new Label();
			this.mt_MoveToIMAPFolder_Folder.Size = new Size(100, 20);
			this.mt_MoveToIMAPFolder_Folder.Location = new Point(0, 125);
			this.mt_MoveToIMAPFolder_Folder.Visible = false;
			this.mt_MoveToIMAPFolder_Folder.TextAlign = ContentAlignment.MiddleRight;
			this.mt_MoveToIMAPFolder_Folder.Text = "IMAP Folder:";
			this.m_pMoveToIMAPFolder_Folder = new TextBox();
			this.m_pMoveToIMAPFolder_Folder.Size = new Size(380, 20);
			this.m_pMoveToIMAPFolder_Folder.Location = new Point(105, 125);
			this.m_pMoveToIMAPFolder_Folder.Visible = false;
			this.mt_AddHeaderField_FieldName = new Label();
			this.mt_AddHeaderField_FieldName.Size = new Size(100, 20);
			this.mt_AddHeaderField_FieldName.Location = new Point(0, 125);
			this.mt_AddHeaderField_FieldName.Visible = false;
			this.mt_AddHeaderField_FieldName.TextAlign = ContentAlignment.MiddleRight;
			this.mt_AddHeaderField_FieldName.Text = "Name:";
			this.m_pAddHeaderField_FieldName = new TextBox();
			this.m_pAddHeaderField_FieldName.Size = new Size(380, 120);
			this.m_pAddHeaderField_FieldName.Location = new Point(105, 125);
			this.m_pAddHeaderField_FieldName.Visible = false;
			this.mt_AddHeaderField_FieldValue = new Label();
			this.mt_AddHeaderField_FieldValue.Size = new Size(100, 20);
			this.mt_AddHeaderField_FieldValue.Location = new Point(0, 150);
			this.mt_AddHeaderField_FieldValue.Visible = false;
			this.mt_AddHeaderField_FieldValue.TextAlign = ContentAlignment.MiddleRight;
			this.mt_AddHeaderField_FieldValue.Text = "Value:";
			this.m_pAddHeaderField_FieldValue = new TextBox();
			this.m_pAddHeaderField_FieldValue.Size = new Size(380, 20);
			this.m_pAddHeaderField_FieldValue.Location = new Point(105, 150);
			this.m_pAddHeaderField_FieldValue.Visible = false;
			this.mt_RemoveHeaderField_FieldName = new Label();
			this.mt_RemoveHeaderField_FieldName.Size = new Size(100, 20);
			this.mt_RemoveHeaderField_FieldName.Location = new Point(0, 125);
			this.mt_RemoveHeaderField_FieldName.Visible = false;
			this.mt_RemoveHeaderField_FieldName.TextAlign = ContentAlignment.MiddleRight;
			this.mt_RemoveHeaderField_FieldName.Text = "Name:";
			this.m_pRemoveHeaderField_FieldName = new TextBox();
			this.m_pRemoveHeaderField_FieldName.Size = new Size(380, 20);
			this.m_pRemoveHeaderField_FieldName.Location = new Point(105, 125);
			this.m_pRemoveHeaderField_FieldName.Visible = false;
			this.mt_SendErrorToClient_ErrorText = new Label();
			this.mt_SendErrorToClient_ErrorText.Size = new Size(100, 20);
			this.mt_SendErrorToClient_ErrorText.Location = new Point(0, 125);
			this.mt_SendErrorToClient_ErrorText.Text = "Error Text:";
			this.mt_SendErrorToClient_ErrorText.TextAlign = ContentAlignment.MiddleRight;
			this.mt_SendErrorToClient_ErrorText.Visible = false;
			this.m_pSendErrorToClient_ErrorText = new TextBox();
			this.m_pSendErrorToClient_ErrorText.Size = new Size(380, 20);
			this.m_pSendErrorToClient_ErrorText.Location = new Point(105, 125);
			this.m_pSendErrorToClient_ErrorText.Visible = false;
			this.mt_StoreToFTPFolder_Server = new Label();
			this.mt_StoreToFTPFolder_Server.Size = new Size(100, 20);
			this.mt_StoreToFTPFolder_Server.Location = new Point(0, 125);
			this.mt_StoreToFTPFolder_Server.TextAlign = ContentAlignment.MiddleRight;
			this.mt_StoreToFTPFolder_Server.Text = "Host:";
			this.mt_StoreToFTPFolder_Server.Visible = false;
			this.m_pStoreToFTPFolder_Server = new TextBox();
			this.m_pStoreToFTPFolder_Server.Size = new Size(310, 20);
			this.m_pStoreToFTPFolder_Server.Location = new Point(105, 125);
			this.m_pStoreToFTPFolder_Server.Visible = false;
			this.m_pStoreToFTPFolder_Port = new NumericUpDown();
			this.m_pStoreToFTPFolder_Port.Size = new Size(65, 13);
			this.m_pStoreToFTPFolder_Port.Location = new Point(420, 125);
			this.m_pStoreToFTPFolder_Port.Minimum = 1m;
			this.m_pStoreToFTPFolder_Port.Maximum = 99999m;
			this.m_pStoreToFTPFolder_Port.Visible = false;
			this.mt_StoreToFTPFolder_User = new Label();
			this.mt_StoreToFTPFolder_User.Size = new Size(100, 20);
			this.mt_StoreToFTPFolder_User.Location = new Point(0, 150);
			this.mt_StoreToFTPFolder_User.TextAlign = ContentAlignment.MiddleRight;
			this.mt_StoreToFTPFolder_User.Text = "User:";
			this.mt_StoreToFTPFolder_User.Visible = false;
			this.m_pStoreToFTPFolder_User = new TextBox();
			this.m_pStoreToFTPFolder_User.Size = new Size(200, 13);
			this.m_pStoreToFTPFolder_User.Location = new Point(105, 150);
			this.m_pStoreToFTPFolder_User.Visible = false;
			this.mt_StoreToFTPFolder_Password = new Label();
			this.mt_StoreToFTPFolder_Password.Size = new Size(100, 20);
			this.mt_StoreToFTPFolder_Password.Location = new Point(0, 175);
			this.mt_StoreToFTPFolder_Password.TextAlign = ContentAlignment.MiddleRight;
			this.mt_StoreToFTPFolder_Password.Text = "Password:";
			this.mt_StoreToFTPFolder_Password.Visible = false;
			this.m_pStoreToFTPFolder_Password = new TextBox();
			this.m_pStoreToFTPFolder_Password.Size = new Size(200, 20);
			this.m_pStoreToFTPFolder_Password.Location = new Point(105, 175);
			this.m_pStoreToFTPFolder_Password.Visible = false;
			this.mt_StoreToFTPFolder_Folder = new Label();
			this.mt_StoreToFTPFolder_Folder.Size = new Size(100, 20);
			this.mt_StoreToFTPFolder_Folder.Location = new Point(0, 200);
			this.mt_StoreToFTPFolder_Folder.TextAlign = ContentAlignment.MiddleRight;
			this.mt_StoreToFTPFolder_Folder.Text = "Folder:";
			this.mt_StoreToFTPFolder_Folder.Visible = false;
			this.m_pStoreToFTPFolder_Folder = new TextBox();
			this.m_pStoreToFTPFolder_Folder.Size = new Size(380, 20);
			this.m_pStoreToFTPFolder_Folder.Location = new Point(105, 200);
			this.m_pStoreToFTPFolder_Folder.Visible = false;
			this.mt_PostToNNTPNewsgroup_Server = new Label();
			this.mt_PostToNNTPNewsgroup_Server.Size = new Size(100, 20);
			this.mt_PostToNNTPNewsgroup_Server.Location = new Point(0, 125);
			this.mt_PostToNNTPNewsgroup_Server.TextAlign = ContentAlignment.MiddleRight;
			this.mt_PostToNNTPNewsgroup_Server.Text = "Host:";
			this.mt_PostToNNTPNewsgroup_Server.Visible = false;
			this.m_pPostToNNTPNewsgroup_Server = new TextBox();
			this.m_pPostToNNTPNewsgroup_Server.Size = new Size(310, 20);
			this.m_pPostToNNTPNewsgroup_Server.Location = new Point(105, 125);
			this.m_pPostToNNTPNewsgroup_Server.Visible = false;
			this.m_pPostToNNTPNewsgroup_Port = new NumericUpDown();
			this.m_pPostToNNTPNewsgroup_Port.Size = new Size(65, 20);
			this.m_pPostToNNTPNewsgroup_Port.Location = new Point(420, 125);
			this.m_pPostToNNTPNewsgroup_Port.Minimum = 1m;
			this.m_pPostToNNTPNewsgroup_Port.Maximum = 99999m;
			this.m_pPostToNNTPNewsgroup_Port.Value = 119m;
			this.m_pPostToNNTPNewsgroup_Port.Visible = false;
			this.mt_PostToNNTPNewsgroup_Newsgroup = new Label();
			this.mt_PostToNNTPNewsgroup_Newsgroup.Size = new Size(100, 20);
			this.mt_PostToNNTPNewsgroup_Newsgroup.Location = new Point(0, 150);
			this.mt_PostToNNTPNewsgroup_Newsgroup.TextAlign = ContentAlignment.MiddleRight;
			this.mt_PostToNNTPNewsgroup_Newsgroup.Text = "Newsgroup:";
			this.mt_PostToNNTPNewsgroup_Newsgroup.Visible = false;
			this.m_pPostToNNTPNewsgroup_Newsgroup = new TextBox();
			this.m_pPostToNNTPNewsgroup_Newsgroup.Size = new Size(380, 13);
			this.m_pPostToNNTPNewsgroup_Newsgroup.Location = new Point(105, 150);
			this.m_pPostToNNTPNewsgroup_Newsgroup.Visible = false;
			this.mt_PostToHTTP_URL = new Label();
			this.mt_PostToHTTP_URL.Size = new Size(100, 20);
			this.mt_PostToHTTP_URL.Location = new Point(0, 125);
			this.mt_PostToHTTP_URL.TextAlign = ContentAlignment.MiddleRight;
			this.mt_PostToHTTP_URL.Text = "URL:";
			this.mt_PostToHTTP_URL.Visible = false;
			this.m_pPostToHTTP_URL = new TextBox();
			this.m_pPostToHTTP_URL.Size = new Size(380, 20);
			this.m_pPostToHTTP_URL.Location = new Point(105, 125);
			this.m_pPostToHTTP_URL.Visible = false;
			base.Controls.Add(this.m_pIcon);
			base.Controls.Add(this.mt_Info);
			base.Controls.Add(this.m_pSeparator1);
			base.Controls.Add(this.mt_Description);
			base.Controls.Add(this.m_pDescription);
			base.Controls.Add(this.mt_Action);
			base.Controls.Add(this.m_pAction);
			base.Controls.Add(this.m_pSeparator2);
			base.Controls.Add(this.m_pSeparator3);
			base.Controls.Add(this.m_pHelp);
			base.Controls.Add(this.m_pCancel);
			base.Controls.Add(this.m_Ok);
			base.Controls.Add(this.mt_AutoResponse_From);
			base.Controls.Add(this.m_pAutoResponse_From);
			base.Controls.Add(this.mt_AutoResponse_FullMEssage);
			base.Controls.Add(this.m_pAutoResponse_FullMEssage);
			base.Controls.Add(this.m_pAutoResponse_Compose);
			base.Controls.Add(this.mt_AutoResponse_Load);
			base.Controls.Add(this.mt_ExecuteProgram_ProgramToExecute);
			base.Controls.Add(this.m_pExecuteProgram_ProgramToExecute);
			base.Controls.Add(this.m_pExecuteProgram_BrowseProgramToExecute);
			base.Controls.Add(this.mt_ExecuteProgram_ProgramArguments);
			base.Controls.Add(this.m_pExecuteProgram_ProgramArguments);
			base.Controls.Add(this.mt_ForwardToEmail_Email);
			base.Controls.Add(this.m_pForwardToEmail_Email);
			base.Controls.Add(this.mt_ForwardToHost_Host);
			base.Controls.Add(this.m_pForwardToHost_Host);
			base.Controls.Add(this.mt_ForwardToHost_HostPort);
			base.Controls.Add(this.m_pForwardToHost_HostPort);
			base.Controls.Add(this.m_pStoreToDiskFolder_Folder);
			base.Controls.Add(this.m_pStoreToDiskFolder_BrowseFolder);
			base.Controls.Add(this.mt_StoreToDiskFolder_Folder);
			base.Controls.Add(this.mt_MoveToIMAPFolder_Folder);
			base.Controls.Add(this.m_pMoveToIMAPFolder_Folder);
			base.Controls.Add(this.mt_AddHeaderField_FieldName);
			base.Controls.Add(this.m_pAddHeaderField_FieldName);
			base.Controls.Add(this.mt_AddHeaderField_FieldValue);
			base.Controls.Add(this.m_pAddHeaderField_FieldValue);
			base.Controls.Add(this.mt_RemoveHeaderField_FieldName);
			base.Controls.Add(this.m_pRemoveHeaderField_FieldName);
			base.Controls.Add(this.mt_SendErrorToClient_ErrorText);
			base.Controls.Add(this.m_pSendErrorToClient_ErrorText);
			base.Controls.Add(this.mt_StoreToFTPFolder_Server);
			base.Controls.Add(this.m_pStoreToFTPFolder_Server);
			base.Controls.Add(this.m_pStoreToFTPFolder_Port);
			base.Controls.Add(this.mt_StoreToFTPFolder_User);
			base.Controls.Add(this.m_pAddHeaderField_FieldValue);
			base.Controls.Add(this.m_pStoreToFTPFolder_User);
			base.Controls.Add(this.mt_StoreToFTPFolder_Password);
			base.Controls.Add(this.m_pStoreToFTPFolder_Password);
			base.Controls.Add(this.mt_StoreToFTPFolder_Folder);
			base.Controls.Add(this.m_pStoreToFTPFolder_Folder);
			base.Controls.Add(this.mt_PostToNNTPNewsgroup_Server);
			base.Controls.Add(this.m_pPostToNNTPNewsgroup_Server);
			base.Controls.Add(this.m_pPostToNNTPNewsgroup_Port);
			base.Controls.Add(this.mt_PostToNNTPNewsgroup_Newsgroup);
			base.Controls.Add(this.m_pPostToNNTPNewsgroup_Newsgroup);
			base.Controls.Add(this.mt_PostToHTTP_URL);
			base.Controls.Add(this.m_pPostToHTTP_URL);
		}

		private void m_pAction_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.mt_AutoResponse_From.Visible = false;
			this.m_pAutoResponse_From.Visible = false;
			this.mt_AutoResponse_FullMEssage.Visible = false;
			this.m_pAutoResponse_FullMEssage.Visible = false;
			this.m_pAutoResponse_Compose.Visible = false;
			this.mt_AutoResponse_Load.Visible = false;
			this.mt_ExecuteProgram_ProgramToExecute.Visible = false;
			this.m_pExecuteProgram_ProgramToExecute.Visible = false;
			this.m_pExecuteProgram_BrowseProgramToExecute.Visible = false;
			this.mt_ExecuteProgram_ProgramArguments.Visible = false;
			this.m_pExecuteProgram_ProgramArguments.Visible = false;
			this.mt_ForwardToEmail_Email.Visible = false;
			this.m_pForwardToEmail_Email.Visible = false;
			this.mt_ForwardToHost_Host.Visible = false;
			this.m_pForwardToHost_Host.Visible = false;
			this.mt_ForwardToHost_HostPort.Visible = false;
			this.m_pForwardToHost_HostPort.Visible = false;
			this.mt_StoreToDiskFolder_Folder.Visible = false;
			this.m_pStoreToDiskFolder_Folder.Visible = false;
			this.m_pStoreToDiskFolder_BrowseFolder.Visible = false;
			this.mt_MoveToIMAPFolder_Folder.Visible = false;
			this.m_pMoveToIMAPFolder_Folder.Visible = false;
			this.mt_AddHeaderField_FieldName.Visible = false;
			this.m_pAddHeaderField_FieldName.Visible = false;
			this.mt_AddHeaderField_FieldValue.Visible = false;
			this.m_pAddHeaderField_FieldValue.Visible = false;
			this.mt_RemoveHeaderField_FieldName.Visible = false;
			this.m_pRemoveHeaderField_FieldName.Visible = false;
			this.mt_SendErrorToClient_ErrorText.Visible = false;
			this.m_pSendErrorToClient_ErrorText.Visible = false;
			this.mt_StoreToFTPFolder_Server.Visible = false;
			this.m_pStoreToFTPFolder_Server.Visible = false;
			this.m_pStoreToFTPFolder_Port.Visible = false;
			this.mt_StoreToFTPFolder_User.Visible = false;
			this.m_pStoreToFTPFolder_User.Visible = false;
			this.mt_StoreToFTPFolder_Password.Visible = false;
			this.m_pStoreToFTPFolder_Password.Visible = false;
			this.mt_StoreToFTPFolder_Folder.Visible = false;
			this.m_pStoreToFTPFolder_Folder.Visible = false;
			this.mt_PostToNNTPNewsgroup_Server.Visible = false;
			this.m_pPostToNNTPNewsgroup_Server.Visible = false;
			this.m_pPostToNNTPNewsgroup_Port.Visible = false;
			this.mt_PostToNNTPNewsgroup_Newsgroup.Visible = false;
			this.m_pPostToNNTPNewsgroup_Newsgroup.Visible = false;
			this.mt_PostToHTTP_URL.Visible = false;
			this.m_pPostToHTTP_URL.Visible = false;
			if (this.m_pAction.SelectedItem.ToString() == "Auto Response")
			{
				this.mt_AutoResponse_From.Visible = true;
				this.m_pAutoResponse_From.Visible = true;
				this.mt_AutoResponse_FullMEssage.Visible = true;
				this.m_pAutoResponse_FullMEssage.Visible = true;
				this.m_pAutoResponse_Compose.Visible = true;
				this.mt_AutoResponse_Load.Visible = true;
				return;
			}
			if (this.m_pAction.SelectedItem.ToString() == "Delete Message")
			{
				return;
			}
			if (this.m_pAction.SelectedItem.ToString() == "Execute Program")
			{
				this.mt_ExecuteProgram_ProgramToExecute.Visible = true;
				this.m_pExecuteProgram_ProgramToExecute.Visible = true;
				this.m_pExecuteProgram_BrowseProgramToExecute.Visible = true;
				this.mt_ExecuteProgram_ProgramArguments.Visible = true;
				this.m_pExecuteProgram_ProgramArguments.Visible = true;
				return;
			}
			if (this.m_pAction.SelectedItem.ToString() == "Forward To Email")
			{
				this.mt_ForwardToEmail_Email.Visible = true;
				this.m_pForwardToEmail_Email.Visible = true;
				return;
			}
			if (this.m_pAction.SelectedItem.ToString() == "Forward To Host")
			{
				this.mt_ForwardToHost_Host.Visible = true;
				this.m_pForwardToHost_Host.Visible = true;
				this.mt_ForwardToHost_HostPort.Visible = true;
				this.m_pForwardToHost_HostPort.Visible = true;
				return;
			}
			if (this.m_pAction.SelectedItem.ToString() == "Store To Disk Folder")
			{
				this.mt_StoreToDiskFolder_Folder.Visible = true;
				this.m_pStoreToDiskFolder_Folder.Visible = true;
				this.m_pStoreToDiskFolder_BrowseFolder.Visible = true;
				return;
			}
			if (this.m_pAction.SelectedItem.ToString() == "Move To IMAP Folder")
			{
				this.mt_MoveToIMAPFolder_Folder.Visible = true;
				this.m_pMoveToIMAPFolder_Folder.Visible = true;
				return;
			}
			if (this.m_pAction.SelectedItem.ToString() == "Add Header Field")
			{
				this.mt_AddHeaderField_FieldName.Visible = true;
				this.m_pAddHeaderField_FieldName.Visible = true;
				this.mt_AddHeaderField_FieldValue.Visible = true;
				this.m_pAddHeaderField_FieldValue.Visible = true;
				return;
			}
			if (this.m_pAction.SelectedItem.ToString() == "Remove Header Field")
			{
				this.mt_RemoveHeaderField_FieldName.Visible = true;
				this.m_pRemoveHeaderField_FieldName.Visible = true;
				return;
			}
			if (this.m_pAction.SelectedItem.ToString() == "Remove Header Field")
			{
				this.mt_RemoveHeaderField_FieldName.Visible = true;
				this.m_pRemoveHeaderField_FieldName.Visible = true;
				return;
			}
			if (this.m_pAction.SelectedItem.ToString() == "Send Error To Client")
			{
				this.mt_SendErrorToClient_ErrorText.Visible = true;
				this.m_pSendErrorToClient_ErrorText.Visible = true;
				return;
			}
			if (this.m_pAction.SelectedItem.ToString() == "Store To FTP Folder")
			{
				this.mt_StoreToFTPFolder_Server.Visible = true;
				this.m_pStoreToFTPFolder_Server.Visible = true;
				this.m_pStoreToFTPFolder_Port.Visible = true;
				this.mt_StoreToFTPFolder_User.Visible = true;
				this.m_pStoreToFTPFolder_User.Visible = true;
				this.mt_StoreToFTPFolder_Password.Visible = true;
				this.m_pStoreToFTPFolder_Password.Visible = true;
				this.mt_StoreToFTPFolder_Folder.Visible = true;
				this.m_pStoreToFTPFolder_Folder.Visible = true;
				return;
			}
			if (this.m_pAction.SelectedItem.ToString() == "Post To NNTP Newsgroup")
			{
				this.mt_PostToNNTPNewsgroup_Server.Visible = true;
				this.m_pPostToNNTPNewsgroup_Server.Visible = true;
				this.m_pPostToNNTPNewsgroup_Port.Visible = true;
				this.mt_PostToNNTPNewsgroup_Newsgroup.Visible = true;
				this.m_pPostToNNTPNewsgroup_Newsgroup.Visible = true;
				return;
			}
			if (this.m_pAction.SelectedItem.ToString() == "Post To HTTP")
			{
				this.mt_PostToHTTP_URL.Visible = true;
				this.m_pPostToHTTP_URL.Visible = true;
			}
		}

		private void m_pHelp_Click(object sender, EventArgs e)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo("explorer", Application.StartupPath + "\\Help\\Grules-Actions.htm");
			Process.Start(startInfo);
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
		}

		private void m_Ok_Click(object sender, EventArgs e)
		{
			if (this.m_pAction.SelectedItem.ToString() == "Auto Response")
			{
				if (this.m_pAutoResponse_FullMEssage.Text == "")
				{
					MessageBox.Show(this, "Full Message: value can't empty !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				if (this.m_pActionData == null)
				{
					this.m_pActionData = this.m_pRule.Actions.Add_AutoResponse(this.m_pDescription.Text, this.m_pAutoResponse_From.Text, Encoding.UTF8.GetBytes(this.m_pAutoResponse_FullMEssage.Text));
				}
				else
				{
					UserMessageRuleAction_AutoResponse userMessageRuleAction_AutoResponse = (UserMessageRuleAction_AutoResponse)this.m_pActionData;
					userMessageRuleAction_AutoResponse.Description = this.m_pDescription.Text;
					userMessageRuleAction_AutoResponse.From = this.m_pAutoResponse_From.Text;
					userMessageRuleAction_AutoResponse.Message = Encoding.UTF8.GetBytes(this.m_pAutoResponse_FullMEssage.Text);
					userMessageRuleAction_AutoResponse.Commit();
				}
			}
			else if (this.m_pAction.SelectedItem.ToString() == "Delete Message")
			{
				if (this.m_pActionData == null)
				{
					this.m_pActionData = this.m_pRule.Actions.Add_DeleteMessage(this.m_pDescription.Text);
				}
				else
				{
					UserMessageRuleAction_DeleteMessage userMessageRuleAction_DeleteMessage = (UserMessageRuleAction_DeleteMessage)this.m_pActionData;
					userMessageRuleAction_DeleteMessage.Description = this.m_pDescription.Text;
					userMessageRuleAction_DeleteMessage.Commit();
				}
			}
			else if (this.m_pAction.SelectedItem.ToString() == "Execute Program")
			{
				if (this.m_pExecuteProgram_ProgramToExecute.Text == "")
				{
					MessageBox.Show(this, "Program to Execute: value can't empty !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				if (this.m_pActionData == null)
				{
					this.m_pActionData = this.m_pRule.Actions.Add_ExecuteProgram(this.m_pDescription.Text, this.m_pExecuteProgram_ProgramToExecute.Text, this.m_pExecuteProgram_ProgramArguments.Text);
				}
				else
				{
					UserMessageRuleAction_ExecuteProgram userMessageRuleAction_ExecuteProgram = (UserMessageRuleAction_ExecuteProgram)this.m_pActionData;
					userMessageRuleAction_ExecuteProgram.Description = this.m_pDescription.Text;
					userMessageRuleAction_ExecuteProgram.Program = this.m_pExecuteProgram_ProgramToExecute.Text;
					userMessageRuleAction_ExecuteProgram.ProgramArguments = this.m_pExecuteProgram_ProgramArguments.Text;
					userMessageRuleAction_ExecuteProgram.Commit();
				}
			}
			else if (this.m_pAction.SelectedItem.ToString() == "Forward To Email")
			{
				if (this.m_pForwardToEmail_Email.Text == "")
				{
					MessageBox.Show(this, "Forward to Email: value can't empty !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				if (this.m_pActionData == null)
				{
					this.m_pActionData = this.m_pRule.Actions.Add_ForwardToEmail(this.m_pDescription.Text, this.m_pForwardToEmail_Email.Text);
				}
				else
				{
					UserMessageRuleAction_ForwardToEmail userMessageRuleAction_ForwardToEmail = (UserMessageRuleAction_ForwardToEmail)this.m_pActionData;
					userMessageRuleAction_ForwardToEmail.Description = this.m_pDescription.Text;
					userMessageRuleAction_ForwardToEmail.EmailAddress = this.m_pForwardToEmail_Email.Text;
					userMessageRuleAction_ForwardToEmail.Commit();
				}
			}
			else if (this.m_pAction.SelectedItem.ToString() == "Forward To Host")
			{
				if (this.m_pForwardToHost_Host.Text == "")
				{
					MessageBox.Show(this, "Forward to Host: value can't empty !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				if (this.m_pActionData == null)
				{
					this.m_pActionData = this.m_pRule.Actions.Add_ForwardToHost(this.m_pDescription.Text, this.m_pForwardToHost_Host.Text, (int)this.m_pForwardToHost_HostPort.Value);
				}
				else
				{
					UserMessageRuleAction_ForwardToHost userMessageRuleAction_ForwardToHost = (UserMessageRuleAction_ForwardToHost)this.m_pActionData;
					userMessageRuleAction_ForwardToHost.Description = this.m_pDescription.Text;
					userMessageRuleAction_ForwardToHost.Host = this.m_pForwardToHost_Host.Text;
					userMessageRuleAction_ForwardToHost.Port = (int)this.m_pForwardToHost_HostPort.Value;
					userMessageRuleAction_ForwardToHost.Commit();
				}
			}
			else if (this.m_pAction.SelectedItem.ToString() == "Store To Disk Folder")
			{
				if (this.m_pStoreToDiskFolder_Folder.Text == "")
				{
					MessageBox.Show(this, "Store to Disk Folder: value can't empty !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				if (this.m_pActionData == null)
				{
					this.m_pActionData = this.m_pRule.Actions.Add_StoreToDisk(this.m_pDescription.Text, this.m_pStoreToDiskFolder_Folder.Text);
				}
				else
				{
					UserMessageRuleAction_StoreToDiskFolder userMessageRuleAction_StoreToDiskFolder = (UserMessageRuleAction_StoreToDiskFolder)this.m_pActionData;
					userMessageRuleAction_StoreToDiskFolder.Description = this.m_pDescription.Text;
					userMessageRuleAction_StoreToDiskFolder.Folder = this.m_pStoreToDiskFolder_Folder.Text;
					userMessageRuleAction_StoreToDiskFolder.Commit();
				}
			}
			else if (this.m_pAction.SelectedItem.ToString() == "Move To IMAP Folder")
			{
				if (this.m_pMoveToIMAPFolder_Folder.Text == "")
				{
					MessageBox.Show(this, "Move to IMAP Folder: value can't empty !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				if (this.m_pActionData == null)
				{
					this.m_pActionData = this.m_pRule.Actions.Add_MoveToImapFolder(this.m_pDescription.Text, this.m_pMoveToIMAPFolder_Folder.Text);
				}
				else
				{
					UserMessageRuleAction_MoveToImapFolder userMessageRuleAction_MoveToImapFolder = (UserMessageRuleAction_MoveToImapFolder)this.m_pActionData;
					userMessageRuleAction_MoveToImapFolder.Description = this.m_pDescription.Text;
					userMessageRuleAction_MoveToImapFolder.Folder = this.m_pMoveToIMAPFolder_Folder.Text;
					userMessageRuleAction_MoveToImapFolder.Commit();
				}
			}
			else if (this.m_pAction.SelectedItem.ToString() == "Add Header Field")
			{
				if (this.m_pAddHeaderField_FieldName.Text == "")
				{
					MessageBox.Show(this, "Header Field Name: value can't empty !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				if (this.m_pActionData == null)
				{
					this.m_pActionData = this.m_pRule.Actions.Add_AddHeaderField(this.m_pDescription.Text, this.m_pAddHeaderField_FieldName.Text, this.m_pAddHeaderField_FieldValue.Text);
				}
				else
				{
					UserMessageRuleAction_AddHeaderField userMessageRuleAction_AddHeaderField = (UserMessageRuleAction_AddHeaderField)this.m_pActionData;
					userMessageRuleAction_AddHeaderField.Description = this.m_pDescription.Text;
					userMessageRuleAction_AddHeaderField.HeaderFieldName = this.m_pAddHeaderField_FieldName.Text;
					userMessageRuleAction_AddHeaderField.HeaderFieldValue = this.m_pAddHeaderField_FieldValue.Text;
					userMessageRuleAction_AddHeaderField.Commit();
				}
			}
			else if (this.m_pAction.SelectedItem.ToString() == "Remove Header Field")
			{
				if (this.m_pRemoveHeaderField_FieldName.Text == "")
				{
					MessageBox.Show(this, "Header Field Name: value can't empty !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				if (this.m_pActionData == null)
				{
					this.m_pActionData = this.m_pRule.Actions.Add_RemoveHeaderField(this.m_pDescription.Text, this.m_pAddHeaderField_FieldName.Text);
				}
				else
				{
					UserMessageRuleAction_RemoveHeaderField userMessageRuleAction_RemoveHeaderField = (UserMessageRuleAction_RemoveHeaderField)this.m_pActionData;
					userMessageRuleAction_RemoveHeaderField.Description = this.m_pDescription.Text;
					userMessageRuleAction_RemoveHeaderField.HeaderFieldName = this.m_pAddHeaderField_FieldName.Text;
					userMessageRuleAction_RemoveHeaderField.Commit();
				}
			}
			else if (this.m_pAction.SelectedItem.ToString() == "Store To FTP Folder")
			{
				if (this.m_pStoreToFTPFolder_Server.Text == "")
				{
					MessageBox.Show(this, "FTP Server: value can't empty !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				if (this.m_pActionData == null)
				{
					this.m_pActionData = this.m_pRule.Actions.Add_StoreToFtp(this.m_pDescription.Text, this.m_pStoreToFTPFolder_Server.Text, (int)this.m_pStoreToFTPFolder_Port.Value, this.m_pStoreToFTPFolder_User.Text, this.m_pStoreToFTPFolder_Password.Text, this.m_pStoreToFTPFolder_Folder.Text);
				}
				else
				{
					UserMessageRuleAction_StoreToFtp userMessageRuleAction_StoreToFtp = (UserMessageRuleAction_StoreToFtp)this.m_pActionData;
					userMessageRuleAction_StoreToFtp.Description = this.m_pDescription.Text;
					userMessageRuleAction_StoreToFtp.Server = this.m_pStoreToFTPFolder_Server.Text;
					userMessageRuleAction_StoreToFtp.Port = (int)this.m_pStoreToFTPFolder_Port.Value;
					userMessageRuleAction_StoreToFtp.UserName = this.m_pStoreToFTPFolder_User.Text;
					userMessageRuleAction_StoreToFtp.Password = this.m_pStoreToFTPFolder_Password.Text;
					userMessageRuleAction_StoreToFtp.Folder = this.m_pStoreToFTPFolder_Folder.Text;
					userMessageRuleAction_StoreToFtp.Commit();
				}
			}
			else if (this.m_pAction.SelectedItem.ToString() == "Post To NNTP Newsgroup")
			{
				if (this.m_pPostToNNTPNewsgroup_Server.Text == "")
				{
					MessageBox.Show(this, "NNTP Server: value can't empty !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				if (this.m_pPostToNNTPNewsgroup_Newsgroup.Text == "")
				{
					MessageBox.Show(this, "Newsgroup: value can't empty !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				if (this.m_pActionData == null)
				{
					this.m_pActionData = this.m_pRule.Actions.Add_PostToNntp(this.m_pDescription.Text, this.m_pPostToNNTPNewsgroup_Server.Text, (int)this.m_pPostToNNTPNewsgroup_Port.Value, this.m_pPostToNNTPNewsgroup_Newsgroup.Text);
				}
				else
				{
					UserMessageRuleAction_PostToNntpNewsgroup userMessageRuleAction_PostToNntpNewsgroup = (UserMessageRuleAction_PostToNntpNewsgroup)this.m_pActionData;
					userMessageRuleAction_PostToNntpNewsgroup.Description = this.m_pDescription.Text;
					userMessageRuleAction_PostToNntpNewsgroup.Server = this.m_pPostToNNTPNewsgroup_Server.Text;
					userMessageRuleAction_PostToNntpNewsgroup.Port = (int)this.m_pPostToNNTPNewsgroup_Port.Value;
					userMessageRuleAction_PostToNntpNewsgroup.Newsgroup = this.m_pPostToNNTPNewsgroup_Newsgroup.Text;
					userMessageRuleAction_PostToNntpNewsgroup.Commit();
				}
			}
			else if (this.m_pAction.SelectedItem.ToString() == "Post To HTTP")
			{
				if (this.m_pActionData == null)
				{
					this.m_pActionData = this.m_pRule.Actions.Add_PostToHttp(this.m_pDescription.Text, this.m_pPostToHTTP_URL.Text);
				}
				else
				{
					UserMessageRuleAction_PostToHttp userMessageRuleAction_PostToHttp = (UserMessageRuleAction_PostToHttp)this.m_pActionData;
					userMessageRuleAction_PostToHttp.Description = this.m_pDescription.Text;
					userMessageRuleAction_PostToHttp.Url = this.m_pPostToHTTP_URL.Text;
					userMessageRuleAction_PostToHttp.Commit();
				}
			}
			base.DialogResult = DialogResult.OK;
		}

		private void m_pAutoResponse_Compose_Click(object sender, EventArgs e)
		{
			GlobalMessageRuleActionComposeForm globalMessageRuleActionComposeForm = new GlobalMessageRuleActionComposeForm();
			if (globalMessageRuleActionComposeForm.ShowDialog(this) == DialogResult.OK)
			{
				this.m_pAutoResponse_FullMEssage.Text = globalMessageRuleActionComposeForm.Message;
			}
		}

		private void mt_AutoResponse_Load_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				this.m_pAutoResponse_FullMEssage.Text = File.ReadAllText(openFileDialog.FileName);
			}
		}

		private void m_pExecuteProgram_BrowseProgramToExecute_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				this.m_pExecuteProgram_ProgramToExecute.Text = openFileDialog.FileName;
			}
		}

		private void m_pStoreToDiskFolder_BrowseFolder_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
			{
				this.m_pStoreToDiskFolder_Folder.Text = folderBrowserDialog.SelectedPath;
			}
		}
	}
}
