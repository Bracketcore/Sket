using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Bracketcore.KetAPI.Model;
using Bracketcore.KetAPI.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Bracketcore.KetAPI.Controllers
{
    public class UserController : BaseController<
    UserModel, UserRepository<
    UserModel>>
    {
        private readonly UserRepository<
        UserModel> _repo;
        public UserController(UserRepository<
        UserModel> repo) : base(repo)
        {
            _repo = repo;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public virtual async Task<IActionResult> Login([FromBody] 
        UserModel 
        User)
        {
            var verify = await _repo.Login(
            User);
            
            var cred = new ClaimsPrincipal();

            if (verify == null)
            {
                return Unauthorized();
            }
            
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("Profile", JsonConvert.SerializeObject(verify.UserInfo)),
                new Claim(ClaimTypes.Email, verify.UserInfo.Email),
                new Claim(ClaimTypes.NameIdentifier, verify.UserInfo.ID),
                new Claim("Token", verify.Tk),
                new Claim(ClaimTypes.Role,  JsonSerializer.Serialize(verify.UserInfo.Role))
            }, CookieAuthenticationDefaults.AuthenticationScheme);
            
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties() {IsPersistent = true});

            // LocalRedirect();

            return Ok(verify);
        }

        [HttpPost("logout")]
        public virtual async Task Logout([FromBody] 
        UserModel user)
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    RedirectUri = "/"
                });
        }

      
    }
}