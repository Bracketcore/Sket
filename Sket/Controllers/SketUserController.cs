using System;
using System.Threading.Tasks;
using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Manager;
using Bracketcore.Sket.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bracketcore.Sket.Controllers
{
    /// <summary>
    ///     Abstract user Controller
    /// </summary>
    /// <typeparam name="T">Model class</typeparam>
    public abstract class SketUserController<T> : SketBaseController<T, ISketUserRepository<T>> where T : SketUserModel
    {
        private readonly ISketAccessTokenRepository<SketAccessTokenModel> _accessTokenRepository;
        private readonly ISketUserRepository<T> _repo;

        protected SketUserController(ISketUserRepository<T> repo,
            AuthenticationStateProvider AuthenticationStateProvider,
            ISketAccessTokenRepository<SketAccessTokenModel> accessTokenRepository) : base(repo)
        {
            _repo = repo;
            _accessTokenRepository = accessTokenRepository;
            _authenticationStateProvider = AuthenticationStateProvider;
        }

        public AuthenticationStateProvider _authenticationStateProvider { get; set; }

        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("login")]
        public async Task<ActionResult<T>> Login(T User)
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

            return BadRequest(new
            {
                Message = "Invalid Credentials",
                Status = "Error"
            });
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("currentUser")]
        public async Task<ActionResult<T>> GetCurrentUser()
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(token)) return BadRequest();

                var access = await _accessTokenRepository.FindByToken(token.Replace("Bearer ", null));

                if (access is null) return NotFound();

                var user = await _repo.FindById(access.OwnerID.ID);

                if (user is null) return NotFound();

                user.Password = string.Empty;
                return Ok(user);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<ActionResult<T>> Logout(SketUserModel user)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(token)) return BadRequest();

            var access = await _accessTokenRepository.FindByToken(token);

            if (access is null) return NotFound();

            await _accessTokenRepository.DestroyByUserId(access.OwnerID.ID);

            await ((SketAuthenticationStateProvider<T>) _authenticationStateProvider).LogOutUser(HttpContext);

            return Ok();
        }
    }
}