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
using System.Security;
using MurrayGrant.ReadablePassphrase.Mutators;

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

        private MurrayGrant.ReadablePassphrase.ReadablePassphraseGenerator _CachedGenerator;

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

            // Create the passphrase generator.
            if (_CachedGenerator == null)
            {
                var generator = new MurrayGrant.ReadablePassphrase.ReadablePassphraseGenerator(new KeePassRandomSource(crsRandomSource));
                LoadDictionary(conf, generator);
                _CachedGenerator = generator;
            }

            if (conf.Mutator != MutatorOption.None)
                return GenerateForMutators(_CachedGenerator, conf);
            else if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return GenerateSecure(_CachedGenerator, conf);
            else
                return GenerateNotSoSecure(_CachedGenerator, conf);
        }

        private ProtectedString GenerateSecure(MurrayGrant.ReadablePassphrase.ReadablePassphraseGenerator generator, Config conf)
        {
            int attempts = 1000;
            // Using the secure version to keep the passphrase encrypted as much as possible.
            SecureString passphrase = new SecureString();
            do
            {
                attempts--;
                if (conf.PhraseStrength == PhraseStrength.Custom)
                    passphrase = generator.GenerateAsSecure(conf.PhraseDescription, conf.SpacesBetweenWords);
                else
                    passphrase = generator.GenerateAsSecure(conf.PhraseStrength, conf.SpacesBetweenWords);
            } while ((passphrase.Length < conf.MinLength || passphrase.Length > conf.MaxLength) && attempts > 0);

            // Bail out if we tried too many times.
            if (attempts <= 0)
                return new ProtectedString(true, "Unable to find a passphrase meeting the min and max length criteria in your settings.");

            IntPtr ustr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(passphrase);
            try
            {
                // Although the secure string ends up as a string for a short time, it will be zeroed in the finally block.
                return new ProtectedString(true, System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ustr));
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ustr);
            }
        }
        private ProtectedString GenerateNotSoSecure(MurrayGrant.ReadablePassphrase.ReadablePassphraseGenerator generator, Config conf)
        {
            var attempts = 1000;
            // This generates the passphrase as UTF8 in a byte[].
            byte[] passphrase = new byte[0];
            do
            {
                attempts--;
                try
                {
                    if (conf.PhraseStrength == PhraseStrength.Custom)
                        passphrase = generator.GenerateAsUtf8Bytes(conf.PhraseDescription, conf.SpacesBetweenWords);
                    else
                        passphrase = generator.GenerateAsUtf8Bytes(conf.PhraseStrength, conf.SpacesBetweenWords);

                    var length = Encoding.UTF8.GetCharCount(passphrase);
                    if (length >= conf.MinLength && length <= conf.MaxLength)
                        return new ProtectedString(true, passphrase);
                    // Bail out if we've tried lots of times.
                    if (attempts <= 0)
                        return new ProtectedString(true, "Unable to find a passphrase meeting the min and max length criteria in your settings.");
                }
                finally
                {
                    // Using the byte[] is better than a String because we can deterministicly overwrite it here with zeros.
                    Array.Clear(passphrase, 0, passphrase.Length);
                }
            } while (true);
        }
        private ProtectedString GenerateForMutators(MurrayGrant.ReadablePassphrase.ReadablePassphraseGenerator generator, Config conf)
        {
            var attempts = 1000;
            // This generates the passphrase as a string, which is bad for security, but lets us use mutators.
            do
            {
                attempts--;
                try
                {
                    // This always includes spaces, and removes them at a later stage after the mutators are applied.
                    var passphrase = "";
                    if (conf.PhraseStrength == PhraseStrength.Custom)
                        passphrase = generator.Generate(conf.PhraseDescription, true, GetMutators(conf));
                    else
                        passphrase = generator.Generate(conf.PhraseStrength, true, GetMutators(conf));

                    // It's now safe to remove whitespace.
                    if (!conf.SpacesBetweenWords)
                        passphrase = new string(passphrase.Where(c => !Char.IsWhiteSpace(c)).ToArray());

                    if (passphrase.Length >= conf.MinLength && passphrase.Length <= conf.MaxLength)
                        return new ProtectedString(true, passphrase);
                    // Bail out if we've tried lots of times.
                    if (attempts <= 0)
                        return new ProtectedString(true, "Unable to find a passphrase meeting the min and max length criteria in your settings.");
                }
                finally
                {
                    // I live in the slim hope that the the GC will actually clear the string we generated.
                    GC.Collect(0);
                }
            } while (true);
        }

        private IEnumerable<IMutator> GetMutators(Config conf)
        {
            if (conf.Mutator == MutatorOption.None)
                return Enumerable.Empty<IMutator>();
            else if (conf.Mutator == MutatorOption.Standard)
                return new IMutator[] { UppercaseWordMutator.Basic, NumericMutator.Basic };
            else if (conf.Mutator == MutatorOption.Custom && conf.UpperStyle > 0 && conf.UpperStyle <= AllUppercaseStyles.Anywhere)
                return new IMutator[] {
                    new UppercaseMutator() { When = (UppercaseStyles)conf.UpperStyle, NumberOfCharactersToCapitalise = conf.UpperCount },
                    new NumericMutator() { When = conf.NumericStyle, NumberOfNumbersToAdd = conf.NumericCount },
                };
            else if (conf.Mutator == MutatorOption.Custom && conf.UpperStyle == AllUppercaseStyles.RunOfLetters)
                return new IMutator[] {
                    new UppercaseRunMutator() { NumberOfRuns = conf.UpperCount },
                    new NumericMutator() { When = conf.NumericStyle, NumberOfNumbersToAdd = conf.NumericCount },
                };
            else if (conf.Mutator == MutatorOption.Custom && conf.UpperStyle == AllUppercaseStyles.WholeWord)
                return new IMutator[] {
                    new UppercaseWordMutator() { NumberOfWordsToCapitalise = conf.UpperCount },
                    new NumericMutator() { When = conf.NumericStyle, NumberOfNumbersToAdd = conf.NumericCount },
                };
            else
                return Enumerable.Empty<IMutator>();
        }
        public void Dispose()
        {
            this._CachedGenerator = null;
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
