using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bracketcore.Sket.Controllers
{
    public interface ISketUserController<T> : ISketBaseController<T>
    {
        public Task<ActionResult<T>> Login(T User);
        public Task<ActionResult<T>> GetCurrentUser();
        public Task Logout(SketUserModel user);
    }
}