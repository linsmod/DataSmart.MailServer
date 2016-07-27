using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.NetworkToolkit
{
    class ExceptionHelper
    {
        public static string GetExceptionMessage(Exception ex, int deep = 1)
        {
            var whiteSpaces = string.Join("----", Enumerable.Repeat("", deep - 1));
            var sb = new StringBuilder();
            sb.AppendLine(whiteSpaces + ex.Message);
            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                sb.AppendLine(whiteSpaces + ex.StackTrace);
            }
            if (ex.InnerException != null)
            {
                sb.AppendFormat(GetExceptionMessage(ex.InnerException, deep + 1));
            }
            return sb.ToString();
        }
    }
}
