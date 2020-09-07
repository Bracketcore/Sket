using Bracketcore.Sket.Entity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Bracketcore.Sket.Manager
{
    public class JwtManager<T> : IJwtManager where T : SketUserModel
    {
        private IDataProtector _protector;
        private string _key;

        public JwtManager(IDataProtectionProvider provider, IConfiguration config)
        {
            _protector = provider.CreateProtector(this.GetType().Name.Replace("`1", null));

            _key = config["JwtKey"];
        }
        /// <summary>
        /// Return created token
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<TokenResponse> Authenticate(string username, string password)
        {
            var user = await DB.Queryable<T>().FirstOrDefaultAsync(i => i.Username == username);

            if (user == null) return null;

            var verify = isPasswordOk(password, user.Password);

            if (!verify) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_key);

            var userClaim = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),
            }, "serverAuth");


            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = userClaim,
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new TokenResponse()
            {
                jwt = tokenHandler.WriteToken(token),
                userId = user.ID,
                Claims = new ClaimsPrincipal(userClaim)
            };

        }

        private bool isPasswordOk(string password, string userPassword)
        {
            return password == _protector.Unprotect(userPassword);
        }

        public string HashPassword(string password)
        {
            return _protector.Protect(password);
        }
    }
}
