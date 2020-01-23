﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;

namespace MurrayGrant.ReadablePassphrase.Helpers
{
    public static class TypeHelpers
    {
        public static IEnumerable<Attribute> GetAttrs(this Type t, Type attrType, bool inherit)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            if (attrType == null) throw new ArgumentNullException(nameof(attrType));

#if NETSTANDARD
            return t.GetTypeInfo().GetCustomAttributes(attrType, inherit);
#else
            return (t.GetCustomAttributes(attrType, inherit) ?? new object[0]).Cast<Attribute>();
#endif
        }

        public static Assembly GetAssembly(this Type t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));

#if NETSTANDARD
            return t.GetTypeInfo().Assembly;
#else
            return t.Assembly;
#endif
        }
    }
}