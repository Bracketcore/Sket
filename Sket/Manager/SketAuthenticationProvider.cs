using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Repository;
using Bracketcore.Sket.Responses;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;

namespace Bracketcore.Sket.Manager
{
    public abstract class SketAuthenticationProvider : AuthenticationStateProvider, ISketAuthenticationProvider
    {
        private readonly ILocalStorageService _localstorage;
        private readonly ISketAccessTokenRepository<SketAccessTokenModel> _accessToken;
        public CancellationToken CancellationToken { get; set; }


        public SketAuthenticationProvider(ILocalStorageService localstorage,
            ISketAccessTokenRepository<SketAccessTokenModel> accessToken)
        {
            _localstorage = localstorage;
            _accessToken = accessToken;
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
            }, "apiAuth");

            user = new ClaimsPrincipal(identity);

            return await Task.FromResult(new AuthenticationState(user));
        }

        public async Task LoginUser<T>(T loginData, string Token) where T : SketUserModel
        {
            loginData.Password = String.Empty;
            ;
            var verifyUser = JsonConvert.SerializeObject(loginData);

            string RoleValue = string.Join(",", values: loginData.Role);

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, loginData.Email),
                new Claim(ClaimTypes.NameIdentifier, loginData.ID),
                new Claim("Profile", verifyUser),
                new Claim("Token", Token),
                new Claim(ClaimTypes.Role, RoleValue)
            }, "apiauth_type");

            await _localstorage.SetItemAsync("Token", Token);
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void LogOutUser()
        {
            _localstorage.RemoveItemAsync("Token");
        
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