using Bracketcore.Sket.Entity;

namespace Bracketcore.Sket.Repository
{
    /// <summary>
    /// Base role repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SketRoleRepository<T> : SketBaseRepository<T>, ISketBaseRepository<T> where T : SketRoleModel
    {

    }
}