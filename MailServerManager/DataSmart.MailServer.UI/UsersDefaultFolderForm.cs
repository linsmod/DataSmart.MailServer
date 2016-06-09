using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class UsersDefaultFolderForm : Form
	{
		private PictureBox m_pIcon;

		private Label mt_Info;

		private GroupBox m_pSeparator1;

		private Label mt_FolderName;

		private TextBox m_pFolderName;

		private CheckBox m_pPermanent;

		private GroupBox m_pGroupbox1;

		private Button m_pCancel;

		private Button m_pOk;

		private VirtualServer m_pVirtualServer;

		public string FolderName
		{
			get
			{
				return this.m_pFolderName.Text;
			}
		}

		public UsersDefaultFolderForm(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(392, 153);
			base.StartPosition = FormStartPosition.CenterScreen;
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			this.Text = "Add Users Default Folder";
			base.Icon = ResManager.GetIcon("folder32.ico");
			this.m_pIcon = new PictureBox();
			this.m_pIcon.Size = new Size(32, 32);
			this.m_pIcon.Location = new Point(10, 10);
			this.m_pIcon.Image = ResManager.GetIcon("folder32.ico").ToBitmap();
			this.mt_Info = new Label();
			this.mt_Info.Size = new Size(200, 32);
			this.mt_Info.Location = new Point(50, 10);
			this.mt_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Info.Text = "Specify default folder information.";
			this.m_pSeparator1 = new GroupBox();
			this.m_pSeparator1.Size = new Size(383, 3);
			this.m_pSeparator1.Location = new Point(7, 50);
			this.mt_FolderName = new Label();
			this.mt_FolderName.Size = new Size(100, 20);
			this.mt_FolderName.Location = new Point(0, 65);
			this.mt_FolderName.TextAlign = ContentAlignment.MiddleRight;
			this.mt_FolderName.Text = "Folder:";
			this.m_pFolderName = new TextBox();
			this.m_pFolderName.Size = new Size(280, 20);
			this.m_pFolderName.Location = new Point(105, 65);
			this.m_pPermanent = new CheckBox();
			this.m_pPermanent.Size = new Size(250, 20);
			this.m_pPermanent.Location = new Point(105, 90);
			this.m_pPermanent.Text = "Folder is permanent, users can't delete it.";
			this.m_pGroupbox1 = new GroupBox();
			this.m_pGroupbox1.Size = new Size(383, 3);
			this.m_pGroupbox1.Location = new Point(7, 120);
			this.m_pCancel = new Button();
			this.m_pCancel.Size = new Size(70, 20);
			this.m_pCancel.Location = new Point(240, 130);
			this.m_pCancel.Text = "Cancel";
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			this.m_pOk = new Button();
			this.m_pOk.Size = new Size(70, 20);
			this.m_pOk.Location = new Point(315, 130);
			this.m_pOk.Text = "Ok";
			this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
			base.Controls.Add(this.m_pIcon);
			base.Controls.Add(this.mt_Info);
			base.Controls.Add(this.m_pSeparator1);
			base.Controls.Add(this.mt_FolderName);
			base.Controls.Add(this.m_pFolderName);
			base.Controls.Add(this.m_pPermanent);
			base.Controls.Add(this.m_pGroupbox1);
			base.Controls.Add(this.m_pCancel);
			base.Controls.Add(this.m_pOk);
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
			base.Close();
		}

		private void m_pOk_Click(object sender, EventArgs e)
		{
			if (this.m_pFolderName.Text == "")
			{
				MessageBox.Show("Folder name cannot be empty!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			try
			{
				this.m_pVirtualServer.UsersDefaultFolders.Add(this.m_pFolderName.Text, this.m_pPermanent.Checked);
				base.DialogResult = DialogResult.OK;
				base.Close();
			}
			catch (Exception x)
			{
				ErrorForm errorForm = new ErrorForm(x, new StackTrace());
				errorForm.ShowDialog(this);
			}
		}
	}
}
