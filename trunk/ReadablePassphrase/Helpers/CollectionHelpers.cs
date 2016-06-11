using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MurrayGrant.ReadablePassphrase.Helpers
{
    public static class CollectionHelpers
    {
        public static T MinOrFallback<T>(this IEnumerable<T> collection, T fallback)
        {
            if (collection.Any())
                return collection.Min();
            else
                return fallback;
        }

        public static T MaxOrFallback<T>(this IEnumerable<T> collection, T fallback)
        {
            if (collection.Any())
                return collection.Max();
            else
                return fallback;
        }
    }
}
