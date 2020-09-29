using Bracketcore.Sket.Entity;
using System;
using System.Security.Claims;

namespace Bracketcore.Sket.Responses
{
    /// <summary>
    /// Get Login data response.
    /// </summary>
    public class LoginResponse:IDisposable 
    {
        public string Message { get; set; }
        public string Tk { get; set; }
        public DateTime CreatedOn { get; set; }
        // public ClaimsPrincipal ClaimsPrincipal { get; set; }

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