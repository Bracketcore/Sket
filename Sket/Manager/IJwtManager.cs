using System.Threading.Tasks;

namespace Bracketcore.Sket.Manager
{
    public interface IJwtManager<T>
    {
        Task<TokenResponse> Authenticate(T Cred);
    }
}
