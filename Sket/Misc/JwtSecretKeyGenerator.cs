using System;
using System.Security.Cryptography;

namespace Bracketcore.Sket.Misc
{
    public class JwtSecretKeyGenerator
    {
        public static string Create()
        {
            HMACSHA256 hmac = new HMACSHA256();
            return Convert.ToBase64String(hmac.Key);
        }
    }
}
