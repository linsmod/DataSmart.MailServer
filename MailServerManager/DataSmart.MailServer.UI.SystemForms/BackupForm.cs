using DataSmart.MailServer.Management;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI.SystemForms
{
	public class BackupForm : Form
	{
		private GroupBox m_pGroubBox_Backup;

		private Button m_pBackup;

		private Button m_pBackupMessages;

		private GroupBox m_pGroupBox_Restore;

		private Button m_pRestore;

		private CheckBox m_pRestoreFlagsAdd;

		private CheckBox m_pRestoreFlagsReplace;

		private VirtualServer m_pVirtualServer;

		public BackupForm(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
		}

		private void InitializeComponent()
		{
			base.Size = new Size(472, 357);
			this.m_pGroubBox_Backup = new GroupBox();
			this.m_pGroubBox_Backup.Size = new Size(400, 150);
			this.m_pGroubBox_Backup.Location = new Point(20, 20);
			this.m_pGroubBox_Backup.Text = "Backup:";
			this.m_pBackup = new Button();
			this.m_pBackup.Size = new Size(110, 20);
			this.m_pBackup.Location = new Point(20, 40);
			this.m_pBackup.Text = "Backup";
			this.m_pBackup.Click += new EventHandler(this.m_pBackup_Click);
			this.m_pBackupMessages = new Button();
			this.m_pBackupMessages.Size = new Size(110, 20);
			this.m_pBackupMessages.Location = new Point(20, 80);
			this.m_pBackupMessages.Text = "Backup Messages";
			this.m_pBackupMessages.Click += new EventHandler(this.m_pBackupMessages_Click);
			this.m_pGroubBox_Backup.Controls.Add(this.m_pBackup);
			this.m_pGroubBox_Backup.Controls.Add(this.m_pBackupMessages);
			this.m_pGroupBox_Restore = new GroupBox();
			this.m_pGroupBox_Restore.Size = new Size(400, 150);
			this.m_pGroupBox_Restore.Location = new Point(20, 200);
			this.m_pGroupBox_Restore.Text = "Restore:";
			this.m_pRestore = new Button();
			this.m_pRestore.Size = new Size(70, 20);
			this.m_pRestore.Location = new Point(20, 40);
			this.m_pRestore.Text = "Restore";
			this.m_pRestore.Click += new EventHandler(this.m_pRestore_Click);
			this.m_pRestoreFlagsAdd = new CheckBox();
			this.m_pRestoreFlagsAdd.Size = new Size(200, 20);
			this.m_pRestoreFlagsAdd.Location = new Point(20, 70);
			this.m_pRestoreFlagsAdd.Text = "Add non existent items";
			this.m_pRestoreFlagsAdd.Checked = true;
			this.m_pRestoreFlagsReplace = new CheckBox();
			this.m_pRestoreFlagsReplace.Size = new Size(200, 20);
			this.m_pRestoreFlagsReplace.Location = new Point(20, 95);
			this.m_pRestoreFlagsReplace.Text = "Replace existing items";
			this.m_pRestoreFlagsReplace.Checked = false;
			this.m_pGroupBox_Restore.Controls.Add(this.m_pRestore);
			this.m_pGroupBox_Restore.Controls.Add(this.m_pRestoreFlagsAdd);
			this.m_pGroupBox_Restore.Controls.Add(this.m_pRestoreFlagsReplace);
			base.Controls.Add(this.m_pGroubBox_Backup);
			base.Controls.Add(this.m_pGroupBox_Restore);
		}

		private void m_pBackup_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "DataSmart Mail Server backup (*.lsmbck)|*.lsmbck";
			if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				this.m_pVirtualServer.Backup(saveFileDialog.FileName);
			}
		}

		private void m_pBackupMessages_Click(object sender, EventArgs e)
		{
			BackupMessagesForm backupMessagesForm = new BackupMessagesForm(this.m_pVirtualServer);
			backupMessagesForm.ShowDialog(this);
		}

		private void m_pRestore_Click(object sender, EventArgs e)
		{
			RestoreFlags_enum restoreFlags_enum = (RestoreFlags_enum)0;
			if (this.m_pRestoreFlagsAdd.Checked)
			{
				restoreFlags_enum |= RestoreFlags_enum.Add;
			}
			if (this.m_pRestoreFlagsReplace.Checked)
			{
				restoreFlags_enum |= RestoreFlags_enum.Replace;
			}
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "DataSmart Mail Server backup (*.lsmbck)|*.lsmbck";
			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				this.m_pVirtualServer.Restore(openFileDialog.FileName, restoreFlags_enum);
			}
		}
	}
}
