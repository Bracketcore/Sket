using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UnoRoute.Sket.Core.Entity;

namespace UnoRoute.Sket.Core.Controllers
{
    public interface ISketUserController<T> : ISketBaseController<T>
    {
        public Task<ActionResult<T>> Login(T User);
        public Task<ActionResult<T>> GetCurrentUser();
        public Task Logout(SketUserModel user);
    }
}