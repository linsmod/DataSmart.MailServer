using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class WLineGraph : Control
	{
		private List<Color> m_pLines;

		private List<int[]> m_pPoints;

		private int m_CellOffset;

		private int m_CellSize = 12;

		private bool m_AutoMaxValue;

		private int m_MaxValue = 100;

		public bool AutoMaxValue
		{
			get
			{
				return this.m_AutoMaxValue;
			}
			set
			{
				this.m_AutoMaxValue = value;
			}
		}

		public int MaximumValue
		{
			get
			{
				return this.m_MaxValue;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentException("MaximumValue value must be >= 1 !");
				}
				this.m_MaxValue = value;
			}
		}

		public WLineGraph()
		{
			this.m_pLines = new List<Color>();
			this.m_pPoints = new List<int[]>();
			try
			{
				base.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic).GetSetMethod(true).Invoke(this, new object[]
				{
					true
				});
			}
			catch
			{
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;
			base.OnPaint(e);
			int num = base.Width / 3;
			decimal d = base.Height / this.m_MaxValue;
			while (this.m_pPoints.Count > num)
			{
				this.m_pPoints.RemoveAt(0);
			}
			graphics.Clear(Color.Black);
			for (int i = 0; i < base.Height; i += this.m_CellSize)
			{
				graphics.DrawLine(new Pen(new SolidBrush(Color.Green)), 0, i, base.Width, i);
			}
			for (int j = base.Width; j > 0; j -= this.m_CellSize)
			{
				graphics.DrawLine(new Pen(new SolidBrush(Color.Green)), j - this.m_CellOffset, 0, j - this.m_CellOffset, base.Height);
			}
			for (int k = 0; k < this.m_pLines.Count; k++)
			{
				Color color = this.m_pLines[k];
				int y = base.Height;
				int num2 = base.Width;
				for (int l = this.m_pPoints.Count - 1; l > -1; l--)
				{
					graphics.DrawLine(new Pen(new SolidBrush(color)), num2 - 3, base.Height - (int)(this.m_pPoints[l][k] * d), num2, y);
					num2 -= 3;
					y = base.Height - (int)(this.m_pPoints[l][k] * d);
				}
			}
		}

		public void AddLine(Color lineColor)
		{
			this.m_pLines.Add(lineColor);
		}

		public void AddValue(int[] values)
		{
			if (this.m_pLines.Count != values.Length)
			{
				throw new ArgumentException("You must provide values for all lines, Lines count must equal values.Lengh !");
			}
			for (int i = 0; i < values.Length; i++)
			{
				int num = values[i];
				if (num < 0)
				{
					throw new ArgumentException("Value must be between > 0 !");
				}
				if (!this.m_AutoMaxValue && num > this.m_MaxValue)
				{
					throw new ArgumentException("Value must be between <= " + this.m_MaxValue + " (MaximumValue) !");
				}
			}
			this.m_pPoints.Add(values);
			if (this.m_AutoMaxValue)
			{
				this.m_MaxValue = 1;
				foreach (int[] current in this.m_pPoints)
				{
					int[] array = current;
					for (int j = 0; j < array.Length; j++)
					{
						int num2 = array[j];
						if (num2 > this.m_MaxValue)
						{
							this.m_MaxValue = num2;
						}
					}
				}
			}
			if (this.m_CellOffset < this.m_CellSize)
			{
				this.m_CellOffset += 3;
			}
			else
			{
				this.m_CellOffset = 3;
			}
			this.Refresh();
		}

		public void ClearValues()
		{
			this.m_pPoints.Clear();
			this.Refresh();
		}
	}
}
