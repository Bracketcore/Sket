﻿using System.Threading.Tasks;
using Bracketcore.Sket.Entity;

namespace Bracketcore.Sket.Repository
{
    public interface ISketAccessTokenRepository<T>
    {
        public Task<string> GenerateToken(SketUserModel userModelInfo);
        public Task Create(string userId, string token);
        public Task<bool> VerifyAccessToken(string token);
        public Task<T> FindById(string tokenId);
        public Task<T> FindByToken(string token);
        public Task<T> FindByUserId(string userId);
        public Task<string> DestroyByUserId(string userId);
        public Task<bool> ExistToken(string token);
    }
}