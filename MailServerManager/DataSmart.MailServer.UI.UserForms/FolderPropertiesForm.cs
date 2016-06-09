using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System.NetworkToolkit.IMAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI.UserForms
{
	public class FolderPropertiesForm : Form
	{
		private TabControl m_pTab;

		private Button m_pClose;

		private PictureBox m_pTab_General_Icon;

		private TextBox m_pTab_General_FolderName;

		private GroupBox m_pTab_General_Separator1;

		private Label mt_Tab_General_Path;

		private Label m_pTab_General_Path;

		private Label mt_Tab_General_Size;

		private Label m_pTab_General_Size;

		private Label mt_Tab_General_Contains;

		private Label m_pTab_General_Contains;

		private GroupBox m_pTab_General_Separator2;

		private Label mt_Tab_General_Created;

		private Label m_pTab_General_Created;

		private GroupBox m_pTab_General_Separator3;

		private PictureBox m_pTab_Sharing_Icon;

		private Label mt_Tab_Sharing_Info;

		private GroupBox m_pTab_Sharing_Separator1;

		private RadioButton m_pTab_Sharing_DontShare;

		private RadioButton m_pTab_Sharing_Share;

		private TextBox m_pTab_Sharing_ShareName;

		private PictureBox m_pTab_Security_Icon;

		private Label mt_Tab_Security_Info;

		private GroupBox m_pTab_Security_Separator1;

		private CheckBox m_pTab_Security_InheritAcl;

		private ToolStrip mt_Tab_Security_UsersOrGroupsToolbar;

		private Label mt_Tab_Security_UsersOrGroups;

		private ListView m_pTab_Security_UsersOrGroups;

		private Label mt_Tab_Security_Permissions;

		private ListView m_pTab_Security_Permissions;

		private VirtualServer m_pVirtualServer;

		private UserFolder m_pFolder;

		private string m_ShareName = "";

		private bool m_EvensLocked;

		public FolderPropertiesForm(VirtualServer virtualServer, UserFolder folder)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pFolder = folder;
			this.InitializeComponent();
			this.LoadData();
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(392, 423);
			base.StartPosition = FormStartPosition.CenterScreen;
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			this.Text = "Folder '' Properties";
			base.Icon = ResManager.GetIcon("icon-folder.ico");
			base.VisibleChanged += new EventHandler(this.wfrm_User_FolderProperties_VisibleChanged);
			this.m_pTab = new TabControl();
			this.m_pTab.Size = new Size(385, 385);
			this.m_pTab.Location = new Point(5, 5);
			this.m_pTab.TabPages.Add(new TabPage("General"));
			this.m_pTab.TabPages.Add(new TabPage("Sharing"));
			this.m_pTab.TabPages.Add(new TabPage("Security"));
			this.m_pClose = new Button();
			this.m_pClose.Size = new Size(70, 20);
			this.m_pClose.Location = new Point(320, 400);
			this.m_pClose.Text = "Close";
			this.m_pClose.Click += new EventHandler(this.m_pClose_Click);
			base.Controls.Add(this.m_pTab);
			base.Controls.Add(this.m_pClose);
			this.m_pTab_General_Icon = new PictureBox();
			this.m_pTab_General_Icon.Size = new Size(32, 32);
			this.m_pTab_General_Icon.Location = new Point(10, 10);
			this.m_pTab_General_Icon.Image = ResManager.GetIcon("folder32.ico").ToBitmap();
			this.m_pTab_General_FolderName = new TextBox();
			this.m_pTab_General_FolderName.Size = new Size(270, 20);
			this.m_pTab_General_FolderName.Location = new Point(100, 17);
			this.m_pTab_General_FolderName.ReadOnly = true;
			this.m_pTab_General_Separator1 = new GroupBox();
			this.m_pTab_General_Separator1.Size = new Size(365, 3);
			this.m_pTab_General_Separator1.Location = new Point(7, 50);
			this.mt_Tab_General_Path = new Label();
			this.mt_Tab_General_Path.Size = new Size(95, 20);
			this.mt_Tab_General_Path.Location = new Point(10, 65);
			this.mt_Tab_General_Path.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Tab_General_Path.Text = "Path:";
			this.m_pTab_General_Path = new Label();
			this.m_pTab_General_Path.Size = new Size(200, 20);
			this.m_pTab_General_Path.Location = new Point(105, 65);
			this.m_pTab_General_Path.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Tab_General_Size = new Label();
			this.mt_Tab_General_Size.Size = new Size(95, 20);
			this.mt_Tab_General_Size.Location = new Point(10, 90);
			this.mt_Tab_General_Size.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Tab_General_Size.Text = "Size:";
			this.m_pTab_General_Size = new Label();
			this.m_pTab_General_Size.Size = new Size(200, 20);
			this.m_pTab_General_Size.Location = new Point(105, 90);
			this.m_pTab_General_Size.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Tab_General_Contains = new Label();
			this.mt_Tab_General_Contains.Size = new Size(95, 20);
			this.mt_Tab_General_Contains.Location = new Point(10, 115);
			this.mt_Tab_General_Contains.Text = "Contains:";
			this.m_pTab_General_Contains = new Label();
			this.m_pTab_General_Contains.Size = new Size(270, 20);
			this.m_pTab_General_Contains.Location = new Point(105, 115);
			this.m_pTab_General_Contains.TextAlign = ContentAlignment.MiddleLeft;
			this.m_pTab_General_Separator2 = new GroupBox();
			this.m_pTab_General_Separator2.Size = new Size(365, 3);
			this.m_pTab_General_Separator2.Location = new Point(7, 140);
			this.mt_Tab_General_Created = new Label();
			this.mt_Tab_General_Created.Size = new Size(95, 20);
			this.mt_Tab_General_Created.Location = new Point(10, 150);
			this.mt_Tab_General_Created.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Tab_General_Created.Text = "Created:";
			this.m_pTab_General_Created = new Label();
			this.m_pTab_General_Created.Size = new Size(270, 20);
			this.m_pTab_General_Created.Location = new Point(105, 150);
			this.m_pTab_General_Created.TextAlign = ContentAlignment.MiddleLeft;
			this.m_pTab_General_Separator3 = new GroupBox();
			this.m_pTab_General_Separator3.Size = new Size(365, 3);
			this.m_pTab_General_Separator3.Location = new Point(7, 180);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Icon);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_FolderName);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Separator1);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_Tab_General_Path);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Path);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_Tab_General_Size);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Size);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_Tab_General_Contains);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Contains);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Separator2);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_Tab_General_Created);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Created);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Separator3);
			this.m_pTab_Sharing_Icon = new PictureBox();
			this.m_pTab_Sharing_Icon.Size = new Size(32, 32);
			this.m_pTab_Sharing_Icon.Location = new Point(10, 10);
			this.m_pTab_Sharing_Icon.Image = ResManager.GetIcon("share32.ico").ToBitmap();
			this.mt_Tab_Sharing_Info = new Label();
			this.mt_Tab_Sharing_Info.Size = new Size(200, 32);
			this.mt_Tab_Sharing_Info.Location = new Point(50, 10);
			this.mt_Tab_Sharing_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Tab_Sharing_Info.Text = "Specify sharing information.";
			this.m_pTab_Sharing_Separator1 = new GroupBox();
			this.m_pTab_Sharing_Separator1.Size = new Size(365, 3);
			this.m_pTab_Sharing_Separator1.Location = new Point(7, 50);
			this.m_pTab_Sharing_DontShare = new RadioButton();
			this.m_pTab_Sharing_DontShare.Size = new Size(200, 20);
			this.m_pTab_Sharing_DontShare.Location = new Point(20, 65);
			this.m_pTab_Sharing_DontShare.Text = "Do not share this folder";
			this.m_pTab_Sharing_DontShare.CheckedChanged += new EventHandler(this.m_pTab_Sharing_Share_CheckedChanged);
			this.m_pTab_Sharing_Share = new RadioButton();
			this.m_pTab_Sharing_Share.Size = new Size(200, 20);
			this.m_pTab_Sharing_Share.Location = new Point(20, 85);
			this.m_pTab_Sharing_Share.Text = "Share this folder";
			this.m_pTab_Sharing_Share.CheckedChanged += new EventHandler(this.m_pTab_Sharing_Share_CheckedChanged);
			this.m_pTab_Sharing_ShareName = new TextBox();
			this.m_pTab_Sharing_ShareName.Size = new Size(330, 20);
			this.m_pTab_Sharing_ShareName.Location = new Point(40, 110);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pTab_Sharing_Icon);
			this.m_pTab.TabPages[1].Controls.Add(this.mt_Tab_Sharing_Info);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pTab_Sharing_Separator1);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pTab_Sharing_DontShare);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pTab_Sharing_Share);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pTab_Sharing_ShareName);
			this.m_pTab_Security_Icon = new PictureBox();
			this.m_pTab_Security_Icon.Size = new Size(32, 32);
			this.m_pTab_Security_Icon.Location = new Point(10, 10);
			this.m_pTab_Security_Icon.Image = ResManager.GetIcon("security32.ico").ToBitmap();
			this.mt_Tab_Security_Info = new Label();
			this.mt_Tab_Security_Info.Size = new Size(200, 32);
			this.mt_Tab_Security_Info.Location = new Point(50, 10);
			this.mt_Tab_Security_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Tab_Security_Info.Text = "Specify folder permissions.";
			this.m_pTab_Security_Separator1 = new GroupBox();
			this.m_pTab_Security_Separator1.Size = new Size(365, 3);
			this.m_pTab_Security_Separator1.Location = new Point(7, 50);
			this.m_pTab_Security_InheritAcl = new CheckBox();
			this.m_pTab_Security_InheritAcl.Size = new Size(300, 20);
			this.m_pTab_Security_InheritAcl.Location = new Point(10, 60);
			this.m_pTab_Security_InheritAcl.Text = "Inherit permissions from parent folder.";
			this.m_pTab_Security_InheritAcl.CheckStateChanged += new EventHandler(this.m_pTab_Security_InheritAcl_CheckStateChanged);
			this.mt_Tab_Security_UsersOrGroups = new Label();
			this.mt_Tab_Security_UsersOrGroups.Size = new Size(200, 20);
			this.mt_Tab_Security_UsersOrGroups.Location = new Point(10, 85);
			this.mt_Tab_Security_UsersOrGroups.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Tab_Security_UsersOrGroups.Text = "Group or user names:";
			this.mt_Tab_Security_UsersOrGroupsToolbar = new ToolStrip();
			this.mt_Tab_Security_UsersOrGroupsToolbar.Location = new Point(315, 80);
			this.mt_Tab_Security_UsersOrGroupsToolbar.Dock = DockStyle.None;
			this.mt_Tab_Security_UsersOrGroupsToolbar.GripStyle = ToolStripGripStyle.Hidden;
			this.mt_Tab_Security_UsersOrGroupsToolbar.BackColor = this.BackColor;
			this.mt_Tab_Security_UsersOrGroupsToolbar.Renderer = new ToolBarRendererEx();
			ToolStripButton toolStripButton = new ToolStripButton(ResManager.GetIcon("add.ico").ToBitmap());
			toolStripButton.Tag = "add";
			this.mt_Tab_Security_UsersOrGroupsToolbar.Items.Add(toolStripButton);
			ToolStripButton toolStripButton2 = new ToolStripButton(ResManager.GetIcon("delete.ico").ToBitmap());
			toolStripButton2.Tag = "delete";
			this.mt_Tab_Security_UsersOrGroupsToolbar.Items.Add(toolStripButton2);
			this.mt_Tab_Security_UsersOrGroupsToolbar.ItemClicked += new ToolStripItemClickedEventHandler(this.mt_Tab_Security_UsersOrGroupsToolbar_ItemClicked);
			ImageList imageList = new ImageList();
			imageList.Images.Add(ResManager.GetIcon("user.ico"));
			imageList.Images.Add(ResManager.GetIcon("group.ico"));
			this.m_pTab_Security_UsersOrGroups = new ListView();
			this.m_pTab_Security_UsersOrGroups.Size = new Size(355, 125);
			this.m_pTab_Security_UsersOrGroups.Location = new Point(10, 105);
			this.m_pTab_Security_UsersOrGroups.View = View.List;
			this.m_pTab_Security_UsersOrGroups.HideSelection = false;
			this.m_pTab_Security_UsersOrGroups.FullRowSelect = true;
			this.m_pTab_Security_UsersOrGroups.SmallImageList = imageList;
			this.m_pTab_Security_UsersOrGroups.Columns.Add("", 300, HorizontalAlignment.Left);
			this.m_pTab_Security_UsersOrGroups.SelectedIndexChanged += new EventHandler(this.m_pTab_Security_UsersOrGroups_SelectedIndexChanged);
			this.mt_Tab_Security_Permissions = new Label();
			this.mt_Tab_Security_Permissions.Size = new Size(195, 20);
			this.mt_Tab_Security_Permissions.Location = new Point(10, 235);
			this.mt_Tab_Security_Permissions.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Tab_Security_Permissions.Text = "Permissions:";
			this.m_pTab_Security_Permissions = new ListView();
			this.m_pTab_Security_Permissions.Size = new Size(355, 95);
			this.m_pTab_Security_Permissions.Location = new Point(10, 255);
			this.m_pTab_Security_Permissions.View = View.List;
			this.m_pTab_Security_Permissions.HideSelection = false;
			this.m_pTab_Security_Permissions.FullRowSelect = true;
			this.m_pTab_Security_Permissions.CheckBoxes = true;
			this.m_pTab_Security_Permissions.Columns.Add("", 145, HorizontalAlignment.Left);
			this.m_pTab_Security_Permissions.Items.Add("Administer");
			this.m_pTab_Security_Permissions.Items.Add("List Folders");
			this.m_pTab_Security_Permissions.Items.Add("Create Folder");
			this.m_pTab_Security_Permissions.Items.Add("Read");
			this.m_pTab_Security_Permissions.Items.Add("Write");
			this.m_pTab_Security_Permissions.Items.Add("Delete Messages");
			this.m_pTab_Security_Permissions.Items.Add("Store Flags");
			this.m_pTab_Security_Permissions.ItemCheck += new ItemCheckEventHandler(this.m_pTab_Security_Permissions_ItemCheck);
			this.m_pTab.TabPages[2].Controls.Add(this.m_pTab_Security_Icon);
			this.m_pTab.TabPages[2].Controls.Add(this.mt_Tab_Security_Info);
			this.m_pTab.TabPages[2].Controls.Add(this.m_pTab_Security_Separator1);
			this.m_pTab.TabPages[2].Controls.Add(this.m_pTab_Security_InheritAcl);
			this.m_pTab.TabPages[2].Controls.Add(this.mt_Tab_Security_UsersOrGroupsToolbar);
			this.m_pTab.TabPages[2].Controls.Add(this.mt_Tab_Security_UsersOrGroups);
			this.m_pTab.TabPages[2].Controls.Add(this.m_pTab_Security_UsersOrGroups);
			this.m_pTab.TabPages[2].Controls.Add(this.mt_Tab_Security_Permissions);
			this.m_pTab.TabPages[2].Controls.Add(this.m_pTab_Security_Permissions);
		}

		private void m_pTab_Security_Permissions_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (this.m_EvensLocked)
			{
				return;
			}
			if (this.m_pTab_Security_UsersOrGroups.SelectedItems.Count > 0)
			{
				IMAP_ACL_Flags iMAP_ACL_Flags = IMAP_ACL_Flags.None;
				if ((e.Index == 0 && e.NewValue == CheckState.Checked) || (e.Index != 0 && this.m_pTab_Security_Permissions.Items[0].Checked))
				{
					iMAP_ACL_Flags |= IMAP_ACL_Flags.a;
				}
				if ((e.Index == 1 && e.NewValue == CheckState.Checked) || (e.Index != 1 && this.m_pTab_Security_Permissions.Items[1].Checked))
				{
					iMAP_ACL_Flags |= IMAP_ACL_Flags.l;
				}
				if ((e.Index == 2 && e.NewValue == CheckState.Checked) || (e.Index != 2 && this.m_pTab_Security_Permissions.Items[2].Checked))
				{
					iMAP_ACL_Flags |= IMAP_ACL_Flags.c;
				}
				if ((e.Index == 3 && e.NewValue == CheckState.Checked) || (e.Index != 3 && this.m_pTab_Security_Permissions.Items[3].Checked))
				{
					iMAP_ACL_Flags |= IMAP_ACL_Flags.r;
				}
				if ((e.Index == 4 && e.NewValue == CheckState.Checked) || (e.Index != 4 && this.m_pTab_Security_Permissions.Items[4].Checked))
				{
					iMAP_ACL_Flags |= (IMAP_ACL_Flags)48;
				}
				if ((e.Index == 5 && e.NewValue == CheckState.Checked) || (e.Index != 5 && this.m_pTab_Security_Permissions.Items[5].Checked))
				{
					iMAP_ACL_Flags |= IMAP_ACL_Flags.d;
				}
				if ((e.Index == 6 && e.NewValue == CheckState.Checked) || (e.Index != 6 && this.m_pTab_Security_Permissions.Items[6].Checked))
				{
					iMAP_ACL_Flags |= (IMAP_ACL_Flags)12;
				}
				UserFolderAcl userFolderAcl = (UserFolderAcl)this.m_pTab_Security_UsersOrGroups.SelectedItems[0].Tag;
				if (userFolderAcl.Permissions != iMAP_ACL_Flags)
				{
					userFolderAcl.Permissions = iMAP_ACL_Flags;
				}
			}
		}

		private void wfrm_User_FolderProperties_VisibleChanged(object sender, EventArgs e)
		{
			if (base.Visible)
			{
				return;
			}
			try
			{
				foreach (UserFolderAcl userFolderAcl in this.m_pFolder.ACL)
				{
					userFolderAcl.Commit();
				}
				if (this.m_pTab_Sharing_DontShare.Checked && this.m_ShareName != "")
				{
					SharedRootFolder rootFolderByName = this.m_pVirtualServer.RootFolders.GetRootFolderByName(this.m_ShareName);
					rootFolderByName.Owner.Remove(rootFolderByName);
				}
				else if (this.m_pTab_Sharing_Share.Checked && this.m_ShareName != this.m_pTab_Sharing_ShareName.Text)
				{
					if (this.m_pVirtualServer.RootFolders.Contains(this.m_ShareName))
					{
						SharedRootFolder rootFolderByName2 = this.m_pVirtualServer.RootFolders.GetRootFolderByName(this.m_ShareName);
						rootFolderByName2.Enabled = true;
						rootFolderByName2.Name = this.m_pTab_Sharing_ShareName.Text;
						rootFolderByName2.Commit();
					}
					else
					{
						this.m_pVirtualServer.RootFolders.Add(true, this.m_pTab_Sharing_ShareName.Text, "", SharedFolderRootType.BoundedRootFolder, this.m_pFolder.User.UserName, this.m_pFolder.FolderFullPath);
					}
				}
			}
			catch (Exception x)
			{
				ErrorForm errorForm = new ErrorForm(x, new StackTrace());
				errorForm.ShowDialog(null);
			}
		}

		private void wfrm_User_FolderProperties_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				if (this.m_pTab_Security_UsersOrGroups.SelectedItems.Count > 0)
				{
					IMAP_ACL_Flags iMAP_ACL_Flags = IMAP_ACL_Flags.None;
					if (this.m_pTab_Security_Permissions.Items[0].Checked)
					{
						iMAP_ACL_Flags |= IMAP_ACL_Flags.a;
					}
					if (this.m_pTab_Security_Permissions.Items[1].Checked)
					{
						iMAP_ACL_Flags |= IMAP_ACL_Flags.l;
					}
					if (this.m_pTab_Security_Permissions.Items[2].Checked)
					{
						iMAP_ACL_Flags |= IMAP_ACL_Flags.c;
					}
					if (this.m_pTab_Security_Permissions.Items[3].Checked)
					{
						iMAP_ACL_Flags |= IMAP_ACL_Flags.r;
					}
					if (this.m_pTab_Security_Permissions.Items[4].Checked)
					{
						iMAP_ACL_Flags |= (IMAP_ACL_Flags)48;
					}
					if (this.m_pTab_Security_Permissions.Items[5].Checked)
					{
						iMAP_ACL_Flags |= IMAP_ACL_Flags.d;
					}
					if (this.m_pTab_Security_Permissions.Items[6].Checked)
					{
						iMAP_ACL_Flags |= (IMAP_ACL_Flags)12;
					}
					UserFolderAcl userFolderAcl = (UserFolderAcl)this.m_pTab_Security_UsersOrGroups.SelectedItems[0].Tag;
					if (userFolderAcl.Permissions != iMAP_ACL_Flags)
					{
						userFolderAcl.Permissions = iMAP_ACL_Flags;
						userFolderAcl.Commit();
					}
				}
				if (this.m_pTab_Sharing_DontShare.Checked && this.m_ShareName != "")
				{
					SharedRootFolder rootFolderByName = this.m_pVirtualServer.RootFolders.GetRootFolderByName(this.m_ShareName);
					rootFolderByName.Owner.Remove(rootFolderByName);
				}
				else if (this.m_pTab_Sharing_Share.Checked && this.m_ShareName != this.m_pTab_Sharing_ShareName.Text)
				{
					if (this.m_pVirtualServer.RootFolders.Contains(this.m_ShareName))
					{
						SharedRootFolder rootFolderByName2 = this.m_pVirtualServer.RootFolders.GetRootFolderByName(this.m_ShareName);
						rootFolderByName2.Enabled = true;
						rootFolderByName2.Name = this.m_pTab_Sharing_ShareName.Text;
						rootFolderByName2.Commit();
					}
					else
					{
						this.m_pVirtualServer.RootFolders.Add(true, this.m_pTab_Sharing_ShareName.Text, "", SharedFolderRootType.BoundedRootFolder, this.m_pFolder.User.UserName, this.m_pFolder.FolderFullPath);
					}
				}
			}
			catch (Exception x)
			{
				ErrorForm errorForm = new ErrorForm(x, new StackTrace());
				errorForm.ShowDialog(null);
				if (MessageBox.Show(this, "Do you want to reconfigure ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					e.Cancel = true;
				}
			}
		}

		private void m_pClose_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void m_pTab_Sharing_Share_CheckedChanged(object sender, EventArgs e)
		{
			if (this.m_pTab_Sharing_Share.Checked)
			{
				this.m_pTab_Sharing_ShareName.Enabled = true;
				return;
			}
			this.m_pTab_Sharing_ShareName.Enabled = false;
		}

		private void m_pTab_Security_InheritAcl_CheckStateChanged(object sender, EventArgs e)
		{
			if (this.m_EvensLocked)
			{
				return;
			}
			this.m_EvensLocked = true;
			if (this.m_pTab_Security_InheritAcl.Checked)
			{
				string text = "Do you want to inherit all permissions from parent folder and loose current permissions from current folder ?";
				if (MessageBox.Show(this, text, "Warning:", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
				{
					this.mt_Tab_Security_UsersOrGroupsToolbar.Enabled = false;
					this.m_pTab_Security_UsersOrGroups.Enabled = false;
					this.m_pTab_Security_Permissions.Enabled = false;
					foreach (ListViewItem listViewItem in this.m_pTab_Security_UsersOrGroups.Items)
					{
						UserFolderAcl userFolderAcl = (UserFolderAcl)listViewItem.Tag;
						userFolderAcl.Owner.Remove(userFolderAcl);
					}
					this.LoadACL();
				}
				else
				{
					this.m_pTab_Security_InheritAcl.Checked = false;
					this.mt_Tab_Security_UsersOrGroupsToolbar.Enabled = true;
					this.m_pTab_Security_UsersOrGroups.Enabled = true;
					this.m_pTab_Security_Permissions.Enabled = true;
				}
			}
			else
			{
				this.mt_Tab_Security_UsersOrGroupsToolbar.Enabled = true;
				this.m_pTab_Security_UsersOrGroups.Enabled = true;
				this.m_pTab_Security_Permissions.Enabled = true;
				this.m_pTab_Security_UsersOrGroups.Items.Clear();
				this.m_pTab_Security_UsersOrGroups_SelectedIndexChanged(null, null);
			}
			this.m_EvensLocked = false;
		}

		private void mt_Tab_Security_UsersOrGroupsToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag.ToString() == "add")
			{
				List<string> list = new List<string>();
				foreach (ListViewItem listViewItem in this.m_pTab_Security_UsersOrGroups.Items)
				{
					list.Add(listViewItem.Text.ToLower());
				}
				SelectUserOrGroupForm selectUserOrGroupForm = new SelectUserOrGroupForm(this.m_pVirtualServer, false, false, list);
				if (selectUserOrGroupForm.ShowDialog(this) == DialogResult.OK)
				{
					ListViewItem listViewItem2 = new ListViewItem(selectUserOrGroupForm.SelectedUserOrGroup);
					listViewItem2.Tag = this.m_pFolder.ACL.Add(selectUserOrGroupForm.SelectedUserOrGroup, IMAP_ACL_Flags.None);
					if (selectUserOrGroupForm.IsGroup)
					{
						listViewItem2.ImageIndex = 1;
					}
					else
					{
						listViewItem2.ImageIndex = 0;
					}
					listViewItem2.Selected = true;
					this.m_pTab_Security_UsersOrGroups.Items.Add(listViewItem2);
					return;
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "delete")
			{
				ListViewItem listViewItem3 = this.m_pTab_Security_UsersOrGroups.SelectedItems[0];
				if (MessageBox.Show(this, "Are you sure you want to remove '" + listViewItem3.Text + "' permissions on current folder ?", "Confirm Delete:", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					UserFolderAcl userFolderAcl = (UserFolderAcl)listViewItem3.Tag;
					userFolderAcl.Owner.Remove(userFolderAcl);
					listViewItem3.Remove();
				}
			}
		}

		private void m_pTab_Security_UsersOrGroups_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.m_EvensLocked = true;
			if (this.m_pTab_Security_UsersOrGroups.SelectedItems.Count > 0)
			{
				this.mt_Tab_Security_UsersOrGroupsToolbar.Items[1].Enabled = true;
				if (!this.m_pTab_Security_InheritAcl.Checked)
				{
					this.m_pTab_Security_Permissions.Enabled = true;
				}
				ListViewItem listViewItem = this.m_pTab_Security_UsersOrGroups.SelectedItems[0];
				UserFolderAcl userFolderAcl = (UserFolderAcl)listViewItem.Tag;
				if ((userFolderAcl.Permissions & IMAP_ACL_Flags.l) != IMAP_ACL_Flags.None)
				{
					this.m_pTab_Security_Permissions.Items[1].Checked = true;
				}
				if ((userFolderAcl.Permissions & IMAP_ACL_Flags.r) != IMAP_ACL_Flags.None)
				{
					this.m_pTab_Security_Permissions.Items[3].Checked = true;
				}
				if ((userFolderAcl.Permissions & IMAP_ACL_Flags.s) != IMAP_ACL_Flags.None)
				{
					this.m_pTab_Security_Permissions.Items[6].Checked = true;
				}
				if ((userFolderAcl.Permissions & IMAP_ACL_Flags.w) != IMAP_ACL_Flags.None)
				{
					this.m_pTab_Security_Permissions.Items[6].Checked = true;
				}
				if ((userFolderAcl.Permissions & IMAP_ACL_Flags.i) != IMAP_ACL_Flags.None)
				{
					this.m_pTab_Security_Permissions.Items[4].Checked = true;
				}
				if ((userFolderAcl.Permissions & IMAP_ACL_Flags.p) != IMAP_ACL_Flags.None)
				{
					this.m_pTab_Security_Permissions.Items[4].Checked = true;
				}
				if ((userFolderAcl.Permissions & IMAP_ACL_Flags.c) != IMAP_ACL_Flags.None)
				{
					this.m_pTab_Security_Permissions.Items[2].Checked = true;
				}
				if ((userFolderAcl.Permissions & IMAP_ACL_Flags.d) != IMAP_ACL_Flags.None)
				{
					this.m_pTab_Security_Permissions.Items[5].Checked = true;
				}
				if ((userFolderAcl.Permissions & IMAP_ACL_Flags.a) != IMAP_ACL_Flags.None)
				{
					this.m_pTab_Security_Permissions.Items[0].Checked = true;
				}
			}
			else
			{
				this.mt_Tab_Security_UsersOrGroupsToolbar.Items[1].Enabled = false;
				this.m_pTab_Security_Permissions.Enabled = false;
				foreach (ListViewItem listViewItem2 in this.m_pTab_Security_Permissions.Items)
				{
					listViewItem2.Checked = false;
				}
			}
			this.m_EvensLocked = false;
		}

		private void m_pTab_Security_Permissions_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			if (this.m_EvensLocked)
			{
				return;
			}
			if (this.m_pTab_Security_UsersOrGroups.SelectedItems.Count > 0)
			{
				IMAP_ACL_Flags iMAP_ACL_Flags = IMAP_ACL_Flags.None;
				if (this.m_pTab_Security_Permissions.Items[0].Checked)
				{
					iMAP_ACL_Flags |= IMAP_ACL_Flags.a;
				}
				if (this.m_pTab_Security_Permissions.Items[1].Checked)
				{
					iMAP_ACL_Flags |= IMAP_ACL_Flags.l;
				}
				if (this.m_pTab_Security_Permissions.Items[2].Checked)
				{
					iMAP_ACL_Flags |= IMAP_ACL_Flags.c;
				}
				if (this.m_pTab_Security_Permissions.Items[3].Checked)
				{
					iMAP_ACL_Flags |= IMAP_ACL_Flags.r;
				}
				if (this.m_pTab_Security_Permissions.Items[4].Checked)
				{
					iMAP_ACL_Flags |= (IMAP_ACL_Flags)48;
				}
				if (this.m_pTab_Security_Permissions.Items[5].Checked)
				{
					iMAP_ACL_Flags |= IMAP_ACL_Flags.d;
				}
				if (this.m_pTab_Security_Permissions.Items[6].Checked)
				{
					iMAP_ACL_Flags |= (IMAP_ACL_Flags)12;
				}
				UserFolderAcl userFolderAcl = (UserFolderAcl)this.m_pTab_Security_UsersOrGroups.SelectedItems[0].Tag;
				if (userFolderAcl.Permissions != iMAP_ACL_Flags)
				{
					userFolderAcl.Permissions = iMAP_ACL_Flags;
				}
			}
		}

		private void LoadData()
		{
			this.Text = "Folder '" + this.m_pFolder.FolderName + "' Properties";
			this.m_pTab_General_FolderName.Text = this.m_pFolder.FolderName;
			this.m_pTab_General_Path.Text = this.m_pFolder.FolderFullPath;
			this.m_pTab_General_Size.Text = (this.m_pFolder.SizeUsed / 1000000m).ToString("f2") + " MB";
			this.m_pTab_General_Contains.Text = string.Concat(new object[]
			{
				this.m_pFolder.MessagesCount,
				" Messages, ",
				this.m_pFolder.ChildFolders.Count,
				" Folders"
			});
			this.m_pTab_General_Created.Text = this.m_pFolder.CreationTime.ToLongDateString() + " " + this.m_pFolder.CreationTime.ToLongTimeString();
			this.m_pTab_Sharing_DontShare.Checked = true;
			foreach (SharedRootFolder sharedRootFolder in this.m_pVirtualServer.RootFolders)
			{
				if (sharedRootFolder.Type == SharedFolderRootType.BoundedRootFolder && sharedRootFolder.BoundedUser == this.m_pFolder.User.UserName && sharedRootFolder.BoundedFolder == this.m_pFolder.FolderFullPath)
				{
					this.m_ShareName = sharedRootFolder.Name;
					this.m_pTab_Sharing_ShareName.Text = sharedRootFolder.Name;
					this.m_pTab_Sharing_Share.Checked = true;
					break;
				}
			}
			this.LoadACL();
		}

		private void LoadACL()
		{
			this.m_EvensLocked = true;
			this.m_pTab_Security_UsersOrGroups.Items.Clear();
			bool flag = false;
			if (this.m_pFolder.ACL.Count > 0)
			{
				foreach (UserFolderAcl userFolderAcl in this.m_pFolder.ACL)
				{
					ListViewItem listViewItem = new ListViewItem(userFolderAcl.UserOrGroup);
					listViewItem.SubItems.Add(IMAP_Utils.ACL_to_String(userFolderAcl.Permissions));
					if (!this.m_pVirtualServer.Groups.Contains(userFolderAcl.UserOrGroup))
					{
						listViewItem.ImageIndex = 0;
					}
					else
					{
						listViewItem.ImageIndex = 1;
					}
					listViewItem.Tag = userFolderAcl;
					this.m_pTab_Security_UsersOrGroups.Items.Add(listViewItem);
				}
				flag = true;
			}
			else
			{
				UserFolder userFolder = this.m_pFolder;
				while (userFolder.Parent != null)
				{
					userFolder = userFolder.Parent;
					if (userFolder.ACL.Count > 0)
					{
						IEnumerator enumerator2 = userFolder.ACL.GetEnumerator();
						try
						{
							while (enumerator2.MoveNext())
							{
								UserFolderAcl userFolderAcl2 = (UserFolderAcl)enumerator2.Current;
								ListViewItem listViewItem2 = new ListViewItem(userFolderAcl2.UserOrGroup);
								listViewItem2.SubItems.Add(IMAP_Utils.ACL_to_String(userFolderAcl2.Permissions));
								if (!this.m_pVirtualServer.Groups.Contains(userFolderAcl2.UserOrGroup))
								{
									listViewItem2.ImageIndex = 0;
								}
								else
								{
									listViewItem2.ImageIndex = 1;
								}
								listViewItem2.Tag = userFolderAcl2;
								this.m_pTab_Security_UsersOrGroups.Items.Add(listViewItem2);
							}
							break;
						}
						finally
						{
							IDisposable disposable2 = enumerator2 as IDisposable;
							if (disposable2 != null)
							{
								disposable2.Dispose();
							}
						}
					}
				}
			}
			if (!flag)
			{
				this.m_pTab_Security_InheritAcl.Checked = true;
				this.mt_Tab_Security_UsersOrGroupsToolbar.Enabled = false;
				this.m_pTab_Security_Permissions.Enabled = false;
			}
			else
			{
				this.m_pTab_Security_InheritAcl.Checked = false;
				this.mt_Tab_Security_UsersOrGroupsToolbar.Enabled = true;
				this.m_pTab_Security_Permissions.Enabled = true;
				this.m_pTab_Security_UsersOrGroups_SelectedIndexChanged(null, null);
			}
			this.m_EvensLocked = false;
		}
	}
}
