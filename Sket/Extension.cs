using Blazored.LocalStorage;
using Bracketcore.Sket.Manager;
using Bracketcore.Sket.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using MongoDB.Entities;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Middleware;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;

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
            #region Check Setup Section

            if (string.IsNullOrEmpty(settings.JwtKey)) throw new Exception("JwtKey is required");
            if (string.IsNullOrEmpty(settings.DomainUrl)) throw new Exception("DomainUrl is required");
            
            #endregion

            #region Core Section

            DB.InitAsync(settings.DatabaseName, settings.MongoSettings);
            var SketInit = Sket.Init(settings);

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
                {
                    option.AddPolicy(sketRoleEnum.ToString(), policy => policy.RequireRole(sketRoleEnum.ToString()));
                }
            });


            Console.WriteLine("Database " +
                              DB.Database(settings.DatabaseName)
                                  .Client
                                  .Cluster
                                  .Description
                                  .State);

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
          

            void addCokies()
            {
                services.AddAuthentication("CookieAuth")
                    .AddCookie("CookieAuth", c =>
                    {
                        c.Cookie.Name = "SketCookies";
                        c.LoginPath = "/login";
                        c.LogoutPath = "/login";
                        
                    });
            }
            
            void addJwt()
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters()
                    {   ValidateIssuer = true,    
                        ValidateAudience = true,    
                        ValidateLifetime = true,    
                        ValidateIssuerSigningKey = true,    
                        ValidIssuer = settings.DomainUrl,    
                        ValidAudience = settings.DomainUrl,    
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.JwtKey))    
                
                    };
                });
            }
            
            void addBoth()
            {
                addJwt();
                addCokies();
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
                    addCokies();
                    break;
            }

            #endregion

            #region CORS security Section

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

            #region Api HttpClient Configuration Section

            if (settings.ApiSetup.Any())
            {
                foreach (var apiConfig in settings.ApiSetup)
                {
                    foreach (var control in apiConfig.Endpoints)
                    {
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
                    }
                }
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