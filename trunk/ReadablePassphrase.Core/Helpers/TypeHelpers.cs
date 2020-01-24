// Copyright 2020 Murray Grant
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
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
