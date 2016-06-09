using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class WFrame : UserControl
	{
		private Panel m_pFormPanel;

		private Splitter splitter1;

		public Panel ControlPanel;

		private Panel TopPane;

		private Panel ToolBarPanel;

		private Container components;

		private Form m_Form;

		private Control m_TooBar;

		private Control m_BarCotrol;

		private Color m_SplitterColor = Color.FromKnownColor(KnownColor.Control);

		private int m_SplitterWidth = 5;

		private int m_SplitterMinSize;

		private int m_SplitterMinExtra;

		private int m_TopPaneHeight = 25;

		private Color m_TopPaneBkColor = Color.FromKnownColor(KnownColor.Control);

		private bool m_IsSplitterLocked;

		private int m_ControlPaneWidth = 120;

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Form Frame_Form
		{
			get
			{
				return this.m_Form;
			}
			set
			{
				if (value != null)
				{
					if (this.m_Form != null)
					{
						this.m_Form.Dispose();
					}
					this.m_Form = value;
					this.m_Form.Location = new Point(0, 0);
					this.m_Form.TopLevel = false;
					this.m_Form.TopMost = false;
					this.m_Form.ControlBox = false;
					this.m_Form.FormBorderStyle = FormBorderStyle.None;
					this.m_Form.StartPosition = FormStartPosition.Manual;
					this.m_Form.Size = this.m_pFormPanel.ClientSize;
					this.m_Form.Parent = this.m_pFormPanel;
					this.m_Form.Visible = true;
					this.m_Form.Focus();
				}
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ToolStrip Frame_ToolStrip
		{
			get
			{
				return (ToolStrip)this.m_TooBar;
			}
			set
			{
				if (this.m_TooBar != null)
				{
					this.m_TooBar.Dispose();
				}
				if (value != null)
				{
					this.m_TooBar = value;
					this.ToolBarPanel.Controls.Add(this.m_TooBar);
					this.m_TooBar.Visible = true;
				}
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Control Frame_BarControl
		{
			get
			{
				return this.m_BarCotrol;
			}
			set
			{
				if (value != null && value.Handle.ToInt32() != 0)
				{
					if (this.m_BarCotrol != null)
					{
						this.m_BarCotrol.Dispose();
					}
					this.m_BarCotrol = value;
					this.ControlPanel.Controls.Add(this.m_BarCotrol);
					this.m_BarCotrol.Location = new Point(0, 0);
					this.m_BarCotrol.Dock = DockStyle.Fill;
					this.m_BarCotrol.Visible = true;
				}
			}
		}

		public BorderStyle FormFrameBorder
		{
			get
			{
				return this.m_pFormPanel.BorderStyle;
			}
			set
			{
				this.m_pFormPanel.BorderStyle = value;
			}
		}

		[Category("SplitterWnd"), DefaultValue(KnownColor.Control), Description("Splitter window color")]
		public Color SplitterColor
		{
			get
			{
				return this.m_SplitterColor;
			}
			set
			{
				this.m_SplitterColor = value;
				this.splitter1.BackColor = this.m_SplitterColor;
				base.Invalidate();
			}
		}

		[Category("SplitterWnd"), DefaultValue(5), Description("Splitter window Width")]
		public int SplitterWidth
		{
			get
			{
				return this.m_SplitterWidth;
			}
			set
			{
				this.m_SplitterWidth = value;
				this.splitter1.Width = this.m_SplitterWidth;
				base.Invalidate();
			}
		}

		[Category("SplitterWnd"), DefaultValue(25), Description("Splitter window MinSize")]
		public int SplitterMinSize
		{
			get
			{
				return this.m_SplitterMinSize;
			}
			set
			{
				this.m_SplitterMinSize = value;
				this.splitter1.MinSize = this.m_SplitterMinSize;
				base.Invalidate();
			}
		}

		[Category("SplitterWnd"), DefaultValue(25), Description("Splitter window MinExtra")]
		public int SplitterMinExtra
		{
			get
			{
				return this.m_SplitterMinExtra;
			}
			set
			{
				this.m_SplitterMinExtra = value;
				this.splitter1.MinExtra = this.m_SplitterMinExtra;
				base.Invalidate();
			}
		}

		[Category("SplitterWnd"), DefaultValue(false), Description("Locks Splitter window")]
		public bool LockSplitter
		{
			get
			{
				return this.m_IsSplitterLocked;
			}
			set
			{
				this.m_IsSplitterLocked = value;
				this.splitter1.Enabled = !this.m_IsSplitterLocked;
				base.Invalidate();
			}
		}

		[Category("TopPane"), DefaultValue(20), Description("Top pane window Height")]
		public int TopPaneHeight
		{
			get
			{
				return this.m_TopPaneHeight;
			}
			set
			{
				this.m_TopPaneHeight = value;
				this.TopPane.Height = this.m_TopPaneHeight;
				base.Invalidate();
			}
		}

		[Category("TopPane"), Description("Top Pane BackRound color")]
		public Color TopPaneBkColor
		{
			get
			{
				return this.m_TopPaneBkColor;
			}
			set
			{
				this.m_TopPaneBkColor = value;
				this.TopPane.BackColor = this.m_TopPaneBkColor;
				base.Invalidate();
			}
		}

		[Category("ControlPane"), Description("Control pane Width")]
		public int ControlPanelWidth
		{
			get
			{
				return this.m_ControlPaneWidth;
			}
			set
			{
				this.m_ControlPaneWidth = value;
				this.ControlPanel.Width = this.m_ControlPaneWidth;
				this.ToolBarPanel.Left = this.m_ControlPaneWidth + this.SplitterWidth + 4;
				if (this.m_BarCotrol != null)
				{
					this.m_BarCotrol.Width = this.m_ControlPaneWidth - 5;
				}
				base.Invalidate();
			}
		}

		public WFrame()
		{
			this.InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.m_pFormPanel = new Panel();
			this.splitter1 = new Splitter();
			this.ControlPanel = new Panel();
			this.TopPane = new Panel();
			this.ToolBarPanel = new Panel();
			this.TopPane.SuspendLayout();
			base.SuspendLayout();
			this.m_pFormPanel.BorderStyle = BorderStyle.FixedSingle;
			this.m_pFormPanel.Dock = DockStyle.Fill;
			this.m_pFormPanel.Location = new Point(103, 25);
			this.m_pFormPanel.Name = "m_pFormPanel";
			this.m_pFormPanel.Size = new Size(444, 423);
			this.m_pFormPanel.TabIndex = 9;
			this.splitter1.Location = new Point(100, 25);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new Size(3, 423);
			this.splitter1.TabIndex = 8;
			this.splitter1.TabStop = false;
			this.splitter1.SplitterMoved += new SplitterEventHandler(this.splitter1_SplitterMoved);
			this.ControlPanel.BorderStyle = BorderStyle.FixedSingle;
			this.ControlPanel.Dock = DockStyle.Left;
			this.ControlPanel.Location = new Point(0, 25);
			this.ControlPanel.Name = "ControlPane";
			this.ControlPanel.Size = new Size(100, 423);
			this.ControlPanel.TabIndex = 6;
			this.TopPane.Controls.Add(this.ToolBarPanel);
			this.TopPane.Dock = DockStyle.Top;
			this.TopPane.Location = new Point(0, 0);
			this.TopPane.Name = "TopPane";
			this.TopPane.Size = new Size(547, 25);
			this.TopPane.TabIndex = 7;
			this.ToolBarPanel.Location = new Point(112, 0);
			this.ToolBarPanel.Name = "ToolBarPanel";
			this.ToolBarPanel.Size = new Size(184, 24);
			this.ToolBarPanel.TabIndex = 1;
			base.Controls.Add(this.m_pFormPanel);
			base.Controls.Add(this.splitter1);
			base.Controls.Add(this.ControlPanel);
			base.Controls.Add(this.TopPane);
			base.Name = "WFrame";
			base.Size = new Size(547, 448);
			this.TopPane.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		protected void splitter1_SplitterMoved(object sender, SplitterEventArgs e)
		{
			this.m_ControlPaneWidth = this.ControlPanel.Width;
			this.OnResize(new EventArgs());
			this.splitter1.Invalidate();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (this.ControlPanelWidth > 0)
			{
				this.ToolBarPanel.Left = this.ControlPanelWidth + this.SplitterWidth + 4;
			}
			this.ToolBarPanel.Width = base.Width - this.ToolBarPanel.Left - 4;
			if (this.m_Form != null)
			{
				this.m_Form.Size = this.m_pFormPanel.ClientSize;
			}
		}
	}
}
