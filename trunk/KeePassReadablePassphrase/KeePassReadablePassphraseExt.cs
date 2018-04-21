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
using System.Linq;
using System.Text;
using KeePass.Plugins;

namespace KeePassReadablePassphrase
{
    public class KeePassReadablePassphraseExt : Plugin
    {
        private IPluginHost _Host;
        private PassphraseGenerator _Generator;

        public override string UpdateUrl => "https://makemeapassword.ligos.net/keepass_plugins.version.txt";

        public override bool Initialize(IPluginHost host)
        {
            this._Host = host;
            this._Generator = new PassphraseGenerator(host);
            this._Host.PwGeneratorPool.Add(this._Generator);
            return true;
        }

        public override void Terminate()
        {
            if (this._Host != null)
            {
                this._Host.PwGeneratorPool.Remove(this._Generator.Uuid);
                this._Host = null;
            }
            if (this._Generator != null)
            {
                this._Generator.Dispose();
                this._Generator = null;
            }
        }
    }
}
