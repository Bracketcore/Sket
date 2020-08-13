using Bracketcore.KetAPI.Model;
using Bracketcore.KetAPI.Repository;

namespace Bracketcore.KetAPI.Controllers
{
    public class SketAccessTokenController: SketBaseController<SketAccessTokenModel, SketAccessTokenRepository>
    {
        public SketAccessTokenController(SketAccessTokenRepository repo) : base(repo)
        {
        }
    }
}