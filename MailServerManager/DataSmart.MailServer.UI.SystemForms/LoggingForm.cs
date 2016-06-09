using DataSmart.MailServer.Management;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI.SystemForms
{
    public class LoggingForm : Form
    {
        private TabControl m_pTab;

        private Button m_pApply;

        private CheckBox m_pLogSMTP;

        private Label mt_SMTPLogPath;

        private TextBox m_pSMTPLogPath;

        private Button m_pGetSMTPLogPath;

        private CheckBox m_pLogPOP3;

        private Label mt_POP3LogPath;

        private TextBox m_pPOP3LogPath;

        private Button m_pGetPOP3LogPath;

        private CheckBox m_pLogIMAP;

        private Label mt_IMAPLogPath;

        private TextBox m_pIMAPLogPath;

        private Button m_pGetIMAPLogPath;

        private CheckBox m_pLogRelay;

        private Label mt_RelayLogPath;

        private TextBox m_pRelayLogPath;

        private Button m_pGetRelayLogPath;

        private CheckBox m_pLogFetchPOP3;

        private Label mt_FetchPOP3LogPath;

        private TextBox m_pFetchPOP3LogPath;

        private Button m_pGetFetchPOP3LogPath;

        private CheckBox m_pLogServer;

        private Label mt_ServerLogPath;

        private TextBox m_pServerLogPath;

        private Button m_pGetServerLogPath;

        private VirtualServer m_pVirtualServer;

        public LoggingForm(VirtualServer virtualServer)
        {
            this.m_pVirtualServer = virtualServer;
            this.InitializeComponent();
            this.LoadData();
        }

        private void InitializeComponent()
        {
            this.m_pTab = new TabControl();
            this.m_pTab.Size = new Size(515, 490);
            this.m_pTab.Location = new Point(5, 0);
            this.m_pTab.TabPages.Add(new TabPage("General"));
            this.m_pApply = new Button();
            this.m_pApply.Size = new Size(70, 20);
            this.m_pApply.Location = new Point(450, 500);
            this.m_pApply.Text = "Apply";
            this.m_pApply.Click += new EventHandler(this.m_pApply_Click);
            this.m_pLogSMTP = new CheckBox();
            this.m_pLogSMTP.Size = new Size(150, 20);
            this.m_pLogSMTP.Location = new Point(10, 20);
            this.m_pLogSMTP.Text = "Log SMTP";
            this.mt_SMTPLogPath = new Label();
            this.mt_SMTPLogPath.Size = new Size(50, 20);
            this.mt_SMTPLogPath.Location = new Point(10, 45);
            this.mt_SMTPLogPath.TextAlign = ContentAlignment.MiddleRight;
            this.mt_SMTPLogPath.Text = "Path:";
            this.m_pSMTPLogPath = new TextBox();
            this.m_pSMTPLogPath.Size = new Size(400, 20);
            this.m_pSMTPLogPath.Location = new Point(65, 45);
            this.m_pSMTPLogPath.ReadOnly = true;
            this.m_pGetSMTPLogPath = new Button();
            this.m_pGetSMTPLogPath.Size = new Size(25, 20);
            this.m_pGetSMTPLogPath.Location = new Point(470, 45);
            this.m_pGetSMTPLogPath.Text = "...";
            this.m_pGetSMTPLogPath.Click += new EventHandler(this.m_pGetSMTPLogPath_Click);
            this.m_pLogPOP3 = new CheckBox();
            this.m_pLogPOP3.Size = new Size(150, 20);
            this.m_pLogPOP3.Location = new Point(10, 70);
            this.m_pLogPOP3.Text = "Log POP3";
            this.mt_POP3LogPath = new Label();
            this.mt_POP3LogPath.Size = new Size(50, 20);
            this.mt_POP3LogPath.Location = new Point(10, 90);
            this.mt_POP3LogPath.TextAlign = ContentAlignment.MiddleRight;
            this.mt_POP3LogPath.Text = "Path:";
            this.m_pPOP3LogPath = new TextBox();
            this.m_pPOP3LogPath.Size = new Size(400, 20);
            this.m_pPOP3LogPath.Location = new Point(65, 90);
            this.m_pPOP3LogPath.ReadOnly = true;
            this.m_pGetPOP3LogPath = new Button();
            this.m_pGetPOP3LogPath.Size = new Size(25, 20);
            this.m_pGetPOP3LogPath.Location = new Point(470, 90);
            this.m_pGetPOP3LogPath.Text = "...";
            this.m_pGetPOP3LogPath.Click += new EventHandler(this.m_pGetPOP3LogPath_Click);
            this.m_pLogIMAP = new CheckBox();
            this.m_pLogIMAP.Size = new Size(150, 20);
            this.m_pLogIMAP.Location = new Point(10, 120);
            this.m_pLogIMAP.Text = "Log IMAP";
            this.mt_IMAPLogPath = new Label();
            this.mt_IMAPLogPath.Size = new Size(50, 20);
            this.mt_IMAPLogPath.Location = new Point(10, 140);
            this.mt_IMAPLogPath.TextAlign = ContentAlignment.MiddleRight;
            this.mt_IMAPLogPath.Text = "Path:";
            this.m_pIMAPLogPath = new TextBox();
            this.m_pIMAPLogPath.Size = new Size(400, 20);
            this.m_pIMAPLogPath.Location = new Point(65, 140);
            this.m_pIMAPLogPath.ReadOnly = true;
            this.m_pGetIMAPLogPath = new Button();
            this.m_pGetIMAPLogPath.Size = new Size(25, 20);
            this.m_pGetIMAPLogPath.Location = new Point(470, 140);
            this.m_pGetIMAPLogPath.Text = "...";
            this.m_pGetIMAPLogPath.Click += new EventHandler(this.m_pGetIMAPLogPath_Click);
            this.m_pLogRelay = new CheckBox();
            this.m_pLogRelay.Size = new Size(150, 20);
            this.m_pLogRelay.Location = new Point(10, 170);
            this.m_pLogRelay.Text = "Log Relay";
            this.mt_RelayLogPath = new Label();
            this.mt_RelayLogPath.Size = new Size(50, 20);
            this.mt_RelayLogPath.Location = new Point(10, 190);
            this.mt_RelayLogPath.TextAlign = ContentAlignment.MiddleRight;
            this.mt_RelayLogPath.Text = "Path:";
            this.m_pRelayLogPath = new TextBox();
            this.m_pRelayLogPath.Size = new Size(400, 20);
            this.m_pRelayLogPath.Location = new Point(65, 190);
            this.m_pRelayLogPath.ReadOnly = true;
            this.m_pGetRelayLogPath = new Button();
            this.m_pGetRelayLogPath.Size = new Size(25, 20);
            this.m_pGetRelayLogPath.Location = new Point(470, 190);
            this.m_pGetRelayLogPath.Text = "...";
            this.m_pGetRelayLogPath.Click += new EventHandler(this.m_pGetRelayLogPath_Click);
            this.m_pLogFetchPOP3 = new CheckBox();
            this.m_pLogFetchPOP3.Size = new Size(150, 20);
            this.m_pLogFetchPOP3.Location = new Point(10, 220);
            this.m_pLogFetchPOP3.Text = "Log Fetch POP3";
            this.mt_FetchPOP3LogPath = new Label();
            this.mt_FetchPOP3LogPath.Size = new Size(50, 20);
            this.mt_FetchPOP3LogPath.Location = new Point(10, 240);
            this.mt_FetchPOP3LogPath.TextAlign = ContentAlignment.MiddleRight;
            this.mt_FetchPOP3LogPath.Text = "Path:";
            this.m_pFetchPOP3LogPath = new TextBox();
            this.m_pFetchPOP3LogPath.Size = new Size(400, 20);
            this.m_pFetchPOP3LogPath.Location = new Point(65, 240);
            this.m_pFetchPOP3LogPath.ReadOnly = true;
            this.m_pGetFetchPOP3LogPath = new Button();
            this.m_pGetFetchPOP3LogPath.Size = new Size(25, 20);
            this.m_pGetFetchPOP3LogPath.Location = new Point(470, 240);
            this.m_pGetFetchPOP3LogPath.Text = "...";
            this.m_pGetFetchPOP3LogPath.Click += new EventHandler(this.m_pGetFetchPOP3LogPath_Click);
            this.m_pLogServer = new CheckBox();
            this.m_pLogServer.Size = new Size(150, 20);
            this.m_pLogServer.Location = new Point(10, 270);
            this.m_pLogServer.Text = "Log Server";
            this.m_pLogServer.Checked = true;
            this.m_pLogServer.Enabled = false;
            this.mt_ServerLogPath = new Label();
            this.mt_ServerLogPath.Size = new Size(50, 20);
            this.mt_ServerLogPath.Location = new Point(10, 290);
            this.mt_ServerLogPath.TextAlign = ContentAlignment.MiddleRight;
            this.mt_ServerLogPath.Text = "Path:";
            this.m_pServerLogPath = new TextBox();
            this.m_pServerLogPath.Size = new Size(400, 20);
            this.m_pServerLogPath.Location = new Point(65, 290);
            this.m_pServerLogPath.ReadOnly = true;
            this.m_pGetServerLogPath = new Button();
            this.m_pGetServerLogPath.Size = new Size(25, 20);
            this.m_pGetServerLogPath.Location = new Point(470, 290);
            this.m_pGetServerLogPath.Text = "...";
            this.m_pTab.TabPages[0].Controls.Add(this.m_pLogSMTP);
            this.m_pTab.TabPages[0].Controls.Add(this.mt_SMTPLogPath);
            this.m_pTab.TabPages[0].Controls.Add(this.m_pSMTPLogPath);
            this.m_pTab.TabPages[0].Controls.Add(this.m_pGetSMTPLogPath);
            this.m_pTab.TabPages[0].Controls.Add(this.m_pLogPOP3);
            this.m_pTab.TabPages[0].Controls.Add(this.mt_POP3LogPath);
            this.m_pTab.TabPages[0].Controls.Add(this.m_pPOP3LogPath);
            this.m_pTab.TabPages[0].Controls.Add(this.m_pGetPOP3LogPath);
            this.m_pTab.TabPages[0].Controls.Add(this.m_pLogIMAP);
            this.m_pTab.TabPages[0].Controls.Add(this.mt_IMAPLogPath);
            this.m_pTab.TabPages[0].Controls.Add(this.m_pIMAPLogPath);
            this.m_pTab.TabPages[0].Controls.Add(this.m_pGetIMAPLogPath);
            this.m_pTab.TabPages[0].Controls.Add(this.m_pLogRelay);
            this.m_pTab.TabPages[0].Controls.Add(this.mt_RelayLogPath);
            this.m_pTab.TabPages[0].Controls.Add(this.m_pRelayLogPath);
            this.m_pTab.TabPages[0].Controls.Add(this.m_pGetRelayLogPath);
            this.m_pTab.TabPages[0].Controls.Add(this.m_pLogFetchPOP3);
            this.m_pTab.TabPages[0].Controls.Add(this.mt_FetchPOP3LogPath);
            this.m_pTab.TabPages[0].Controls.Add(this.m_pFetchPOP3LogPath);
            this.m_pTab.TabPages[0].Controls.Add(this.m_pGetFetchPOP3LogPath);
            this.m_pTab.TabPages[0].Controls.Add(this.m_pLogServer);
            this.m_pTab.TabPages[0].Controls.Add(this.mt_ServerLogPath);
            this.m_pTab.TabPages[0].Controls.Add(this.m_pServerLogPath);
            this.m_pTab.TabPages[0].Controls.Add(this.m_pGetServerLogPath);
            base.Controls.Add(this.m_pTab);
            base.Controls.Add(this.m_pApply);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (!base.Visible)
            {
                this.SaveData(true);
            }
        }

        private void m_pGetSMTPLogPath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.SelectedPath = this.m_pSMTPLogPath.Text;
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    this.m_pSMTPLogPath.Text = folderBrowserDialog.SelectedPath;
                    if (this.m_pSMTPLogPath.Text.ToLower() == (Application.StartupPath + "\\Logs\\Smtp").ToLower())
                    {
                        this.m_pSMTPLogPath.Text = "";
                    }
                }
            }
        }

        private void m_pGetPOP3LogPath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.SelectedPath = this.m_pPOP3LogPath.Text;
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    this.m_pPOP3LogPath.Text = folderBrowserDialog.SelectedPath;
                    if (this.m_pPOP3LogPath.Text.ToLower() == (Application.StartupPath + "\\Logs\\Pop3").ToLower())
                    {
                        this.m_pPOP3LogPath.Text = "";
                    }
                }
            }
        }

        private void m_pGetIMAPLogPath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.SelectedPath = this.m_pIMAPLogPath.Text;
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    this.m_pIMAPLogPath.Text = folderBrowserDialog.SelectedPath;
                    if (this.m_pIMAPLogPath.Text.ToLower() == (Application.StartupPath + "\\Logs\\IMAP").ToLower())
                    {
                        this.m_pIMAPLogPath.Text = "";
                    }
                }
            }
        }

        private void m_pGetRelayLogPath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.SelectedPath = this.m_pRelayLogPath.Text;
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    this.m_pRelayLogPath.Text = folderBrowserDialog.SelectedPath;
                    if (this.m_pRelayLogPath.Text.ToLower() == (Application.StartupPath + "\\Logs\\Relay").ToLower())
                    {
                        this.m_pRelayLogPath.Text = "";
                    }
                }
            }
        }

        private void m_pGetFetchPOP3LogPath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.SelectedPath = this.m_pFetchPOP3LogPath.Text;
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    this.m_pFetchPOP3LogPath.Text = folderBrowserDialog.SelectedPath;
                    if (this.m_pFetchPOP3LogPath.Text.ToLower() == (Application.StartupPath + "\\Logs\\FetchPOP3").ToLower())
                    {
                        this.m_pFetchPOP3LogPath.Text = "";
                    }
                }
            }
        }

        private void m_pApply_Click(object sender, EventArgs e)
        {
            this.SaveData(false);
        }

        private void LoadData()
        {
            try
            {
                Logging_Settings logging = this.m_pVirtualServer.SystemSettings.Logging;
                this.m_pLogSMTP.Checked = logging.LogSMTP;
                this.m_pSMTPLogPath.Text = logging.SmtpLogsPath;
                this.m_pLogPOP3.Checked = logging.LogPOP3;
                this.m_pPOP3LogPath.Text = logging.Pop3LogsPath;
                this.m_pLogIMAP.Checked = logging.LogIMAP;
                this.m_pIMAPLogPath.Text = logging.ImapLogsPath;
                this.m_pLogRelay.Checked = logging.LogRelay;
                this.m_pRelayLogPath.Text = logging.RelayLogsPath;
                this.m_pLogFetchPOP3.Checked = logging.LogFetchMessages;
                this.m_pFetchPOP3LogPath.Text = logging.FetchMessagesLogsPath;
            }
            catch (Exception x)
            {
                ErrorForm errorForm = new ErrorForm(x, new StackTrace());
                errorForm.ShowDialog(this);
            }
        }

        private void SaveData(bool confirmSave)
        {
            try
            {
                Logging_Settings logging = this.m_pVirtualServer.SystemSettings.Logging;
                logging.LogSMTP = this.m_pLogSMTP.Checked;
                logging.SmtpLogsPath = this.m_pSMTPLogPath.Text;
                logging.LogPOP3 = this.m_pLogPOP3.Checked;
                logging.Pop3LogsPath = this.m_pPOP3LogPath.Text;
                logging.LogIMAP = this.m_pLogIMAP.Checked;
                logging.ImapLogsPath = this.m_pIMAPLogPath.Text;
                logging.LogRelay = this.m_pLogRelay.Checked;
                logging.RelayLogsPath = this.m_pRelayLogPath.Text;
                logging.LogFetchMessages = this.m_pLogFetchPOP3.Checked;
                logging.FetchMessagesLogsPath = this.m_pFetchPOP3LogPath.Text;
                if (this.m_pVirtualServer.SystemSettings.HasChanges && (!confirmSave || MessageBox.Show(this, "You have changes settings, do you want to save them ?", "Confirm:", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
                {
                    this.m_pVirtualServer.SystemSettings.Commit();
                }
            }
            catch (Exception x)
            {
                ErrorForm errorForm = new ErrorForm(x, new StackTrace());
                errorForm.ShowDialog(this);
            }
        }
    }
}
