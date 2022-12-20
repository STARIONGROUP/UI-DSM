// --------------------------------------------------------------------------------------------------------
// <copyright file="DiagrammingConfigurationModule.cs" company="RHEA System S.A.">
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
    using System.Text.Json;
    using Carter.Response;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Components;
    using UI_DSM.Client.Services.Administration.ParticipantService;
    using UI_DSM.Server.Context;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Services.AboutService;
    using UI_DSM.Server.Services.FileService;
    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    ///     This module saves the product diagram layout
    /// </summary>
    [Route("api/Layout/{projectId:guid}/{reviewTaskId:guid}/{configurationName}")]
    public class DiagrammingConfigurationModule : ModuleBase
    {
        /// <summary>
        ///     The <see cref="IFileService" />
        /// </summary>
        private readonly IFileService fileService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DiagrammingConfigurationModule" /> class.
        /// </summary>
        /// <param name="fileService">The <see cref="IFileService" /></param>
        public DiagrammingConfigurationModule(IFileService fileService)
        {
            this.fileService = fileService;
        }

        /// <summary>
        ///     Adds routes to the <see cref="IEndpointRouteBuilder" />
        /// </summary>
        /// <param name="app">The <see cref="IEndpointRouteBuilder" /></param>
        [ExcludeFromCodeCoverage]
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost($"{this.MainRoute}/Save", this.SaveLayoutConfiguration)
                .Accepts<List<DiagramLayoutInformationDto>>("application/json")
                .WithTags("Layout")
                .WithName("Layout/Save");
        }

        /// <summary>
        ///     Saves the product diagram layout
        /// </summary>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task SaveLayoutConfiguration(Guid projectId, Guid reviewTaskId, String configurationName, List<DiagramLayoutInformationDto> dtos, HttpContext context)
        {
            var participantManager = context.RequestServices.GetService<IParticipantManager>();

            if (participantManager == null)
            {
                return;
            }

            var participant = await participantManager.GetParticipantForProject(projectId, context.User.Identity?.Name);

            if (participant == null)
            {
                return;
            }

            var path = Path.Combine(this.fileService.GetTempFolder(), reviewTaskId.ToString());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            var json = JsonSerializer.Serialize(dtos);
            File.WriteAllText(Path.Combine(path, configurationName+".json"), json);
        }
    }
}
