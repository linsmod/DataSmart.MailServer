using DataSmart.MailServer.Resources;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
    public class TrayAppForm : Form
    {
        private ContextMenuStrip m_pMenu;

        private NotifyIcon m_pNotyfyIcon;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem toolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem toolStripMenuItem4;
        private Server m_pMailServer;

        public TrayAppForm()
        {
            this.InitializeComponent();
            this.m_pMailServer = new Server();
            this.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (this.m_pMailServer != null)
            {
                this.m_pMailServer.Dispose();
                this.m_pMailServer = null;
            }
            if (this.m_pNotyfyIcon != null)
            {
                this.m_pNotyfyIcon.Dispose();
                this.m_pNotyfyIcon = null;
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrayAppForm));
            this.m_pMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.m_pNotyfyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.m_pMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_pMenu
            // 
            this.m_pMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4});
            this.m_pMenu.Name = "m_pMenu";
            this.m_pMenu.Size = new System.Drawing.Size(166, 82);
            this.m_pMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.m_pMenu_ItemClicked);
            // 
            // toolStripMenuItem
            // 
            this.toolStripMenuItem.Name = "toolStripMenuItem";
            this.toolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.toolStripMenuItem.Tag = "manager";
            this.toolStripMenuItem.Text = "Open New Manager";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(165, 22);
            this.toolStripMenuItem2.Tag = "start";
            this.toolStripMenuItem2.Text = "Start";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(165, 22);
            this.toolStripMenuItem3.Tag = "stop";
            this.toolStripMenuItem3.Text = "Stop";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem4.Image")));
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(32, 19);
            this.toolStripMenuItem4.Tag = "exit";
            this.toolStripMenuItem4.Text = "Exit";
            // 
            // m_pNotyfyIcon
            // 
            this.m_pNotyfyIcon.ContextMenuStrip = this.m_pMenu;
            this.m_pNotyfyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("m_pNotyfyIcon.Icon")));
            this.m_pNotyfyIcon.Text = "DataSmart Mail Server";
            this.m_pNotyfyIcon.Visible = true;
            this.m_pNotyfyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.m_pNotyfyIcon_MouseDoubleClick);
            // 
            // TrayAppForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.ControlBox = false;
            this.Name = "TrayAppForm";
            this.ShowInTaskbar = false;
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.m_pMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void m_pMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Tag == null)
            {
                return;
            }
            if (e.ClickedItem.Tag.ToString() == "manager")
            {
                var p = Process.Start(Application.StartupPath + "/mailservermanager.exe");
                p.WaitForInputIdle();
                mgrHandle = p.Handle;
                return;
            }
            if (e.ClickedItem.Tag.ToString() == "start")
            {
                this.Start();

                return;
            }
            if (e.ClickedItem.Tag.ToString() == "stop")
            {
                this.Stop();
                return;
            }
            if (e.ClickedItem.Tag.ToString() == "exit")
            {
                this.Exit();
            }
        }

        private void Start()
        {
            try
            {
                this.m_pMailServer.Start();
                this.m_pMenu.Items[1].Enabled = false;
                this.m_pMenu.Items[2].Enabled = true;
            }
            catch (Exception x)
            {
                Error.DumpError(x);
                MessageBox.Show(x.Message + "\r\n \r\n" + x.StackTrace);
            }
        }

        private void Stop()
        {
            try
            {
                this.m_pMailServer.Stop();
                this.m_pMenu.Items[1].Enabled = true;
                this.m_pMenu.Items[2].Enabled = false;
            }
            catch (Exception x)
            {
                Error.DumpError(x);
            }
        }

        private void Exit()
        {
            try
            {
                base.Close();
            }
            catch (Exception x)
            {
                Error.DumpError(x);
            }
        }
        IntPtr mgrHandle;
        private void m_pNotyfyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (mgrHandle == IntPtr.Zero)
            {
                var p = Process.Start(Application.StartupPath + "/mailservermanager.exe");
                p.WaitForInputIdle();
                mgrHandle = p.Handle;
            }
        }
    }
}
