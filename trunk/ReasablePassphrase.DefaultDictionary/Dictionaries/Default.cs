using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace MurrayGrant.ReadablePassphrase.Dictionaries
{
    public static class Default
    {
        private const string DictionaryResourceName = "MurrayGrant.ReadablePassphrase.dictionary.xml.gz";

        public static WordDictionary Load()
        {
            var loader = new ExplicitXmlDictionaryLoader();
            using (var s = typeof(Default).GetTypeInfo().Assembly.GetManifestResourceStream(DictionaryResourceName))
            {
                WordDictionary result = loader.LoadFrom(s);
                return result;
            }
        }
    }
}
