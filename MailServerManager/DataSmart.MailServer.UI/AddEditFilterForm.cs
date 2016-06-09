using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class AddEditFilterForm : Form
	{
		private PictureBox m_pIcon;

		private Label mt_Info;

		private GroupBox m_pSeparator1;

		private CheckBox m_pEnabled;

		private Label mt_Description;

		private TextBox m_pDescription;

		private Label mt_Assembly;

		private TextBox m_pAssembly;

		private Button m_pGetAssembly;

		private Label mt_Class;

		private TextBox m_pClass;

		private GroupBox m_pSeparator2;

		private Button m_Cancel;

		private Button m_pOk;

		private VirtualServer m_pVirtualServer;

		private Filter m_pFilter;

		public string FilterID
		{
			get
			{
				if (this.m_pFilter != null)
				{
					return this.m_pFilter.ID;
				}
				return "";
			}
		}

		public AddEditFilterForm(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
		}

		public AddEditFilterForm(VirtualServer virtualServer, Filter filter)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pFilter = filter;
			this.InitializeComponent();
			this.m_pEnabled.Checked = filter.Enabled;
			this.m_pDescription.Text = filter.Description;
			this.m_pAssembly.Text = filter.AssemblyName;
			this.m_pClass.Text = filter.Class;
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(392, 208);
			base.StartPosition = FormStartPosition.CenterScreen;
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			this.Text = "Add/Edit filter";
			base.Icon = ResManager.GetIcon("filter.ico");
			this.m_pIcon = new PictureBox();
			this.m_pIcon.Size = new Size(32, 32);
			this.m_pIcon.Location = new Point(10, 10);
			this.m_pIcon.Image = ResManager.GetIcon("filter32.ico").ToBitmap();
			this.mt_Info = new Label();
			this.mt_Info.Size = new Size(200, 32);
			this.mt_Info.Location = new Point(50, 10);
			this.mt_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Info.Text = "Specify filter information.";
			this.m_pSeparator1 = new GroupBox();
			this.m_pSeparator1.Size = new Size(383, 3);
			this.m_pSeparator1.Location = new Point(7, 50);
			this.m_pEnabled = new CheckBox();
			this.m_pEnabled.Size = new Size(100, 20);
			this.m_pEnabled.Location = new Point(105, 60);
			this.m_pEnabled.Text = "Enabled";
			this.m_pEnabled.Checked = true;
			this.mt_Description = new Label();
			this.mt_Description.Size = new Size(100, 20);
			this.mt_Description.Location = new Point(0, 85);
			this.mt_Description.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Description.Text = "Description:";
			this.m_pDescription = new TextBox();
			this.m_pDescription.Size = new Size(280, 20);
			this.m_pDescription.Location = new Point(105, 85);
			this.mt_Assembly = new Label();
			this.mt_Assembly.Size = new Size(100, 20);
			this.mt_Assembly.Location = new Point(0, 110);
			this.mt_Assembly.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Assembly.Text = "Assembly:";
			this.m_pAssembly = new TextBox();
			this.m_pAssembly.Size = new Size(250, 20);
			this.m_pAssembly.Location = new Point(105, 110);
			this.m_pAssembly.ReadOnly = true;
			this.m_pGetAssembly = new Button();
			this.m_pGetAssembly.Size = new Size(25, 20);
			this.m_pGetAssembly.Location = new Point(360, 110);
			this.m_pGetAssembly.Text = "...";
			this.m_pGetAssembly.Click += new EventHandler(this.m_pGetAssembly_Click);
			this.mt_Class = new Label();
			this.mt_Class.Size = new Size(100, 20);
			this.mt_Class.Location = new Point(0, 135);
			this.mt_Class.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Class.Text = "Class:";
			this.m_pClass = new TextBox();
			this.m_pClass.Size = new Size(280, 20);
			this.m_pClass.Location = new Point(105, 135);
			this.m_pClass.ReadOnly = true;
			this.m_pSeparator2 = new GroupBox();
			this.m_pSeparator2.Size = new Size(383, 3);
			this.m_pSeparator2.Location = new Point(7, 165);
			this.m_Cancel = new Button();
			this.m_Cancel.Size = new Size(70, 20);
			this.m_Cancel.Location = new Point(240, 180);
			this.m_Cancel.Text = "Cancel";
			this.m_Cancel.Click += new EventHandler(this.m_Cancel_Click);
			this.m_pOk = new Button();
			this.m_pOk.Size = new Size(70, 20);
			this.m_pOk.Location = new Point(315, 180);
			this.m_pOk.Text = "Ok";
			this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
			base.Controls.Add(this.m_pIcon);
			base.Controls.Add(this.mt_Info);
			base.Controls.Add(this.m_pSeparator1);
			base.Controls.Add(this.m_pEnabled);
			base.Controls.Add(this.mt_Description);
			base.Controls.Add(this.m_pDescription);
			base.Controls.Add(this.mt_Assembly);
			base.Controls.Add(this.m_pAssembly);
			base.Controls.Add(this.m_pGetAssembly);
			base.Controls.Add(this.mt_Class);
			base.Controls.Add(this.m_pClass);
			base.Controls.Add(this.m_pSeparator2);
			base.Controls.Add(this.m_Cancel);
			base.Controls.Add(this.m_pOk);
		}

		private void m_pGetAssembly_Click(object sender, EventArgs e)
		{
			SelectFilterForm selectFilterForm = new SelectFilterForm(this.m_pVirtualServer);
			if (selectFilterForm.ShowDialog() == DialogResult.OK)
			{
				this.m_pAssembly.Text = selectFilterForm.AssemblyName;
				this.m_pClass.Text = selectFilterForm.TypeName;
			}
		}

		private void m_Cancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
			base.Close();
		}

		private void m_pOk_Click(object sender, EventArgs e)
		{
			if (this.m_pFilter == null)
			{
				this.m_pFilter = this.m_pVirtualServer.Filters.Add(this.m_pEnabled.Checked, this.m_pDescription.Text, this.m_pAssembly.Text, this.m_pClass.Text);
			}
			else
			{
				this.m_pFilter.Enabled = this.m_pEnabled.Checked;
				this.m_pFilter.Description = this.m_pDescription.Text;
				this.m_pFilter.AssemblyName = this.m_pAssembly.Text;
				this.m_pFilter.Class = this.m_pClass.Text;
				this.m_pFilter.Commit();
			}
			base.DialogResult = DialogResult.OK;
		}
	}
}
