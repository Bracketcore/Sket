using Bracketcore.Sket.Model;
using Bracketcore.Sket.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Entities;
using System;

namespace Bracketcore.Sket
{
    public static class Extension
    {
        public static IServiceCollection AddSket(
            this IServiceCollection services, SketSettings settings)
        {
            Auth(settings, services);

            services.AddMongoDBEntities(settings.MongoSettings, settings.DatabaseName);
            services.AddSingleton<AccessTokenRepository>();
            services.AddSingleton<EmailRepository>();
            services.AddSingleton<RoleRepository>();
            services.AddSingleton<UserRepository<UserModel>>();

            new Sket();


            Console.WriteLine("Database " +
                              DB.GetInstance(settings.DatabaseName).GetDatabase().Client.Cluster.Description.State);
            return services;
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