using System;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class WTreeView : TreeView
	{
		public WTreeView()
		{
			base.AfterCheck += new TreeViewEventHandler(this.WTreeView_AfterCheck);
		}

		private void WTreeView_AfterCheck(object sender, TreeViewEventArgs e)
		{
			TreeNode node = e.Node;
			foreach (TreeNode treeNode in node.Nodes)
			{
				treeNode.Checked = node.Checked;
			}
		}
	}
}
