// Copyright 2011 Murray Grant
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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using KeePassLib;
using KeePassLib.Cryptography;
using KeePassLib.Cryptography.PasswordGenerator;
using KeePassLib.Security;
using MurrayGrant.ReadablePassphrase;
using MurrayGrant.ReadablePassphrase.Dictionaries;

namespace KeePassReadablePassphrase
{
    /// <summary>
    /// Passphrase generator as a KeePass plugin.
    /// </summary>
    public sealed class PassphraseGenerator : CustomPwGenerator, IDisposable
    {
        private const string DictionaryResourceName = "KeePassReadablePassphrase.dictionary.xml";
        public KeePass.Plugins.IPluginHost Host { get; private set; }

        public override string Name
        {
            get { return "Readable Passphrase Generator"; }
        }

        private static readonly PwUuid _Uuid = new PwUuid(new Guid("cb3582ee-a00d-48e2-be30-e925040f2124").ToByteArray());
        public override PwUuid Uuid
        {
            get { return _Uuid; }
        }

        public override bool SupportsOptions
        {
            get { return true; }
        }

        public PassphraseGenerator() { }
        public PassphraseGenerator(KeePass.Plugins.IPluginHost host)
        {
            this.Host = host;
        }

        public override string GetOptions(string strCurrentOptions)
        {
            using (var frm = new ConfigRoot(strCurrentOptions, new KeePassRandomSource()))
            {
                frm.ShowDialog(this.Host.MainWindow);
                return frm.ConfigForKeePass;
            }
        }

        public override ProtectedString Generate(PwProfile prf, CryptoRandomStream crsRandomSource)
        {
            var profile = prf;
            if (profile == null)
            {
                profile = new PwProfile();
            }

            // Load the phrase template from config.
            var conf = new Config(profile.CustomAlgorithmOptions);

            // Use the more portable generator to create our passphrase.
            var generator = new MurrayGrant.ReadablePassphrase.ReadablePassphraseGenerator(new KeePassRandomSource(crsRandomSource));
            LoadDictionary(conf, generator);

            // Using the secure version to keep the passphrase encrypted as much as possible.
            var passphrase = generator.GenerateAsSecure(conf.PhraseDescription, conf.SpacesBetweenWords);
            IntPtr ustr = System.Runtime.InteropServices.Marshal.SecureStringToGlobalAllocUnicode(passphrase);
            try
            {
                // Although the secure string ends up as a string for a short time, it will hopefully be garbage collected in the finally block.
                return new ProtectedString(true, System.Runtime.InteropServices.Marshal.PtrToStringUni(ustr));
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeGlobalAllocUnicode(ustr);
                GC.Collect(0);
            }
        }

        public void Dispose()
        {
            this.Host = null;
            GC.SuppressFinalize(this);
        }

        public static void LoadDictionary(Config conf, MurrayGrant.ReadablePassphrase.ReadablePassphraseGenerator generator)
        {
            var loader = new ExplicitXmlDictionaryLoader();
            if (conf.UseCustomDictionary && !String.IsNullOrEmpty(conf.PathOfCustomDictionary) && System.IO.File.Exists(conf.PathOfCustomDictionary))
            {
                var dict = loader.LoadFrom(conf.PathOfCustomDictionary);
                generator.SetDictionary(dict);
            }
            else
            {
                using (var s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(DictionaryResourceName))
                {
                    var dict = loader.LoadFrom(s);
                    generator.SetDictionary(dict);
                }
            }
        }
    }
}
