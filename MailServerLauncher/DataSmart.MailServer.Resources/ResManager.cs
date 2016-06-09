using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DataSmart.MailServer.Resources
{
	public class ResManager
	{
		private static Assembly Self = Assembly.GetExecutingAssembly();

		public static Icon GetIcon(string iconName)
		{
			string text = ResManager.Self.GetManifestResourceNames().FirstOrDefault((string x) => x.IndexOf(iconName) != -1);
			if (text == null)
			{
				return null;
			}
			Stream manifestResourceStream = ResManager.Self.GetManifestResourceStream(text);
			return new Icon(manifestResourceStream);
		}

		public static Image GetImage(string imageName)
		{
			Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DataSmart.MailServer.Resources." + imageName);
			return Image.FromStream(manifestResourceStream);
		}
	}
}
