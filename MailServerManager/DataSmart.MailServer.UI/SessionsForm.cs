using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class SessionsForm : Form
	{
		private Label mt_Show;

		private ComboBox m_pShow;

		private Button m_pKill;

		private Button m_pViewSession;

		private ImageList m_pSessionsImages;

		private WListView m_pSessions;

		private Server m_pServer;

		private bool m_Run;

		public SessionsForm(Server server)
		{
			this.m_pServer = server;
			this.InitializeComponent();
			this.m_pShow.SelectedIndex = 0;
			this.StartMonitoring();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			this.m_Run = false;
		}

		private void InitializeComponent()
		{
			base.Size = new Size(472, 357);
			this.mt_Show = new Label();
			this.mt_Show.Size = new Size(70, 20);
			this.mt_Show.Location = new Point(10, 20);
			this.mt_Show.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Show.Text = "Show:";
			this.m_pShow = new ComboBox();
			this.m_pShow.Size = new Size(100, 20);
			this.m_pShow.Location = new Point(85, 20);
			this.m_pShow.DropDownStyle = ComboBoxStyle.DropDownList;
			this.m_pShow.SelectedIndexChanged += new EventHandler(this.m_pShow_SelectedIndexChanged);
			this.m_pShow.Items.Add("ALL");
			this.m_pShow.Items.Add("SMTP");
			this.m_pShow.Items.Add("POP3");
			this.m_pShow.Items.Add("IMAP");
			this.m_pShow.Items.Add("RELAY");
			this.m_pShow.Items.Add("ADMIN");
			this.m_pKill = new Button();
			this.m_pKill.Size = new Size(70, 20);
			this.m_pKill.Location = new Point(307, 20);
			this.m_pKill.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.m_pKill.Text = "Kill";
			this.m_pKill.Enabled = false;
			this.m_pKill.Click += new EventHandler(this.m_pKill_Click);
			this.m_pViewSession = new Button();
			this.m_pViewSession.Size = new Size(70, 20);
			this.m_pViewSession.Location = new Point(382, 20);
			this.m_pViewSession.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.m_pViewSession.Text = "View";
			this.m_pViewSession.Enabled = false;
			this.m_pViewSession.Click += new EventHandler(this.m_pViewSession_Click);
			this.m_pSessionsImages = new ImageList();
			this.m_pSessionsImages.Images.Add(ResManager.GetIcon("user.ico"));
			this.m_pSessions = new WListView();
			this.m_pSessions.Size = new Size(445, 265);
			this.m_pSessions.Location = new Point(9, 47);
			this.m_pSessions.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pSessions.View = View.Details;
			this.m_pSessions.FullRowSelect = true;
			this.m_pSessions.HideSelection = false;
			this.m_pSessions.SmallImageList = this.m_pSessionsImages;
			this.m_pSessions.SelectedIndexChanged += new EventHandler(this.m_pSessions_SelectedIndexChanged);
			this.m_pSessions.MouseClick += new MouseEventHandler(this.m_pSessions_MouseClick);
			this.m_pSessions.Columns.Add("Type", 60, HorizontalAlignment.Left);
			this.m_pSessions.Columns.Add("UserName", 80, HorizontalAlignment.Left);
			this.m_pSessions.Columns.Add("LocalEndPoint", 100, HorizontalAlignment.Left);
			this.m_pSessions.Columns.Add("RemoteEndPoint", 100, HorizontalAlignment.Left);
			this.m_pSessions.Columns.Add("R KB/S", 55, HorizontalAlignment.Left);
			this.m_pSessions.Columns.Add("W KB/S", 55, HorizontalAlignment.Left);
			this.m_pSessions.Columns.Add("Session Start", 100, HorizontalAlignment.Left);
			this.m_pSessions.Columns.Add("Timeout after sec.", 100, HorizontalAlignment.Left);
			base.Controls.Add(this.mt_Show);
			base.Controls.Add(this.m_pShow);
			base.Controls.Add(this.m_pKill);
			base.Controls.Add(this.m_pViewSession);
			base.Controls.Add(this.m_pSessions);
		}

		private void m_pShow_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.m_pSessions.Items.Clear();
		}

		private void m_pKill_Click(object sender, EventArgs e)
		{
			this.SwitchToolBarTask("kill");
		}

		private void m_pViewSession_Click(object sender, EventArgs e)
		{
			this.SwitchToolBarTask("view");
		}

		private void m_pSessions_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pSessions.SelectedItems.Count > 0)
			{
				this.m_pKill.Enabled = true;
				this.m_pViewSession.Enabled = true;
				return;
			}
			this.m_pKill.Enabled = false;
			this.m_pViewSession.Enabled = false;
		}

		private void m_pSessions_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right)
			{
				return;
			}
			ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
			contextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pSessions_ContextMenuItem_Clicked);
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("View");
			toolStripMenuItem.Image = ResManager.GetIcon("viewmessages.ico").ToBitmap();
			toolStripMenuItem.Tag = "view";
			contextMenuStrip.Items.Add(toolStripMenuItem);
			ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem("Kill");
			toolStripMenuItem2.Tag = "kill";
			toolStripMenuItem2.Image = ResManager.GetIcon("exit.ico").ToBitmap();
			contextMenuStrip.Items.Add(toolStripMenuItem2);
			contextMenuStrip.Show(Control.MousePosition);
		}

		private void m_pSessions_ContextMenuItem_Clicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			this.SwitchToolBarTask(e.ClickedItem.Tag.ToString());
		}

		private void SwitchToolBarTask(string taskID)
		{
			if (taskID == "view")
			{
				if (this.m_pSessions.SelectedItems.Count > 0)
				{
					Form form = new Form();
					form.Size = new Size(800, 600);
					form.StartPosition = FormStartPosition.CenterScreen;
					TextBox textBox = new TextBox();
					textBox.Dock = DockStyle.Fill;
					textBox.Multiline = true;
					textBox.ReadOnly = true;
					textBox.Text = ((Session)this.m_pSessions.SelectedItems[0].Tag).SessionLog;
					textBox.SelectionStart = 0;
					textBox.SelectionLength = 0;
					form.Controls.Add(textBox);
					form.Visible = true;
					return;
				}
			}
			else if (taskID == "kill" && this.m_pSessions.SelectedItems.Count > 0)
			{
				((Session)this.m_pSessions.SelectedItems[0].Tag).Kill();
				this.m_pSessions.SelectedItems[0].Remove();
			}
		}

		private void StartMonitoring()
		{
			this.m_Run = true;
			Thread thread = new Thread(new ThreadStart(this.Run));
            thread.Name = "Mgmt Session Refresh Thread";
			thread.Start();
		}

		private void Run()
		{
			while (this.m_Run)
			{
				try
				{
					this.m_pServer.Sessions.Refresh();
					base.Invoke(new ThreadStart(this.RefreshSessions));
				}
				catch (Exception)
				{
					if (this.m_pSessions.Items.Count > 0)
					{
						this.m_pSessions.Invoke(new Action(delegate
						{
							this.m_pSessions.Clear();
						}));
					}
				}
				Thread.Sleep(3000);
			}
		}

		private void RefreshSessions()
		{
			this.m_pSessions.BeginUpdate();
			try
			{
				for (int i = 0; i < this.m_pSessions.Items.Count; i++)
				{
					ListViewItem listViewItem = this.m_pSessions.Items[i];
					if (this.m_pServer.Sessions.ConatainsID(((Session)listViewItem.Tag).ID))
					{
						Session sessionByID = this.m_pServer.Sessions.GetSessionByID(((Session)listViewItem.Tag).ID);
						listViewItem.Tag = sessionByID;
						listViewItem.SubItems[1].Text = sessionByID.UserName;
						listViewItem.SubItems[2].Text = sessionByID.LocalEndPoint;
						listViewItem.SubItems[3].Text = sessionByID.RemoteEndPoint;
						listViewItem.SubItems[4].Text = (sessionByID.ReadKbInSecond / 1000m).ToString("f2");
						listViewItem.SubItems[5].Text = (sessionByID.WriteKbInSecond / 1000m).ToString("f2");
						listViewItem.SubItems[6].Text = sessionByID.SartTime.ToString("yy.MM.dd HH:mm");
						listViewItem.SubItems[7].Text = sessionByID.IdleTimeOutSeconds.ToString();
					}
					else
					{
						listViewItem.Remove();
						i++;
					}
				}
				foreach (Session session in this.m_pServer.Sessions)
				{
					if (this.m_pShow.SelectedIndex == 0 || ((this.m_pShow.SelectedIndex != 1 || !(session.Type != "SMTP")) && (this.m_pShow.SelectedIndex != 2 || !(session.Type != "POP3")) && (this.m_pShow.SelectedIndex != 3 || !(session.Type != "IMAP")) && (this.m_pShow.SelectedIndex != 4 || !(session.Type != "RELAY"))))
					{
						bool flag = false;
						foreach (ListViewItem listViewItem2 in this.m_pSessions.Items)
						{
							if (session.ID == ((Session)listViewItem2.Tag).ID)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							ListViewItem listViewItem3 = new ListViewItem();
							listViewItem3.ImageIndex = 0;
							listViewItem3.Text = session.Type;
							listViewItem3.SubItems.Add(session.UserName);
							listViewItem3.SubItems.Add(session.LocalEndPoint);
							listViewItem3.SubItems.Add(session.RemoteEndPoint);
							listViewItem3.SubItems.Add((session.ReadKbInSecond / 1000m).ToString("f2"));
							listViewItem3.SubItems.Add((session.WriteKbInSecond / 1000m).ToString("f2"));
							listViewItem3.SubItems.Add(session.SartTime.ToString("yy.MM.dd HH:mm"));
							listViewItem3.SubItems.Add(session.IdleTimeOutSeconds.ToString());
							listViewItem3.Tag = session;
							this.m_pSessions.Items.Add(listViewItem3);
						}
					}
				}
			}
			catch
			{
			}
			this.m_pSessions.EndUpdate();
		}
	}
}
