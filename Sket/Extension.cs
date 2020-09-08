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

namespace Bracketcore.Sket
{
    public static class Extension
    {
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

            services.AddMongoDBEntities(settings.MongoSettings, settings.DatabaseName);
            services.TryAddScoped(typeof(SketAccessTokenRepository<>));
            services.TryAddScoped(typeof(SketEmailRepository<>));
            services.TryAddScoped(typeof(SketRoleRepository<>));
            services.TryAddScoped(typeof(SketUserRepository<>));
            services.TryAddScoped(typeof(JwtManager<>));

            services.AddScoped<AuthenticationStateProvider, SketAuthenticationStateProvider>();
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

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Login";
                options.AccessDeniedPath = "/AccessDenied";
                options.SlidingExpiration = true;
            });

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

        public static IApplicationBuilder UseSket(this IApplicationBuilder app)
        {

            app.UseAuthentication();

            return app;
        }


    }
}