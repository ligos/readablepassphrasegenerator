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
