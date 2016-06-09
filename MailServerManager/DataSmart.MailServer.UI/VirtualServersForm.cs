using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class VirtualServersForm : Form
	{
		private ToolStrip m_pToolbar;

		private WListView m_pServers;

		private MainForm m_pFrmMain;

		private TreeNode m_pVirtualServersNode;

		private Server m_pServer;

		public VirtualServersForm(MainForm mainFrm, TreeNode virtualServersNode, Server server, WFrame frame)
		{
			this.m_pFrmMain = mainFrm;
			this.m_pVirtualServersNode = virtualServersNode;
			this.m_pServer = server;
			this.InitializeComponent();
			frame.Frame_ToolStrip = this.m_pToolbar;
			this.LoadVirtualServers();
		}

		private void InitializeComponent()
		{
			base.Size = new Size(450, 300);
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
			ImageList imageList = new ImageList();
			imageList.Images.Add(ResManager.GetImage("icon-server-running.png"));
			imageList.Images.Add(ResManager.GetImage("icon-server-stopped.png"));
			this.m_pServers = new WListView();
			this.m_pServers.Size = new Size(425, 210);
			this.m_pServers.Location = new Point(9, 47);
			this.m_pServers.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pServers.View = View.Details;
			this.m_pServers.FullRowSelect = true;
			this.m_pServers.HideSelection = false;
			this.m_pServers.SmallImageList = imageList;
			this.m_pServers.SelectedIndexChanged += new EventHandler(this.m_pServers_SelectedIndexChanged);
			this.m_pServers.DoubleClick += new EventHandler(this.m_pServers_DoubleClick);
			this.m_pServers.Columns.Add("Name", 400, HorizontalAlignment.Left);
			base.Controls.Add(this.m_pServers);
		}

		private void m_pToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			if (e.ClickedItem.Tag.ToString() == "add")
			{
				AddEditVirtualServerForm addEditVirtualServerForm = new AddEditVirtualServerForm(this.m_pServer);
				if (addEditVirtualServerForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadVirtualServers();
					this.m_pFrmMain.LoadVirtualServers(this.m_pVirtualServersNode, this.m_pServer);
					return;
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "edit")
			{
				VirtualServer virtualServer = (VirtualServer)this.m_pServers.SelectedItems[0].Tag;
				AddEditVirtualServerForm addEditVirtualServerForm2 = new AddEditVirtualServerForm(this.m_pServer, virtualServer);
				if (addEditVirtualServerForm2.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadVirtualServers();
					this.m_pFrmMain.LoadVirtualServers(this.m_pVirtualServersNode, this.m_pServer);
					return;
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "delete")
			{
				VirtualServer virtualServer2 = (VirtualServer)this.m_pServers.SelectedItems[0].Tag;
				if (MessageBox.Show(this, "Are you sure you want to delete Virtual server '" + virtualServer2.Name + "' !", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					virtualServer2.Owner.Remove(virtualServer2);
					this.LoadVirtualServers();
					this.m_pFrmMain.LoadVirtualServers(this.m_pVirtualServersNode, this.m_pServer);
				}
			}
		}

		private void m_pServers_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pServers.SelectedItems.Count == 0)
			{
				this.m_pToolbar.Items[1].Enabled = false;
				this.m_pToolbar.Items[2].Enabled = false;
				return;
			}
			this.m_pToolbar.Items[1].Enabled = true;
			this.m_pToolbar.Items[2].Enabled = true;
		}

		private void m_pServers_DoubleClick(object sender, EventArgs e)
		{
			if (this.m_pServers.SelectedItems.Count > 0)
			{
				VirtualServer virtualServer = (VirtualServer)this.m_pServers.SelectedItems[0].Tag;
				AddEditVirtualServerForm addEditVirtualServerForm = new AddEditVirtualServerForm(this.m_pServer, virtualServer);
				if (addEditVirtualServerForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadVirtualServers();
					this.m_pFrmMain.LoadVirtualServers(this.m_pVirtualServersNode, this.m_pServer);
				}
			}
		}

		private void LoadVirtualServers()
		{
			this.m_pServers.Items.Clear();
			foreach (VirtualServer virtualServer in this.m_pServer.VirtualServers)
			{
				ListViewItem listViewItem = new ListViewItem(virtualServer.Name);
				if (virtualServer.Enabled)
				{
					listViewItem.ImageIndex = 0;
				}
				else
				{
					listViewItem.ImageIndex = 1;
				}
				listViewItem.Tag = virtualServer;
				this.m_pServers.Items.Add(listViewItem);
			}
			this.m_pServers_SelectedIndexChanged(this, null);
		}
	}
}
