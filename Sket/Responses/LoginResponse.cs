using Bracketcore.Sket.Entity;
using System;
using System.Security.Claims;

namespace Bracketcore.Sket.Responses
{
    /// <summary>
    /// Get Login data response.
    /// </summary>
    public class LoginResponse : SketPersistedModel
    {
        public string Message { get; set; }
        public string Tk { get; set; }
        public DateTime CreatedOn { get; set; }
        public ClaimsPrincipal ClaimsPrincipal { get; set; }
    }
}