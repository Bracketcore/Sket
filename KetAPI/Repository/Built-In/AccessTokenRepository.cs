using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using KetAPI.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;

namespace KetAPI.Repository
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class AccessTokenRepository<T> : BaseRepository<T>
    {
        private readonly IConfiguration _config;

        public AccessTokenRepository(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// 
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
                    new Claim(ClaimTypes.Name, userModelInfo.ID),
                    new Claim(ClaimTypes.Email, userModelInfo.Email),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var Tk = tokenHandler.WriteToken(token);
            
            await DB.Collection<Token>()
                .InsertOneAsync(new Token
                {
                    Tk = Tk, 
                    OwnerID = userModelInfo.ID,
                    CustomerId = userModelInfo.ID, 
                });
            return Tk;
            
        }

        public bool VerifyAccessToken(string token)
        {
            var data = Convert.FromBase64String(token);
            var when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
            return when < DateTime.UtcNow.AddDays(-7);
        }

        public override async Task<Token> FindById(string TokenId)
        {
            var search = await DB.Find<Token>().OneAsync(TokenId);

            return search;
        }

        /// <summary>
        /// Get token by token value
        /// </summary>
        /// <param name="Token">Token Value</param>
        /// <returns> returns token and token owner id</returns>
        public async Task<Token> FindByToken(string Token)
        {
            var search = await DB.Queryable<Token>().FirstOrDefaultAsync(i => i.Tk == Token);
            return search;
        }

        public async Task<Token> FindByUserId(string userId)
        {
            var search = await DB.Queryable<Token>().FirstOrDefaultAsync(i => i.OwnerID == userId);
            return search ?? null;
        }

        /// <summary>
        /// Delete token by users id
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<string> DestroyByUserId(string UserId)
        {
            var tokenId = await DB.Queryable<Token>().Where(i => i.OwnerID == UserId).ToListAsync();

            if (tokenId.Count != 0)
            {
                var ls = new List<string>();

                foreach (var tk in tokenId)
                {
                    await DestroyById(tk.ID);
                    ls.Add(tk.ID);
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
            var exist = await DB.Queryable<Token>().FirstOrDefaultAsync(i => i.Tk == Token);

            return exist != null ? true : false;
        }
    }
}