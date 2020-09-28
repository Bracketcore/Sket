using Bracketcore.Sket.Manager;
using Bracketcore.Sket.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Entities;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Blazored.LocalStorage;
using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Misc;
using Bracketcore.Sket.StateManager;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.OpenApi.Models;
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
            SetupServices(services, settings); // DI Services

            ApiStructure(services, settings);

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

        private static void ApiStructure(IServiceCollection services, SketSettings settings)
        {
            if (!settings.ApiSetup.Any()) return;
            
            foreach (var apiConfig in settings.ApiSetup)
            {
                foreach (var control in apiConfig.Endpoints)
                {
                    services.AddHttpClient(control,
                        client =>
                        {
                            client.BaseAddress = new Uri(Path.Join(apiConfig.BaseUrl , control));
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                        });
                }
            }
        }


        /// <summary>
        /// Initial setup for Sket middleware
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSket(this IApplicationBuilder app)
        {
            app.UseSwagger(i => { i.SerializeAsV2 = true; });

            app.UseSwaggerUI(i =>
            {
                i.SwaggerEndpoint("/swagger/v1/swagger.json", "Api Explorer");
                i.RoutePrefix = "swagger";
            });

            app.UseAuthentication();

            return app;
        }

        private static void SecuritySetup(IServiceCollection services, SketSettings settings)
        {
            //Add data protection
            services.AddDataProtection();

            switch (settings.AuthType)
            {
                case AuthType.Jwt:
                    break;

                default:
                    services.AddAuthentication(opt =>
                        {
                            opt.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                            opt.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        })
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
            services.TryAddScoped(typeof(ISketAccessTokenRepository<>), typeof(SketAccessTokenRepository<>));
            // services.TryAddScoped(typeof(SketEmailRepository<>));
            services.TryAddScoped(typeof(ISketRoleRepository<>), typeof(SketRoleRepository<>));
            services.TryAddScoped(typeof(ISketUserRepository<>), typeof(SketUserRepository<>));
            services.TryAddScoped(typeof(ISketAuthenticationManager<>), typeof(SketAuthenticationManager<>));
            // services.TryAddSingleton(typeof(ISketAppState), typeof(SketAppState));

            services.AddBlazoredLocalStorage(config =>
                config.JsonSerializerOptions.WriteIndented = true);

            services.AddSwaggerGen(i =>
            {
                i.SwaggerDoc("V1", new OpenApiInfo()
                {
                    Title = "Api Explorer",
                    Version = "V1"
                });
            });


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