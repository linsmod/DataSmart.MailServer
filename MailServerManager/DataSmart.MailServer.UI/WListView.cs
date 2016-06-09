using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class WListView : ListView
	{
		private ColumnHeader m_pSortingColumn;

		private SortOrder m_SortOrder;

		protected override void OnColumnClick(ColumnClickEventArgs e)
		{
			ColumnHeader columnHeader = base.Columns[e.Column];
			if (this.m_pSortingColumn != null && columnHeader == this.m_pSortingColumn)
			{
				if (this.m_SortOrder == SortOrder.Descending || this.m_SortOrder == SortOrder.None)
				{
					this.m_SortOrder = SortOrder.Ascending;
				}
				else
				{
					this.m_SortOrder = SortOrder.Descending;
				}
			}
			else
			{
				this.m_SortOrder = SortOrder.Ascending;
			}
			this.m_pSortingColumn = columnHeader;
			this.SortItems();
			base.OnColumnClick(e);
		}

		public void SortItems()
		{
			if (this.m_pSortingColumn == null)
			{
				return;
			}
			base.BeginUpdate();
			ListViewItem[] array = new ListViewItem[base.Items.Count];
			base.Items.CopyTo(array, 0);
			List<ListViewItem> list = new List<ListViewItem>(array);
			for (int i = 0; i < list.Count; i++)
			{
				bool flag = false;
				for (int j = 0; j < list.Count - 1; j++)
				{
					ListViewItem listViewItem = list[j];
					ListViewItem listViewItem2 = list[j + 1];
					if (this.m_SortOrder == SortOrder.Ascending)
					{
						if (listViewItem.SubItems[this.m_pSortingColumn.Index].Text.CompareTo(listViewItem2.SubItems[this.m_pSortingColumn.Index].Text) > 0)
						{
							list.Remove(listViewItem);
							list.Insert(j + 1, listViewItem);
							flag = true;
						}
					}
					else if (listViewItem2.SubItems[this.m_pSortingColumn.Index].Text.CompareTo(listViewItem.SubItems[this.m_pSortingColumn.Index].Text) > 0)
					{
						list.Remove(listViewItem);
						list.Insert(j + 1, listViewItem);
						flag = true;
					}
				}
				if (!flag)
				{
					break;
				}
			}
			base.Items.Clear();
			base.Items.AddRange(list.ToArray());
			base.EndUpdate();
			this.Refresh();
		}
	}
}
