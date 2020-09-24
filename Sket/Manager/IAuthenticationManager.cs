using System.Threading.Tasks;

namespace Bracketcore.Sket.Manager
{
    public interface IAuthenticationManager<T>
    {
        public Task<TokenResponse> Authenticate(T Cred);
        private  bool isPasswordOk(string password, string userPassword)
        {
            throw new System.NotImplementedException();
        }

        public string HashPassword(string password);
    }
}
