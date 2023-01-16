// --------------------------------------------------------------------------------------------------------
// <copyright file="ReportingModule.cs" company="RHEA System S.A.">
//  Copyright (c) 2023 RHEA System S.A.
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

    using UI_DSM.Server.Services.ReportingService;

    /// <summary>
    ///     This module gets reporting information
    /// </summary>
    [Route("api/Reporting")]
    public class ReportingModule : ModuleBase
    {
        /// <summary>
        ///     Adds routes to the <see cref="IEndpointRouteBuilder" />
        /// </summary>
        /// <param name="app">The <see cref="IEndpointRouteBuilder" /></param>
        [ExcludeFromCodeCoverage]
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet(this.MainRoute + "/{projectId:guid}", this.GetAvailableReports)
                .Produces<List<string>>()
                .WithTags("Reporting")
                .WithName("Reporting/GetAvailableReports");
        }

        /// <summary>
        ///     Gets the Available Reports for a Budget review task
        /// </summary>
        /// <param name="reportingService">The <see cref="IReportingService" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="projectId">The project id</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public Task GetAvailableReports(IReportingService reportingService, HttpContext context, Guid projectId)
        {
            var availableReports = reportingService.GetAvailableReports(projectId);
            return context.Response.Negotiate(availableReports);
        }
    }
}
