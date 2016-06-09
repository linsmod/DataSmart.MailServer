using System;
using System.Data;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace DataSmart.MailServer.AccessManager
{
	public class IPAccessForm : Form
	{
		private Label mt_StartIP;

		private TextBox m_pStartIP;

		private Label mt_EndIP;

		private TextBox m_pEndIP;

		private GroupBox m_pGroupbox1;

		private Button m_pCancel;

		private Button m_pOk;

		private DataSet m_pDsSettings;

		private DataRow m_pDrIPAccess;

		private bool m_Add_Edit = true;

		public IPAccessForm(DataSet dsSettings)
		{
			this.m_pDsSettings = dsSettings;
			this.m_Add_Edit = true;
			this.InitializeComponent();
		}

		public IPAccessForm(DataSet dsSettings, DataRow dr)
		{
			this.m_pDsSettings = dsSettings;
			this.m_pDrIPAccess = dr;
			this.m_Add_Edit = false;
			this.InitializeComponent();
			this.m_pStartIP.Text = dr["StartIP"].ToString();
			this.m_pEndIP.Text = dr["EndIP"].ToString();
		}

		private void InitializeComponent()
		{
			base.Size = new Size(330, 140);
			base.StartPosition = FormStartPosition.CenterScreen;
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			this.Text = "IP Access settings:";
			this.mt_StartIP = new Label();
			this.mt_StartIP.Size = new Size(100, 18);
			this.mt_StartIP.Location = new Point(5, 20);
			this.mt_StartIP.TextAlign = ContentAlignment.MiddleRight;
			this.mt_StartIP.Text = "Start IP:";
			this.m_pStartIP = new TextBox();
			this.m_pStartIP.Size = new Size(200, 20);
			this.m_pStartIP.Location = new Point(105, 20);
			this.mt_EndIP = new Label();
			this.mt_EndIP.Size = new Size(100, 18);
			this.mt_EndIP.Location = new Point(5, 45);
			this.mt_EndIP.TextAlign = ContentAlignment.MiddleRight;
			this.mt_EndIP.Text = "End IP:";
			this.m_pEndIP = new TextBox();
			this.m_pEndIP.Size = new Size(200, 20);
			this.m_pEndIP.Location = new Point(105, 45);
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
			base.Controls.Add(this.mt_StartIP);
			base.Controls.Add(this.m_pStartIP);
			base.Controls.Add(this.mt_EndIP);
			base.Controls.Add(this.m_pEndIP);
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
			string text = this.m_pStartIP.Text;
			string text2 = this.m_pEndIP.Text;
			if (text2 == "")
			{
				text2 = text;
			}
			try
			{
				IPAddress.Parse(text);
			}
			catch
			{
				MessageBox.Show("Invalid start IP value !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			try
			{
				IPAddress.Parse(text2);
			}
			catch
			{
				MessageBox.Show("Invalid end IP value !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (this.m_Add_Edit)
			{
				foreach (DataRow dataRow in this.m_pDsSettings.Tables["IP_Access"].Rows)
				{
					if (dataRow["StartIP"].ToString() == text && dataRow["EndIP"].ToString() == text2)
					{
						MessageBox.Show("Specified IP range already exists !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						return;
					}
				}
			}
			if (this.m_Add_Edit)
			{
				DataRow dataRow2 = this.m_pDsSettings.Tables["IP_Access"].NewRow();
				dataRow2["StartIP"] = this.m_pStartIP.Text;
				dataRow2["EndIP"] = this.m_pEndIP.Text;
				this.m_pDsSettings.Tables["IP_Access"].Rows.Add(dataRow2);
			}
			else
			{
				this.m_pDrIPAccess["StartIP"] = this.m_pStartIP.Text;
				this.m_pDrIPAccess["EndIP"] = this.m_pEndIP.Text;
			}
			base.DialogResult = DialogResult.OK;
			base.Close();
		}
	}
}
