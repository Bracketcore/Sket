using Bracketcore.KetAPI.Model;
using Bracketcore.KetAPI.Repository;

namespace Bracketcore.KetAPI.Controllers
{
    public class AccessTokenController: BaseController<AccessTokenModel, AccessTokenRepository>
    {
        public AccessTokenController(AccessTokenRepository repo) : base(repo)
        {
        }
    }
}