using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI.SecurityForms
{
	public class IPSecurityForm : Form
	{
		private ToolStrip m_pToolbar;

		private ImageList m_pIPSecurityImages;

		private ListView m_pIPSecurity;

		private VirtualServer m_pVirtualServer;

		public IPSecurityForm(VirtualServer virtualServer, WFrame frame)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			frame.Frame_ToolStrip = this.m_pToolbar;
			this.LoadSecurity("");
		}

		private void InitializeComponent()
		{
			base.Size = new Size(472, 357);
			ImageList imageList = new ImageList();
			imageList.Images.Add(ResManager.GetIcon("add.ico"));
			imageList.Images.Add(ResManager.GetIcon("edit.ico"));
			imageList.Images.Add(ResManager.GetIcon("delete.ico"));
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
			this.m_pIPSecurityImages = new ImageList();
			this.m_pIPSecurityImages.Images.Add(ResManager.GetIcon("security.ico"));
			this.m_pIPSecurityImages.Images.Add(ResManager.GetIcon("security_disabled.ico"));
			this.m_pIPSecurity = new ListView();
			this.m_pIPSecurity.Size = new Size(445, 265);
			this.m_pIPSecurity.Location = new Point(9, 47);
			this.m_pIPSecurity.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pIPSecurity.View = View.Details;
			this.m_pIPSecurity.FullRowSelect = true;
			this.m_pIPSecurity.HideSelection = false;
			this.m_pIPSecurity.SmallImageList = this.m_pIPSecurityImages;
			this.m_pIPSecurity.SelectedIndexChanged += new EventHandler(this.m_pIPSecurity_SelectedIndexChanged);
			this.m_pIPSecurity.DoubleClick += new EventHandler(this.m_pIPSecurity_DoubleClick);
			this.m_pIPSecurity.MouseUp += new MouseEventHandler(this.m_pIPSecurity_MouseUp);
			this.m_pIPSecurity.Columns.Add("Description", 180, HorizontalAlignment.Left);
			this.m_pIPSecurity.Columns.Add("Service", 55, HorizontalAlignment.Left);
			this.m_pIPSecurity.Columns.Add("Action", 55, HorizontalAlignment.Left);
			this.m_pIPSecurity.Columns.Add("StartIP", 120, HorizontalAlignment.Left);
			this.m_pIPSecurity.Columns.Add("EndIP", 120, HorizontalAlignment.Left);
			base.Controls.Add(this.m_pToolbar);
			base.Controls.Add(this.m_pIPSecurity);
		}

		private void m_pToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			this.SwitchToolBarTask(e.ClickedItem.Tag.ToString());
		}

		private void m_pIPSecurity_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pIPSecurity.Items.Count > 0 && this.m_pIPSecurity.SelectedItems.Count > 0)
			{
				this.m_pToolbar.Items[1].Enabled = true;
				this.m_pToolbar.Items[2].Enabled = true;
				return;
			}
			this.m_pToolbar.Items[1].Enabled = false;
			this.m_pToolbar.Items[2].Enabled = false;
		}

		private void m_pIPSecurity_DoubleClick(object sender, EventArgs e)
		{
			if (this.m_pIPSecurity.Items.Count > 0 && this.m_pIPSecurity.SelectedItems.Count > 0)
			{
				IPSecurity securityEntry = (IPSecurity)this.m_pIPSecurity.SelectedItems[0].Tag;
				AddEditIPSecurityEntryForm addEditIPSecurityEntryForm = new AddEditIPSecurityEntryForm(this.m_pVirtualServer, securityEntry);
				if (addEditIPSecurityEntryForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadSecurity(addEditIPSecurityEntryForm.SecurityEntryID);
				}
			}
		}

		private void m_pIPSecurity_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right)
			{
				return;
			}
			ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
			contextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pIPSecurity_ContextMenuItem_Clicked);
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("Add");
			toolStripMenuItem.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripMenuItem.Tag = "add";
			contextMenuStrip.Items.Add(toolStripMenuItem);
			ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem("Edit");
			toolStripMenuItem2.Enabled = (this.m_pIPSecurity.SelectedItems.Count > 0);
			toolStripMenuItem2.Tag = "edit";
			toolStripMenuItem2.Image = ResManager.GetIcon("edit.ico").ToBitmap();
			contextMenuStrip.Items.Add(toolStripMenuItem2);
			ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem("Delete");
			toolStripMenuItem3.Enabled = (this.m_pIPSecurity.SelectedItems.Count > 0);
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

		private void m_pIPSecurity_ContextMenuItem_Clicked(object sender, ToolStripItemClickedEventArgs e)
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
				AddEditIPSecurityEntryForm addEditIPSecurityEntryForm = new AddEditIPSecurityEntryForm(this.m_pVirtualServer);
				if (addEditIPSecurityEntryForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadSecurity(addEditIPSecurityEntryForm.SecurityEntryID);
					return;
				}
			}
			else if (taskID == "edit")
			{
				IPSecurity securityEntry = (IPSecurity)this.m_pIPSecurity.SelectedItems[0].Tag;
				AddEditIPSecurityEntryForm addEditIPSecurityEntryForm2 = new AddEditIPSecurityEntryForm(this.m_pVirtualServer, securityEntry);
				if (addEditIPSecurityEntryForm2.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadSecurity(addEditIPSecurityEntryForm2.SecurityEntryID);
					return;
				}
			}
			else if (taskID == "delete")
			{
				IPSecurity iPSecurity = (IPSecurity)this.m_pIPSecurity.SelectedItems[0].Tag;
				if (MessageBox.Show(this, "Are you sure you want to delete IP Security entry '" + iPSecurity.Description + "' !", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					iPSecurity.Owner.Remove(iPSecurity);
					this.m_pIPSecurity.SelectedItems[0].Remove();
					return;
				}
			}
			else if (taskID == "refresh")
			{
				this.LoadSecurity("");
			}
		}

		private void LoadSecurity(string selectedSecurityEntry)
		{
			this.m_pIPSecurity.Items.Clear();
			this.m_pVirtualServer.IpSecurity.Refresh();
			foreach (IPSecurity iPSecurity in this.m_pVirtualServer.IpSecurity)
			{
				ListViewItem listViewItem = new ListViewItem();
				if (!iPSecurity.Enabled)
				{
					listViewItem.ForeColor = Color.Purple;
					listViewItem.Font = new Font(listViewItem.Font.FontFamily, listViewItem.Font.Size, FontStyle.Strikeout);
					listViewItem.ImageIndex = 1;
				}
				else
				{
					listViewItem.ImageIndex = 0;
				}
				listViewItem.Tag = iPSecurity;
				listViewItem.Text = iPSecurity.Description;
				listViewItem.SubItems.Add(iPSecurity.Service.ToString());
				listViewItem.SubItems.Add(iPSecurity.Action.ToString());
				listViewItem.SubItems.Add(iPSecurity.StartIP.ToString());
				listViewItem.SubItems.Add(iPSecurity.EndIP.ToString());
				this.m_pIPSecurity.Items.Add(listViewItem);
				if (iPSecurity.ID == selectedSecurityEntry)
				{
					listViewItem.Selected = true;
				}
			}
			this.m_pIPSecurity_SelectedIndexChanged(this, new EventArgs());
		}
	}
}
