﻿// Copyright 2011 Murray Grant
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
    public sealed class MaterialisedArticle : Article
    {
        public override string Definite { get; }
        public override string Indefinite { get; }
        public override string IndefiniteBeforeVowel { get; }
        public override IReadOnlyList<string> Tags { get; }

        public MaterialisedArticle(string definite, string indefinite, string indefiniteBeforeVowel, IReadOnlyList<string> tags)
        {
            Definite = definite;
            Indefinite = indefinite;
            IndefiniteBeforeVowel = indefiniteBeforeVowel;
            Tags = tags;
        }
    }
}
