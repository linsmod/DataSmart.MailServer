using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.AccessManager
{
	public class UserForm : Form
	{
		private Label mt_UserName;

		private TextBox m_pUserName;

		private Label mt_Password;

		private TextBox m_pPassword;

		private GroupBox m_pGroupbox1;

		private Button m_pCancel;

		private Button m_pOk;

		private DataSet m_pDsSettings;

		private DataRow m_pDrUser;

		private bool m_Add_Edit = true;

		public UserForm(DataSet dsSettings)
		{
			this.m_pDsSettings = dsSettings;
			this.m_Add_Edit = true;
			this.InitializeComponent();
		}

		public UserForm(DataSet dsSettings, DataRow dr)
		{
			this.m_pDsSettings = dsSettings;
			this.m_pDrUser = dr;
			this.m_Add_Edit = false;
			this.InitializeComponent();
			this.m_pUserName.Text = dr["UserName"].ToString();
			this.m_pUserName.ReadOnly = true;
			this.m_pPassword.Text = dr["Password"].ToString();
		}

		private void InitializeComponent()
		{
			base.Size = new Size(330, 140);
			base.StartPosition = FormStartPosition.CenterScreen;
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			this.Text = "User settings:";
			this.mt_UserName = new Label();
			this.mt_UserName.Size = new Size(100, 18);
			this.mt_UserName.Location = new Point(5, 20);
			this.mt_UserName.TextAlign = ContentAlignment.MiddleRight;
			this.mt_UserName.Text = "User Name:";
			this.m_pUserName = new TextBox();
			this.m_pUserName.Size = new Size(200, 20);
			this.m_pUserName.Location = new Point(105, 20);
			this.mt_Password = new Label();
			this.mt_Password.Size = new Size(100, 18);
			this.mt_Password.Location = new Point(5, 45);
			this.mt_Password.TextAlign = ContentAlignment.MiddleRight;
			this.mt_Password.Text = "Password:";
			this.m_pPassword = new TextBox();
			this.m_pPassword.Size = new Size(200, 20);
			this.m_pPassword.Location = new Point(105, 45);
			this.m_pPassword.PasswordChar = '*';
			this.m_pGroupbox1 = new GroupBox();
			this.m_pGroupbox1.Size = new Size(325, 3);
			this.m_pGroupbox1.Location = new Point(3, 75);
			this.m_pCancel = new Button();
			this.m_pCancel.Size = new Size(70, 20);
			this.m_pCancel.Location = new Point(160, 85);
			this.m_pCancel.Text = "Cancel";
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			this.m_pOk = new Button();
			this.m_pOk.Size = new Size(70, 20);
			this.m_pOk.Location = new Point(235, 85);
			this.m_pOk.Text = "Ok";
			this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
			base.Controls.Add(this.mt_UserName);
			base.Controls.Add(this.m_pUserName);
			base.Controls.Add(this.mt_Password);
			base.Controls.Add(this.m_pPassword);
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
			if (this.m_pUserName.Text == "")
			{
				MessageBox.Show("User name cannot be empty !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (this.m_Add_Edit)
			{
				foreach (DataRow dataRow in this.m_pDsSettings.Tables["Users"].Rows)
				{
					if (dataRow["UserName"].ToString().ToLower() == this.m_pUserName.Text.ToLower())
					{
						MessageBox.Show("Specified user '" + this.m_pUserName.Text + "' already exists !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						return;
					}
				}
			}
			if (this.m_Add_Edit)
			{
				DataRow dataRow2 = this.m_pDsSettings.Tables["Users"].NewRow();
				dataRow2["UserName"] = this.m_pUserName.Text;
				dataRow2["Password"] = this.m_pPassword.Text;
				this.m_pDsSettings.Tables["Users"].Rows.Add(dataRow2);
			}
			else
			{
				this.m_pDrUser["Password"] = this.m_pPassword.Text;
			}
			base.DialogResult = DialogResult.OK;
			base.Close();
		}
	}
}
