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

        public PassphraseGenerator(KeePass.Plugins.IPluginHost host)
        {
            this.Host = host;
        }

        private Config _ConfigForCachedDictionary;
        private WordDictionary _CachedDictionary;

        public override string GetOptions(string strCurrentOptions)
        {
            if (Host == null)
                return "";

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

            // Create and cache the dictionary.
            // Important note: do not cache the CryptoRandomStream or ReadablePassphraseGenerator
            //    If you do, the CryptoRandomStream is disposed after the method returns, and you end up with very deterministic random numbers.
            //    This can manifest itself as the name sandom words are generated in the Preview tab in KeeyPass's Generate Password form.
            //    OR in more recent version of KeePass, you get an ObjectDisposedException
            var dict = GetDictionary(conf);
            var generator = new MurrayGrant.ReadablePassphrase.ReadablePassphraseGenerator(dict, new KeePassRandomSource(crsRandomSource));

            if (conf.Mutator != MutatorOption.None
                || conf.CountBy == CountByOption.Words)
                return GenerateForMutators(generator, conf);
            else if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return GenerateSecure(generator, conf);
            else
                return GenerateNotSoSecure(generator, conf);
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
                    passphrase = generator.GenerateAsSecure(conf.PhraseDescription, conf.ActualSeparator);
                else
                    passphrase = generator.GenerateAsSecure(conf.PhraseStrength, conf.ActualSeparator);
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
                        passphrase = generator.GenerateAsUtf8Bytes(conf.PhraseDescription, conf.ActualSeparator);
                    else
                        passphrase = generator.GenerateAsUtf8Bytes(conf.PhraseStrength, conf.ActualSeparator);

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
                    // This always includes a space delimiter, and removes them at a later stage after the mutators are applied.
                    // We use a space delimiter as the constant because the mutators depend on actual whitespace between words.
                    var passphrase = "";
                    if (conf.PhraseStrength == PhraseStrength.Custom)
                        passphrase = generator.Generate(conf.PhraseDescription, " ", GetMutators(conf).ToList());
                    else
                        passphrase = generator.Generate(conf.PhraseStrength, " ", GetMutators(conf).ToList());

                    // It's now safe to remove whitespace.
                    if (conf.WordSeparator == WordSeparatorOption.None)
                        passphrase = new string(passphrase.Where(c => !Char.IsWhiteSpace(c)).ToArray());
                    else if (conf.WordSeparator == WordSeparatorOption.Space) {
                        // No op.
                    } else if (conf.WordSeparator == WordSeparatorOption.Custom)
                        passphrase = passphrase.Replace(" ", conf.CustomSeparator);

                    if (PhraseIsWithinLengthCriteria(passphrase, conf))
                        return new ProtectedString(true, passphrase);
                    // Bail out if we've tried lots of times.
                    if (attempts <= 0)
                        return new ProtectedString(true, "Unable to find a passphrase meeting the min and max length criteria in your settings.");
                }
                finally
                {
                    // I live in the slim hope that the the GC will actually clear the string(s) we generated.
                    GC.Collect(0);
                }
            } while (true);
        }

        private IEnumerable<IMutator> GetMutators(Config conf)
        {
            if (conf.Mutator == MutatorOption.None)
                yield break;
            else if (conf.Mutator == MutatorOption.Standard)
            {
                yield return UppercaseWordMutator.Basic;
                yield return NumericMutator.Basic;
                yield return ConstantMutator.Basic;
            }
            else if (conf.Mutator == MutatorOption.Custom)
            {
                if (conf.UpperStyle == AllUppercaseStyles.Anywhere || conf.UpperStyle == AllUppercaseStyles.StartOfWord)
                    yield return new UppercaseMutator() { When = (UppercaseStyles)conf.UpperStyle, NumberOfCharactersToCapitalise = conf.UpperCount };
                else if (conf.UpperStyle == AllUppercaseStyles.RunOfLetters)
                    yield return new UppercaseRunMutator() { NumberOfRuns = conf.UpperCount };
                else if (conf.UpperStyle == AllUppercaseStyles.WholeWord)
                    yield return new UppercaseWordMutator() { NumberOfWordsToCapitalise = conf.UpperCount };

                if (conf.NumericStyle != NumericStyles.Never)
                    yield return new NumericMutator() { When = conf.NumericStyle, NumberOfNumbersToAdd = conf.NumericCount };

                if (conf.ConstantStyle != ConstantStyles.Never)
                    yield return new ConstantMutator() { When = conf.ConstantStyle, ValueToAdd = conf.ConstantValue };
            }
        }

        public void Dispose()
        {
            this._CachedDictionary = null;
            this.Host = null;
            GC.SuppressFinalize(this);
        }

        public WordDictionary GetDictionary(Config conf)
        {
            if (!ConfigValidForCachedDictionary(_ConfigForCachedDictionary, conf))
            {
                _CachedDictionary = LoadDictionary(conf);
                _ConfigForCachedDictionary = conf;
            }
            return _CachedDictionary;
        }
        private static bool ConfigValidForCachedDictionary(Config cacheConf, Config thisConf)
        {
            if (cacheConf == null || thisConf == null)
                return false;
            return cacheConf.UseCustomDictionary == thisConf.UseCustomDictionary
                && cacheConf.PathOfCustomDictionary == thisConf.PathOfCustomDictionary;
        }
        public static WordDictionary LoadDictionary(Config conf)
        {
            var loader = new ExplicitXmlDictionaryLoader();
            WordDictionary result;
            if (conf.UseCustomDictionary && !String.IsNullOrEmpty(conf.PathOfCustomDictionary) && System.IO.File.Exists(conf.PathOfCustomDictionary))
            {
                result = loader.LoadFrom(conf.PathOfCustomDictionary);
            }
            else
            {
                using (var s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(DictionaryResourceName))
                {
                    result = loader.LoadFrom(s);
                }
            }
            return result;
        }

        static bool PhraseIsWithinLengthCriteria(string phrase, Config conf)
        {
            if (conf.CountBy == CountByOption.Words && conf.ActualSeparator == "")
                return true;
            else if (conf.CountBy == CountByOption.Words && conf.ActualSeparator != "")
            {
                var separator = new[] { conf.ActualSeparator };
                var words = phrase.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                return words.Length >= conf.MinLength && words.Length <= conf.MaxLength;
            }
            else
                return phrase.Length >= conf.MinLength && phrase.Length <= conf.MaxLength;
        }
    }
}
