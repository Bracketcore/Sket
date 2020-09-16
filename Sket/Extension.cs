using Bracketcore.Sket.Manager;
using Bracketcore.Sket.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Entities;
using System;
using Bracketcore.Sket.Misc;
using Microsoft.AspNetCore.Components.Authorization;
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
        /// <param name="config"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IServiceCollection AddSket(
            this IServiceCollection services, IConfiguration config, SketSettings settings)
        {
            if (string.IsNullOrEmpty(settings.JwtKey))
            {
                throw new Exception("JwtKey is required");
            }

            #region Sket Dependency injection section

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = "Bearer";
                option.DefaultChallengeScheme = "Bearer";
            });

            //    .AddJwtBearer(x =>
            //{
            //    x.SaveToken = true;
            //    x.RequireHttpsMetadata = false;
            //    x.TokenValidationParameters = new TokenValidationParameters()
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["Jwt:Key"])),
            //        ValidateIssuer = false,
            //        ValidateAudience = false
            //    };
            //});

            // services.AddMongoDBEntities(settings.MongoSettings, settings.DatabaseName);
            DB.InitAsync(settings.DatabaseName, settings.MongoSettings);
            services.TryAddScoped(typeof(SketAccessTokenRepository<>));
            services.TryAddScoped(typeof(SketEmailRepository<>));
            services.TryAddScoped(typeof(SketRoleRepository<>));
            services.TryAddScoped(typeof(SketUserRepository<>));
            services.TryAddScoped(typeof(JwtManager<>));

            var init = new Sket(settings);
            services.Add(new ServiceDescriptor(typeof(Sket), init));

            Console.WriteLine("Database " +
                              DB.Database(settings.DatabaseName).Client.Cluster.Description.State);

            #endregion

            //Add data protection
            services.AddDataProtection();

            #region Lockout user on failed attempts

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            // services.ConfigureApplicationCookie(options =>
            // {
            //     // Cookie settings
            //     options.Cookie.HttpOnly = true;
            //     options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            //
            //     options.LoginPath = "/Login";
            //     options.AccessDeniedPath = "/AccessDenied";
            //     options.SlidingExpiration = true;
            // });

            #endregion


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
    }
}