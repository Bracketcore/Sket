using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using MongoDB.Entities;
using Sket.Core.Manager;
using Sket.Core.Models;
using Sket.Core.Repository;

namespace Sket.Core.Init
{
    /// <summary>
    ///     This class setup the roles and context models.
    /// </summary>
    public static class Sket
    {
        public static SketConfig Cfg { get; set; } = new();

        public static IServiceCollection SketServices { get; set; }

        /// <summary>
        ///     Initiate a normal setup for your app
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Config"></param>
        /// <param name="settings"></param>
        public static SketConfig Init(SketSettings settings)
        {
            Cfg.Settings = settings;
            if (settings is not null)
            {
                GetModels();
                GetRoles();
            }

            return Cfg;
        }

        private static void GetRoles()
        {
            var getRoles = DB.Queryable<SketRoleModel>().Any();
            var normalRole = Enum.GetValues(typeof(SketRoleEnum)).Cast<SketRoleEnum>();

            if (getRoles)
                Console.WriteLine("Roles Set");
            else
                foreach (var role in normalRole)
                    DB.SaveAsync(new SketRoleModel
                    {
                        Name = role.ToString()
                    });
        }

        private static void GetModels()
        {
            //Todo: work on the context data which will allow the user to access every repo with ease
            var type = typeof(SketBaseRepository<>);
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));

            foreach (var t in types.ToList()) Cfg.Context.Add(t);
        }


        public static void Initialize(this IServiceCollection services, SketSettings settings)
        {
            services.AddSket(settings);
        }

        public static void Initialize(this IServiceCollection services)
        {
            services.AddSket(null);
        }

        public static void SketStaticFileDirectory(this IApplicationBuilder app, string resourcePath,
            string reqPath = null)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Environment.CurrentDirectory, resourcePath)),
                RequestPath = reqPath == null ? $"/{resourcePath}" : reqPath
            });
        }
   //todo create a middleware to handle easy creation of site route from the content root folder which will use SketStaticFileDirectory to handle the static directory


        /// <summary>
        ///     This static method adds authentication state provider to the every SketUserModel inheritance
        /// </summary>
        /// <param name="services"></param>
        public static void SketAppAuthenticator(this IServiceCollection services)
        {
            var type = typeof(SketUserModel);

            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p)).ToList();

            for (var i = 0; i < types.Count(); i++)
                if (!types[i].FullName.Contains("Sket.Core.Models.SketUserModel"))
                {
                    var t = typeof(SketAuthenticationStateProvider<>).MakeGenericType(types[i]);

                    services.AddScoped(typeof(AuthenticationStateProvider), t);
                }
        }

        //todo WIP
        public static void SketRepositorySetup(this IServiceCollection services)
        {
            var type = typeof(SketUserModel);

            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p)).ToList();

            for (var i = 0; i < types.Count(); i++)
                if (!types[i].FullName.Contains("Sket.Core.Models.SketPersistedModel"))
                {
                    var t = typeof(SketBaseRepository<>).MakeGenericType(types[i]);

                    services.AddScoped(t);
                }
        }
    }
}
