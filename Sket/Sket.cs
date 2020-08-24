using System;
using System.Collections.Generic;
using System.Linq;
using Bracketcore.KetAPI.Model;
using MongoDB.Entities;

namespace Bracketcore.KetAPI
{
    public class Sket
    {
        public static IEnumerable<ContextModel<SketPersistedModel>> Context = new List<ContextModel<SketPersistedModel>>();
        public static IEnumerable<RoleModel> Roles = new List<RoleModel>();
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
            var getRoles = DB.Queryable<RoleModel>().ToList();
            var normalRole = Enum.GetValues(typeof(RoleEnum)).Cast<RoleEnum>();

            if (getRoles.Count < normalRole.ToList().Count)
            {
                foreach (var role in normalRole)
                {
                    DB.Save(new RoleModel()
                    {
                        Name = role.ToString()
                    });
                }

            }
        }

    }
}