using System;
using System.Threading.Tasks;
using Sket.Core.Models;

namespace Sket.Core.Manager
{
    public interface ISketAuthenticationStateProvider<T> : IDisposable where T : SketUserModel
    {
        public Task LogOutUser();
        Task LoginUser(string Token);
    }
}