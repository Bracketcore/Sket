using System.Threading.Tasks;

namespace Bracketcore.Sket.Manager
{
    public interface IJwtManager
    {
        Task<TokenResponse> Authenticate(string userId, string password);
    }
}
