using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using Sket.Core.Models;
using Sket.Core.Responses;

namespace Sket.Core.Manager
{
    /// <summary>
    ///     use this to create claims
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SketAuthenticationManager<T> : ISketAuthenticationManager<T>, IDisposable where T : SketUserModel
    {
        public SketAuthenticationManager(IDataProtectionProvider provider)
        {
            _key = Init.Sket.Cfg.Settings.JwtKey;
            _Issuer = Init.Sket.Cfg.Settings.DomainUrl;
            _protector = provider.CreateProtector(new StringBuilder(GetType().Namespace).Append(_key).ToString());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IDataProtector _protector { get; set; }
        public string _key { get; set; }
        public string _Issuer { get; set; }

        /// <summary>
        ///     Return created token
        /// </summary>
        /// <param name="cred"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public virtual async Task<TokenResponse> Authenticate(SketLoginModel cred)
        {
            try
            {
                T user;

                user = await DB.Queryable<T>().FirstOrDefaultAsync(i => i.Username == cred.Username);

                if (user is null)
                {
                    user = await DB.Queryable<T>().FirstOrDefaultAsync(i => i.Email == cred.Email);

                    if (user is null)
                    {
                        user = await DB.Queryable<T>().FirstOrDefaultAsync(i => i.Phone == cred.Phone);

                        if (user is null) throw new Exception("Invalid Credentials");
                    }
                }


                var verify = isPasswordOk(cred.Password, user.Password);

                if (!verify) throw new Exception("Invalid Password");
                ;

                switch (Init.Sket.Cfg.Settings.AuthType.ToString())
                {
                    case CookieAuthenticationDefaults.AuthenticationScheme:
                        return await GenerateCookieToken(user);

                    default:
                        return await GenerateJSONWebToken(user);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public bool isPasswordOk(string password, string userPassword)
        {
            return _protector.Unprotect(userPassword).Equals(password);
        }

        /// <summary>
        ///     Hash password from a given string
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string HashPassword(string password)
        {
            return _protector.Protect(password);
        }

        public Task<TokenResponse> GenerateJSONWebToken(T userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = GenerateClaims(userInfo);
            var userClaim = new ClaimsIdentity(claims, Init.Sket.Cfg.Settings.AuthType.ToString());

            var token = new JwtSecurityToken(_Issuer,
                _Issuer,
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            var key = new JwtSecurityTokenHandler().WriteToken(token);

            return Task.FromResult(new TokenResponse
            {
                jwt = key,
                userId = userInfo.ID,
                Claims = new ClaimsPrincipal(userClaim)
            });
        }

        public Task<TokenResponse> GenerateCookieToken(T user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_key);

            //todo auth schema
            var claims = GenerateClaims(user);
            var userClaim = new ClaimsIdentity(claims, Init.Sket.Cfg.Settings.AuthType.ToString());

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = userClaim,
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var key = tokenHandler.WriteToken(token);

            return Task.FromResult(new TokenResponse
            {
                jwt = key,
                userId = user.ID,
                Claims = new ClaimsPrincipal(userClaim)
            });
        }

        private Claim[] GenerateClaims(T user)
        {
            return new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}