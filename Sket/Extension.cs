using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using Blazored.LocalStorage;
using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Init;
using Bracketcore.Sket.Manager;
using Bracketcore.Sket.Middleware;
using Bracketcore.Sket.Repository;
using Bracketcore.Sket.Repository.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Entities;

namespace Bracketcore.Sket
{
    /// <summary>
    ///     This extension class is used for Dependency injections
    /// </summary>
    public static class Extension
    {
        /// <summary>
        ///     Initial setup for Sket for dependency injection.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IServiceCollection AddSket(
            this IServiceCollection services, SketSettings settings)
        {
            #region Check Setup Section

            if (string.IsNullOrEmpty(settings.JwtKey)) throw new Exception("JwtKey is required");
            if (string.IsNullOrEmpty(settings.DomainUrl)) throw new Exception("DomainUrl is required");

            #endregion

            #region Core Section

            DB.InitAsync(settings.DatabaseName, settings.MongoSettings);
            var SketInit = Init.Sket.Init(settings);

            services.Add(new ServiceDescriptor(typeof(SketConfig), SketInit));

            #endregion

            #region Dependency Injection Section

            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.TryAddScoped(typeof(ISketAccessTokenRepository<>), typeof(SketAccessTokenRepository<>));
            services.TryAddScoped(typeof(ISketRoleRepository<>), typeof(SketRoleRepository<>));
            services.TryAddScoped(typeof(ISketUserRepository<>), typeof(SketUserRepository<>));
            services.TryAddScoped(typeof(ISketAuthenticationManager<>), typeof(SketAuthenticationManager<>));

            services.AddBlazoredLocalStorage(config =>
                config.JsonSerializerOptions.WriteIndented = true);

            services.AddAuthorizationCore(option =>
            {
                var normalRole = Enum.GetValues(typeof(SketRoleEnum)).Cast<SketRoleEnum>();

                foreach (var sketRoleEnum in normalRole)
                    option.AddPolicy(sketRoleEnum.ToString(), policy => policy.RequireRole(sketRoleEnum.ToString()));
            });


            Console.WriteLine("Database " +
                              DB.Database(settings.DatabaseName)
                                  .Client
                                  .Cluster
                                  .Description
                                  .State);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API Explorer",
                    Version = "v1"
                });
            });

            #endregion

            #region Middlewares Section

            services.TryAddTransient<SketTokenHeaderHandler>();

            #endregion

            #region Identity Setup Section

            #endregion

            #region XSS and CRSF security Section

            //     services.AddMvc(options =>
            //{
            //    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            //});

            #endregion

            #region Security Section

            services.AddDataProtection();

            void addCookies()
            {
                services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, c =>
                    {
                        c.Cookie.Name = Init.Sket.Cfg.Settings.AuthType.ToString();
                        c.LoginPath = "/login";
                        c.LogoutPath = "/login";
                    });
            }

            void addJwt()
            {
                services.AddAuthentication(option =>
                    {
                        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = false,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = settings.DomainUrl,
                            ValidAudience = settings.DomainUrl,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.JwtKey))
                        };
                        options.SaveToken = true;
                    })
                    .AddCookie(Init.Sket.Cfg.Settings.AuthType.ToString(), c =>
                    {
                        c.Cookie.Name = Init.Sket.Cfg.Settings.AuthType.ToString();
                        c.LoginPath = "/login";
                        c.LogoutPath = "/login";
                    });
            }

            void addBoth()
            {
                addJwt();
                addCookies();
            }


            switch (settings.AuthType)
            {
                case AuthType.Jwt:
                    addJwt();
                    break;
                case AuthType.Both:
                    addBoth();
                    break;
                default:
                    addCookies();
                    break;
            }

            #endregion

            #region CORS security Section

            if (settings.CorsDomains.Any())
                services.AddCors(options =>
                {
                    options.AddPolicy("Custom", builder =>
                    {
                        foreach (var domains in settings.CorsDomains)
                            builder.WithOrigins(domains).AllowAnyHeader().AllowAnyMethod();
                    });
                });

            #endregion

            #region Api HttpClient Configuration Section

            if (settings.ApiSetup.Any())
                foreach (var apiConfig in settings.ApiSetup)
                foreach (var control in apiConfig.Endpoints)
                    services.AddHttpClient(control,
                        client =>
                        {
                            client.BaseAddress = new Uri(Path.Join(apiConfig.BaseUrl, control));
                            client.DefaultRequestHeaders.Accept.Add(
                                new MediaTypeWithQualityHeaderValue("application/json"));
                            client.DefaultRequestHeaders.Accept.Add(
                                new MediaTypeWithQualityHeaderValue("application/xml"));
                            client.DefaultRequestHeaders.Accept.Add(
                                new MediaTypeWithQualityHeaderValue("text/plain"));
                        });
            // .AddHttpMessageHandler<SketTokenHeaderHandler>();

            #endregion

            return services;
        }


        /// <summary>
        ///     Initial setup for Sket middleware
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSket(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "API Explorer";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Explorer");
            });
            app.UseAuthentication();

            return app;
        }
    }
}