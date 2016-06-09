using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class MailingListsForm : Form
	{
		private ToolStrip m_pToolbar;

		private Label mt_Filter;

		private TextBox m_pFilter;

		private Button m_pGetMailingLists;

		private ImageList m_pMailingListsImages;

		private WListView m_pMailingLists;

		private VirtualServer m_pVirtualServer;

		public MailingListsForm(VirtualServer virtualServer, WFrame frame)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			frame.Frame_ToolStrip = this.m_pToolbar;
			this.LoadMailingLists("");
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
			this.m_pFilter.Size = new Size(150, 13);
			this.m_pFilter.Location = new Point(115, 20);
			this.m_pFilter.Text = "*";
			this.m_pGetMailingLists = new Button();
			this.m_pGetMailingLists.Size = new Size(70, 20);
			this.m_pGetMailingLists.Location = new Point(280, 20);
			this.m_pGetMailingLists.Text = "Get";
			this.m_pGetMailingLists.Click += new EventHandler(this.m_pGetMailingLists_Click);
			this.m_pMailingListsImages = new ImageList();
			this.m_pMailingListsImages.Images.Add(ResManager.GetImage("icon-mail.png"));
			this.m_pMailingListsImages.Images.Add(ResManager.GetImage("mailinglist_disabled.ico"));
			this.m_pMailingLists = new WListView();
			this.m_pMailingLists.Size = new Size(270, 200);
			this.m_pMailingLists.Location = new Point(10, 50);
			this.m_pMailingLists.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pMailingLists.View = View.Details;
			this.m_pMailingLists.FullRowSelect = true;
			this.m_pMailingLists.HideSelection = false;
			this.m_pMailingLists.SmallImageList = this.m_pMailingListsImages;
			this.m_pMailingLists.SelectedIndexChanged += new EventHandler(this.m_pMailingLists_SelectedIndexChanged);
			this.m_pMailingLists.DoubleClick += new EventHandler(this.m_pMailingLists_DoubleClick);
			this.m_pMailingLists.MouseUp += new MouseEventHandler(this.m_pMailingLists_MouseUp);
			this.m_pMailingLists.Columns.Add("Name", 190, HorizontalAlignment.Left);
			this.m_pMailingLists.Columns.Add("Description", 290, HorizontalAlignment.Left);
			base.Controls.Add(this.m_pToolbar);
			base.Controls.Add(this.mt_Filter);
			base.Controls.Add(this.m_pFilter);
			base.Controls.Add(this.m_pGetMailingLists);
			base.Controls.Add(this.m_pMailingLists);
		}

		private void m_pToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			this.SwitchToolBarTask(e.ClickedItem.Tag.ToString());
		}

		private void m_pGetMailingLists_Click(object sender, EventArgs e)
		{
			this.LoadMailingLists("");
		}

		private void m_pMailingLists_DoubleClick(object sender, EventArgs e)
		{
			if (this.m_pMailingLists.SelectedItems.Count > 0)
			{
				MailingList mailingList = (MailingList)this.m_pMailingLists.SelectedItems[0].Tag;
				AddEditMailingListForm addEditMailingListForm = new AddEditMailingListForm(this.m_pVirtualServer, mailingList);
				if (addEditMailingListForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadMailingLists(addEditMailingListForm.MailingListID);
				}
			}
		}

		private void m_pMailingLists_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pMailingLists.Items.Count > 0 && this.m_pMailingLists.SelectedItems.Count > 0)
			{
				this.m_pToolbar.Items[1].Enabled = true;
				this.m_pToolbar.Items[2].Enabled = true;
				return;
			}
			this.m_pToolbar.Items[1].Enabled = false;
			this.m_pToolbar.Items[2].Enabled = false;
		}

		private void m_pMailingLists_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right)
			{
				return;
			}
			ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
			contextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pMailingLists_ContextMenuItem_Clicked);
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("Add");
			toolStripMenuItem.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripMenuItem.Tag = "add";
			contextMenuStrip.Items.Add(toolStripMenuItem);
			ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem("Edit");
			toolStripMenuItem2.Enabled = (this.m_pMailingLists.SelectedItems.Count > 0);
			toolStripMenuItem2.Tag = "edit";
			toolStripMenuItem2.Image = ResManager.GetIcon("edit.ico").ToBitmap();
			contextMenuStrip.Items.Add(toolStripMenuItem2);
			ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem("Delete");
			toolStripMenuItem3.Enabled = (this.m_pMailingLists.SelectedItems.Count > 0);
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

		private void m_pMailingLists_ContextMenuItem_Clicked(object sender, ToolStripItemClickedEventArgs e)
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
				AddEditMailingListForm addEditMailingListForm = new AddEditMailingListForm(this.m_pVirtualServer);
				if (addEditMailingListForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadMailingLists(addEditMailingListForm.MailingListID);
					return;
				}
			}
			else if (taskID == "edit")
			{
				MailingList mailingList = (MailingList)this.m_pMailingLists.SelectedItems[0].Tag;
				AddEditMailingListForm addEditMailingListForm2 = new AddEditMailingListForm(this.m_pVirtualServer, mailingList);
				if (addEditMailingListForm2.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadMailingLists(addEditMailingListForm2.MailingListID);
					return;
				}
			}
			else if (taskID == "delete")
			{
				MailingList mailingList2 = (MailingList)this.m_pMailingLists.SelectedItems[0].Tag;
				if (MessageBox.Show(this, "Are you sure you want to delete Mailing List '" + mailingList2.Name + "' !", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					mailingList2.Owner.Remove(mailingList2);
					this.m_pMailingLists.SelectedItems[0].Remove();
					return;
				}
			}
			else if (taskID == "refresh")
			{
				this.LoadMailingLists("");
			}
		}

		private void LoadMailingLists(string selectedMailingListID)
		{
			this.m_pMailingLists.Items.Clear();
			this.m_pVirtualServer.MailingLists.Refresh();
			foreach (MailingList mailingList in this.m_pVirtualServer.MailingLists)
			{
				if (this.m_pFilter.Text == "" || this.IsAstericMatch(this.m_pFilter.Text, mailingList.Name.ToLower()))
				{
					ListViewItem listViewItem = new ListViewItem();
					listViewItem.ImageIndex = 0;
					if (mailingList.Enabled)
					{
						listViewItem.ImageIndex = 0;
					}
					else
					{
						listViewItem.ForeColor = Color.Purple;
						listViewItem.Font = new Font(listViewItem.Font.FontFamily, listViewItem.Font.Size, FontStyle.Strikeout);
						listViewItem.ImageIndex = 1;
					}
					listViewItem.Tag = mailingList;
					listViewItem.Text = mailingList.Name;
					listViewItem.SubItems.Add(mailingList.Description);
					this.m_pMailingLists.Items.Add(listViewItem);
					if (mailingList.ID == selectedMailingListID)
					{
						listViewItem.Selected = true;
					}
				}
			}
			this.m_pMailingLists.SortItems();
			this.m_pMailingLists_SelectedIndexChanged(this, new EventArgs());
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
