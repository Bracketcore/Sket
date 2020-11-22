using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UnoRoute.Sket.Core.Entity;

namespace UnoRoute.Sket.Core.Manager
{
    public interface ISketAuthenticationStateProvider<T> : IDisposable where T : SketUserModel
    {
        public Task LogOutUser(HttpContext httpContext);
        Task LoginUser(string Token, HttpContext httpContext);
    }
}