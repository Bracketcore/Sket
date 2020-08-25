using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Bracketcore.Sket.Model;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;

namespace Bracketcore.Sket.Repository
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AccessTokenRepository : SketBaseRepository<SketAccessTokenModel>
    {
        private string _key;

        public AccessTokenRepository(string key)
        {
            this._key = key;
        }

        /// <summary>
        /// Creates Token on user login successfully
        /// </summary>
        /// <param name="userModelInfo"></param>
        /// <returns></returns>
        public async Task<string> CreateAccessToken(SketUserModel userModelInfo)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_key);
            var ttl = DateTime.UtcNow.AddDays(7);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userModelInfo.ID),
                    new Claim(ClaimTypes.Role, userModelInfo.Role.ToString()),
                }),
                Expires = ttl,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tk = tokenHandler.WriteToken(token);

            await DB.Collection<SketAccessTokenModel>()
                .InsertOneAsync(new SketAccessTokenModel()
                {
                    Tk = tk,
                    Ttl = ttl,
                    OwnerID = userModelInfo.ID,
                });
            return tk;

        }

        /// <summary>
        /// Verify if the token exist and valid
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> VerifyAccessToken(string token)
        {
            var find = (await ExistToken(token));

            if (find) return false;

            var data = Convert.FromBase64String(token);
            var when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
            return when < DateTime.UtcNow.AddDays(-14);
        }

        public override async Task<SketAccessTokenModel> FindById(string tokenId)
        {
            var search = await DB.Find<SketAccessTokenModel>().OneAsync(tokenId);

            return search;
        }

        /// <summary>
        /// Get token by token value
        /// </summary>
        /// <param name="token">Token Value</param>
        /// <returns> returns token and token owner id</returns>
        public async Task<SketAccessTokenModel> FindByToken(string token)
        {
            var search = await DB.Queryable<SketAccessTokenModel>().FirstOrDefaultAsync(i => i.Tk == token);
            return search;
        }

        public async Task<SketAccessTokenModel> FindByUserId(string userId)
        {
            var search = await DB.Queryable<SketAccessTokenModel>().FirstOrDefaultAsync(i => i.OwnerID.ID == userId);
            return search ?? null;
        }

        /// <summary>
        /// Delete token by users id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<string> DestroyByUserId(string userId)
        {
            var tokenId = await DB.Queryable<SketAccessTokenModel>().Where(i => i.OwnerID.ID == userId).ToListAsync();

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
            else
            {
                return "Error Id not found";
            }
        }

        /// <summary>
        /// Check if token exist by the value of the token
        /// </summary>
        /// <param name="token">Token Value</param>
        /// <returns></returns>
        public async Task<bool> ExistToken(string token)
        {
            var exist = await DB.Queryable<SketAccessTokenModel>().FirstOrDefaultAsync(i => i.Tk == token);

            return exist != null;
        }
    }
}