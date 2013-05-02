// Copyright 2012 Murray Grant
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

namespace MurrayGrant.ReadablePassphrase
{
    /// <summary>
    /// A list of standard passphrase strengths.
    /// </summary>
    /// <remarks>
    /// See <c>PhraseDescription.Clause</c> for the details of each of these.
    /// </remarks>
    public enum PhraseStrength
    {
        Random,
        RandomShort,
        RandomLong,
        RandomForever,
        
        Normal,
        Strong,
        Insane,

        NormalAnd,
        NormalSpeech,
        NormalEqual,
        NormalEqualAnd,
        NormalEqualSpeech,
        NormalRequired,
        NormalRequiredAnd,
        NormalRequiredSpeech,

        StrongAnd,
        StrongSpeech,
        StrongEqual,
        StrongEqualAnd,
        StrongEqualSpeech,
        StrongRequired,
        StrongRequiredAnd,
        StrongRequiredSpeech,
        
        InsaneAnd,
        InsaneSpeech,
        InsaneEqual,
        InsaneEqualAnd,
        InsaneEqualSpeech,
        InsaneRequired,
        InsaneRequiredAnd,
        InsaneRequiredSpeech,

        Custom
    }
}
