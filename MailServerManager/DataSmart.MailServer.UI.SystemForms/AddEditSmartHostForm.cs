using DataSmart.MailServer.UI.Resources;
using System.NetworkToolkit;
using System.NetworkToolkit.SMTP.Relay;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI.SystemForms
{
	public class AddEditSmartHostForm : Form
	{
		private PictureBox m_pIcon;

		private Label mt_Info;

		private GroupBox m_pSeparator1;

		private Label mt_Server;

		private TextBox m_pServer;

		private NumericUpDown m_pPort;

		private Label mt_SslMode;

		private ComboBox m_pSslMode;

		private Label mt_UserName;

		private TextBox m_pUserName;

		private Label mt_Password;

		private TextBox m_pPassword;

		private GroupBox m_pSeparator2;

		private Button m_pCancel;

		private Button m_pOk;

		public string Host
		{
			get
			{
				return this.m_pServer.Text;
			}
		}

		public int Port
		{
			get
			{
				return (int)this.m_pPort.Value;
			}
		}

		public SslMode SslMode
		{
			get
			{
				if (this.m_pSslMode.Text.ToLower() == "ssl")
				{
					return SslMode.SSL;
				}
				if (this.m_pSslMode.Text.ToLower() == "tls")
				{
					return SslMode.TLS;
				}
				return SslMode.None;
			}
		}

		public string UserName
		{
			get
			{
				return this.m_pUserName.Text;
			}
		}

		public string Password
		{
			get
			{
				return this.m_pPassword.Text;
			}
		}

		public AddEditSmartHostForm()
		{
			this.InitializeComponent();
		}

		public AddEditSmartHostForm(string host, int port, SslMode sslMode, string userName, string password)
		{
			this.InitializeComponent();
			this.m_pServer.Text = host;
			this.m_pPort.Value = port;
			if (sslMode == SslMode.None)
			{
				this.m_pSslMode.SelectedIndex = 0;
			}
			else if (sslMode == SslMode.SSL)
			{
				this.m_pSslMode.SelectedIndex = 1;
			}
			else if (sslMode == SslMode.TLS)
			{
				this.m_pSslMode.SelectedIndex = 2;
			}
			this.m_pUserName.Text = userName;
			this.m_pPassword.Text = password;
		}

		public AddEditSmartHostForm(Relay_SmartHost smartHost)
		{
			this.InitializeComponent();
			this.m_pServer.Text = smartHost.Host;
			this.m_pPort.Value = smartHost.Port;
			if (smartHost.SslMode == SslMode.None)
			{
				this.m_pSslMode.SelectedIndex = 0;
			}
			else if (smartHost.SslMode == SslMode.SSL)
			{
				this.m_pSslMode.SelectedIndex = 1;
			}
			else if (smartHost.SslMode == SslMode.TLS)
			{
				this.m_pSslMode.SelectedIndex = 2;
			}
			this.m_pUserName.Text = smartHost.UserName;
			this.m_pPassword.Text = smartHost.Password;
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(400, 230);
			base.StartPosition = FormStartPosition.CenterScreen;
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			this.Text = "Add/Edit smart host";
			this.m_pIcon = new PictureBox();
			this.m_pIcon.Size = new Size(32, 32);
			this.m_pIcon.Location = new Point(10, 10);
			this.m_pIcon.Image = ResManager.GetIcon("server.ico").ToBitmap();
			this.mt_Info = new Label();
			this.mt_Info.Size = new Size(200, 32);
			this.mt_Info.Location = new Point(50, 10);
			this.mt_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Info.Text = "Specify smart host information.";
			this.m_pSeparator1 = new GroupBox();
			this.m_pSeparator1.Size = new Size(385, 3);
			this.m_pSeparator1.Location = new Point(10, 50);
			this.mt_Server = new Label();
			this.mt_Server.Size = new Size(100, 20);
			this.mt_Server.Location = new Point(0, 70);
			this.mt_Server.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Server.Text = "Host:";
			this.m_pServer = new TextBox();
			this.m_pServer.Size = new Size(200, 20);
			this.m_pServer.Location = new Point(105, 70);
			this.m_pPort = new NumericUpDown();
			this.m_pPort.Size = new Size(75, 20);
			this.m_pPort.Location = new Point(310, 70);
			this.m_pPort.Minimum = 1m;
			this.m_pPort.Maximum = 99999m;
			this.m_pPort.Value = WellKnownPorts.SMTP;
			this.mt_SslMode = new Label();
			this.mt_SslMode.Size = new Size(100, 20);
			this.mt_SslMode.Location = new Point(0, 95);
			this.mt_SslMode.TextAlign = ContentAlignment.MiddleRight;
			this.mt_SslMode.Text = "SSL Mode:";
			this.m_pSslMode = new ComboBox();
			this.m_pSslMode.Size = new Size(100, 20);
			this.m_pSslMode.Location = new Point(105, 95);
			this.m_pSslMode.DropDownStyle = ComboBoxStyle.DropDownList;
			this.m_pSslMode.SelectedIndexChanged += new EventHandler(this.m_pSslMode_SelectedIndexChanged);
			this.m_pSslMode.Items.Add("None");
			this.m_pSslMode.Items.Add("SSL");
			this.m_pSslMode.Items.Add("TLS");
			this.m_pSslMode.SelectedIndex = 0;
			this.mt_UserName = new Label();
			this.mt_UserName.Size = new Size(100, 20);
			this.mt_UserName.Location = new Point(0, 120);
			this.mt_UserName.TextAlign = ContentAlignment.MiddleRight;
			this.mt_UserName.Text = "User Name:";
			this.m_pUserName = new TextBox();
			this.m_pUserName.Size = new Size(200, 20);
			this.m_pUserName.Location = new Point(105, 120);
			this.mt_Password = new Label();
			this.mt_Password.Size = new Size(100, 20);
			this.mt_Password.Location = new Point(0, 145);
			this.mt_Password.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Password.Text = "Password:";
			this.m_pPassword = new TextBox();
			this.m_pPassword.Size = new Size(200, 20);
			this.m_pPassword.Location = new Point(105, 145);
			this.m_pPassword.PasswordChar = '*';
			this.m_pSeparator2 = new GroupBox();
			this.m_pSeparator2.Size = new Size(383, 4);
			this.m_pSeparator2.Location = new Point(7, 180);
			this.m_pCancel = new Button();
			this.m_pCancel.Size = new Size(70, 20);
			this.m_pCancel.Location = new Point(240, 200);
			this.m_pCancel.Text = "Cancel";
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			this.m_pOk = new Button();
			this.m_pOk.Size = new Size(70, 20);
			this.m_pOk.Location = new Point(315, 200);
			this.m_pOk.Text = "Ok";
			this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
			base.Controls.Add(this.m_pIcon);
			base.Controls.Add(this.mt_Info);
			base.Controls.Add(this.m_pSeparator1);
			base.Controls.Add(this.mt_Server);
			base.Controls.Add(this.m_pServer);
			base.Controls.Add(this.m_pPort);
			base.Controls.Add(this.mt_SslMode);
			base.Controls.Add(this.m_pSslMode);
			base.Controls.Add(this.mt_UserName);
			base.Controls.Add(this.m_pUserName);
			base.Controls.Add(this.mt_Password);
			base.Controls.Add(this.m_pPassword);
			base.Controls.Add(this.m_pSeparator2);
			base.Controls.Add(this.m_pCancel);
			base.Controls.Add(this.m_pOk);
		}

		private void m_pSslMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pSslMode.Text == "SSL")
			{
				this.m_pPort.Value = WellKnownPorts.SMTP_SSL;
				return;
			}
			this.m_pPort.Value = WellKnownPorts.SMTP;
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
			base.Close();
		}

		private void m_pOk_Click(object sender, EventArgs e)
		{
			if (this.m_pServer.Text.Trim() == "")
			{
				MessageBox.Show(this, "Please specify Host value !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			base.DialogResult = DialogResult.OK;
			base.Close();
		}
	}
}
