using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Bracketcore.Sket.Controllers
{
    public abstract class SketUserController<T> : SketBaseController<T, SketUserRepository<T>>, IDisposable where T : SketUserModel
    {
        private SketUserRepository<T> _repo;

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] T User)
        {

            var verify = await _repo.Login(User);
            await HttpContext.SignInAsync(verify.ClaimsPrincipal);

            return Ok(verify);
        }


        [HttpGet("currentuser")]
        public async Task<ActionResult> GetCurrentUser()
        {
            return Ok();
        }

        [HttpPost("logout")]
        public virtual async Task Logout([FromBody]
        SketUserModel user)
        {

            await HttpContext.SignOutAsync("Bearer",
                new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    RedirectUri = "/"
                });
        }


        protected SketUserController(SketUserRepository<T> repo) : base(repo)
        {
            _repo = repo;

        }


    }
}