using System;
using System.Collections.Generic;
using System.Linq;
using Bracketcore.KetAPI.Model;
using MongoDB.Entities;

namespace Bracketcore.KetAPI
{
    public class Sket
    {
        public static IEnumerable<SketContextModel<SketPersistedModel>> Context = new List<SketContextModel<SketPersistedModel>>();
        public static IEnumerable<SketRoleModel> Roles = new List<SketRoleModel>();
        public static List<Type> _context;

        public Sket()
        {
            SetupRoles();
            GetModelContext();
        }

        private void GetModelContext()
        {
            var type = typeof(SketPersistedModel);
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));

            _context = types.ToList();
        }

        private void SetupRoles()
        {
            // Setup roles
            var getRoles = DB.Queryable<SketRoleModel>().ToList();
            var normalRole = Enum.GetValues(typeof(SketRoleEnum)).Cast<SketRoleEnum>();

            if (getRoles.Count < normalRole.ToList().Count)
            {
                foreach (var role in normalRole)
                {
                    DB.Save(new SketRoleModel()
                    {
                        Name = role.ToString()
                    });
                }

            }
        }

    }
}