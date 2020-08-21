using Bracketcore.Sket.Model;
using MongoDB.Driver;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bracketcore.Sket
{
    public class Sket
    {
        public static IEnumerable<ContextModel<PersistedModel>> Context = new List<ContextModel<PersistedModel>>();
        public static IEnumerable<RoleModel> Roles = new List<RoleModel>();
        public static List<Type> _context;

        public Sket()
        {
            SetupRoles();
            GetModelContext();
        }

        private void GetModelContext()
        {
            var type = typeof(PersistedModel);
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