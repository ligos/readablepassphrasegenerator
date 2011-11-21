using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace MurrayGrant.ReadablePassphrase.Random
{
    /// <summary>
    /// The default random source uses the <c>RNGCryptoServiceProvider</c>.
    /// </summary>
    public class CryptoRandomSource : RandomSourceBase
    {
        private RNGCryptoServiceProvider _RandomProvider;
        public CryptoRandomSource()
        {
            this._RandomProvider = new RNGCryptoServiceProvider();
        }

        public override byte[] GetRandomBytes(int numberOfBytes)
        {
            var result = new byte[numberOfBytes];
            this._RandomProvider.GetBytes(result);
            return result;
        }
    }
}
