using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Repository;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace Bracketcore.Sket.Stores
{
    public class UserStore<T> : IUserStore<T>, IUserPasswordStore<T> where T : SketUserModel
    {
        private SketUserRepository<T> _repo;

        public UserStore(SketUserRepository<T> repo)
        {
            this._repo = repo;
        }
        public void Dispose()
        {
        }

        public async Task<string> GetUserIdAsync(T user, CancellationToken cancellationToken)
        {
            return await Task.FromResult(user.ID);
        }

        public async Task<string> GetUserNameAsync(T user, CancellationToken cancellationToken)
        {
            return await Task.FromResult(user.Username);
        }

        public async Task SetUserNameAsync(T user, string userName, CancellationToken cancellationToken)
        {
            user.Username = userName;
            await Task.CompletedTask;
        }

        public async Task<string> GetNormalizedUserNameAsync(T user, CancellationToken cancellationToken)
        {
            return await Task.FromResult(user.NormalizedUsername);
        }

        public async Task SetNormalizedUserNameAsync(T user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUsername = normalizedName;
            await Task.CompletedTask;
        }

        public async Task<IdentityResult> CreateAsync(T user, CancellationToken cancellationToken)
        {
            var u = await _repo.Create(user);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(T user, CancellationToken cancellationToken)
        {
            await _repo.Update(user.ID, user);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(T user, CancellationToken cancellationToken)
        {
            await _repo.DestroyById(user.ID);
            return IdentityResult.Success;
        }

        public async Task<T> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _repo.FindById(userId);
            return user;
        }

        public async Task<T> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = await _repo.FindByUsername(normalizedUserName);
            return user;
        }

        public async Task SetPasswordHashAsync(T user, string passwordHash, CancellationToken cancellationToken)
        {
            var hash = SketUserRepository<T>.HashPassword(passwordHash);
            await Task.CompletedTask;
        }

        public async Task<string> GetPasswordHashAsync(T user, CancellationToken cancellationToken)
        {
            return await Task.FromResult(user.PasswordHash);
        }

        public async Task<bool> HasPasswordAsync(T user, CancellationToken cancellationToken)
        {
            return await Task.FromResult(user.PasswordHash != null);
        }
    }
}
