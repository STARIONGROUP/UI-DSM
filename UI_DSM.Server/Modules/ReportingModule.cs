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

    using UI_DSM.Server.Services.FileService;
    using UI_DSM.Shared.DTO.CometData;

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
            app.MapPost($"{this.MainRoute}/Upload", this.UploadReport);
        }

        /// <summary>
        ///     Uploads a report file
        /// </summary>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task UploadReport(HttpContext context)
        {
            var response = new CometAuthenticationResponse();

            if (context.Request.Form.Files.Count != 1 || context.Request.Form.Files[0].ContentType != "application/x-zip-compressed")
            {
                response.Errors = new List<string>
                {
                    "The request only accept a single zip file!"
                };

                context.Response.StatusCode = 400;
                await context.Response.Negotiate(response);
                return;
            }

            var file = context.Request.Form.Files[0];
            var guid = Guid.NewGuid();
            var fileName = $"{guid}.rep4";
            var fileService = context.RequestServices.GetService<IFileService>();
            await fileService!.WriteToFile(file, fileName);
            response.IsRequestSuccessful = true;
            response.SessionId = guid;
            await context.Response.Negotiate(response);
        }
    }
}
