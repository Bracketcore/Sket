using Bracketcore.Sket.Entity;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Bracketcore.Sket
{
    /// <summary>
    /// This class setup the roles and context models.
    /// </summary>
    public class Sket : IDisposable
    {
        public static SketConfig Cfg { get; set; }

        /// <summary>
        /// Initiate a normal setup for your app
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Config"></param>
        /// <param name="settings"></param>
        public Sket(SketSettings settings)
        {
            Cfg = new SketConfig()
            {
                Settings = settings,
                Context = new List<Type>()
            };

            

            GetModels();
            GetRoles();
        }

        public void GetRoles()
        {
            var getRoles = DB.Queryable<SketRoleModel>().Any();
            var normalRole = Enum.GetValues(typeof(SketRoleEnum)).Cast<SketRoleEnum>();

            if (getRoles)
            {
                Console.WriteLine("Roles Set");
            }
            else
                foreach (var role in normalRole)
                {
                    DB.SaveAsync(new SketRoleModel()
                    {
                        Name = role.ToString()
                    });
                }
        }

        public void GetModels()
        {
            var type = typeof(SketPersistedModel);
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));

            foreach (var t in types.ToList())
            {
                Cfg.Context.Add(t);
            }
        }

        public static SketConfig Init(IServiceCollection services, SketSettings settings)
        {
            Extension.AddSket(services, settings);
            return new SketConfig()
            {
                Settings = settings
            };
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