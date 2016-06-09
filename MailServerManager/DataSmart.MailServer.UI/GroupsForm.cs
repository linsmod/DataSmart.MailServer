using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class GroupsForm : Form
	{
		private ToolStrip m_pToolbar;

		private Label mt_Filter;

		private TextBox m_pFilter;

		private Button m_pGetGroups;

		private ImageList m_pGroupsImages;

		private WListView m_pGroups;

		private VirtualServer m_pVirtualServer;

		public GroupsForm(VirtualServer virtualServer, WFrame frame)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			frame.Frame_ToolStrip = this.m_pToolbar;
			this.LoadGroups("");
		}

		private void InitializeComponent()
		{
			base.Size = new Size(300, 300);
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
			this.mt_Filter = new Label();
			this.mt_Filter.Size = new Size(100, 20);
			this.mt_Filter.Location = new Point(9, 20);
			this.mt_Filter.Text = "Filter:";
			this.mt_Filter.TextAlign = ContentAlignment.MiddleRight;
			this.m_pFilter = new TextBox();
			this.m_pFilter.Size = new Size(150, 20);
			this.m_pFilter.Location = new Point(115, 20);
			this.m_pFilter.Text = "*";
			this.m_pGetGroups = new Button();
			this.m_pGetGroups.Size = new Size(70, 20);
			this.m_pGetGroups.Location = new Point(280, 20);
			this.m_pGetGroups.Text = "Get";
			this.m_pGetGroups.Click += new EventHandler(this.m_pGetGroups_Click);
			this.m_pGroupsImages = new ImageList();
			this.m_pGroupsImages.Images.Add(ResManager.GetIcon("group.ico"));
			this.m_pGroupsImages.Images.Add(ResManager.GetIcon("group_disabled.ico"));
			this.m_pGroups = new WListView();
			this.m_pGroups.Size = new Size(270, 200);
			this.m_pGroups.Location = new Point(10, 50);
			this.m_pGroups.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pGroups.View = View.Details;
			this.m_pGroups.FullRowSelect = true;
			this.m_pGroups.HideSelection = false;
			this.m_pGroups.SmallImageList = this.m_pGroupsImages;
			this.m_pGroups.SelectedIndexChanged += new EventHandler(this.m_pGroups_SelectedIndexChanged);
			this.m_pGroups.DoubleClick += new EventHandler(this.m_pGroups_DoubleClick);
			this.m_pGroups.MouseUp += new MouseEventHandler(this.m_pGroups_MouseUp);
			this.m_pGroups.Columns.Add("Name", 190, HorizontalAlignment.Left);
			this.m_pGroups.Columns.Add("Description", 290, HorizontalAlignment.Left);
			base.Controls.Add(this.m_pToolbar);
			base.Controls.Add(this.mt_Filter);
			base.Controls.Add(this.m_pFilter);
			base.Controls.Add(this.m_pGetGroups);
			base.Controls.Add(this.m_pGroups);
		}

		private void m_pToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			this.SwitchToolBarTask(e.ClickedItem.Tag.ToString());
		}

		private void m_pGetGroups_Click(object sender, EventArgs e)
		{
			this.LoadGroups("");
		}

		private void m_pGroups_DoubleClick(object sender, EventArgs e)
		{
			if (this.m_pGroups.SelectedItems.Count > 0)
			{
				Group group = (Group)this.m_pGroups.SelectedItems[0].Tag;
				AddEditGroupForm addEditGroupForm = new AddEditGroupForm(this.m_pVirtualServer, group);
				if (addEditGroupForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadGroups(addEditGroupForm.GroupID);
				}
			}
		}

		private void m_pGroups_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pGroups.Items.Count > 0 && this.m_pGroups.SelectedItems.Count > 0)
			{
				this.m_pToolbar.Items[1].Enabled = true;
				this.m_pToolbar.Items[2].Enabled = true;
				return;
			}
			this.m_pToolbar.Items[1].Enabled = false;
			this.m_pToolbar.Items[2].Enabled = false;
		}

		private void m_pGroups_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right)
			{
				return;
			}
			ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
			contextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pGroups_ContextMenuItem_Clicked);
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("Add");
			toolStripMenuItem.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripMenuItem.Tag = "add";
			contextMenuStrip.Items.Add(toolStripMenuItem);
			ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem("Edit");
			toolStripMenuItem2.Enabled = (this.m_pGroups.SelectedItems.Count > 0);
			toolStripMenuItem2.Tag = "edit";
			toolStripMenuItem2.Image = ResManager.GetIcon("edit.ico").ToBitmap();
			contextMenuStrip.Items.Add(toolStripMenuItem2);
			ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem("Delete");
			toolStripMenuItem3.Enabled = (this.m_pGroups.SelectedItems.Count > 0);
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

		private void m_pGroups_ContextMenuItem_Clicked(object sender, ToolStripItemClickedEventArgs e)
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
				AddEditGroupForm addEditGroupForm = new AddEditGroupForm(this.m_pVirtualServer);
				if (addEditGroupForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadGroups("");
					return;
				}
			}
			else if (taskID == "edit")
			{
				Group group = (Group)this.m_pGroups.SelectedItems[0].Tag;
				AddEditGroupForm addEditGroupForm2 = new AddEditGroupForm(this.m_pVirtualServer, group);
				if (addEditGroupForm2.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadGroups(addEditGroupForm2.GroupID);
					return;
				}
			}
			else if (taskID == "delete")
			{
				Group group2 = (Group)this.m_pGroups.SelectedItems[0].Tag;
				if (MessageBox.Show(this, "Are you sure you want to delete Group '" + group2.GroupName + "' !", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					group2.Owner.Remove(group2);
					this.LoadGroups("");
					return;
				}
			}
			else if (taskID == "refresh")
			{
				this.LoadGroups("");
			}
		}

		private void LoadGroups(string selectedGroupID)
		{
			this.m_pGroups.Items.Clear();
			this.m_pVirtualServer.Groups.Refresh();
			foreach (Group group in this.m_pVirtualServer.Groups)
			{
				if (this.m_pFilter.Text == "" || this.IsAstericMatch(this.m_pFilter.Text, group.GroupName.ToLower()))
				{
					ListViewItem listViewItem = new ListViewItem();
					if (group.Enabled)
					{
						listViewItem.ImageIndex = 0;
					}
					else
					{
						listViewItem.ForeColor = Color.Purple;
						listViewItem.Font = new Font(listViewItem.Font.FontFamily, listViewItem.Font.Size, FontStyle.Strikeout);
						listViewItem.ImageIndex = 1;
					}
					listViewItem.Tag = group;
					listViewItem.Text = group.GroupName;
					listViewItem.SubItems.Add(group.Description);
					this.m_pGroups.Items.Add(listViewItem);
					if (group.GroupID == selectedGroupID)
					{
						listViewItem.Selected = true;
					}
				}
			}
			this.m_pGroups.SortItems();
			this.m_pGroups_SelectedIndexChanged(this, new EventArgs());
		}

		private bool IsAstericMatch(string pattern, string text)
		{
			pattern = pattern.ToLower();
			text = text.ToLower();
			if (pattern == "")
			{
				pattern = "*";
			}
			while (pattern.Length > 0)
			{
				if (pattern.StartsWith("*"))
				{
					if (pattern.IndexOf("*", 1) <= -1)
					{
						return text.EndsWith(pattern.Substring(1));
					}
					string text2 = pattern.Substring(1, pattern.IndexOf("*", 1) - 1);
					if (text.IndexOf(text2) == -1)
					{
						return false;
					}
					text = text.Substring(text.IndexOf(text2) + text2.Length);
					pattern = pattern.Substring(pattern.IndexOf("*", 1));
				}
				else
				{
					if (pattern.IndexOfAny(new char[]
					{
						'*'
					}) <= -1)
					{
						return text == pattern;
					}
					string text3 = pattern.Substring(0, pattern.IndexOfAny(new char[]
					{
						'*'
					}));
					if (!text.StartsWith(text3))
					{
						return false;
					}
					text = text.Substring(text.IndexOf(text3) + text3.Length);
					pattern = pattern.Substring(pattern.IndexOfAny(new char[]
					{
						'*'
					}));
				}
			}
			return true;
		}
	}
}
