using DataSmart.MailServer.Management;
using DataSmart.MailServer.UI.Resources;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class GlobalMessageRulesForm : Form
	{
		private ToolStrip m_pToolbar;

		private ImageList m_pRulesImages;

		private ListView m_pRules;

		private VirtualServer m_pVirtualServer;

		public GlobalMessageRulesForm(VirtualServer virtualServer, WFrame frame)
		{
			this.m_pVirtualServer = virtualServer;
			this.InitializeComponent();
			frame.Frame_ToolStrip = this.m_pToolbar;
			this.LoadRules("");
		}

		private void InitializeComponent()
		{
			base.Size = new Size(472, 357);
			ImageList imageList = new ImageList();
			imageList.Images.Add(ResManager.GetIcon("add.ico"));
			imageList.Images.Add(ResManager.GetIcon("edit.ico"));
			imageList.Images.Add(ResManager.GetIcon("delete.ico"));
			this.m_pToolbar = new ToolStrip();
			this.m_pToolbar.GripStyle = ToolStripGripStyle.Hidden;
			this.m_pToolbar.BackColor = this.BackColor;
			this.m_pToolbar.Renderer = new ToolBarRendererEx();
			this.m_pToolbar.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pToolbar_ItemClicked);
			ToolStripButton toolStripButton = new ToolStripButton();
			toolStripButton.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripButton.Tag = "add";
			this.m_pToolbar.Items.Add(toolStripButton);
			ToolStripButton toolStripButton2 = new ToolStripButton();
			toolStripButton2.Enabled = false;
			toolStripButton2.Image = ResManager.GetIcon("edit.ico").ToBitmap();
			toolStripButton2.Tag = "edit";
			this.m_pToolbar.Items.Add(toolStripButton2);
			ToolStripButton toolStripButton3 = new ToolStripButton();
			toolStripButton3.Enabled = false;
			toolStripButton3.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			toolStripButton3.Tag = "delete";
			this.m_pToolbar.Items.Add(toolStripButton3);
			this.m_pToolbar.Items.Add(new ToolStripSeparator());
			ToolStripButton toolStripButton4 = new ToolStripButton();
			toolStripButton4.Image = ResManager.GetIcon("refresh.ico").ToBitmap();
			toolStripButton4.Tag = "refresh";
			toolStripButton4.ToolTipText = "Refresh";
			this.m_pToolbar.Items.Add(toolStripButton4);
			this.m_pToolbar.Items.Add(new ToolStripSeparator());
			ToolStripButton toolStripButton5 = new ToolStripButton();
			toolStripButton5.Enabled = false;
			toolStripButton5.Image = ResManager.GetIcon("up.ico").ToBitmap();
			toolStripButton5.Tag = "up";
			this.m_pToolbar.Items.Add(toolStripButton5);
			ToolStripButton toolStripButton6 = new ToolStripButton();
			toolStripButton6.Enabled = false;
			toolStripButton6.Image = ResManager.GetIcon("down.ico").ToBitmap();
			toolStripButton6.Tag = "down";
			this.m_pToolbar.Items.Add(toolStripButton6);
			this.m_pRulesImages = new ImageList();
			this.m_pRulesImages.Images.Add(ResManager.GetIcon("messagerule.ico"));
			this.m_pRulesImages.Images.Add(ResManager.GetIcon("messagerule_disabled.ico"));
			this.m_pRules = new ListView();
			this.m_pRules.Size = new Size(445, 290);
			this.m_pRules.Location = new Point(10, 20);
			this.m_pRules.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pRules.View = View.Details;
			this.m_pRules.FullRowSelect = true;
			this.m_pRules.HideSelection = false;
			this.m_pRules.SmallImageList = this.m_pRulesImages;
			this.m_pRules.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			this.m_pRules.DoubleClick += new EventHandler(this.m_pRules_DoubleClick);
			this.m_pRules.SelectedIndexChanged += new EventHandler(this.m_pRules_SelectedIndexChanged);
			this.m_pRules.MouseUp += new MouseEventHandler(this.m_pRules_MouseUp);
			this.m_pRules.Columns.Add("Description", 460, HorizontalAlignment.Left);
			base.Controls.Add(this.m_pRules);
		}

		private void m_pToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			this.SwitchToolBarTask(e.ClickedItem.Tag.ToString());
		}

		private void m_pRules_DoubleClick(object sender, EventArgs e)
		{
			if (this.m_pRules.SelectedItems.Count > 0)
			{
				GlobalMessageRule globalMessageRule = (GlobalMessageRule)this.m_pRules.SelectedItems[0].Tag;
				AddEditGlobalMessageRuleForm addEditGlobalMessageRuleForm = new AddEditGlobalMessageRuleForm(this.m_pVirtualServer, globalMessageRule);
				if (addEditGlobalMessageRuleForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadRules(globalMessageRule.ID);
				}
			}
		}

		private void m_pRules_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.m_pRules.SelectedItems.Count > 0)
			{
				this.m_pToolbar.Items[1].Enabled = true;
				this.m_pToolbar.Items[2].Enabled = true;
				if (this.m_pRules.SelectedItems[0].Index > 0)
				{
					this.m_pToolbar.Items[6].Enabled = true;
				}
				if (this.m_pRules.SelectedItems[0].Index < this.m_pRules.Items.Count - 1)
				{
					this.m_pToolbar.Items[7].Enabled = true;
					return;
				}
			}
			else
			{
				this.m_pToolbar.Items[1].Enabled = false;
				this.m_pToolbar.Items[2].Enabled = false;
				this.m_pToolbar.Items[6].Enabled = false;
				this.m_pToolbar.Items[7].Enabled = false;
			}
		}

		private void m_pRules_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right)
			{
				return;
			}
			ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
			contextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(this.m_pRules_ContextMenuItem_Clicked);
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("Add");
			toolStripMenuItem.Image = ResManager.GetIcon("add.ico").ToBitmap();
			toolStripMenuItem.Tag = "add";
			contextMenuStrip.Items.Add(toolStripMenuItem);
			ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem("Edit");
			toolStripMenuItem2.Enabled = (this.m_pRules.SelectedItems.Count > 0);
			toolStripMenuItem2.Tag = "edit";
			toolStripMenuItem2.Image = ResManager.GetIcon("edit.ico").ToBitmap();
			contextMenuStrip.Items.Add(toolStripMenuItem2);
			ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem("Delete");
			toolStripMenuItem3.Enabled = (this.m_pRules.SelectedItems.Count > 0);
			toolStripMenuItem3.Tag = "delete";
			toolStripMenuItem3.Image = ResManager.GetIcon("delete.ico").ToBitmap();
			contextMenuStrip.Items.Add(toolStripMenuItem3);
			contextMenuStrip.Items.Add(new ToolStripSeparator());
			ToolStripMenuItem toolStripMenuItem4 = new ToolStripMenuItem("Refresh");
			toolStripMenuItem4.Image = ResManager.GetIcon("refresh.ico").ToBitmap();
			toolStripMenuItem4.Tag = "refresh";
			contextMenuStrip.Items.Add(toolStripMenuItem4);
			contextMenuStrip.Items.Add(new ToolStripSeparator());
			ToolStripMenuItem toolStripMenuItem5 = new ToolStripMenuItem("Move Up");
			if (this.m_pRules.SelectedItems.Count <= 0 || this.m_pRules.SelectedItems[0].Index <= 0)
			{
				toolStripMenuItem5.Enabled = false;
			}
			toolStripMenuItem5.Image = ResManager.GetIcon("up.ico").ToBitmap();
			toolStripMenuItem5.Tag = "up";
			contextMenuStrip.Items.Add(toolStripMenuItem5);
			ToolStripMenuItem toolStripMenuItem6 = new ToolStripMenuItem("Move Down");
			if (this.m_pRules.SelectedItems.Count <= 0 || this.m_pRules.SelectedItems[0].Index >= this.m_pRules.Items.Count - 1)
			{
				toolStripMenuItem6.Enabled = false;
			}
			toolStripMenuItem6.Image = ResManager.GetIcon("down.ico").ToBitmap();
			toolStripMenuItem6.Tag = "down";
			contextMenuStrip.Items.Add(toolStripMenuItem6);
			contextMenuStrip.Show(Control.MousePosition);
		}

		private void m_pRules_ContextMenuItem_Clicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem.Tag == null)
			{
				return;
			}
			this.SwitchToolBarTask(e.ClickedItem.Tag.ToString());
		}

		private void SwitchToolBarTask(string taskID)
		{
			if (taskID == "add")
			{
				AddEditGlobalMessageRuleForm addEditGlobalMessageRuleForm = new AddEditGlobalMessageRuleForm(this.m_pVirtualServer);
				if (addEditGlobalMessageRuleForm.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadRules("");
					return;
				}
			}
			else if (taskID == "edit")
			{
				GlobalMessageRule globalMessageRule = (GlobalMessageRule)this.m_pRules.SelectedItems[0].Tag;
				AddEditGlobalMessageRuleForm addEditGlobalMessageRuleForm2 = new AddEditGlobalMessageRuleForm(this.m_pVirtualServer, globalMessageRule);
				if (addEditGlobalMessageRuleForm2.ShowDialog(this) == DialogResult.OK)
				{
					this.LoadRules(globalMessageRule.ID);
					return;
				}
			}
			else if (taskID == "delete")
			{
				GlobalMessageRule globalMessageRule2 = (GlobalMessageRule)this.m_pRules.SelectedItems[0].Tag;
				if (MessageBox.Show(this, "Are you sure you want to delete Rule '" + globalMessageRule2.Description + "' !", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					globalMessageRule2.Owner.Remove(globalMessageRule2);
					this.m_pRules.SelectedItems[0].Remove();
					return;
				}
			}
			else
			{
				if (taskID == "refresh")
				{
					this.LoadRules("");
					return;
				}
				if (taskID == "up")
				{
					if (this.m_pRules.SelectedItems.Count > 0 && this.m_pRules.SelectedItems[0].Index > 0)
					{
						this.SwapRules(this.m_pRules.SelectedItems[0], this.m_pRules.Items[this.m_pRules.SelectedItems[0].Index - 1]);
						return;
					}
				}
				else if (taskID == "down" && this.m_pRules.SelectedItems.Count > 0 && this.m_pRules.SelectedItems[0].Index < this.m_pRules.Items.Count - 1)
				{
					this.SwapRules(this.m_pRules.SelectedItems[0], this.m_pRules.Items[this.m_pRules.SelectedItems[0].Index + 1]);
				}
			}
		}

		private void LoadRules(string selectedRuleID)
		{
			this.m_pRules.Items.Clear();
			this.m_pVirtualServer.GlobalMessageRules.Refresh();
			foreach (GlobalMessageRule globalMessageRule in this.m_pVirtualServer.GlobalMessageRules)
			{
				ListViewItem listViewItem = new ListViewItem();
				if (!globalMessageRule.Enabled)
				{
					listViewItem.ForeColor = Color.Purple;
					listViewItem.Font = new Font(listViewItem.Font.FontFamily, listViewItem.Font.Size, FontStyle.Strikeout);
					listViewItem.ImageIndex = 1;
				}
				else
				{
					listViewItem.ImageIndex = 0;
				}
				listViewItem.Tag = globalMessageRule;
				listViewItem.Text = globalMessageRule.Description;
				this.m_pRules.Items.Add(listViewItem);
				if (globalMessageRule.ID == selectedRuleID)
				{
					listViewItem.Selected = true;
				}
			}
			this.m_pRules_SelectedIndexChanged(this, new EventArgs());
		}

		private void SwapRules(ListViewItem item1, ListViewItem item2)
		{
			GlobalMessageRule globalMessageRule = (GlobalMessageRule)item1.Tag;
			GlobalMessageRule globalMessageRule2 = (GlobalMessageRule)item2.Tag;
			string selectedRuleID = "";
			if (item1.Selected)
			{
				selectedRuleID = globalMessageRule.ID;
			}
			else if (item2.Selected)
			{
				selectedRuleID = globalMessageRule2.ID;
			}
			long cost = globalMessageRule2.Cost;
			globalMessageRule2.Cost = globalMessageRule.Cost;
			globalMessageRule2.Commit();
			globalMessageRule.Cost = cost;
			globalMessageRule.Commit();
			this.m_pVirtualServer.GlobalMessageRules.Refresh();
			this.LoadRules(selectedRuleID);
		}
	}
}
