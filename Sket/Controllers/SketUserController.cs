using Bracketcore.Sket.Model;
using Bracketcore.Sket.Repository;

namespace Bracketcore.Sket.Controllers
{
    public abstract class SketUserController<T> : SketBaseController<T, SketUserRepository<T>> where T : SketUserModel
    {
        private SketUserRepository<T> _repo;


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


        protected SketUserController(SketUserRepository<T> repo) : base(repo)
        {
            _repo = repo;
        }


    }
}