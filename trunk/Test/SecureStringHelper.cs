using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;

namespace Test
{
    public static class SecureStringHelper
    {
        public static string ToUnencryptedString(this SecureString ss)
        {
            IntPtr ustr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(ss);
            try
            {
                return System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ustr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ustr);
            }
        }
    }
}
