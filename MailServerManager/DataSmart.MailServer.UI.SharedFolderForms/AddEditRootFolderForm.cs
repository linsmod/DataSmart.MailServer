using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI.SharedFolderForms
{
	public class AddEditRootFolderForm : Form
	{
		private PictureBox m_pIcon;

		private Label mt_Info;

		private GroupBox m_pSeparator1;

		private CheckBox m_pEnabled;

		private Label mt_RootFolderName;

		private TextBox m_pRootFolderName;

		private Label mt_Description;

		private TextBox m_pDescription;

		private Label mt_RootFolderType;

		private ComboBox m_pRootFolderType;

		private Label mt_BoundedUser;

		private TextBox m_pBoundedUser;

		private Button m_pGetBoundedUser;

		private Label mt_BoundedFolder;

		private TextBox m_pBoundedFolder;

		private Button m_pGetBoundedFolder;

		private GroupBox m_pSeparator2;

		private Button m_pCancel;

		private Button m_Ok;

		private VirtualServer m_pVirtualServer;

		private SharedRootFolder m_pRootFolder;

		public string RootID
		{
			get
			{
				if (this.m_pRootFolder != null)
				{
					return this.m_pRootFolder.ID;
				}
				return "";
			}
		}

		public AddEditRootFolderForm(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			this.m_pRootFolderType.SelectedIndex = 0;
		}

		public AddEditRootFolderForm(VirtualServer virtualServer, SharedRootFolder root)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pRootFolder = root;
			this.InitializeComponent();
			this.m_pEnabled.Checked = root.Enabled;
			if (root.Type == SharedFolderRootType.BoundedRootFolder)
			{
				this.m_pRootFolderType.SelectedIndex = 0;
			}
			else if (root.Type == SharedFolderRootType.UsersSharedFolder)
			{
				this.m_pRootFolderType.SelectedIndex = 1;
			}
			this.m_pRootFolderName.Text = root.Name;
			this.m_pDescription.Text = root.Description;
			this.m_pBoundedUser.Text = root.BoundedUser;
			this.m_pBoundedFolder.Text = root.BoundedFolder;
		}

		private void InitializeComponent()
		{
			base.MinimizeBox = false;
			base.StartPosition = FormStartPosition.CenterParent;
			base.ClientSize = new Size(442, 273);
			this.Text = "Shared Folders Add/Edit Root folder";
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = true;
			base.Icon = ResManager.GetIcon("share32.ico");
			this.m_pIcon = new PictureBox();
			this.m_pIcon.Size = new Size(32, 32);
			this.m_pIcon.Location = new Point(10, 10);
			this.m_pIcon.Image = ResManager.GetIcon("share32.ico").ToBitmap();
			this.mt_Info = new Label();
			this.mt_Info.Size = new Size(200, 32);
			this.mt_Info.Location = new Point(50, 10);
			this.mt_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Info.Text = "Specify share information.";
			this.m_pSeparator1 = new GroupBox();
			this.m_pSeparator1.Size = new Size(432, 3);
			this.m_pSeparator1.Location = new Point(7, 50);
			this.m_pEnabled = new CheckBox();
			this.m_pEnabled.Size = new Size(300, 20);
			this.m_pEnabled.Location = new Point(125, 65);
			this.m_pEnabled.Text = "Enabled";
			this.m_pEnabled.Checked = true;
			this.mt_RootFolderName = new Label();
			this.mt_RootFolderName.Size = new Size(120, 20);
			this.mt_RootFolderName.Location = new Point(0, 90);
			this.mt_RootFolderName.TextAlign = ContentAlignment.MiddleRight;
			this.mt_RootFolderName.Text = "Root Folder Name:";
			this.m_pRootFolderName = new TextBox();
			this.m_pRootFolderName.Size = new Size(310, 20);
			this.m_pRootFolderName.Location = new Point(125, 90);
			this.mt_Description = new Label();
			this.mt_Description.Size = new Size(120, 20);
			this.mt_Description.Location = new Point(0, 115);
			this.mt_Description.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Description.Text = "Description:";
			this.m_pDescription = new TextBox();
			this.m_pDescription.Size = new Size(310, 20);
			this.m_pDescription.Location = new Point(125, 115);
			this.mt_RootFolderType = new Label();
			this.mt_RootFolderType.Size = new Size(120, 20);
			this.mt_RootFolderType.Location = new Point(0, 140);
			this.mt_RootFolderType.TextAlign = ContentAlignment.MiddleRight;
			this.mt_RootFolderType.Text = "Root Folder Type:";
			this.m_pRootFolderType = new ComboBox();
			this.m_pRootFolderType.Size = new Size(200, 20);
			this.m_pRootFolderType.Location = new Point(125, 140);
			this.m_pRootFolderType.DropDownStyle = ComboBoxStyle.DropDownList;
			this.m_pRootFolderType.SelectedIndexChanged += new EventHandler(this.m_pRootFolderType_SelectedIndexChanged);
			this.m_pRootFolderType.Items.Add(new WComboBoxItem("Bounded Root Folder", SharedFolderRootType.BoundedRootFolder));
			this.m_pRootFolderType.Items.Add(new WComboBoxItem("Users Shared Folder", SharedFolderRootType.UsersSharedFolder));
			this.mt_BoundedUser = new Label();
			this.mt_BoundedUser.Size = new Size(120, 20);
			this.mt_BoundedUser.Location = new Point(0, 170);
			this.mt_BoundedUser.TextAlign = ContentAlignment.MiddleRight;
			this.mt_BoundedUser.Text = "Bounded User:";
			this.m_pBoundedUser = new TextBox();
			this.m_pBoundedUser.Size = new Size(280, 20);
			this.m_pBoundedUser.Location = new Point(125, 170);
			this.m_pBoundedUser.ReadOnly = true;
			this.m_pGetBoundedUser = new Button();
			this.m_pGetBoundedUser.Size = new Size(25, 20);
			this.m_pGetBoundedUser.Location = new Point(410, 170);
			this.m_pGetBoundedUser.Text = "...";
			this.m_pGetBoundedUser.Click += new EventHandler(this.m_pGetBoundedUser_Click);
			this.mt_BoundedFolder = new Label();
			this.mt_BoundedFolder.Size = new Size(120, 20);
			this.mt_BoundedFolder.Location = new Point(0, 195);
			this.mt_BoundedFolder.TextAlign = ContentAlignment.MiddleRight;
			this.mt_BoundedFolder.Text = "Bounded User Folder:";
			this.m_pBoundedFolder = new TextBox();
			this.m_pBoundedFolder.Size = new Size(280, 20);
			this.m_pBoundedFolder.Location = new Point(125, 195);
			this.m_pBoundedFolder.ReadOnly = true;
			this.m_pGetBoundedFolder = new Button();
			this.m_pGetBoundedFolder.Size = new Size(25, 20);
			this.m_pGetBoundedFolder.Location = new Point(410, 195);
			this.m_pGetBoundedFolder.Text = "...";
			this.m_pGetBoundedFolder.Click += new EventHandler(this.m_pGetBoundedFolder_Click);
			this.m_pSeparator2 = new GroupBox();
			this.m_pSeparator2.Size = new Size(432, 2);
			this.m_pSeparator2.Location = new Point(7, 235);
			this.m_pCancel = new Button();
			this.m_pCancel.Size = new Size(70, 21);
			this.m_pCancel.Location = new Point(290, 248);
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			this.m_pCancel.Text = "Cancel";
			this.m_Ok = new Button();
			this.m_Ok.Size = new Size(70, 21);
			this.m_Ok.Location = new Point(365, 248);
			this.m_Ok.Click += new EventHandler(this.m_Ok_Click);
			this.m_Ok.Text = "Ok";
			base.Controls.Add(this.m_pIcon);
			base.Controls.Add(this.mt_Info);
			base.Controls.Add(this.m_pSeparator1);
			base.Controls.Add(this.m_pEnabled);
			base.Controls.Add(this.mt_RootFolderName);
			base.Controls.Add(this.m_pRootFolderName);
			base.Controls.Add(this.mt_Description);
			base.Controls.Add(this.m_pDescription);
			base.Controls.Add(this.mt_RootFolderType);
			base.Controls.Add(this.m_pRootFolderType);
			base.Controls.Add(this.mt_BoundedUser);
			base.Controls.Add(this.m_pBoundedUser);
			base.Controls.Add(this.m_pGetBoundedUser);
			base.Controls.Add(this.mt_BoundedFolder);
			base.Controls.Add(this.m_pBoundedFolder);
			base.Controls.Add(this.m_pGetBoundedFolder);
			base.Controls.Add(this.m_pSeparator2);
			base.Controls.Add(this.m_pCancel);
			base.Controls.Add(this.m_Ok);
		}

		private void m_pRootFolderType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pRootFolderType.SelectedItem.ToString() == "Bounded Root Folder")
			{
				this.mt_BoundedUser.Visible = true;
				this.m_pBoundedUser.Visible = true;
				this.m_pGetBoundedUser.Visible = true;
				this.mt_BoundedFolder.Visible = true;
				this.m_pBoundedFolder.Visible = true;
				this.m_pGetBoundedFolder.Visible = true;
				return;
			}
			if (this.m_pRootFolderType.SelectedItem.ToString() == "Users Shared Folder")
			{
				this.mt_BoundedUser.Visible = false;
				this.m_pBoundedUser.Visible = false;
				this.m_pGetBoundedUser.Visible = false;
				this.mt_BoundedFolder.Visible = false;
				this.m_pBoundedFolder.Visible = false;
				this.m_pGetBoundedFolder.Visible = false;
			}
		}

		private void m_pGetBoundedUser_Click(object sender, EventArgs e)
		{
			SelectUserOrGroupForm selectUserOrGroupForm = new SelectUserOrGroupForm(this.m_pVirtualServer, false, false);
			if (selectUserOrGroupForm.ShowDialog(this) == DialogResult.OK)
			{
				this.m_pBoundedUser.Text = selectUserOrGroupForm.SelectedUserOrGroup;
				this.m_pBoundedFolder.Text = "";
			}
		}

		private void m_pGetBoundedFolder_Click(object sender, EventArgs e)
		{
			if (this.m_pBoundedUser.Text == "")
			{
				MessageBox.Show(this, "Please select bounded user !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			SelectUserFolderForm selectUserFolderForm = new SelectUserFolderForm(this.m_pVirtualServer, this.m_pBoundedUser.Text);
			if (selectUserFolderForm.ShowDialog(this) == DialogResult.OK)
			{
				this.m_pBoundedFolder.Text = selectUserFolderForm.SelectedFolder;
			}
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
		}

		private void m_Ok_Click(object sender, EventArgs e)
		{
			SharedFolderRootType sharedFolderRootType_enum = (SharedFolderRootType)((WComboBoxItem)this.m_pRootFolderType.SelectedItem).Tag;
			if (sharedFolderRootType_enum == SharedFolderRootType.BoundedRootFolder && this.m_pBoundedUser.Text == "")
			{
				MessageBox.Show(this, "Please select bounded user !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (sharedFolderRootType_enum == SharedFolderRootType.BoundedRootFolder && this.m_pBoundedFolder.Text == "")
			{
				MessageBox.Show(this, "Please select bounded folder !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (this.m_pRootFolder == null)
			{
				this.m_pRootFolder = this.m_pVirtualServer.RootFolders.Add(this.m_pEnabled.Checked, this.m_pRootFolderName.Text, this.m_pDescription.Text, (SharedFolderRootType)((WComboBoxItem)this.m_pRootFolderType.SelectedItem).Tag, this.m_pBoundedUser.Text, this.m_pBoundedFolder.Text);
			}
			else
			{
				this.m_pRootFolder.Enabled = this.m_pEnabled.Checked;
				this.m_pRootFolder.Name = this.m_pRootFolderName.Text;
				this.m_pRootFolder.Description = this.m_pDescription.Text;
				this.m_pRootFolder.Type = (SharedFolderRootType)((WComboBoxItem)this.m_pRootFolderType.SelectedItem).Tag;
				this.m_pRootFolder.BoundedUser = this.m_pBoundedUser.Text;
				this.m_pRootFolder.BoundedFolder = this.m_pBoundedFolder.Text;
				this.m_pRootFolder.Commit();
			}
			base.DialogResult = DialogResult.OK;
		}
	}
}
