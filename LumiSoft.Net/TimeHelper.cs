using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.NetworkToolkit
{
    public class TimeHelper
    {
        /// <summary>
        /// 用于阅读的时间格式
        /// </summary>
        public const string ReadalbeTimeFormat = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// 用于数据传输的时间格式
        /// </summary>
        public const string TransferTimeFormat = "yyyyMMddHHmmss";


        public static DateTime Parse(string time)
        {
            DateTime utcTime;
            if (!DateTime.TryParseExact(time, new string[] { TransferTimeFormat, ReadalbeTimeFormat }, DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None, out utcTime))
            {
                if (!DateTime.TryParse(time, DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None, out utcTime))
                {
                    throw new FormatException("unsopported time format:" + time);
                }
            }
            return utcTime.ToLocalTime();
        }

        public static string ToString(DateTime dt, string format = TransferTimeFormat)
        {
            return dt.ToString(TransferTimeFormat, DateTimeFormatInfo.InvariantInfo);
        }
    }
}
