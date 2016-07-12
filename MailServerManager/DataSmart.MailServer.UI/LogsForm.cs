using DataSmart.MailServer.Management;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
    public class LogsForm : Form
    {
        private Label mt_VirtualServer;

        private ComboBox m_pVirtualServer;

        private Button m_pGet;

        private Label mt_Service;

        private ComboBox m_pService;

        private Label mt_Limit;

        private NumericUpDown m_pLimit;

        private Label mt_Date;

        private DateTimePicker m_pDate;

        private Label mt_Between;

        private DateTimePicker m_pStartTime;

        private DateTimePicker m_pEndTime;

        private Label mt_ContainsText;

        private TextBox m_pContainsText;

        private ListView m_pLogSessions;

        private Server m_pServer;

        public LogsForm(Server server, WFrame frame)
        {
            this.m_pServer = server;
            this.InitializeComponent();
            this.LoadVirtualServers();
            if (this.m_pVirtualServer.Items.Count > 0)
            {
                this.m_pVirtualServer.SelectedIndex = 0;
                m_pGet_Click(null, null);
            }
        }

        private void InitializeComponent()
        {
            base.Size = new Size(450, 300);
            this.mt_VirtualServer = new Label();
            this.mt_VirtualServer.Size = new Size(80, 20);
            this.mt_VirtualServer.Location = new Point(0, 20);
            this.mt_VirtualServer.TextAlign = ContentAlignment.MiddleRight;
            this.mt_VirtualServer.Text = "Virtual Server:";
            this.m_pVirtualServer = new ComboBox();
            this.m_pVirtualServer.Size = new Size(305, 20);
            this.m_pVirtualServer.Location = new Point(85, 20);
            this.m_pVirtualServer.DropDownStyle = ComboBoxStyle.DropDownList;
            this.m_pVirtualServer.SelectedIndexChanged += new EventHandler(this.m_pVirtualServer_SelectedIndexChanged);
            this.m_pGet = new Button();
            this.m_pGet.Size = new Size(70, 20);
            this.m_pGet.Location = new Point(400, 20);
            this.m_pGet.Text = "Get";
            this.m_pGet.Click += new EventHandler(this.m_pGet_Click);
            this.mt_Service = new Label();
            this.mt_Service.Size = new Size(80, 20);
            this.mt_Service.Location = new Point(0, 45);
            this.mt_Service.TextAlign = ContentAlignment.MiddleRight;
            this.mt_Service.Text = "Service:";
            this.m_pService = new ComboBox();
            this.m_pService.Size = new Size(120, 20);
            this.m_pService.Location = new Point(85, 45);
            this.m_pService.DropDownStyle = ComboBoxStyle.DropDownList;
            this.m_pService.SelectedIndexChanged += new EventHandler(this.m_pService_SelectedIndexChanged);
            this.m_pService.Items.Add("SMTP");
            this.m_pService.Items.Add("POP3");
            this.m_pService.Items.Add("IMAP");
            this.m_pService.Items.Add("RELAY");
            this.m_pService.Items.Add("FETCH");
            this.m_pService.SelectedIndex = 0;
            this.mt_Limit = new Label();
            this.mt_Limit.Size = new Size(60, 20);
            this.mt_Limit.Location = new Point(210, 45);
            this.mt_Limit.TextAlign = ContentAlignment.MiddleRight;
            this.mt_Limit.Text = "Limit To:";
            this.m_pLimit = new NumericUpDown();
            this.m_pLimit.Size = new Size(55, 20);
            this.m_pLimit.Location = new Point(275, 45);
            this.m_pLimit.Minimum = 0m;
            this.m_pLimit.Maximum = 99999m;
            this.m_pLimit.Value = 1000m;
            this.mt_Date = new Label();
            this.mt_Date.Size = new Size(80, 20);
            this.mt_Date.Location = new Point(0, 70);
            this.mt_Date.TextAlign = ContentAlignment.MiddleRight;
            this.mt_Date.Text = "Date:";
            this.m_pDate = new DateTimePicker();
            this.m_pDate.Size = new Size(120, 20);
            this.m_pDate.Location = new Point(85, 70);
            this.m_pDate.Format = DateTimePickerFormat.Short;
            this.m_pDate.ValueChanged += new EventHandler(this.m_pDate_ValueChanged);
            this.mt_Between = new Label();
            this.mt_Between.Size = new Size(70, 20);
            this.mt_Between.Location = new Point(200, 70);
            this.mt_Between.TextAlign = ContentAlignment.MiddleRight;
            this.mt_Between.Text = "Between:";
            this.m_pStartTime = new DateTimePicker();
            this.m_pStartTime.Size = new Size(55, 20);
            this.m_pStartTime.Location = new Point(275, 70);
            this.m_pStartTime.CustomFormat = "HH:mm";
            this.m_pStartTime.Format = DateTimePickerFormat.Custom;
            this.m_pStartTime.ShowUpDown = true;
            this.m_pStartTime.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0);
            this.m_pStartTime.ValueChanged += new EventHandler(this.m_pStartTime_ValueChanged);
            this.m_pEndTime = new DateTimePicker();
            this.m_pEndTime.Size = new Size(55, 20);
            this.m_pEndTime.Location = new Point(335, 70);
            this.m_pEndTime.CustomFormat = "HH:mm";
            this.m_pEndTime.Format = DateTimePickerFormat.Custom;
            this.m_pEndTime.ShowUpDown = true;
            this.m_pEndTime.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 23, 59, 59);
            this.m_pEndTime.ValueChanged += new EventHandler(this.m_pEndTime_ValueChanged);
            this.mt_ContainsText = new Label();
            this.mt_ContainsText.Size = new Size(80, 20);
            this.mt_ContainsText.Location = new Point(0, 95);
            this.mt_ContainsText.TextAlign = ContentAlignment.MiddleRight;
            this.mt_ContainsText.Text = "Contains Text:";
            this.m_pContainsText = new TextBox();
            this.m_pContainsText.Size = new Size(305, 20);
            this.m_pContainsText.Location = new Point(85, 95);
            this.m_pContainsText.TextChanged += new EventHandler(this.m_pContainsText_TextChanged);
            this.m_pLogSessions = new ListView();
            this.m_pLogSessions.Size = new Size(422, 135);
            this.m_pLogSessions.Location = new Point(10, 125);
            this.m_pLogSessions.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.m_pLogSessions.View = View.Details;
            this.m_pLogSessions.FullRowSelect = true;
            this.m_pLogSessions.HideSelection = false;
            this.m_pLogSessions.DoubleClick += new EventHandler(this.m_pLogSessions_DoubleClick);
            this.m_pLogSessions.Columns.Add("Start Time", 110, HorizontalAlignment.Left);
            this.m_pLogSessions.Columns.Add("User", 100, HorizontalAlignment.Left);
            this.m_pLogSessions.Columns.Add("Remote End Point", 150, HorizontalAlignment.Left);
            this.m_pLogSessions.Columns.Add("Session ID", 100, HorizontalAlignment.Left);
            base.Controls.Add(this.mt_VirtualServer);
            base.Controls.Add(this.m_pVirtualServer);
            base.Controls.Add(this.m_pGet);
            base.Controls.Add(this.mt_Service);
            base.Controls.Add(this.m_pService);
            base.Controls.Add(this.mt_Limit);
            base.Controls.Add(this.m_pLimit);
            base.Controls.Add(this.mt_Date);
            base.Controls.Add(this.mt_Date);
            base.Controls.Add(this.m_pDate);
            base.Controls.Add(this.mt_Between);
            base.Controls.Add(this.m_pStartTime);
            base.Controls.Add(this.m_pEndTime);
            base.Controls.Add(this.mt_ContainsText);
            base.Controls.Add(this.m_pContainsText);
            base.Controls.Add(this.m_pLogSessions);
        }

        private void m_pVirtualServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.m_pLogSessions != null)
            {
                this.m_pLogSessions.Items.Clear();
            }
            m_pGet_Click(null, null);
        }

        private void m_pGet_Click(object sender, EventArgs e)
        {
            if (this.m_pVirtualServer.Items.Count == 0)
            {
                return;
            }
            this.Cursor = Cursors.WaitCursor;
            this.m_pLogSessions.BeginUpdate();
            this.m_pLogSessions.Items.Clear();
            DateTime startTime = new DateTime(this.m_pDate.Value.Year, this.m_pDate.Value.Month, this.m_pDate.Value.Day, this.m_pStartTime.Value.Hour, this.m_pStartTime.Value.Minute, 0);
            DateTime endTime = new DateTime(this.m_pDate.Value.Year, this.m_pDate.Value.Month, this.m_pDate.Value.Day, this.m_pEndTime.Value.Hour, this.m_pEndTime.Value.Minute, 0);
            LogSession[] array = null;
            VirtualServer virtualServer = (VirtualServer)((WComboBoxItem)this.m_pVirtualServer.SelectedItem).Tag;
            if (this.m_pService.Text == "SMTP")
            {
                array = virtualServer.Logs.GetSmtpLogSessions((int)this.m_pLimit.Value, this.m_pDate.Value, startTime, endTime, this.m_pContainsText.Text);
            }
            else if (this.m_pService.Text == "POP3")
            {
                array = virtualServer.Logs.GetPop3LogSessions((int)this.m_pLimit.Value, this.m_pDate.Value, startTime, endTime, this.m_pContainsText.Text);
            }
            else if (this.m_pService.Text == "IMAP")
            {
                array = virtualServer.Logs.GetImapLogSessions((int)this.m_pLimit.Value, this.m_pDate.Value, startTime, endTime, this.m_pContainsText.Text);
            }
            else if (this.m_pService.Text == "RELAY")
            {
                array = virtualServer.Logs.GetRelayLogSessions((int)this.m_pLimit.Value, this.m_pDate.Value, startTime, endTime, this.m_pContainsText.Text);
            }
            else if (this.m_pService.Text == "FETCH")
            {
                array = virtualServer.Logs.GetFetchLogSessions((int)this.m_pLimit.Value, this.m_pDate.Value, startTime, endTime, this.m_pContainsText.Text);
            }
            LogSession[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                LogSession logSession = array2[i];
                ListViewItem listViewItem = new ListViewItem(logSession.StartTime.ToString());
                listViewItem.SubItems.Add(logSession.UserName);
                listViewItem.SubItems.Add(logSession.RemoteEndPoint.ToString());
                listViewItem.SubItems.Add(logSession.SessionID);
                listViewItem.Tag = logSession;
                this.m_pLogSessions.Items.Add(listViewItem);
            }
            this.m_pLogSessions.EndUpdate();
            this.Cursor = Cursors.Default;
        }

        private void m_pService_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.m_pLogSessions != null)
            {
                this.m_pLogSessions.Items.Clear();
            }
            m_pGet_Click(null, null);
        }

        private void m_pDate_ValueChanged(object sender, EventArgs e)
        {
            if (this.m_pLogSessions != null)
            {
                this.m_pLogSessions.Items.Clear();
            }
            m_pGet_Click(null, null);
        }

        private void m_pStartTime_ValueChanged(object sender, EventArgs e)
        {
            if (this.m_pLogSessions != null)
            {
                this.m_pLogSessions.Items.Clear();
            }
            m_pGet_Click(null, null);
        }

        private void m_pEndTime_ValueChanged(object sender, EventArgs e)
        {
            if (this.m_pLogSessions != null)
            {
                this.m_pLogSessions.Items.Clear();
            }
            m_pGet_Click(null, null);
        }
        public CancellationTokenSource tokenSource = new CancellationTokenSource();
        private void m_pContainsText_TextChanged(object sender, EventArgs e)
        {
            if (this.m_pLogSessions != null)
            {
                this.m_pLogSessions.Items.Clear();
            }
            if (!tokenSource.IsCancellationRequested)
            {
                tokenSource.Cancel();
                tokenSource = new CancellationTokenSource();
            }
            Task.Delay(600, tokenSource.Token).ContinueWith(x =>
            {
                if (!x.IsCanceled)
                {
                    this.Invoke(new Action(() => { m_pGet_Click(null, null); }));
                }
            });
        }

        private void m_pLogSessions_DoubleClick(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (this.m_pLogSessions.SelectedItems.Count > 0)
            {
                LogSession logSession = (LogSession)this.m_pLogSessions.SelectedItems[0].Tag;
                LogViewerForm logViewerForm = new LogViewerForm(logSession.LogText, this.m_pContainsText.Text);
                logViewerForm.ShowDialog(this);
            }
            this.Cursor = Cursors.Default;
        }

        private void LoadVirtualServers()
        {
            foreach (VirtualServer virtualServer in this.m_pServer.VirtualServers)
            {
                this.m_pVirtualServer.Items.Add(new WComboBoxItem(virtualServer.Name, virtualServer));
            }
        }
    }
}
