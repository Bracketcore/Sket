using System.Security.Claims;

namespace Bracketcore.Sket.Manager
{
    /// <summary>
    /// Get token response data.
    /// </summary>
    public class TokenResponse
    {
        public string userId { get; set; }
        public string jwt { get; set; }
        public ClaimsPrincipal Claims { get; set; }
    }
}