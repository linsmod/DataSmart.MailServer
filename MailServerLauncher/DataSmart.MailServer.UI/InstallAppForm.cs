using DataSmart.MailServer.Resources;
using System;
using System.Diagnostics;
using System.Drawing;
using System.ServiceProcess;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class InstallAppForm : Form
	{
		private const string MailServerServiceFile = "/MailServerService.exe";

		private GroupBox m_pService;

		private Button m_pInstallAsService;

		private Button m_pUninstallService;

		private GroupBox m_pRun;

		private Button m_pRunAsTryApp;

		private Button m_pRunAsWindowsForm;

		public InstallAppForm()
		{
			this.InitializeComponent();
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				this.m_pRunAsTryApp.Enabled = true;
				this.m_pRunAsWindowsForm.Enabled = true;
				return;
			}
			if (!this.IsServiceInstalled())
			{
				this.m_pInstallAsService.Enabled = true;
				this.m_pUninstallService.Enabled = false;
				this.m_pRunAsTryApp.Enabled = true;
				this.m_pRunAsWindowsForm.Enabled = true;
				return;
			}
			this.m_pInstallAsService.Enabled = false;
			this.m_pUninstallService.Enabled = true;
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(220, 200);
			base.StartPosition = FormStartPosition.CenterScreen;
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			this.Text = "Mail Server Installer";
			base.Icon = ResManager.GetIcon("trayicon.ico");
			this.m_pService = new GroupBox();
			this.m_pService.Size = new Size(200, 80);
			this.m_pService.Location = new Point(10, 10);
			this.m_pService.Text = "Service:";
			this.m_pInstallAsService = new Button();
			this.m_pInstallAsService.Size = new Size(180, 20);
			this.m_pInstallAsService.Location = new Point(10, 20);
			this.m_pInstallAsService.Enabled = false;
			this.m_pInstallAsService.Text = "Install as Service";
			this.m_pInstallAsService.Click += new EventHandler(this.m_pInstallAsService_Click);
			this.m_pUninstallService = new Button();
			this.m_pUninstallService.Size = new Size(180, 20);
			this.m_pUninstallService.Location = new Point(10, 45);
			this.m_pUninstallService.Enabled = false;
			this.m_pUninstallService.Text = "Uninstall";
			this.m_pUninstallService.Click += new EventHandler(this.m_pUninstallService_Click);
			this.m_pService.Controls.Add(this.m_pInstallAsService);
			this.m_pService.Controls.Add(this.m_pUninstallService);
			this.m_pRun = new GroupBox();
			this.m_pRun.Size = new Size(200, 80);
			this.m_pRun.Location = new Point(10, 100);
			this.m_pRun.Text = "Run:";
			this.m_pRunAsTryApp = new Button();
			this.m_pRunAsTryApp.Size = new Size(180, 20);
			this.m_pRunAsTryApp.Location = new Point(10, 20);
			this.m_pRunAsTryApp.Enabled = false;
			this.m_pRunAsTryApp.Text = "Run as tray application";
			this.m_pRunAsTryApp.Click += new EventHandler(this.m_pRunAsTryApp_Click);
			this.m_pRunAsWindowsForm = new Button();
			this.m_pRunAsWindowsForm.Size = new Size(180, 20);
			this.m_pRunAsWindowsForm.Location = new Point(10, 45);
			this.m_pRunAsWindowsForm.Enabled = false;
			this.m_pRunAsWindowsForm.Text = "Run as windows form application";
			this.m_pRunAsWindowsForm.Click += new EventHandler(this.m_pRunAsWindowsForm_Click);
			this.m_pRun.Controls.Add(this.m_pRunAsTryApp);
			this.m_pRun.Controls.Add(this.m_pRunAsWindowsForm);
			base.Controls.Add(this.m_pService);
			base.Controls.Add(this.m_pRun);
		}

		private void m_pInstallAsService_Click(object sender, EventArgs e)
		{
			Process process = Process.Start(new ProcessStartInfo(Application.StartupPath + "/MailServerService.exe")
			{
				Arguments = "-install",
				Verb = "runas"
			});
			process.WaitForExit();
			if (process.ExitCode == 0)
			{
				this.m_pInstallAsService.Enabled = false;
				this.m_pUninstallService.Enabled = true;
				this.m_pRunAsTryApp.Enabled = false;
				this.m_pRunAsWindowsForm.Enabled = false;
			}
		}

		private void m_pUninstallService_Click(object sender, EventArgs e)
		{
			Process process = Process.Start(new ProcessStartInfo(Application.StartupPath + "/MailServerService.exe")
			{
				Arguments = "-uninstall",
				Verb = "runas"
			});
			process.WaitForExit();
			if (process.ExitCode == 0)
			{
				this.m_pInstallAsService.Enabled = true;
				this.m_pUninstallService.Enabled = false;
				this.m_pRunAsTryApp.Enabled = true;
				this.m_pRunAsWindowsForm.Enabled = true;
			}
		}

		private void m_pRunAsTryApp_Click(object sender, EventArgs e)
		{
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				Process.Start("mono", Application.ExecutablePath + " -trayapp");
			}
			else
			{
				Process.Start(Application.ExecutablePath, "-trayapp");
			}
			this.m_pInstallAsService.Enabled = false;
			this.m_pUninstallService.Enabled = false;
			this.m_pRunAsTryApp.Enabled = false;
			this.m_pRunAsWindowsForm.Enabled = false;
		}

		private void m_pRunAsWindowsForm_Click(object sender, EventArgs e)
		{
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				Process.Start("mono", Application.ExecutablePath + " - winform");
			}
			else
			{
				Process.Start(Application.ExecutablePath, "-winform");
			}
			this.m_pInstallAsService.Enabled = false;
			this.m_pUninstallService.Enabled = false;
			this.m_pRunAsTryApp.Enabled = false;
			this.m_pRunAsWindowsForm.Enabled = false;
		}

		private bool IsServiceInstalled()
		{
			ServiceController[] services = ServiceController.GetServices();
			for (int i = 0; i < services.Length; i++)
			{
				ServiceController serviceController = services[i];
				if (serviceController.ServiceName == "DataSmart Mail Server")
				{
					return true;
				}
			}
			return false;
		}
	}
}
