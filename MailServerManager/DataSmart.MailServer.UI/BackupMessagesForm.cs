using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class BackupMessagesForm : Form
	{
		private delegate void StartNewFolderDelegate(string folderName, int messagesCount);

		private delegate void SetTotalProgressDelegate(int maxValue);

		private delegate void AddErrorDelegate(Exception x);

		private PictureBox m_pIcon;

		private Label m_pTitle;

		private GroupBox m_pSeparator1;

		private GroupBox m_pSeparator2;

		private Button m_pBack;

		private Button m_pNext;

		private Button m_pCancel;

		private Panel m_pDestination;

		private Label mt_Destination_Folder;

		private TextBox m_pDestionation_Folder;

		private Button m_pDestination_GetFolder;

		private Panel m_pUsers;

		private Button m_pUsers_SelectAll;

		private WTreeView m_pUsers_Users;

		private Panel m_pFinish;

		private ProgressBar m_pFinish_ProgressTotal;

		private ProgressBar m_pFinish_Progress;

		private ListView m_pFinish_Completed;

		private VirtualServer m_pVirtualServer;

		private string m_Step = "";

		private string m_Folder = "";

		public BackupMessagesForm(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			this.SwitchStep("destination");
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(500, 400);
			base.StartPosition = FormStartPosition.CenterScreen;
			base.SizeGripStyle = SizeGripStyle.Hide;
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			this.Text = "Virtual Server Messages Backup Wizard";
			base.Icon = ResManager.GetIcon("ruleaction.ico");
			this.m_pIcon = new PictureBox();
			this.m_pIcon.Size = new Size(36, 36);
			this.m_pIcon.Location = new Point(10, 5);
			this.m_pIcon.Image = ResManager.GetIcon("ruleaction.ico").ToBitmap();
			this.m_pTitle = new Label();
			this.m_pTitle.Size = new Size(300, 30);
			this.m_pTitle.Location = new Point(50, 10);
			this.m_pTitle.TextAlign = ContentAlignment.MiddleLeft;
			this.m_pSeparator1 = new GroupBox();
			this.m_pSeparator1.Size = new Size(490, 3);
			this.m_pSeparator1.Location = new Point(5, 44);
			this.m_pSeparator2 = new GroupBox();
			this.m_pSeparator2.Size = new Size(490, 3);
			this.m_pSeparator2.Location = new Point(5, 360);
			this.m_pBack = new Button();
			this.m_pBack.Size = new Size(70, 20);
			this.m_pBack.Location = new Point(265, 375);
			this.m_pBack.Text = "Back";
			this.m_pBack.Click += new EventHandler(this.m_pBack_Click);
			this.m_pNext = new Button();
			this.m_pNext.Size = new Size(70, 20);
			this.m_pNext.Location = new Point(340, 375);
			this.m_pNext.Text = "Next";
			this.m_pNext.Click += new EventHandler(this.m_pNext_Click);
			this.m_pCancel = new Button();
			this.m_pCancel.Size = new Size(70, 20);
			this.m_pCancel.Location = new Point(420, 375);
			this.m_pCancel.Text = "Cancel";
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			base.Controls.Add(this.m_pIcon);
			base.Controls.Add(this.m_pTitle);
			base.Controls.Add(this.m_pSeparator1);
			base.Controls.Add(this.m_pSeparator2);
			base.Controls.Add(this.m_pBack);
			base.Controls.Add(this.m_pNext);
			base.Controls.Add(this.m_pCancel);
			this.m_pDestination = new Panel();
			this.m_pDestination.Size = new Size(500, 300);
			this.m_pDestination.Location = new Point(0, 75);
			this.m_pDestination.Visible = false;
			this.mt_Destination_Folder = new Label();
			this.mt_Destination_Folder.Size = new Size(100, 20);
			this.mt_Destination_Folder.Location = new Point(0, 0);
			this.mt_Destination_Folder.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Destination_Folder.Text = "Folder:";
			this.m_pDestionation_Folder = new TextBox();
			this.m_pDestionation_Folder.Size = new Size(270, 20);
			this.m_pDestionation_Folder.Location = new Point(105, 0);
			this.m_pDestionation_Folder.ReadOnly = true;
			this.m_pDestination_GetFolder = new Button();
			this.m_pDestination_GetFolder.Size = new Size(25, 20);
			this.m_pDestination_GetFolder.Location = new Point(380, 0);
			this.m_pDestination_GetFolder.Text = "...";
			this.m_pDestination_GetFolder.Click += new EventHandler(this.m_pDestination_GetFolder_Click);
			this.m_pDestination.Controls.Add(this.mt_Destination_Folder);
			this.m_pDestination.Controls.Add(this.m_pDestionation_Folder);
			this.m_pDestination.Controls.Add(this.m_pDestination_GetFolder);
			base.Controls.Add(this.m_pDestination);
			this.m_pUsers = new Panel();
			this.m_pUsers.Size = new Size(500, 300);
			this.m_pUsers.Location = new Point(0, 50);
			this.m_pUsers.Visible = false;
			this.m_pUsers_SelectAll = new Button();
			this.m_pUsers_SelectAll.Size = new Size(70, 20);
			this.m_pUsers_SelectAll.Location = new Point(10, 0);
			this.m_pUsers_SelectAll.Text = "Select All";
			this.m_pUsers_SelectAll.Click += new EventHandler(this.m_pUsers_SelectAll_Click);
			ImageList imageList = new ImageList();
			imageList.Images.Add(ResManager.GetIcon("user.ico"));
			this.m_pUsers_Users = new WTreeView();
			this.m_pUsers_Users.Size = new Size(480, 265);
			this.m_pUsers_Users.Location = new Point(10, 25);
			this.m_pUsers_Users.CheckBoxes = true;
			this.m_pUsers_Users.ImageList = imageList;
			this.m_pUsers.Controls.Add(this.m_pUsers_SelectAll);
			this.m_pUsers.Controls.Add(this.m_pUsers_Users);
			base.Controls.Add(this.m_pUsers);
			this.m_pFinish = new Panel();
			this.m_pFinish.Size = new Size(500, 300);
			this.m_pFinish.Location = new Point(0, 50);
			this.m_pFinish.Visible = false;
			this.m_pFinish_ProgressTotal = new ProgressBar();
			this.m_pFinish_ProgressTotal.Size = new Size(480, 20);
			this.m_pFinish_ProgressTotal.Location = new Point(10, 0);
			this.m_pFinish_Progress = new ProgressBar();
			this.m_pFinish_Progress.Size = new Size(480, 20);
			this.m_pFinish_Progress.Location = new Point(10, 25);
			ImageList imageList2 = new ImageList();
			imageList2.Images.Add(ResManager.GetIcon("folder32.ico"));
			this.m_pFinish_Completed = new ListView();
			this.m_pFinish_Completed.Size = new Size(480, 250);
			this.m_pFinish_Completed.Location = new Point(10, 50);
			this.m_pFinish_Completed.View = View.Details;
			this.m_pFinish_Completed.FullRowSelect = true;
			this.m_pFinish_Completed.HideSelection = false;
			this.m_pFinish_Completed.SmallImageList = imageList2;
			this.m_pFinish_Completed.Columns.Add("Folder", 340);
			this.m_pFinish_Completed.Columns.Add("Count", 70);
			this.m_pFinish_Completed.Columns.Add("Errors", 50);
			this.m_pFinish_Completed.DoubleClick += new EventHandler(this.m_pFinish_Completed_DoubleClick);
			this.m_pFinish.Controls.Add(this.m_pFinish_ProgressTotal);
			this.m_pFinish.Controls.Add(this.m_pFinish_Progress);
			this.m_pFinish.Controls.Add(this.m_pFinish_Completed);
			base.Controls.Add(this.m_pFinish);
		}

		private void m_pDestination_GetFolder_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
			{
				this.m_pDestionation_Folder.Text = folderBrowserDialog.SelectedPath;
			}
		}

		private void m_pUsers_SelectAll_Click(object sender, EventArgs e)
		{
			foreach (TreeNode treeNode in this.m_pUsers_Users.Nodes)
			{
				treeNode.Checked = true;
			}
		}

		private void m_pFinish_Completed_DoubleClick(object sender, EventArgs e)
		{
			if (this.m_pFinish_Completed.SelectedItems.Count > 0)
			{
				List<Exception> list = (List<Exception>)this.m_pFinish_Completed.SelectedItems[0].Tag;
				Form form = new Form();
				form.Size = new Size(400, 300);
				form.StartPosition = FormStartPosition.CenterScreen;
				form.Text = "Folder: '" + this.m_pFinish_Completed.SelectedItems[0].Text + "' Errors:";
				form.Icon = ResManager.GetIcon("error.ico");
				TextBox textBox = new TextBox();
				textBox.Dock = DockStyle.Fill;
				textBox.Multiline = true;
				StringBuilder stringBuilder = new StringBuilder();
				foreach (Exception current in list)
				{
					stringBuilder.Append(current.Message + "\n\n");
				}
				textBox.Lines = stringBuilder.ToString().Split(new char[]
				{
					'\n'
				});
				textBox.SelectionStart = 0;
				textBox.SelectionLength = 0;
				form.Controls.Add(textBox);
				form.Show();
			}
		}

		private void m_pBack_Click(object sender, EventArgs e)
		{
			if (this.m_Step == "users")
			{
				this.SwitchStep("destination");
				return;
			}
			if (this.m_Step == "finish")
			{
				this.SwitchStep("users");
			}
		}

		private void m_pNext_Click(object sender, EventArgs e)
		{
			if (this.m_Step == "destination")
			{
				if (this.m_pDestionation_Folder.Text == "")
				{
					MessageBox.Show(this, "Please select backup destination !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				this.m_Folder = this.m_pDestionation_Folder.Text;
				foreach (MailServer.Management.User user in this.m_pVirtualServer.Users)
				{
					TreeNode treeNode = new TreeNode(user.UserName);
					treeNode.ImageIndex = 0;
					treeNode.Tag = user;
					this.m_pUsers_Users.Nodes.Add(treeNode);
				}
				this.SwitchStep("users");
				return;
			}
			else
			{
				if (this.m_Step == "users")
				{
					this.SwitchStep("finish");
					return;
				}
				if (this.m_Step == "finish")
				{
					this.m_pTitle.Text = "Backingup messages ...";
					this.m_pBack.Enabled = false;
					this.m_pNext.Enabled = false;
					Thread thread = new Thread(new ThreadStart(this.Start));
					thread.Start();
				}
				return;
			}
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void SwitchStep(string step)
		{
			if (step == "destination")
			{
				this.m_pTitle.Text = "Please select backup location.";
				this.m_pDestination.Visible = true;
				this.m_pUsers.Visible = false;
				this.m_pFinish.Visible = false;
				this.m_pBack.Enabled = false;
				this.m_pNext.Enabled = true;
				this.m_pCancel.Enabled = true;
			}
			else if (step == "users")
			{
				this.m_pTitle.Text = "Please select user who's messages to backup.";
				this.m_pDestination.Visible = false;
				this.m_pUsers.Visible = true;
				this.m_pFinish.Visible = false;
				this.m_pBack.Enabled = true;
				this.m_pNext.Enabled = true;
				this.m_pCancel.Enabled = true;
			}
			else if (step == "finish")
			{
				this.m_pTitle.Text = "Click start to begin.";
				this.m_pDestination.Visible = false;
				this.m_pUsers.Visible = false;
				this.m_pFinish.Visible = true;
				this.m_pBack.Enabled = true;
				this.m_pNext.Enabled = true;
				this.m_pCancel.Enabled = true;
				this.m_pNext.Text = "Start";
			}
			this.m_Step = step;
		}

		private void Start()
		{
			List<User> list = new List<User>();
			foreach (TreeNode treeNode in this.m_pUsers_Users.Nodes)
			{
				if (treeNode.Checked)
				{
					list.Add((User)treeNode.Tag);
				}
			}
			base.Invoke(new BackupMessagesForm.SetTotalProgressDelegate(this.SetTotalProgress), new object[]
			{
				list.Count
			});
			foreach (User current in list)
			{
				List<UserFolder> list2 = new List<UserFolder>();
				Stack<Queue<UserFolder>> stack = new Stack<Queue<UserFolder>>();
				Queue<UserFolder> queue = new Queue<UserFolder>();
				foreach (UserFolder item in current.Folders)
				{
					queue.Enqueue(item);
				}
				stack.Push(queue);
				while (stack.Count > 0)
				{
					UserFolder userFolder = stack.Peek().Dequeue();
					if (stack.Peek().Count == 0)
					{
						stack.Pop();
					}
					list2.Add(userFolder);
					if (userFolder.ChildFolders.Count > 0)
					{
						Queue<UserFolder> queue2 = new Queue<UserFolder>();
						foreach (UserFolder item2 in userFolder.ChildFolders)
						{
							queue2.Enqueue(item2);
						}
						stack.Push(queue2);
					}
				}
				try
				{
					ZipArchive zipArchive = ZipFile.Open(this.m_Folder + "/" + current.UserName + ".zip", ZipArchiveMode.Create);
					foreach (UserFolder current2 in list2)
					{
						DataSet messagesInfo = current2.GetMessagesInfo();
						if (!messagesInfo.Tables.Contains("MessagesInfo"))
						{
							messagesInfo.Tables.Add("MessagesInfo");
						}
						base.Invoke(new BackupMessagesForm.StartNewFolderDelegate(this.StartNewFolder), new object[]
						{
							current.UserName + "/" + current2.FolderFullPath,
							messagesInfo.Tables["MessagesInfo"].Rows.Count
						});
						try
						{
							foreach (DataRow dataRow in messagesInfo.Tables["MessagesInfo"].Rows)
							{
								try
								{
									ZipArchiveEntry zipArchiveEntry = zipArchive.CreateEntry(current2.FolderFullPath.Replace("/", "\\") + "\\" + Guid.NewGuid().ToString() + ".eml", CompressionLevel.Optimal);
									using (Stream stream = zipArchiveEntry.Open())
									{
										current2.GetMessage(dataRow["ID"].ToString(), stream);
									}
								}
								catch (Exception ex)
								{
									base.Invoke(new BackupMessagesForm.AddErrorDelegate(this.AddError), new object[]
									{
										ex
									});
								}
								base.Invoke(new MethodInvoker(this.IncreaseMessages));
							}
						}
						catch (Exception ex2)
						{
							base.Invoke(new BackupMessagesForm.AddErrorDelegate(this.AddError), new object[]
							{
								ex2
							});
						}
					}
					zipArchive.Dispose();
				}
				catch (Exception ex3)
				{
					base.Invoke(new BackupMessagesForm.AddErrorDelegate(this.AddError), new object[]
					{
						ex3
					});
				}
				base.Invoke(new MethodInvoker(this.IncreaseTotal));
			}
			base.Invoke(new MethodInvoker(this.Finish));
		}

		private void StartNewFolder(string folderName, int messagesCount)
		{
			this.m_pFinish_Progress.Value = 0;
			this.m_pFinish_Progress.Maximum = messagesCount;
			ListViewItem listViewItem = new ListViewItem(folderName);
			listViewItem.ImageIndex = 0;
			listViewItem.Tag = new List<Exception>();
			listViewItem.SubItems.Add("0");
			listViewItem.SubItems.Add("0");
			this.m_pFinish_Completed.Items.Add(listViewItem);
			listViewItem.EnsureVisible();
		}

		private void SetTotalProgress(int maxValue)
		{
			this.m_pFinish_ProgressTotal.Maximum = maxValue;
		}

		private void IncreaseTotal()
		{
			this.m_pFinish_ProgressTotal.Value++;
		}

		private void IncreaseMessages()
		{
			this.m_pFinish_Progress.Value++;
			this.m_pFinish_Completed.Items[this.m_pFinish_Completed.Items.Count - 1].SubItems[1].Text = this.m_pFinish_Progress.Value.ToString() + "/" + this.m_pFinish_Progress.Maximum.ToString();
		}

		private void AddError(Exception x)
		{
			if (this.m_pFinish_Completed.Items.Count == 0)
			{
				MessageBox.Show("Error:" + x.ToString(), "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			ListViewItem listViewItem = this.m_pFinish_Completed.Items[this.m_pFinish_Completed.Items.Count - 1];
			List<Exception> list = (List<Exception>)listViewItem.Tag;
			list.Add(x);
			listViewItem.SubItems[2].Text = list.Count.ToString();
		}

		private void Finish()
		{
			this.m_pTitle.Text = "Completed.";
			this.m_pFinish_ProgressTotal.Value = 0;
			this.m_pFinish_Progress.Value = 0;
			this.m_pCancel.Text = "Finish";
		}
	}
}
