using Bracketcore.Sket.Manager;
using Bracketcore.Sket.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Entities;
using System;
using Bracketcore.Sket.Misc;
using Bracketcore.Sket.StateManager;
using Microsoft.AspNetCore.Authentication.Cookies;
using MongoDB.Driver;

namespace Bracketcore.Sket
{
    /// <summary>
    /// This extension class is used for Dependency injections
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// Initial setup for Sket for dependency injection.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IServiceCollection AddSket(
            this IServiceCollection services, SketSettings settings)
        {
            if (string.IsNullOrEmpty(settings.JwtKey))
            {
                throw new Exception("JwtKey is required");
            }

            SecuritySetup(services, settings);
            SetupServices(services, settings); /// DI Services
            
            #region Identity Setup Section

            //services.AddIdentityCore<SketUserModel>(option =>
            //{

            //});

            //services.AddScoped<IUserStore<SketUserModel>, UserStore<SketUserModel>>();

            #endregion

            #region xss and crsf security

            //     services.AddMvc(options =>
            //{
            //    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            //});

            #endregion
            
            return services;
        }

        /// <summary>
        /// Initial setup for Sket middleware
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSket(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            
            return app;
        }

        private static void SecuritySetup(IServiceCollection services, SketSettings settings)
        {
            //Add data protection
            services.AddDataProtection();

            switch (settings.AuthType)
            {
              case AuthType.Jwt :
                    break;
              
              default:
                  services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                      .AddCookie();
                  break;
            }
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = "Bearer";
                option.DefaultChallengeScheme = "Bearer";
            });
            
            #region CORS security section

            if (settings.CorsDomains != null)
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("Custom", builder =>
                    {
                        foreach (var domains in settings.CorsDomains)
                        {
                            builder.WithOrigins(domains).AllowAnyHeader().AllowAnyMethod();
                        }
                    });
                });
            }

            #endregion
        }

        private static void SetupServices(IServiceCollection services, SketSettings settings)
        {
            DB.InitAsync(settings.DatabaseName, settings.MongoSettings);
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.TryAddScoped(typeof(SketAccessTokenRepository<>));
            services.TryAddScoped(typeof(SketEmailRepository<>));
            services.TryAddScoped(typeof(SketRoleRepository<>));
            services.TryAddScoped(typeof(SketUserRepository<>));
            services.TryAddScoped(typeof(AuthenticationManager<>));
            // services.TryAddScoped(typeof(SketAppState));

            var SketInit = new Sket(settings);

            services.Add(new ServiceDescriptor(typeof(Sket), SketInit));

            Console.WriteLine("Database " +
                              DB.Database(settings.DatabaseName)
                                  .Client
                                  .Cluster
                                  .Description
                                  .State);
        }
    }
}