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
        private readonly ISketAccessTokenRepository<SketAccessTokenModel> _accessTokenRepository;
        public AuthenticationStateProvider _authenticationStateProvider { get; set; }

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
                await ((SketAuthenticationStateProvider<T>) _authenticationStateProvider).LoginUser(User, verify.Tk,
                    HttpContext);
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

        
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("currentUser")]
        public async Task<ActionResult> GetCurrentUser()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(token)) return BadRequest();

            var access = await _accessTokenRepository.FindByToken(token.Replace("Bearer ", null));

            if (access is null) return NotFound();

            var user = await _repo.FindById(access.OwnerID.ID);

            if (user is null) return NotFound();

            user.Password = String.Empty;
            return Ok(user);
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<ActionResult> Logout(SketUserModel user)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(token)) return BadRequest();

            var access = await _accessTokenRepository.FindByToken(token);

            if (access is null) return NotFound();
            await _accessTokenRepository.DestroyByUserId(access.OwnerID.ID);

            await ((SketAuthenticationStateProvider<T>) _authenticationStateProvider).LogOutUser(HttpContext);

            return Ok();
        }

        protected SketUserController(ISketUserRepository<T> repo,
            AuthenticationStateProvider AuthenticationStateProvider,
            ISketAccessTokenRepository<SketAccessTokenModel> accessTokenRepository) : base(repo)
        {
            _repo = repo;
            _accessTokenRepository = accessTokenRepository;
            _authenticationStateProvider = AuthenticationStateProvider;
        }
    }
}