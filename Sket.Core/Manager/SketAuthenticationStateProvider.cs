using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Sket.Core.Models;
using Sket.Core.Repository.Interfaces;

namespace Sket.Core.Manager
{
    /// <summary>
    ///     This is an abstract of the Authentication state provider.
    ///     used for blazor based application
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SketAuthenticationStateProvider<T> : AuthenticationStateProvider,
        ISketAuthenticationStateProvider<T>
        where T : SketUserModel
    {
        private readonly ISketAccessTokenRepository<SketAccessTokenModel> _accessToken;
        private readonly ILocalStorageService _localstorage;
        private readonly ISketUserRepository<T> _userRepository;
        private IHttpContextAccessor _HttpContext;

        public SketAuthenticationStateProvider(ILocalStorageService localstorage,
            ISketAccessTokenRepository<SketAccessTokenModel> accessToken, ISketUserRepository<T> userRepository)
        {
            _localstorage = localstorage;
            _accessToken = accessToken;
            _userRepository = userRepository;
        }

        public CancellationToken CancellationToken { get; set; }

        public async Task LoginUser(string token)
        {
            try
            {
                var u = new ClaimsPrincipal();

                var getToken = await _accessToken.FindByToken(token);

                var loggedUser = await _userRepository.FindById(getToken.OwnerId.ID);

                loggedUser.Password = string.Empty;

                var verifyUser = JsonConvert.SerializeObject(loggedUser);

                var roleValue = string.Join(",", loggedUser.Role);

                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, loggedUser.Email),
                    new Claim(ClaimTypes.NameIdentifier, loggedUser.ID),
                    new Claim("Profile", verifyUser),
                    new Claim("Token", token),
                    new Claim(ClaimTypes.Role, roleValue)
                }, Init.Sket.Cfg.Settings.AuthType.ToString());

                var user = new ClaimsPrincipal(identity);

                // todo work on security here
                // if(_HttpContext.HttpContext is not null)
                //     await _HttpContext.HttpContext.SignInAsync(Init.Sket.Cfg.Settings.AuthType.ToString(), user);


                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task LogOutUser()
        {
            await _HttpContext.HttpContext.SignOutAsync(Init.Sket.Cfg.Settings.AuthType.ToString(),
                new AuthenticationProperties
                {
                    AllowRefresh = true,
                    RedirectUri = "/login"
                });

            var user = new ClaimsPrincipal();
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                // Todo work on the authentication to give user access to the platform
                var user = new ClaimsPrincipal();

                var getToken = await _localstorage.GetItemAsync<string>("Token");

                if (getToken == null) return await Task.FromResult(new AuthenticationState(user));

                var tokenExist = await _accessToken.FindByToken(getToken);

                if (DateTime.Now.Ticks > tokenExist.Ttl.Ticks)
                {
                    await _localstorage.ClearAsync();
                    await _accessToken.DestroyByUserId(tokenExist.ID);
                    return await Task.FromResult(new AuthenticationState(user));
                }

                var getUser = await _userRepository.FindById(tokenExist.OwnerId.ID);

                if (getUser is null) return await Task.FromResult(new AuthenticationState(user));

                getUser.Password = string.Empty;

                var roleValue = string.Join(",", getUser.Role);

                var identity = new ClaimsIdentity(new[]
                {
                    new Claim("Profile", JsonConvert.SerializeObject(getUser)),
                    new Claim(ClaimTypes.Email, getUser.Email),
                    new Claim(ClaimTypes.NameIdentifier, getUser.ID),
                    new Claim("Token", getToken),
                    new Claim(ClaimTypes.Role, roleValue)
                }, "SketAuth");

                user = new ClaimsPrincipal(identity);

                return await Task.FromResult(new AuthenticationState(user));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await _localstorage.ClearAsync();
                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}