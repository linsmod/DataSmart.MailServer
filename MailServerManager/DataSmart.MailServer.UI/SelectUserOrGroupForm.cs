using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class SelectUserOrGroupForm : Form
	{
		private Label mt_Filter;

		private TextBox m_pFilter;

		private ImageList m_pUsersAndGroupsImages;

		private ListView m_pUsersAndGroups;

		private GroupBox m_pGroupBox1;

		private Button m_pCancel;

		private Button m_pOk;

		private VirtualServer m_pVirtualServer;

		private bool m_ShowGroups = true;

		private bool m_ShowAnyOne;

		private bool m_ShowAuthUsers;

		private List<string> m_pExcludeList;

		private string m_UserOrGroup;

		private bool m_IsGruop;

		public string SelectedUserOrGroup
		{
			get
			{
				return this.m_UserOrGroup;
			}
		}

		public bool IsGroup
		{
			get
			{
				return this.m_IsGruop;
			}
		}

		public SelectUserOrGroupForm(VirtualServer virtualServer, bool showGroups, bool showAnyOne) : this(virtualServer, showGroups, showAnyOne, false, new List<string>())
		{
		}

		public SelectUserOrGroupForm(VirtualServer virtualServer, bool showGroups, bool showAnyOne, List<string> excludeList) : this(virtualServer, showGroups, showAnyOne, false, excludeList)
		{
		}

		public SelectUserOrGroupForm(VirtualServer virtualServer, bool showGroups, bool showAnyOne, bool showAuthUsers, List<string> excludeList)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_ShowGroups = showGroups;
			this.m_ShowAnyOne = showAnyOne;
			this.m_ShowAuthUsers = showAuthUsers;
			this.m_pExcludeList = excludeList;
			this.InitializeComponent();
			this.LoadUsersAndGroups();
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(392, 373);
			base.StartPosition = FormStartPosition.CenterScreen;
			if (this.m_ShowGroups)
			{
				this.Text = "Select User or Group";
			}
			else
			{
				this.Text = "Select User";
			}
			this.mt_Filter = new Label();
			this.mt_Filter.Size = new Size(200, 13);
			this.mt_Filter.Location = new Point(10, 10);
			this.mt_Filter.Text = "Filter:";
			this.m_pFilter = new TextBox();
			this.m_pFilter.Size = new Size(200, 20);
			this.m_pFilter.Location = new Point(10, 25);
			this.m_pFilter.TextChanged += new EventHandler(this.m_pFilter_TextChanged);
			this.m_pUsersAndGroupsImages = new ImageList();
			this.m_pUsersAndGroupsImages.Images.Add(ResManager.GetIcon("group.ico"));
			this.m_pUsersAndGroupsImages.Images.Add(ResManager.GetIcon("group_disabled.ico"));
			this.m_pUsersAndGroupsImages.Images.Add(ResManager.GetIcon("user.ico"));
			this.m_pUsersAndGroupsImages.Images.Add(ResManager.GetIcon("user_disabled.ico"));
			this.m_pUsersAndGroups = new ListView();
			this.m_pUsersAndGroups.Size = new Size(370, 270);
			this.m_pUsersAndGroups.Location = new Point(10, 50);
			this.m_pUsersAndGroups.View = View.Details;
			this.m_pUsersAndGroups.HideSelection = false;
			this.m_pUsersAndGroups.FullRowSelect = true;
			this.m_pUsersAndGroups.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pUsersAndGroups.SmallImageList = this.m_pUsersAndGroupsImages;
			this.m_pUsersAndGroups.DoubleClick += new EventHandler(this.m_pUsersAndGroups_DoubleClick);
			this.m_pUsersAndGroups.Columns.Add("User Name:", 200, HorizontalAlignment.Left);
			this.m_pGroupBox1 = new GroupBox();
			this.m_pGroupBox1.Size = new Size(386, 4);
			this.m_pGroupBox1.Location = new Point(4, 332);
			this.m_pGroupBox1.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pCancel = new Button();
			this.m_pCancel.Size = new Size(70, 20);
			this.m_pCancel.Location = new Point(235, 345);
			this.m_pCancel.Text = "Cancel";
			this.m_pCancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			this.m_pOk = new Button();
			this.m_pOk.Size = new Size(70, 20);
			this.m_pOk.Location = new Point(310, 345);
			this.m_pOk.Text = "Ok";
			this.m_pOk.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
			base.Controls.Add(this.mt_Filter);
			base.Controls.Add(this.m_pFilter);
			base.Controls.Add(this.m_pUsersAndGroups);
			base.Controls.Add(this.m_pGroupBox1);
			base.Controls.Add(this.m_pCancel);
			base.Controls.Add(this.m_pOk);
		}

		private void m_pFilter_TextChanged(object sender, EventArgs e)
		{
			this.LoadUsersAndGroups();
		}

		private void m_pUsersAndGroups_DoubleClick(object sender, EventArgs e)
		{
			if (this.m_pUsersAndGroups.SelectedItems.Count > 0)
			{
				this.m_UserOrGroup = this.m_pUsersAndGroups.SelectedItems[0].Text;
				this.m_IsGruop = (this.m_pUsersAndGroups.SelectedItems[0].ImageIndex == 0);
				base.DialogResult = DialogResult.OK;
			}
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
		}

		private void m_pOk_Click(object sender, EventArgs e)
		{
			if (this.m_pUsersAndGroups.SelectedItems.Count > 0)
			{
				this.m_UserOrGroup = this.m_pUsersAndGroups.SelectedItems[0].Text;
				this.m_IsGruop = (this.m_pUsersAndGroups.SelectedItems[0].ImageIndex == 0);
				base.DialogResult = DialogResult.OK;
				return;
			}
			base.DialogResult = DialogResult.Cancel;
		}

		private void LoadUsersAndGroups()
		{
			this.m_pUsersAndGroups.Items.Clear();
			if (this.m_ShowGroups)
			{
				foreach (Group group in this.m_pVirtualServer.Groups)
				{
					if (!this.m_pExcludeList.Contains(group.GroupName.ToLower()) && (this.m_pFilter.Text == "" || this.IsAstericMatch(this.m_pFilter.Text, group.GroupName.ToLower())))
					{
						if (group.Enabled)
						{
							this.m_pUsersAndGroups.Items.Add(group.GroupName, 0);
						}
						else
						{
							this.m_pUsersAndGroups.Items.Add(group.GroupName, 1);
						}
					}
				}
				if (this.m_ShowAuthUsers)
				{
					this.m_pUsersAndGroups.Items.Add("Authenticated Users", 0);
				}
			}
			if (this.m_ShowAnyOne)
			{
				this.m_pUsersAndGroups.Items.Add("anyone", 2);
			}
			foreach (MailServer.Management.User user in this.m_pVirtualServer.Users)
			{
				if (!this.m_pExcludeList.Contains(user.UserName.ToLower()) && (this.m_pFilter.Text == "" || this.IsAstericMatch(this.m_pFilter.Text, user.UserName.ToLower())))
				{
					if (user.Enabled)
					{
						this.m_pUsersAndGroups.Items.Add(user.UserName, 2);
					}
					else
					{
						this.m_pUsersAndGroups.Items.Add(user.UserName, 3);
					}
				}
			}
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
