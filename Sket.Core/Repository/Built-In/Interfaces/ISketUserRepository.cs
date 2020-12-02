using System.Threading.Tasks;
using Sket.Core.Entity;
using Sket.Core.Responses;

namespace Sket.Core.Repository.Interfaces
{
    public interface ISketUserRepository<T> : ISketBaseRepository<T>
    {
        ISketAccessTokenRepository<SketAccessTokenModel> _accessToken { get; set; }
        Task<LoginResponse> Login(SketLoginModel user);
        Task<T> Verify(T user);
        Task<bool> LogOut(T user);
        Task<string> Confirm(string email, string userId, string token);
        void Reset();
        void ChangePassword(string userId, string oldPassword, string newPassword, string resetToken);
        Task<T> FindByUsername(string username);
    }
}