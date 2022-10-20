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

namespace UI_DSM.Client
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using Blazored.SessionStorage;

    using CDP4JsonSerializer;

    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

    using UI_DSM.Client.Services;
    using UI_DSM.Client.Services.AuthenticationService;
    using UI_DSM.Client.Services.ViewProviderService;
    using UI_DSM.Serializer.Json;

    /// <summary>
    ///     Entry class of the <see cref="UI_DSM.Client" /> project
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Program
    {
        /// <summary>
        ///     Entry point of the <see cref="UI_DSM.Client" /> project
        /// </summary>
        /// <param name="args">A collection of <see cref="string" /></param>
        /// <returns>A running <see cref="Task" /></returns>
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
            
            AddServices(builder);
            AddViewModels(builder);

            await builder.Build().RunAsync();
        }

        /// <summary>
        ///     Register all ViewModels into the <see cref="WebAssemblyHostBuilder" />
        /// </summary>
        /// <param name="builder">The <see cref="WebAssemblyHostBuilder" /></param>
        private static void AddViewModels(WebAssemblyHostBuilder builder)
        {
            var viewModelInterfaces = Assembly.GetCallingAssembly().GetExportedTypes()
                .Where(x => x.IsInterface && x.Name.EndsWith("ViewModel")).ToList();

            foreach (var viewModelInterface in viewModelInterfaces)
            {
                var viewModel = Assembly.GetCallingAssembly().GetExportedTypes()
                    .FirstOrDefault(x => x.IsClass
                                         && !x.IsAbstract
                                         && x.Name == viewModelInterface.Name.Remove(0, 1)
                                         && x.GetInterface(viewModelInterface.Name) == viewModelInterface);

                if (viewModel != null)
                {
                    builder.Services.AddTransient(viewModelInterface, viewModel);
                }
            }
        }

        /// <summary>
        ///     Register all services into the <see cref="WebAssemblyHostBuilder.Services" />
        /// </summary>
        /// <param name="builder">The <see cref="WebAssemblyHostBuilder" /></param>
        private static void AddServices(WebAssemblyHostBuilder builder)
        {
            builder.Services.AddSingleton<IJsonSerializer, JsonSerializer>();
            builder.Services.AddSingleton<IJsonDeserializer, JsonDeserializer>();
            builder.Services.AddSingleton<ICdp4JsonSerializer, Cdp4JsonSerializer>();

            builder.Services.AddScoped(_ => new HttpClient
            {
                BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/")
            });

            builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationProvider>();

            var serviceInterfaces = Assembly.GetCallingAssembly().GetExportedTypes()
                .Where(x => x.IsInterface && x.Name.EndsWith("Service")).ToList();

            foreach (var serviceInterface in serviceInterfaces)
            {
                var service = Assembly.GetCallingAssembly().GetExportedTypes()
                    .FirstOrDefault(x => x.IsClass
                                         && x.Name == serviceInterface.Name.Remove(0, 1)
                                         && x.GetInterface(serviceInterface.Name) == serviceInterface);

                if (service != null)
                {
                    builder.Services.AddScoped(serviceInterface, service);
                    ServiceBase.RegisterService(service);
                }
            }

            ViewProviderService.RegisterViews();
            builder.Services.AddAuthorizationCore();

            builder.Services.AddBlazoredSessionStorage();
            builder.Services.AddDevExpressBlazor();
        }
    }
}
