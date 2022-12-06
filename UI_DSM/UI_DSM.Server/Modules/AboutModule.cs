// --------------------------------------------------------------------------------------------------------
// <copyright file="AboutModule.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Server.Modules
{
    using System.Diagnostics.CodeAnalysis;

    using Carter.Response;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Services.AboutService;
    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    ///     This module get information related to the current system setup
    /// </summary>
    [Route("api/About")]
    public class AboutModule : ModuleBase
    {
        /// <summary>
        ///     Adds routes to the <see cref="IEndpointRouteBuilder" />
        /// </summary>
        /// <param name="app">The <see cref="IEndpointRouteBuilder" /></param>
        [ExcludeFromCodeCoverage]
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet(this.MainRoute, this.GetSystemInformation)
                .Produces<SystemInformationDto>()
                .WithTags("About")
                .WithName("About/SystemInformation");
        }

        /// <summary>
        ///     Gets this system information
        /// </summary>
        /// <param name="aboutService">The <see cref="IAboutService" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="databaseContext">The <see cref="DatabaseContext" /></param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize(Roles = "Administrator")]
        public Task GetSystemInformation(IAboutService aboutService, HttpContext context, DatabaseContext databaseContext)
        {
            var systemInfo = aboutService.GetSystemInformation(databaseContext);
            return context.Response.Negotiate(systemInfo);
        }
    }
}
