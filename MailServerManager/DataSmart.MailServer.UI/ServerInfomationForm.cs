using DataSmart.MailServer.Management;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class ServerInfomationForm : Form
	{
		private GroupBox m_pGroupbox1;

		private Label mt_OS;

		private Label m_pOS;

		private Label mt_MemUsage;

		private Label m_pMemUsage;

		private Label mt_ServerVersion;

		private Label m_pServerVersion;

		private Label mt_CpuUsage;

		private WLineGraph m_pCpuUsage;

		private Label mt_BandwidthUsage;

		private WLineGraph m_pBandwidthUsage;

		private Label mt_MaxBandwidth;

		private Label m_pMaxBandwidth;

		private Panel m_pReadColor;

		private Label mt_Read;

		private Panel m_pWriteColor;

		private Label mt_Write;

		private Label mt_ConnectionsUsage;

		private WLineGraph m_pConnectionsUsage;

		private Panel m_pSmtpColor;

		private Label mt_Smtp;

		private Panel m_pPop3Color;

		private Label mt_Pop3;

		private Panel m_pImapColor;

		private Label mt_Imap;

		private Panel m_pRelayColor;

		private Label mt_Relay;

		private Server m_pServer;

		private bool m_Run;

		private ServerInfo m_pServerInfo;

		public ServerInfomationForm(Server server)
		{
			this.m_pServer = server;
			this.InitializeComponent();
			this.Start();
		}

		protected override void Dispose(bool disposing)
		{
			this.m_Run = false;
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.m_pGroupbox1 = new GroupBox();
			this.mt_OS = new Label();
			this.m_pOS = new Label();
			this.mt_MemUsage = new Label();
			this.m_pMemUsage = new Label();
			this.mt_ServerVersion = new Label();
			this.m_pServerVersion = new Label();
			this.mt_CpuUsage = new Label();
			this.mt_BandwidthUsage = new Label();
			this.mt_MaxBandwidth = new Label();
			this.m_pMaxBandwidth = new Label();
			this.m_pReadColor = new Panel();
			this.mt_Read = new Label();
			this.m_pWriteColor = new Panel();
			this.mt_Write = new Label();
			this.mt_ConnectionsUsage = new Label();
			this.m_pSmtpColor = new Panel();
			this.mt_Smtp = new Label();
			this.m_pPop3Color = new Panel();
			this.mt_Pop3 = new Label();
			this.m_pImapColor = new Panel();
			this.mt_Imap = new Label();
			this.m_pRelayColor = new Panel();
			this.mt_Relay = new Label();
			this.m_pCpuUsage = new WLineGraph();
			this.m_pConnectionsUsage = new WLineGraph();
			this.m_pBandwidthUsage = new WLineGraph();
			this.m_pGroupbox1.SuspendLayout();
			base.SuspendLayout();
			this.m_pGroupbox1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pGroupbox1.Controls.Add(this.mt_OS);
			this.m_pGroupbox1.Controls.Add(this.mt_Relay);
			this.m_pGroupbox1.Controls.Add(this.m_pOS);
			this.m_pGroupbox1.Controls.Add(this.m_pRelayColor);
			this.m_pGroupbox1.Controls.Add(this.mt_MemUsage);
			this.m_pGroupbox1.Controls.Add(this.mt_Imap);
			this.m_pGroupbox1.Controls.Add(this.m_pMemUsage);
			this.m_pGroupbox1.Controls.Add(this.m_pImapColor);
			this.m_pGroupbox1.Controls.Add(this.mt_ServerVersion);
			this.m_pGroupbox1.Controls.Add(this.mt_Pop3);
			this.m_pGroupbox1.Controls.Add(this.m_pServerVersion);
			this.m_pGroupbox1.Controls.Add(this.m_pPop3Color);
			this.m_pGroupbox1.Controls.Add(this.mt_CpuUsage);
			this.m_pGroupbox1.Controls.Add(this.mt_Smtp);
			this.m_pGroupbox1.Controls.Add(this.m_pCpuUsage);
			this.m_pGroupbox1.Controls.Add(this.m_pSmtpColor);
			this.m_pGroupbox1.Controls.Add(this.mt_BandwidthUsage);
			this.m_pGroupbox1.Controls.Add(this.m_pConnectionsUsage);
			this.m_pGroupbox1.Controls.Add(this.m_pBandwidthUsage);
			this.m_pGroupbox1.Controls.Add(this.mt_ConnectionsUsage);
			this.m_pGroupbox1.Controls.Add(this.mt_MaxBandwidth);
			this.m_pGroupbox1.Controls.Add(this.mt_Write);
			this.m_pGroupbox1.Controls.Add(this.m_pMaxBandwidth);
			this.m_pGroupbox1.Controls.Add(this.m_pWriteColor);
			this.m_pGroupbox1.Controls.Add(this.m_pReadColor);
			this.m_pGroupbox1.Controls.Add(this.mt_Read);
			this.m_pGroupbox1.Location = new Point(12, 12);
			this.m_pGroupbox1.Name = "m_pGroupbox1";
			this.m_pGroupbox1.Size = new Size(643, 542);
			this.m_pGroupbox1.TabIndex = 1;
			this.m_pGroupbox1.TabStop = false;
			this.m_pGroupbox1.Text = "SERVER INFOMATION";
			this.mt_OS.Location = new Point(21, 33);
			this.mt_OS.Name = "mt_OS";
			this.mt_OS.Size = new Size(88, 20);
			this.mt_OS.TabIndex = 2;
			this.mt_OS.Text = "OS:";
			this.mt_OS.TextAlign = ContentAlignment.MiddleRight;
			this.m_pOS.Location = new Point(126, 33);
			this.m_pOS.Name = "m_pOS";
			this.m_pOS.Size = new Size(200, 20);
			this.m_pOS.TabIndex = 3;
			this.m_pOS.Text = "LOADING...";
			this.m_pOS.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_MemUsage.Location = new Point(21, 60);
			this.mt_MemUsage.Name = "mt_MemUsage";
			this.mt_MemUsage.Size = new Size(88, 20);
			this.mt_MemUsage.TabIndex = 4;
			this.mt_MemUsage.Text = "Memory Usage:";
			this.mt_MemUsage.TextAlign = ContentAlignment.MiddleRight;
			this.m_pMemUsage.Location = new Point(126, 61);
			this.m_pMemUsage.Name = "m_pMemUsage";
			this.m_pMemUsage.Size = new Size(200, 20);
			this.m_pMemUsage.TabIndex = 5;
			this.m_pMemUsage.Text = "LOADING...";
			this.m_pMemUsage.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_ServerVersion.Location = new Point(21, 88);
			this.mt_ServerVersion.Name = "mt_ServerVersion";
			this.mt_ServerVersion.Size = new Size(88, 20);
			this.mt_ServerVersion.TabIndex = 6;
			this.mt_ServerVersion.Text = "Version:";
			this.mt_ServerVersion.TextAlign = ContentAlignment.MiddleRight;
			this.m_pServerVersion.Location = new Point(126, 88);
			this.m_pServerVersion.Name = "m_pServerVersion";
			this.m_pServerVersion.Size = new Size(200, 20);
			this.m_pServerVersion.TabIndex = 7;
			this.m_pServerVersion.Text = "LOADING...";
			this.m_pServerVersion.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_CpuUsage.Location = new Point(21, 117);
			this.mt_CpuUsage.Name = "mt_CpuUsage";
			this.mt_CpuUsage.Size = new Size(88, 20);
			this.mt_CpuUsage.TabIndex = 8;
			this.mt_CpuUsage.Text = "CPU Usage:";
			this.mt_CpuUsage.TextAlign = ContentAlignment.MiddleRight;
			this.mt_BandwidthUsage.Location = new Point(6, 242);
			this.mt_BandwidthUsage.Name = "mt_BandwidthUsage";
			this.mt_BandwidthUsage.Size = new Size(103, 20);
			this.mt_BandwidthUsage.TabIndex = 10;
			this.mt_BandwidthUsage.Text = "Bandwidth Usage:";
			this.mt_BandwidthUsage.TextAlign = ContentAlignment.MiddleRight;
			this.mt_MaxBandwidth.Location = new Point(21, 352);
			this.mt_MaxBandwidth.Name = "mt_MaxBandwidth";
			this.mt_MaxBandwidth.Size = new Size(88, 20);
			this.mt_MaxBandwidth.TabIndex = 12;
			this.mt_MaxBandwidth.Text = "Max bandwidth:";
			this.mt_MaxBandwidth.TextAlign = ContentAlignment.MiddleRight;
			this.m_pMaxBandwidth.Location = new Point(126, 352);
			this.m_pMaxBandwidth.Name = "m_pMaxBandwidth";
			this.m_pMaxBandwidth.Size = new Size(80, 20);
			this.m_pMaxBandwidth.TabIndex = 13;
			this.m_pMaxBandwidth.Text = "N/A";
			this.m_pMaxBandwidth.TextAlign = ContentAlignment.MiddleLeft;
			this.m_pReadColor.BackColor = Color.LightGreen;
			this.m_pReadColor.BorderStyle = BorderStyle.FixedSingle;
			this.m_pReadColor.Location = new Point(211, 354);
			this.m_pReadColor.Name = "m_pReadColor";
			this.m_pReadColor.Size = new Size(16, 16);
			this.m_pReadColor.TabIndex = 14;
			this.mt_Read.Location = new Point(231, 352);
			this.mt_Read.Name = "mt_Read";
			this.mt_Read.Size = new Size(100, 20);
			this.mt_Read.TabIndex = 15;
			this.mt_Read.Text = "Reads";
			this.mt_Read.TextAlign = ContentAlignment.MiddleLeft;
			this.m_pWriteColor.BackColor = Color.Red;
			this.m_pWriteColor.BorderStyle = BorderStyle.FixedSingle;
			this.m_pWriteColor.Location = new Point(341, 354);
			this.m_pWriteColor.Name = "m_pWriteColor";
			this.m_pWriteColor.Size = new Size(16, 16);
			this.m_pWriteColor.TabIndex = 16;
			this.mt_Write.Location = new Point(361, 352);
			this.mt_Write.Name = "mt_Write";
			this.mt_Write.Size = new Size(120, 20);
			this.mt_Write.TabIndex = 17;
			this.mt_Write.Text = "Writes";
			this.mt_Write.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_ConnectionsUsage.Location = new Point(6, 397);
			this.mt_ConnectionsUsage.Name = "mt_ConnectionsUsage";
			this.mt_ConnectionsUsage.Size = new Size(103, 20);
			this.mt_ConnectionsUsage.TabIndex = 18;
			this.mt_ConnectionsUsage.Text = "Connections Usage:";
			this.mt_ConnectionsUsage.TextAlign = ContentAlignment.MiddleRight;
			this.m_pSmtpColor.BackColor = Color.LightGreen;
			this.m_pSmtpColor.BorderStyle = BorderStyle.FixedSingle;
			this.m_pSmtpColor.Location = new Point(126, 487);
			this.m_pSmtpColor.Name = "m_pSmtpColor";
			this.m_pSmtpColor.Size = new Size(14, 14);
			this.m_pSmtpColor.TabIndex = 20;
			this.mt_Smtp.Location = new Point(141, 487);
			this.mt_Smtp.Name = "mt_Smtp";
			this.mt_Smtp.Size = new Size(80, 14);
			this.mt_Smtp.TabIndex = 21;
			this.mt_Smtp.Text = "SMTP";
			this.mt_Smtp.TextAlign = ContentAlignment.MiddleLeft;
			this.m_pPop3Color.BackColor = Color.Red;
			this.m_pPop3Color.BorderStyle = BorderStyle.FixedSingle;
			this.m_pPop3Color.Location = new Point(221, 487);
			this.m_pPop3Color.Name = "m_pPop3Color";
			this.m_pPop3Color.Size = new Size(14, 14);
			this.m_pPop3Color.TabIndex = 22;
			this.mt_Pop3.Location = new Point(236, 487);
			this.mt_Pop3.Name = "mt_Pop3";
			this.mt_Pop3.Size = new Size(75, 14);
			this.mt_Pop3.TabIndex = 23;
			this.mt_Pop3.Text = "POP3";
			this.mt_Pop3.TextAlign = ContentAlignment.MiddleLeft;
			this.m_pImapColor.BackColor = Color.Yellow;
			this.m_pImapColor.BorderStyle = BorderStyle.FixedSingle;
			this.m_pImapColor.Location = new Point(311, 487);
			this.m_pImapColor.Name = "m_pImapColor";
			this.m_pImapColor.Size = new Size(14, 14);
			this.m_pImapColor.TabIndex = 24;
			this.mt_Imap.Location = new Point(326, 487);
			this.mt_Imap.Name = "mt_Imap";
			this.mt_Imap.Size = new Size(75, 14);
			this.mt_Imap.TabIndex = 25;
			this.mt_Imap.Text = "IMAP";
			this.mt_Imap.TextAlign = ContentAlignment.MiddleLeft;
			this.m_pRelayColor.BackColor = Color.DarkOrange;
			this.m_pRelayColor.BorderStyle = BorderStyle.FixedSingle;
			this.m_pRelayColor.Location = new Point(401, 487);
			this.m_pRelayColor.Name = "m_pRelayColor";
			this.m_pRelayColor.Size = new Size(14, 14);
			this.m_pRelayColor.TabIndex = 26;
			this.mt_Relay.Location = new Point(416, 487);
			this.mt_Relay.Name = "mt_Relay";
			this.mt_Relay.Size = new Size(80, 14);
			this.mt_Relay.TabIndex = 27;
			this.mt_Relay.Text = "Relay";
			this.mt_Relay.TextAlign = ContentAlignment.MiddleLeft;
			this.m_pCpuUsage.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pCpuUsage.AutoMaxValue = false;
			this.m_pCpuUsage.Location = new Point(126, 117);
			this.m_pCpuUsage.MaximumValue = 100;
			this.m_pCpuUsage.Name = "m_pCpuUsage";
			this.m_pCpuUsage.Size = new Size(505, 100);
			this.m_pCpuUsage.TabIndex = 9;
			this.m_pConnectionsUsage.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pConnectionsUsage.AutoMaxValue = true;
			this.m_pConnectionsUsage.Location = new Point(126, 397);
			this.m_pConnectionsUsage.MaximumValue = 100;
			this.m_pConnectionsUsage.Name = "m_pConnectionsUsage";
			this.m_pConnectionsUsage.Size = new Size(505, 80);
			this.m_pConnectionsUsage.TabIndex = 19;
			this.m_pBandwidthUsage.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pBandwidthUsage.AutoMaxValue = true;
			this.m_pBandwidthUsage.Location = new Point(126, 242);
			this.m_pBandwidthUsage.MaximumValue = 100;
			this.m_pBandwidthUsage.Name = "m_pBandwidthUsage";
			this.m_pBandwidthUsage.Size = new Size(505, 100);
			this.m_pBandwidthUsage.TabIndex = 11;
			base.ClientSize = new Size(667, 566);
			base.Controls.Add(this.m_pGroupbox1);
			base.Name = "wfrm_Server_Info";

            m_pCpuUsage.AddLine(Color.LightGreen);
            m_pBandwidthUsage.AddLine(Color.LightGreen);
            m_pBandwidthUsage.AddLine(Color.Red);
            m_pConnectionsUsage.AddLine(Color.LightGreen);
            m_pConnectionsUsage.AddLine(Color.Red);
            m_pConnectionsUsage.AddLine(Color.Yellow);
            m_pConnectionsUsage.AddLine(Color.DarkOrange);

            this.m_pGroupbox1.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		private void Start()
		{
			this.m_Run = true;
			Thread thread = new Thread(new ThreadStart(this.Run));
            thread.Name = "Server Infomation Refresh Thread";
			thread.Start();
		}

		private void Run()
		{
			while (this.m_Run)
			{
				try
				{
					this.m_pServerInfo = this.m_pServer.ServerInfo;
					base.Invoke(new ThreadStart(this.RefreshUI));
				}
				catch
				{
				}
				Thread.Sleep(1500);
			}
		}

		private void RefreshUI()
		{
			this.m_pOS.Text = this.m_pServerInfo.OS;
			this.m_pMemUsage.Text = this.m_pServerInfo.MemoryUsage + " MB";
			this.m_pServerVersion.Text = this.m_pServerInfo.MailServerVersion;
			this.m_pCpuUsage.AddValue(new int[]
			{
				Math.Min(this.m_pServerInfo.CpuUsage, 100)
			});
			this.m_pBandwidthUsage.AddValue(new int[]
			{
				this.m_pServerInfo.ReadsInSecond,
				this.m_pServerInfo.WritesInSecond
			});
			this.m_pMaxBandwidth.Text = this.m_pBandwidthUsage.MaximumValue + " KB";
			this.mt_Read.Text = "Reads (" + this.m_pServerInfo.ReadsInSecond + " KB)";
			this.mt_Write.Text = "Writes (" + this.m_pServerInfo.WritesInSecond + " KB)";
			this.m_pConnectionsUsage.AddValue(new int[]
			{
				this.m_pServerInfo.TotalSmtpSessions,
				this.m_pServerInfo.TotalPop3Sessions,
				this.m_pServerInfo.TotalImapSessions,
				this.m_pServerInfo.TotalRelaySessions
			});
			this.mt_Smtp.Text = "SMTP (" + this.m_pServerInfo.TotalSmtpSessions + ")";
			this.mt_Pop3.Text = "POP3 (" + this.m_pServerInfo.TotalPop3Sessions + ")";
			this.mt_Imap.Text = "IMAP (" + this.m_pServerInfo.TotalImapSessions + ")";
			this.mt_Relay.Text = "Relay (" + this.m_pServerInfo.TotalRelaySessions + ")";
		}
	}
}
