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

    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Services.FileService;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This module saves the product diagram layout
    /// </summary>
    [Route("api/Layout/{projectId:guid}/{reviewTaskId:guid}")]
    public class DiagrammingConfigurationModule : ModuleBase
    {
        /// <summary>
        ///     The name of the directory where to store diagrams
        /// </summary>
        public const string DirectoryName = "diagrams";

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
            app.MapPost($"{this.MainRoute}/{{configurationName}}/Save", this.SaveLayoutConfiguration)
                .Accepts<List<DiagramNodeDto>>("application/json")
                .WithTags("Layout")
                .WithName("Layout/Save");

            app.MapGet($"{this.MainRoute}/Load", this.LoadLayoutConfigurationNames)
                .WithTags("Layout")
                .WithName("Layout/Load");

            app.MapGet($"{this.MainRoute}/{{configurationName}}/Load", this.LoadLayoutConfiguration)
                .WithTags("Layout")
                .WithName("Layout/LoadConfig");

            app.MapDelete($"{this.MainRoute}/{{configurationName}}", this.DeleteLayoutConfiguration)
                .WithTags("Layout")
                .WithName("Layout/DeleteConfig");
        }

        /// <summary>
        ///     Deletes a configuration
        /// </summary>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewTaskId">The <see cref="Entity.Id" /> of the <see cref="ReviewTask" /></param>
        /// <param name="configurationName">The name of the configuration</param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task DeleteLayoutConfiguration(IParticipantManager participantManager, Guid projectId, Guid reviewTaskId, string configurationName, HttpContext context)
        {
            var participant = await participantManager.GetParticipantForProject(projectId, context.User.Identity?.Name);

            if (participant == null || !participant.IsAllowedTo(AccessRight.CreateDiagramConfiguration))
            {
                context.Response.StatusCode = 403;
                await context.Response.Negotiate("Unauthorized");
                return;
            }

            try
            {
                var folderPath = Path.Combine(DirectoryName, reviewTaskId.ToString());

                var fileName = $"{configurationName}.json";

                if (this.fileService.Exists(folderPath, fileName))
                {
                    this.fileService.DeleteFile(Path.Combine(folderPath, fileName));
                }
                else
                {
                    context.Response.StatusCode = 300;
                    await context.Response.Negotiate("Configuration not found");
                }
            }
            catch (Exception exception)
            {
                context.Response.StatusCode = 500;
                await context.Response.Negotiate(exception.Message);
            }
        }

        /// <summary>
        ///     Saves the product diagram layout
        /// </summary>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewTaskId">The <see cref="Entity.Id" /> of the <see cref="ReviewTask" /></param>
        /// <param name="configurationName">The name of the configuration</param>
        /// <param name="dto">The <see cref="DiagramDto" /> to save</param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task SaveLayoutConfiguration(IParticipantManager participantManager, Guid projectId, Guid reviewTaskId, string configurationName, DiagramDto dto, HttpContext context)
        {
            var participant = await participantManager.GetParticipantForProject(projectId, context.User.Identity?.Name);

            if (participant == null || !participant.IsAllowedTo(AccessRight.CreateDiagramConfiguration))
            {
                context.Response.StatusCode = 403;
                return;
            }

            try
            {
                var folderPath = Path.Combine(this.fileService.GetDirectoryPath(DirectoryName), reviewTaskId.ToString());
                var configurationsfiles = Directory.EnumerateFiles(folderPath).Select(Path.GetFileNameWithoutExtension).ToList();

                if (configurationsfiles.Any(x => string.Equals(configurationName, x, StringComparison.InvariantCultureIgnoreCase)))
                {
                    context.Response.StatusCode = 300;
                    await context.Response.Negotiate(new List<string> { "A configuration with the same name already exists" });
                    return;
                }

                if (configurationsfiles.Count >= 20)
                {
                    context.Response.StatusCode = 300;
                    await context.Response.Negotiate(new List<string> { "This diagram already has reach the maximum amount of configurations" });
                    return;
                }
            }
            catch
            {
                // No need to do anything if it catches, means that no configuration has been created
            }

            var json = JsonSerializer.Serialize(dto);
            var fileName = configurationName + ".json";

            try
            {
                await File.WriteAllTextAsync(Path.Combine(this.fileService.GetTempFolder(), fileName), json);
                var configurationPath = Path.Combine(DirectoryName, reviewTaskId.ToString());
                var pathOfSavedConfig = this.fileService.MoveFile(fileName, configurationPath);
                await context.Response.Negotiate(pathOfSavedConfig);
            }
            catch (Exception exception)
            {
                context.Response.StatusCode = 300;
                await context.Response.Negotiate(new List<string> { exception.Message });
            }
        }

        /// <summary>
        ///     Load the product diagram layout
        /// </summary>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewTaskId">The <see cref="Entity.Id" /> of the <see cref="ReviewTask" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task LoadLayoutConfigurationNames(IParticipantManager participantManager, Guid projectId, Guid reviewTaskId, HttpContext context)
        {
            var participant = await participantManager.GetParticipantForProject(projectId, context.User.Identity?.Name);

            if (participant == null)
            {
                context.Response.StatusCode = 403;
                return;
            }

            var folderPath = Path.Combine(this.fileService.GetDirectoryPath(DirectoryName), reviewTaskId.ToString());

            try
            {
                var configurationsfiles = Directory.EnumerateFiles(folderPath).Select(Path.GetFileNameWithoutExtension);
                await context.Response.Negotiate(configurationsfiles);
            }
            catch (DirectoryNotFoundException)
            {
                await context.Response.Negotiate(new List<string>());
            }
        }

        /// <summary>
        ///     Load the product diagram layout
        /// </summary>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewTaskId">The <see cref="Entity.Id" /> of the <see cref="ReviewTask" /></param>
        /// <param name="configurationName">The name of the configuration</param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task LoadLayoutConfiguration(IParticipantManager participantManager, Guid projectId, Guid reviewTaskId, string configurationName, HttpContext context)
        {
            var participant = await participantManager.GetParticipantForProject(projectId, context.User.Identity?.Name);

            if (participant == null)
            {
                context.Response.StatusCode = 403;
                return;
            }

            var folderPath = Path.Combine(this.fileService.GetDirectoryPath(DirectoryName), reviewTaskId.ToString());

            try
            {
                var configurationfile = Directory.EnumerateFiles(folderPath).FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == configurationName);

                await using var stream = File.OpenRead(configurationfile);

                var dto = await JsonSerializer.DeserializeAsync<DiagramDto>(stream);
                await context.Response.Negotiate(dto);
            }
            catch (DirectoryNotFoundException)
            {
                await context.Response.Negotiate(new List<string>());
            }
        }
    }
}
