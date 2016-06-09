using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class AddEditGroupForm : Form
	{
		private TabControl m_pTab;

		private Button m_pCancel;

		private Button m_pOk;

		private CheckBox m_pTab_General_Enabled;

		private Label mt_Tab_General_Name;

		private TextBox m_pTab_General_Name;

		private Label mt_Tab_General_Description;

		private TextBox m_pTab_General_Description;

		private ImageList m_pTab_General_GroupMembersImages;

		private Label mt_Tab_General_GroupMembers;

		private ToolStrip m_pTab_General_Toolbar;

		private ListView m_pTab_General_GroupMembers;

		private VirtualServer m_pVirtualServer;

		private Group m_pGroup;

		public string GroupID
		{
			get
			{
				if (this.m_pGroup != null)
				{
					return this.m_pGroup.GroupID;
				}
				return "";
			}
		}

		public AddEditGroupForm(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
		}

		public AddEditGroupForm(VirtualServer virtualServer, Group group)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pGroup = group;
			this.InitializeComponent();
			this.m_pTab_General_Name.Text = group.GroupName;
			this.m_pTab_General_Description.Text = group.Description;
			this.m_pTab_General_Enabled.Checked = group.Enabled;
			this.LoadMembers();
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(392, 373);
			this.MinimumSize = new Size(400, 400);
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Add/Edit Group";
			base.Icon = ResManager.GetIcon("group.ico");
			this.m_pTab = new TabControl();
			this.m_pTab.Size = new Size(393, 330);
			this.m_pTab.Location = new Point(0, 5);
			this.m_pTab.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pTab.TabPages.Add(new TabPage("General"));
			this.m_pTab.TabPages[0].Size = new Size(387, 304);
			this.m_pCancel = new Button();
			this.m_pCancel.Size = new Size(70, 20);
			this.m_pCancel.Location = new Point(240, 345);
			this.m_pCancel.Text = "Cancel";
			this.m_pCancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			this.m_pOk = new Button();
			this.m_pOk.Size = new Size(70, 20);
			this.m_pOk.Location = new Point(315, 345);
			this.m_pOk.Text = "Ok";
			this.m_pOk.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
			base.Controls.Add(this.m_pTab);
			base.Controls.Add(this.m_pCancel);
			base.Controls.Add(this.m_pOk);
			this.m_pTab_General_Enabled = new CheckBox();
			this.m_pTab_General_Enabled.Size = new Size(100, 13);
			this.m_pTab_General_Enabled.Location = new Point(115, 15);
			this.m_pTab_General_Enabled.Text = "Enabled";
			this.m_pTab_General_Enabled.Checked = true;
			this.mt_Tab_General_Name = new Label();
			this.mt_Tab_General_Name.Size = new Size(100, 13);
			this.mt_Tab_General_Name.Location = new Point(9, 35);
			this.mt_Tab_General_Name.Text = "Name:";
			this.m_pTab_General_Name = new TextBox();
			this.m_pTab_General_Name.Size = new Size(265, 13);
			this.m_pTab_General_Name.Location = new Point(115, 35);
			this.mt_Tab_General_Description = new Label();
			this.mt_Tab_General_Description.Size = new Size(100, 13);
			this.mt_Tab_General_Description.Location = new Point(9, 60);
			this.mt_Tab_General_Description.Text = "Description:";
			this.m_pTab_General_Description = new TextBox();
			this.m_pTab_General_Description.Size = new Size(265, 13);
			this.m_pTab_General_Description.Location = new Point(115, 60);
			this.m_pTab_General_GroupMembersImages = new ImageList();
			this.m_pTab_General_GroupMembersImages.Images.Add(ResManager.GetIcon("group.ico"));
			this.m_pTab_General_GroupMembersImages.Images.Add(ResManager.GetIcon("user.ico"));
			this.mt_Tab_General_GroupMembers = new Label();
			this.mt_Tab_General_GroupMembers.Size = new Size(100, 13);
			this.mt_Tab_General_GroupMembers.Location = new Point(9, 90);
			this.mt_Tab_General_GroupMembers.Text = "Members:";
			this.m_pTab_General_Toolbar = new ToolStrip();
			this.m_pTab_General_Toolbar.AutoSize = false;
			this.m_pTab_General_Toolbar.Size = new Size(100, 25);
			this.m_pTab_General_Toolbar.Location = new Point(330, 85);
			this.m_pTab_General_Toolbar.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.m_pTab_General_Toolbar.Dock = DockStyle.None;
			this.m_pTab_General_Toolbar.GripStyle = ToolStripGripStyle.Hidden;
			this.m_pTab_General_Toolbar.BackColor = this.BackColor;
			this.m_pTab_General_Toolbar.Renderer = new ToolBarRendererEx();
			this.m_pTab_General_Toolbar.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pTab_General_Toolbar_ItemClicked);
			ToolStripButton toolStripButton = new ToolStripButton();
			toolStripButton.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripButton.Tag = "add";
			this.m_pTab_General_Toolbar.Items.Add(toolStripButton);
			ToolStripButton toolStripButton2 = new ToolStripButton();
			toolStripButton2.Enabled = false;
			toolStripButton2.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			toolStripButton2.Tag = "delete";
			this.m_pTab_General_Toolbar.Items.Add(toolStripButton2);
			this.m_pTab_General_GroupMembers = new ListView();
			this.m_pTab_General_GroupMembers.Size = new Size(370, 180);
			this.m_pTab_General_GroupMembers.Location = new Point(9, 110);
			this.m_pTab_General_GroupMembers.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pTab_General_GroupMembers.View = View.Details;
			this.m_pTab_General_GroupMembers.HideSelection = false;
			this.m_pTab_General_GroupMembers.FullRowSelect = true;
			this.m_pTab_General_GroupMembers.SmallImageList = this.m_pTab_General_GroupMembersImages;
			this.m_pTab_General_GroupMembers.SelectedIndexChanged += new EventHandler(this.m_pTab_General_GroupMembers_SelectedIndexChanged);
			this.m_pTab_General_GroupMembers.Columns.Add("", 345, HorizontalAlignment.Left);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Enabled);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_Tab_General_Name);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Name);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_Tab_General_Description);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Description);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_Tab_General_GroupMembers);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Toolbar);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_GroupMembers);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_General_Enabled);
		}

		private void m_pTab_General_Toolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			if (e.ClickedItem.Tag.ToString() == "add")
			{
				if (this.m_pTab_General_Name.Text == "")
				{
					MessageBox.Show(this, "Please fill Group name !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				if (this.m_pGroup == null)
				{
					this.m_pGroup = this.m_pVirtualServer.Groups.Add(this.m_pTab_General_Name.Text, this.m_pTab_General_Description.Text, this.m_pTab_General_Enabled.Checked);
				}
				List<string> list = new List<string>();
				list.Add(this.m_pTab_General_Name.Text.ToLower());
				foreach (ListViewItem listViewItem in this.m_pTab_General_GroupMembers.Items)
				{
					list.Add(listViewItem.Text.ToLower());
				}
				SelectUserOrGroupForm selectUserOrGroupForm = new SelectUserOrGroupForm(this.m_pVirtualServer, true, false, list);
				if (selectUserOrGroupForm.ShowDialog(this) == DialogResult.OK)
				{
					this.m_pGroup.Members.Add(selectUserOrGroupForm.SelectedUserOrGroup);
					this.m_pTab_General_GroupMembers.SelectedItems.Clear();
					ListViewItem listViewItem2 = new ListViewItem(selectUserOrGroupForm.SelectedUserOrGroup);
					listViewItem2.Selected = true;
					if (this.m_pVirtualServer.Groups.Contains(selectUserOrGroupForm.SelectedUserOrGroup))
					{
						listViewItem2.ImageIndex = 0;
					}
					else
					{
						listViewItem2.ImageIndex = 1;
					}
					this.m_pTab_General_GroupMembers.Items.Add(listViewItem2);
					return;
				}
			}
			else if (e.ClickedItem.Tag.ToString() == "delete")
			{
				this.m_pGroup.Members.Remove(this.m_pTab_General_GroupMembers.SelectedItems[0].Text);
				this.m_pTab_General_GroupMembers.SelectedItems[0].Remove();
			}
		}

		private void m_pTab_General_GroupMembers_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pTab_General_GroupMembers.SelectedItems.Count > 0)
			{
				this.m_pTab_General_Toolbar.Items[1].Enabled = true;
				return;
			}
			this.m_pTab_General_Toolbar.Items[1].Enabled = false;
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
		}

		private void m_pOk_Click(object sender, EventArgs e)
		{
			if (this.m_pTab_General_Name.Text == "")
			{
				MessageBox.Show(this, "Please fill Group name !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (this.m_pGroup == null)
			{
				this.m_pGroup = this.m_pVirtualServer.Groups.Add(this.m_pTab_General_Name.Text, this.m_pTab_General_Description.Text, this.m_pTab_General_Enabled.Checked);
			}
			else
			{
				this.m_pGroup.GroupName = this.m_pTab_General_Name.Text;
				this.m_pGroup.Description = this.m_pTab_General_Description.Text;
				this.m_pGroup.Enabled = this.m_pTab_General_Enabled.Checked;
				this.m_pGroup.Commit();
			}
			base.DialogResult = DialogResult.OK;
		}

		private void LoadMembers()
		{
			foreach (string text in this.m_pGroup.Members)
			{
				if (this.m_pGroup.Owner.Contains(text))
				{
					this.m_pTab_General_GroupMembers.Items.Add(text, 0);
				}
				else
				{
					this.m_pTab_General_GroupMembers.Items.Add(text, 1);
				}
			}
			this.m_pTab_General_GroupMembers_SelectedIndexChanged(this, new EventArgs());
		}
	}
}
