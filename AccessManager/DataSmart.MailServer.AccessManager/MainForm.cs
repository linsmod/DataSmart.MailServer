using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DataSmart.MailServer.AccessManager
{
    public class MainForm : Form
    {
        private TabControl m_pTab;

        private Button m_pCancel;

        private Button m_pOk;

        private ListView m_pTab_IPAccess_List;

        private Button m_pTab_IPAccess_Add;

        private Button m_pTab_IPAccess_Edit;

        private Button m_pTab_IPAccess_Delete;

        private ListView m_pTab_Users_List;

        private Button m_pTab_Users_Add;

        private Button m_pTab_Users_Edit;

        private Button m_pTab_Users_Delete;

        private DataSet m_pDsSettings;

        public MainForm()
        {
            this.InitializeComponent();
            this.LoadSettings();
        }

        private void InitializeComponent()
        {
            base.ClientSize = new Size(400, 275);
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "DataSmart Mail Server Administration Access Configurator:";
            this.m_pTab = new TabControl();
            this.m_pTab.Size = new Size(392, 230);
            this.m_pTab.Location = new Point(5, 5);
            this.m_pTab.TabPages.Add(new TabPage("IP Access"));
            this.m_pTab.TabPages.Add(new TabPage("Users"));
            this.m_pCancel = new Button();
            this.m_pCancel.Size = new Size(70, 20);
            this.m_pCancel.Location = new Point(245, 245);
            this.m_pCancel.Text = "Cancel";
            this.m_pCancel.Click += new EventHandler(this.m_pCancel_Click);
            this.m_pOk = new Button();
            this.m_pOk.Size = new Size(70, 20);
            this.m_pOk.Location = new Point(320, 245);
            this.m_pOk.Text = "Ok";
            this.m_pOk.Click += new EventHandler(this.m_pOk_Click);
            base.Controls.Add(this.m_pTab);
            base.Controls.Add(this.m_pCancel);
            base.Controls.Add(this.m_pOk);
            this.m_pTab_IPAccess_List = new ListView();
            this.m_pTab_IPAccess_List.Size = new Size(300, 150);
            this.m_pTab_IPAccess_List.Location = new Point(5, 30);
            this.m_pTab_IPAccess_List.View = View.Details;
            this.m_pTab_IPAccess_List.HideSelection = false;
            this.m_pTab_IPAccess_List.FullRowSelect = true;
            this.m_pTab_IPAccess_List.SelectedIndexChanged += new EventHandler(this.m_pTab_IPAccess_List_SelectedIndexChanged);
            this.m_pTab_IPAccess_List.Columns.Add("Start IP", 145, HorizontalAlignment.Center);
            this.m_pTab_IPAccess_List.Columns.Add("End IP", 145, HorizontalAlignment.Center);
            this.m_pTab_IPAccess_Add = new Button();
            this.m_pTab_IPAccess_Add.Size = new Size(70, 20);
            this.m_pTab_IPAccess_Add.Location = new Point(310, 30);
            this.m_pTab_IPAccess_Add.Text = "Add";
            this.m_pTab_IPAccess_Add.Click += new EventHandler(this.m_pTab_IPAccess_Add_Click);
            this.m_pTab_IPAccess_Edit = new Button();
            this.m_pTab_IPAccess_Edit.Size = new Size(70, 20);
            this.m_pTab_IPAccess_Edit.Location = new Point(310, 55);
            this.m_pTab_IPAccess_Edit.Text = "Edit";
            this.m_pTab_IPAccess_Edit.Click += new EventHandler(this.m_pTab_IPAccess_Edit_Click);
            this.m_pTab_IPAccess_Delete = new Button();
            this.m_pTab_IPAccess_Delete.Size = new Size(70, 20);
            this.m_pTab_IPAccess_Delete.Location = new Point(310, 80);
            this.m_pTab_IPAccess_Delete.Text = "Delete";
            this.m_pTab_IPAccess_Delete.Click += new EventHandler(this.m_pTab_IPAccess_Delete_Click);
            this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_IPAccess_List);
            this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_IPAccess_Add);
            this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_IPAccess_Edit);
            this.m_pTab.TabPages[0].Controls.Add(this.m_pTab_IPAccess_Delete);
            this.m_pTab_Users_List = new ListView();
            this.m_pTab_Users_List.Size = new Size(300, 150);
            this.m_pTab_Users_List.Location = new Point(5, 30);
            this.m_pTab_Users_List.View = View.Details;
            this.m_pTab_Users_List.HideSelection = false;
            this.m_pTab_Users_List.FullRowSelect = true;
            this.m_pTab_Users_List.SelectedIndexChanged += new EventHandler(this.m_pTab_Users_List_SelectedIndexChanged);
            this.m_pTab_Users_List.Columns.Add("User Name", 250, HorizontalAlignment.Center);
            this.m_pTab_Users_Add = new Button();
            this.m_pTab_Users_Add.Size = new Size(70, 20);
            this.m_pTab_Users_Add.Location = new Point(310, 30);
            this.m_pTab_Users_Add.Text = "Add";
            this.m_pTab_Users_Add.Click += new EventHandler(this.m_pTab_Users_Add_Click);
            this.m_pTab_Users_Edit = new Button();
            this.m_pTab_Users_Edit.Size = new Size(70, 20);
            this.m_pTab_Users_Edit.Location = new Point(310, 55);
            this.m_pTab_Users_Edit.Text = "Edit";
            this.m_pTab_Users_Edit.Click += new EventHandler(this.m_pTab_Users_Edit_Click);
            this.m_pTab_Users_Delete = new Button();
            this.m_pTab_Users_Delete.Size = new Size(70, 20);
            this.m_pTab_Users_Delete.Location = new Point(310, 80);
            this.m_pTab_Users_Delete.Text = "Delete";
            this.m_pTab_Users_Delete.Click += new EventHandler(this.m_pTab_Users_Delete_Click);
            this.m_pTab.TabPages[1].Controls.Add(this.m_pTab_Users_List);
            this.m_pTab.TabPages[1].Controls.Add(this.m_pTab_Users_Add);
            this.m_pTab.TabPages[1].Controls.Add(this.m_pTab_Users_Edit);
            this.m_pTab.TabPages[1].Controls.Add(this.m_pTab_Users_Delete);
        }

        private void m_pTab_IPAccess_List_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.m_pTab_IPAccess_List.SelectedItems.Count == 0)
            {
                this.m_pTab_IPAccess_Edit.Enabled = false;
                this.m_pTab_IPAccess_Delete.Enabled = false;
                return;
            }
            this.m_pTab_IPAccess_Edit.Enabled = true;
            this.m_pTab_IPAccess_Delete.Enabled = true;
        }

        private void m_pTab_IPAccess_Add_Click(object sender, EventArgs e)
        {
            IPAccessForm iPAccessForm = new IPAccessForm(this.m_pDsSettings);
            if (iPAccessForm.ShowDialog(this) == DialogResult.OK)
            {
                this.m_pTab_IPAccess_List.Items.Clear();
                foreach (DataRow dataRow in this.m_pDsSettings.Tables["IP_Access"].Rows)
                {
                    ListViewItem listViewItem = new ListViewItem(dataRow["StartIP"].ToString());
                    listViewItem.SubItems.Add(dataRow["EndIP"].ToString());
                    listViewItem.Tag = dataRow;
                    this.m_pTab_IPAccess_List.Items.Add(listViewItem);
                }
            }
        }

        private void m_pTab_IPAccess_Edit_Click(object sender, EventArgs e)
        {
            if (this.m_pTab_IPAccess_List.SelectedItems[0].Text.ToLower() == "127.0.0.1" && this.m_pTab_IPAccess_List.SelectedItems[0].SubItems[0].Text == "127.0.0.1")
            {
                MessageBox.Show("IP range 127.0.0.1 - 127.0.0.1 is permanent system entry and cannot be modified !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            IPAccessForm iPAccessForm = new IPAccessForm(this.m_pDsSettings, (DataRow)this.m_pTab_IPAccess_List.SelectedItems[0].Tag);
            if (iPAccessForm.ShowDialog(this) == DialogResult.OK)
            {
                this.m_pTab_IPAccess_List.Items.Clear();
                foreach (DataRow dataRow in this.m_pDsSettings.Tables["IP_Access"].Rows)
                {
                    ListViewItem listViewItem = new ListViewItem(dataRow["StartIP"].ToString());
                    listViewItem.SubItems.Add(dataRow["EndIP"].ToString());
                    listViewItem.Tag = dataRow;
                    this.m_pTab_IPAccess_List.Items.Add(listViewItem);
                }
            }
        }

        private void m_pTab_IPAccess_Delete_Click(object sender, EventArgs e)
        {
            if (this.m_pTab_IPAccess_List.SelectedItems[0].Text.ToLower() == "127.0.0.1")
            {
                MessageBox.Show("IP range 127.0.0.1 - 127.0.0.1 is permanent system entry and cannot be deleted !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete entry '" + this.m_pTab_IPAccess_List.SelectedItems[0].Text + "' !", "Confirm:", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                ((DataRow)this.m_pTab_IPAccess_List.SelectedItems[0].Tag).Delete();
                this.m_pTab_IPAccess_List.SelectedItems[0].Remove();
            }
        }

        private void m_pTab_Users_List_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.m_pTab_Users_List.SelectedItems.Count == 0)
            {
                this.m_pTab_Users_Edit.Enabled = false;
                this.m_pTab_Users_Delete.Enabled = false;
                return;
            }
            this.m_pTab_Users_Edit.Enabled = true;
            this.m_pTab_Users_Delete.Enabled = true;
        }

        private void m_pTab_Users_Add_Click(object sender, EventArgs e)
        {
            UserForm userForm = new UserForm(this.m_pDsSettings);
            if (userForm.ShowDialog(this) == DialogResult.OK)
            {
                this.m_pTab_Users_List.Items.Clear();
                foreach (DataRow dataRow in this.m_pDsSettings.Tables["Users"].Rows)
                {
                    ListViewItem listViewItem = new ListViewItem(dataRow["UserName"].ToString());
                    listViewItem.Tag = dataRow;
                    this.m_pTab_Users_List.Items.Add(listViewItem);
                }
            }
        }

        private void m_pTab_Users_Edit_Click(object sender, EventArgs e)
        {
            UserForm userForm = new UserForm(this.m_pDsSettings, (DataRow)this.m_pTab_Users_List.SelectedItems[0].Tag);
            userForm.ShowDialog(this);
        }

        private void m_pTab_Users_Delete_Click(object sender, EventArgs e)
        {
            if (this.m_pTab_Users_List.SelectedItems[0].Text.ToLower() == "administrator")
            {
                MessageBox.Show("User Administrator is permanent user and cannot be deleted !", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete user '" + this.m_pTab_Users_List.SelectedItems[0].Text + "' !", "Confirm:", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                ((DataRow)this.m_pTab_Users_List.SelectedItems[0].Tag).Delete();
                this.m_pTab_Users_List.SelectedItems[0].Remove();
            }
        }

        private void m_pCancel_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
            base.Close();
        }

        private void m_pOk_Click(object sender, EventArgs e)
        {
            if (this.m_pDsSettings.HasChanges())
            {
                var path = Path.Combine(Application.StartupPath, "Settings");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = Path.Combine(path, "AdminAccess.xml");
                this.m_pDsSettings.WriteXml(path);
            }
            base.DialogResult = DialogResult.OK;
            base.Close();
        }

        private void LoadSettings()
        {
            this.m_pDsSettings = new DataSet();
            this.m_pDsSettings.Tables.Add("IP_Access");
            this.m_pDsSettings.Tables["IP_Access"].Columns.Add("StartIP");
            this.m_pDsSettings.Tables["IP_Access"].Columns.Add("EndIP");
            this.m_pDsSettings.Tables.Add("Users");
            this.m_pDsSettings.Tables["Users"].Columns.Add("UserName");
            this.m_pDsSettings.Tables["Users"].Columns.Add("Password");
            if (File.Exists(Application.StartupPath + "/Settings/AdminAccess.xml"))
            {
                this.m_pDsSettings.ReadXml(Application.StartupPath + "/Settings/AdminAccess.xml");
            }
            else
            {
                DataRow dataRow = this.m_pDsSettings.Tables["IP_Access"].NewRow();
                dataRow["StartIP"] = "127.0.0.1";
                dataRow["EndIP"] = "127.0.0.1";
                this.m_pDsSettings.Tables["IP_Access"].Rows.Add(dataRow);
                dataRow = this.m_pDsSettings.Tables["Users"].NewRow();
                dataRow["UserName"] = "Administrator";
                dataRow["Password"] = "";
                this.m_pDsSettings.Tables["Users"].Rows.Add(dataRow);
            }
            foreach (DataRow dataRow2 in this.m_pDsSettings.Tables["IP_Access"].Rows)
            {
                ListViewItem listViewItem = new ListViewItem(dataRow2["StartIP"].ToString());
                listViewItem.SubItems.Add(dataRow2["EndIP"].ToString());
                listViewItem.Tag = dataRow2;
                this.m_pTab_IPAccess_List.Items.Add(listViewItem);
            }
            foreach (DataRow dataRow3 in this.m_pDsSettings.Tables["Users"].Rows)
            {
                ListViewItem listViewItem2 = new ListViewItem(dataRow3["UserName"].ToString());
                listViewItem2.Tag = dataRow3;
                this.m_pTab_Users_List.Items.Add(listViewItem2);
            }
            this.m_pTab_IPAccess_List_SelectedIndexChanged(this, null);
            this.m_pTab_Users_List_SelectedIndexChanged(this, null);
        }
    }
}
