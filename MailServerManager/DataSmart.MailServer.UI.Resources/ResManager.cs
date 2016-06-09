using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace DataSmart.MailServer.UI.Resources
{
	public class ResManager
	{
		public static Icon GetIcon(string iconName)
		{
			return ResManager.GetIcon(iconName, new Size(32, 32));
		}

		public static Icon GetIcon(string iconName, Size size)
		{
			Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(ResManager).Namespace + "." + iconName);
			return new Icon(manifestResourceStream, size);
		}

		public static Image GetImage(string imageName)
		{
			Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(ResManager).Namespace + "." + imageName);
			return Image.FromStream(manifestResourceStream);
		}
	}
}
