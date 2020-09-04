using Bracketcore.Sket.Entity;
using Microsoft.AspNetCore.DataProtection;

namespace Bracketcore.Sket.Repository
{
    public class SketRoleRepository<T> : SketBaseRepository<T> where T : SketRoleModel
    {
        public SketRoleRepository(IDataProtectionProvider provider) : base(provider)
        {
        }
    }
}