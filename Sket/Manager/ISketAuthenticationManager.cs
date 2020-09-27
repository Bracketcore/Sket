using System.Threading.Tasks;
using Bracketcore.Sket.Entity;
using Microsoft.AspNetCore.DataProtection;

namespace Bracketcore.Sket.Manager
{
    public interface ISketAuthenticationManager<T>
    {
        IDataProtector _protector { get; set; }
        string _key { get; set; }
        public Task<TokenResponse> Authenticate(T Cred);

        public bool isPasswordOk(string password, string userPassword);

        public string HashPassword(string password);
    }
}