using System.Linq;
using System.Threading.Tasks;
using Bracketcore.KetAPI.Model;
using MongoDB.Driver;
using MongoDB.Entities;

namespace Bracketcore.KetAPI
{
    public class KetAPI
    {

        public static  void SetupKet()
        {
            // Setup roles
          var getRoles =   DB.Queryable<RoleModel>().ToListAsync().Result;
          var normalRole =  RoleEnum.GetValues(typeof(RoleEnum)).Cast<RoleEnum>();

          if (getRoles.Count <= normalRole.ToList().Count)
          {
              foreach (var role in normalRole)
              {
                  DB.SaveAsync(new RoleModel()
                  {
                      Name = role.ToString()
                  });
              }
          }
        }
    }
}