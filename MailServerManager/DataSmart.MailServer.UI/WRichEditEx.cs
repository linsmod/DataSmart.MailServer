using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class WRichEditEx : UserControl
	{
		private class RickTextBoxEx : RichTextBox
		{
			private bool m_SuspendPaint;

			public bool SuspendPaint
			{
				get
				{
					return this.m_SuspendPaint;
				}
				set
				{
					this.m_SuspendPaint = value;
					if (!value)
					{
						this.Refresh();
					}
				}
			}

			protected override void WndProc(ref Message m)
			{
				if (m.Msg != 15)
				{
					base.WndProc(ref m);
					return;
				}
				if (this.m_SuspendPaint)
				{
					m.Result = IntPtr.Zero;
					return;
				}
				base.WndProc(ref m);
			}
		}

		private ToolStrip m_pToolbar;

		private WRichEditEx.RickTextBoxEx m_pTextbox;

		private bool m_Lock;

		public new string Text
		{
			get
			{
				return this.m_pTextbox.Text;
			}
			set
			{
				this.m_pTextbox.Text = value;
			}
		}

		public string Rtf
		{
			get
			{
				return this.m_pTextbox.Rtf;
			}
			set
			{
				this.m_pTextbox.Rtf = value;
			}
		}

		internal RichTextBox RichTextBox
		{
			get
			{
				return this.m_pTextbox;
			}
		}

		internal bool SuspendPaint
		{
			get
			{
				return this.m_pTextbox.SuspendPaint;
			}
			set
			{
				this.m_pTextbox.SuspendPaint = value;
			}
		}

		public WRichEditEx()
		{
			this.InitializeComponent();
			this.SetSelectionFont();
			Font font = (Font)this.m_pTextbox.SelectionFont.Clone();
			this.m_pTextbox.Font = (Font)font.Clone();
			this.m_pTextbox.SelectionFont = (Font)font.Clone();
		}

		private void InitializeComponent()
		{
			base.Size = new Size(100, 100);
			this.m_pToolbar = new ToolStrip();
			this.m_pToolbar.GripStyle = ToolStripGripStyle.Hidden;
			this.m_pToolbar.BackColor = SystemColors.Control;
			this.m_pToolbar.Renderer = new ToolBarRendererEx();
			ToolStripComboBox toolStripComboBox = new ToolStripComboBox();
			toolStripComboBox.Size = new Size(150, 20);
			toolStripComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			toolStripComboBox.SelectedIndexChanged += new EventHandler(this.font_SelectedIndexChanged);
			toolStripComboBox.Items.Add("Arial");
			toolStripComboBox.Items.Add("Courier New");
			toolStripComboBox.Items.Add("Times New Roman");
			toolStripComboBox.Items.Add("Verdana");
			if (toolStripComboBox.Items.Count > 0)
			{
				toolStripComboBox.SelectedIndex = 0;
			}
			this.m_pToolbar.Items.Add(toolStripComboBox);
			ToolStripComboBox toolStripComboBox2 = new ToolStripComboBox();
			toolStripComboBox2.AutoSize = false;
			toolStripComboBox2.Size = new Size(50, 20);
			toolStripComboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
			toolStripComboBox2.Items.Add("8");
			toolStripComboBox2.Items.Add("10");
			toolStripComboBox2.Items.Add("12");
			toolStripComboBox2.Items.Add("14");
			toolStripComboBox2.Items.Add("18");
			toolStripComboBox2.Items.Add("24");
			toolStripComboBox2.Items.Add("32");
			toolStripComboBox2.SelectedIndex = 1;
			toolStripComboBox2.SelectedIndexChanged += new EventHandler(this.fontSize_SelectedIndexChanged);
			this.m_pToolbar.Items.Add(toolStripComboBox2);
			this.m_pToolbar.Items.Add(new ToolStripSeparator());
			ToolStripButton toolStripButton = new ToolStripButton();
			toolStripButton.Image = ResManager.GetIcon("bold.ico").ToBitmap();
			toolStripButton.Click += new EventHandler(this.bold_Click);
			this.m_pToolbar.Items.Add(toolStripButton);
			ToolStripButton toolStripButton2 = new ToolStripButton();
			toolStripButton2.Image = ResManager.GetIcon("italic.ico").ToBitmap();
			toolStripButton2.Click += new EventHandler(this.italic_Click);
			this.m_pToolbar.Items.Add(toolStripButton2);
			ToolStripButton toolStripButton3 = new ToolStripButton();
			toolStripButton3.Image = ResManager.GetIcon("underline.ico").ToBitmap();
			toolStripButton3.Click += new EventHandler(this.underline_Click);
			this.m_pToolbar.Items.Add(toolStripButton3);
			this.m_pToolbar.Items.Add(new ToolStripSeparator());
			ToolStripButton toolStripButton4 = new ToolStripButton();
			toolStripButton4.Image = this.CreateFontColorIcon(Color.Black);
			toolStripButton4.Click += new EventHandler(this.fontColor_Click);
			this.m_pToolbar.Items.Add(toolStripButton4);
			ToolStripButton toolStripButton5 = new ToolStripButton();
			toolStripButton5.Image = this.CreateFontBackColorIcon(Color.White);
			toolStripButton5.Click += new EventHandler(this.fontBackColor_Click);
			this.m_pToolbar.Items.Add(toolStripButton5);
			this.m_pTextbox = new WRichEditEx.RickTextBoxEx();
			this.m_pTextbox.Size = new Size(97, 73);
			this.m_pTextbox.Location = new Point(1, 25);
			this.m_pTextbox.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pTextbox.BorderStyle = BorderStyle.None;
			this.m_pTextbox.HideSelection = false;
			this.m_pTextbox.SelectionChanged += new EventHandler(this.m_pTextbox_SelectionChanged);
			base.Controls.Add(this.m_pToolbar);
			base.Controls.Add(this.m_pTextbox);
		}

		public void SetText(string text)
		{
			this.m_pTextbox.AppendText(text);
		}

		private void font_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pToolbar.Items.Count == 0)
			{
				return;
			}
			this.SetSelectionFont();
		}

		private void fontSize_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pToolbar.Items.Count == 0)
			{
				return;
			}
			this.SetSelectionFont();
		}

		private void bold_Click(object sender, EventArgs e)
		{
			if (((ToolStripButton)this.m_pToolbar.Items[3]).Checked)
			{
				((ToolStripButton)this.m_pToolbar.Items[3]).Checked = false;
			}
			else
			{
				((ToolStripButton)this.m_pToolbar.Items[3]).Checked = true;
			}
			this.SetSelectionFont();
		}

		private void italic_Click(object sender, EventArgs e)
		{
			if (((ToolStripButton)this.m_pToolbar.Items[4]).Checked)
			{
				((ToolStripButton)this.m_pToolbar.Items[4]).Checked = false;
			}
			else
			{
				((ToolStripButton)this.m_pToolbar.Items[4]).Checked = true;
			}
			this.SetSelectionFont();
		}

		private void underline_Click(object sender, EventArgs e)
		{
			if (((ToolStripButton)this.m_pToolbar.Items[5]).Checked)
			{
				((ToolStripButton)this.m_pToolbar.Items[5]).Checked = false;
			}
			else
			{
				((ToolStripButton)this.m_pToolbar.Items[5]).Checked = true;
			}
			this.SetSelectionFont();
		}

		private void fontColor_Click(object sender, EventArgs e)
		{
			ColorDialog colorDialog = new ColorDialog();
			if (colorDialog.ShowDialog(this) == DialogResult.OK)
			{
				this.m_pTextbox.SelectionColor = colorDialog.Color;
				((ToolStripButton)this.m_pToolbar.Items[7]).Image = this.CreateFontColorIcon(colorDialog.Color);
			}
		}

		private void fontBackColor_Click(object sender, EventArgs e)
		{
			ColorDialog colorDialog = new ColorDialog();
			if (colorDialog.ShowDialog(this) == DialogResult.OK)
			{
				this.m_pTextbox.SelectionBackColor = colorDialog.Color;
				((ToolStripButton)this.m_pToolbar.Items[8]).Image = this.CreateFontBackColorIcon(colorDialog.Color);
			}
		}

		private void m_pTextbox_SelectionChanged(object sender, EventArgs e)
		{
			if (this.m_pTextbox.SelectionFont == null)
			{
				return;
			}
			this.m_Lock = true;
			((ToolStripComboBox)this.m_pToolbar.Items[0]).Text = this.m_pTextbox.SelectionFont.Name;
			((ToolStripComboBox)this.m_pToolbar.Items[1]).Text = this.m_pTextbox.SelectionFont.Size.ToString();
			((ToolStripButton)this.m_pToolbar.Items[3]).Checked = this.m_pTextbox.SelectionFont.Bold;
			((ToolStripButton)this.m_pToolbar.Items[4]).Checked = this.m_pTextbox.SelectionFont.Italic;
			((ToolStripButton)this.m_pToolbar.Items[5]).Checked = this.m_pTextbox.SelectionFont.Underline;
			((ToolStripButton)this.m_pToolbar.Items[7]).Image = this.CreateFontColorIcon(this.m_pTextbox.SelectionColor);
			((ToolStripButton)this.m_pToolbar.Items[8]).Image = this.CreateFontBackColorIcon(this.m_pTextbox.SelectionBackColor);
			this.m_Lock = false;
		}

		private void SetSelectionFont()
		{
			if (this.m_Lock)
			{
				return;
			}
			FontStyle fontStyle = FontStyle.Regular;
			if (((ToolStripButton)this.m_pToolbar.Items[3]).Checked)
			{
				fontStyle |= FontStyle.Bold;
			}
			if (((ToolStripButton)this.m_pToolbar.Items[4]).Checked)
			{
				fontStyle |= FontStyle.Italic;
			}
			if (((ToolStripButton)this.m_pToolbar.Items[5]).Checked)
			{
				fontStyle |= FontStyle.Underline;
			}
			this.m_pTextbox.SelectionFont = new Font(((ToolStripComboBox)this.m_pToolbar.Items[0]).Text, (float)Convert.ToInt32(((ToolStripComboBox)this.m_pToolbar.Items[1]).Text), fontStyle);
			this.m_pTextbox.Focus();
		}

		private Bitmap CreateFontColorIcon(Color color)
		{
			Bitmap bitmap = ResManager.GetIcon("fontcolor.ico").ToBitmap();
			for (int i = 0; i < bitmap.Width; i++)
			{
				for (int j = 12; j < bitmap.Height; j++)
				{
					bitmap.SetPixel(i, j, color);
				}
			}
			return bitmap;
		}

		private Bitmap CreateFontBackColorIcon(Color color)
		{
			Bitmap bitmap = ResManager.GetIcon("fontbackcolor.ico").ToBitmap();
			for (int i = 0; i < bitmap.Width; i++)
			{
				for (int j = 12; j < bitmap.Height; j++)
				{
					bitmap.SetPixel(i, j, color);
				}
			}
			return bitmap;
		}
	}
}
