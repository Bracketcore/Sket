using System;
using System.Security.Claims;

namespace Bracketcore.Sket.Manager
{
    /// <summary>
    /// Get token response data.
    /// </summary>
    public class TokenResponse : IDisposable
    {
        public string userId { get; set; }
        public string jwt { get; set; }
        public ClaimsPrincipal Claims { get; set; }

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