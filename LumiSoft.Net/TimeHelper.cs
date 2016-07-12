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
            if (time.Length == TransferTimeFormat.Length)
            {
                return DateTime.ParseExact(time, TransferTimeFormat, DateTimeFormatInfo.CurrentInfo);
            }
            else if (time.Length == ReadalbeTimeFormat.Length)
            {
                return DateTime.ParseExact(time, ReadalbeTimeFormat, DateTimeFormatInfo.CurrentInfo);
            }
            return DateTime.Parse(time);
        }
    }
}
