using Bracketcore.KetAPI.Model;
using Bracketcore.KetAPI.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Entities;

namespace Bracketcore.KetAPI
{
    public static class Extension
    {
        public static IServiceCollection AddKetAPI(
            this IServiceCollection services, KetAPISetting settings)
        {
            if (settings.EnableJwt)
            {
                settings.EnableCookies = false;
                // add services for jwt
            }
            else
            {
                services.AddAuthenticationCore(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                });
            }

            services.AddMongoDBEntities(settings.MongoSettings, settings.DatabaseName);
            services.AddSingleton<AccessTokenRepository>();
            services.AddSingleton(new KetAPI());
            return services;
        }
    }

    public class KetAPISetting
    {
        public bool EnableCookies { get; set; } = true;
        public bool EnableJwt { get; set; } = false;
        public string DatabaseName { get; set; }

        public MongoClientSettings MongoSettings { get; set; } = new MongoClientSettings()
            {Server = new MongoServerAddress("localhost"), ReadConcern = ReadConcern.Majority};
    }
}