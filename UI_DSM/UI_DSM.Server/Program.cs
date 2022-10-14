// --------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Server
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Text;

    using Carter;

    using CDP4JsonSerializer;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;

    using NLog;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Serializer.Json;
    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Modules;
    using UI_DSM.Server.Services.CometService;
    using UI_DSM.Server.Services.FileService;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Entry class of the <see cref="UI_DSM.Server" /> project
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Program
    {
        /// <summary>
        ///     The <see cref="NLog" /> logger
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Entry point of the <see cref="UI_DSM.Server" /> project
        /// </summary>
        /// <param name="args">A collection of <see cref="string" /></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddCarter();
            builder.Services.AddAuthorization();

            builder.Services.AddCors(policy =>
            {
                policy.AddPolicy("CorsPolicy", opt => opt
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });

            builder.Services.AddDbContext<DatabaseContext>(opt => { opt.UseNpgsql(builder.Configuration["DataBaseConnection"]); });

            builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<DatabaseContext>();
            builder.Services.AddSingleton<IJsonSerializer, JsonSerializer>();
            builder.Services.AddSingleton<IJsonDeserializer, JsonDeserializer>();
            builder.Services.AddSingleton<ICdp4JsonSerializer, Cdp4JsonSerializer>();
            builder.Services.AddSingleton<IJsonService, JsonService>();
            builder.Services.AddSingleton<ICometService, CometService>();
            builder.Services.AddSingleton<IFileService, FileService>(_ => new FileService(builder.Configuration["StoragePath"]));

            builder.Services.AddRouting(options =>
            {
                options.ConstraintMap.Add("EnumerableOfGuid", typeof(EnumerableOfGuidRouteConstraint));
            });

            RegisterManagers(builder);
            RegisterModules();
            RegisterEntities();

            var jwtSettings = builder.Configuration.GetSection("JWTSettings");

            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = false,

                    ValidIssuer = jwtSettings["validIssuer"],
                    ValidAudience = jwtSettings["validAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["securityKey"]))
                };
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapCarter();

            app.MapFallbackToFile("index.html");

            LogAppStart();

            app.Run();
        }

        /// <summary>
        ///     Register all <see cref="Entity" /> class for deep level properties computation
        /// </summary>
        public static void RegisterEntities()
        {
            var entityType = typeof(Entity);
            var entityAssembly = Assembly.GetAssembly(entityType)!;

            var entities = entityAssembly.GetExportedTypes()
                .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(entityType)).ToList();

            foreach (var entity in entities)
            {
                Entity.RegisterEntityProperties(entity);
            }

            Entity.RegisterAbstractEntity(entityAssembly.GetExportedTypes()
                .Where(x => x.IsClass && x.IsSubclassOf(entityType)).ToList());
        }

        /// <summary>
        ///     Register all Modules for route computation
        /// </summary>
        private static void RegisterModules()
        {
            var modules = Assembly.GetCallingAssembly().GetExportedTypes()
                .Where(x => x.IsClass && x.IsSubclassOf(typeof(ModuleBase)) && x.Name.EndsWith("Module")).ToList();

            foreach (var module in modules)
            {
                ModuleBase.RegisterModule(module);
            }
        }

        /// <summary>
        ///     Register all Managers into the <see cref="WebApplicationBuilder" />
        /// </summary>
        /// <param name="builder">The <see cref="WebApplicationBuilder" /></param>
        private static void RegisterManagers(WebApplicationBuilder builder)
        {
            var managerInterfaces = Assembly.GetCallingAssembly().GetExportedTypes()
                .Where(x => x.IsInterface && x.Name.EndsWith("Manager")).ToList();

            foreach (var managerInterface in managerInterfaces)
            {
                var manager = Assembly.GetCallingAssembly().GetExportedTypes()
                    .FirstOrDefault(x => x.IsClass
                                         && x.Name == managerInterface.Name.Remove(0, 1)
                                         && x.GetInterface(managerInterface.Name) == managerInterface);

                if (manager != null)
                {
                    builder.Services.AddScoped(managerInterface, manager);

                    foreach (var managerBaseInterface in managerInterface.GetInterfaces())
                    {
                        builder.Services.AddScoped(managerBaseInterface, manager);
                    }
                }
            }
        }

        /// <summary>
        ///     Add a header to the log file
        /// </summary>
        private static void LogAppStart()
        {
            Logger.Info("-----------------------------------------------------------------------------------------");
            Logger.Info($"Starting UI-DSM Server {Assembly.GetExecutingAssembly().GetName().Version}");
            Logger.Info("-----------------------------------------------------------------------------------------");
        }
    }
}
