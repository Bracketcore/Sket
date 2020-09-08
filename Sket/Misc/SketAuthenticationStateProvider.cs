using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Responses;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bracketcore.Sket.Misc
{
    public class SketAuthenticationStateProvider : AuthenticationStateProvider
    {
        [CascadingParameter] public AppStateBase DataShare { get; set; }
        [Inject] private ILocalStorageService _ss { get; set; }


        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Todo work on the authentication to give user access to the platform
            var user = new ClaimsPrincipal();

            var getToken = await _ss.GetItemAsync<string>("Token");

            if (getToken == null) return await Task.FromResult(new AuthenticationState(user));


            using (var httpclient = new HttpClient())
            {
                var tokenExist = await httpclient.GetAsync($"{DataShare.BaseApiUrl}/tokens/{getToken}");
                tokenExist.EnsureSuccessStatusCode();
                var tokenValue =
                    JsonConvert.DeserializeObject<DataResponse<SketAccessTokenModel>>(
                        await tokenExist.Content.ReadAsStringAsync());



                if (tokenValue.Data == null) return await Task.FromResult(new AuthenticationState(user));


                var getUser = await tokenValue.Data.OwnerID.ToEntityAsync();


                getUser.Password = String.Empty;

                string RoleValue = string.Join(",", values: getUser.Role);

                var identity = new ClaimsIdentity(new[]
                {
                new Claim("Profile", JsonConvert.SerializeObject(getUser)),
                new Claim(ClaimTypes.Email, getUser.Email),
                new Claim(ClaimTypes.NameIdentifier, getUser.ID),
                new Claim("Token", getToken),
                new Claim(ClaimTypes.Role,  RoleValue)
            }, "apiAuth");

                user = new ClaimsPrincipal(identity);

                return await Task.FromResult(new AuthenticationState(user));
            }



        }

        //public async Task LoginUser(LoginResponse loginData)
        //{
        //    var verifyUser = JsonConvert.SerializeObject(loginData.UserInfo);

        //    string RoleValue = string.Join(",", values: loginData.UserInfo.Role);

        //    var identity = new ClaimsIdentity(new[]
        //    {
        //        new Claim(ClaimTypes.Email, loginData.UserInfo.Email),
        //        new Claim(ClaimTypes.NameIdentifier, loginData.UserInfo.ID),
        //        new Claim("Profile", verifyUser),
        //        new Claim("Token", loginData.Tk),
        //        new Claim(ClaimTypes.Role, RoleValue)
        //    }, "apiauth_type");

        //    await _ss.SetItemAsync("Token", loginData.Tk);
        //    var user = new ClaimsPrincipal(identity);
        //    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        //}

        //public void LogOutUser()
        //{
        //    _ss.RemoveItemAsync("Token");

        //    var user = new ClaimsPrincipal();
        //    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        //}
    }
}
