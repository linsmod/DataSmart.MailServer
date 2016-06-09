using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class UsersDefaultFoldersForm : Form
	{
		private ToolStrip m_pToolbar;

		private ListView m_pFolders;

		private VirtualServer m_pVirtualServer;

		public UsersDefaultFoldersForm(VirtualServer virtualServer, WFrame frame)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			frame.Frame_ToolStrip = this.m_pToolbar;
			this.LoadFolders("");
		}

		private void InitializeComponent()
		{
			base.Size = new Size(400, 300);
			this.m_pToolbar = new ToolStrip();
			this.m_pToolbar.GripStyle = ToolStripGripStyle.Hidden;
			this.m_pToolbar.BackColor = this.BackColor;
			this.m_pToolbar.Renderer = new ToolBarRendererEx();
			this.m_pToolbar.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pToolbar_ItemClicked);
			ToolStripButton toolStripButton = new ToolStripButton();
			toolStripButton.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripButton.Tag = "add";
			this.m_pToolbar.Items.Add(toolStripButton);
			ToolStripButton toolStripButton2 = new ToolStripButton();
			toolStripButton2.Enabled = false;
			toolStripButton2.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			toolStripButton2.Tag = "delete";
			this.m_pToolbar.Items.Add(toolStripButton2);
			ImageList imageList = new ImageList();
			imageList.Images.Add(ResManager.GetIcon("icon-folder.ico"));
			this.m_pFolders = new ListView();
			this.m_pFolders.Size = new Size(375, 230);
			this.m_pFolders.Location = new Point(10, 20);
			this.m_pFolders.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pFolders.View = View.Details;
			this.m_pFolders.HideSelection = false;
			this.m_pFolders.FullRowSelect = true;
			this.m_pFolders.SmallImageList = imageList;
			this.m_pFolders.SelectedIndexChanged += new EventHandler(this.m_pFolders_SelectedIndexChanged);
			this.m_pFolders.Columns.Add("Folder", 200, HorizontalAlignment.Left);
			this.m_pFolders.Columns.Add("Permanent", 65, HorizontalAlignment.Left);
			base.Controls.Add(this.m_pFolders);
		}

		private void m_pToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			if (e.ClickedItem.Tag.ToString() == "add")
			{
				UsersDefaultFolderForm usersDefaultFolderForm = new UsersDefaultFolderForm(this.m_pVirtualServer);
				if (usersDefaultFolderForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadFolders(usersDefaultFolderForm.FolderName);
					return;
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "delete")
			{
				UsersDefaultFolder usersDefaultFolder = (UsersDefaultFolder)this.m_pFolders.SelectedItems[0].Tag;
				if (usersDefaultFolder.FolderName.ToLower() == "inbox")
				{
					MessageBox.Show(this, "Inbox is permanent system folder and can't be deleted ! '", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				if (MessageBox.Show(this, "Are you sure you want to delete Users Default Folder '" + usersDefaultFolder.FolderName + "' !", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					usersDefaultFolder.Owner.Remove(usersDefaultFolder);
					this.LoadFolders("");
				}
			}
		}

		private void m_pFolders_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pFolders.SelectedItems.Count > 0)
			{
				this.m_pToolbar.Items[1].Enabled = true;
				return;
			}
			this.m_pToolbar.Items[1].Enabled = false;
		}

		private void LoadFolders(string selectedFolderName)
		{
			this.m_pFolders.Items.Clear();
			foreach (UsersDefaultFolder usersDefaultFolder in this.m_pVirtualServer.UsersDefaultFolders)
			{
				ListViewItem listViewItem = new ListViewItem(usersDefaultFolder.FolderName);
				listViewItem.ImageIndex = 0;
				listViewItem.SubItems.Add(usersDefaultFolder.Permanent.ToString());
				listViewItem.Tag = usersDefaultFolder;
				this.m_pFolders.Items.Add(listViewItem);
				if (usersDefaultFolder.FolderName.ToLower() == selectedFolderName.ToLower())
				{
					listViewItem.Selected = true;
				}
			}
			this.m_pFolders_SelectedIndexChanged(this, new EventArgs());
		}
	}
}
