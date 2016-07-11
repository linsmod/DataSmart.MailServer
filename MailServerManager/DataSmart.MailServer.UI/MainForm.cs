using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.MonitoringForms;
using DataSmart.MailServer.UI.QueryForms;
using DataSmart.MailServer.UI.Resources;
using DataSmart.MailServer.UI.RoutingForms;
using DataSmart.MailServer.UI.SecurityForms;
using DataSmart.MailServer.UI.SharedFolderForms;
using DataSmart.MailServer.UI.SystemForms;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DataSmart.MailServer.Extensions;
namespace DataSmart.MailServer.UI
{
    public class MainForm : Form
    {
        private enum NodeType
        {
            Dummy,
            Machine,
            LocalMachineMonitoring,
            Monitoring_SipRegistrations,
            Monitoring_SipCalls,
            VirtualServers,
            Server,
            System,
            System_General,
            System_Authentication,
            System_Services,
            System_Services_SMTP,
            System_Services_POP3,
            System_Services_IMAP,
            System_Services_Relay,
            System_Services_FetchPOP3,
            System_Services_SIP,
            System_Logging,
            System_Backup,
            System_ReturnMessages,
            Domains,
            UsersAndGroups,
            UsersAndGroups_Users,
            UsersAndGroups_Groups,
            MailingLists,
            Rules,
            Rules_Message_Global,
            Routing,
            Security,
            Filters,
            SharedFolders_Users,
            SharedFolders_RootFolders,
            Folders_UsersDefaultFolders,
            Folders_RecycleBin,
            Queues_IncomingSMTP,
            Queues_OutgoingSMTP,
            EventsAndLogs_Events,
            EventsAndLogs_Logs
        }

        private class TreeNodeInfo
        {
            private TreeNode m_pNode;

            private Server m_pServer;

            private MainForm.NodeType m_NodeType;

            private VirtualServer m_VirturalServer;

            public TreeNode OwnerNode
            {
                get
                {
                    return this.m_pNode;
                }
            }

            public Server Server
            {
                get
                {
                    return this.m_pServer;
                }
            }

            public MainForm.NodeType TreeNodeType
            {
                get
                {
                    return this.m_NodeType;
                }
            }

            public VirtualServer VirturalServer
            {
                get
                {
                    return this.m_VirturalServer;
                }
                set
                {
                    this.m_VirturalServer = value;
                }
            }

            public TreeNodeInfo(Server server, MainForm.NodeType type, VirtualServer virtualServer = null)
            {
                this.m_pServer = server;
                this.m_NodeType = type;
                this.m_VirturalServer = virtualServer;
            }

            public TreeNodeInfo(TreeNode node, Server server, MainForm.NodeType type, VirtualServer virtualServer = null)
            {
                this.m_pNode = node;
                this.m_pServer = server;
                this.m_NodeType = type;
                this.m_VirturalServer = virtualServer;
            }
        }

        private static string mgrServersXml = Application.StartupPath + "/Settings/managerServers.xml";

        private MenuStrip m_pMenu;

        private WFrame m_pFrame;

        private ToolStripMenuItem file_Connect;

        private ToolStripMenuItem file_Exit;

        private ToolStripMenuItem help_Forum;

        private ToolStripMenuItem help_About;

        private ImageList treeImageList;

        private IContainer components;

        private TreeView m_pTree;

        private ContextMenuStrip _serverCtxMenu;

        public MainForm()
        {
            this.InitializeComponent();
            this.treeImageList.Images.Add("server", ResManager.GetIcon("icon-server.ico"));
            this.treeImageList.Images.Add("list", ResManager.GetImage("icon-list.png"));
            this.treeImageList.Images.Add("sip", ResManager.GetImage("icon-sip.png"));
            this.treeImageList.Images.Add("call", ResManager.GetImage("icon-call.png"));
            this.treeImageList.Images.Add("monitor", ResManager.GetImage("icon-monitor.png"));
            this.treeImageList.Images.Add("session", ResManager.GetImage("icon-session.png"));
            this.treeImageList.Images.Add("servers", ResManager.GetImage("icon-servers.png"));
            this.treeImageList.Images.Add("services", ResManager.GetIcon("services.ico"));
            this.treeImageList.Images.Add("domain", ResManager.GetImage("icon-domain.png"));
            this.treeImageList.Images.Add("user", ResManager.GetImage("icon-user.png"));
            this.treeImageList.Images.Add("group", ResManager.GetImage("icon-group.png"));
            this.treeImageList.Images.Add("mail", ResManager.GetImage("icon-mail.png"));
            this.treeImageList.Images.Add("system", ResManager.GetImage("icon-system.png"));
            this.treeImageList.Images.Add("acl", ResManager.GetIcon("acl.ico"));
            this.treeImageList.Images.Add("filter", ResManager.GetImage("icon-filter.png"));
            this.treeImageList.Images.Add("messagerule", ResManager.GetIcon("messagerule.ico"));
            this.treeImageList.Images.Add("folder_share", ResManager.GetIcon("icon-folder-share.ico"));
            this.treeImageList.Images.Add("folders", ResManager.GetIcon("icon-folders.ico"));
            this.treeImageList.Images.Add("folder", ResManager.GetIcon("icon-folder.ico"));
            this.treeImageList.Images.Add("queue", ResManager.GetIcon("queue.ico"));
            this.treeImageList.Images.Add("logging", ResManager.GetImage("icon-logging.png"));
            this.treeImageList.Images.Add("system", ResManager.GetImage("icon-system.png"));
            this.treeImageList.Images.Add("route", ResManager.GetImage("icon-route.png"));
            this.treeImageList.Images.Add("authentication", ResManager.GetIcon("authentication.ico"));
            this.treeImageList.Images.Add("backup", ResManager.GetIcon("backup.ico"));
            this.treeImageList.Images.Add("server_running", ResManager.GetImage("icon-server-running.png"));
            this.treeImageList.Images.Add("server_stopped", ResManager.GetImage("icon-server-stopped.png"));
            this.treeImageList.Images.Add("recyclebin", ResManager.GetImage("icon-recycle.png"));
            this.treeImageList.Images.Add("message", ResManager.GetIcon("message16.ico"));
            this.treeImageList.Images.Add("security", ResManager.GetImage("icon-security.png"));
            this.LoadServers();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            try
            {
                this.m_pFrame.Frame_Form = null;
                this.m_pFrame.Dispose();
            }
            catch
            {
            }
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MainForm));
            this.m_pMenu = new MenuStrip();
            this.file_Connect = new ToolStripMenuItem();
            this.file_Exit = new ToolStripMenuItem();
            this.help_About = new ToolStripMenuItem();
            this.help_Forum = new ToolStripMenuItem();
            this.m_pTree = new TreeView();
            this.m_pTree.NodeMouseClick += M_pTree_NodeMouseClick;
            this.treeImageList = new ImageList(this.components);
            this.m_pFrame = new WFrame();
            this.m_pMenu.SuspendLayout();
            base.SuspendLayout();
            this.m_pMenu.BackColor = this.BackColor;
            this.m_pMenu.Items.AddRange(new ToolStripItem[]
            {
                this.file_Connect,
                this.file_Exit,
                this.help_About,
                this.help_Forum
            });
            this.m_pMenu.Location = new Point(0, 0);
            this.m_pMenu.Name = "m_pMenu";
            this.m_pMenu.Size = new Size(764, 25);
            this.m_pMenu.TabIndex = 1;
            this.file_Connect.Image = (Image)componentResourceManager.GetObject("file_Connect.Image");
            this.file_Connect.Name = "file_Connect";
            this.file_Connect.Size = new Size(83, 21);
            this.file_Connect.Tag = "file_connect";
            this.file_Connect.Text = "Connect";
            this.file_Connect.Click += new EventHandler(this.file_Connect_Click);
            this.file_Exit.Image = (Image)componentResourceManager.GetObject("file_Exit.Image");
            this.file_Exit.Name = "file_Exit";
            this.file_Exit.Size = new Size(56, 21);
            this.file_Exit.Tag = "file_exit";
            this.file_Exit.Text = "Exit";
            this.file_Exit.Click += new EventHandler(this.file_Exit_Click);
            this.help_About.Image = (Image)componentResourceManager.GetObject("help_About.Image");
            this.help_About.Name = "help_About";
            this.help_About.Size = new Size(71, 21);
            this.help_About.Tag = "help_about";
            this.help_About.Text = "About";
            this.help_About.Click += new EventHandler(this.help_About_Click);
            this.m_pTree.BorderStyle = BorderStyle.None;
            this.m_pTree.Dock = DockStyle.Fill;
            this.m_pTree.HideSelection = false;
            this.m_pTree.HotTracking = true;
            this.m_pTree.ImageIndex = 0;
            this.m_pTree.ImageList = this.treeImageList;
            this.m_pTree.ItemHeight = 23;
            this.m_pTree.Location = new Point(0, 0);
            this.m_pTree.Name = "m_pTree";
            this.m_pTree.SelectedImageIndex = 0;
            this.m_pTree.Size = new Size(228, 554);
            this.m_pTree.TabIndex = 0;
            this.m_pTree.AfterSelect += new TreeViewEventHandler(this.m_pTree_AfterSelect);
            this.m_pTree.DoubleClick += new EventHandler(this.m_pTree_DoubleClick);
            this.treeImageList.ColorDepth = ColorDepth.Depth8Bit;
            this.treeImageList.ImageSize = new Size(16, 16);
            this.treeImageList.TransparentColor = Color.Transparent;
            this.m_pFrame.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.m_pFrame.ControlPanelWidth = 200;
            this.m_pFrame.FormFrameBorder = BorderStyle.FixedSingle;
            this.m_pFrame.Location = new Point(0, 27);
            this.m_pFrame.Name = "m_pFrame";
            this.m_pFrame.Size = new Size(764, 582);
            this.m_pFrame.SplitterColor = SystemColors.Control;
            this.m_pFrame.SplitterMinExtra = 0;
            this.m_pFrame.SplitterMinSize = 0;
            this.m_pFrame.TabIndex = 0;
            this.m_pFrame.TopPaneBkColor = SystemColors.Control;
            this.m_pFrame.TopPaneHeight = 25;
            base.ClientSize = new Size(764, 611);
            base.Controls.Add(this.m_pFrame);
            base.Controls.Add(this.m_pMenu);
            base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            this.MinimumSize = new Size(780, 650);
            base.Name = "wfrm_Main";
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "DataSmart Mail Server Manager";
            this.m_pMenu.ResumeLayout(false);
            this.m_pMenu.PerformLayout();
            base.ResumeLayout(false);
            base.PerformLayout();
            this.m_pFrame.ControlPanel.Controls.Add(this.m_pTree);
        }

        private void M_pTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            (sender as TreeView).SelectedNode = e.Node;
        }

        private void file_Connect_Click(object sender, EventArgs e)
        {
            ConnectForm connectForm = new ConnectForm();
            if (connectForm.ShowDialog(this) == DialogResult.OK)
            {
                string text = "";
                if (connectForm.SaveConnection)
                {
                    var host = connectForm.Host;
                    if (string.IsNullOrEmpty(host))
                    {
                        host = "127.0.0.1";
                    }
                    text = Guid.NewGuid().ToString();
                    if (!Directory.Exists(Application.StartupPath + "/Settings"))
                    {
                        Directory.CreateDirectory(Application.StartupPath + "/Settings");
                    }
                    DataSet dataSet = this.LoadRegisteredServers();
                    DataRow dataRow = dataSet.Tables["Servers"].NewRow();
                    dataRow["ID"] = text;
                    dataRow["Host"] = host;
                    dataRow["UserName"] = connectForm.UserName;
                    dataRow["Password"] = connectForm.Password;
                    dataSet.Tables["Servers"].Rows.Add(dataRow);
                    dataSet.WriteXml(MainForm.mgrServersXml);
                }
                this.LoadServer(connectForm.Server, text);
            }
        }

        private void file_Exit_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void help_Forum_Click(object sender, EventArgs e)
        {
            if (Environment.OSVersion.Platform != PlatformID.Unix)
            {
                Process.Start("explorer", "http://projects.dsvisual.cn/mailserver");
                return;
            }
            try
            {
                Process.Start("firefox", "http://projects.dsvisual.cn/mailserver");
            }
            catch
            {
            }
        }

        private void help_About_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog(this);
        }

        private void m_pTree_DoubleClick(object sender, EventArgs e)
        {
            if (this.m_pTree.SelectedNode == null)
            {
                return;
            }
            if (this.m_pTree.SelectedNode.Parent == null && this.m_pTree.SelectedNode.Nodes.Count == 0)
            {
                MainForm.TreeNodeInfo treeNodeInfo = (MainForm.TreeNodeInfo)this.m_pTree.SelectedNode.Tag;
                treeNodeInfo.Server.Connect();
                this.LoadServer(this.m_pTree.SelectedNode, treeNodeInfo.Server, treeNodeInfo.Server.ID);
            }
        }

        private void m_pTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.m_pFrame.FormFrameBorder = BorderStyle.FixedSingle;
            this.m_pFrame.Frame_ToolStrip = null;
            if (e.Node == null || e.Node.Tag == null)
            {
                this.m_pFrame.Frame_Form = new Form();
                return;
            }
            if (e.Node.Parent == null && e.Node.Nodes.Count == 0)
            {
                return;
            }
            MainForm.TreeNodeInfo treeNodeInfo = (MainForm.TreeNodeInfo)e.Node.Tag;
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.Dummy)
            {
                this.m_pFrame.Frame_Form = new Form();
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.Server)
            {
                this.m_pFrame.Frame_Form = new ServerInfomationForm(treeNodeInfo.Server);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.LocalMachineMonitoring)
            {
                this.m_pFrame.Frame_Form = new SessionsForm(treeNodeInfo.Server);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.Monitoring_SipRegistrations)
            {
                this.m_pFrame.Frame_Form = new SipRegistrationsForm(treeNodeInfo.Server, this.m_pFrame);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.Monitoring_SipCalls)
            {
                this.m_pFrame.Frame_Form = new SipCallsForm(treeNodeInfo.Server, this.m_pFrame);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.EventsAndLogs_Events)
            {
                this.m_pFrame.Frame_Form = new EventsForm(treeNodeInfo.Server, this.m_pFrame);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.EventsAndLogs_Logs)
            {
                this.m_pFrame.Frame_Form = new LogsForm(treeNodeInfo.Server, this.m_pFrame);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.VirtualServers)
            {
                this.m_pFrame.Frame_Form = new VirtualServersForm(this, treeNodeInfo.OwnerNode, treeNodeInfo.Server, this.m_pFrame);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.System_General)
            {
                this.m_pFrame.FormFrameBorder = BorderStyle.None;
                this.m_pFrame.Frame_Form = new GeneralForm(treeNodeInfo.VirturalServer);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.System_Authentication)
            {
                this.m_pFrame.FormFrameBorder = BorderStyle.None;
                this.m_pFrame.Frame_Form = new AuthenticationForm(treeNodeInfo.VirturalServer);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.System_Services_SMTP)
            {
                this.m_pFrame.FormFrameBorder = BorderStyle.None;
                this.m_pFrame.Frame_Form = new SmtpServicesForm(treeNodeInfo.VirturalServer);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.System_Services_POP3)
            {
                this.m_pFrame.FormFrameBorder = BorderStyle.None;
                this.m_pFrame.Frame_Form = new Pop3ServicesForm(treeNodeInfo.VirturalServer);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.System_Services_IMAP)
            {
                this.m_pFrame.FormFrameBorder = BorderStyle.None;
                this.m_pFrame.Frame_Form = new ImapServiceForm(treeNodeInfo.VirturalServer);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.System_Services_Relay)
            {
                this.m_pFrame.FormFrameBorder = BorderStyle.None;
                this.m_pFrame.Frame_Form = new RelayServicesForm(treeNodeInfo.VirturalServer);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.System_Services_FetchPOP3)
            {
                this.m_pFrame.FormFrameBorder = BorderStyle.None;
                this.m_pFrame.Frame_Form = new FetchPop3ServiceForm(treeNodeInfo.VirturalServer);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.System_Services_SIP)
            {
                this.m_pFrame.FormFrameBorder = BorderStyle.None;
                this.m_pFrame.Frame_Form = new SipServiceForm(treeNodeInfo.VirturalServer);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.System_Logging)
            {
                this.m_pFrame.FormFrameBorder = BorderStyle.None;
                this.m_pFrame.Frame_Form = new LoggingForm(treeNodeInfo.VirturalServer);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.System_Backup)
            {
                this.m_pFrame.Frame_Form = new BackupForm(treeNodeInfo.VirturalServer);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.System_ReturnMessages)
            {
                this.m_pFrame.Frame_Form = new ReturnMessagesForm(treeNodeInfo.VirturalServer);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.Domains)
            {
                this.m_pFrame.Frame_Form = new DomainsForm(treeNodeInfo.VirturalServer, this.m_pFrame);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.UsersAndGroups_Users)
            {
                this.m_pFrame.Frame_Form = new UsersForm(treeNodeInfo.VirturalServer, this.m_pFrame);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.UsersAndGroups_Groups)
            {
                this.m_pFrame.Frame_Form = new GroupsForm(treeNodeInfo.VirturalServer, this.m_pFrame);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.MailingLists)
            {
                this.m_pFrame.Frame_Form = new MailingListsForm(treeNodeInfo.VirturalServer, this.m_pFrame);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.Routing)
            {
                this.m_pFrame.Frame_Form = new RoutesForm(treeNodeInfo.VirturalServer, this.m_pFrame);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.Rules_Message_Global)
            {
                this.m_pFrame.Frame_Form = new GlobalMessageRulesForm(treeNodeInfo.VirturalServer, this.m_pFrame);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.SharedFolders_RootFolders)
            {
                this.m_pFrame.Frame_Form = new RootFoldersForm(treeNodeInfo.VirturalServer, this.m_pFrame);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.Folders_UsersDefaultFolders)
            {
                this.m_pFrame.Frame_Form = new UsersDefaultFoldersForm(treeNodeInfo.VirturalServer, this.m_pFrame);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.Folders_RecycleBin)
            {
                this.m_pFrame.Frame_Form = new RecycleBinForm(treeNodeInfo.VirturalServer, this.m_pFrame);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.Filters)
            {
                this.m_pFrame.Frame_Form = new FiltersForm(treeNodeInfo.VirturalServer, this.m_pFrame);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.Queues_IncomingSMTP)
            {
                this.m_pFrame.Frame_Form = new IncomingSMTPForm(treeNodeInfo.VirturalServer);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.Queues_OutgoingSMTP)
            {
                this.m_pFrame.Frame_Form = new OutgoingSmtpForm(treeNodeInfo.VirturalServer);
                return;
            }
            if (treeNodeInfo.TreeNodeType == MainForm.NodeType.Security)
            {
                this.m_pFrame.Frame_Form = new IPSecurityForm(treeNodeInfo.VirturalServer, this.m_pFrame);
            }
        }

        private ContextMenuStrip GetServerContextMenu()
        {
            if (this._serverCtxMenu == null)
            {
                ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("Reconnect");
                toolStripMenuItem.Click += new EventHandler(this.ReloadServer_Click);
                contextMenuStrip.Items.Add(toolStripMenuItem);
                ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem("Delete");
                toolStripMenuItem2.Click += new EventHandler(this.deleteServer_Click);
                contextMenuStrip.Items.Add(toolStripMenuItem2);
                this._serverCtxMenu = contextMenuStrip;
            }
            return this._serverCtxMenu;
        }

        private void ReloadServer_Click(object sender, EventArgs e)
        {
            TreeNode treeNode = this.FindAssociatedServer(this.m_pTree.SelectedNode);
            MainForm.TreeNodeInfo treeNodeInfo = (MainForm.TreeNodeInfo)treeNode.Tag;
            treeNodeInfo.Server.Disconnect();
            this.m_pTree.SelectedNode.Nodes.Clear();
            this.m_pTree_DoubleClick(null, null);
        }

        private void deleteServer_Click(object sender, EventArgs e)
        {
            TreeNode treeNode = this.FindAssociatedServer(this.m_pTree.SelectedNode);
            MainForm.TreeNodeInfo treeNodeInfo = (MainForm.TreeNodeInfo)treeNode.Tag;
            if (MessageBox.Show(this, "Are you sure you want to remove server '" + treeNode.Text + "' from list ?", "Confirm delete:", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DataSet dataSet = this.LoadRegisteredServers();
                this.RemoveRowByID(treeNodeInfo.Server.ID);
                treeNodeInfo.Server.Disconnect();
                treeNode.Remove();
            }
        }

        private DataRow GetServerById(string id)
        {
            DataSet dataSet = this.LoadRegisteredServers();
            foreach (DataRow dataRow in dataSet.Tables["Servers"].Rows)
            {
                if (dataRow["ID"].ToString() == id)
                {
                    return dataRow;
                }
            }
            return null;
        }

        private bool RemoveRowByID(string id)
        {
            DataRow row = null;
            DataSet dataSet = this.LoadRegisteredServers();
            foreach (DataRow dataRow in dataSet.Tables["Servers"].Rows)
            {
                if (dataRow["ID"].ToString() == id)
                {
                    row = dataRow;
                    dataSet.Tables["Servers"].Rows.Remove(row);
                    dataSet.WriteXml(mgrServersXml);
                    return true;
                }
            }
            return false;
        }

        private DataSet LoadRegisteredServers()
        {
            DataSet dataSet = new DataSet("dsRegisteredServers");
            dataSet.Tables.Add("Servers");
            dataSet.Tables["Servers"].Columns.Add("ID");
            dataSet.Tables["Servers"].Columns.Add("Host");
            dataSet.Tables["Servers"].Columns.Add("UserName");
            dataSet.Tables["Servers"].Columns.Add("Password");
            if (File.Exists(MainForm.mgrServersXml))
            {
                dataSet.ReadXml(MainForm.mgrServersXml);
            }
            return dataSet;
        }

        private void LoadServers()
        {
            foreach (DataRow dataRow in this.LoadRegisteredServers().Tables["Servers"].Rows)
            {
                TreeNode treeNode = new TreeNode(dataRow["Host"].ToString());
                treeNode.ImageKey = (treeNode.SelectedImageKey = "machine");
                treeNode.Tag = new MainForm.TreeNodeInfo(new Server(dataRow["Host"].ToString(), dataRow["UserName"].ToString(), dataRow["Password"].ToString().ConvertToSecureString())
                {
                    ID = dataRow["Id"].ToString(),
                }, MainForm.NodeType.Server, null);
                if (treeNode.ContextMenuStrip == null)
                {
                    treeNode.ContextMenuStrip = this.GetServerContextMenu();
                }
                this.m_pTree.Nodes.Add(treeNode);
            }
        }

        private void LoadServer(Server server, string serverID)
        {
            this.LoadServer(null, server, serverID);
        }

        private TreeNode FindAssociatedServer(TreeNode node)
        {
            MainForm.TreeNodeInfo treeNodeInfo = node.Tag as MainForm.TreeNodeInfo;
            if (treeNodeInfo != null && treeNodeInfo.TreeNodeType == MainForm.NodeType.Server)
            {
                return node;
            }
            if (node.Parent != null)
            {
                return this.FindAssociatedServer(node.Parent);
            }
            return null;
        }

        private void LoadServer(TreeNode serverNode, Server server, string serverID)
        {
            TreeNode treeNode = serverNode;
            if (serverNode == null)
            {
                treeNode = new TreeNode(server.Host);
                treeNode.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.Server, null);
                this.m_pTree.Nodes.Add(treeNode);
            }
            else
            {
                treeNode.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.Server, null);
            }
            if (treeNode.ContextMenuStrip == null)
            {
                treeNode.ContextMenuStrip = this.GetServerContextMenu();
            }
            TreeNode treeNode2 = new TreeNode("Monitoring");
            treeNode2.ImageKey = (treeNode2.SelectedImageKey = "monitor");
            treeNode.Nodes.Add(treeNode2);
            TreeNode treeNode3 = new TreeNode("Sessions");
            treeNode3.ImageKey = (treeNode3.SelectedImageKey = "session");
            treeNode3.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.LocalMachineMonitoring, null);
            treeNode2.Nodes.Add(treeNode3);
            TreeNode treeNode4 = new TreeNode("SIP")
            {
                ImageKey = "sip",
                SelectedImageKey = "sip"
            };
            treeNode2.Nodes.Add(treeNode4);
            TreeNode treeNode5 = new TreeNode("Registrations")
            {
                ImageKey = "list",
                SelectedImageKey = "list"
            };
            treeNode5.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.Monitoring_SipRegistrations, null);
            treeNode4.Nodes.Add(treeNode5);
            TreeNode treeNode6 = new TreeNode("Calls")
            {
                ImageKey = "call",
                SelectedImageKey = "call"
            };
            treeNode6.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.Monitoring_SipCalls, null);
            treeNode4.Nodes.Add(treeNode6);
            TreeNode treeNode7 = new TreeNode("Logs and Events")
            {
                ImageKey = "logging",
                SelectedImageKey = "logging"
            };
            treeNode.Nodes.Add(treeNode7);
            TreeNode treeNode8 = new TreeNode("Events")
            {
                ImageKey = "system",
                SelectedImageKey = "system"
            };
            treeNode8.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.EventsAndLogs_Events, null);
            treeNode7.Nodes.Add(treeNode8);
            TreeNode treeNode9 = new TreeNode("Logs")
            {
                ImageKey = "logging",
                SelectedImageKey = "logging"
            };
            treeNode9.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.EventsAndLogs_Logs, null);
            treeNode7.Nodes.Add(treeNode9);
            TreeNode treeNode10 = new TreeNode("Virtual Servers")
            {
                ImageKey = "servers",
                SelectedImageKey = "servers"
            };
            treeNode10.Tag = new MainForm.TreeNodeInfo(treeNode10, server, MainForm.NodeType.VirtualServers, null);
            treeNode.Nodes.Add(treeNode10);
            this.LoadVirtualServers(treeNode10, server);
        }

        internal void LoadVirtualServers(TreeNode virtualServersNode, Server server)
        {
            virtualServersNode.Nodes.Clear();
            foreach (VirtualServer virtualServer in server.VirtualServers)
            {
                TreeNode treeNode = new TreeNode(virtualServer.Name);
                if (virtualServer.Enabled)
                {
                    treeNode.ImageKey = "server_running";
                    treeNode.SelectedImageKey = "server_running";
                }
                else
                {
                    treeNode.ImageKey = "server_stopped";
                    treeNode.SelectedImageKey = "server_stopped";
                }
                virtualServersNode.Nodes.Add(treeNode);
                TreeNode treeNode2 = new TreeNode("System")
                {
                    ImageKey = "system",
                    SelectedImageKey = "system"
                };
                treeNode.Nodes.Add(treeNode2);
                TreeNode treeNode3 = new TreeNode("General")
                {
                    ImageKey = "system",
                    SelectedImageKey = "system"
                };
                treeNode3.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.System_General, virtualServer);
                treeNode2.Nodes.Add(treeNode3);
                TreeNode treeNode4 = new TreeNode("Authentication")
                {
                    ImageKey = "authentication",
                    SelectedImageKey = "authentication"
                };
                treeNode4.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.System_Authentication, virtualServer);
                treeNode2.Nodes.Add(treeNode4);
                TreeNode treeNode5 = new TreeNode("Services")
                {
                    ImageKey = "services",
                    SelectedImageKey = "services"
                };
                treeNode2.Nodes.Add(treeNode5);
                TreeNode treeNode6 = new TreeNode("SMTP")
                {
                    ImageKey = "system",
                    SelectedImageKey = "system"
                };
                treeNode6.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.System_Services_SMTP, virtualServer);
                treeNode5.Nodes.Add(treeNode6);
                TreeNode treeNode7 = new TreeNode("POP3")
                {
                    ImageKey = "system",
                    SelectedImageKey = "system"
                };
                treeNode7.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.System_Services_POP3, virtualServer);
                treeNode5.Nodes.Add(treeNode7);
                TreeNode treeNode8 = new TreeNode("IMAP")
                {
                    ImageKey = "system",
                    SelectedImageKey = "system"
                };
                treeNode8.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.System_Services_IMAP, virtualServer);
                treeNode5.Nodes.Add(treeNode8);
                TreeNode treeNode9 = new TreeNode("Relay")
                {
                    ImageKey = "system",
                    SelectedImageKey = "system"
                };
                treeNode9.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.System_Services_Relay, virtualServer);
                treeNode5.Nodes.Add(treeNode9);
                TreeNode treeNode10 = new TreeNode("Fetch POP3")
                {
                    ImageKey = "system",
                    SelectedImageKey = "system"
                };
                treeNode10.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.System_Services_FetchPOP3, virtualServer);
                treeNode5.Nodes.Add(treeNode10);
                TreeNode treeNode11 = new TreeNode("SIP")
                {
                    ImageKey = "system",
                    SelectedImageKey = "system"
                };
                treeNode11.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.System_Services_SIP, virtualServer);
                treeNode5.Nodes.Add(treeNode11);
                TreeNode treeNode12 = new TreeNode("Logging", 13, 13)
                {
                    ImageKey = "logging",
                    SelectedImageKey = "logging"
                };
                treeNode12.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.System_Logging, virtualServer);
                treeNode2.Nodes.Add(treeNode12);
                TreeNode treeNode13 = new TreeNode("Backup", 17, 17)
                {
                    ImageKey = "backup",
                    SelectedImageKey = "backup"
                };
                treeNode13.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.System_Backup, virtualServer);
                treeNode2.Nodes.Add(treeNode13);
                TreeNode treeNode14 = new TreeNode("Return Messages", 21, 21)
                {
                    ImageKey = "system",
                    SelectedImageKey = "system"
                };
                treeNode14.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.System_ReturnMessages, virtualServer);
                treeNode2.Nodes.Add(treeNode14);
                TreeNode treeNode15 = new TreeNode("Domains", 3, 3)
                {
                    ImageKey = "domain",
                    SelectedImageKey = "domain"
                };
                treeNode15.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.Domains, virtualServer);
                treeNode.Nodes.Add(treeNode15);
                TreeNode treeNode16 = new TreeNode("Users")
                {
                    ImageKey = "user",
                    SelectedImageKey = "user"
                };
                treeNode16.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.UsersAndGroups_Users, virtualServer);
                treeNode.Nodes.Add(treeNode16);
                TreeNode treeNode17 = new TreeNode("Groups")
                {
                    ImageKey = "group",
                    SelectedImageKey = "group"
                };
                treeNode17.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.UsersAndGroups_Groups, virtualServer);
                treeNode.Nodes.Add(treeNode17);
                TreeNode treeNode18 = new TreeNode("Message Rules")
                {
                    ImageKey = "messagerule",
                    SelectedImageKey = "messagerule"
                };
                treeNode.Nodes.Add(treeNode18);
                TreeNode treeNode19 = new TreeNode("Global")
                {
                    ImageKey = "messagerule",
                    SelectedImageKey = "messagerule"
                };
                treeNode19.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.Rules_Message_Global, virtualServer);
                treeNode18.Nodes.Add(treeNode19);
                TreeNode treeNode20 = new TreeNode("Mailing Lists", 5, 5)
                {
                    ImageKey = "mail",
                    SelectedImageKey = "mail"
                };
                treeNode20.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.MailingLists, virtualServer);
                treeNode.Nodes.Add(treeNode20);
                TreeNode treeNode21 = new TreeNode("Routing", 6, 6)
                {
                    ImageKey = "route",
                    SelectedImageKey = "route"
                };
                treeNode21.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.Routing, virtualServer);
                treeNode.Nodes.Add(treeNode21);
                TreeNode treeNode22 = new TreeNode("Folders Management")
                {
                    ImageKey = "folders",
                    SelectedImageKey = "folders"
                };
                treeNode.Nodes.Add(treeNode22);
                TreeNode treeNode23 = new TreeNode("Shared Root Folders")
                {
                    ImageKey = "folder_share",
                    SelectedImageKey = "folder_share"
                };
                treeNode23.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.SharedFolders_RootFolders, virtualServer);
                treeNode22.Nodes.Add(treeNode23);
                TreeNode treeNode24 = new TreeNode("Users Default Folders")
                {
                    ImageKey = "folder",
                    SelectedImageKey = "folder"
                };
                treeNode24.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.Folders_UsersDefaultFolders, virtualServer);
                treeNode22.Nodes.Add(treeNode24);
                TreeNode treeNode25 = new TreeNode("Recycle Bin")
                {
                    ImageKey = "recyclebin",
                    SelectedImageKey = "recyclebin"
                };
                treeNode25.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.Folders_RecycleBin, virtualServer);
                treeNode22.Nodes.Add(treeNode25);
                TreeNode treeNode26 = new TreeNode("Queues")
                {
                    ImageKey = "queue",
                    SelectedImageKey = "queue"
                };
                treeNode.Nodes.Add(treeNode26);
                TreeNode treeNode27 = new TreeNode("Incoming SMTP")
                {
                    ImageKey = "folders",
                    SelectedImageKey = "folders"
                };
                treeNode27.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.Queues_IncomingSMTP, virtualServer);
                treeNode26.Nodes.Add(treeNode27);
                TreeNode treeNode28 = new TreeNode("Outgoing SMTP")
                {
                    ImageKey = "folders",
                    SelectedImageKey = "folders"
                };
                treeNode28.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.Queues_OutgoingSMTP, virtualServer);
                treeNode26.Nodes.Add(treeNode28);
                TreeNode treeNode29 = new TreeNode("Security")
                {
                    ImageKey = "security",
                    SelectedImageKey = "security"
                };
                treeNode29.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.Security, virtualServer);
                treeNode.Nodes.Add(treeNode29);
                TreeNode treeNode30 = new TreeNode("Filters")
                {
                    ImageKey = "filter",
                    SelectedImageKey = "filter"
                };
                treeNode30.Tag = new MainForm.TreeNodeInfo(server, MainForm.NodeType.Filters, virtualServer);
                treeNode.Nodes.Add(treeNode30);
            }
        }
    }
}
