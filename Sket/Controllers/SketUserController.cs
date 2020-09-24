using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Bracketcore.Sket.Controllers
{
    /// <summary>
    /// Abstract user Controller
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SketUserController<T> : SketBaseController<T, SketUserRepository<T>> where T : SketUserModel
    {
        private SketUserRepository<T> _repo;

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(T User)
        {
            using var verify = await _repo.Login(User);

            if (verify != null)
            {
                // todo auth schema check
                await HttpContext.SignInAsync(
                    Sket.Cfg.Settings.AuthType == AuthType.Cookie
                        ? CookieAuthenticationDefaults.AuthenticationScheme
                        : "", verify.ClaimsPrincipal);
                // Redirect(Url.Content( "~/live"));

                return Ok(verify);
            }
            else
            {
                return BadRequest(new
                {
                    Message = "Invalid Credentials",
                    Status = "Error"
                });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public override Task<IActionResult> Create(T doc)
        {
            return base.Create(doc);
        }

        [HttpGet("currentuser")]
        public async Task<ActionResult> GetCurrentUser()
        {
            await Task.Run(() => { });
            return Ok();
        }

        [HttpPost("logout")]
        public virtual async Task Logout(SketUserModel user)
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