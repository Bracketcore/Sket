using System;
using System.Threading.Tasks;
using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Responses;

namespace Bracketcore.Sket.Repository
{
    public interface ISketUserRepository<T>: ISketBaseRepository<T> 
    {
        ISketAccessTokenRepository<SketAccessTokenModel> _accessToken { get; set; }
        Task<LoginResponse> Login(T user);
        Task<T> Verify(T user);
        Task<bool> LogOut(T user);
        Task<string> Confirm(string email, string userId, string token);
        void Reset();
        void ChangePassword(string userId, string oldPassword, string newPassword, string resetToken);
        Task<T> FindByUsername(string username);
    }
}