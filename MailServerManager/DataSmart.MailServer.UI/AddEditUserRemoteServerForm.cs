using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class AddEditUserRemoteServerForm : Form
	{
		private PictureBox m_pIcon;

		private Label mt_Info;

		private GroupBox m_pSeparator1;

		private CheckBox m_pEnabled;

		private Label mt_Description;

		private TextBox m_pDescription;

		private Label mt_Server;

		private TextBox m_pServer;

		private NumericUpDown m_pPort;

		private CheckBox m_UseSSL;

		private Label mt_User;

		private TextBox m_pUser;

		private Label mt_Password;

		private TextBox m_pPassword;

		private GroupBox m_pSeparator2;

		private Button m_pCancel;

		private Button m_pOk;

		private MailServer.Management.User m_pOwnerUser;

		private UserRemoteServer m_pRemoteServer;

		public string RemoteServerID
		{
			get
			{
				if (this.m_pRemoteServer != null)
				{
					return this.m_pRemoteServer.ID;
				}
				return "";
			}
		}

		public AddEditUserRemoteServerForm(MailServer.Management.User user)
		{
			this.m_pOwnerUser = user;
			this.InitializeComponent();
		}

		public AddEditUserRemoteServerForm(MailServer.Management.User user, UserRemoteServer remoteServer)
		{
			this.m_pRemoteServer = remoteServer;
			this.InitializeComponent();
			this.m_pDescription.Text = remoteServer.Description;
			this.m_pServer.Text = remoteServer.Host;
			this.m_pPort.Value = remoteServer.Port;
			this.m_pUser.Text = remoteServer.UserName;
			this.m_pPassword.Text = remoteServer.Password;
			this.m_UseSSL.Checked = remoteServer.SSL;
			this.m_pEnabled.Checked = remoteServer.Enabled;
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(392, 258);
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Add/Edit User Remote Server";
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = true;
			this.m_pIcon = new PictureBox();
			this.m_pIcon.Size = new Size(32, 32);
			this.m_pIcon.Location = new Point(10, 10);
			this.m_pIcon.Image = ResManager.GetIcon("remoteserver32.ico").ToBitmap();
			this.mt_Info = new Label();
			this.mt_Info.Size = new Size(200, 32);
			this.mt_Info.Location = new Point(50, 10);
			this.mt_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Info.Text = "Specify remote mail server information.";
			this.m_pSeparator1 = new GroupBox();
			this.m_pSeparator1.Size = new Size(383, 3);
			this.m_pSeparator1.Location = new Point(7, 50);
			this.m_pEnabled = new CheckBox();
			this.m_pEnabled.Size = new Size(200, 20);
			this.m_pEnabled.Location = new Point(105, 65);
			this.m_pEnabled.Checked = true;
			this.m_pEnabled.Text = "Enabled";
			this.mt_Description = new Label();
			this.mt_Description.Size = new Size(100, 20);
			this.mt_Description.Location = new Point(0, 90);
			this.mt_Description.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Description.Text = "Description:";
			this.m_pDescription = new TextBox();
			this.m_pDescription.Size = new Size(280, 20);
			this.m_pDescription.Location = new Point(105, 90);
			this.mt_Server = new Label();
			this.mt_Server.Size = new Size(100, 20);
			this.mt_Server.Location = new Point(0, 115);
			this.mt_Server.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Server.Text = "Server:";
			this.m_pServer = new TextBox();
			this.m_pServer.Size = new Size(215, 20);
			this.m_pServer.Location = new Point(105, 115);
			this.m_pPort = new NumericUpDown();
			this.m_pPort.Size = new Size(60, 20);
			this.m_pPort.Location = new Point(325, 115);
			this.m_pPort.Minimum = 1m;
			this.m_pPort.Maximum = 99999m;
			this.m_pPort.Value = 110m;
			this.m_UseSSL = new CheckBox();
			this.m_UseSSL.Size = new Size(200, 20);
			this.m_UseSSL.Location = new Point(105, 140);
			this.m_UseSSL.Text = "Connect via SSL";
			this.m_UseSSL.CheckedChanged += new EventHandler(this.m_UseSSL_CheckedChanged);
			this.mt_User = new Label();
			this.mt_User.Size = new Size(100, 20);
			this.mt_User.Location = new Point(0, 165);
			this.mt_User.TextAlign = ContentAlignment.MiddleRight;
			this.mt_User.Text = "User:";
			this.m_pUser = new TextBox();
			this.m_pUser.Size = new Size(280, 20);
			this.m_pUser.Location = new Point(105, 165);
			this.mt_Password = new Label();
			this.mt_Password.Size = new Size(100, 20);
			this.mt_Password.Location = new Point(0, 190);
			this.mt_Password.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Password.Text = "Password:";
			this.m_pPassword = new TextBox();
			this.m_pPassword.Size = new Size(280, 20);
			this.m_pPassword.Location = new Point(105, 190);
			this.m_pPassword.PasswordChar = '*';
			this.m_pSeparator2 = new GroupBox();
			this.m_pSeparator2.Size = new Size(383, 3);
			this.m_pSeparator2.Location = new Point(7, 225);
			this.m_pCancel = new Button();
			this.m_pCancel.Size = new Size(70, 20);
			this.m_pCancel.Location = new Point(240, 235);
			this.m_pCancel.Text = "Cancel";
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			this.m_pOk = new Button();
			this.m_pOk.Size = new Size(70, 20);
			this.m_pOk.Location = new Point(315, 235);
			this.m_pOk.Text = "Ok";
			this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
			base.Controls.Add(this.m_pIcon);
			base.Controls.Add(this.mt_Info);
			base.Controls.Add(this.m_pSeparator1);
			base.Controls.Add(this.m_pEnabled);
			base.Controls.Add(this.mt_Description);
			base.Controls.Add(this.m_pDescription);
			base.Controls.Add(this.mt_Server);
			base.Controls.Add(this.m_pServer);
			base.Controls.Add(this.m_pPort);
			base.Controls.Add(this.m_UseSSL);
			base.Controls.Add(this.mt_User);
			base.Controls.Add(this.m_pUser);
			base.Controls.Add(this.mt_Password);
			base.Controls.Add(this.m_pPassword);
			base.Controls.Add(this.m_pSeparator2);
			base.Controls.Add(this.m_pCancel);
			base.Controls.Add(this.m_pOk);
		}

		private void m_UseSSL_CheckedChanged(object sender, EventArgs e)
		{
			if (this.m_UseSSL.Checked)
			{
				if (this.m_pPort.Value == 110m)
				{
					this.m_pPort.Value = 995m;
					return;
				}
			}
			else if (this.m_pPort.Value == 995m)
			{
				this.m_pPort.Value = 110m;
			}
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
			base.Close();
		}

		private void m_pOk_Click(object sender, EventArgs e)
		{
			if (this.m_pServer.Text == "")
			{
				MessageBox.Show(this, "Please fill Server !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (this.m_pUser.Text == "")
			{
				MessageBox.Show(this, "Please fill User !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (this.m_pRemoteServer == null)
			{
				this.m_pRemoteServer = this.m_pOwnerUser.RemoteServers.Add(this.m_pDescription.Text, this.m_pServer.Text, (int)this.m_pPort.Value, this.m_UseSSL.Checked, this.m_pUser.Text, this.m_pPassword.Text, this.m_pEnabled.Checked);
			}
			else
			{
				this.m_pRemoteServer.Enabled = this.m_pEnabled.Checked;
				this.m_pRemoteServer.Description = this.m_pDescription.Text;
				this.m_pRemoteServer.Host = this.m_pServer.Text;
				this.m_pRemoteServer.Port = (int)this.m_pPort.Value;
				this.m_pRemoteServer.SSL = this.m_UseSSL.Checked;
				this.m_pRemoteServer.UserName = this.m_pUser.Text;
				this.m_pRemoteServer.Password = this.m_pPassword.Text;
				this.m_pRemoteServer.Commit();
			}
			base.DialogResult = DialogResult.OK;
			base.Close();
		}
	}
}
