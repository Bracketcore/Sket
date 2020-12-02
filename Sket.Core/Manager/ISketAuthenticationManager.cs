using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Sket.Core.Entity;
using Sket.Core.Responses;

namespace Sket.Core.Manager
{
    public interface ISketAuthenticationManager<T>
    {
        IDataProtector _protector { get; set; }
        string _key { get; set; }
        string _Issuer { get; set; }

        Task<TokenResponse> Authenticate(SketLoginModel Cred);

        bool isPasswordOk(string password, string userPassword);

        string HashPassword(string password);
        Task<TokenResponse> GenerateCookieToken(T user);

        Task<TokenResponse> GenerateJSONWebToken(T userInfo);
    }
}