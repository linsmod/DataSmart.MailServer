using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public static class SecureStringExtensions
    {
        public static SecureString ConvertToSecureString(this string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }
            SecureString str = new SecureString();
            foreach (char ch in password)
            {
                str.AppendChar(ch);
            }
            return str;
        }

        public static string ConvertToUnsecureString(this SecureString securePassword)
        {
            string str;
            if (securePassword == null)
            {
                throw new ArgumentNullException("securePassword");
            }
            IntPtr zero = IntPtr.Zero;
            try
            {
                zero = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                str = Marshal.PtrToStringUni(zero);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(zero);
            }
            return str;
        }

        public static bool SecureEquals(this SecureString originalString, SecureString value)
        {
            bool flag;
            if ((originalString == null) && (value == null))
            {
                return true;
            }
            if ((originalString == null) || (value == null))
            {
                return false;
            }
            if (originalString.Length != value.Length)
            {
                return false;
            }
            if ((originalString.Length == 0) && (value.Length == 0))
            {
                return true;
            }
            IntPtr ptr = Marshal.SecureStringToCoTaskMemUnicode(originalString);
            IntPtr ptr2 = Marshal.SecureStringToCoTaskMemUnicode(value);
            try
            {
                byte num = 1;
                byte num2 = 1;
                for (int i = 0; (num != 0) && (num2 != 0); i += 2)
                {
                    num = Marshal.ReadByte(ptr, i);
                    num2 = Marshal.ReadByte(ptr2, i);
                    if (num != num2)
                    {
                        return false;
                    }
                }
                flag = true;
            }
            finally
            {
                Marshal.ZeroFreeCoTaskMemUnicode(ptr);
                Marshal.ZeroFreeCoTaskMemUnicode(ptr2);
            }
            return flag;
        }
    }
}
