using Bracketcore.Sket.Entity;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bracketcore.Sket
{
    /// <summary>
    /// This class setup the roles and context models.
    /// </summary>
    public class Sket : IDisposable
    {
        public IEnumerable<SketRoleModel> Roles = new List<SketRoleModel>();
        public List<Type> Context = new List<Type>();

        /// <summary>
        /// Get the Sket settings data.
        /// </summary>
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

        /// <summary>
        /// Initiate a normal setup for your app
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Config"></param>
        /// <param name="settings"></param>
        public static void Init(IServiceCollection services, IConfiguration Config, SketSettings settings)
        {
            // DB.InitAsync( settings.DatabaseName, settings.MongoSettings);
            Extension.AddSket(services, Config, settings);
        }

        /// <summary>
        /// This will fetch all the model/entity of the project and give you access from any model
        /// </summary>
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

        /// <summary>
        /// this setup the default Roles for you application
        /// </summary>
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
                        DB.SaveAsync(new SketRoleModel()
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