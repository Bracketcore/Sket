using System;
using System.Security.Cryptography;

namespace Sket.Core.Misc
{
    /// <summary>
    ///     Auto generate JWT secret key
    /// </summary>
    public class JwtSecretKeyGenerator : IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Create auto jwt secret keys
        /// </summary>
        /// <returns></returns>
        public static string Create()
        {
            var hmac = new HMACSHA256();
            return Convert.ToBase64String(hmac.Key);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}