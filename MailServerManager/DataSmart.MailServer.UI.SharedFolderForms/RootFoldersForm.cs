using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI.SharedFolderForms
{
	public class RootFoldersForm : Form
	{
		private ToolStrip m_pToolbar;

		private ImageList m_pRootFoldersImages;

		private ListView m_pRootFolders;

		private VirtualServer m_pVirtualServer;

		public RootFoldersForm(VirtualServer virtualServer, WFrame frame)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			frame.Frame_ToolStrip = this.m_pToolbar;
			this.LoadRoots("");
		}

		private void InitializeComponent()
		{
			base.Size = new Size(472, 357);
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
			toolStripButton2.Image = ResManager.GetIcon("edit.ico").ToBitmap();
			toolStripButton2.Tag = "edit";
			this.m_pToolbar.Items.Add(toolStripButton2);
			ToolStripButton toolStripButton3 = new ToolStripButton();
			toolStripButton3.Enabled = false;
			toolStripButton3.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			toolStripButton3.Tag = "delete";
			this.m_pToolbar.Items.Add(toolStripButton3);
			this.m_pRootFoldersImages = new ImageList();
			this.m_pRootFoldersImages.Images.Add(ResManager.GetIcon("icon-folder-share.ico"));
			this.m_pRootFoldersImages.Images.Add(ResManager.GetIcon("rootfolder_disabled.ico"));
			this.m_pRootFolders = new ListView();
			this.m_pRootFolders.Size = new Size(445, 265);
			this.m_pRootFolders.Location = new Point(10, 50);
			this.m_pRootFolders.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pRootFolders.View = View.Details;
			this.m_pRootFolders.HideSelection = false;
			this.m_pRootFolders.FullRowSelect = true;
			this.m_pRootFolders.SmallImageList = this.m_pRootFoldersImages;
			this.m_pRootFolders.SelectedIndexChanged += new EventHandler(this.m_pRootFolders_SelectedIndexChanged);
			this.m_pRootFolders.DoubleClick += new EventHandler(this.m_pRootFolders_DoubleClick);
			this.m_pRootFolders.Columns.Add("Folder Name", 170, HorizontalAlignment.Left);
			this.m_pRootFolders.Columns.Add("Description", 180, HorizontalAlignment.Left);
			this.m_pRootFolders.Columns.Add("Root Type", 120, HorizontalAlignment.Left);
			base.Controls.Add(this.m_pRootFolders);
		}

		private void m_pToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			if (e.ClickedItem.Tag.ToString() == "add")
			{
				AddEditRootFolderForm addEditRootFolderForm = new AddEditRootFolderForm(this.m_pVirtualServer);
				if (addEditRootFolderForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadRoots(addEditRootFolderForm.RootID);
					return;
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "edit")
			{
				SharedRootFolder sharedRootFolder = (SharedRootFolder)this.m_pRootFolders.SelectedItems[0].Tag;
				AddEditRootFolderForm addEditRootFolderForm2 = new AddEditRootFolderForm(this.m_pVirtualServer, sharedRootFolder);
				if (addEditRootFolderForm2.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadRoots(sharedRootFolder.ID);
					return;
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "delete")
			{
				SharedRootFolder sharedRootFolder2 = (SharedRootFolder)this.m_pRootFolders.SelectedItems[0].Tag;
				if (MessageBox.Show(this, "Are you sure you want to delete Root folder '" + sharedRootFolder2.Name + "' !", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					sharedRootFolder2.Owner.Remove(sharedRootFolder2);
					this.LoadRoots("");
				}
			}
		}

		private void m_pRootFolders_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pRootFolders.Items.Count > 0 && this.m_pRootFolders.SelectedItems.Count > 0)
			{
				this.m_pToolbar.Items[1].Enabled = true;
				this.m_pToolbar.Items[2].Enabled = true;
				return;
			}
			this.m_pToolbar.Items[1].Enabled = false;
			this.m_pToolbar.Items[2].Enabled = false;
		}

		private void m_pRootFolders_DoubleClick(object sender, EventArgs e)
		{
			if (this.m_pRootFolders.SelectedItems.Count > 0)
			{
				SharedRootFolder sharedRootFolder = (SharedRootFolder)this.m_pRootFolders.SelectedItems[0].Tag;
				AddEditRootFolderForm addEditRootFolderForm = new AddEditRootFolderForm(this.m_pVirtualServer, sharedRootFolder);
				if (addEditRootFolderForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadRoots(sharedRootFolder.ID);
				}
			}
		}

		private void LoadRoots(string selectedRootID)
		{
			this.m_pRootFolders.Items.Clear();
			foreach (SharedRootFolder sharedRootFolder in this.m_pVirtualServer.RootFolders)
			{
				ListViewItem listViewItem = new ListViewItem();
				listViewItem.Text = sharedRootFolder.Enabled.ToString();
				if (!sharedRootFolder.Enabled)
				{
					listViewItem.ForeColor = Color.Purple;
					listViewItem.Font = new Font(listViewItem.Font.FontFamily, listViewItem.Font.Size, FontStyle.Strikeout);
					listViewItem.ImageIndex = 1;
				}
				else
				{
					listViewItem.ImageIndex = 0;
				}
				listViewItem.Tag = sharedRootFolder;
				listViewItem.Text = sharedRootFolder.Name;
				listViewItem.SubItems.Add(sharedRootFolder.Description);
				listViewItem.SubItems.Add(sharedRootFolder.Type.ToString());
				this.m_pRootFolders.Items.Add(listViewItem);
				if (sharedRootFolder.ID == selectedRootID)
				{
					listViewItem.Selected = true;
				}
			}
			this.m_pRootFolders_SelectedIndexChanged(this, new EventArgs());
		}
	}
}
