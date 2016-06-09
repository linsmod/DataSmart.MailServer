using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class SelectUserFolderForm : Form
	{
		private ImageList m_pFoldersImages;

		private TreeView m_pFolders;

		private GroupBox m_pGroupBox1;

		private Button m_pCancel;

		private Button m_pOk;

		private VirtualServer m_pVirtualServer;

		private string m_User = "";

		private string m_SelectedFolder;

		public string SelectedFolder
		{
			get
			{
				return this.m_SelectedFolder;
			}
		}

		public SelectUserFolderForm(VirtualServer virtualServer, string user)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_User = user;
			this.InitializeComponent();
			this.LoadFolders();
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(392, 373);
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Select User Folder";
			this.m_pFoldersImages = new ImageList();
			this.m_pFoldersImages.Images.Add(ResManager.GetIcon("folder.ico"));
			this.m_pFolders = new TreeView();
			this.m_pFolders.Size = new Size(370, 270);
			this.m_pFolders.Location = new Point(10, 50);
			this.m_pFolders.HideSelection = false;
			this.m_pFolders.FullRowSelect = true;
			this.m_pFolders.ImageList = this.m_pFoldersImages;
			this.m_pFolders.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pFolders.DoubleClick += new EventHandler(this.m_pFolders_DoubleClick);
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
			base.Controls.Add(this.m_pFolders);
			base.Controls.Add(this.m_pGroupBox1);
			base.Controls.Add(this.m_pCancel);
			base.Controls.Add(this.m_pOk);
		}

		private void m_pFolders_DoubleClick(object sender, EventArgs e)
		{
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
		}

		private void m_pOk_Click(object sender, EventArgs e)
		{
			if (this.m_pFolders.SelectedNode != null)
			{
				this.m_SelectedFolder = this.m_pFolders.SelectedNode.Tag.ToString();
				base.DialogResult = DialogResult.OK;
				return;
			}
			base.DialogResult = DialogResult.Cancel;
		}

		private void LoadFolders()
		{
			this.m_pFolders.Nodes.Clear();
			Queue<object> queue = new Queue<object>();
			IEnumerator enumerator = this.m_pVirtualServer.Users.GetUserByName(this.m_User).Folders.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					UserFolder userFolder = (UserFolder)enumerator.Current;
					TreeNode treeNode = new TreeNode(userFolder.FolderName);
					treeNode.ImageIndex = 0;
					treeNode.Tag = userFolder.FolderFullPath;
					this.m_pFolders.Nodes.Add(treeNode);
					queue.Enqueue(new object[]
					{
						userFolder,
						treeNode
					});
				}
				goto IL_162;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			IL_B5:
			object[] array = (object[])queue.Dequeue();
			UserFolder userFolder2 = (UserFolder)array[0];
			TreeNode treeNode2 = (TreeNode)array[1];
			foreach (UserFolder userFolder3 in userFolder2.ChildFolders)
			{
				TreeNode treeNode3 = new TreeNode(userFolder3.FolderName);
				treeNode3.ImageIndex = 0;
				treeNode3.Tag = userFolder3.FolderFullPath;
				treeNode2.Nodes.Add(treeNode3);
				queue.Enqueue(new object[]
				{
					userFolder3,
					treeNode3
				});
			}
			IL_162:
			if (queue.Count <= 0)
			{
				return;
			}
			goto IL_B5;
		}
	}
}
