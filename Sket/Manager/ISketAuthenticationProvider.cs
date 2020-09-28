using System;
using System.Threading.Tasks;
using Bracketcore.Sket.Entity;
using Microsoft.AspNetCore.Components.Authorization;

namespace Bracketcore.Sket.Manager
{
    public interface ISketAuthenticationProvider  :IDisposable
    {
        public void LogOutUser();
        public Task<AuthenticationState> GetAuthenticationStateAsync();
        Task LoginUser<T>(T loginData, string Token) where T : SketUserModel;
    }
}