using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class AddEditDomainForm : Form
	{
		private PictureBox m_pIcon;

		private Label mt_Info;

		private GroupBox m_pSeparator1;

		private Label mt_Domain;

		private TextBox m_pDomainName;

		private Label mt_Description;

		private TextBox m_pDescription;

		private GroupBox m_pSeparator2;

		private Button m_pCancel;

		private Button m_pOk;

		private VirtualServer m_pVirtualServer;

		private Domain m_pDomain;

		public string DomainID
		{
			get
			{
				if (this.m_pDomain != null)
				{
					return this.m_pDomain.DomainID;
				}
				return "";
			}
		}

		public AddEditDomainForm(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
		}

		public AddEditDomainForm(VirtualServer virtualServer, Domain domain)
		{
			this.m_pVirtualServer = virtualServer;
			this.m_pDomain = domain;
			this.InitializeComponent();
			this.m_pDomainName.Text = domain.DomainName;
			this.m_pDescription.Text = domain.Description;
			this.m_pDomainName.SelectionStart = 0;
			this.m_pDomainName.SelectionLength = 0;
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(392, 173);
			base.StartPosition = FormStartPosition.CenterScreen;
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			this.Text = "Add/Edit domain";
			this.m_pIcon = new PictureBox();
			this.m_pIcon.Size = new Size(32, 32);
			this.m_pIcon.Location = new Point(10, 10);
			this.m_pIcon.Image = ResManager.GetIcon("domain32.ico").ToBitmap();
			this.mt_Info = new Label();
			this.mt_Info.Size = new Size(200, 32);
			this.mt_Info.Location = new Point(50, 10);
			this.mt_Info.TextAlign = ContentAlignment.MiddleLeft;
			this.mt_Info.Text = "Specify domain information.";
			this.m_pSeparator1 = new GroupBox();
			this.m_pSeparator1.Size = new Size(383, 3);
			this.m_pSeparator1.Location = new Point(7, 50);
			this.mt_Domain = new Label();
			this.mt_Domain.Size = new Size(100, 20);
			this.mt_Domain.Location = new Point(0, 70);
			this.mt_Domain.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Domain.Text = "Domain Name:";
			this.m_pDomainName = new TextBox();
			this.m_pDomainName.Size = new Size(280, 20);
			this.m_pDomainName.Location = new Point(105, 70);
			this.mt_Description = new Label();
			this.mt_Description.Size = new Size(100, 20);
			this.mt_Description.Location = new Point(0, 95);
			this.mt_Description.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Description.Text = "Description:";
			this.m_pDescription = new TextBox();
			this.m_pDescription.Size = new Size(280, 20);
			this.m_pDescription.Location = new Point(105, 95);
			this.m_pSeparator2 = new GroupBox();
			this.m_pSeparator2.Size = new Size(383, 4);
			this.m_pSeparator2.Location = new Point(7, 130);
			this.m_pCancel = new Button();
			this.m_pCancel.Size = new Size(70, 20);
			this.m_pCancel.Location = new Point(240, 150);
			this.m_pCancel.Text = "Cancel";
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			this.m_pOk = new Button();
			this.m_pOk.Size = new Size(70, 20);
			this.m_pOk.Location = new Point(315, 150);
			this.m_pOk.Text = "Ok";
			this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
			base.Controls.Add(this.m_pIcon);
			base.Controls.Add(this.mt_Info);
			base.Controls.Add(this.m_pSeparator1);
			base.Controls.Add(this.mt_Domain);
			base.Controls.Add(this.m_pDomainName);
			base.Controls.Add(this.mt_Description);
			base.Controls.Add(this.m_pDescription);
			base.Controls.Add(this.m_pSeparator2);
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
				if (this.m_pDomainName.Text.Length <= 0)
				{
					MessageBox.Show("Domain name cannot be empty!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
				else
				{
					if (this.m_pDomain == null)
					{
						this.m_pDomain = this.m_pVirtualServer.Domains.Add(this.m_pDomainName.Text, this.m_pDescription.Text);
					}
					else
					{
						this.m_pDomain.DomainName = this.m_pDomainName.Text;
						this.m_pDomain.Description = this.m_pDescription.Text;
						this.m_pDomain.Commit();
					}
					base.DialogResult = DialogResult.OK;
					base.Close();
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
