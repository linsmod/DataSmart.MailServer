using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class AboutForm : Form
	{
		private Label mt_Name;

		private LinkLabel linkLabel1;

		private Button m_pOk;

		public AboutForm()
		{
			this.InitializeComponent();
		}

		private void InitializeComponent()
		{
			this.mt_Name = new Label();
			this.m_pOk = new Button();
			this.linkLabel1 = new LinkLabel();
			base.SuspendLayout();
			this.mt_Name.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.mt_Name.Font = new Font("Microsoft Sans Serif", 10f);
			this.mt_Name.Location = new Point(30, 75);
			this.mt_Name.Name = "mt_Name";
			this.mt_Name.Size = new Size(250, 25);
			this.mt_Name.TabIndex = 0;
			this.mt_Name.Text = "DataSmart Mail Server Manager 1.0";
			this.m_pOk.Location = new Point(210, 195);
			this.m_pOk.Name = "m_pOk";
			this.m_pOk.Size = new Size(70, 23);
			this.m_pOk.TabIndex = 1;
			this.m_pOk.Text = "Ok";
			this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
			this.linkLabel1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Location = new Point(51, 122);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new Size(178, 13);
			this.linkLabel1.TabIndex = 2;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "http://project.dsvisual.cn/mailserver";
			base.ClientSize = new Size(292, 233);
			base.Controls.Add(this.linkLabel1);
			base.Controls.Add(this.mt_Name);
			base.Controls.Add(this.m_pOk);
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.Name = "wfrm_About";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "About";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void m_pOk_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.OK;
			base.Close();
		}
	}
}
