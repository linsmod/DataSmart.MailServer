using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class SystemSipServiceGatewayForm : Form
	{
		private PictureBox m_pIcon;

		private Label mt_Info;

		private GroupBox m_pSeparator1;

		private Label mt_UriScheme;

		private ComboBox m_pUriScheme;

		private Label mt_Transport;

		private ComboBox m_pTransport;

		private Label mt_Host;

		private TextBox m_pHost;

		private NumericUpDown m_pPort;

		private Label mt_Realm;

		private TextBox m_pRealm;

		private Label mt_UserName;

		private TextBox m_pUserName;

		private Label mt_Password;

		private TextBox m_pPassword;

		private GroupBox m_pSeparator2;

		private Button m_pCancel;

		private Button m_pOk;

		public string UriScheme
		{
			get
			{
				return this.m_pUriScheme.Text;
			}
		}

		public string Transport
		{
			get
			{
				return this.m_pTransport.Text;
			}
		}

		public string Host
		{
			get
			{
				return this.m_pHost.Text;
			}
		}

		public int Port
		{
			get
			{
				return (int)this.m_pPort.Value;
			}
		}

		public string Realm
		{
			get
			{
				return this.m_pRealm.Text;
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

		public SystemSipServiceGatewayForm()
		{
			this.InitializeComponent();
		}

		public SystemSipServiceGatewayForm(string uriScheme, string transport, string host, int port, string realm, string userName, string password)
		{
			this.InitializeComponent();
			this.m_pUriScheme.Text = uriScheme;
			this.m_pTransport.Text = transport;
			this.m_pHost.Text = host;
			this.m_pPort.Value = port;
			this.m_pRealm.Text = realm;
			this.m_pUserName.Text = userName;
			this.m_pPassword.Text = password;
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(400, 260);
			base.StartPosition = FormStartPosition.CenterScreen;
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.Text = "SIP Gateway";
			base.Icon = ResManager.GetIcon("rule.ico");
			this.m_pIcon = new PictureBox();
			this.m_pIcon.Size = new Size(32, 32);
			this.m_pIcon.Location = new Point(10, 10);
			this.m_pIcon.Image = ResManager.GetIcon("rule.ico").ToBitmap();
			this.mt_Info = new Label();
			this.mt_Info.Size = new Size(350, 32);
			this.mt_Info.Location = new Point(50, 10);
			this.mt_Info.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.mt_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Info.Text = "SIP gateway info.";
			this.m_pSeparator1 = new GroupBox();
			this.m_pSeparator1.Size = new Size(385, 3);
			this.m_pSeparator1.Location = new Point(7, 50);
			this.m_pSeparator1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.mt_UriScheme = new Label();
			this.mt_UriScheme.Size = new Size(100, 20);
			this.mt_UriScheme.Location = new Point(0, 65);
			this.mt_UriScheme.TextAlign = ContentAlignment.MiddleRight;
			this.mt_UriScheme.Text = "URI scheme:";
			this.m_pUriScheme = new ComboBox();
			this.m_pUriScheme.Size = new Size(80, 20);
			this.m_pUriScheme.Location = new Point(105, 65);
			this.m_pUriScheme.Items.Add("tel");
			this.m_pUriScheme.Text = "tel";
			this.mt_Transport = new Label();
			this.mt_Transport.Size = new Size(100, 20);
			this.mt_Transport.Location = new Point(0, 90);
			this.mt_Transport.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Transport.Text = "Transport:";
			this.m_pTransport = new ComboBox();
			this.m_pTransport.Size = new Size(80, 20);
			this.m_pTransport.Location = new Point(105, 90);
			this.m_pTransport.DropDownStyle = ComboBoxStyle.DropDownList;
			this.m_pTransport.Items.Add("UDP");
			this.m_pTransport.Items.Add("TCP");
			this.m_pTransport.Items.Add("TLS");
			this.m_pTransport.Text = "UDP";
			this.mt_Host = new Label();
			this.mt_Host.Size = new Size(100, 20);
			this.mt_Host.Location = new Point(0, 115);
			this.mt_Host.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Host.Text = "Host:";
			this.m_pHost = new TextBox();
			this.m_pHost.Size = new Size(205, 20);
			this.m_pHost.Location = new Point(105, 115);
			this.m_pHost.TabIndex = 1;
			this.m_pPort = new NumericUpDown();
			this.m_pPort.Size = new Size(70, 20);
			this.m_pPort.Location = new Point(315, 115);
			this.m_pPort.Minimum = 1m;
			this.m_pPort.Maximum = 99999m;
			this.m_pPort.Value = 5060m;
			this.mt_Realm = new Label();
			this.mt_Realm.Size = new Size(100, 20);
			this.mt_Realm.Location = new Point(0, 140);
			this.mt_Realm.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Realm.Text = "Realm:";
			this.m_pRealm = new TextBox();
			this.m_pRealm.Size = new Size(205, 20);
			this.m_pRealm.Location = new Point(105, 140);
			this.mt_UserName = new Label();
			this.mt_UserName.Size = new Size(100, 20);
			this.mt_UserName.Location = new Point(0, 165);
			this.mt_UserName.TextAlign = ContentAlignment.MiddleRight;
			this.mt_UserName.Text = "User:";
			this.m_pUserName = new TextBox();
			this.m_pUserName.Size = new Size(205, 20);
			this.m_pUserName.Location = new Point(105, 165);
			this.mt_Password = new Label();
			this.mt_Password.Size = new Size(100, 20);
			this.mt_Password.Location = new Point(0, 190);
			this.mt_Password.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Password.Text = "Password:";
			this.m_pPassword = new TextBox();
			this.m_pPassword.Size = new Size(205, 20);
			this.m_pPassword.Location = new Point(105, 190);
			this.m_pPassword.PasswordChar = '*';
			this.m_pSeparator2 = new GroupBox();
			this.m_pSeparator2.Size = new Size(385, 3);
			this.m_pSeparator2.Location = new Point(7, 225);
			this.m_pSeparator2.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
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
			base.Controls.Add(this.mt_UriScheme);
			base.Controls.Add(this.m_pUriScheme);
			base.Controls.Add(this.mt_Transport);
			base.Controls.Add(this.m_pTransport);
			base.Controls.Add(this.mt_Host);
			base.Controls.Add(this.m_pHost);
			base.Controls.Add(this.m_pPort);
			base.Controls.Add(this.mt_Realm);
			base.Controls.Add(this.m_pRealm);
			base.Controls.Add(this.mt_UserName);
			base.Controls.Add(this.m_pUserName);
			base.Controls.Add(this.mt_Password);
			base.Controls.Add(this.m_pPassword);
			base.Controls.Add(this.m_pSeparator2);
			base.Controls.Add(this.m_pCancel);
			base.Controls.Add(this.m_pOk);
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void m_pOk_Click(object sender, EventArgs e)
		{
			if (this.m_pUriScheme.Text == "")
			{
				MessageBox.Show(this, "Please fill URI scheme !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (this.m_pHost.Text == "")
			{
				MessageBox.Show(this, "Please fill Host !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			base.DialogResult = DialogResult.OK;
			base.Close();
		}
	}
}
