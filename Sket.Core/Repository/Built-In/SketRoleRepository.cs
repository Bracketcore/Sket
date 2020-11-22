using UnoRoute.Sket.Core.Entity;
using UnoRoute.Sket.Core.Repository.Interfaces;

namespace UnoRoute.Sket.Core.Repository
{
    /// <summary>
    ///     Base role repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SketRoleRepository<T> : SketBaseRepository<T>, ISketBaseRepository<T> where T : SketRoleModel
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }
    }
}