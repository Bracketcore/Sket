using Bracketcore.Sket.Entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Bracketcore.Sket.Manager
{
    public interface ISketAuthenticationStateProvider<T> : IDisposable where T : SketUserModel
    {
        public Task LogOutUser(HttpContext httpContext);
        Task LoginUser(T loginData, string Token, HttpContext httpContext);
    }
}