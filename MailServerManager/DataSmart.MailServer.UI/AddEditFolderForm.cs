using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class AddEditFolderForm : Form
	{
		private Label mt_Folder;

		private TextBox m_pFolder;

		private Button m_pOk;

		private Button m_Cancel;

		private bool m_MayContainPath = true;

		public string Folder
		{
			get
			{
				return this.m_pFolder.Text;
			}
		}

		public AddEditFolderForm(bool create_rename, string folder, bool mayContainPath)
		{
			this.m_MayContainPath = mayContainPath;
			this.InitializeComponent();
			if (create_rename)
			{
				this.Text = "Add new Folder";
			}
			else
			{
				this.Text = "Rename Folder";
			}
			this.m_pFolder.Text = folder;
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(292, 103);
			base.StartPosition = FormStartPosition.CenterScreen;
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MinimizeBox = false;
			base.MaximizeBox = false;
			this.mt_Folder = new Label();
			this.mt_Folder.Size = new Size(200, 13);
			this.mt_Folder.Location = new Point(9, 20);
			this.mt_Folder.Text = "Folder Name:";
			this.m_pFolder = new TextBox();
			this.m_pFolder.Size = new Size(270, 13);
			this.m_pFolder.Location = new Point(9, 35);
			this.m_Cancel = new Button();
			this.m_Cancel.Size = new Size(70, 20);
			this.m_Cancel.Location = new Point(135, 70);
			this.m_Cancel.Text = "Cancel";
			this.m_Cancel.Click += new EventHandler(this.m_Cancel_Click);
			this.m_pOk = new Button();
			this.m_pOk.Size = new Size(70, 20);
			this.m_pOk.Location = new Point(210, 70);
			this.m_pOk.Text = "Ok";
			this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
			base.Controls.Add(this.mt_Folder);
			base.Controls.Add(this.m_pFolder);
			base.Controls.Add(this.m_Cancel);
			base.Controls.Add(this.m_pOk);
		}

		private void m_Cancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
			base.Close();
		}

		private void m_pOk_Click(object sender, EventArgs e)
		{
			if (this.m_pFolder.Text == "")
			{
				MessageBox.Show(this, "Folder name can't be empty !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (!this.m_MayContainPath && this.m_pFolder.Text.IndexOfAny(new char[]
			{
				'/',
				'\\'
			}) > -1)
			{
				MessageBox.Show(this, "Path in folder name not allowed, please specify folder name only !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			base.DialogResult = DialogResult.OK;
			base.Close();
		}
	}
}
