using System;
using System.Security.Cryptography;

namespace Bracketcore.Sket.Misc
{
    /// <summary>
    /// Auto generate JWT secret key
    /// </summary>
    public class JwtSecretKeyGenerator:IDisposable
    {
        /// <summary>
        /// Create auto jwt secret keys
        /// </summary>
        /// <returns></returns>
        public static string Create()
        {
            HMACSHA256 hmac = new HMACSHA256();
            return Convert.ToBase64String(hmac.Key);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
