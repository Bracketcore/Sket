using Bracketcore.Sket.Entity;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bracketcore.Sket
{
    /// <inheritdoc />
    public class Sket : IDisposable
    {
        public IEnumerable<SketRoleModel> Roles = new List<SketRoleModel>();
        public List<Type> Context = new List<Type>();

        public SketSettings SketSettings { get; set; }

        public Sket(SketSettings setting)
        {
            this.SketSettings = setting;

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

            foreach (var t in types.ToList())
            {
                Context.Add(t);
            }


        }

        private async Task SetupRoles()
        {
            // Setup roles
            try
            {
                var getRoles = await DB.Queryable<SketRoleModel>().FirstOrDefaultAsync();
                var normalRole = Enum.GetValues(typeof(SketRoleEnum)).Cast<SketRoleEnum>();

                if (getRoles == null)
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
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