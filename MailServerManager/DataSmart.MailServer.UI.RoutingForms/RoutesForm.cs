using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI.RoutingForms
{
	public class RoutesForm : Form
	{
		private ToolStrip m_pToolbar;

		private ImageList m_pRoutesImages;

		private ListView m_pRoutes;

		private VirtualServer m_pVirtualServer;

		public RoutesForm(VirtualServer virtualServer, WFrame frame)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			frame.Frame_ToolStrip = this.m_pToolbar;
			this.LoadRoutes("");
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
			this.m_pToolbar.Items.Add(new ToolStripSeparator());
			ToolStripButton toolStripButton4 = new ToolStripButton();
			toolStripButton4.Image = ResManager.GetIcon("refresh.ico").ToBitmap();
			toolStripButton4.Tag = "refresh";
			toolStripButton4.ToolTipText = "Refresh";
			this.m_pToolbar.Items.Add(toolStripButton4);
			this.m_pToolbar.Items.Add(new ToolStripSeparator());
			ToolStripButton toolStripButton5 = new ToolStripButton();
			toolStripButton5.Enabled = false;
			toolStripButton5.Image = ResManager.GetIcon("up.ico").ToBitmap();
			toolStripButton5.Tag = "up";
			this.m_pToolbar.Items.Add(toolStripButton5);
			ToolStripButton toolStripButton6 = new ToolStripButton();
			toolStripButton6.Enabled = false;
			toolStripButton6.Image = ResManager.GetIcon("down.ico").ToBitmap();
			toolStripButton6.Tag = "down";
			this.m_pToolbar.Items.Add(toolStripButton6);
			this.m_pRoutesImages = new ImageList();
			this.m_pRoutesImages.Images.Add(ResManager.GetIcon("filter.ico"));
			this.m_pRoutesImages.Images.Add(ResManager.GetIcon("filter_disabled.ico"));
			this.m_pRoutes = new ListView();
			this.m_pRoutes.Size = new Size(445, 290);
			this.m_pRoutes.Location = new Point(10, 20);
			this.m_pRoutes.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pRoutes.View = View.Details;
			this.m_pRoutes.FullRowSelect = true;
			this.m_pRoutes.HideSelection = false;
			this.m_pRoutes.SmallImageList = this.m_pRoutesImages;
			this.m_pRoutes.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			this.m_pRoutes.DoubleClick += new EventHandler(this.m_pRoutes_DoubleClick);
			this.m_pRoutes.SelectedIndexChanged += new EventHandler(this.m_pRoutes_SelectedIndexChanged);
			this.m_pRoutes.MouseUp += new MouseEventHandler(this.m_pRoutes_MouseUp);
			this.m_pRoutes.Columns.Add("Pattern", 190, HorizontalAlignment.Left);
			this.m_pRoutes.Columns.Add("Description", 280, HorizontalAlignment.Left);
			base.Controls.Add(this.m_pRoutes);
		}

		private void m_pToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			this.SwitchToolBarTask(e.ClickedItem.Tag.ToString());
		}

		private void m_pRoutes_DoubleClick(object sender, EventArgs e)
		{
			if (this.m_pRoutes.SelectedItems.Count > 0)
			{
				Route route = (Route)this.m_pRoutes.SelectedItems[0].Tag;
				AddEditRouteForm addEditRouteForm = new AddEditRouteForm(this.m_pVirtualServer, route);
				if (addEditRouteForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadRoutes(route.ID);
				}
			}
		}

		private void m_pRoutes_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pRoutes.SelectedItems.Count > 0)
			{
				this.m_pToolbar.Items[1].Enabled = true;
				this.m_pToolbar.Items[2].Enabled = true;
				if (this.m_pRoutes.SelectedItems[0].Index > 0)
				{
					this.m_pToolbar.Items[6].Enabled = true;
				}
				if (this.m_pRoutes.SelectedItems[0].Index < this.m_pRoutes.Items.Count - 1)
				{
					this.m_pToolbar.Items[7].Enabled = true;
					return;
				}
			}
			else
			{
				this.m_pToolbar.Items[1].Enabled = false;
				this.m_pToolbar.Items[2].Enabled = false;
				this.m_pToolbar.Items[6].Enabled = false;
				this.m_pToolbar.Items[7].Enabled = false;
			}
		}

		private void m_pRoutes_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right)
			{
				return;
			}
			ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
			contextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pRoutes_ContextMenuItem_Clicked);
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("Add");
			toolStripMenuItem.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripMenuItem.Tag = "add";
			contextMenuStrip.Items.Add(toolStripMenuItem);
			ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem("Edit");
			toolStripMenuItem2.Enabled = (this.m_pRoutes.SelectedItems.Count > 0);
			toolStripMenuItem2.Tag = "edit";
			toolStripMenuItem2.Image = ResManager.GetIcon("edit.ico").ToBitmap();
			contextMenuStrip.Items.Add(toolStripMenuItem2);
			ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem("Delete");
			toolStripMenuItem3.Enabled = (this.m_pRoutes.SelectedItems.Count > 0);
			toolStripMenuItem3.Tag = "delete";
			toolStripMenuItem3.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			contextMenuStrip.Items.Add(toolStripMenuItem3);
			contextMenuStrip.Items.Add(new ToolStripSeparator());
			ToolStripMenuItem toolStripMenuItem4 = new ToolStripMenuItem("Refresh");
			toolStripMenuItem4.Image = ResManager.GetIcon("refresh.ico").ToBitmap();
			toolStripMenuItem4.Tag = "refresh";
			contextMenuStrip.Items.Add(toolStripMenuItem4);
			contextMenuStrip.Items.Add(new ToolStripSeparator());
			ToolStripMenuItem toolStripMenuItem5 = new ToolStripMenuItem("Move Up");
			if (this.m_pRoutes.SelectedItems.Count <= 0 || this.m_pRoutes.SelectedItems[0].Index <= 0)
			{
				toolStripMenuItem5.Enabled = false;
			}
			toolStripMenuItem5.Image = ResManager.GetIcon("up.ico").ToBitmap();
			toolStripMenuItem5.Tag = "up";
			contextMenuStrip.Items.Add(toolStripMenuItem5);
			ToolStripMenuItem toolStripMenuItem6 = new ToolStripMenuItem("Move Down");
			if (this.m_pRoutes.SelectedItems.Count <= 0 || this.m_pRoutes.SelectedItems[0].Index >= this.m_pRoutes.Items.Count - 1)
			{
				toolStripMenuItem6.Enabled = false;
			}
			toolStripMenuItem6.Image = ResManager.GetIcon("down.ico").ToBitmap();
			toolStripMenuItem6.Tag = "down";
			contextMenuStrip.Items.Add(toolStripMenuItem6);
			contextMenuStrip.Show(Control.MousePosition);
		}

		private void m_pRoutes_ContextMenuItem_Clicked(object sender, ToolStripItemClickedEventArgs e)
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
				AddEditRouteForm addEditRouteForm = new AddEditRouteForm(this.m_pVirtualServer);
				if (addEditRouteForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadRoutes(addEditRouteForm.RouteID);
					return;
				}
			}
			else if (taskID == "edit")
			{
				Route route = (Route)this.m_pRoutes.SelectedItems[0].Tag;
				AddEditRouteForm addEditRouteForm2 = new AddEditRouteForm(this.m_pVirtualServer, route);
				if (addEditRouteForm2.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadRoutes(route.ID);
					return;
				}
			}
			else if (taskID == "delete")
			{
				Route route2 = (Route)this.m_pRoutes.SelectedItems[0].Tag;
				if (MessageBox.Show(this, "Are you sure you want to delete Route '" + route2.Pattern + "' !", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					route2.Owner.Remove(route2);
					this.m_pRoutes.SelectedItems[0].Remove();
					return;
				}
			}
			else
			{
				if (taskID == "refresh")
				{
					this.LoadRoutes("");
					return;
				}
				if (taskID == "up")
				{
					if (this.m_pRoutes.SelectedItems.Count > 0 && this.m_pRoutes.SelectedItems[0].Index > 0)
					{
						this.SwapRoutes(this.m_pRoutes.SelectedItems[0], this.m_pRoutes.Items[this.m_pRoutes.SelectedItems[0].Index - 1]);
						return;
					}
				}
				else if (taskID == "down" && this.m_pRoutes.SelectedItems.Count > 0 && this.m_pRoutes.SelectedItems[0].Index < this.m_pRoutes.Items.Count - 1)
				{
					this.SwapRoutes(this.m_pRoutes.SelectedItems[0], this.m_pRoutes.Items[this.m_pRoutes.SelectedItems[0].Index + 1]);
				}
			}
		}

		private void LoadRoutes(string selectedRouteID)
		{
			this.m_pRoutes.Items.Clear();
			this.m_pVirtualServer.Routes.Refresh();
			foreach (Route route in this.m_pVirtualServer.Routes)
			{
				ListViewItem listViewItem = new ListViewItem();
				if (!route.Enabled)
				{
					listViewItem.ForeColor = Color.Purple;
					listViewItem.Font = new Font(listViewItem.Font.FontFamily, listViewItem.Font.Size, FontStyle.Strikeout);
					listViewItem.ImageIndex = 1;
				}
				else
				{
					listViewItem.ImageIndex = 0;
				}
				listViewItem.Tag = route;
				listViewItem.Text = route.Pattern;
				listViewItem.SubItems.Add(route.Description);
				this.m_pRoutes.Items.Add(listViewItem);
				if (route.ID == selectedRouteID)
				{
					listViewItem.Selected = true;
				}
			}
			this.m_pRoutes_SelectedIndexChanged(this, new EventArgs());
		}

		private void SwapRoutes(ListViewItem item1, ListViewItem item2)
		{
		}
	}
}
