using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Reflection;

namespace DataSmart.MailServer.MssqlStorage
{
    /// <summary>
    /// Resource manager.
    /// </summary>
    internal class ResManager
    {
        #region static method GetText

        /// <summary>
        /// Gets stored resource as text.
        /// </summary>
        /// <returns></returns>
        public static string GetText(string fileName,System.Text.Encoding encoding)
        {
            Stream rs = Assembly.GetExecutingAssembly().GetManifestResourceStream("LumiSoft.MailServer.Resources." + fileName);
            byte[] text = new byte[rs.Length];
            rs.Read(text,0,text.Length);
            return encoding.GetString(text);
        }

        #endregion
    }
}
