using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class UsersForm : Form
	{
		private ToolStrip m_pToolbar;

		private Label mt_Filter;

		private TextBox m_pFilter;

		private Button m_pGetUsers;

		private ImageList m_pUsersImages;

		private WListView m_pUsers;

		private VirtualServer m_pVirtualServer;

		public UsersForm(VirtualServer virtualServer, WFrame frame)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			frame.Frame_ToolStrip = this.m_pToolbar;
			this.LoadUsers("");
		}

		private void InitializeComponent()
		{
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
			this.m_pGetUsers = new Button();
			this.m_pGetUsers.Size = new Size(70, 20);
			this.m_pGetUsers.Location = new Point(280, 20);
			this.m_pGetUsers.Text = "Get";
			this.m_pGetUsers.Click += new EventHandler(this.m_pGetUsers_Click);
			this.m_pUsersImages = new ImageList();
			this.m_pUsersImages.Images.Add(ResManager.GetIcon("user.ico"));
			this.m_pUsersImages.Images.Add(ResManager.GetIcon("user_disabled.ico"));
			this.m_pUsers = new WListView();
			this.m_pUsers.Size = new Size(270, 200);
			this.m_pUsers.Location = new Point(10, 50);
			this.m_pUsers.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pUsers.View = View.Details;
			this.m_pUsers.FullRowSelect = true;
			this.m_pUsers.HideSelection = false;
			this.m_pUsers.SmallImageList = this.m_pUsersImages;
			this.m_pUsers.SelectedIndexChanged += new EventHandler(this.m_pUsers_SelectedIndexChanged);
			this.m_pUsers.DoubleClick += new EventHandler(this.m_pUsers_DoubleClick);
			this.m_pUsers.MouseUp += new MouseEventHandler(this.m_pUsers_MouseUp);
			this.m_pUsers.Columns.Add("Name", 190, HorizontalAlignment.Left);
			this.m_pUsers.Columns.Add("Description", 290, HorizontalAlignment.Left);
			base.Controls.Add(this.mt_Filter);
			base.Controls.Add(this.m_pFilter);
			base.Controls.Add(this.m_pGetUsers);
			base.Controls.Add(this.m_pUsers);
		}

		private void m_pToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			this.SwitchToolBarTask(e.ClickedItem.Tag.ToString());
		}

		private void m_pGetUsers_Click(object sender, EventArgs e)
		{
			this.LoadUsers("");
		}

		private void m_pUsers_DoubleClick(object sender, EventArgs e)
		{
			if (this.m_pUsers.SelectedItems.Count > 0)
			{
                MailServer.Management.User user = (MailServer.Management.User)this.m_pUsers.SelectedItems[0].Tag;
				AddEditUserSettingForm addEditUserSettingForm = new AddEditUserSettingForm(this.m_pVirtualServer, user);
				if (addEditUserSettingForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadUsers(addEditUserSettingForm.UserID);
				}
			}
		}

		private void m_pUsers_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pUsers.Items.Count > 0 && this.m_pUsers.SelectedItems.Count > 0)
			{
				this.m_pToolbar.Items[1].Enabled = true;
				this.m_pToolbar.Items[2].Enabled = true;
				return;
			}
			this.m_pToolbar.Items[1].Enabled = false;
			this.m_pToolbar.Items[2].Enabled = false;
		}

		private void m_pUsers_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right)
			{
				return;
			}
			ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
			contextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pUsers_ContextMenuItem_Clicked);
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("Add");
			toolStripMenuItem.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripMenuItem.Tag = "add";
			contextMenuStrip.Items.Add(toolStripMenuItem);
			ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem("Edit");
			toolStripMenuItem2.Enabled = (this.m_pUsers.SelectedItems.Count > 0);
			toolStripMenuItem2.Tag = "edit";
			toolStripMenuItem2.Image = ResManager.GetIcon("edit.ico").ToBitmap();
			contextMenuStrip.Items.Add(toolStripMenuItem2);
			ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem("Delete");
			toolStripMenuItem3.Enabled = (this.m_pUsers.SelectedItems.Count > 0);
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

		private void m_pUsers_ContextMenuItem_Clicked(object sender, ToolStripItemClickedEventArgs e)
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
				AddEditUserSettingForm addEditUserSettingForm = new AddEditUserSettingForm(this.m_pVirtualServer);
				if (addEditUserSettingForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadUsers(addEditUserSettingForm.UserID);
					return;
				}
			}
			else if (taskID == "edit")
			{
				User user = (User)this.m_pUsers.SelectedItems[0].Tag;
				AddEditUserSettingForm addEditUserSettingForm2 = new AddEditUserSettingForm(this.m_pVirtualServer, user);
				if (addEditUserSettingForm2.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadUsers(addEditUserSettingForm2.UserID);
					return;
				}
			}
			else if (taskID == "delete")
			{
				User user2 = (User)this.m_pUsers.SelectedItems[0].Tag;
				if (MessageBox.Show(this, "Are you sure you want to delete User '" + user2.UserName + "' !", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					user2.Owner.Remove(user2);
					this.m_pUsers.SelectedItems[0].Remove();
					return;
				}
			}
			else if (taskID == "refresh")
			{
				this.LoadUsers("");
			}
		}

		private void LoadUsers(string selectedUserID)
		{
			this.m_pUsers.Items.Clear();
			this.m_pVirtualServer.Users.Refresh();
			foreach (User user in this.m_pVirtualServer.Users)
			{
				if (this.m_pFilter.Text == "" || this.IsAstericMatch(this.m_pFilter.Text, user.UserName.ToLower()))
				{
					ListViewItem listViewItem = new ListViewItem();
					if (user.Enabled)
					{
						listViewItem.ImageIndex = 0;
					}
					else
					{
						listViewItem.ForeColor = Color.Purple;
						listViewItem.Font = new Font(listViewItem.Font.FontFamily, listViewItem.Font.Size, FontStyle.Strikeout);
						listViewItem.ImageIndex = 1;
					}
					listViewItem.Tag = user;
					listViewItem.Text = user.UserName;
					listViewItem.SubItems.Add(user.Description);
					this.m_pUsers.Items.Add(listViewItem);
					if (user.UserID == selectedUserID)
					{
						listViewItem.Selected = true;
					}
				}
			}
			this.m_pUsers.SortItems();
			this.m_pUsers_SelectedIndexChanged(this, new EventArgs());
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
