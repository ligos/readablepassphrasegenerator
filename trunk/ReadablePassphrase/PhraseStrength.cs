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
        Normal,
        NormalEqual,
        Strong,
        StrongEqual,
        Insane,
        InsaneEqual,
        Custom
    }
}
