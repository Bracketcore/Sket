using System.Linq;
using System.Threading.Tasks;
using Bracketcore.KetAPI.Model;
using MongoDB.Driver;
using MongoDB.Entities;

namespace Bracketcore.KetAPI
{
    public class KetAPI
    {

        public static async Task SetupKet()
        {
            // Setup roles
          var getRoles = await  DB.Queryable<SketRoleModel>().ToListAsync();
          var normalRole =  SketRoleEnum.GetValues(typeof(SketRoleEnum)).Cast<SketRoleEnum>();

          if (getRoles.Count <= normalRole.ToList().Count)
          {
              foreach (var role in normalRole)
              {
                 await DB.SaveAsync(new SketRoleModel()
                  {
                      Name = role.ToString()
                  });
              }
              
          }
      
        }
    }
}