﻿// Copyright 2012 Murray Grant
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
using System.Xml;
using MurrayGrant.ReadablePassphrase.Words;

namespace MurrayGrant.ReadablePassphrase.MaterialisedWords
{
    public sealed class MaterialisedInterrogative : Interrogative
    {
        public override string Singular { get; }
        public override string Plural { get; }
        public override IReadOnlyList<string> Tags { get; }

        public MaterialisedInterrogative(string singular, string plural, IReadOnlyList<string> tags)
        {
            Singular = singular;
            Plural = plural;
            Tags = tags;
        }
    }
}
