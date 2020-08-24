using Bracketcore.KetAPI.Model;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bracketcore.KetAPI.Stores
{
    public class SketUserStore : IUserStore<SketUserModel>, IUserPasswordStore<SketUserModel>
    {
        private IUserPasswordStore<SketUserModel> _userPasswordStoreImplementation;

        public void Dispose()
        {

        }

        public Task<string> GetUserIdAsync(SketUserModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.ID);
        }

        public Task<string> GetUserNameAsync(SketUserModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Username);
        }

        public Task SetUserNameAsync(SketUserModel user, string userName, CancellationToken cancellationToken)
        {
            user.Username = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(SketUserModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUsername);
        }

        public Task SetNormalizedUserNameAsync(SketUserModel user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUsername = normalizedName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> CreateAsync(SketUserModel user, CancellationToken cancellationToken)
        {
            await DB.SaveAsync(user);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(SketUserModel user, CancellationToken cancellationToken)
        {
            await DB.SaveAsync(user);
            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(SketUserModel user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<SketUserModel> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var d = await DB.Queryable<SketUserModel>().FirstOrDefaultAsync(i => i.ID == userId);
            return d;

        }

        public Task<SketUserModel> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(SketUserModel user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;

            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(SketUserModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(SketUserModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }
    }
}
