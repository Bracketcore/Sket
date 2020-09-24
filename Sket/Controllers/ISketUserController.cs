using System.Threading.Tasks;
using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Bracketcore.Sket.Controllers
{
    public interface ISketUserController<T> : ISketBaseController<T>
    {
        public Task<IActionResult> Login(T User);
        public Task<ActionResult> GetCurrentUser();
        public Task Logout(SketUserModel user);
    }
}