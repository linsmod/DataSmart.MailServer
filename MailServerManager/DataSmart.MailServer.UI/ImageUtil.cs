using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataSmart.MailServer.UI
{
	public class ImageUtil
	{
		public static Image GetGrayImage(Image image)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			Image image2 = (Image)image.Clone();
			using (Graphics graphics = Graphics.FromImage(image2))
			{
				ControlPaint.DrawImageDisabled(graphics, image, 0, 0, Color.Transparent);
			}
			return image2;
		}
	}
}
