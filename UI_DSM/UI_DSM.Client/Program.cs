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

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace UI_DSM.Client
{
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

            builder.Services.AddScoped(_ => new HttpClient
                { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddDevExpressBlazor();

            await builder.Build().RunAsync();
        }
    }
}