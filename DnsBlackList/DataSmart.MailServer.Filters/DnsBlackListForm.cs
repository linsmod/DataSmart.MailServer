using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.Filters
{
	public class DnsBlackListForm : Form
	{
		private Label mt_Server;

		private ComboBox m_pServer;

		private Label mt_RejectionText;

		private TextBox m_pRejectionText;

		private GroupBox m_pGroupBox1;

		private Button m_pCancel;

		private Button m_pOk;

		public string Server
		{
			get
			{
				return this.m_pServer.Text;
			}
		}

		public string DefaultRejectionText
		{
			get
			{
				return this.m_pRejectionText.Text;
			}
		}

		public DnsBlackListForm()
		{
			this.InitializeComponent();
		}

		private void InitializeComponent()
		{
			this.mt_Server = new Label();
			this.m_pServer = new ComboBox();
			this.mt_RejectionText = new Label();
			this.m_pRejectionText = new TextBox();
			this.m_pGroupBox1 = new GroupBox();
			this.m_pCancel = new Button();
			this.m_pOk = new Button();
			base.SuspendLayout();
			this.mt_Server.Location = new Point(10, 10);
			this.mt_Server.Name = "mt_Server";
			this.mt_Server.Size = new Size(100, 20);
			this.mt_Server.TabIndex = 0;
			this.mt_Server.Text = "Server:";
			this.m_pServer.Items.AddRange(new object[]
			{
				"sbl-xbl.spamhaus.org",
				"bl.spamcop.net",
				"dnsbl.sorbs.net",
				"relays.ordb.org"
			});
			this.m_pServer.Location = new Point(10, 30);
			this.m_pServer.Name = "m_pServer";
			this.m_pServer.Size = new Size(370, 21);
			this.m_pServer.TabIndex = 1;
			this.mt_RejectionText.Location = new Point(10, 60);
			this.mt_RejectionText.Name = "mt_RejectionText";
			this.mt_RejectionText.Size = new Size(300, 20);
			this.mt_RejectionText.TabIndex = 2;
			this.mt_RejectionText.Text = "Default rejection text: (if server won't provide TXT record)";
			this.m_pRejectionText.Location = new Point(10, 80);
			this.m_pRejectionText.Name = "m_pRejectionText";
			this.m_pRejectionText.Size = new Size(370, 20);
			this.m_pRejectionText.TabIndex = 3;
			this.m_pGroupBox1.Location = new Point(5, 135);
			this.m_pGroupBox1.Name = "m_pGroupBox1";
			this.m_pGroupBox1.Size = new Size(370, 4);
			this.m_pGroupBox1.TabIndex = 4;
			this.m_pGroupBox1.TabStop = false;
			this.m_pCancel.Location = new Point(235, 145);
			this.m_pCancel.Name = "m_pCancel";
			this.m_pCancel.Size = new Size(70, 23);
			this.m_pCancel.TabIndex = 5;
			this.m_pCancel.Text = "Cancel";
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			this.m_pOk.Location = new Point(310, 145);
			this.m_pOk.Name = "m_pOk";
			this.m_pOk.Size = new Size(70, 23);
			this.m_pOk.TabIndex = 6;
			this.m_pOk.Text = "Ok";
			this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
			base.ClientSize = new Size(402, 182);
			base.Controls.Add(this.mt_Server);
			base.Controls.Add(this.m_pServer);
			base.Controls.Add(this.mt_RejectionText);
			base.Controls.Add(this.m_pRejectionText);
			base.Controls.Add(this.m_pGroupBox1);
			base.Controls.Add(this.m_pCancel);
			base.Controls.Add(this.m_pOk);
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.Name = "wfrm_DNSBL_Entry";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "DNSBL Entry Settings";
			base.ResumeLayout(false);
			base.PerformLayout();
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
				MessageBox.Show(this, "Server can't be empty !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			base.DialogResult = DialogResult.OK;
			base.Close();
		}
	}
}
