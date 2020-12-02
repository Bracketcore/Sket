using System.Threading.Tasks;
using Sket.Core.Entity;

namespace Sket.Core.Repository.Interfaces
{
    public interface ISketAccessTokenRepository<T>
    {
        string Config { get; set; }
        public Task<string> GenerateToken(SketUserModel userModelInfo);
        public Task<SketAccessTokenModel> Create(string userId, string token);
        public Task<bool> VerifyAccessToken(string token);
        public Task<T> FindById(string tokenId);
        public Task<T> FindByToken(string token);
        public Task<T> FindByUserId(string userId);
        public Task<string> DestroyByUserId(string userId);
        public Task<bool> ExistToken(string token);
    }
}