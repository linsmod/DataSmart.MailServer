using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class DomainsForm : Form
	{
		private ToolStrip m_pToolbar;

		private ImageList m_pDomainsImages;

		private WListView m_pDomains;

		private VirtualServer m_pVirtualServer;

		public DomainsForm(VirtualServer virtualServer, WFrame frame)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			frame.Frame_ToolStrip = this.m_pToolbar;
			this.LoadDomains();
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
			toolStripButton.ToolTipText = "Add";
			this.m_pToolbar.Items.Add(toolStripButton);
			ToolStripButton toolStripButton2 = new ToolStripButton();
			toolStripButton2.Enabled = false;
			toolStripButton2.Image = ResManager.GetIcon("edit.ico").ToBitmap();
			toolStripButton2.Tag = "edit";
			toolStripButton2.ToolTipText = "Edit";
			this.m_pToolbar.Items.Add(toolStripButton2);
			ToolStripButton toolStripButton3 = new ToolStripButton();
			toolStripButton3.Enabled = false;
			toolStripButton3.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			toolStripButton3.Tag = "delete";
			toolStripButton3.ToolTipText = "Delete";
			this.m_pToolbar.Items.Add(toolStripButton3);
			this.m_pToolbar.Items.Add(new ToolStripSeparator());
			ToolStripButton toolStripButton4 = new ToolStripButton();
			toolStripButton4.Image = ResManager.GetIcon("refresh.ico").ToBitmap();
			toolStripButton4.Tag = "refresh";
			toolStripButton4.ToolTipText = "Refresh";
			this.m_pToolbar.Items.Add(toolStripButton4);
			this.m_pDomainsImages = new ImageList();
			this.m_pDomainsImages.Images.Add(ResManager.GetImage("icon-domain.png"));
			this.m_pDomains = new WListView();
			this.m_pDomains.Size = new Size(445, 265);
			this.m_pDomains.Location = new Point(9, 47);
			this.m_pDomains.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pDomains.View = View.Details;
			this.m_pDomains.FullRowSelect = true;
			this.m_pDomains.HideSelection = false;
			this.m_pDomains.SmallImageList = this.m_pDomainsImages;
			this.m_pDomains.SelectedIndexChanged += new EventHandler(this.m_pDomains_SelectedIndexChanged);
			this.m_pDomains.DoubleClick += new EventHandler(this.m_pDomains_DoubleClick);
			this.m_pDomains.MouseUp += new MouseEventHandler(this.m_pDomains_MouseUp);
			this.m_pDomains.Columns.Add("Name", 190, HorizontalAlignment.Left);
			this.m_pDomains.Columns.Add("Description", 290, HorizontalAlignment.Left);
			base.Controls.Add(this.m_pToolbar);
			base.Controls.Add(this.m_pDomains);
		}

		private void m_pToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			this.SwitchToolBarTask(e.ClickedItem.Tag.ToString());
		}

		private void m_pDomains_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pDomains.Items.Count > 0 && this.m_pDomains.SelectedItems.Count > 0)
			{
				this.m_pToolbar.Items[1].Enabled = true;
				this.m_pToolbar.Items[2].Enabled = true;
				return;
			}
			this.m_pToolbar.Items[1].Enabled = false;
			this.m_pToolbar.Items[2].Enabled = false;
		}

		private void m_pDomains_DoubleClick(object sender, EventArgs e)
		{
			if (this.m_pDomains.SelectedItems.Count > 0)
			{
				Domain domain = (Domain)this.m_pDomains.SelectedItems[0].Tag;
				AddEditDomainForm addEditDomainForm = new AddEditDomainForm(this.m_pVirtualServer, domain);
				if (addEditDomainForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadDomains();
				}
			}
		}

		private void m_pDomains_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right)
			{
				return;
			}
			ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
			contextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pDomains_ContextMenuItem_Clicked);
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("Add");
			toolStripMenuItem.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripMenuItem.Tag = "add";
			contextMenuStrip.Items.Add(toolStripMenuItem);
			ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem("Edit");
			toolStripMenuItem2.Enabled = (this.m_pDomains.SelectedItems.Count > 0);
			toolStripMenuItem2.Tag = "edit";
			toolStripMenuItem2.Image = ResManager.GetIcon("edit.ico").ToBitmap();
			contextMenuStrip.Items.Add(toolStripMenuItem2);
			ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem("Delete");
			toolStripMenuItem3.Enabled = (this.m_pDomains.SelectedItems.Count > 0);
			toolStripMenuItem3.Tag = "delete";
			toolStripMenuItem3.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			contextMenuStrip.Items.Add(toolStripMenuItem3);
			contextMenuStrip.Items.Add(new ToolStripSeparator());
			ToolStripMenuItem toolStripMenuItem4 = new ToolStripMenuItem("Refresh");
			toolStripMenuItem4.Image = ResManager.GetIcon("refresh.ico").ToBitmap();
			toolStripMenuItem4.Tag = "refresh";
			contextMenuStrip.Items.Add(toolStripMenuItem4);
			contextMenuStrip.Show(Control.MousePosition);
		}

		private void m_pDomains_ContextMenuItem_Clicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			this.SwitchToolBarTask(e.ClickedItem.Tag.ToString());
		}

		private void SwitchToolBarTask(string taskID)
		{
			if (taskID == "add")
			{
				AddEditDomainForm addEditDomainForm = new AddEditDomainForm(this.m_pVirtualServer);
				if (addEditDomainForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadDomains();
					return;
				}
			}
			else if (taskID == "edit")
			{
				Domain domain = (Domain)this.m_pDomains.SelectedItems[0].Tag;
				AddEditDomainForm addEditDomainForm2 = new AddEditDomainForm(this.m_pVirtualServer, domain);
				if (addEditDomainForm2.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadDomains();
					return;
				}
			}
			else if (taskID == "delete")
			{
				Domain domain2 = (Domain)this.m_pDomains.SelectedItems[0].Tag;
				if (MessageBox.Show(this, "Warning: Deleting domain '" + domain2.DomainName + "', deletes all domain users and mailing lists,...!!!\nDo you want to continue?", "Delete confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					domain2.Owner.Remove(domain2);
					this.m_pDomains.SelectedItems[0].Remove();
					return;
				}
			}
			else if (taskID == "refresh")
			{
				this.LoadDomains();
			}
		}

		private void LoadDomains()
		{
			this.m_pDomains.Items.Clear();
			this.m_pVirtualServer.Domains.Refresh();
			foreach (Domain domain in this.m_pVirtualServer.Domains)
			{
				ListViewItem listViewItem = new ListViewItem();
				listViewItem.ImageIndex = 0;
				listViewItem.Tag = domain;
				listViewItem.Text = domain.DomainName;
				listViewItem.SubItems.Add(domain.Description);
				this.m_pDomains.Items.Add(listViewItem);
			}
			this.m_pDomains.SortItems();
			this.m_pDomains_SelectedIndexChanged(this, new EventArgs());
		}
	}
}
