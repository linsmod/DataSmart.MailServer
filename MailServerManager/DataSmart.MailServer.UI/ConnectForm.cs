using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.IO;
using System.Security;
using System.Text;
using System.Windows.Forms;
using DataSmart.MailServer.Extensions;
namespace DataSmart.MailServer.UI
{
    public class ConnectForm : Form
    {
        private PictureBox m_pIcon;

        private Label mt_Info;

        private GroupBox m_pSeparator1;

        private Label mt_Server;

        private ComboBox m_pServer;

        private Label mt_UserName;

        private TextBox m_pUserName;

        private Label mt_Password;

        private TextBox m_pPassword;

        private CheckBox m_pSaveConnection;

        private GroupBox m_pGroupbox1;

        private Button m_pCancel;

        private Button m_pOk;

        private Server m_pApiServer;

        public Server Server
        {
            get
            {
                return this.m_pApiServer;
            }
        }

        public string Host
        {
            get
            {
                return this.m_pServer.Text;
            }
        }

        public string UserName
        {
            get
            {
                return this.m_pUserName.Text;
            }
        }

        public string Password
        {
            get
            {
                return this.m_pPassword.Text;
            }
        }

        public bool SaveConnection
        {
            get
            {
                return this.m_pSaveConnection.Checked;
            }
        }

        public ConnectForm()
        {
            this.InitializeComponent();
            this.LoadRecentConnections();
        }

        public ConnectForm(string host, string userName, SecureString password, bool saveConnectionEnabled)
        {
            this.InitializeComponent();
            this.m_pServer.Text = host;
            this.m_pUserName.Text = userName;
            this.m_pPassword.Text = password.ConvertToUnsecureString();
            this.m_pSaveConnection.Enabled = saveConnectionEnabled;
            this.m_pApiServer = new Server(host, userName, password);
            this.LoadRecentConnections();
        }

        private void InitializeComponent()
        {
            base.ClientSize = new Size(392, 203);
            base.StartPosition = FormStartPosition.CenterScreen;
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            this.Text = "Connect to Server";
            this.m_pIcon = new PictureBox();
            this.m_pIcon.Size = new Size(32, 32);
            this.m_pIcon.Location = new Point(10, 10);
            this.m_pIcon.Image = ResManager.GetImage("icon-connect.png");
            this.mt_Info = new Label();
            this.mt_Info.Size = new Size(200, 32);
            this.mt_Info.Location = new Point(50, 10);
            this.mt_Info.TextAlign = ContentAlignment.MiddleLeft;
            this.mt_Info.Text = "Specify connection parameters.";
            this.m_pSeparator1 = new GroupBox();
            this.m_pSeparator1.Size = new Size(380, 3);
            this.m_pSeparator1.Location = new Point(7, 50);
            this.mt_Server = new Label();
            this.mt_Server.Size = new Size(100, 20);
            this.mt_Server.Location = new Point(5, 60);
            this.mt_Server.TextAlign = ContentAlignment.MiddleLeft;
            this.mt_Server.Text = "Server:";
            this.m_pServer = new ComboBox();
            this.m_pServer.Size = new Size(225, 20);
            this.m_pServer.Location = new Point(155, 60);
            this.mt_UserName = new Label();
            this.mt_UserName.Size = new Size(100, 20);
            this.mt_UserName.Location = new Point(5, 85);
            this.mt_UserName.TextAlign = ContentAlignment.MiddleLeft;
            this.mt_UserName.Text = "User Name:";
            this.m_pUserName = new TextBox();
            this.m_pUserName.Size = new Size(225, 20);
            this.m_pUserName.Location = new Point(155, 85);
            this.m_pUserName.Text = "Administrator";
            this.mt_Password = new Label();
            this.mt_Password.Size = new Size(100, 20);
            this.mt_Password.Location = new Point(5, 110);
            this.mt_Password.TextAlign = ContentAlignment.MiddleLeft;
            this.mt_Password.Text = "Password:";
            this.m_pPassword = new TextBox();
            this.m_pPassword.Size = new Size(225, 20);
            this.m_pPassword.Location = new Point(155, 110);
            this.m_pPassword.PasswordChar = '*';
            this.m_pSaveConnection = new CheckBox();
            this.m_pSaveConnection.Size = new Size(220, 20);
            this.m_pSaveConnection.Location = new Point(155, 135);
            this.m_pSaveConnection.Text = "Save Connection";
            this.m_pGroupbox1 = new GroupBox();
            this.m_pGroupbox1.Size = new Size(380, 3);
            this.m_pGroupbox1.Location = new Point(7, 160);
            this.m_pCancel = new Button();
            this.m_pCancel.Size = new Size(70, 20);
            this.m_pCancel.Location = new Point(235, 175);
            this.m_pCancel.Text = "Cancel";
            this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
            this.m_pOk = new Button();
            this.m_pOk.Size = new Size(70, 20);
            this.m_pOk.Location = new Point(310, 175);
            this.m_pOk.Text = "Ok";
            this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
            base.Controls.Add(this.m_pIcon);
            base.Controls.Add(this.mt_Info);
            base.Controls.Add(this.m_pSeparator1);
            base.Controls.Add(this.mt_Server);
            base.Controls.Add(this.m_pServer);
            base.Controls.Add(this.mt_UserName);
            base.Controls.Add(this.m_pUserName);
            base.Controls.Add(this.mt_Password);
            base.Controls.Add(this.m_pPassword);
            base.Controls.Add(this.m_pSaveConnection);
            base.Controls.Add(this.m_pGroupbox1);
            base.Controls.Add(this.m_pCancel);
            base.Controls.Add(this.m_pOk);
        }

        private void m_pCancel_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
            base.Close();
        }

        private void m_pOk_Click(object sender, EventArgs e)
        {
            try
            {
                string text = this.m_pServer.Text;
                if (text == "")
                {
                    text = "localhost";
                }
                Server server = new Server(text, this.m_pUserName.Text, this.m_pPassword.Text.ConvertToSecureString());
                server.Connect();
                server.connectForm = this;
                this.m_pApiServer = server;
                this.SaveRecentConnections();
                base.DialogResult = DialogResult.OK;
                base.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error connecting to server:\r\n\t" + ex.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void LoadRecentConnections()
        {
            if (File.Exists(Application.StartupPath + "/Settings/RecentConnections.txt"))
            {
                string[] array = File.ReadAllText(Application.StartupPath + "/Settings/RecentConnections.txt", Encoding.UTF8).Replace("\r\n", "\n").Split(new char[]
                {
                    '\n'
                });
                string[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    string text = array2[i];
                    if (text != "")
                    {
                        this.m_pServer.Items.Add(text);
                    }
                }
            }
        }

        private void SaveRecentConnections()
        {
            if (!this.m_pServer.Items.Contains(this.m_pServer.Text))
            {
                if (this.m_pServer.Items.Count > 25)
                {
                    this.m_pServer.Items.RemoveAt(this.m_pServer.Items.Count - 1);
                }
                this.m_pServer.Items.Insert(0, this.m_pServer.Text);
                string text = "";
                foreach (string serverInfo in this.m_pServer.Items)
                {
                    text = text + serverInfo + "\r\n";
                }
                string path = Application.StartupPath + "/Settings/RecentConnections.txt"; 
                string directoryName = Path.GetDirectoryName(path);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                File.WriteAllText(path, text, Encoding.UTF8);
            }
        }
    }
}
