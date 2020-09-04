using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Repository;
using Bracketcore.Sket.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Entities;
using System;

namespace Bracketcore.Sket
{
    public static class Extension
    {
        public static IServiceCollection AddSket(
            this IServiceCollection services, SketSettings settings)
        {

            if (string.IsNullOrEmpty(settings.JwtKey))
            {
                throw new Exception("JwtKey is required");
            }


            //services.AddAuthentication();
            //services.AddAuthorizationCore();

            Auth(settings, services);

            #region Sket Dependency injection section

            services.AddMongoDBEntities(settings.MongoSettings, settings.DatabaseName);
            services.TryAddScoped(typeof(SketAccessTokenRepository<>));
            services.TryAddScoped(typeof(SketEmailRepository<>));
            services.TryAddScoped(typeof(SketRoleRepository<>));
            services.TryAddScoped(typeof(SketUserRepository<>));
            var init = new Sket(settings);
            services.Add(new ServiceDescriptor(typeof(Sket), init));

            Console.WriteLine("Database " +
                              DB.GetInstance(settings.DatabaseName).GetDatabase().Client.Cluster.Description.State);
            #endregion

            //Add data protection
            services.AddDataProtection();

            #region Lockout user on failed attempts

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            });

            #endregion


            #region Identity Setup Section

            services.AddIdentityCore<SketUserModel>(option =>
            {

            });

            services.AddScoped<IUserStore<SketUserModel>, UserStore<SketUserModel>>();

            #endregion

            //  xss and crsf security
            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            #region CORS security section

            if (settings.CorsDomains.Count > 0)
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


            return services;
        }

        public static IApplicationBuilder UseSket(this IApplicationBuilder app)
        {

            app.UseAuthentication();

            return app;
        }

        static void Auth(SketSettings settings, IServiceCollection services)
        {
            if (settings.EnableJwt)
            {
                settings.EnableCookies = false;
                // add services for jwt
            }
            else
            {
                //services.AddAuthentication(options =>
                //{
                //    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //});

                //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
                //{
                //    options.LoginPath = new PathString("/auth/login");
                //    options.AccessDeniedPath = new PathString("/auth/denied");
                //});
            }
        }

        static void JsonCamelCase(SketSettings settings, IServiceCollection services)
        {

            // work on json
        }
    }
}