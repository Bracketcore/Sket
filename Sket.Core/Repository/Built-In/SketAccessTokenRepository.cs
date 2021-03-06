using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using Sket.Core.Models;
using Sket.Core.Repository.Interfaces;

namespace Sket.Core.Repository
{
    /// <summary>
    ///     Base Access token Repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SketAccessTokenRepository<T> : SketBaseRepository<T>, ISketAccessTokenRepository<T>
        where T : SketAccessTokenModel
    {
        public SketAccessTokenRepository()
        {
            Config = Init.Sket.Cfg.Settings.JwtKey;
        }

        public string Config { get; set; }

        /// <summary>
        ///     Creates Token on user login successfully
        /// </summary>
        /// <param name="userModelInfo"></param>
        /// <returns></returns>
        public virtual async Task<string> GenerateToken(SketUserModel userModelInfo)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Config);
            var ttl = DateTime.UtcNow.AddDays(7);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userModelInfo.ID),
                    new Claim(ClaimTypes.Role, userModelInfo.Role.ToString())
                }),
                Expires = ttl,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tk = tokenHandler.WriteToken(token);

            await DB.Collection<SketAccessTokenModel>()
                .InsertOneAsync(new SketAccessTokenModel
                {
                    Tk = tk,
                    Ttl = ttl,
                    OwnerId = userModelInfo.ID
                });
            return tk;
        }

        public virtual async Task<SketAccessTokenModel> Create(string userId, string token)
        {
            var ttl = DateTime.UtcNow.AddDays(7);
            var access = new SketAccessTokenModel
            {
                Tk = token,
                Ttl = ttl,
                OwnerId = userId
            };

            await DB.SaveAsync(access);
            return access;
        }

        /// <summary>
        ///     Verify if the token exist and valid
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual async Task<bool> VerifyAccessToken(string token)
        {
            var find = await ExistToken(token);

            if (find) return false;

            var data = Convert.FromBase64String(token);
            var when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
            return when < DateTime.UtcNow.AddDays(-14);
        }

        /// <summary>
        ///     Get token by token value
        /// </summary>
        /// <param name="token">Token Value</param>
        /// <returns> returns token and token owner id</returns>
        public virtual async Task<T> FindByToken(string token)
        {
            var search = await DB.Queryable<T>().FirstOrDefaultAsync(i => i.Tk == token);
            return search;
        }

        /// <summary>
        ///     Get user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<T> FindByUserId(string userId)
        {
            var search = await DB.Queryable<T>().FirstOrDefaultAsync(i => i.OwnerId.ID == userId);
            return search ?? null;
        }

        /// <summary>
        ///     Delete token by users id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<string> DestroyByUserId(string userId)
        {
            var tokenId = await DB.Queryable<T>().Where(i => i.OwnerId.ID == userId).ToListAsync();

            if (tokenId.Count != 0)
            {
                var ls = new List<string>();

                foreach (var token in tokenId)
                {
                    await DestroyById(token.ID);
                    ls.Add(token.ID);
                }

                return $"{string.Join(",", ls.ToArray())} Deleted";
            }

            return "Error Id not found";
        }

        /// <summary>
        ///     Check if token exist by the value of the token
        /// </summary>
        /// <param name="token">Token Value</param>
        /// <returns></returns>
        public virtual async Task<bool> ExistToken(string token)
        {
            var exist = await DB.Queryable<T>().FirstOrDefaultAsync(i => i.Tk == token);

            return exist != null;
        }
    }
}