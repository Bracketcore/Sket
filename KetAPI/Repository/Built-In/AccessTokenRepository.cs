using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Bracketcore.KetAPI.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;

namespace Bracketcore.KetAPI.Repository
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public  class AccessTokenRepository : BaseRepository<AccessTokenModel>
    {
        private readonly IConfiguration _config;

        public AccessTokenRepository(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Creates Token on user login successfully
        /// </summary>
        /// <param name="userModelInfo"></param>
        /// <returns></returns>
        public async Task<string> CreateAccessToken(UserModel userModelInfo)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userModelInfo.ID),
                    new Claim(ClaimTypes.Email, userModelInfo.Email),
                    new Claim(ClaimTypes.Role, (await userModelInfo.Role.ToEntityAsync()).Name),
                }),
                Expires = DateTime.UtcNow.AddDays(14),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var Tk = tokenHandler.WriteToken(token);
            
            await DB.Collection<AccessTokenModel>()
                .InsertOneAsync(new AccessTokenModel()
                { 
                    Tk = Tk, 
                    OwnerID = userModelInfo.ID,
                });
            return Tk;
            
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
            return when < DateTime.UtcNow.AddDays(-14) ;
        }

        public override async Task<AccessTokenModel> FindById(string TokenId)
        {
            var search = await DB.Find<AccessTokenModel>().OneAsync(TokenId);

            return search;
        }

        /// <summary>
        /// Get token by token value
        /// </summary>
        /// <param name="Token">Token Value</param>
        /// <returns> returns token and token owner id</returns>
        public async Task<AccessTokenModel> FindByToken(string Token)
        {
            var search = await DB.Queryable<AccessTokenModel>().FirstOrDefaultAsync(i => i.Tk == Token);
            return search;
        }

        public async Task<AccessTokenModel> FindByUserId(string userId)
        {
            var search = await DB.Queryable<AccessTokenModel>().FirstOrDefaultAsync(i => i.OwnerID.ID == userId);
            return search ?? null;
        }

        /// <summary>
        /// Delete token by users id
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<string> DestroyByUserId(string UserId)
        {
            var tokenId = await DB.Queryable<AccessTokenModel>().Where(i => i.OwnerID.ID == UserId).ToListAsync();

            if (tokenId.Count != 0)
            {
                var ls = new List<string>();

                foreach (var Token in tokenId)
                {
                    await DestroyById(Token.ID);
                    ls.Add(Token.ID);
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
        /// <param name="Token">Token Value</param>
        /// <returns></returns>
        public async Task<bool> ExistToken(string Token)
        {
            var exist = await DB.Queryable<AccessTokenModel>().FirstOrDefaultAsync(i => i.Tk == Token);

            return exist != null ;
        }
    }
}