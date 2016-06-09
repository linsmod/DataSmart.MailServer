using DataSmart.MailServer.Management;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class AddEditVirtualServerForm : Form
	{
		private class XML_InitString
		{
			private string m_SettingsPath = "";

			private string m_MailStorePath = "";

			[Description("Settings location.")]
			public string SettingsPath
			{
				get
				{
					return this.m_SettingsPath;
				}
				set
				{
					this.m_SettingsPath = value;
				}
			}

			[Description("Mail store location.")]
			public string MailStorePath
			{
				get
				{
					return this.m_MailStorePath;
				}
				set
				{
					this.m_MailStorePath = value;
				}
			}

			public XML_InitString()
			{
				Guid guid = Guid.NewGuid();
				this.m_SettingsPath = "Virtural Servers\\" + guid + "\\Settings\\";
				this.m_MailStorePath = "Virtural Servers\\" + guid + "\\MailStore\\";
			}

			public XML_InitString(string initString)
			{
				string[] array = initString.Replace("\r\n", "\n").Split(new char[]
				{
					'\n'
				});
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text = array2[i];
					if (text.ToLower().IndexOf("datapath=") > -1)
					{
						this.m_SettingsPath = text.Substring(9);
					}
					else if (text.ToLower().IndexOf("mailstorepath=") > -1)
					{
						this.m_MailStorePath = text.Substring(14);
					}
				}
			}

			public override string ToString()
			{
				return "datapath=" + this.m_SettingsPath + "\r\nmailstorepath=" + this.m_MailStorePath;
			}
		}

		private class MSSQL_InitString
		{
			private string m_SqlConStr = "";

			private string m_MailStorePath = "";

			[Description("SQL connection string.")]
			public string SqlConnectionString
			{
				get
				{
					return this.m_SqlConStr;
				}
				set
				{
					this.m_SqlConStr = value;
				}
			}

			[Description("Mail store location.")]
			public string MailStorePath
			{
				get
				{
					return this.m_MailStorePath;
				}
				set
				{
					this.m_MailStorePath = value;
				}
			}

			public MSSQL_InitString()
			{
				Guid guid = Guid.NewGuid();
				this.m_SqlConStr = "server=localhost;uid=sa;pwd=;database=lsMailServer";
				this.m_MailStorePath = "Virtural Servers\\" + guid + "\\MailStore\\";
			}

			public MSSQL_InitString(string initString)
			{
				string[] array = initString.Replace("\r\n", "\n").Split(new char[]
				{
					'\n'
				});
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text = array2[i];
					if (text.ToLower().IndexOf("connectionstring=") > -1)
					{
						this.m_SqlConStr = text.Substring(17);
					}
					else if (text.ToLower().IndexOf("mailstorepath=") > -1)
					{
						this.m_MailStorePath = text.Substring(14);
					}
				}
			}

			public override string ToString()
			{
				return "connectionstring=" + this.m_SqlConStr + "\r\nmailstorepath=" + this.m_MailStorePath;
			}
		}

		private class PGSQL_InitString
		{
			private string m_SqlConStr = "";

			private string m_MailStorePath = "";

			[Description("SQL connection string.")]
			public string SqlConnectionString
			{
				get
				{
					return this.m_SqlConStr;
				}
				set
				{
					this.m_SqlConStr = value;
				}
			}

			[Description("Mail store location.")]
			public string MailStorePath
			{
				get
				{
					return this.m_MailStorePath;
				}
				set
				{
					this.m_MailStorePath = value;
				}
			}

			public PGSQL_InitString()
			{
				this.m_SqlConStr = "Server=127.0.0.1;User Id=user;Password=;Database=lsMailServer;";
			}

			public PGSQL_InitString(string initString)
			{
				string[] array = initString.Replace("\r\n", "\n").Split(new char[]
				{
					'\n'
				});
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text = array2[i];
					if (text.ToLower().IndexOf("connectionstring=") > -1)
					{
						this.m_SqlConStr = text.Substring(17);
					}
					else if (text.ToLower().IndexOf("mailstorepath=") > -1)
					{
						this.m_MailStorePath = text.Substring(14);
					}
				}
			}

			public override string ToString()
			{
				return "connectionstring=" + this.m_SqlConStr + "\r\nmailstorepath=" + this.m_MailStorePath;
			}
		}

		private CheckBox m_pEnabled;

		private Label mt_Name;

		private TextBox m_pName;

		private Label mt_Assembly;

		private TextBox m_pAssembly;

		private Button m_pGetAPI;

		private Label mt_Type;

		private TextBox m_pType;

		private GroupBox m_pGroupbox1;

		private PropertyGrid m_pPropertyGrid;

		private Button m_pCancel;

		private Button m_pOk;

		private Server m_pServer;

		private VirtualServer m_pVirtualServer;

		public AddEditVirtualServerForm(Server server)
		{
			this.m_pServer = server;
			this.InitializeComponent();
		}

		public AddEditVirtualServerForm(Server server, VirtualServer virtualServer)
		{
			this.m_pServer = server;
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			this.m_pGetAPI.Enabled = false;
			this.m_pEnabled.Checked = virtualServer.Enabled;
			this.m_pName.Text = virtualServer.Name;
			this.m_pAssembly.Text = virtualServer.AssemblyName;
			this.m_pType.Text = virtualServer.TypeName;
			if (this.m_pAssembly.Text == "MailServer.LocalXmlStorage.dll")
			{
				this.m_pPropertyGrid.SelectedObject = new AddEditVirtualServerForm.XML_InitString(virtualServer.InitString);
				return;
			}
			if (this.m_pAssembly.Text == "MailServer.SqlServerStorage.dll")
			{
				this.m_pPropertyGrid.SelectedObject = new AddEditVirtualServerForm.MSSQL_InitString(virtualServer.InitString);
				return;
			}
			MessageBox.Show("Unsupported Storage Type");
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(442, 373);
			base.StartPosition = FormStartPosition.CenterScreen;
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			this.Text = "Add/Edit virtual server";
			this.m_pEnabled = new CheckBox();
			this.m_pEnabled.Size = new Size(280, 20);
			this.m_pEnabled.Location = new Point(105, 15);
			this.m_pEnabled.Text = "Enabled";
			this.m_pEnabled.Checked = true;
			this.mt_Name = new Label();
			this.mt_Name.Size = new Size(100, 20);
			this.mt_Name.Location = new Point(0, 45);
			this.mt_Name.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Name.Text = "Name:";
			this.m_pName = new TextBox();
			this.m_pName.Size = new Size(330, 20);
			this.m_pName.Location = new Point(105, 45);
			this.mt_Assembly = new Label();
			this.mt_Assembly.Size = new Size(100, 20);
			this.mt_Assembly.Location = new Point(0, 70);
			this.mt_Assembly.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Assembly.Text = "Storage Provider:";
			this.m_pAssembly = new TextBox();
			this.m_pAssembly.Size = new Size(300, 20);
			this.m_pAssembly.Location = new Point(105, 70);
			this.m_pAssembly.ReadOnly = true;
			this.m_pGetAPI = new Button();
			this.m_pGetAPI.Size = new Size(25, 20);
			this.m_pGetAPI.Location = new Point(410, 70);
			this.m_pGetAPI.Text = "...";
			this.m_pGetAPI.Click += new EventHandler(this.m_pGetAPI_Click);
			this.mt_Type = new Label();
			this.mt_Type.Size = new Size(100, 20);
			this.mt_Type.Location = new Point(0, 95);
			this.mt_Type.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Type.Text = "Type:";
			this.m_pType = new TextBox();
			this.m_pType.Size = new Size(330, 20);
			this.m_pType.Location = new Point(105, 95);
			this.m_pType.ReadOnly = true;
			this.m_pGroupbox1 = new GroupBox();
			this.m_pGroupbox1.Size = new Size(435, 3);
			this.m_pGroupbox1.Location = new Point(9, 130);
			this.m_pPropertyGrid = new PropertyGrid();
			this.m_pPropertyGrid.Size = new Size(425, 200);
			this.m_pPropertyGrid.Location = new Point(10, 135);
			this.m_pCancel = new Button();
			this.m_pCancel.Size = new Size(70, 20);
			this.m_pCancel.Location = new Point(290, 350);
			this.m_pCancel.Text = "Cancel";
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			this.m_pOk = new Button();
			this.m_pOk.Size = new Size(70, 20);
			this.m_pOk.Location = new Point(365, 350);
			this.m_pOk.Text = "Ok";
			this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
			base.Controls.Add(this.m_pEnabled);
			base.Controls.Add(this.mt_Name);
			base.Controls.Add(this.m_pName);
			base.Controls.Add(this.mt_Assembly);
			base.Controls.Add(this.m_pAssembly);
			base.Controls.Add(this.m_pGetAPI);
			base.Controls.Add(this.mt_Type);
			base.Controls.Add(this.m_pType);
			base.Controls.Add(this.m_pGroupbox1);
			base.Controls.Add(this.m_pPropertyGrid);
			base.Controls.Add(this.m_pCancel);
			base.Controls.Add(this.m_pOk);
		}

		private void m_pGetAPI_Click(object sender, EventArgs e)
		{
			SelectVirtualServerApiForm selectVirtualServerApiForm = new SelectVirtualServerApiForm(this.m_pServer);
			if (selectVirtualServerApiForm.ShowDialog(this) == DialogResult.OK)
			{
				this.m_pAssembly.Text = selectVirtualServerApiForm.AssemblyName;
				this.m_pType.Text = selectVirtualServerApiForm.TypeName;
				if (this.m_pAssembly.Text == "MailServer.LocalXmlStorage.dll")
				{
					this.m_pPropertyGrid.SelectedObject = new AddEditVirtualServerForm.XML_InitString();
					return;
				}
				if (this.m_pAssembly.Text == "MailServer.SqlServerStorage.dll")
				{
					this.m_pPropertyGrid.SelectedObject = new AddEditVirtualServerForm.MSSQL_InitString();
					return;
				}
				MessageBox.Show("Unsupported Storage Type");
			}
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
			base.Close();
		}

		private void m_pOk_Click(object sender, EventArgs e)
		{
			if (this.m_pName.Text == "")
			{
				MessageBox.Show("Virtual server name cannot be empty !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (this.m_pAssembly.Text == "")
			{
				MessageBox.Show("Please select storage provider!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			this.Cursor = Cursors.WaitCursor;
			if (this.m_pVirtualServer == null)
			{
				this.m_pServer.VirtualServers.Add(this.m_pEnabled.Checked, this.m_pName.Text, this.m_pAssembly.Text, this.m_pType.Text, this.m_pPropertyGrid.SelectedObject.ToString());
			}
			else
			{
				this.m_pVirtualServer.Enabled = this.m_pEnabled.Checked;
				this.m_pVirtualServer.Name = this.m_pName.Text;
				this.m_pVirtualServer.InitString = this.m_pPropertyGrid.SelectedObject.ToString();
				this.m_pVirtualServer.Commit();
			}
			this.Cursor = Cursors.Default;
			base.DialogResult = DialogResult.OK;
			base.Close();
		}
	}
}
