using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System.NetworkToolkit;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class AddEditBindInfoForm : Form
	{
		private PictureBox m_pIcon;

		private Label mt_Info;

		private GroupBox m_pSeparator1;

		private Label mt_HostName;

		private TextBox m_pHostName;

		private Label mt_Protocol;

		private ComboBox m_pProtocol;

		private Label mt_IpEndPoint;

		private ComboBox m_pIP;

		private NumericUpDown m_pPort;

		private Label mt_SslMode;

		private ComboBox m_pSslMode;

		private ToolStrip m_pSslToolbar;

		private PictureBox m_pSslIcon;

		private GroupBox m_pSeparator2;

		private Button m_pCancel;

		private Button m_pOk;

		private bool m_SslEnabled = true;

		private int m_DefaultPort = 10000;

		private int m_DefaultSSLPort = 10001;

		private X509Certificate2 m_pCert;

		public string HostName
		{
			get
			{
				return this.m_pHostName.Text;
			}
		}

		public BindInfoProtocol Protocol
		{
			get
			{
				return (BindInfoProtocol)Enum.Parse(typeof(BindInfoProtocol), this.m_pProtocol.SelectedItem.ToString());
			}
		}

		public IPAddress IP
		{
			get
			{
				return (IPAddress)((WComboBoxItem)this.m_pIP.SelectedItem).Tag;
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
				if (this.m_pSslMode.SelectedIndex == 1)
				{
					return SslMode.SSL;
				}
				if (this.m_pSslMode.SelectedIndex == 2)
				{
					return SslMode.TLS;
				}
				return SslMode.None;
			}
		}

		public X509Certificate2 Certificate
		{
			get
			{
				return this.m_pCert;
			}
		}

		public AddEditBindInfoForm(Server server, int defaultPort, int defaultSSLPort) : this(server, false, defaultPort, defaultSSLPort)
		{
		}

		public AddEditBindInfoForm(Server server, bool allowUDP, int defaultPort, int defaultSSLPort) : this(server, allowUDP, defaultPort, defaultSSLPort, null)
		{
		}

		public AddEditBindInfoForm(Server server, bool allowUDP, int defaultPort, int defaultSSLPort, IPBindInfo bindInfo) : this(server, allowUDP, true, true, defaultPort, defaultSSLPort, bindInfo)
		{
		}

		public AddEditBindInfoForm(Server server, bool allowUDP, bool allowSSL, bool allowChangePort, int defaultPort, int defaultSSLPort, IPBindInfo bindInfo)
		{
			this.m_SslEnabled = allowSSL;
			this.m_DefaultPort = defaultPort;
			this.m_DefaultSSLPort = defaultSSLPort;
			this.InitializeComponent();
			this.m_pSslMode.SelectedIndex = 0;
			if (!allowSSL)
			{
				this.m_pSslMode.Enabled = false;
			}
			if (!allowChangePort)
			{
				this.m_pPort.Enabled = false;
			}
			if (bindInfo != null)
			{
				this.m_pHostName.Text = bindInfo.HostName;
			}
			this.m_pProtocol.Items.Add("TCP");
			if (allowUDP)
			{
				this.m_pProtocol.Items.Add("UDP");
			}
			if (bindInfo == null)
			{
				this.m_pProtocol.SelectedIndex = 0;
			}
			else
			{
				this.m_pProtocol.Text = bindInfo.Protocol.ToString();
			}
			IPAddress[] iPAddresses = server.IPAddresses;
			for (int i = 0; i < iPAddresses.Length; i++)
			{
				IPAddress iPAddress = iPAddresses[i];
				this.m_pIP.Items.Add(new WComboBoxItem(this.IpToString(iPAddress), iPAddress));
			}
			if (bindInfo == null)
			{
				this.m_pIP.SelectedIndex = 0;
				this.m_pPort.Value = defaultPort;
			}
			else
			{
				this.m_pCert = bindInfo.Certificate;
				this.m_pIP.Text = this.IpToString(bindInfo.IP);
				this.m_pPort.Value = bindInfo.Port;
				this.m_pSslMode.Text = bindInfo.SslMode.ToString();
			}
			this.UpdateCertStatus();
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(380, 210);
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Add/Edit Bind info";
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			this.m_pIcon = new PictureBox();
			this.m_pIcon.Size = new Size(32, 32);
			this.m_pIcon.Location = new Point(10, 10);
			this.m_pIcon.Image = ResManager.GetIcon("server.ico").ToBitmap();
			this.mt_Info = new Label();
			this.mt_Info.Size = new Size(200, 32);
			this.mt_Info.Location = new Point(50, 10);
			this.mt_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Info.Text = "Specify IP binding information.";
			this.m_pSeparator1 = new GroupBox();
			this.m_pSeparator1.Size = new Size(365, 3);
			this.m_pSeparator1.Location = new Point(10, 50);
			this.mt_HostName = new Label();
			this.mt_HostName.Size = new Size(100, 20);
			this.mt_HostName.Location = new Point(0, 60);
			this.mt_HostName.TextAlign = ContentAlignment.MiddleRight;
			this.mt_HostName.Text = "Host Name:";
			this.m_pHostName = new TextBox();
			this.m_pHostName.Size = new Size(270, 20);
			this.m_pHostName.Location = new Point(105, 60);
			this.mt_Protocol = new Label();
			this.mt_Protocol.Size = new Size(100, 20);
			this.mt_Protocol.Location = new Point(0, 85);
			this.mt_Protocol.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Protocol.Text = "Protocol:";
			this.m_pProtocol = new ComboBox();
			this.m_pProtocol.Size = new Size(60, 20);
			this.m_pProtocol.Location = new Point(105, 85);
			this.m_pProtocol.DropDownStyle = ComboBoxStyle.DropDownList;
			this.m_pProtocol.SelectedIndexChanged += new EventHandler(this.m_pProtocol_SelectedIndexChanged);
			this.mt_IpEndPoint = new Label();
			this.mt_IpEndPoint.Size = new Size(100, 20);
			this.mt_IpEndPoint.Location = new Point(0, 110);
			this.mt_IpEndPoint.TextAlign = ContentAlignment.MiddleRight;
			this.mt_IpEndPoint.Text = "IP EndPoint:";
			this.m_pIP = new ComboBox();
			this.m_pIP.Size = new Size(200, 20);
			this.m_pIP.Location = new Point(105, 110);
			this.m_pIP.DropDownStyle = ComboBoxStyle.DropDownList;
			this.m_pPort = new NumericUpDown();
			this.m_pPort.Size = new Size(63, 20);
			this.m_pPort.Location = new Point(310, 110);
			this.m_pPort.Minimum = 0m;
			this.m_pPort.Maximum = 99999m;
			this.mt_SslMode = new Label();
			this.mt_SslMode.Size = new Size(100, 20);
			this.mt_SslMode.Location = new Point(0, 140);
			this.mt_SslMode.TextAlign = ContentAlignment.MiddleRight;
			this.mt_SslMode.Text = "SSL Mode:";
			this.m_pSslMode = new ComboBox();
			this.m_pSslMode.Size = new Size(60, 20);
			this.m_pSslMode.Location = new Point(105, 140);
			this.m_pSslMode.DropDownStyle = ComboBoxStyle.DropDownList;
			this.m_pSslMode.SelectedIndexChanged += new EventHandler(this.m_pSslMode_SelectedIndexChanged);
			this.m_pSslMode.Items.Add("None");
			this.m_pSslMode.Items.Add("SSL");
			this.m_pSslMode.Items.Add("TLS");
			this.m_pSslToolbar = new ToolStrip();
			this.m_pSslToolbar.Size = new Size(95, 25);
			this.m_pSslToolbar.Location = new Point(210, 140);
			this.m_pSslToolbar.Dock = DockStyle.None;
			this.m_pSslToolbar.GripStyle = ToolStripGripStyle.Hidden;
			this.m_pSslToolbar.BackColor = this.BackColor;
			this.m_pSslToolbar.Renderer = new ToolBarRendererEx();
			this.m_pSslToolbar.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pSslToolbar_ItemClicked);
			ToolStripButton toolStripButton = new ToolStripButton();
			toolStripButton.Image = ResManager.GetIcon("write.ico").ToBitmap();
			toolStripButton.Name = "create";
			toolStripButton.ToolTipText = "Create SSL certificate.";
			this.m_pSslToolbar.Items.Add(toolStripButton);
			ToolStripButton toolStripButton2 = new ToolStripButton();
			toolStripButton2.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripButton2.Name = "add";
			toolStripButton2.ToolTipText = "Add SSL certificate.";
			this.m_pSslToolbar.Items.Add(toolStripButton2);
			ToolStripButton toolStripButton3 = new ToolStripButton();
			toolStripButton3.Enabled = false;
			toolStripButton3.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			toolStripButton3.Name = "delete";
			toolStripButton3.ToolTipText = "Delete SSL certificate.";
			this.m_pSslToolbar.Items.Add(toolStripButton3);
			ToolStripButton toolStripButton4 = new ToolStripButton();
			toolStripButton4.Enabled = false;
			toolStripButton4.Image = ResManager.GetIcon("save.ico").ToBitmap();
			toolStripButton4.Name = "save";
			toolStripButton4.ToolTipText = "Export SSL certificate.";
			this.m_pSslToolbar.Items.Add(toolStripButton4);
			this.m_pSslIcon = new PictureBox();
			this.m_pSslIcon.Size = new Size(32, 32);
			this.m_pSslIcon.Location = new Point(180, 135);
			this.m_pSslIcon.BorderStyle = BorderStyle.None;
			this.m_pSslIcon.SizeMode = PictureBoxSizeMode.StretchImage;
			this.m_pSslIcon.Image = ImageUtil.GetGrayImage(ResManager.GetIcon("ssl.ico", new Size(32, 32)).ToBitmap());
			this.m_pSeparator2 = new GroupBox();
			this.m_pSeparator2.Size = new Size(365, 2);
			this.m_pSeparator2.Location = new Point(5, 175);
			this.m_pCancel = new Button();
			this.m_pCancel.Size = new Size(70, 20);
			this.m_pCancel.Location = new Point(225, 185);
			this.m_pCancel.Text = "Cancel";
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			this.m_pOk = new Button();
			this.m_pOk.Size = new Size(70, 20);
			this.m_pOk.Location = new Point(300, 185);
			this.m_pOk.Text = "Ok";
			this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
			base.Controls.Add(this.m_pIcon);
			base.Controls.Add(this.mt_Info);
			base.Controls.Add(this.m_pSeparator1);
			base.Controls.Add(this.mt_HostName);
			base.Controls.Add(this.m_pHostName);
			base.Controls.Add(this.mt_Protocol);
			base.Controls.Add(this.m_pProtocol);
			base.Controls.Add(this.mt_IpEndPoint);
			base.Controls.Add(this.m_pIP);
			base.Controls.Add(this.m_pPort);
			base.Controls.Add(this.mt_SslMode);
			base.Controls.Add(this.m_pSslMode);
			base.Controls.Add(this.m_pSslToolbar);
			base.Controls.Add(this.m_pSslIcon);
			base.Controls.Add(this.m_pSeparator2);
			base.Controls.Add(this.m_pCancel);
			base.Controls.Add(this.m_pOk);
		}

		private void m_pProtocol_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_SslEnabled && this.m_pProtocol.SelectedItem.ToString() == "TCP")
			{
				this.m_pSslMode.Enabled = true;
				return;
			}
			this.m_pSslMode.Enabled = false;
		}

		private void m_pSslMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pSslMode.SelectedIndex > 0)
			{
				this.m_pSslToolbar.Enabled = true;
				this.m_pSslIcon.Enabled = true;
			}
			else
			{
				this.m_pSslToolbar.Enabled = false;
				this.m_pSslIcon.Enabled = false;
			}
			if (this.m_pSslMode.SelectedItem.ToString() == "SSL")
			{
				this.m_pPort.Value = this.m_DefaultSSLPort;
				return;
			}
			this.m_pPort.Value = this.m_DefaultPort;
		}

		private void m_pSslToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Name == null)
			{
				return;
			}
			if (e.ClickedItem.Name == "create")
			{
				CreateCertificateForm createCertificateForm = new CreateCertificateForm(this.m_pHostName.Text);
				if (createCertificateForm.ShowDialog(this) != DialogResult.OK)
				{
					return;
				}
				try
				{
					X509Certificate2 x509Certificate = new X509Certificate2(createCertificateForm.Certificate, "", X509KeyStorageFlags.Exportable);
					if (!x509Certificate.HasPrivateKey)
					{
						MessageBox.Show(this, "Certificate is not server certificate, private key is missing !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						return;
					}
					this.m_pCert = x509Certificate;
					this.UpdateCertStatus();
					return;
				}
				catch
				{
					MessageBox.Show(this, "Invalid or not supported certificate file !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
			}
			if (e.ClickedItem.Name == "add")
			{
				OpenFileDialog openFileDialog = new OpenFileDialog();
				if (openFileDialog.ShowDialog(this) != DialogResult.OK)
				{
					return;
				}
				try
				{
					X509Certificate2 x509Certificate2 = new X509Certificate2(openFileDialog.FileName, "", X509KeyStorageFlags.Exportable);
					if (!x509Certificate2.HasPrivateKey)
					{
						MessageBox.Show(this, "Certificate is not server certificate, private key is missing !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						return;
					}
					this.m_pCert = x509Certificate2;
					this.UpdateCertStatus();
					return;
				}
				catch
				{
					MessageBox.Show(this, "Invalid or not supported certificate file !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
			}
			if (e.ClickedItem.Name == "delete")
			{
				if (MessageBox.Show(this, "Are you sure you want to delete active SSL certificate ?", "Confirm delete:", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
				{
					this.m_pCert = null;
					this.UpdateCertStatus();
					return;
				}
			}
			else if (e.ClickedItem.Name == "save")
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Filter = "*.pfx | *.p12";
				if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
				{
					File.WriteAllBytes(saveFileDialog.FileName, this.m_pCert.Export(X509ContentType.Pfx));
				}
			}
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
			base.Close();
		}

		private void m_pOk_Click(object sender, EventArgs e)
		{
			if (this.m_pSslMode.SelectedIndex > 0 && this.m_pCert == null)
			{
				MessageBox.Show(this, "Please load certificate !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			base.DialogResult = DialogResult.OK;
			base.Close();
		}

		private void UpdateCertStatus()
		{
			if (this.m_pCert == null)
			{
				ImageUtil.GetGrayImage(ResManager.GetIcon("ssl.ico", new Size(32, 32)).ToBitmap());
				this.m_pSslToolbar.Items["create"].Enabled = true;
				this.m_pSslToolbar.Items["add"].Enabled = true;
				this.m_pSslToolbar.Items["delete"].Enabled = false;
				this.m_pSslToolbar.Items["save"].Enabled = false;
				return;
			}
			this.m_pSslIcon.Image = ResManager.GetIcon("ssl.ico", new Size(48, 48)).ToBitmap();
			this.m_pSslToolbar.Items["create"].Enabled = false;
			this.m_pSslToolbar.Items["add"].Enabled = false;
			this.m_pSslToolbar.Items["delete"].Enabled = true;
			this.m_pSslToolbar.Items["save"].Enabled = true;
		}

		private string IpToString(IPAddress ip)
		{
			if (ip.Equals(IPAddress.Any))
			{
				return "any IPv4";
			}
			if (ip.Equals(IPAddress.Loopback))
			{
				return "localhost IPv4";
			}
			if (ip.Equals(IPAddress.IPv6Any))
			{
				return "Any IPv6";
			}
			if (ip.Equals(IPAddress.IPv6Loopback))
			{
				return "localhost IPv6";
			}
			return ip.ToString();
		}
	}
}
