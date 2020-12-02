using System;

namespace Sket.Core.Responses
{
    /// <summary>
    /// Get Login data response.
    /// </summary>
    public class LoginResponse : IDisposable
    {
        public string Message { get; set; } = "Invalid Credentials";
        public string Tk { get; set; } = null;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public bool IsStatueOk { get; set; } = false;

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