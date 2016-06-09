using DataSmart.MailServer.Filters;
using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class FiltersForm : Form
	{
		private ToolStrip m_pToolbar;

		private ImageList m_pFiltersImages;

		private ListView m_pFilters;

		private VirtualServer m_pVirtualServer;

		public FiltersForm(VirtualServer virtualServer, WFrame frame)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			frame.Frame_ToolStrip = this.m_pToolbar;
			this.LoadFilters("");
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
			this.m_pFiltersImages = new ImageList();
			this.m_pFiltersImages.Images.Add(ResManager.GetIcon("filter.ico"));
			this.m_pFiltersImages.Images.Add(ResManager.GetIcon("filter_disabled.ico"));
			this.m_pFilters = new ListView();
			this.m_pFilters.Size = new Size(445, 290);
			this.m_pFilters.Location = new Point(10, 20);
			this.m_pFilters.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pFilters.View = View.Details;
			this.m_pFilters.FullRowSelect = true;
			this.m_pFilters.HideSelection = false;
			this.m_pFilters.SmallImageList = this.m_pFiltersImages;
			this.m_pFilters.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			this.m_pFilters.DoubleClick += new EventHandler(this.m_pFilters_DoubleClick);
			this.m_pFilters.SelectedIndexChanged += new EventHandler(this.m_pFilters_SelectedIndexChanged);
			this.m_pFilters.MouseUp += new MouseEventHandler(this.m_pFilters_MouseUp);
			this.m_pFilters.Columns.Add("Description", 460, HorizontalAlignment.Left);
			base.Controls.Add(this.m_pFilters);
		}

		private void m_pToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			this.SwitchToolBarTask(e.ClickedItem.Tag.ToString());
		}

		private void m_pFilters_DoubleClick(object sender, EventArgs e)
		{
			if (this.m_pFilters.SelectedItems.Count > 0)
			{
				Filter filter = (Filter)this.m_pFilters.SelectedItems[0].Tag;
				AddEditFilterForm addEditFilterForm = new AddEditFilterForm(this.m_pVirtualServer, filter);
				if (addEditFilterForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadFilters(addEditFilterForm.FilterID);
				}
			}
		}

		private void m_pFilters_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pFilters.SelectedItems.Count > 0)
			{
				this.m_pToolbar.Items[1].Enabled = true;
				this.m_pToolbar.Items[2].Enabled = true;
				if (this.m_pFilters.SelectedItems[0].Index > 0)
				{
					this.m_pToolbar.Items[6].Enabled = true;
				}
				if (this.m_pFilters.SelectedItems[0].Index < this.m_pFilters.Items.Count - 1)
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

		private void m_pFilters_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right)
			{
				return;
			}
			ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
			contextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pFilters_ContextMenuItem_Clicked);
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("Add");
			toolStripMenuItem.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripMenuItem.Tag = "add";
			contextMenuStrip.Items.Add(toolStripMenuItem);
			ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem("Edit");
			toolStripMenuItem2.Enabled = (this.m_pFilters.SelectedItems.Count > 0);
			toolStripMenuItem2.Tag = "edit";
			toolStripMenuItem2.Image = ResManager.GetIcon("edit.ico").ToBitmap();
			contextMenuStrip.Items.Add(toolStripMenuItem2);
			ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem("Delete");
			toolStripMenuItem3.Enabled = (this.m_pFilters.SelectedItems.Count > 0);
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
			if (this.m_pFilters.SelectedItems.Count <= 0 || this.m_pFilters.SelectedItems[0].Index <= 0)
			{
				toolStripMenuItem5.Enabled = false;
			}
			toolStripMenuItem5.Image = ResManager.GetIcon("up.ico").ToBitmap();
			toolStripMenuItem5.Tag = "up";
			contextMenuStrip.Items.Add(toolStripMenuItem5);
			ToolStripMenuItem toolStripMenuItem6 = new ToolStripMenuItem("Move Down");
			if (this.m_pFilters.SelectedItems.Count <= 0 || this.m_pFilters.SelectedItems[0].Index >= this.m_pFilters.Items.Count - 1)
			{
				toolStripMenuItem6.Enabled = false;
			}
			toolStripMenuItem6.Image = ResManager.GetIcon("down.ico").ToBitmap();
			toolStripMenuItem6.Tag = "down";
			contextMenuStrip.Items.Add(toolStripMenuItem6);
			contextMenuStrip.Show(Control.MousePosition);
		}

		private void m_pFilters_ContextMenuItem_Clicked(object sender, ToolStripItemClickedEventArgs e)
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
				AddEditFilterForm addEditFilterForm = new AddEditFilterForm(this.m_pVirtualServer);
				if (addEditFilterForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadFilters(addEditFilterForm.FilterID);
					return;
				}
			}
			else if (taskID == "edit")
			{
				Filter filter = (Filter)this.m_pFilters.SelectedItems[0].Tag;
				AddEditFilterForm addEditFilterForm2 = new AddEditFilterForm(this.m_pVirtualServer, filter);
				if (addEditFilterForm2.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadFilters(addEditFilterForm2.FilterID);
					return;
				}
			}
			else if (taskID == "delete")
			{
				Filter filter2 = (Filter)this.m_pFilters.SelectedItems[0].Tag;
				if (MessageBox.Show(this, "Are you sure you want to delete Filter '" + filter2.Description + "' !", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					filter2.Owner.Remove(filter2);
					this.m_pFilters.SelectedItems[0].Remove();
					return;
				}
			}
			else
			{
				if (taskID == "refresh")
				{
					this.LoadFilters("");
					return;
				}
				if (taskID == "up")
				{
					if (this.m_pFilters.SelectedItems.Count > 0 && this.m_pFilters.SelectedItems[0].Index > 0)
					{
						this.SwapFilters(this.m_pFilters.SelectedItems[0], this.m_pFilters.Items[this.m_pFilters.SelectedItems[0].Index - 1]);
						return;
					}
				}
				else if (taskID == "down" && this.m_pFilters.SelectedItems.Count > 0 && this.m_pFilters.SelectedItems[0].Index < this.m_pFilters.Items.Count - 1)
				{
					this.SwapFilters(this.m_pFilters.SelectedItems[0], this.m_pFilters.Items[this.m_pFilters.SelectedItems[0].Index + 1]);
				}
			}
		}

		private void LoadFilters(string selectedFilterID)
		{
			this.m_pFilters.Items.Clear();
			this.m_pVirtualServer.Filters.Refresh();
			foreach (Filter filter in this.m_pVirtualServer.Filters)
			{
				ListViewItem listViewItem = new ListViewItem();
				if (!filter.Enabled)
				{
					listViewItem.ForeColor = Color.Purple;
					listViewItem.Font = new Font(listViewItem.Font.FontFamily, listViewItem.Font.Size, FontStyle.Strikeout);
					listViewItem.ImageIndex = 1;
				}
				else
				{
					listViewItem.ImageIndex = 0;
				}
				listViewItem.Tag = filter;
				listViewItem.Text = filter.Description;
				this.m_pFilters.Items.Add(listViewItem);
				if (filter.ID == selectedFilterID)
				{
					listViewItem.Selected = true;
				}
			}
			this.m_pFilters_SelectedIndexChanged(this, new EventArgs());
		}

		private void SwapFilters(ListViewItem item1, ListViewItem item2)
		{
			Filter filter = (Filter)item1.Tag;
			Filter filter2 = (Filter)item2.Tag;
			string selectedFilterID = "";
			if (item1.Selected)
			{
				selectedFilterID = filter2.ID;
			}
			else if (item2.Selected)
			{
				selectedFilterID = filter.ID;
			}
			bool enabled = filter2.Enabled;
			string description = filter2.Description;
			string assemblyName = filter2.AssemblyName;
			string @class = filter2.Class;
			filter2.Enabled = filter.Enabled;
			filter2.Description = filter.Description;
			filter2.AssemblyName = filter.AssemblyName;
			filter2.Class = filter.Class;
			filter2.Commit();
			filter.Enabled = enabled;
			filter.Description = description;
			filter.AssemblyName = assemblyName;
			filter.Class = @class;
			filter.Commit();
			this.LoadFilters(selectedFilterID);
		}

		private Form GetFilterSettingsUI()
		{
			try
			{
				if (this.m_pFilters.SelectedItems.Count > 0)
				{
					Filter filter = (Filter)this.m_pFilters.SelectedItems[0].Tag;
					string text = filter.AssemblyName;
					if (!File.Exists(text))
					{
						text = Application.StartupPath + "\\Filters\\" + text;
					}
					Assembly assembly = Assembly.LoadFrom(text);
					Type type = assembly.ExportedTypes.FirstOrDefault((Type x) => typeof(ISettingsUI).IsAssignableFrom(x));
					ISettingsUI settingsUI = (ISettingsUI)Activator.CreateInstance(type);
					return settingsUI.GetUI();
				}
			}
			catch
			{
			}
			return null;
		}
	}
}
