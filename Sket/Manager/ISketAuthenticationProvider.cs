using System;
using System.Threading.Tasks;
using Bracketcore.Sket.Entity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

namespace Bracketcore.Sket.Manager
{
    public interface ISketAuthenticationStateProvider<T>  : IDisposable where T:SketUserModel
    {
        public  Task LogOutUser(HttpContext httpContext);
        Task LoginUser(T loginData, string Token, HttpContext httpContext);
    }
}