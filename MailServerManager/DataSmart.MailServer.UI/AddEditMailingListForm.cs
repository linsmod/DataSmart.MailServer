using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class AddEditMailingListForm : Form
	{
		private TabControl m_pTab;

		private GroupBox m_pGroupBox1;

		private Button m_pCancel;

		private Button m_pOk;

		private Label mt_MailingListName;

		private TextBox m_pMailingListName;

		private Label mt_At;

		private ComboBox m_pDomains;

		private Label mt_Description;

		private TextBox m_pDescription;

		private CheckBox m_pEnabled;

		private Label mt_Member;

		private TextBox m_pMember;

		private Button m_pGetUserOrGroup;

		private ListView m_pMembers;

		private Button m_pMembers_Add;

		private Button m_pMembers_Remove;

		private ImageList m_pAccessImages;

		private ListView m_pAccess;

		private Button m_pAccess_Add;

		private Button m_pAccess_Remove;

		private VirtualServer m_pVirtualServer;

		private MailingList m_pMailingList;

		public string MailingListID
		{
			get
			{
				if (this.m_pMailingList != null)
				{
					return this.m_pMailingList.ID;
				}
				return "";
			}
		}

		public AddEditMailingListForm(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			this.LoadDomains();
		}

		public AddEditMailingListForm(VirtualServer virtualServer, MailingList mailingList)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pMailingList = mailingList;
			this.InitializeComponent();
			this.LoadDomains();
			this.m_pMailingListName.Text = mailingList.Name.Split(new char[]
			{
				'@'
			})[0];
			this.m_pDomains.SelectedText = mailingList.Name.Split(new char[]
			{
				'@'
			})[1];
			this.m_pDescription.Text = mailingList.Description;
			this.m_pEnabled.Checked = mailingList.Enabled;
			this.LoadMembers("");
			this.LoadAccess("");
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(392, 323);
			this.MinimumSize = new Size(400, 350);
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Add/Edit Mailing List";
			this.m_pTab = new TabControl();
			this.m_pTab.Size = new Size(393, 280);
			this.m_pTab.Location = new Point(0, 5);
			this.m_pTab.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pTab.TabPages.Add(new TabPage("General"));
			this.m_pTab.TabPages.Add(new TabPage("Members"));
			this.m_pTab.TabPages.Add(new TabPage("Access"));
			this.m_pGroupBox1 = new GroupBox();
			this.m_pGroupBox1.Size = new Size(385, 4);
			this.m_pGroupBox1.Location = new Point(5, 290);
			this.m_pGroupBox1.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pCancel = new Button();
			this.m_pCancel.Size = new Size(70, 20);
			this.m_pCancel.Location = new Point(245, 300);
			this.m_pCancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.m_pCancel.Text = "Cancel";
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			this.m_pOk = new Button();
			this.m_pOk.Size = new Size(70, 20);
			this.m_pOk.Location = new Point(320, 300);
			this.m_pOk.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.m_pOk.Text = "Ok";
			this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
			base.Controls.Add(this.m_pTab);
			base.Controls.Add(this.m_pGroupBox1);
			base.Controls.Add(this.m_pCancel);
			base.Controls.Add(this.m_pOk);
			this.mt_MailingListName = new Label();
			this.mt_MailingListName.Size = new Size(200, 13);
			this.mt_MailingListName.Location = new Point(5, 10);
			this.mt_MailingListName.Text = "Name:";
			this.m_pMailingListName = new TextBox();
			this.m_pMailingListName.Size = new Size(190, 20);
			this.m_pMailingListName.Location = new Point(5, 25);
			this.mt_At = new Label();
			this.mt_At.Size = new Size(13, 13);
			this.mt_At.Location = new Point(195, 28);
			this.mt_At.Text = "@";
			this.m_pDomains = new ComboBox();
			this.m_pDomains.Size = new Size(170, 20);
			this.m_pDomains.Location = new Point(210, 25);
			this.m_pDomains.DropDownStyle = ComboBoxStyle.DropDownList;
			this.mt_Description = new Label();
			this.mt_Description.Size = new Size(200, 13);
			this.mt_Description.Location = new Point(5, 55);
			this.mt_Description.Text = "Description:";
			this.m_pDescription = new TextBox();
			this.m_pDescription.Size = new Size(375, 20);
			this.m_pDescription.Location = new Point(5, 70);
			this.m_pEnabled = new CheckBox();
			this.m_pEnabled.Size = new Size(265, 13);
			this.m_pEnabled.Location = new Point(5, 105);
			this.m_pEnabled.Checked = true;
			this.m_pEnabled.Text = "Enabled";
			this.m_pTab.TabPages[0].Controls.Add(this.mt_MailingListName);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pMailingListName);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_At);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pDomains);
			this.m_pTab.TabPages[0].Controls.Add(this.mt_Description);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pDescription);
			this.m_pTab.TabPages[0].Controls.Add(this.m_pEnabled);
			this.mt_Member = new Label();
			this.mt_Member.Size = new Size(50, 13);
			this.mt_Member.Location = new Point(5, 22);
			this.mt_Member.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Member.Text = "Member:";
			this.m_pMember = new TextBox();
			this.m_pMember.Size = new Size(215, 20);
			this.m_pMember.Location = new Point(65, 20);
			this.m_pGetUserOrGroup = new Button();
			this.m_pGetUserOrGroup.Size = new Size(20, 20);
			this.m_pGetUserOrGroup.Location = new Point(285, 20);
			this.m_pGetUserOrGroup.Image = ResManager.GetIcon("group.ico").ToBitmap();
			this.m_pGetUserOrGroup.Click += new EventHandler(this.m_pGetUserOrGroup_Click);
			this.m_pMembers = new ListView();
			this.m_pMembers.Size = new Size(300, 200);
			this.m_pMembers.Location = new Point(5, 45);
			this.m_pMembers.View = View.List;
			this.m_pMembers.FullRowSelect = true;
			this.m_pMembers.HideSelection = false;
			this.m_pMembers.SelectedIndexChanged += new EventHandler(this.m_pMembers_SelectedIndexChanged);
			this.m_pMembers_Add = new Button();
			this.m_pMembers_Add.Size = new Size(70, 20);
			this.m_pMembers_Add.Location = new Point(310, 20);
			this.m_pMembers_Add.Text = "Add";
			this.m_pMembers_Add.Click += new EventHandler(this.m_pMembers_Add_Click);
			this.m_pMembers_Remove = new Button();
			this.m_pMembers_Remove.Size = new Size(70, 20);
			this.m_pMembers_Remove.Location = new Point(310, 45);
			this.m_pMembers_Remove.Text = "Remove";
			this.m_pMembers_Remove.Click += new EventHandler(this.m_pMembers_Remove_Click);
			this.m_pTab.TabPages[1].Controls.Add(this.mt_Member);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pMember);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pGetUserOrGroup);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pMembers);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pMembers_Add);
			this.m_pTab.TabPages[1].Controls.Add(this.m_pMembers_Remove);
			this.m_pAccessImages = new ImageList();
			this.m_pAccessImages.Images.Add(ResManager.GetIcon("user.ico"));
			this.m_pAccessImages.Images.Add(ResManager.GetIcon("group.ico"));
			this.m_pAccess = new ListView();
			this.m_pAccess.Size = new Size(300, 220);
			this.m_pAccess.Location = new Point(5, 20);
			this.m_pAccess.View = View.List;
			this.m_pAccess.FullRowSelect = true;
			this.m_pAccess.HideSelection = false;
			this.m_pAccess.SmallImageList = this.m_pAccessImages;
			this.m_pAccess.SelectedIndexChanged += new EventHandler(this.m_pAccess_SelectedIndexChanged);
			this.m_pAccess_Add = new Button();
			this.m_pAccess_Add.Size = new Size(70, 20);
			this.m_pAccess_Add.Location = new Point(310, 20);
			this.m_pAccess_Add.Text = "Add";
			this.m_pAccess_Add.Click += new EventHandler(this.m_pAccess_Add_Click);
			this.m_pAccess_Remove = new Button();
			this.m_pAccess_Remove.Size = new Size(70, 20);
			this.m_pAccess_Remove.Location = new Point(310, 45);
			this.m_pAccess_Remove.Text = "Remove";
			this.m_pAccess_Remove.Click += new EventHandler(this.m_pAccess_Remove_Click);
			this.m_pTab.TabPages[2].Controls.Add(this.m_pAccess);
			this.m_pTab.TabPages[2].Controls.Add(this.m_pAccess_Add);
			this.m_pTab.TabPages[2].Controls.Add(this.m_pAccess_Remove);
		}

		private void m_pGetUserOrGroup_Click(object sender, EventArgs e)
		{
			SelectUserOrGroupForm selectUserOrGroupForm = new SelectUserOrGroupForm(this.m_pVirtualServer, true, false);
			if (selectUserOrGroupForm.ShowDialog(this) == DialogResult.OK)
			{
				this.m_pMember.Text = selectUserOrGroupForm.SelectedUserOrGroup;
			}
		}

		private void m_pMembers_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pMembers.SelectedItems.Count > 0)
			{
				this.m_pMembers_Remove.Enabled = true;
				return;
			}
			this.m_pMembers_Remove.Enabled = false;
		}

		private void m_pMembers_Add_Click(object sender, EventArgs e)
		{
			if (this.m_pMailingList == null && !this.AddOrUpdate())
			{
				return;
			}
			if (this.m_pMember.Text == "")
			{
				MessageBox.Show(this, "Please fill member name !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			this.m_pMailingList.Members.Add(this.m_pMember.Text);
			this.LoadMembers(this.m_pMember.Text);
			this.m_pMember.Text = "";
		}

		private void m_pMembers_Remove_Click(object sender, EventArgs e)
		{
			if (this.m_pMembers.SelectedItems.Count > 0)
			{
				this.m_pMailingList.Members.Remove(this.m_pMembers.SelectedItems[0].Text);
				this.LoadMembers("");
			}
		}

		private void m_pAccess_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pAccess.SelectedItems.Count > 0)
			{
				this.m_pAccess_Remove.Enabled = true;
				return;
			}
			this.m_pAccess_Remove.Enabled = false;
		}

		private void m_pAccess_Add_Click(object sender, EventArgs e)
		{
			List<string> list = new List<string>();
			foreach (ListViewItem listViewItem in this.m_pAccess.Items)
			{
				list.Add(listViewItem.Text.ToLower());
			}
			SelectUserOrGroupForm selectUserOrGroupForm = new SelectUserOrGroupForm(this.m_pVirtualServer, true, true, true, list);
			if (selectUserOrGroupForm.ShowDialog(this) == DialogResult.OK)
			{
				this.m_pMailingList.ACL.Add(selectUserOrGroupForm.SelectedUserOrGroup);
				this.LoadAccess(selectUserOrGroupForm.SelectedUserOrGroup);
			}
		}

		private void m_pAccess_Remove_Click(object sender, EventArgs e)
		{
			if (this.m_pAccess.SelectedItems.Count > 0)
			{
				this.m_pMailingList.ACL.Remove(this.m_pAccess.SelectedItems[0].Text);
				this.LoadAccess("");
			}
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
			base.Close();
		}

		private void m_pOk_Click(object sender, EventArgs e)
		{
			this.AddOrUpdate();
			base.DialogResult = DialogResult.OK;
			base.Close();
		}

		private bool AddOrUpdate()
		{
			if (this.m_pMailingListName.Text == "")
			{
				MessageBox.Show(this, "Please fill mailing list name !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			if (this.m_pDomains.SelectedIndex == -1)
			{
				MessageBox.Show(this, "Please choose domain !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			if (this.m_pMailingList == null)
			{
				this.m_pMailingList = this.m_pVirtualServer.MailingLists.Add(this.m_pMailingListName.Text + "@" + this.m_pDomains.SelectedItem.ToString(), this.m_pDescription.Text, this.m_pEnabled.Checked);
			}
			else
			{
				this.m_pMailingList.Enabled = this.m_pEnabled.Checked;
				this.m_pMailingList.Name = this.m_pMailingListName.Text + "@" + this.m_pDomains.SelectedItem.ToString();
				this.m_pMailingList.Description = this.m_pDescription.Text;
				this.m_pMailingList.Commit();
			}
			return true;
		}

		private void LoadDomains()
		{
			foreach (Domain domain in this.m_pVirtualServer.Domains)
			{
				this.m_pDomains.Items.Add(domain.DomainName);
			}
			if (this.m_pDomains.Items.Count > 0)
			{
				this.m_pDomains.SelectedIndex = 0;
			}
		}

		private void LoadMembers(string selectedMember)
		{
			this.m_pMembers.Items.Clear();
			foreach (string text in this.m_pMailingList.Members)
			{
				ListViewItem listViewItem = new ListViewItem(text);
				this.m_pMembers.Items.Add(listViewItem);
				if (selectedMember == text)
				{
					listViewItem.Selected = true;
				}
			}
			this.m_pMembers_SelectedIndexChanged(this, null);
		}

		private void LoadAccess(string selectedUserOrGroup)
		{
			this.m_pAccess.Items.Clear();
			foreach (string text in this.m_pMailingList.ACL)
			{
				ListViewItem listViewItem = new ListViewItem(text);
				if (this.m_pVirtualServer.Domains.Contains(text))
				{
					listViewItem.ImageIndex = 1;
				}
				else
				{
					listViewItem.ImageIndex = 0;
				}
				this.m_pAccess.Items.Add(listViewItem);
				if (selectedUserOrGroup == text)
				{
					listViewItem.Selected = true;
				}
			}
			this.m_pAccess_SelectedIndexChanged(this, new EventArgs());
		}
	}
}
