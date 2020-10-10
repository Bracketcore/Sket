using System.Threading.Tasks;
using Bracketcore.Sket.Responses;
using Microsoft.AspNetCore.DataProtection;

namespace Bracketcore.Sket.Manager
{
    public interface ISketAuthenticationManager<T>
    {
        IDataProtector _protector { get; set; }
        string _key { get; set; }
        string _Issuer { get; set; }

        Task<TokenResponse> Authenticate(T Cred);

        bool isPasswordOk(string password, string userPassword);

        string HashPassword(string password);
        Task<TokenResponse> GenerateCookieToken(T user);

        Task<TokenResponse> GenerateJSONWebToken(T userInfo);
    }
}