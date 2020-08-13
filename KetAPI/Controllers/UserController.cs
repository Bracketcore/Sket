using System.Security.Claims;
using System.Threading.Tasks;
using Bracketcore.KetAPI.Model;
using Bracketcore.KetAPI.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Bracketcore.KetAPI.Controllers
{
    public class UserController : BaseController<UserModel, UserRepository<UserModel>>
    {
        private readonly UserRepository<UserModel> _repo;
        public UserController(UserRepository<UserModel> repo) : base(repo)
        {
            _repo = repo;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public virtual async Task<IActionResult> Login([FromBody] UserModel user)
        {
            var verify = await _repo.Login(user);
            
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
                new Claim("Token", verify.TK),
                new Claim(ClaimTypes.Role,  (await verify.UserInfo.Role.ToEntityAsync()).Name)
            }, CookieAuthenticationDefaults.AuthenticationScheme);
            
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties() {IsPersistent = true});

            // LocalRedirect();

            return Ok(verify);
        }
        
        

      
    }
}