using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Responses;
using System.Threading.Tasks;

namespace Bracketcore.Sket.Interfaces
{
    public interface ISketUserRepository<T> : ISketBaseRepository<T> where T : SketUserModel
    {
        Task<LoginResponse> Login(T user);
        Task<T> Verify(T user);
        Task<bool> LogOut(T user);
        Task<string> Confirm(string email, string userId, string token);
        void Reset();
        void ChangePassword(string userId, string oldPassword, string newPassword, string resetToken);
        Task<T> FindByUsername(string username);
    }


}
