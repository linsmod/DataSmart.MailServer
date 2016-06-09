using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace DataSmart.MailServer.Filters
{
	public class MainForm : Form
	{
		private Label mt_ScanProgram;

		private ComboBox m_pScanProgram;

		private Button m_pGetScanProgram;

		private Label mt_ScanProgramArgs;

		private TextBox m_pScanProgramArgs;

		private Label mt_VirusIfExitCode;

		private NumericUpDown m_pVirusIfExitCode;

		private GroupBox m_pGroupBox1;

		private Button m_pCancel;

		private Button m_pOk;

		private DataSet m_pDsSettings;

		public MainForm()
		{
			this.InitializeComponent();
			this.LoadSettings();
		}

		private void InitializeComponent()
		{
            this.mt_ScanProgram = new System.Windows.Forms.Label();
            this.m_pScanProgram = new System.Windows.Forms.ComboBox();
            this.m_pGetScanProgram = new System.Windows.Forms.Button();
            this.mt_ScanProgramArgs = new System.Windows.Forms.Label();
            this.m_pScanProgramArgs = new System.Windows.Forms.TextBox();
            this.mt_VirusIfExitCode = new System.Windows.Forms.Label();
            this.m_pVirusIfExitCode = new System.Windows.Forms.NumericUpDown();
            this.m_pGroupBox1 = new System.Windows.Forms.GroupBox();
            this.m_pCancel = new System.Windows.Forms.Button();
            this.m_pOk = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.m_pVirusIfExitCode)).BeginInit();
            this.SuspendLayout();
            // 
            // mt_ScanProgram
            // 
            this.mt_ScanProgram.Location = new System.Drawing.Point(10, 10);
            this.mt_ScanProgram.Name = "mt_ScanProgram";
            this.mt_ScanProgram.Size = new System.Drawing.Size(400, 20);
            this.mt_ScanProgram.TabIndex = 0;
            this.mt_ScanProgram.Text = "Scan program:";
            // 
            // m_pScanProgram
            // 
            this.m_pScanProgram.Items.AddRange(new object[] {
            "C:\\Program Files\\ClamWin\\bin\\clamscan.exe"});
            this.m_pScanProgram.Location = new System.Drawing.Point(10, 30);
            this.m_pScanProgram.Name = "m_pScanProgram";
            this.m_pScanProgram.Size = new System.Drawing.Size(400, 21);
            this.m_pScanProgram.TabIndex = 1;
            this.m_pScanProgram.SelectedIndexChanged += new System.EventHandler(this.m_pScanProgram_SelectedIndexChanged);
            // 
            // m_pGetScanProgram
            // 
            this.m_pGetScanProgram.Location = new System.Drawing.Point(415, 30);
            this.m_pGetScanProgram.Name = "m_pGetScanProgram";
            this.m_pGetScanProgram.Size = new System.Drawing.Size(25, 20);
            this.m_pGetScanProgram.TabIndex = 2;
            this.m_pGetScanProgram.Text = "...";
            this.m_pGetScanProgram.Click += new System.EventHandler(this.m_pGetScanProgram_Click);
            // 
            // mt_ScanProgramArgs
            // 
            this.mt_ScanProgramArgs.Location = new System.Drawing.Point(10, 60);
            this.mt_ScanProgramArgs.Name = "mt_ScanProgramArgs";
            this.mt_ScanProgramArgs.Size = new System.Drawing.Size(400, 20);
            this.mt_ScanProgramArgs.TabIndex = 3;
            this.mt_ScanProgramArgs.Text = "Scan program command line arguments:";
            // 
            // m_pScanProgramArgs
            // 
            this.m_pScanProgramArgs.Location = new System.Drawing.Point(10, 80);
            this.m_pScanProgramArgs.Name = "m_pScanProgramArgs";
            this.m_pScanProgramArgs.Size = new System.Drawing.Size(400, 20);
            this.m_pScanProgramArgs.TabIndex = 4;
            // 
            // mt_VirusIfExitCode
            // 
            this.mt_VirusIfExitCode.Location = new System.Drawing.Point(10, 109);
            this.mt_VirusIfExitCode.Name = "mt_VirusIfExitCode";
            this.mt_VirusIfExitCode.Size = new System.Drawing.Size(136, 20);
            this.mt_VirusIfExitCode.TabIndex = 5;
            this.mt_VirusIfExitCode.Text = "Is virus if exit code equals:";
            this.mt_VirusIfExitCode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_pVirusIfExitCode
            // 
            this.m_pVirusIfExitCode.Location = new System.Drawing.Point(193, 111);
            this.m_pVirusIfExitCode.Name = "m_pVirusIfExitCode";
            this.m_pVirusIfExitCode.Size = new System.Drawing.Size(60, 20);
            this.m_pVirusIfExitCode.TabIndex = 6;
            // 
            // m_pGroupBox1
            // 
            this.m_pGroupBox1.Location = new System.Drawing.Point(5, 135);
            this.m_pGroupBox1.Name = "m_pGroupBox1";
            this.m_pGroupBox1.Size = new System.Drawing.Size(435, 4);
            this.m_pGroupBox1.TabIndex = 7;
            this.m_pGroupBox1.TabStop = false;
            // 
            // m_pCancel
            // 
            this.m_pCancel.Location = new System.Drawing.Point(295, 145);
            this.m_pCancel.Name = "m_pCancel";
            this.m_pCancel.Size = new System.Drawing.Size(70, 23);
            this.m_pCancel.TabIndex = 8;
            this.m_pCancel.Text = "Cancel";
            this.m_pCancel.Click += new System.EventHandler(this.m_pCancel_Click);
            // 
            // m_pOk
            // 
            this.m_pOk.Location = new System.Drawing.Point(370, 145);
            this.m_pOk.Name = "m_pOk";
            this.m_pOk.Size = new System.Drawing.Size(70, 23);
            this.m_pOk.TabIndex = 9;
            this.m_pOk.Text = "Ok";
            this.m_pOk.Click += new System.EventHandler(this.m_pOk_Click);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(453, 178);
            this.Controls.Add(this.mt_ScanProgram);
            this.Controls.Add(this.m_pScanProgram);
            this.Controls.Add(this.m_pGetScanProgram);
            this.Controls.Add(this.mt_ScanProgramArgs);
            this.Controls.Add(this.m_pScanProgramArgs);
            this.Controls.Add(this.mt_VirusIfExitCode);
            this.Controls.Add(this.m_pVirusIfExitCode);
            this.Controls.Add(this.m_pGroupBox1);
            this.Controls.Add(this.m_pCancel);
            this.Controls.Add(this.m_pOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mail Server Virus Scan Settings";
            ((System.ComponentModel.ISupportInitialize)(this.m_pVirusIfExitCode)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		private void m_pGetScanProgram_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.RestoreDirectory = true;
			openFileDialog.Filter = "Executable (*.exe)|*.exe";
			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				this.m_pScanProgram.Text = openFileDialog.FileName;
			}
		}

		private void m_pScanProgram_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pScanProgram.Text.ToLower().IndexOf("clamscan") > -1)
			{
				this.m_pScanProgramArgs.Text = "--database=\"C:\\Documents and Settings\\All Users\\.clamwin\\db\" \"#FileName\"";
				this.m_pVirusIfExitCode.Value = 1m;
			}
		}

		private void m_pCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
			base.Close();
		}

		private void m_pOk_Click(object sender, EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			bool flag = false;
			try
			{
				Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DataSmart.MailServer.Filters.Resources.eicar_message.eml.base64");
				byte[] array = new byte[manifestResourceStream.Length];
				manifestResourceStream.Read(array, 0, array.Length);
				array = Convert.FromBase64String(Encoding.UTF8.GetString(array));
				File.WriteAllBytes(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\eicar_virus_test.eml", array);
				string text = this.m_pScanProgram.Text;
				string arguments = this.m_pScanProgramArgs.Text.Replace("#FileName", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\eicar_virus_test.eml");
				int num = (int)this.m_pVirusIfExitCode.Value;
				int num2 = 0;
				Process process = Process.Start(new ProcessStartInfo(text, arguments)
				{
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardError = true
				});
				if (process != null)
				{
					process.WaitForExit(60000);
					num2 = process.ExitCode;
				}
				if (num != num2)
				{
					string text2 = process.StandardError.ReadToEnd();
					if (text2 == null)
					{
						text2 = "";
					}
					MessageBox.Show(this, "Error your scanner don't block test 'Eicar' virus file !\r\n\r\nScanner returned:\r\n" + text2, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
				else
				{
					flag = true;
				}
				if (File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\eicar_virus_test.eml"))
				{
					File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\eicar_virus_test.eml");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error: " + ex.Message);
			}
			if (flag)
			{
				this.SaveSettings();
				base.DialogResult = DialogResult.OK;
				base.Close();
			}
			this.Cursor = Cursors.Default;
		}

		private void LoadSettings()
		{
			this.m_pDsSettings = new DataSet();
			this.m_pDsSettings.Tables.Add("Settings");
			this.m_pDsSettings.Tables["Settings"].Columns.Add("Program");
			this.m_pDsSettings.Tables["Settings"].Columns.Add("Arguments");
			this.m_pDsSettings.Tables["Settings"].Columns.Add("VirusExitCode");
			if (File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\VirusScan.xml"))
			{
				this.m_pDsSettings.ReadXml(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\VirusScan.xml");
			}
			else
			{
				DataRow dataRow = this.m_pDsSettings.Tables["Settings"].NewRow();
				dataRow["Program"] = "";
				dataRow["Arguments"] = "";
				dataRow["VirusExitCode"] = 1;
				this.m_pDsSettings.Tables["Settings"].Rows.Add(new object[0]);
			}
			this.m_pScanProgram.Text = this.m_pDsSettings.Tables["Settings"].Rows[0]["Program"].ToString();
			this.m_pScanProgramArgs.Text = this.m_pDsSettings.Tables["Settings"].Rows[0]["Arguments"].ToString();
			this.m_pVirusIfExitCode.Value = ConvertEx.ToInt32(this.m_pDsSettings.Tables["Settings"].Rows[0]["VirusExitCode"], 1);
		}

		private void SaveSettings()
		{
			this.m_pDsSettings.Tables["Settings"].Rows[0]["Program"] = this.m_pScanProgram.Text;
			this.m_pDsSettings.Tables["Settings"].Rows[0]["Arguments"] = this.m_pScanProgramArgs.Text;
			this.m_pDsSettings.Tables["Settings"].Rows[0]["VirusExitCode"] = this.m_pVirusIfExitCode.Value;
			this.m_pDsSettings.WriteXml(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\VirusScan.xml");
		}
	}
}
