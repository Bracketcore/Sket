﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sket.Core.Manager;
using Sket.Core.Models;
using Sket.Core.Repository.Interfaces;

namespace Sket.Core.Controllers
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


        /// <summary>
        ///     Login user
        /// </summary>
        /// <remarks>
        ///     {
        ///     username:"username",
        ///     email:"your email",
        ///     password:"your password"
        ///     }
        /// </remarks>
        /// <param name="User"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("login")]
        public virtual async Task<ActionResult<T>> Login([FromBody] SketLoginModel User)
        {
            try
            {
                var verify = await _repo.Login(User);


                // todo auth schema check
                // await HttpContext.SignInAsync(verify.ClaimsPrincipal);
                await ((SketAuthenticationStateProvider<T>) _authenticationStateProvider).LoginUser(verify.Tk);
                return Ok(verify);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    message = e.Message,
                    Status = "Error"
                });
                throw;
            }
        }

        /// <summary>
        ///     Get Current Users Details
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("currentUser")]
        public virtual async Task<ActionResult<T>> GetCurrentUser()
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(token)) return BadRequest();

                var access = await _accessTokenRepository.FindByToken(token.Replace("Bearer ", null));

                if (access is null) return NotFound();

                if (access.Ttl.Ticks < DateTime.Now.Ticks)
                {
                    await _accessTokenRepository.DestroyByUserId(access.OwnerId.ID);
                    return NotFound();
                }

                var user = await _repo.FindById(access.OwnerId.ID);

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
        public virtual async Task<ActionResult<T>> Logout([FromBody] T user)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(token)) return BadRequest();

            var access = await _accessTokenRepository.FindByToken(token);

            if (access is null) return NotFound();

            await _accessTokenRepository.DestroyByUserId(access.OwnerId.ID);

            // await ((SketAuthenticationStateProvider<T>) _authenticationStateProvider).LogOutUser(HttpContext);

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }
    }
}