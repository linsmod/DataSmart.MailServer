using DataSmart.MailServer.Management;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class SelectFilterForm : Form
	{
		private ListView m_pList;

		private GroupBox m_pGroupbox1;

		private Button m_pCancel;

		private Button m_pOk;

		private VirtualServer m_pVirtualServer;

		private string m_Assembly = "";

		private string m_Type = "";

		public string AssemblyName
		{
			get
			{
				return this.m_Assembly;
			}
		}

		public string TypeName
		{
			get
			{
				return this.m_Type;
			}
		}

		public SelectFilterForm(VirtualServer virtualServer)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			this.LoadFilters();
		}

		private void InitializeComponent()
		{
			base.ClientSize = new Size(392, 273);
			base.StartPosition = FormStartPosition.CenterScreen;
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			this.Text = "Select Filter";
			this.m_pList = new ListView();
			this.m_pList.Size = new Size(375, 220);
			this.m_pList.Location = new Point(10, 10);
			this.m_pList.View = View.Details;
			this.m_pList.FullRowSelect = true;
			this.m_pList.HideSelection = false;
			this.m_pList.DoubleClick += new EventHandler(this.m_pList_DoubleClick);
			this.m_pList.Columns.Add("Assembly", 150, HorizontalAlignment.Left);
			this.m_pList.Columns.Add("Type", 200, HorizontalAlignment.Left);
			this.m_pGroupbox1 = new GroupBox();
			this.m_pGroupbox1.Size = new Size(390, 3);
			this.m_pGroupbox1.Location = new Point(5, 240);
			this.m_pCancel = new Button();
			this.m_pCancel.Size = new Size(70, 20);
			this.m_pCancel.Location = new Point(245, 250);
			this.m_pCancel.Text = "Cancel";
			this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
			this.m_pOk = new Button();
			this.m_pOk.Size = new Size(70, 20);
			this.m_pOk.Location = new Point(320, 250);
			this.m_pOk.Text = "Ok";
			this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
			base.Controls.Add(this.m_pList);
			base.Controls.Add(this.m_pGroupbox1);
			base.Controls.Add(this.m_pCancel);
			base.Controls.Add(this.m_pOk);
		}

		private void m_pList_DoubleClick(object sender, EventArgs e)
		{
			this.m_pOk_Click(sender, e);
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
			base.Close();
		}

		private void m_pOk_Click(object sender, EventArgs e)
		{
			if (this.m_pList.SelectedItems.Count > 0)
			{
				this.m_Assembly = this.m_pList.SelectedItems[0].Text;
				this.m_Type = this.m_pList.SelectedItems[0].SubItems[1].Text;
			}
			base.DialogResult = DialogResult.OK;
			base.Close();
		}

		private void LoadFilters()
		{
			DataSet filterTypes = this.m_pVirtualServer.Filters.GetFilterTypes();
			if (filterTypes.Tables.Contains("Filters"))
			{
				foreach (DataRow dataRow in filterTypes.Tables["Filters"].Rows)
				{
					ListViewItem listViewItem = new ListViewItem(dataRow["AssemblyName"].ToString());
					listViewItem.SubItems.Add(dataRow["TypeName"].ToString());
					this.m_pList.Items.Add(listViewItem);
				}
			}
		}
	}
}
