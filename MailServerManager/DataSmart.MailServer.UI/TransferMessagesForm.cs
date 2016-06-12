using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System.NetworkToolkit;
using System.NetworkToolkit.IMAP;
using System.NetworkToolkit.IMAP.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class TransferMessagesForm : Form
	{
		private delegate void StartNewFolderDelegate(string folderName, int messagesCount);

		private delegate void AddErrorDelegate(Exception x);

		private PictureBox m_pIcon;

		private Label m_pTitle;

		private GroupBox m_pSeparator1;

		private GroupBox m_pSeparator2;

		private Button m_pBack;

		private Button m_pNext;

		private Button m_pCancel;

		private Panel m_pSource;

		private Label mt_Source_Type;

		private ComboBox m_pSource_Type;

		private Label mt_Source_TypeLSUser_User;

		private TextBox m_pSource_TypeLSUser_User;

		private Button m_pSource_TypeLSUser_UserGet;

		private Label mt_Source_TypeIMAP_Host;

		private TextBox m_pSource_TypeIMAP_Host;

		private NumericUpDown m_pSource_TypeIMAP_Port;

		private CheckBox m_pSource_TypeIMAP_UseSSL;

		private Label mt_Source_TypeIMAP_User;

		private TextBox m_pSource_TypeIMAP_User;

		private Label mt_Source_TypeIMAP_Password;

		private TextBox m_pSource_TypeIMAP_Password;

		private Label mt_Source_TypeZIP_File;

		private TextBox m_pSource_TypeZIP_File;

		private Button m_pSource_TypeZIP_FileGet;

		private Panel m_pFolders;

		private Button m_pFolders_SelectAll;

		private WTreeView m_pFolders_Folders;

		private Panel m_pDestination;

		private Label mt_Destination_Type;

		private ComboBox m_pDestination_Type;

		private Label mt_Destination_TypeLSUser_User;

		private TextBox m_pDestination_TypeLSUser_User;

		private Button m_pDestination_TypeLSUser_UserGet;

		private Label mt_Destination_TypeIMAP_Host;

		private TextBox m_pDestination_TypeIMAP_Host;

		private NumericUpDown m_pDestination_TypeIMAP_Port;

		private CheckBox m_pDestination_TypeIMAP_UseSSL;

		private Label mt_Destination_TypeIMAP_User;

		private TextBox m_pDestination_TypeIMAP_User;

		private Label mt_Destination_TypeIMAP_Password;

		private TextBox m_pDestination_TypeIMAP_Password;

		private Label mt_Destination_TypeZIP_File;

		private TextBox m_pDestination_TypeZIP_File;

		private Button m_pDestination_TypeZIP_FileGet;

		private Panel m_pFinish;

		private ProgressBar m_pFinish_Progress;

		private ListView m_pFinish_Completed;

		private MailServer.Management.User m_pUser;

		private string m_Step = "source";

		private int m_SourceType = -1;

		private object m_pSourceObject;

		private int m_DestinationType = -1;

		private object m_pDestinationObject;

		public TransferMessagesForm(MailServer.Management.User user)
		{
			this.m_pUser = user;
			this.InitializeComponent();
			this.SwitchStep("source");
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(500, 400);
			base.StartPosition = FormStartPosition.CenterScreen;
			base.SizeGripStyle = SizeGripStyle.Hide;
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			this.Text = "Transfer Messages";
			base.Icon = ResManager.GetIcon("ruleaction.ico");
			base.FormClosed += new FormClosedEventHandler(this.wfrm_utils_MessagesTransferer_FormClosed);
			this.m_pIcon = new PictureBox();
			this.m_pIcon.Size = new Size(36, 36);
			this.m_pIcon.Location = new Point(10, 5);
			this.m_pIcon.Image = ResManager.GetIcon("ruleaction.ico").ToBitmap();
			this.m_pTitle = new Label();
			this.m_pTitle.Size = new Size(300, 30);
			this.m_pTitle.Location = new Point(50, 10);
			this.m_pTitle.TextAlign = ContentAlignment.MiddleLeft;
			this.m_pSeparator1 = new GroupBox();
			this.m_pSeparator1.Size = new Size(490, 3);
			this.m_pSeparator1.Location = new Point(5, 44);
			this.m_pSeparator2 = new GroupBox();
			this.m_pSeparator2.Size = new Size(490, 3);
			this.m_pSeparator2.Location = new Point(5, 360);
			this.m_pBack = new Button();
			this.m_pBack.Size = new Size(70, 20);
			this.m_pBack.Location = new Point(265, 375);
			this.m_pBack.Text = "Back";
			this.m_pBack.Click += new EventHandler(this.m_pBack_Click);
			this.m_pNext = new Button();
			this.m_pNext.Size = new Size(70, 20);
			this.m_pNext.Location = new Point(340, 375);
			this.m_pNext.Text = "Next";
			this.m_pNext.Click += new EventHandler(this.m_pNext_Click);
			this.m_pCancel = new Button();
			this.m_pCancel.Size = new Size(70, 20);
			this.m_pCancel.Location = new Point(420, 375);
			this.m_pCancel.Text = "Cancel";
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			base.Controls.Add(this.m_pIcon);
			base.Controls.Add(this.m_pTitle);
			base.Controls.Add(this.m_pSeparator1);
			base.Controls.Add(this.m_pSeparator2);
			base.Controls.Add(this.m_pBack);
			base.Controls.Add(this.m_pNext);
			base.Controls.Add(this.m_pCancel);
			this.m_pSource = new Panel();
			this.m_pSource.Size = new Size(500, 300);
			this.m_pSource.Location = new Point(0, 75);
			this.m_pSource.Visible = false;
			this.mt_Source_Type = new Label();
			this.mt_Source_Type.Size = new Size(100, 20);
			this.mt_Source_Type.Location = new Point(0, 0);
			this.mt_Source_Type.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Source_Type.Text = "Source:";
			this.m_pSource_Type = new ComboBox();
			this.m_pSource_Type.Size = new Size(300, 20);
			this.m_pSource_Type.Location = new Point(105, 0);
			this.m_pSource_Type.DropDownStyle = ComboBoxStyle.DropDownList;
			this.m_pSource_Type.Items.Add("DataSmart Mail Server User");
			this.m_pSource_Type.Items.Add("IMAP");
			this.m_pSource_Type.Items.Add("ZIP Messages Archive");
			this.m_pSource_Type.SelectedIndexChanged += new EventHandler(this.m_pSource_Type_SelectedIndexChanged);
			this.mt_Source_TypeLSUser_User = new Label();
			this.mt_Source_TypeLSUser_User.Size = new Size(100, 20);
			this.mt_Source_TypeLSUser_User.Location = new Point(0, 25);
			this.mt_Source_TypeLSUser_User.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Source_TypeLSUser_User.Text = "User:";
			this.mt_Source_TypeLSUser_User.Visible = false;
			this.m_pSource_TypeLSUser_User = new TextBox();
			this.m_pSource_TypeLSUser_User.Size = new Size(270, 20);
			this.m_pSource_TypeLSUser_User.Location = new Point(105, 25);
			this.m_pSource_TypeLSUser_User.ReadOnly = true;
			this.m_pSource_TypeLSUser_User.Visible = false;
			this.m_pSource_TypeLSUser_UserGet = new Button();
			this.m_pSource_TypeLSUser_UserGet.Size = new Size(25, 20);
			this.m_pSource_TypeLSUser_UserGet.Location = new Point(380, 25);
			this.m_pSource_TypeLSUser_UserGet.Text = "...";
			this.m_pSource_TypeLSUser_UserGet.Visible = false;
			this.m_pSource_TypeLSUser_UserGet.Click += new EventHandler(this.m_pSource_TypeLSUser_UserGet_Click);
			this.mt_Source_TypeIMAP_Host = new Label();
			this.mt_Source_TypeIMAP_Host.Size = new Size(100, 20);
			this.mt_Source_TypeIMAP_Host.Location = new Point(0, 25);
			this.mt_Source_TypeIMAP_Host.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Source_TypeIMAP_Host.Text = "Host:";
			this.mt_Source_TypeIMAP_Host.Visible = false;
			this.m_pSource_TypeIMAP_Host = new TextBox();
			this.m_pSource_TypeIMAP_Host.Size = new Size(150, 20);
			this.m_pSource_TypeIMAP_Host.Location = new Point(105, 25);
			this.m_pSource_TypeIMAP_Host.Visible = false;
			this.m_pSource_TypeIMAP_Port = new NumericUpDown();
			this.m_pSource_TypeIMAP_Port.Size = new Size(45, 20);
			this.m_pSource_TypeIMAP_Port.Location = new Point(260, 25);
			this.m_pSource_TypeIMAP_Port.Minimum = 1m;
			this.m_pSource_TypeIMAP_Port.Maximum = 99999m;
			this.m_pSource_TypeIMAP_Port.Value = 143m;
			this.m_pSource_TypeIMAP_Port.Visible = false;
			this.m_pSource_TypeIMAP_UseSSL = new CheckBox();
			this.m_pSource_TypeIMAP_UseSSL.Size = new Size(200, 20);
			this.m_pSource_TypeIMAP_UseSSL.Location = new Point(315, 25);
			this.m_pSource_TypeIMAP_UseSSL.Text = "Use SSL";
			this.m_pSource_TypeIMAP_UseSSL.Visible = false;
			this.mt_Source_TypeIMAP_User = new Label();
			this.mt_Source_TypeIMAP_User.Size = new Size(100, 20);
			this.mt_Source_TypeIMAP_User.Location = new Point(0, 50);
			this.mt_Source_TypeIMAP_User.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Source_TypeIMAP_User.Text = "User:";
			this.mt_Source_TypeIMAP_User.Visible = false;
			this.m_pSource_TypeIMAP_User = new TextBox();
			this.m_pSource_TypeIMAP_User.Size = new Size(150, 20);
			this.m_pSource_TypeIMAP_User.Location = new Point(105, 50);
			this.m_pSource_TypeIMAP_User.Visible = false;
			this.mt_Source_TypeIMAP_Password = new Label();
			this.mt_Source_TypeIMAP_Password.Size = new Size(100, 20);
			this.mt_Source_TypeIMAP_Password.Location = new Point(0, 75);
			this.mt_Source_TypeIMAP_Password.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Source_TypeIMAP_Password.Text = "Password:";
			this.mt_Source_TypeIMAP_Password.Visible = false;
			this.m_pSource_TypeIMAP_Password = new TextBox();
			this.m_pSource_TypeIMAP_Password.Size = new Size(150, 20);
			this.m_pSource_TypeIMAP_Password.Location = new Point(105, 75);
			this.m_pSource_TypeIMAP_Password.PasswordChar = '*';
			this.m_pSource_TypeIMAP_Password.Visible = false;
			this.mt_Source_TypeZIP_File = new Label();
			this.mt_Source_TypeZIP_File.Size = new Size(100, 20);
			this.mt_Source_TypeZIP_File.Location = new Point(0, 25);
			this.mt_Source_TypeZIP_File.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Source_TypeZIP_File.Text = "Zip File:";
			this.mt_Source_TypeZIP_File.Visible = false;
			this.m_pSource_TypeZIP_File = new TextBox();
			this.m_pSource_TypeZIP_File.Size = new Size(270, 20);
			this.m_pSource_TypeZIP_File.Location = new Point(105, 25);
			this.m_pSource_TypeZIP_File.ReadOnly = true;
			this.m_pSource_TypeZIP_File.Visible = false;
			this.m_pSource_TypeZIP_FileGet = new Button();
			this.m_pSource_TypeZIP_FileGet.Size = new Size(25, 20);
			this.m_pSource_TypeZIP_FileGet.Location = new Point(380, 25);
			this.m_pSource_TypeZIP_FileGet.Text = "...";
			this.m_pSource_TypeZIP_FileGet.Visible = false;
			this.m_pSource_TypeZIP_FileGet.Click += new EventHandler(this.m_pSource_TypeZIP_FileGet_Click);
			this.m_pSource.Controls.Add(this.mt_Source_Type);
			this.m_pSource.Controls.Add(this.m_pSource_Type);
			this.m_pSource.Controls.Add(this.mt_Source_TypeLSUser_User);
			this.m_pSource.Controls.Add(this.m_pSource_TypeLSUser_User);
			this.m_pSource.Controls.Add(this.m_pSource_TypeLSUser_UserGet);
			this.m_pSource.Controls.Add(this.mt_Source_TypeIMAP_Host);
			this.m_pSource.Controls.Add(this.m_pSource_TypeIMAP_Host);
			this.m_pSource.Controls.Add(this.m_pSource_TypeIMAP_Port);
			this.m_pSource.Controls.Add(this.m_pSource_TypeIMAP_UseSSL);
			this.m_pSource.Controls.Add(this.mt_Source_TypeIMAP_User);
			this.m_pSource.Controls.Add(this.m_pSource_TypeIMAP_User);
			this.m_pSource.Controls.Add(this.mt_Source_TypeIMAP_Password);
			this.m_pSource.Controls.Add(this.m_pSource_TypeIMAP_Password);
			this.m_pSource.Controls.Add(this.mt_Source_TypeZIP_File);
			this.m_pSource.Controls.Add(this.m_pSource_TypeZIP_File);
			this.m_pSource.Controls.Add(this.m_pSource_TypeZIP_FileGet);
			base.Controls.Add(this.m_pSource);
			this.m_pFolders = new Panel();
			this.m_pFolders.Size = new Size(500, 300);
			this.m_pFolders.Location = new Point(0, 50);
			this.m_pFolders.Visible = false;
			this.m_pFolders_SelectAll = new Button();
			this.m_pFolders_SelectAll.Size = new Size(70, 20);
			this.m_pFolders_SelectAll.Location = new Point(10, 0);
			this.m_pFolders_SelectAll.Text = "Select All";
			this.m_pFolders_SelectAll.Click += new EventHandler(this.m_pFolders_SelectAll_Click);
			ImageList imageList = new ImageList();
			imageList.Images.Add(ResManager.GetIcon("folder.ico"));
			this.m_pFolders_Folders = new WTreeView();
			this.m_pFolders_Folders.Size = new Size(480, 265);
			this.m_pFolders_Folders.Location = new Point(10, 25);
			this.m_pFolders_Folders.CheckBoxes = true;
			this.m_pFolders_Folders.ImageList = imageList;
			this.m_pFolders.Controls.Add(this.m_pFolders_SelectAll);
			this.m_pFolders.Controls.Add(this.m_pFolders_Folders);
			base.Controls.Add(this.m_pFolders);
			this.m_pDestination = new Panel();
			this.m_pDestination.Size = new Size(500, 300);
			this.m_pDestination.Location = new Point(0, 50);
			this.m_pDestination.Visible = false;
			this.mt_Destination_Type = new Label();
			this.mt_Destination_Type.Size = new Size(100, 20);
			this.mt_Destination_Type.Location = new Point(0, 25);
			this.mt_Destination_Type.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Destination_Type.Text = "Destination:";
			this.m_pDestination_Type = new ComboBox();
			this.m_pDestination_Type.Size = new Size(300, 20);
			this.m_pDestination_Type.Location = new Point(105, 25);
			this.m_pDestination_Type.DropDownStyle = ComboBoxStyle.DropDownList;
			this.m_pDestination_Type.Items.Add("DataSmart Mail Server User");
			this.m_pDestination_Type.Items.Add("IMAP");
			this.m_pDestination_Type.Items.Add("ZIP Messages Archive");
			this.m_pDestination_Type.SelectedIndexChanged += new EventHandler(this.m_pDestination_Type_SelectedIndexChanged);
			this.mt_Destination_TypeLSUser_User = new Label();
			this.mt_Destination_TypeLSUser_User.Size = new Size(100, 20);
			this.mt_Destination_TypeLSUser_User.Location = new Point(0, 50);
			this.mt_Destination_TypeLSUser_User.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Destination_TypeLSUser_User.Text = "User:";
			this.mt_Destination_TypeLSUser_User.Visible = false;
			this.m_pDestination_TypeLSUser_User = new TextBox();
			this.m_pDestination_TypeLSUser_User.Size = new Size(270, 20);
			this.m_pDestination_TypeLSUser_User.Location = new Point(105, 50);
			this.m_pDestination_TypeLSUser_User.ReadOnly = true;
			this.m_pDestination_TypeLSUser_User.Visible = false;
			this.m_pDestination_TypeLSUser_UserGet = new Button();
			this.m_pDestination_TypeLSUser_UserGet.Size = new Size(25, 20);
			this.m_pDestination_TypeLSUser_UserGet.Location = new Point(380, 50);
			this.m_pDestination_TypeLSUser_UserGet.Text = "...";
			this.m_pDestination_TypeLSUser_UserGet.Visible = false;
			this.m_pDestination_TypeLSUser_UserGet.Click += new EventHandler(this.m_pDestination_TypeLSUser_UserGet_Click);
			this.mt_Destination_TypeIMAP_Host = new Label();
			this.mt_Destination_TypeIMAP_Host.Size = new Size(100, 20);
			this.mt_Destination_TypeIMAP_Host.Location = new Point(0, 50);
			this.mt_Destination_TypeIMAP_Host.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Destination_TypeIMAP_Host.Text = "Host:";
			this.mt_Destination_TypeIMAP_Host.Visible = false;
			this.m_pDestination_TypeIMAP_Host = new TextBox();
			this.m_pDestination_TypeIMAP_Host.Size = new Size(150, 20);
			this.m_pDestination_TypeIMAP_Host.Location = new Point(105, 50);
			this.m_pDestination_TypeIMAP_Host.Visible = false;
			this.m_pDestination_TypeIMAP_Port = new NumericUpDown();
			this.m_pDestination_TypeIMAP_Port.Size = new Size(45, 20);
			this.m_pDestination_TypeIMAP_Port.Location = new Point(260, 50);
			this.m_pDestination_TypeIMAP_Port.Minimum = 1m;
			this.m_pDestination_TypeIMAP_Port.Maximum = 99999m;
			this.m_pDestination_TypeIMAP_Port.Value = 143m;
			this.m_pDestination_TypeIMAP_Port.Visible = false;
			this.m_pDestination_TypeIMAP_UseSSL = new CheckBox();
			this.m_pDestination_TypeIMAP_UseSSL.Size = new Size(200, 20);
			this.m_pDestination_TypeIMAP_UseSSL.Location = new Point(315, 50);
			this.m_pDestination_TypeIMAP_UseSSL.Text = "Use SSL";
			this.m_pDestination_TypeIMAP_UseSSL.Visible = false;
			this.mt_Destination_TypeIMAP_User = new Label();
			this.mt_Destination_TypeIMAP_User.Size = new Size(100, 20);
			this.mt_Destination_TypeIMAP_User.Location = new Point(0, 75);
			this.mt_Destination_TypeIMAP_User.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Destination_TypeIMAP_User.Text = "User:";
			this.mt_Destination_TypeIMAP_User.Visible = false;
			this.m_pDestination_TypeIMAP_User = new TextBox();
			this.m_pDestination_TypeIMAP_User.Size = new Size(150, 20);
			this.m_pDestination_TypeIMAP_User.Location = new Point(105, 75);
			this.m_pDestination_TypeIMAP_User.Visible = false;
			this.mt_Destination_TypeIMAP_Password = new Label();
			this.mt_Destination_TypeIMAP_Password.Size = new Size(100, 20);
			this.mt_Destination_TypeIMAP_Password.Location = new Point(0, 100);
			this.mt_Destination_TypeIMAP_Password.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Destination_TypeIMAP_Password.Text = "Password:";
			this.mt_Destination_TypeIMAP_Password.Visible = false;
			this.m_pDestination_TypeIMAP_Password = new TextBox();
			this.m_pDestination_TypeIMAP_Password.Size = new Size(150, 20);
			this.m_pDestination_TypeIMAP_Password.Location = new Point(105, 100);
			this.m_pDestination_TypeIMAP_Password.PasswordChar = '*';
			this.m_pDestination_TypeIMAP_Password.Visible = false;
			this.mt_Destination_TypeZIP_File = new Label();
			this.mt_Destination_TypeZIP_File.Size = new Size(100, 20);
			this.mt_Destination_TypeZIP_File.Location = new Point(0, 50);
			this.mt_Destination_TypeZIP_File.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Destination_TypeZIP_File.Text = "Zip File:";
			this.mt_Destination_TypeZIP_File.Visible = false;
			this.m_pDestination_TypeZIP_File = new TextBox();
			this.m_pDestination_TypeZIP_File.Size = new Size(270, 20);
			this.m_pDestination_TypeZIP_File.Location = new Point(105, 50);
			this.m_pDestination_TypeZIP_File.ReadOnly = true;
			this.m_pDestination_TypeZIP_File.Visible = false;
			this.m_pDestination_TypeZIP_FileGet = new Button();
			this.m_pDestination_TypeZIP_FileGet.Size = new Size(25, 20);
			this.m_pDestination_TypeZIP_FileGet.Location = new Point(380, 50);
			this.m_pDestination_TypeZIP_FileGet.Text = "...";
			this.m_pDestination_TypeZIP_FileGet.Visible = false;
			this.m_pDestination_TypeZIP_FileGet.Click += new EventHandler(this.m_pDestination_TypeZIP_FileGet_Click);
			this.m_pDestination.Controls.Add(this.mt_Destination_Type);
			this.m_pDestination.Controls.Add(this.m_pDestination_Type);
			this.m_pDestination.Controls.Add(this.mt_Destination_TypeLSUser_User);
			this.m_pDestination.Controls.Add(this.m_pDestination_TypeLSUser_User);
			this.m_pDestination.Controls.Add(this.m_pDestination_TypeLSUser_UserGet);
			this.m_pDestination.Controls.Add(this.mt_Destination_TypeIMAP_Host);
			this.m_pDestination.Controls.Add(this.m_pDestination_TypeIMAP_Host);
			this.m_pDestination.Controls.Add(this.m_pDestination_TypeIMAP_Port);
			this.m_pDestination.Controls.Add(this.m_pDestination_TypeIMAP_UseSSL);
			this.m_pDestination.Controls.Add(this.mt_Destination_TypeIMAP_User);
			this.m_pDestination.Controls.Add(this.m_pDestination_TypeIMAP_User);
			this.m_pDestination.Controls.Add(this.mt_Destination_TypeIMAP_Password);
			this.m_pDestination.Controls.Add(this.m_pDestination_TypeIMAP_Password);
			this.m_pDestination.Controls.Add(this.mt_Destination_TypeZIP_File);
			this.m_pDestination.Controls.Add(this.m_pDestination_TypeZIP_File);
			this.m_pDestination.Controls.Add(this.m_pDestination_TypeZIP_FileGet);
			base.Controls.Add(this.m_pDestination);
			this.m_pFinish = new Panel();
			this.m_pFinish.Size = new Size(500, 300);
			this.m_pFinish.Location = new Point(0, 50);
			this.m_pFinish.Visible = false;
			this.m_pFinish_Progress = new ProgressBar();
			this.m_pFinish_Progress.Size = new Size(480, 20);
			this.m_pFinish_Progress.Location = new Point(10, 0);
			ImageList imageList2 = new ImageList();
			imageList2.Images.Add(ResManager.GetIcon("folder.ico"));
			this.m_pFinish_Completed = new ListView();
			this.m_pFinish_Completed.Size = new Size(480, 275);
			this.m_pFinish_Completed.Location = new Point(10, 25);
			this.m_pFinish_Completed.View = View.Details;
			this.m_pFinish_Completed.FullRowSelect = true;
			this.m_pFinish_Completed.HideSelection = false;
			this.m_pFinish_Completed.SmallImageList = imageList2;
			this.m_pFinish_Completed.Columns.Add("Folder", 340);
			this.m_pFinish_Completed.Columns.Add("Count", 70);
			this.m_pFinish_Completed.Columns.Add("Errors", 50);
			this.m_pFinish_Completed.DoubleClick += new EventHandler(this.m_pFinish_Completed_DoubleClick);
			this.m_pFinish.Controls.Add(this.m_pFinish_Progress);
			this.m_pFinish.Controls.Add(this.m_pFinish_Completed);
			base.Controls.Add(this.m_pFinish);
		}

		private void m_pSource_Type_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.mt_Source_TypeLSUser_User.Visible = false;
			this.m_pSource_TypeLSUser_User.Visible = false;
			this.m_pSource_TypeLSUser_UserGet.Visible = false;
			this.mt_Source_TypeIMAP_Host.Visible = false;
			this.m_pSource_TypeIMAP_Host.Visible = false;
			this.m_pSource_TypeIMAP_Port.Visible = false;
			this.m_pSource_TypeIMAP_UseSSL.Visible = false;
			this.mt_Source_TypeIMAP_User.Visible = false;
			this.m_pSource_TypeIMAP_User.Visible = false;
			this.mt_Source_TypeIMAP_Password.Visible = false;
			this.m_pSource_TypeIMAP_Password.Visible = false;
			this.mt_Source_TypeZIP_File.Visible = false;
			this.m_pSource_TypeZIP_File.Visible = false;
			this.m_pSource_TypeZIP_FileGet.Visible = false;
			if (this.m_pSource_Type.SelectedIndex == 0)
			{
				this.mt_Source_TypeLSUser_User.Visible = true;
				this.m_pSource_TypeLSUser_User.Visible = true;
				this.m_pSource_TypeLSUser_UserGet.Visible = true;
				return;
			}
			if (this.m_pSource_Type.SelectedIndex == 1)
			{
				this.mt_Source_TypeIMAP_Host.Visible = true;
				this.m_pSource_TypeIMAP_Host.Visible = true;
				this.m_pSource_TypeIMAP_Port.Visible = true;
				this.m_pSource_TypeIMAP_UseSSL.Visible = true;
				this.mt_Source_TypeIMAP_User.Visible = true;
				this.m_pSource_TypeIMAP_User.Visible = true;
				this.mt_Source_TypeIMAP_Password.Visible = true;
				this.m_pSource_TypeIMAP_Password.Visible = true;
				return;
			}
			if (this.m_pSource_Type.SelectedIndex == 2)
			{
				this.mt_Source_TypeZIP_File.Visible = true;
				this.m_pSource_TypeZIP_File.Visible = true;
				this.m_pSource_TypeZIP_FileGet.Visible = true;
			}
		}

		private void m_pSource_TypeLSUser_UserGet_Click(object sender, EventArgs e)
		{
			SelectUserOrGroupForm selectUserOrGroupForm = new SelectUserOrGroupForm(this.m_pUser.VirtualServer, false, false);
			if (selectUserOrGroupForm.ShowDialog(this) == DialogResult.OK)
			{
				this.m_pSource_TypeLSUser_User.Text = selectUserOrGroupForm.SelectedUserOrGroup;
			}
		}

		private void m_pSource_TypeZIP_FileGet_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Zipped Email Archive (*.zip)|*.zip";
			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				this.m_pSource_TypeZIP_File.Text = openFileDialog.FileName;
			}
		}

		private void m_pFolders_SelectAll_Click(object sender, EventArgs e)
		{
			foreach (TreeNode treeNode in this.m_pFolders_Folders.Nodes)
			{
				treeNode.Checked = true;
			}
		}

		private void m_pDestination_Type_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.mt_Destination_TypeLSUser_User.Visible = false;
			this.m_pDestination_TypeLSUser_User.Visible = false;
			this.m_pDestination_TypeLSUser_UserGet.Visible = false;
			this.mt_Destination_TypeIMAP_Host.Visible = false;
			this.m_pDestination_TypeIMAP_Host.Visible = false;
			this.m_pDestination_TypeIMAP_Port.Visible = false;
			this.m_pDestination_TypeIMAP_UseSSL.Visible = false;
			this.mt_Destination_TypeIMAP_User.Visible = false;
			this.m_pDestination_TypeIMAP_User.Visible = false;
			this.mt_Destination_TypeIMAP_Password.Visible = false;
			this.m_pDestination_TypeIMAP_Password.Visible = false;
			this.mt_Destination_TypeZIP_File.Visible = false;
			this.m_pDestination_TypeZIP_File.Visible = false;
			this.m_pDestination_TypeZIP_FileGet.Visible = false;
			if (this.m_pDestination_Type.SelectedIndex == 0)
			{
				this.mt_Destination_TypeLSUser_User.Visible = true;
				this.m_pDestination_TypeLSUser_User.Visible = true;
				this.m_pDestination_TypeLSUser_UserGet.Visible = true;
				return;
			}
			if (this.m_pDestination_Type.SelectedIndex == 1)
			{
				this.mt_Destination_TypeIMAP_Host.Visible = true;
				this.m_pDestination_TypeIMAP_Host.Visible = true;
				this.m_pDestination_TypeIMAP_Port.Visible = true;
				this.m_pDestination_TypeIMAP_UseSSL.Visible = true;
				this.mt_Destination_TypeIMAP_User.Visible = true;
				this.m_pDestination_TypeIMAP_User.Visible = true;
				this.mt_Destination_TypeIMAP_Password.Visible = true;
				this.m_pDestination_TypeIMAP_Password.Visible = true;
				return;
			}
			if (this.m_pDestination_Type.SelectedIndex == 2)
			{
				this.mt_Destination_TypeZIP_File.Visible = true;
				this.m_pDestination_TypeZIP_File.Visible = true;
				this.m_pDestination_TypeZIP_FileGet.Visible = true;
			}
		}

		private void m_pDestination_TypeLSUser_UserGet_Click(object sender, EventArgs e)
		{
			SelectUserOrGroupForm selectUserOrGroupForm = new SelectUserOrGroupForm(this.m_pUser.VirtualServer, false, false);
			if (selectUserOrGroupForm.ShowDialog(this) == DialogResult.OK)
			{
				this.m_pDestination_TypeLSUser_User.Text = selectUserOrGroupForm.SelectedUserOrGroup;
			}
		}

		private void m_pDestination_TypeZIP_FileGet_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Zipped Email Archive (*.zip)|*.zip";
			if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				this.m_pDestination_TypeZIP_File.Text = saveFileDialog.FileName;
			}
		}

		private void m_pFinish_Completed_DoubleClick(object sender, EventArgs e)
		{
			if (this.m_pFinish_Completed.SelectedItems.Count > 0)
			{
				List<Exception> list = (List<Exception>)this.m_pFinish_Completed.SelectedItems[0].Tag;
				Form form = new Form();
				form.Size = new Size(400, 300);
				form.StartPosition = FormStartPosition.CenterScreen;
				form.Text = "Folder: '" + this.m_pFinish_Completed.SelectedItems[0].Text + "' Errors:";
				form.Icon = ResManager.GetIcon("error.ico");
				TextBox textBox = new TextBox();
				textBox.Dock = DockStyle.Fill;
				textBox.Multiline = true;
				StringBuilder stringBuilder = new StringBuilder();
				foreach (Exception current in list)
				{
					stringBuilder.Append(current.Message + "\n\n");
				}
				textBox.Lines = stringBuilder.ToString().Split(new char[]
				{
					'\n'
				});
				textBox.SelectionStart = 0;
				textBox.SelectionLength = 0;
				form.Controls.Add(textBox);
				form.Show();
			}
		}

		private void wfrm_utils_MessagesTransferer_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.DisposeSource();
			this.DisposeDestination();
		}

		private void m_pBack_Click(object sender, EventArgs e)
		{
			if (this.m_Step == "folders")
			{
				this.SwitchStep("source");
				return;
			}
			if (this.m_Step == "destination")
			{
				this.SwitchStep("folders");
				return;
			}
			if (this.m_Step == "finish")
			{
				this.SwitchStep("destination");
			}
		}

		private void m_pNext_Click(object sender, EventArgs e)
		{
			if (this.m_Step == "source")
			{
				try
				{
					if (this.m_pSource_Type.SelectedIndex == -1)
					{
						MessageBox.Show(this, "Please select messages source !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						return;
					}
					this.DisposeSource();
					this.m_SourceType = this.m_pSource_Type.SelectedIndex;
					this.InitSource();
					this.m_pFolders_Folders.Nodes.Clear();
					string[] sourceFolders = this.GetSourceFolders();
					string[] array = sourceFolders;
					for (int i = 0; i < array.Length; i++)
					{
						string text = array[i];
						TreeNodeCollection nodes = this.m_pFolders_Folders.Nodes;
						string[] array2 = text.Split(new char[]
						{
							'/',
							'\\'
						});
						string[] array3 = array2;
						for (int j = 0; j < array3.Length; j++)
						{
							string text2 = array3[j];
							bool flag = false;
							foreach (TreeNode treeNode in nodes)
							{
								if (treeNode.Text == text2)
								{
									nodes = treeNode.Nodes;
									flag = true;
								}
							}
							if (!flag)
							{
								nodes.Add(new TreeNode(text2)
								{
									ImageIndex = 0,
									Tag = text
								});
							}
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(this, "Error: " + ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				this.SwitchStep("folders");
				return;
			}
			if (this.m_Step == "folders")
			{
				this.SwitchStep("destination");
				return;
			}
			if (!(this.m_Step == "destination"))
			{
				if (this.m_Step == "finish")
				{
					this.m_pTitle.Text = "Transfering messages ...";
					this.m_pBack.Enabled = false;
					this.m_pNext.Enabled = false;
					Thread thread = new Thread(new ThreadStart(this.Start));
                    thread.Name = "Message Transfer Thread";
					thread.Start();
				}
				return;
			}
			if (this.m_pDestination_Type.SelectedIndex != -1)
			{
				this.DisposeDestination();
				this.m_DestinationType = this.m_pDestination_Type.SelectedIndex;
				this.InitDestination();
				this.SwitchStep("finish");
				return;
			}
			MessageBox.Show(this, "Please select messages destination !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void SwitchStep(string step)
		{
			if (step == "source")
			{
				this.m_pTitle.Text = "Please select messages source.";
				this.m_pSource.Visible = true;
				this.m_pFolders.Visible = false;
				this.m_pDestination.Visible = false;
				this.m_pFinish.Visible = false;
				this.m_pBack.Enabled = false;
				this.m_pNext.Text = "Next";
			}
			else if (step == "folders")
			{
				this.m_pTitle.Text = "Please select folders which messages to transfer.";
				this.m_pSource.Visible = false;
				this.m_pFolders.Visible = true;
				this.m_pDestination.Visible = false;
				this.m_pFinish.Visible = false;
				this.m_pBack.Enabled = true;
				this.m_pNext.Text = "Next";
			}
			else if (step == "destination")
			{
				this.m_pTitle.Text = "Please select messages destination.";
				this.m_pSource.Visible = false;
				this.m_pFolders.Visible = false;
				this.m_pDestination.Visible = true;
				this.m_pFinish.Visible = false;
				this.m_pBack.Enabled = true;
				this.m_pNext.Text = "Next";
			}
			else if (step == "finish")
			{
				this.m_pTitle.Text = "Click start to begin messages transfer.";
				this.m_pSource.Visible = false;
				this.m_pFolders.Visible = false;
				this.m_pDestination.Visible = false;
				this.m_pFinish.Visible = true;
				this.m_pNext.Text = "Start";
			}
			this.m_Step = step;
		}

		private void InitSource()
		{
			if (this.m_SourceType == 0)
			{
				this.m_pSourceObject = this.m_pUser.VirtualServer.Users.GetUserByName(this.m_pSource_TypeLSUser_User.Text);
				return;
			}
			if (this.m_SourceType == 1)
			{
				IMAP_Client iMAP_Client = new IMAP_Client();
				iMAP_Client.Connect(this.m_pSource_TypeIMAP_Host.Text, (int)this.m_pSource_TypeIMAP_Port.Value, this.m_pSource_TypeIMAP_UseSSL.Checked);
				iMAP_Client.Login(this.m_pSource_TypeIMAP_User.Text, this.m_pSource_TypeIMAP_Password.Text);
				this.m_pSourceObject = iMAP_Client;
				return;
			}
			if (this.m_SourceType == 2)
			{
				this.m_pSourceObject = ZipFile.OpenRead(this.m_pSource_TypeZIP_File.Text);
				return;
			}
			throw new Exception("Invalid source type '" + this.m_SourceType.ToString() + "' !");
		}

		private void DisposeSource()
		{
			if (this.m_pSourceObject == null)
			{
				return;
			}
			if (this.m_pSourceObject is IMAP_Client)
			{
				((IMAP_Client)this.m_pSourceObject).Dispose();
			}
			else if (this.m_pSourceObject is ZipArchive)
			{
				((ZipArchive)this.m_pSourceObject).Dispose();
			}
			this.m_pSourceObject = null;
		}

		private void InitDestination()
		{
			if (this.m_DestinationType == 0)
			{
				this.m_pDestinationObject = this.m_pUser.VirtualServer.Users.GetUserByName(this.m_pDestination_TypeLSUser_User.Text);
				return;
			}
			if (this.m_DestinationType == 1)
			{
				IMAP_Client iMAP_Client = new IMAP_Client();
				iMAP_Client.Connect(this.m_pDestination_TypeIMAP_Host.Text, (int)this.m_pDestination_TypeIMAP_Port.Value, this.m_pDestination_TypeIMAP_UseSSL.Checked);
				iMAP_Client.Login(this.m_pDestination_TypeIMAP_User.Text, this.m_pDestination_TypeIMAP_Password.Text);
				this.m_pDestinationObject = iMAP_Client;
				return;
			}
			if (this.m_DestinationType == 2)
			{
				this.m_pDestinationObject = ZipFile.Open(this.m_pDestination_TypeZIP_File.Text, ZipArchiveMode.Create);
				return;
			}
			throw new Exception("Invalid destination type '" + this.m_DestinationType.ToString() + "' !");
		}

		private void DisposeDestination()
		{
			if (this.m_pDestinationObject == null)
			{
				return;
			}
			if (this.m_pDestinationObject is IMAP_Client)
			{
				((IMAP_Client)this.m_pDestinationObject).Dispose();
			}
			else if (this.m_pDestinationObject is ZipArchive)
			{
				((ZipArchive)this.m_pDestinationObject).Dispose();
			}
			this.m_pDestinationObject = null;
		}

		private string[] GetSourceFolders()
		{
			if (this.m_pSourceObject is User)
			{
				User user = (User)this.m_pSourceObject;
				List<string> list = new List<string>();
				Stack<Queue<UserFolder>> stack = new Stack<Queue<UserFolder>>();
				Queue<UserFolder> queue = new Queue<UserFolder>();
				foreach (UserFolder item in user.Folders)
				{
					queue.Enqueue(item);
				}
				stack.Push(queue);
				while (stack.Count > 0)
				{
					UserFolder userFolder = stack.Peek().Dequeue();
					if (stack.Peek().Count == 0)
					{
						stack.Pop();
					}
					list.Add(userFolder.FolderFullPath);
					if (userFolder.ChildFolders.Count > 0)
					{
						Queue<UserFolder> queue2 = new Queue<UserFolder>();
						foreach (UserFolder item2 in userFolder.ChildFolders)
						{
							queue2.Enqueue(item2);
						}
						stack.Push(queue2);
					}
				}
				return list.ToArray();
			}
			if (this.m_pSourceObject is IMAP_Client)
			{
				IMAP_Client iMAP_Client = (IMAP_Client)this.m_pSourceObject;
				List<string> list2 = new List<string>();
				try
				{
					IMAP_r_u_Namespace[] namespaces = iMAP_Client.GetNamespaces();
					for (int i = 0; i < namespaces.Length; i++)
					{
						IMAP_r_u_Namespace iMAP_r_u_Namespace = namespaces[i];
						if (iMAP_r_u_Namespace.OtherUsersNamespaces != null)
						{
							IMAP_Namespace_Entry[] otherUsersNamespaces = iMAP_r_u_Namespace.OtherUsersNamespaces;
							for (int j = 0; j < otherUsersNamespaces.Length; j++)
							{
								IMAP_Namespace_Entry iMAP_Namespace_Entry = otherUsersNamespaces[j];
								list2.Add(iMAP_Namespace_Entry.NamespaceName);
							}
						}
						if (iMAP_r_u_Namespace.SharedNamespaces != null)
						{
							IMAP_Namespace_Entry[] sharedNamespaces = iMAP_r_u_Namespace.SharedNamespaces;
							for (int k = 0; k < sharedNamespaces.Length; k++)
							{
								IMAP_Namespace_Entry iMAP_Namespace_Entry2 = sharedNamespaces[k];
								list2.Add(iMAP_Namespace_Entry2.NamespaceName);
							}
						}
					}
				}
				catch
				{
				}
				List<string> list3 = new List<string>();
				IMAP_r_u_List[] folders = iMAP_Client.GetFolders(null);
				for (int l = 0; l < folders.Length; l++)
				{
					IMAP_r_u_List iMAP_r_u_List = folders[l];
					bool flag = false;
					foreach (string current in list2)
					{
						if (iMAP_r_u_List.FolderName.ToLower().StartsWith(current.ToLower()))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						list3.Add(iMAP_r_u_List.FolderName);
					}
				}
				return list3.ToArray();
			}
			if (this.m_pSourceObject is ZipArchive)
			{
				ZipArchive zipArchive = (ZipArchive)this.m_pSourceObject;
				List<string> list4 = new List<string>();
				foreach (ZipArchiveEntry current2 in zipArchive.Entries)
				{
					string directoryName = Path.GetDirectoryName(current2.FullName);
					if (directoryName.Length > 0 && !list4.Contains(directoryName))
					{
						list4.Add(directoryName);
					}
				}
				return list4.ToArray();
			}
			throw new Exception("Invalid source");
		}

		private string[] GetSourceMessages(string folder)
		{
			if (this.m_pSource == null)
			{
				throw new Exception("Source not inited !");
			}
			if (this.m_pSourceObject is User)
			{
				User user = (User)this.m_pSourceObject;
				string[] array = folder.Split(new char[]
				{
					'/',
					'\\'
				});
				UserFolderCollection userFolderCollection = user.Folders;
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string folderName = array2[i];
					if (!userFolderCollection.Contains(folderName))
					{
						throw new Exception("Source folder '" + folder + "' doesn't exist !");
					}
					userFolderCollection = userFolderCollection[folderName].ChildFolders;
				}
				UserFolder userFolder = userFolderCollection.Parent;
				if (userFolder == null)
				{
					userFolder = this.m_pUser.Folders[folder];
				}
				List<string> list = new List<string>();
				DataSet messagesInfo = userFolder.GetMessagesInfo();
				if (messagesInfo.Tables.Contains("MessagesInfo"))
				{
					foreach (DataRow dataRow in messagesInfo.Tables["MessagesInfo"].Rows)
					{
						list.Add(dataRow["ID"].ToString());
					}
				}
				return list.ToArray();
			}
			if (this.m_pSourceObject is IMAP_Client)
			{
				IMAP_Client iMAP_Client = (IMAP_Client)this.m_pSourceObject;
				iMAP_Client.SelectFolder(folder);
				List<string> retVal = new List<string>();
				IMAP_Client_FetchHandler iMAP_Client_FetchHandler = new IMAP_Client_FetchHandler();
				iMAP_Client_FetchHandler.UID += delegate(object s, EventArgs<long> e)
				{
					retVal.Add(e.Value.ToString());
				};
				IMAP_SequenceSet iMAP_SequenceSet = new IMAP_SequenceSet();
				iMAP_SequenceSet.Parse("1:*");
				iMAP_Client.Fetch(false, iMAP_SequenceSet, new IMAP_Fetch_DataItem[]
				{
					new IMAP_Fetch_DataItem_Uid()
				}, iMAP_Client_FetchHandler);
				return retVal.ToArray();
			}
			if (this.m_pSourceObject is ZipArchive)
			{
				ZipArchive zipArchive = (ZipArchive)this.m_pSourceObject;
				List<string> list2 = new List<string>();
				foreach (ZipArchiveEntry current in zipArchive.Entries)
				{
					if (Path.GetDirectoryName(current.FullName).Replace("\\", "/").ToLower() == folder.Replace("\\", "/").ToLower() && current.FullName.EndsWith(".eml"))
					{
						list2.Add(current.FullName);
					}
				}
				return list2.ToArray();
			}
			throw new Exception("Invalid source, never should reach here !");
		}

		private void GetSourceMessage(string folder, string messageID, Stream storeStream)
		{
			if (this.m_pSource == null)
			{
				throw new Exception("Source not inited !");
			}
			if (this.m_pSourceObject is User)
			{
				User user = (User)this.m_pSourceObject;
				string[] array = folder.Split(new char[]
				{
					'/',
					'\\'
				});
				UserFolderCollection userFolderCollection = user.Folders;
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string folderName = array2[i];
					if (!userFolderCollection.Contains(folderName))
					{
						throw new Exception("Source folder '" + folder + "' doesn't exist !");
					}
					userFolderCollection = userFolderCollection[folderName].ChildFolders;
				}
				UserFolder userFolder = userFolderCollection.Parent;
				if (userFolder == null)
				{
					userFolder = this.m_pUser.Folders[folder];
				}
				userFolder.GetMessage(messageID, storeStream);
				return;
			}
			if (this.m_pSourceObject is IMAP_Client)
			{
				IMAP_Client iMAP_Client = (IMAP_Client)this.m_pSourceObject;
				new List<string>();
				IMAP_Client_FetchHandler iMAP_Client_FetchHandler = new IMAP_Client_FetchHandler();
				iMAP_Client_FetchHandler.Rfc822 += delegate(object s, IMAP_Client_Fetch_Rfc822_EArgs e)
				{
					e.Stream = storeStream;
					e.StoringCompleted += delegate(object s1, EventArgs e1)
					{
					};
				};
				IMAP_SequenceSet iMAP_SequenceSet = new IMAP_SequenceSet();
				iMAP_SequenceSet.Parse(messageID);
				iMAP_Client.Fetch(true, iMAP_SequenceSet, new IMAP_Fetch_DataItem[]
				{
					new IMAP_Fetch_DataItem_Rfc822()
				}, iMAP_Client_FetchHandler);
				return;
			}
			if (this.m_pSourceObject is ZipArchive)
			{
				ZipArchive zipArchive = (ZipArchive)this.m_pSourceObject;
				foreach (ZipArchiveEntry current in zipArchive.Entries)
				{
					if (current.FullName == messageID)
					{
						using (Stream stream = current.Open())
						{
							SCore.StreamCopy(stream, storeStream);
						}
						return;
					}
				}
				throw new Exception(string.Concat(new string[]
				{
					"Folder '",
					folder,
					"' message with ID '",
					messageID,
					"', not found !"
				}));
			}
		}

		private void StoreDestinationMessage(string folder, Stream messageStream)
		{
			if (this.m_pDestinationObject is User)
			{
				User user = (User)this.m_pDestinationObject;
				string[] array = folder.Split(new char[]
				{
					'/',
					'\\'
				});
				UserFolderCollection userFolderCollection = user.Folders;
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text = array2[i];
					if (!userFolderCollection.Contains(text))
					{
						userFolderCollection = userFolderCollection.Add(text).ChildFolders;
					}
					else
					{
						userFolderCollection = userFolderCollection[text].ChildFolders;
					}
				}
				UserFolder userFolder = userFolderCollection.Parent;
				if (userFolder == null)
				{
					userFolder = this.m_pUser.Folders[folder];
				}
				userFolder.StoreMessage(messageStream);
				return;
			}
			if (this.m_pDestinationObject is IMAP_Client)
			{
				IMAP_Client iMAP_Client = (IMAP_Client)this.m_pDestinationObject;
				try
				{
					iMAP_Client.CreateFolder(folder);
				}
				catch
				{
				}
				messageStream.Position = 0L;
				iMAP_Client.StoreMessage(folder, IMAP_MessageFlags.None, DateTime.MinValue, messageStream, (int)messageStream.Length);
				return;
			}
			if (this.m_pDestinationObject is ZipArchive)
			{
				ZipArchive zipArchive = (ZipArchive)this.m_pDestinationObject;
				ZipArchiveEntry zipArchiveEntry = zipArchive.CreateEntry(folder.Replace("/", "\\") + "\\" + Guid.NewGuid().ToString() + ".eml", CompressionLevel.Optimal);
				using (Stream stream = zipArchiveEntry.Open())
				{
					SCore.StreamCopy(messageStream, stream);
				}
			}
		}

		private void Start()
		{
			try
			{
				List<string> list = new List<string>();
				Stack<Queue<TreeNode>> stack = new Stack<Queue<TreeNode>>();
				Queue<TreeNode> queue = new Queue<TreeNode>();
				foreach (TreeNode item in this.m_pFolders_Folders.Nodes)
				{
					queue.Enqueue(item);
				}
				stack.Push(queue);
				while (stack.Count > 0)
				{
					if (stack.Peek().Count == 0)
					{
						stack.Pop();
					}
					else
					{
						TreeNode treeNode = stack.Peek().Dequeue();
						if (treeNode.Checked)
						{
							list.Add(treeNode.Tag.ToString());
						}
						if (treeNode.Nodes.Count > 0)
						{
							Queue<TreeNode> queue2 = new Queue<TreeNode>();
							foreach (TreeNode item2 in treeNode.Nodes)
							{
								queue2.Enqueue(item2);
							}
							stack.Push(queue2);
						}
					}
				}
				foreach (string current in list)
				{
					string[] sourceMessages = this.GetSourceMessages(current);
					base.Invoke(new TransferMessagesForm.StartNewFolderDelegate(this.StartNewFolder), new object[]
					{
						current,
						sourceMessages.Length
					});
					try
					{
						string[] array = sourceMessages;
						for (int i = 0; i < array.Length; i++)
						{
							string messageID = array[i];
							try
							{
								MemoryStream memoryStream = new MemoryStream();
								this.GetSourceMessage(current, messageID, memoryStream);
								memoryStream.Position = 0L;
								this.StoreDestinationMessage(current, memoryStream);
							}
							catch (Exception ex)
							{
								base.Invoke(new TransferMessagesForm.AddErrorDelegate(this.AddError), new object[]
								{
									ex
								});
							}
							base.Invoke(new MethodInvoker(this.IncreaseMessages));
						}
					}
					catch (Exception ex2)
					{
						base.Invoke(new TransferMessagesForm.AddErrorDelegate(this.AddError), new object[]
						{
							ex2
						});
					}
				}
			}
			finally
			{
				this.DisposeSource();
				this.DisposeDestination();
			}
			base.Invoke(new MethodInvoker(this.Finish));
		}

		private void StartNewFolder(string folderName, int messagesCount)
		{
			this.m_pFinish_Progress.Value = 0;
			this.m_pFinish_Progress.Maximum = messagesCount;
			ListViewItem listViewItem = new ListViewItem(folderName);
			listViewItem.ImageIndex = 0;
			listViewItem.Tag = new List<Exception>();
			listViewItem.SubItems.Add("0");
			listViewItem.SubItems.Add("0");
			this.m_pFinish_Completed.Items.Add(listViewItem);
		}

		private void IncreaseMessages()
		{
			this.m_pFinish_Progress.Value++;
			this.m_pFinish_Completed.Items[this.m_pFinish_Completed.Items.Count - 1].SubItems[1].Text = this.m_pFinish_Progress.Value.ToString() + "/" + this.m_pFinish_Progress.Maximum.ToString();
		}

		private void AddError(Exception x)
		{
			ListViewItem listViewItem = this.m_pFinish_Completed.Items[this.m_pFinish_Completed.Items.Count - 1];
			List<Exception> list = (List<Exception>)listViewItem.Tag;
			list.Add(x);
			listViewItem.SubItems[2].Text = list.Count.ToString();
		}

		private void Finish()
		{
			this.m_pTitle.Text = "Completed.";
			this.m_pFinish_Progress.Value = 0;
			this.m_pCancel.Text = "Finish";
		}
	}
}
