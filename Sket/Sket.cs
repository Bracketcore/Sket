using Bracketcore.Sket.Model;
using Bracketcore.Sket.Repository;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bracketcore.Sket
{
    public class Sket : IDisposable
    {
        public static IEnumerable<SketContextModel<SketPersistedModel>> Context = new List<SketContextModel<SketPersistedModel>>();
        public static IEnumerable<SketRoleModel> Roles = new List<SketRoleModel>();
        public static List<Type> _context;
        private readonly SketRoleRepository<SketRoleModel> _sketRoleManager;

        public Sket(SketRoleRepository<SketRoleModel> sketRoleManager)
        {
            this._sketRoleManager = sketRoleManager;
            Task.Run(async () =>
            {
                await SetupRoles();
                GetModelContext();
            });


        }

        private void GetModelContext()
        {
            var type = typeof(SketPersistedModel);
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));

            _context = types.ToList();
        }

        private async Task SetupRoles()
        {
            // Setup roles
            //var getRoles = DB.Queryable<SketRoleModel>().ToList();
            var getRoles = await _sketRoleManager.FindAll();
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}