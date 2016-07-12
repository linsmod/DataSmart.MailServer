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
			base.ShowInTaskbar = false;
			base.ControlBox = false;
			base.WindowState = FormWindowState.Minimized;
			this.m_pMenu = new ContextMenuStrip();
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("Open Manager");
			toolStripMenuItem.Tag = "manager";
			this.m_pMenu.Items.Add(toolStripMenuItem);
			this.m_pMenu.Items.Add(new ToolStripSeparator());
			ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem("Start");
			toolStripMenuItem2.Tag = "start";
			this.m_pMenu.Items.Add(toolStripMenuItem2);
			ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem("Stop");
			toolStripMenuItem3.Tag = "stop";
			this.m_pMenu.Items.Add(toolStripMenuItem3);
			this.m_pMenu.Items.Add(new ToolStripSeparator());
			ToolStripMenuItem toolStripMenuItem4 = new ToolStripMenuItem("Exit");
			toolStripMenuItem4.Image = ResManager.GetIcon("exit.ico").ToBitmap();
			toolStripMenuItem4.Tag = "exit";
			this.m_pMenu.Items.Add(toolStripMenuItem4);
			this.m_pMenu.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pMenu_ItemClicked);
			this.m_pNotyfyIcon = new NotifyIcon();
			this.m_pNotyfyIcon.Icon = ResManager.GetIcon("trayicon.ico");
			this.m_pNotyfyIcon.ContextMenuStrip = this.m_pMenu;
			this.m_pNotyfyIcon.Text = "DataSmart Mail Server";
			this.m_pNotyfyIcon.Visible = true;
		}

		private void m_pMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			if (e.ClickedItem.Tag.ToString() == "manager")
			{
				Process.Start(Application.StartupPath + "/mailservermanager.exe");
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
				this.m_pMenu.Items[2].Enabled = false;
				this.m_pMenu.Items[3].Enabled = true;
			}
			catch (Exception x)
			{
				Error.DumpError(x);
			}
		}

		private void Stop()
		{
			try
			{
				this.m_pMailServer.Stop();
				this.m_pMenu.Items[2].Enabled = true;
				this.m_pMenu.Items[3].Enabled = false;
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
	}
}
