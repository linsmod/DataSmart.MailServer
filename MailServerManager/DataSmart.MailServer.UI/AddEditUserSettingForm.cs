using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using DataSmart.MailServer.UI.UserForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class AddEditUserSettingForm : Form
	{
		private TabControl m_pTab;

		private TabPage m_pTabPage_General;

		private TabPage m_pTabPage_Addressing;

		private TabPage m_pTabPage_Rules;

		private TabPage m_pTabPage_RemoteServers;

		private TabPage m_pTabPage_Permissions;

		private TabPage m_pTabPage_Folders;

		private Button m_Cancel;

		private Button m_pOk;

		private PictureBox m_pTab_General_Icon;

		private Label mt_Tab_General_Info;

		private GroupBox m_pTab_General_Separator1;

		private CheckBox m_pGeneral_Enabled;

		private Label mt_General_FullName;

		private TextBox m_pGeneral_FullName;

		private Label mt_General_Description;

		private TextBox m_pGeneral_Description;

		private Label mt_General_LoginName;

		private TextBox m_pGeneral_LoginName;

		private Label mt_General_Password;

		private TextBox m_pGeneral_Password;

		private Button m_pGeneral_GeneratePwd;

		private Label mt_General_MaxMailboxSize;

		private NumericUpDown m_pGeneral_MaxMailboxSize;

		private Label mt_General_MaxMailboxMB;

		private GroupBox m_pTab_General_Separator2;

		private Label mt_Tab_General_MailboxSize;

		private Label m_pTab_General_MailboxSize;

		private ProgressBar m_pGeneral_MailboxSizeIndicator;

		private Label mt_Tab_General_Created;

		private Label m_pTab_General_Created;

		private Label mt_Tab_General_LastLogin;

		private Label m_pTab_General_LastLogin;

		private Button m_pTab_General_Create;

		private PictureBox m_pTab_Addressing_Icon;

		private Label mt_Tab_Addressing_Info;

		private GroupBox m_pTab_Addressing_Separator1;

		private TextBox m_pTab_Addressing_LocalPart;

		private Label mt_Tab_Addressing_At;

		private ComboBox m_pTab_Addressing_Domain;

		private ToolStrip m_pTab_Addressing_Toolbar;

		private ListView m_pTab_Addressing_Addresses;

		private PictureBox m_pTab_Rules_Icon;

		private Label mt_Tab_Rules_Info;

		private GroupBox m_pTab_Rules_Separator1;

		private ToolStrip m_pTab_Rules_Toolbar;

		private ListView m_pRules_Rules;

		private PictureBox m_pTab_RemoteServers_Icon;

		private Label mt_Tab_RemoteServers_Info;

		private GroupBox m_pTab_RemoteServers_Separator1;

		private ToolStrip m_pTab_RemoteServers_Toolbar;

		private ListView m_pRemoteServers_Servers;

		private PictureBox m_pTab_Permissions_Icon;

		private Label mt_Tab_Permissions_Info;

		private GroupBox m_pTab_Permissions_Separator1;

		private CheckBox m_pPermissions_AllowPop3;

		private CheckBox m_pPermissions_AllowImap;

		private CheckBox m_pPermissions_AllowRelay;

		private CheckBox m_pPermissions_AllowSIP;

		private Label mt_Permissions_SipGwAccess;

		private ToolStrip m_pPermissions_SipGwAccessToolbar;

		private ListView m_pPermissions_SipGwAccess;

		private PictureBox m_pTab_Folders_Icon;

		private Label mt_Tab_Folders_Info;

		private GroupBox m_pTab_Folders_Separator1;

		private ToolStrip m_pTab_Folders_Toolbar;

		private TreeView m_pTab_Folders_Folders;

		private VirtualServer m_pVirtualServer;

		private MailServer.Management.User m_pUser;

		public string UserID
		{
			get
			{
				if (this.m_pUser != null)
				{
					return this.m_pUser.UserID;
				}
				return "";
			}
		}

		public AddEditUserSettingForm(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			this.LoadDomains();
			this.m_pTab.TabPages.Remove(this.m_pTabPage_Addressing);
			this.m_pTab.TabPages.Remove(this.m_pTabPage_Rules);
			this.m_pTab.TabPages.Remove(this.m_pTabPage_RemoteServers);
			this.m_pTab.TabPages.Remove(this.m_pTabPage_Permissions);
			this.m_pTab.TabPages.Remove(this.m_pTabPage_Folders);
			this.m_pTab_General_Create.Visible = true;
		}

		public AddEditUserSettingForm(VirtualServer virtualServer, MailServer.Management.User user)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pUser = user;
			this.InitializeComponent();
			this.LoadDomains();
			this.LoadSettings();
			this.LoadRules("");
			this.LoadRemoteServers("");
			this.LoadFolders("");
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(492, 373);
			base.StartPosition = FormStartPosition.CenterScreen;
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			this.Text = "Add/Edit user settings";
			base.Icon = ResManager.GetIcon("user.ico");
			this.m_pTab = new TabControl();
			this.m_pTab.Size = new Size(493, 335);
			this.m_pTab.Location = new Point(0, 5);
			this.m_pTab.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pTab.TabPages.Add(new TabPage("General"));
			this.m_pTab.TabPages.Add(new TabPage("Addressing"));
			this.m_pTab.TabPages.Add(new TabPage("Rules"));
			this.m_pTab.TabPages.Add(new TabPage("Remote Servers"));
			this.m_pTab.TabPages.Add(new TabPage("Permissions"));
			this.m_pTab.TabPages.Add(new TabPage("Folders"));
			this.m_pTab.TabPages[1].Size = new Size(487, 311);
			this.m_pTab.TabPages[2].Size = new Size(487, 311);
			this.m_pTabPage_General = this.m_pTab.TabPages[0];
			this.m_pTabPage_Addressing = this.m_pTab.TabPages[1];
			this.m_pTabPage_Rules = this.m_pTab.TabPages[2];
			this.m_pTabPage_RemoteServers = this.m_pTab.TabPages[3];
			this.m_pTabPage_Permissions = this.m_pTab.TabPages[4];
			this.m_pTabPage_Folders = this.m_pTab.TabPages[5];
			this.m_Cancel = new Button();
			this.m_Cancel.Size = new Size(70, 20);
			this.m_Cancel.Location = new Point(340, 350);
			this.m_Cancel.Text = "Cancel";
			this.m_Cancel.Click += new EventHandler(this.m_Cancel_Click);
			this.m_pOk = new Button();
			this.m_pOk.Size = new Size(70, 20);
			this.m_pOk.Location = new Point(415, 350);
			this.m_pOk.Text = "Ok";
			this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
			base.Controls.Add(this.m_pTab);
			base.Controls.Add(this.m_Cancel);
			base.Controls.Add(this.m_pOk);
			this.m_pTab_General_Icon = new PictureBox();
			this.m_pTab_General_Icon.Size = new Size(32, 32);
			this.m_pTab_General_Icon.Location = new Point(10, 10);
			this.m_pTab_General_Icon.Image = ResManager.GetIcon("userinfo.ico").ToBitmap();
			this.mt_Tab_General_Info = new Label();
			this.mt_Tab_General_Info.Size = new Size(200, 32);
			this.mt_Tab_General_Info.Location = new Point(50, 10);
			this.mt_Tab_General_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Tab_General_Info.Text = "Specify user info.";
			this.m_pTab_General_Separator1 = new GroupBox();
			this.m_pTab_General_Separator1.Size = new Size(475, 3);
			this.m_pTab_General_Separator1.Location = new Point(7, 50);
			this.m_pGeneral_Enabled = new CheckBox();
			this.m_pGeneral_Enabled.Size = new Size(70, 20);
			this.m_pGeneral_Enabled.Location = new Point(110, 60);
			this.m_pGeneral_Enabled.Text = "Enabled";
			this.m_pGeneral_Enabled.Checked = true;
			this.mt_General_FullName = new Label();
			this.mt_General_FullName.Size = new Size(100, 20);
			this.mt_General_FullName.Location = new Point(5, 85);
			this.mt_General_FullName.TextAlign = ContentAlignment.MiddleRight;
			this.mt_General_FullName.Text = "Full Name:";
			this.m_pGeneral_FullName = new TextBox();
			this.m_pGeneral_FullName.Size = new Size(360, 20);
			this.m_pGeneral_FullName.Location = new Point(110, 85);
			this.mt_General_Description = new Label();
			this.mt_General_Description.Size = new Size(100, 20);
			this.mt_General_Description.Location = new Point(5, 110);
			this.mt_General_Description.TextAlign = ContentAlignment.MiddleRight;
			this.mt_General_Description.Text = "Description:";
			this.m_pGeneral_Description = new TextBox();
			this.m_pGeneral_Description.Size = new Size(360, 20);
			this.m_pGeneral_Description.Location = new Point(110, 110);
			this.mt_General_LoginName = new Label();
			this.mt_General_LoginName.Size = new Size(100, 20);
			this.mt_General_LoginName.Location = new Point(5, 135);
			this.mt_General_LoginName.TextAlign = ContentAlignment.MiddleRight;
			this.mt_General_LoginName.Text = "Login Name:";
			this.m_pGeneral_LoginName = new TextBox();
			this.m_pGeneral_LoginName.Size = new Size(180, 20);
			this.m_pGeneral_LoginName.Location = new Point(110, 135);
			this.mt_General_Password = new Label();
			this.mt_General_Password.Size = new Size(100, 20);
			this.mt_General_Password.Location = new Point(5, 160);
			this.mt_General_Password.TextAlign = ContentAlignment.MiddleRight;
			this.mt_General_Password.Text = "Password:";
			this.m_pGeneral_Password = new TextBox();
			this.m_pGeneral_Password.Size = new Size(180, 20);
			this.m_pGeneral_Password.Location = new Point(110, 160);
			this.m_pGeneral_GeneratePwd = new Button();
			this.m_pGeneral_GeneratePwd.Size = new Size(170, 20);
			this.m_pGeneral_GeneratePwd.Location = new Point(300, 160);
			this.m_pGeneral_GeneratePwd.Text = "Generate Password";
			this.m_pGeneral_GeneratePwd.Click += new EventHandler(this.m_pGeneral_GeneratePwd_Click);
			this.mt_General_MaxMailboxSize = new Label();
			this.mt_General_MaxMailboxSize.Size = new Size(100, 20);
			this.mt_General_MaxMailboxSize.Location = new Point(5, 185);
			this.mt_General_MaxMailboxSize.TextAlign = ContentAlignment.MiddleRight;
			this.mt_General_MaxMailboxSize.Text = "Max mailbox size:";
			this.m_pGeneral_MaxMailboxSize = new NumericUpDown();
			this.m_pGeneral_MaxMailboxSize.Size = new Size(70, 20);
			this.m_pGeneral_MaxMailboxSize.Location = new Point(110, 185);
			this.m_pGeneral_MaxMailboxSize.Minimum = 0m;
			this.m_pGeneral_MaxMailboxSize.Maximum = 999999m;
			this.m_pGeneral_MaxMailboxSize.Value = 20m;
			this.mt_General_MaxMailboxMB = new Label();
			this.mt_General_MaxMailboxMB.Size = new Size(150, 20);
			this.mt_General_MaxMailboxMB.Location = new Point(180, 185);
			this.mt_General_MaxMailboxMB.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_General_MaxMailboxMB.Text = "MB (0 for unlimited)";
			this.m_pTab_General_Separator2 = new GroupBox();
			this.m_pTab_General_Separator2.Size = new Size(475, 3);
			this.m_pTab_General_Separator2.Location = new Point(7, 215);
			this.mt_Tab_General_MailboxSize = new Label();
			this.mt_Tab_General_MailboxSize.Size = new Size(100, 20);
			this.mt_Tab_General_MailboxSize.Location = new Point(5, 225);
			this.mt_Tab_General_MailboxSize.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Tab_General_MailboxSize.Text = "Mailbox size:";
			this.m_pTab_General_MailboxSize = new Label();
			this.m_pTab_General_MailboxSize.Size = new Size(170, 20);
			this.m_pTab_General_MailboxSize.Location = new Point(110, 225);
			this.m_pTab_General_MailboxSize.TextAlign = ContentAlignment.MiddleLeft;
			this.m_pGeneral_MailboxSizeIndicator = new ProgressBar();
			this.m_pGeneral_MailboxSizeIndicator.Size = new Size(170, 20);
			this.m_pGeneral_MailboxSizeIndicator.Location = new Point(300, 225);
			this.m_pGeneral_MailboxSizeIndicator.Style = ProgressBarStyle.Continuous;
			this.mt_Tab_General_Created = new Label();
			this.mt_Tab_General_Created.Size = new Size(100, 20);
			this.mt_Tab_General_Created.Location = new Point(5, 250);
			this.mt_Tab_General_Created.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Tab_General_Created.Text = "Created:";
			this.m_pTab_General_Created = new Label();
			this.m_pTab_General_Created.Size = new Size(250, 20);
			this.m_pTab_General_Created.Location = new Point(110, 250);
			this.m_pTab_General_Created.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Tab_General_LastLogin = new Label();
			this.mt_Tab_General_LastLogin.Size = new Size(100, 20);
			this.mt_Tab_General_LastLogin.Location = new Point(5, 275);
			this.mt_Tab_General_LastLogin.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Tab_General_LastLogin.Text = "Last login:";
			this.m_pTab_General_LastLogin = new Label();
			this.m_pTab_General_LastLogin.Size = new Size(250, 20);
			this.m_pTab_General_LastLogin.Location = new Point(110, 275);
			this.m_pTab_General_LastLogin.TextAlign = ContentAlignment.MiddleLeft;
			this.m_pTab_General_Create = new Button();
			this.m_pTab_General_Create.Size = new Size(70, 20);
			this.m_pTab_General_Create.Location = new Point(410, 280);
			this.m_pTab_General_Create.Text = "Create";
			this.m_pTab_General_Create.Visible = false;
			this.m_pTab_General_Create.Click += new EventHandler(this.m_pTab_General_Create_Click);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Icon);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_Tab_General_Info);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Separator1);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pGeneral_Enabled);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_General_FullName);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pGeneral_FullName);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_General_Description);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pGeneral_Description);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_General_LoginName);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pGeneral_LoginName);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_General_Password);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pGeneral_Password);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pGeneral_GeneratePwd);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_General_MaxMailboxSize);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pGeneral_MaxMailboxSize);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_General_MaxMailboxMB);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Separator2);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_Tab_General_MailboxSize);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_MailboxSize);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pGeneral_MailboxSizeIndicator);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_Tab_General_Created);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Created);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_Tab_General_LastLogin);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_LastLogin);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Create);
			this.m_pTab_Addressing_Icon = new PictureBox();
			this.m_pTab_Addressing_Icon.Size = new Size(32, 32);
			this.m_pTab_Addressing_Icon.Location = new Point(10, 10);
			this.m_pTab_Addressing_Icon.Image = ResManager.GetIcon("addressing.ico").ToBitmap();
			this.mt_Tab_Addressing_Info = new Label();
			this.mt_Tab_Addressing_Info.Size = new Size(200, 32);
			this.mt_Tab_Addressing_Info.Location = new Point(50, 10);
			this.mt_Tab_Addressing_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Tab_Addressing_Info.Text = "Specify user email addresses.";
			this.m_pTab_Addressing_Separator1 = new GroupBox();
			this.m_pTab_Addressing_Separator1.Size = new Size(475, 3);
			this.m_pTab_Addressing_Separator1.Location = new Point(7, 50);
			this.m_pTab_Addressing_LocalPart = new TextBox();
			this.m_pTab_Addressing_LocalPart.Size = new Size(190, 20);
			this.m_pTab_Addressing_LocalPart.Location = new Point(8, 55);
			this.mt_Tab_Addressing_At = new Label();
			this.mt_Tab_Addressing_At.Size = new Size(20, 20);
			this.mt_Tab_Addressing_At.Location = new Point(205, 55);
			this.mt_Tab_Addressing_At.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Tab_Addressing_At.Text = "@";
			this.m_pTab_Addressing_Domain = new ComboBox();
			this.m_pTab_Addressing_Domain.Size = new Size(190, 20);
			this.m_pTab_Addressing_Domain.Location = new Point(230, 55);
			this.m_pTab_Addressing_Domain.DropDownStyle = ComboBoxStyle.DropDownList;
			this.m_pTab_Addressing_Toolbar = new ToolStrip();
			this.m_pTab_Addressing_Toolbar.Location = new Point(430, 55);
			this.m_pTab_Addressing_Toolbar.Dock = DockStyle.None;
			this.m_pTab_Addressing_Toolbar.GripStyle = ToolStripGripStyle.Hidden;
			this.m_pTab_Addressing_Toolbar.BackColor = this.BackColor;
			this.m_pTab_Addressing_Toolbar.Renderer = new ToolBarRendererEx();
			this.m_pTab_Addressing_Toolbar.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pTab_Addressing_Toolbar_ItemClicked);
			ToolStripButton toolStripButton = new ToolStripButton();
			toolStripButton.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripButton.Tag = "add";
			this.m_pTab_Addressing_Toolbar.Items.Add(toolStripButton);
			ToolStripButton toolStripButton2 = new ToolStripButton();
			toolStripButton2.Enabled = false;
			toolStripButton2.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			toolStripButton2.Tag = "delete";
			this.m_pTab_Addressing_Toolbar.Items.Add(toolStripButton2);
			this.m_pTab_Addressing_Addresses = new ListView();
			this.m_pTab_Addressing_Addresses.Size = new Size(475, 220);
			this.m_pTab_Addressing_Addresses.Location = new Point(8, 80);
			this.m_pTab_Addressing_Addresses.View = View.Details;
			this.m_pTab_Addressing_Addresses.HideSelection = false;
			this.m_pTab_Addressing_Addresses.FullRowSelect = true;
			this.m_pTab_Addressing_Addresses.Columns.Add("Email Address", 450, HorizontalAlignment.Left);
			this.m_pTab_Addressing_Addresses.SelectedIndexChanged += new EventHandler(this.m_pTab_Addressing_Addresses_SelectedIndexChanged);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pTab_Addressing_Icon);
			this.m_pTab.TabPages[1].Controls.Add(this.mt_Tab_Addressing_Info);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pTab_Addressing_Separator1);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pTab_Addressing_LocalPart);
			this.m_pTab.TabPages[1].Controls.Add(this.mt_Tab_Addressing_At);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pTab_Addressing_Domain);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pTab_Addressing_Toolbar);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pTab_Addressing_Addresses);
			this.m_pTab_Rules_Icon = new PictureBox();
			this.m_pTab_Rules_Icon.Size = new Size(32, 32);
			this.m_pTab_Rules_Icon.Location = new Point(10, 10);
			this.m_pTab_Rules_Icon.Image = ResManager.GetIcon("rule.ico").ToBitmap();
			this.mt_Tab_Rules_Info = new Label();
			this.mt_Tab_Rules_Info.Size = new Size(200, 32);
			this.mt_Tab_Rules_Info.Location = new Point(50, 10);
			this.mt_Tab_Rules_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Tab_Rules_Info.Text = "Specify user message rules.";
			this.m_pTab_Rules_Separator1 = new GroupBox();
			this.m_pTab_Rules_Separator1.Size = new Size(475, 3);
			this.m_pTab_Rules_Separator1.Location = new Point(7, 50);
			this.m_pTab_Rules_Toolbar = new ToolStrip();
			this.m_pTab_Rules_Toolbar.Size = new Size(125, 25);
			this.m_pTab_Rules_Toolbar.Location = new Point(360, 55);
			this.m_pTab_Rules_Toolbar.Dock = DockStyle.None;
			this.m_pTab_Rules_Toolbar.GripStyle = ToolStripGripStyle.Hidden;
			this.m_pTab_Rules_Toolbar.BackColor = this.BackColor;
			this.m_pTab_Rules_Toolbar.Renderer = new ToolBarRendererEx();
			this.m_pTab_Rules_Toolbar.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pTab_Rules_Toolbar_ItemClicked);
			ToolStripButton toolStripButton3 = new ToolStripButton();
			toolStripButton3.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripButton3.Tag = "add";
			this.m_pTab_Rules_Toolbar.Items.Add(toolStripButton3);
			ToolStripButton toolStripButton4 = new ToolStripButton();
			toolStripButton4.Enabled = false;
			toolStripButton4.Image = ResManager.GetIcon("edit.ico").ToBitmap();
			toolStripButton4.Tag = "edit";
			this.m_pTab_Rules_Toolbar.Items.Add(toolStripButton4);
			ToolStripButton toolStripButton5 = new ToolStripButton();
			toolStripButton5.Enabled = false;
			toolStripButton5.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			toolStripButton5.Tag = "delete";
			this.m_pTab_Rules_Toolbar.Items.Add(toolStripButton5);
			this.m_pTab_Rules_Toolbar.Items.Add(new ToolStripSeparator());
			ToolStripButton toolStripButton6 = new ToolStripButton();
			toolStripButton6.Enabled = false;
			toolStripButton6.Image = ResManager.GetIcon("up.ico").ToBitmap();
			toolStripButton6.Tag = "up";
			this.m_pTab_Rules_Toolbar.Items.Add(toolStripButton6);
			ToolStripButton toolStripButton7 = new ToolStripButton();
			toolStripButton7.Enabled = false;
			toolStripButton7.Image = ResManager.GetIcon("down.ico").ToBitmap();
			toolStripButton7.Tag = "down";
			this.m_pTab_Rules_Toolbar.Items.Add(toolStripButton7);
			this.m_pRules_Rules = new ListView();
			this.m_pRules_Rules.Size = new Size(475, 220);
			this.m_pRules_Rules.Location = new Point(8, 80);
			this.m_pRules_Rules.View = View.Details;
			this.m_pRules_Rules.HideSelection = false;
			this.m_pRules_Rules.FullRowSelect = true;
			this.m_pRules_Rules.Columns.Add("Description", 450, HorizontalAlignment.Left);
			this.m_pRules_Rules.DoubleClick += new EventHandler(this.m_pRules_Rules_DoubleClick);
			this.m_pRules_Rules.SelectedIndexChanged += new EventHandler(this.m_pRules_Rules_SelectedIndexChanged);
			this.m_pTab.TabPages[2].Controls.Add(this.m_pTab_Rules_Icon);
			this.m_pTab.TabPages[2].Controls.Add(this.mt_Tab_Rules_Info);
			this.m_pTab.TabPages[2].Controls.Add(this.m_pTab_Rules_Separator1);
			this.m_pTab.TabPages[2].Controls.Add(this.m_pTab_Rules_Toolbar);
			this.m_pTab.TabPages[2].Controls.Add(this.m_pRules_Rules);
			this.m_pTab_RemoteServers_Icon = new PictureBox();
			this.m_pTab_RemoteServers_Icon.Size = new Size(32, 32);
			this.m_pTab_RemoteServers_Icon.Location = new Point(10, 10);
			this.m_pTab_RemoteServers_Icon.Image = ResManager.GetIcon("remoteserver32.ico").ToBitmap();
			this.mt_Tab_RemoteServers_Info = new Label();
			this.mt_Tab_RemoteServers_Info.Size = new Size(200, 32);
			this.mt_Tab_RemoteServers_Info.Location = new Point(50, 10);
			this.mt_Tab_RemoteServers_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Tab_RemoteServers_Info.Text = "Specify user remote mail servers.";
			this.m_pTab_RemoteServers_Separator1 = new GroupBox();
			this.m_pTab_RemoteServers_Separator1.Size = new Size(475, 3);
			this.m_pTab_RemoteServers_Separator1.Location = new Point(7, 50);
			this.m_pTab_RemoteServers_Toolbar = new ToolStrip();
			this.m_pTab_RemoteServers_Toolbar.Size = new Size(75, 25);
			this.m_pTab_RemoteServers_Toolbar.Location = new Point(410, 55);
			this.m_pTab_RemoteServers_Toolbar.Dock = DockStyle.None;
			this.m_pTab_RemoteServers_Toolbar.GripStyle = ToolStripGripStyle.Hidden;
			this.m_pTab_RemoteServers_Toolbar.BackColor = this.BackColor;
			this.m_pTab_RemoteServers_Toolbar.Renderer = new ToolBarRendererEx();
			this.m_pTab_RemoteServers_Toolbar.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pTab_RemoteServers_Toolbar_ItemClicked);
			ToolStripButton toolStripButton8 = new ToolStripButton();
			toolStripButton8.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripButton8.Tag = "add";
			this.m_pTab_RemoteServers_Toolbar.Items.Add(toolStripButton8);
			ToolStripButton toolStripButton9 = new ToolStripButton();
			toolStripButton9.Enabled = false;
			toolStripButton9.Image = ResManager.GetIcon("edit.ico").ToBitmap();
			toolStripButton9.Tag = "edit";
			this.m_pTab_RemoteServers_Toolbar.Items.Add(toolStripButton9);
			ToolStripButton toolStripButton10 = new ToolStripButton();
			toolStripButton10.Enabled = false;
			toolStripButton10.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			toolStripButton10.Tag = "delete";
			this.m_pTab_RemoteServers_Toolbar.Items.Add(toolStripButton10);
			this.m_pRemoteServers_Servers = new ListView();
			this.m_pRemoteServers_Servers.Size = new Size(475, 220);
			this.m_pRemoteServers_Servers.Location = new Point(8, 80);
			this.m_pRemoteServers_Servers.View = View.Details;
			this.m_pRemoteServers_Servers.FullRowSelect = true;
			this.m_pRemoteServers_Servers.HideSelection = false;
			this.m_pRemoteServers_Servers.DoubleClick += new EventHandler(this.m_pRemoteServers_Servers_DoubleClick);
			this.m_pRemoteServers_Servers.SelectedIndexChanged += new EventHandler(this.m_pRemoteServers_Servers_SelectedIndexChanged);
			this.m_pRemoteServers_Servers.Columns.Add("Server", 150, HorizontalAlignment.Left);
			this.m_pRemoteServers_Servers.Columns.Add("Description", 300, HorizontalAlignment.Left);
			this.m_pTab.TabPages[3].Controls.Add(this.m_pTab_RemoteServers_Icon);
			this.m_pTab.TabPages[3].Controls.Add(this.mt_Tab_RemoteServers_Info);
			this.m_pTab.TabPages[3].Controls.Add(this.m_pTab_RemoteServers_Separator1);
			this.m_pTab.TabPages[3].Controls.Add(this.m_pTab_RemoteServers_Toolbar);
			this.m_pTab.TabPages[3].Controls.Add(this.m_pRemoteServers_Servers);
			this.m_pTab_Permissions_Icon = new PictureBox();
			this.m_pTab_Permissions_Icon.Size = new Size(32, 32);
			this.m_pTab_Permissions_Icon.Location = new Point(10, 10);
			this.m_pTab_Permissions_Icon.Image = ResManager.GetIcon("security32.ico").ToBitmap();
			this.mt_Tab_Permissions_Info = new Label();
			this.mt_Tab_Permissions_Info.Size = new Size(200, 32);
			this.mt_Tab_Permissions_Info.Location = new Point(50, 10);
			this.mt_Tab_Permissions_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Tab_Permissions_Info.Text = "Specify user permissions.";
			this.m_pTab_Permissions_Separator1 = new GroupBox();
			this.m_pTab_Permissions_Separator1.Size = new Size(475, 3);
			this.m_pTab_Permissions_Separator1.Location = new Point(7, 50);
			this.m_pPermissions_AllowPop3 = new CheckBox();
			this.m_pPermissions_AllowPop3.Size = new Size(200, 20);
			this.m_pPermissions_AllowPop3.Location = new Point(20, 60);
			this.m_pPermissions_AllowPop3.Text = "Allow POP3";
			this.m_pPermissions_AllowPop3.Checked = true;
			this.m_pPermissions_AllowImap = new CheckBox();
			this.m_pPermissions_AllowImap.Size = new Size(200, 20);
			this.m_pPermissions_AllowImap.Location = new Point(20, 80);
			this.m_pPermissions_AllowImap.Text = "Allow IMAP";
			this.m_pPermissions_AllowImap.Checked = true;
			this.m_pPermissions_AllowRelay = new CheckBox();
			this.m_pPermissions_AllowRelay.Size = new Size(200, 20);
			this.m_pPermissions_AllowRelay.Location = new Point(20, 100);
			this.m_pPermissions_AllowRelay.Text = "Allow Relay";
			this.m_pPermissions_AllowRelay.Checked = true;
			this.m_pPermissions_AllowSIP = new CheckBox();
			this.m_pPermissions_AllowSIP.Size = new Size(200, 20);
			this.m_pPermissions_AllowSIP.Location = new Point(20, 120);
			this.m_pPermissions_AllowSIP.Text = "Allow SIP";
			this.m_pPermissions_AllowSIP.Checked = true;
			this.mt_Permissions_SipGwAccess = new Label();
			this.mt_Permissions_SipGwAccess.Size = new Size(200, 20);
			this.mt_Permissions_SipGwAccess.Location = new Point(15, 150);
			this.mt_Permissions_SipGwAccess.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Permissions_SipGwAccess.Text = "SIP Gateway access:";
			this.m_pPermissions_SipGwAccessToolbar = new ToolStrip();
			this.m_pPermissions_SipGwAccessToolbar.Size = new Size(75, 25);
			this.m_pPermissions_SipGwAccessToolbar.Location = new Point(400, 145);
			this.m_pPermissions_SipGwAccessToolbar.Dock = DockStyle.None;
			this.m_pPermissions_SipGwAccessToolbar.GripStyle = ToolStripGripStyle.Hidden;
			this.m_pPermissions_SipGwAccessToolbar.BackColor = this.BackColor;
			this.m_pPermissions_SipGwAccessToolbar.Renderer = new ToolBarRendererEx();
			ToolStripButton toolStripButton11 = new ToolStripButton();
			toolStripButton11.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripButton11.Tag = "add";
			this.m_pPermissions_SipGwAccessToolbar.Items.Add(toolStripButton11);
			ToolStripButton toolStripButton12 = new ToolStripButton();
			toolStripButton12.Enabled = false;
			toolStripButton12.Image = ResManager.GetIcon("edit.ico").ToBitmap();
			toolStripButton12.Tag = "edit";
			this.m_pPermissions_SipGwAccessToolbar.Items.Add(toolStripButton12);
			ToolStripButton toolStripButton13 = new ToolStripButton();
			toolStripButton13.Enabled = false;
			toolStripButton13.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			toolStripButton13.Tag = "delete";
			this.m_pPermissions_SipGwAccessToolbar.Items.Add(toolStripButton13);
			this.m_pPermissions_SipGwAccess = new ListView();
			this.m_pPermissions_SipGwAccess.Size = new Size(455, 125);
			this.m_pPermissions_SipGwAccess.Location = new Point(15, 170);
			this.m_pPermissions_SipGwAccess.View = View.Details;
			this.m_pPermissions_SipGwAccess.HideSelection = false;
			this.m_pPermissions_SipGwAccess.FullRowSelect = true;
			this.m_pPermissions_SipGwAccess.Columns.Add("URI", 60);
			this.m_pPermissions_SipGwAccess.Columns.Add("Access pattern", 360);
			this.m_pTab.TabPages[4].Controls.Add(this.m_pTab_Permissions_Icon);
			this.m_pTab.TabPages[4].Controls.Add(this.mt_Tab_Permissions_Info);
			this.m_pTab.TabPages[4].Controls.Add(this.m_pTab_Permissions_Separator1);
			this.m_pTab.TabPages[4].Controls.Add(this.m_pPermissions_AllowPop3);
			this.m_pTab.TabPages[4].Controls.Add(this.m_pPermissions_AllowImap);
			this.m_pTab.TabPages[4].Controls.Add(this.m_pPermissions_AllowRelay);
			this.m_pTab.TabPages[4].Controls.Add(this.m_pPermissions_AllowSIP);
			this.m_pTab.TabPages[4].Controls.Add(this.mt_Permissions_SipGwAccess);
			this.m_pTab.TabPages[4].Controls.Add(this.m_pPermissions_SipGwAccessToolbar);
			this.m_pTab.TabPages[4].Controls.Add(this.m_pPermissions_SipGwAccess);
			this.m_pTab_Folders_Icon = new PictureBox();
			this.m_pTab_Folders_Icon.Size = new Size(32, 32);
			this.m_pTab_Folders_Icon.Location = new Point(10, 10);
			this.m_pTab_Folders_Icon.Image = ResManager.GetIcon("folder32.ico").ToBitmap();
			this.mt_Tab_Folders_Info = new Label();
			this.mt_Tab_Folders_Info.Size = new Size(200, 32);
			this.mt_Tab_Folders_Info.Location = new Point(50, 10);
			this.mt_Tab_Folders_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Tab_Folders_Info.Text = "Manage user folders.";
			this.m_pTab_Folders_Separator1 = new GroupBox();
			this.m_pTab_Folders_Separator1.Size = new Size(475, 3);
			this.m_pTab_Folders_Separator1.Location = new Point(7, 50);
			this.m_pTab_Folders_Toolbar = new ToolStrip();
			this.m_pTab_Folders_Toolbar.Size = new Size(180, 25);
			this.m_pTab_Folders_Toolbar.Location = new Point(305, 55);
			this.m_pTab_Folders_Toolbar.Dock = DockStyle.None;
			this.m_pTab_Folders_Toolbar.GripStyle = ToolStripGripStyle.Hidden;
			this.m_pTab_Folders_Toolbar.BackColor = this.BackColor;
			this.m_pTab_Folders_Toolbar.Renderer = new ToolBarRendererEx();
			this.m_pTab_Folders_Toolbar.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pTab_Folders_Toolbar_ItemClicked);
			ToolStripButton toolStripButton14 = new ToolStripButton();
			toolStripButton14.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripButton14.Tag = "add";
			toolStripButton14.ToolTipText = "Create folder";
			this.m_pTab_Folders_Toolbar.Items.Add(toolStripButton14);
			ToolStripButton toolStripButton15 = new ToolStripButton();
			toolStripButton15.Enabled = false;
			toolStripButton15.Image = ResManager.GetIcon("edit.ico").ToBitmap();
			toolStripButton15.Tag = "edit";
			toolStripButton15.ToolTipText = "Rename folder";
			this.m_pTab_Folders_Toolbar.Items.Add(toolStripButton15);
			ToolStripButton toolStripButton16 = new ToolStripButton();
			toolStripButton16.Enabled = false;
			toolStripButton16.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			toolStripButton16.Tag = "delete";
			toolStripButton16.ToolTipText = "Delete folder";
			this.m_pTab_Folders_Toolbar.Items.Add(toolStripButton16);
			this.m_pTab_Folders_Toolbar.Items.Add(new ToolStripSeparator());
			ToolStripButton toolStripButton17 = new ToolStripButton();
			toolStripButton17.Enabled = false;
			toolStripButton17.Image = ResManager.GetIcon("properties.ico").ToBitmap();
			toolStripButton17.Tag = "properties";
			toolStripButton17.ToolTipText = "Folder properties";
			this.m_pTab_Folders_Toolbar.Items.Add(toolStripButton17);
			this.m_pTab_Folders_Toolbar.Items.Add(new ToolStripSeparator());
			ToolStripButton toolStripButton18 = new ToolStripButton();
			toolStripButton18.Enabled = false;
			toolStripButton18.Image = ResManager.GetIcon("viewmessages.ico").ToBitmap();
			toolStripButton18.Tag = "viewmessages";
			toolStripButton18.ToolTipText = "View folder messages";
			this.m_pTab_Folders_Toolbar.Items.Add(toolStripButton18);
			ToolStripButton toolStripButton19 = new ToolStripButton();
			toolStripButton19.Enabled = true;
			toolStripButton19.Image = ResManager.GetIcon("recyclebin16.ico").ToBitmap();
			toolStripButton19.Tag = "recyclebin";
			toolStripButton19.ToolTipText = "Recycle bin";
			this.m_pTab_Folders_Toolbar.Items.Add(toolStripButton19);
			ToolStripButton toolStripButton20 = new ToolStripButton();
			toolStripButton20.Enabled = true;
			toolStripButton20.Image = ResManager.GetIcon("ruleaction.ico").ToBitmap();
			toolStripButton20.Tag = "transfer";
			toolStripButton20.ToolTipText = "Import / Export messages";
			this.m_pTab_Folders_Toolbar.Items.Add(toolStripButton20);
			ImageList imageList = new ImageList();
			imageList.Images.Add(ResManager.GetIcon("folder32.ico"));
			imageList.Images.Add(ResManager.GetIcon("share32.ico"));
			this.m_pTab_Folders_Folders = new TreeView();
			this.m_pTab_Folders_Folders.Size = new Size(475, 220);
			this.m_pTab_Folders_Folders.Location = new Point(5, 80);
			this.m_pTab_Folders_Folders.HideSelection = false;
			this.m_pTab_Folders_Folders.FullRowSelect = true;
			this.m_pTab_Folders_Folders.PathSeparator = "/";
			this.m_pTab_Folders_Folders.ImageList = imageList;
			this.m_pTab_Folders_Folders.AfterSelect += new TreeViewEventHandler(this.m_pTab_Folders_Folders_AfterSelect);
			this.m_pTabPage_Folders.Controls.Add(this.m_pTab_Folders_Icon);
			this.m_pTabPage_Folders.Controls.Add(this.mt_Tab_Folders_Info);
			this.m_pTabPage_Folders.Controls.Add(this.m_pTab_Folders_Separator1);
			this.m_pTabPage_Folders.Controls.Add(this.m_pTab_Folders_Toolbar);
			this.m_pTabPage_Folders.Controls.Add(this.m_pTab_Folders_Folders);
		}

		private void m_pGeneral_GeneratePwd_Click(object sender, EventArgs e)
		{
			this.m_pGeneral_Password.Text = Guid.NewGuid().ToString().Substring(0, 8);
			this.m_pGeneral_Password.PasswordChar = '\0';
		}

		private void m_pTab_General_Create_Click(object sender, EventArgs e)
		{
			if (this.m_pGeneral_LoginName.Text.Length == 0)
			{
				MessageBox.Show("Login Name can't be empty !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			UserPermissions userPermissions_enum = UserPermissions.None;
			if (this.m_pPermissions_AllowPop3.Checked)
			{
				userPermissions_enum |= UserPermissions.POP3;
			}
			if (this.m_pPermissions_AllowImap.Checked)
			{
				userPermissions_enum |= UserPermissions.IMAP;
			}
			if (this.m_pPermissions_AllowRelay.Checked)
			{
				userPermissions_enum |= UserPermissions.Relay;
			}
			this.m_pUser = this.m_pVirtualServer.Users.Add(this.m_pGeneral_LoginName.Text, this.m_pGeneral_FullName.Text, this.m_pGeneral_Password.Text, this.m_pGeneral_Description.Text, (int)this.m_pGeneral_MaxMailboxSize.Value, this.m_pGeneral_Enabled.Checked, userPermissions_enum);
			this.m_pTab.TabPages.Add(this.m_pTabPage_Addressing);
			this.m_pTab.TabPages.Add(this.m_pTabPage_Rules);
			this.m_pTab.TabPages.Add(this.m_pTabPage_RemoteServers);
			this.m_pTab.TabPages.Add(this.m_pTabPage_Permissions);
			this.m_pTab.TabPages.Add(this.m_pTabPage_Folders);
			this.m_pTab_General_Create.Visible = false;
			this.LoadFolders("");
		}

		private void m_pTab_Addressing_Toolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			if (!(e.ClickedItem.Tag.ToString() == "add"))
			{
				if (e.ClickedItem.Tag.ToString() == "delete" && this.m_pTab_Addressing_Addresses.SelectedItems.Count > 0 && MessageBox.Show(this, "Are you sure you want to delete email address '" + this.m_pTab_Addressing_Addresses.SelectedItems[0].Text + "' !", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					this.m_pUser.EmailAddresses.Remove(this.m_pTab_Addressing_Addresses.SelectedItems[0].Text);
					this.m_pTab_Addressing_Addresses.SelectedItems[0].Remove();
				}
				return;
			}
			string text = this.m_pTab_Addressing_LocalPart.Text + "@" + this.m_pTab_Addressing_Domain.Text;
			if (this.m_pTab_Addressing_LocalPart.Text.Length == 0)
			{
				MessageBox.Show("Emails address can't be empty!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (this.m_pTab_Addressing_Domain.Text.Length == 0)
			{
				MessageBox.Show("Domain must be selected !!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			UserPermissions userPermissions_enum = UserPermissions.None;
			if (this.m_pPermissions_AllowPop3.Checked)
			{
				userPermissions_enum |= UserPermissions.POP3;
			}
			if (this.m_pPermissions_AllowImap.Checked)
			{
				userPermissions_enum |= UserPermissions.IMAP;
			}
			if (this.m_pPermissions_AllowRelay.Checked)
			{
				userPermissions_enum |= UserPermissions.Relay;
			}
			this.m_pUser.EmailAddresses.Add(text);
			ListViewItem value = new ListViewItem(text);
			this.m_pTab_Addressing_Addresses.Items.Add(value);
			this.m_pTab_Addressing_LocalPart.Text = "";
			this.m_pTab_Addressing_LocalPart.Focus();
		}

		private void m_pTab_Addressing_Addresses_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pTab_Addressing_Addresses.SelectedItems.Count > 0)
			{
				this.m_pTab_Addressing_Toolbar.Items[1].Enabled = true;
				return;
			}
			this.m_pTab_Addressing_Toolbar.Items[1].Enabled = false;
		}

		private void m_pTab_Rules_Toolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			if (e.ClickedItem.Tag.ToString() == "add")
			{
				AddEditUserMessageRuleForm addEditUserMessageRuleForm = new AddEditUserMessageRuleForm(this.m_pUser);
				if (addEditUserMessageRuleForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadRules(addEditUserMessageRuleForm.RuleID);
					return;
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "edit")
			{
				UserMessageRule userMessageRule = (UserMessageRule)this.m_pRules_Rules.SelectedItems[0].Tag;
				AddEditUserMessageRuleForm addEditUserMessageRuleForm2 = new AddEditUserMessageRuleForm(this.m_pUser, userMessageRule);
				if (addEditUserMessageRuleForm2.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadRules(userMessageRule.ID);
					return;
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "delete")
			{
				UserMessageRule userMessageRule2 = (UserMessageRule)this.m_pRules_Rules.SelectedItems[0].Tag;
				if (MessageBox.Show(this, "Are you sure you want to delete Rule '" + userMessageRule2.Description + "' !", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					userMessageRule2.Owner.Remove(userMessageRule2);
					this.LoadRules("");
					return;
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "up")
			{
				if (this.m_pRules_Rules.SelectedItems.Count > 0 && this.m_pRules_Rules.SelectedItems[0].Index > 0)
				{
					this.SwapRules(this.m_pRules_Rules.SelectedItems[0], this.m_pRules_Rules.Items[this.m_pRules_Rules.SelectedItems[0].Index - 1]);
					return;
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "down" && this.m_pRules_Rules.SelectedItems.Count > 0 && this.m_pRules_Rules.SelectedItems[0].Index < this.m_pRules_Rules.Items.Count - 1)
			{
				this.SwapRules(this.m_pRules_Rules.SelectedItems[0], this.m_pRules_Rules.Items[this.m_pRules_Rules.SelectedItems[0].Index + 1]);
			}
		}

		private void m_pRules_Rules_DoubleClick(object sender, EventArgs e)
		{
			if (this.m_pRules_Rules.SelectedItems.Count > 0)
			{
				UserMessageRule userMessageRule = (UserMessageRule)this.m_pRules_Rules.SelectedItems[0].Tag;
				AddEditUserMessageRuleForm addEditUserMessageRuleForm = new AddEditUserMessageRuleForm(this.m_pUser, userMessageRule);
				if (addEditUserMessageRuleForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadRules(userMessageRule.ID);
				}
			}
		}

		private void m_pRules_Rules_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pRules_Rules.SelectedItems.Count > 0)
			{
				this.m_pTab_Rules_Toolbar.Items[1].Enabled = true;
				this.m_pTab_Rules_Toolbar.Items[2].Enabled = true;
				if (this.m_pRules_Rules.SelectedItems[0].Index > 0)
				{
					this.m_pTab_Rules_Toolbar.Items[4].Enabled = true;
				}
				if (this.m_pRules_Rules.SelectedItems[0].Index < this.m_pRules_Rules.Items.Count - 1)
				{
					this.m_pTab_Rules_Toolbar.Items[5].Enabled = true;
					return;
				}
			}
			else
			{
				this.m_pTab_Rules_Toolbar.Items[1].Enabled = false;
				this.m_pTab_Rules_Toolbar.Items[2].Enabled = false;
				this.m_pTab_Rules_Toolbar.Items[4].Enabled = false;
				this.m_pTab_Rules_Toolbar.Items[5].Enabled = false;
			}
		}

		private void m_pTab_RemoteServers_Toolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			if (e.ClickedItem.Tag.ToString() == "add")
			{
				AddEditUserRemoteServerForm addEditUserRemoteServerForm = new AddEditUserRemoteServerForm(this.m_pUser);
				if (addEditUserRemoteServerForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadRemoteServers(addEditUserRemoteServerForm.RemoteServerID);
					return;
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "edit")
			{
				UserRemoteServer remoteServer = (UserRemoteServer)this.m_pRemoteServers_Servers.SelectedItems[0].Tag;
				AddEditUserRemoteServerForm addEditUserRemoteServerForm2 = new AddEditUserRemoteServerForm(this.m_pUser, remoteServer);
				if (addEditUserRemoteServerForm2.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadRemoteServers(addEditUserRemoteServerForm2.RemoteServerID);
					return;
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "delete")
			{
				UserRemoteServer userRemoteServer = (UserRemoteServer)this.m_pRemoteServers_Servers.SelectedItems[0].Tag;
				if (MessageBox.Show(this, "Are you sure you want to delete user Remote server '" + userRemoteServer.Host + "' !", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					userRemoteServer.Owner.Remove(userRemoteServer);
					this.LoadRemoteServers("");
				}
			}
		}

		private void m_pRemoteServers_Servers_DoubleClick(object sender, EventArgs e)
		{
			if (this.m_pRemoteServers_Servers.SelectedItems.Count > 0)
			{
				UserRemoteServer remoteServer = (UserRemoteServer)this.m_pRemoteServers_Servers.SelectedItems[0].Tag;
				AddEditUserRemoteServerForm addEditUserRemoteServerForm = new AddEditUserRemoteServerForm(this.m_pUser, remoteServer);
				if (addEditUserRemoteServerForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadRemoteServers(addEditUserRemoteServerForm.RemoteServerID);
				}
			}
		}

		private void m_pRemoteServers_Servers_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pRemoteServers_Servers.Items.Count > 0 && this.m_pRemoteServers_Servers.SelectedItems.Count > 0)
			{
				this.m_pTab_RemoteServers_Toolbar.Items[1].Enabled = true;
				this.m_pTab_RemoteServers_Toolbar.Items[2].Enabled = true;
				return;
			}
			this.m_pTab_RemoteServers_Toolbar.Items[1].Enabled = false;
			this.m_pTab_RemoteServers_Toolbar.Items[2].Enabled = false;
		}

		private void m_pTab_Folders_Toolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			if (e.ClickedItem.Tag.ToString() == "add")
			{
				if (this.m_pTab_Folders_Folders.SelectedNode == null)
				{
					AddEditFolderForm addEditFolderForm = new AddEditFolderForm(true, "", false);
					if (addEditFolderForm.ShowDialog(this) == DialogResult.OK)
					{
						this.m_pUser.Folders.Add(addEditFolderForm.Folder);
						this.LoadFolders(addEditFolderForm.Folder);
						return;
					}
				}
				else
				{
					AddEditFolderForm addEditFolderForm2 = new AddEditFolderForm(true, "", false);
					if (addEditFolderForm2.ShowDialog(this) == DialogResult.OK)
					{
						UserFolder userFolder = (UserFolder)this.m_pTab_Folders_Folders.SelectedNode.Tag;
						userFolder.ChildFolders.Add(addEditFolderForm2.Folder);
						this.LoadFolders(addEditFolderForm2.Folder);
						return;
					}
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "edit" && this.m_pTab_Folders_Folders.SelectedNode != null)
			{
				AddEditFolderForm addEditFolderForm3 = new AddEditFolderForm(false, this.m_pTab_Folders_Folders.SelectedNode.FullPath, true);
				if (addEditFolderForm3.ShowDialog(this) == DialogResult.OK && this.m_pTab_Folders_Folders.SelectedNode.FullPath != addEditFolderForm3.Folder)
				{
					UserFolder userFolder2 = (UserFolder)this.m_pTab_Folders_Folders.SelectedNode.Tag;
					userFolder2.Rename(addEditFolderForm3.Folder);
					this.LoadFolders(addEditFolderForm3.Folder);
					return;
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "delete" && this.m_pTab_Folders_Folders.SelectedNode != null)
			{
				UserFolder userFolder3 = (UserFolder)this.m_pTab_Folders_Folders.SelectedNode.Tag;
				if (MessageBox.Show(this, "Are you sure you want to delete Folder '" + userFolder3.FolderFullPath + "' ?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					userFolder3.Owner.Remove(userFolder3);
					this.LoadFolders("");
					return;
				}
			}
			else
			{
				if (e.ClickedItem.Tag.ToString() == "properties" && this.m_pTab_Folders_Folders.SelectedNode != null)
				{
					UserFolder folder = (UserFolder)this.m_pTab_Folders_Folders.SelectedNode.Tag;
					FolderPropertiesForm folderPropertiesForm = new FolderPropertiesForm(this.m_pVirtualServer, folder);
					folderPropertiesForm.ShowDialog(this);
					return;
				}
				if (e.ClickedItem.Tag.ToString() == "viewmessages" && this.m_pTab_Folders_Folders.SelectedNode != null)
				{
					UserFolder folder2 = (UserFolder)this.m_pTab_Folders_Folders.SelectedNode.Tag;
					FolderMessagesForm folderMessagesForm = new FolderMessagesForm(this.m_pVirtualServer, folder2);
					folderMessagesForm.ShowDialog(this);
					return;
				}
				if (e.ClickedItem.Tag.ToString() == "recyclebin")
				{
                    UserForms.RecycleBinForm recycleBinForm = new UserForms.RecycleBinForm(this.m_pVirtualServer, this.m_pUser);
					recycleBinForm.ShowDialog(this);
					return;
				}
				if (e.ClickedItem.Tag.ToString() == "transfer")
				{
					TransferMessagesForm transferMessagesForm = new TransferMessagesForm(this.m_pUser);
					transferMessagesForm.ShowDialog();
				}
			}
		}

		private void m_pTab_Folders_Folders_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node != null)
			{
				this.m_pTab_Folders_Toolbar.Items[1].Enabled = true;
				this.m_pTab_Folders_Toolbar.Items[2].Enabled = true;
				this.m_pTab_Folders_Toolbar.Items[4].Enabled = true;
				this.m_pTab_Folders_Toolbar.Items[6].Enabled = true;
				return;
			}
			this.m_pTab_Folders_Toolbar.Items[1].Enabled = false;
			this.m_pTab_Folders_Toolbar.Items[2].Enabled = false;
			this.m_pTab_Folders_Toolbar.Items[4].Enabled = false;
			this.m_pTab_Folders_Toolbar.Items[6].Enabled = false;
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			if (keyData == Keys.Escape)
			{
				this.m_Cancel_Click(null, null);
			}
			return base.ProcessDialogKey(keyData);
		}

		private void m_Cancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
			base.Close();
		}

		private void m_pOk_Click(object sender, EventArgs e)
		{
			if (this.m_pGeneral_LoginName.Text.Length == 0)
			{
				MessageBox.Show("Login Name can't be empty!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			UserPermissions userPermissions_enum = UserPermissions.None;
			if (this.m_pPermissions_AllowPop3.Checked)
			{
				userPermissions_enum |= UserPermissions.POP3;
			}
			if (this.m_pPermissions_AllowImap.Checked)
			{
				userPermissions_enum |= UserPermissions.IMAP;
			}
			if (this.m_pPermissions_AllowRelay.Checked)
			{
				userPermissions_enum |= UserPermissions.Relay;
			}
			if (this.m_pPermissions_AllowSIP.Checked)
			{
				userPermissions_enum |= UserPermissions.SIP;
			}
			if (this.m_pUser == null)
			{
				this.m_pUser = this.m_pVirtualServer.Users.Add(this.m_pGeneral_LoginName.Text, this.m_pGeneral_FullName.Text, this.m_pGeneral_Password.Text, this.m_pGeneral_Description.Text, (int)this.m_pGeneral_MaxMailboxSize.Value, this.m_pGeneral_Enabled.Checked, userPermissions_enum);
			}
			else
			{
				this.m_pUser.UserName = this.m_pGeneral_LoginName.Text;
				this.m_pUser.Password = this.m_pGeneral_Password.Text;
				this.m_pUser.FullName = this.m_pGeneral_FullName.Text;
				this.m_pUser.Description = this.m_pGeneral_Description.Text;
				this.m_pUser.MaximumMailboxSize = (int)this.m_pGeneral_MaxMailboxSize.Value;
				this.m_pUser.Enabled = this.m_pGeneral_Enabled.Checked;
				this.m_pUser.Permissions = userPermissions_enum;
				this.m_pUser.Commit();
			}
			base.DialogResult = DialogResult.OK;
			base.Close();
		}

		private void LoadSettings()
		{
			this.m_pGeneral_Enabled.Checked = this.m_pUser.Enabled;
			this.m_pGeneral_FullName.Text = this.m_pUser.FullName;
			this.m_pGeneral_Description.Text = this.m_pUser.Description;
			this.m_pGeneral_LoginName.Text = this.m_pUser.UserName;
			this.m_pGeneral_Password.Text = this.m_pUser.Password;
			this.m_pGeneral_Password.PasswordChar = '*';
			this.m_pGeneral_MaxMailboxSize.Value = this.m_pUser.MaximumMailboxSize;
			long mailboxSize = this.m_pUser.MailboxSize;
			this.m_pTab_General_MailboxSize.Text = Convert.ToDecimal(this.m_pUser.MailboxSize / 1000000m).ToString("f2") + " MB";
			if (this.m_pGeneral_MaxMailboxSize.Value > 0m)
			{
				Label expr_107 = this.m_pTab_General_MailboxSize;
				expr_107.Text = expr_107.Text + " of " + this.m_pGeneral_MaxMailboxSize.Value.ToString() + " MB";
			}
			this.m_pGeneral_MailboxSizeIndicator.Maximum = this.m_pUser.MaximumMailboxSize;
			if (mailboxSize / 1000000L < (long)this.m_pGeneral_MailboxSizeIndicator.Maximum)
			{
				this.m_pGeneral_MailboxSizeIndicator.Value = (int)(mailboxSize / 1000000L);
			}
			else
			{
				this.m_pGeneral_MailboxSizeIndicator.Value = this.m_pGeneral_MailboxSizeIndicator.Maximum;
			}
			DateTime creationTime = this.m_pUser.CreationTime;
			this.m_pTab_General_Created.Text = creationTime.ToLongDateString() + " " + creationTime.ToLongTimeString();
			DateTime lastLogin = this.m_pUser.LastLogin;
			this.m_pTab_General_LastLogin.Text = lastLogin.ToLongDateString() + " " + lastLogin.ToLongTimeString();
			foreach (string text in this.m_pUser.EmailAddresses)
			{
				ListViewItem value = new ListViewItem(text);
				this.m_pTab_Addressing_Addresses.Items.Add(value);
			}
			this.m_pPermissions_AllowPop3.Checked = ((this.m_pUser.Permissions & UserPermissions.POP3) != UserPermissions.None);
			this.m_pPermissions_AllowImap.Checked = ((this.m_pUser.Permissions & UserPermissions.IMAP) != UserPermissions.None);
			this.m_pPermissions_AllowRelay.Checked = ((this.m_pUser.Permissions & UserPermissions.Relay) != UserPermissions.None);
			this.m_pPermissions_AllowSIP.Checked = ((this.m_pUser.Permissions & UserPermissions.SIP) != UserPermissions.None);
		}

		private void LoadRules(string selectedRuleID)
		{
			this.m_pRules_Rules.Items.Clear();
			foreach (UserMessageRule userMessageRule in this.m_pUser.MessageRules)
			{
				ListViewItem listViewItem = new ListViewItem();
				if (!userMessageRule.Enabled)
				{
					listViewItem.ForeColor = Color.Purple;
					listViewItem.Font = new Font(listViewItem.Font.FontFamily, listViewItem.Font.Size, FontStyle.Strikeout);
					listViewItem.ImageIndex = 1;
				}
				else
				{
					listViewItem.ImageIndex = 0;
				}
				listViewItem.Tag = userMessageRule;
				listViewItem.Text = userMessageRule.Description;
				this.m_pRules_Rules.Items.Add(listViewItem);
				if (userMessageRule.ID == selectedRuleID)
				{
					listViewItem.Selected = true;
				}
			}
			this.m_pRules_Rules_SelectedIndexChanged(this, new EventArgs());
		}

		private void LoadRemoteServers(string selectedServerID)
		{
			this.m_pRemoteServers_Servers.Items.Clear();
			foreach (UserRemoteServer userRemoteServer in this.m_pUser.RemoteServers)
			{
				ListViewItem listViewItem = new ListViewItem();
				if (userRemoteServer.Enabled)
				{
					listViewItem.ImageIndex = 0;
				}
				else
				{
					listViewItem.ForeColor = Color.Purple;
					listViewItem.Font = new Font(listViewItem.Font.FontFamily, listViewItem.Font.Size, FontStyle.Strikeout);
					listViewItem.ImageIndex = 1;
				}
				listViewItem.Tag = userRemoteServer;
				listViewItem.Text = userRemoteServer.Host;
				listViewItem.SubItems.Add(userRemoteServer.Description);
				this.m_pRemoteServers_Servers.Items.Add(listViewItem);
				if (userRemoteServer.ID == selectedServerID)
				{
					listViewItem.Selected = true;
				}
			}
			this.m_pRemoteServers_Servers_SelectedIndexChanged(this, new EventArgs());
		}

		private void LoadDomains()
		{
			foreach (Domain domain in this.m_pVirtualServer.Domains)
			{
				this.m_pTab_Addressing_Domain.Items.Add(domain.DomainName);
			}
			if (this.m_pTab_Addressing_Domain.Items.Count > 0)
			{
				this.m_pTab_Addressing_Domain.SelectedIndex = 0;
			}
		}

		private void LoadFolders(string selectedFolder)
		{
			this.m_pTab_Folders_Folders.Nodes.Clear();
			Queue<object> queue = new Queue<object>();
			IEnumerator enumerator = this.m_pUser.Folders.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					UserFolder userFolder = (UserFolder)enumerator.Current;
					TreeNode treeNode = new TreeNode(userFolder.FolderName);
					if (!this.IsFolderShared(userFolder.FolderName))
					{
						treeNode.ImageIndex = 0;
						treeNode.SelectedImageIndex = 0;
					}
					else
					{
						treeNode.ImageIndex = 1;
						treeNode.SelectedImageIndex = 1;
					}
					treeNode.Tag = userFolder;
					this.m_pTab_Folders_Folders.Nodes.Add(treeNode);
					queue.Enqueue(new object[]
					{
						userFolder,
						treeNode
					});
				}
				goto IL_19F;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			IL_C8:
			object[] array = (object[])queue.Dequeue();
			UserFolder userFolder2 = (UserFolder)array[0];
			TreeNode treeNode2 = (TreeNode)array[1];
			foreach (UserFolder userFolder3 in userFolder2.ChildFolders)
			{
				TreeNode treeNode3 = new TreeNode(userFolder3.FolderName);
				if (!this.IsFolderShared(userFolder3.FolderName))
				{
					treeNode3.ImageIndex = 0;
					treeNode3.SelectedImageIndex = 0;
				}
				else
				{
					treeNode3.ImageIndex = 1;
					treeNode3.SelectedImageIndex = 1;
				}
				treeNode3.Tag = userFolder3;
				treeNode2.Nodes.Add(treeNode3);
				queue.Enqueue(new object[]
				{
					userFolder3,
					treeNode3
				});
			}
			IL_19F:
			if (queue.Count <= 0)
			{
				this.m_pTab_Folders_Folders_AfterSelect(this, new TreeViewEventArgs(this.m_pTab_Folders_Folders.SelectedNode));
				return;
			}
			goto IL_C8;
		}

		private bool IsFolderShared(string folder)
		{
			foreach (SharedRootFolder sharedRootFolder in this.m_pVirtualServer.RootFolders)
			{
				if (sharedRootFolder.Type == SharedFolderRootType.BoundedRootFolder && sharedRootFolder.BoundedUser == this.m_pUser.UserName && sharedRootFolder.BoundedFolder == folder)
				{
					return true;
				}
			}
			return false;
		}

		private void SwapRules(ListViewItem item1, ListViewItem item2)
		{
			UserMessageRule userMessageRule = (UserMessageRule)item1.Tag;
			UserMessageRule userMessageRule2 = (UserMessageRule)item2.Tag;
			string selectedRuleID = "";
			if (item1.Selected)
			{
				selectedRuleID = userMessageRule.ID;
			}
			else if (item2.Selected)
			{
				selectedRuleID = userMessageRule2.ID;
			}
			long cost = userMessageRule2.Cost;
			userMessageRule2.Cost = userMessageRule.Cost;
			userMessageRule2.Commit();
			userMessageRule.Cost = cost;
			userMessageRule.Commit();
			this.m_pUser.MessageRules.Refresh();
			this.LoadRules(selectedRuleID);
		}
	}
}
