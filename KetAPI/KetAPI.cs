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
          var getRoles = await  DB.Queryable<RoleModel>().ToListAsync();
          var normalRole =  RoleEnum.GetValues(typeof(RoleEnum)).Cast<RoleEnum>();

          if (getRoles.Count <= normalRole.ToList().Count)
          {
              foreach (var role in normalRole)
              {
                 await DB.SaveAsync(new RoleModel()
                  {
                      Name = role.ToString()
                  });
              }
              
          }
      
        }
    }
}