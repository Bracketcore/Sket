using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Repository.Interfaces;

namespace Bracketcore.Sket.Repository
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