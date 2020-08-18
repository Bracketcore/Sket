using Bracketcore.KetAPI.Model;
using MongoDB.Driver;
using MongoDB.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bracketcore.KetAPI
{
    internal static class Sket
    {

        internal static async Task SetupSket()
        {
            // Setup roles
            var getRoles = await DB.Queryable<RoleModel>().ToListAsync();
            var normalRole = Enum.GetValues(typeof(RoleEnum)).Cast<RoleEnum>();

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