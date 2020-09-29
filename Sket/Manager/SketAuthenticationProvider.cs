using Blazored.LocalStorage;
using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Bracketcore.Sket.Manager
{
    /// <summary>
    /// This is an abstract of the Authentication state provider.
    /// used for blazor based application 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SketAuthenticationProvider<T> : AuthenticationStateProvider, ISketAuthenticationStateProvider<T>
        where T : SketUserModel
    {
        private readonly ILocalStorageService _localstorage;
        private readonly ISketAccessTokenRepository<SketAccessTokenModel> _accessToken;
        private readonly ISketAuthenticationManager<T> _authMan;
        private readonly ISketUserRepository<T> _userRepository;
        public CancellationToken CancellationToken { get; set; }


        public SketAuthenticationProvider(ILocalStorageService localstorage,
            ISketAccessTokenRepository<SketAccessTokenModel> accessToken, ISketAuthenticationManager<T> AuthMan, ISketUserRepository<T> userRepository)
        {
            _localstorage = localstorage;
            _accessToken = accessToken;
            _authMan = AuthMan;
            _userRepository = userRepository;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Todo work on the authentication to give user access to the platform
            var user = new ClaimsPrincipal();

            var getToken = await _localstorage.GetItemAsync<string>("Token");

            if (getToken == null) return await Task.FromResult(new AuthenticationState(user));

            var tokenExist = await _accessToken.FindByToken(getToken);

            if (tokenExist is null) return await Task.FromResult(new AuthenticationState(user));

            var getUser = await tokenExist.OwnerID.ToEntityAsync(cancellation: CancellationToken);

            getUser.Password = String.Empty;

            string RoleValue = string.Join(",", values: getUser.Role);

            var identity = new ClaimsIdentity(new[]
            {
                new Claim("Profile", JsonConvert.SerializeObject(getUser)),
                new Claim(ClaimTypes.Email, getUser.Email),
                new Claim(ClaimTypes.NameIdentifier, getUser.ID),
                new Claim("Token", getToken),
                new Claim(ClaimTypes.Role, RoleValue)
            }, "SketAuth");

            user = new ClaimsPrincipal(identity);

            return await Task.FromResult(new AuthenticationState(user));
        }

        public async Task LoginUser(T loginData, string Token, HttpContext httpContext)
        {
            try
            {
                var u = new ClaimsPrincipal();

                // var login = await _authMan.Authenticate(loginData);
                //
                // if(login is null)
                // {
                //     await Task.FromResult(new AuthenticationState(u));
                //     return;
                // }
                var GetToken = await _accessToken.FindByToken(Token);

                var LoggedUser = await _userRepository.FindById(GetToken.OwnerID.ID);

                LoggedUser.Password = String.Empty;

                var verifyUser = JsonConvert.SerializeObject(LoggedUser);

                string RoleValue = string.Join(",", values: LoggedUser.Role);

                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, LoggedUser.Email),
                    new Claim(ClaimTypes.NameIdentifier, LoggedUser.ID),
                    new Claim("Profile", verifyUser),
                    new Claim("Token", Token),
                    new Claim(ClaimTypes.Role, RoleValue)
                }, "SketAuth");

                // await _localstorage.SetItemAsync("Token", Token);
                var user = new ClaimsPrincipal(identity);

                await httpContext.SignInAsync(user);

                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task LogOutUser(HttpContext httpContext)
        {
            await _localstorage.RemoveItemAsync("Token");

            await httpContext.SignOutAsync("Bearer",
                new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    RedirectUri = "/login"
                });

            var user = new ClaimsPrincipal();
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        // protected virtual void Dispose(bool disposing)
        // {
        //     if (disposing)
        //     {
        //         _accessToken?.Dispose();
        //     }
        // }
        //
        // public void Dispose()
        // {
        //     Dispose(true);
        //     GC.SuppressFinalize(this);
        // }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}