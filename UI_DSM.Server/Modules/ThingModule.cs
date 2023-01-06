// --------------------------------------------------------------------------------------------------------
// <copyright file="ThingModule.cs" company="RHEA System S.A.">
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

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using UI_DSM.Client.Extensions;
    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Server.Managers.ArtifactManager;
    using UI_DSM.Server.Managers.ThingManager;
    using UI_DSM.Server.Services.SearchService;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="ThingModule" /> is a <see cref="ModuleBase" /> for getting <see cref="Thing" />
    /// </summary>
    [Microsoft.AspNetCore.Components.Route("api/Project/{projectId:guid}/Model/{modelIds:EnumerableOfGuid}/Thing")]
    public class ThingModule : ModuleBase
    {
        /// <summary>
        ///     The name of the <see cref="CDP4Common.DTO.Thing" />
        /// </summary>
        private const string ThingName = nameof(Thing);

        /// <summary>
        ///     Adds routes to the <see cref="IEndpointRouteBuilder" />
        /// </summary>
        /// <param name="app">The <see cref="IEndpointRouteBuilder" /></param>
        [ExcludeFromCodeCoverage]
        public override void
            AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet(this.MainRoute, this.GetThings)
                .Produces<IEnumerable<Thing>>()
                .WithTags(ThingName)
                .WithName($"{ThingName}/GetThings");

            app.MapPut(this.MainRoute, this.IndexThings)
                .WithTags(ThingName)
                .WithName($"{ThingName}/IndexThings");
        }

        /// <summary>
        ///     Indexes all <see cref="Thing" /> that are contained into an <see cref="Iteration" />
        /// </summary>
        /// <param name="artifactManager">The <see cref="IArtifactManager" /></param>
        /// <param name="thingManager">The <see cref="IThingManager" /></param>
        /// <param name="searchService">The <see cref="ISearchService" /></param>
        /// <param name="jsonService">The <see cref="IJsonService"/></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="modelIds">A collection of all <see cref="Model" /> id</param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task IndexThings(IArtifactManager artifactManager, IThingManager thingManager, ISearchService searchService, IJsonService jsonService,
            HttpContext context, Guid projectId, string modelIds)
        {
            await context.Response.CompleteAsync();
            var models = new List<Model>();

            foreach (var modelId in modelIds.FromGuidArray().ToList())
            {
                var artifact = await artifactManager.FindEntityWithContainer(modelId);

                if (!TryConvertToModel(context, projectId, artifact, out var model))
                {
                    return;
                }

                models.Add(model);
            }

            foreach (var model in models.ToList())
            {
                var response = await searchService.SearchAfter(model.IterationId.ToString());
                var searchDtos = jsonService.Deserialize<IEnumerable<CommonBaseSearchDto>>(response);

                if (searchDtos.Any())
                {
                    models.Remove(model);
                }
            }

            if (models.Any())
            {
                var things = await thingManager.GetIterations(models);
                await searchService.IndexIteration(things);
            }
        }

        /// <summary>
        ///     Gets all <see cref="Thing" /> that is contained into a <see cref="Model" />
        /// </summary>
        /// <param name="artifactManager">The <see cref="IArtifactManager" /></param>
        /// <param name="thingManager">The <see cref="IThingManager" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="modelIds">A collection of <see cref="Guid" /> of the <see cref="Model" /></param>
        /// <param name="classKind">Optional parameter to specify the <see cref="ClassKind" /></param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task GetThings(IArtifactManager artifactManager, IThingManager thingManager, HttpContext context,
            Guid projectId, string modelIds, [FromQuery] string classKind = "")
        {
            var models = new List<Model>();

            foreach (var modelId in modelIds.FromGuidArray().ToList())
            {
                var artifact = await artifactManager.FindEntityWithContainer(modelId);

                if (!TryConvertToModel(context, projectId, artifact, out var model))
                {
                    return;
                }

                models.Add(model);
            }

            ClassKind classKindValue;

            if (string.IsNullOrEmpty(classKind))
            {
                classKindValue = ClassKind.Iteration;
            }
            else
            {
                if (!Enum.TryParse(classKind, true, out classKindValue))
                {
                    context.Response.StatusCode = 400;
                    return;
                }
            }

            var things = classKindValue switch
            {
                ClassKind.Iteration => await thingManager.GetIterations(models),
                ClassKind.DomainOfExpertise => await thingManager.GetDomainOfExpertises(models),
                _ => await thingManager.GetThings(models, classKindValue)
            };

            await context.Response.Negotiate(things);
        }

        /// <summary>
        ///     Tries to get a <see cref="Model" /> from the <see cref="Artifact" />
        /// </summary>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="projectId">The expected <see cref="Guid" /> of the container of the <see cref="Model" /></param>
        /// <param name="artifact">The <see cref="Artifact" /> to convert</param>
        /// <param name="model">The <see cref="Model" /></param>
        /// <returns>True is the <see cref="Model" /> has been successfully converted</returns>
        private static bool TryConvertToModel(HttpContext context, Guid projectId, Artifact artifact, out Model model)
        {
            model = null;

            if (artifact == null)
            {
                context.Response.StatusCode = 404;
                return false;
            }

            if (artifact is not Model asModel || asModel.EntityContainer.Id != projectId)
            {
                context.Response.StatusCode = 400;
                return false;
            }

            model = asModel;
            return true;
        }
    }
}
