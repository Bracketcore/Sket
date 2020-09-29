using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Bracketcore.Sket.Manager;
using Bracketcore.Sket.Responses;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Bracketcore.Sket.Controllers
{
    /// <summary>
    /// Abstract user Controller
    /// </summary>
    /// <typeparam name="T">Model class</typeparam>
    public abstract class SketUserController<T> : SketBaseController<T, ISketUserRepository<T>> where T : SketUserModel
    {
        private ISketUserRepository<T> _repo;
        public ISketAuthenticationStateProvider<T> _authenticationStateProvider { get; set; }

        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(T User)
        {
            var verify = await _repo.Login(User);

            if (verify != null)
            {
                // todo auth schema check
                // await HttpContext.SignInAsync(verify.ClaimsPrincipal);
                await ((SketAuthenticationProvider<T>)_authenticationStateProvider).LoginUser(User, verify.Tk, HttpContext);
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

        [HttpGet("currentUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetCurrentUser()
        {
            await Task.Run(() => { });
            return Ok();
        }

        [HttpPost("logout")]
        public virtual async void Logout(SketUserModel user)
        {
          await  ((SketAuthenticationProvider<T>)_authenticationStateProvider).LogOutUser(HttpContext);
        }

        protected SketUserController(ISketUserRepository<T> repo,
            ISketAuthenticationStateProvider<T> AuthenticationStateProvider) : base(repo)
        {
            _repo = repo;
            _authenticationStateProvider = AuthenticationStateProvider;
        }
    }
}