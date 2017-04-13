using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Test
{
    public static class Utf8StringHelper
    {
        public static string Utf8BytesToString(this byte[] utf8String)
        {
            return Encoding.UTF8.GetString(utf8String);
        }
    }
}
