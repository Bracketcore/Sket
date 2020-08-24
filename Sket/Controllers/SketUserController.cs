using Bracketcore.KetAPI.Model;
using Bracketcore.KetAPI.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using System.Threading.Tasks;

namespace Bracketcore.KetAPI.Controllers
{
    public abstract class SketUserController<T> : SketBaseController<T, SketUserRepository<T>> where T : SketUserModel
    {
        private SketUserRepository<T> _repo;
        private UserManager<T> userManager;


        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Register(T model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Username);

                if (user == null)
                {
                    user = model;
                    var result = await userManager.CreateAsync(user, model.Password);
                    await DB.SaveAsync(user);
                    return Ok("Successful");
                }

            }

            return NoContent();

        }


        //[AllowAnonymous]
        //[HttpPost("login")]
        //public virtual async Task<IActionResult> Login([FromBody]
        //SketUserModel
        //User)
        //{
        //    var verify = await _repo.Login(
        //    User);

        //    var cred = new ClaimsPrincipal();

        //    if (verify == null)
        //    {
        //        return Unauthorized();
        //    }

        //    var identity = new ClaimsIdentity(new[]
        //    {
        //        new Claim("Profile", JsonConvert.SerializeObject(verify.UserInfo)),
        //        new Claim(ClaimTypes.Email, verify.UserInfo.Email),
        //        new Claim(ClaimTypes.NameIdentifier, verify.UserInfo.ID),
        //        new Claim("Token", verify.Tk),
        //        new Claim(ClaimTypes.Role,  JsonSerializer.Serialize(verify.UserInfo.Role))
        //    }, CookieAuthenticationDefaults.AuthenticationScheme);

        //    var principal = new ClaimsPrincipal(identity);

        //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
        //        new AuthenticationProperties() { IsPersistent = true });

        //    // LocalRedirect();

        //    return Ok(verify);
        //}

        //[HttpPost("logout")]
        //public virtual async Task Logout([FromBody]
        //SketUserModel user)
        //{

        //    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme,
        //        new AuthenticationProperties()
        //        {
        //            AllowRefresh = true,
        //            RedirectUri = "/"
        //        });
        //}


        protected SketUserController(SketUserRepository<T> repo, UserManager<T> userManager) : base(repo)
        {
            _repo = repo;
            this.userManager = userManager;
        }
    }
}