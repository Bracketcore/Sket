using Bracketcore.KetAPI.Interfaces;
using Bracketcore.KetAPI.Model;
using Bracketcore.KetAPI.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Entities;
using System;

namespace Bracketcore.KetAPI
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
            services.AddIdentityCore<string>(opt => { });
            services.AddAuthentication();
            services.AddAuthorizationCore();

            Auth(settings, services);

            services.AddMongoDBEntities(settings.MongoSettings, settings.DatabaseName);
            services.AddSingleton<AccessTokenRepository>(new AccessTokenRepository(settings.JwtKey));
            services.AddSingleton<IBaseRepository<SketPersistedModel>, SketBaseRepository<SketPersistedModel>>();
            services.AddSingleton<EmailRepository>();
            services.AddSingleton<RoleRepository>();
            services.AddSingleton<SketUserRepository<SketUserModel>>();

            new Sket();


            Console.WriteLine("Database " +
                              DB.GetInstance(settings.DatabaseName).GetDatabase().Client.Cluster.Description.State);
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

                services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
                {
                    options.LoginPath = new PathString("/auth/login");
                    options.AccessDeniedPath = new PathString("/auth/denied");
                });
            }
        }

        static void JsonCamelCase(SketSettings settings, IServiceCollection services)
        {

            // work on json
        }
    }
}