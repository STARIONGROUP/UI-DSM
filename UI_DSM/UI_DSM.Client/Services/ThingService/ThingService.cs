// --------------------------------------------------------------------------------------------------------
// <copyright file="ThingService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.ThingService
{
    using CDP4Common.CommonData;

    using CDP4Dal;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.WebUtilities;

    using UI_DSM.Client.Extensions;
    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This <see cref="ServiceBase" /> provides capabilities to query <see cref="Thing" /> contained into a
    ///     <see cref="Model" />
    /// </summary>
    [Route("Project/{0}/Model/{1}/Thing")]
    public class ThingService : ServiceBase, IThingService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceBase" /> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="ServiceBase.HttpClient" /></param>
        /// <param name="jsonService">The <see crf="IJsonService" /></param>
        public ThingService(HttpClient httpClient, IJsonService jsonService) : base(httpClient, jsonService)
        {
        }

        /// <summary>
        ///     Gets a collection of <see cref="Thing" /> contained into <see cref="Model" />s
        /// </summary>
        /// <param name="projectId">The <see cref="Project" /> id where <see cref="Model" />s are contained</param>
        /// <param name="modelsId">A collection of <see cref="Model" /> id</param>
        /// <param name="classKind">The <see cref="ClassKind" /></param>
        /// <returns>A <see cref="Task" /> with the collection of retrieved <see cref="Thing" /></returns>
        public async Task<IEnumerable<Thing>> GetThings(Guid projectId, IEnumerable<Guid> modelsId, ClassKind classKind = ClassKind.Iteration)
        {
            var models = modelsId.ToList();

            if (!models.Any())
            {
                return Enumerable.Empty<Thing>();
            }

            this.ComputeMainRoute(projectId, models);
            var uri = this.MainRoute;

            if (classKind != ClassKind.Iteration)
            {
                uri = QueryHelpers.AddQueryString(uri, nameof(ClassKind), classKind.ToString());
            }

            var response = await this.HttpClient.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
            {
                return Enumerable.Empty<Thing>();
            }

            var thingsDto = this.jsonService.Deserialize<IEnumerable<CDP4Common.DTO.Thing>>(await response.Content.ReadAsStreamAsync());
            var assembler = new Assembler(this.HttpClient.BaseAddress);
            await assembler.Synchronize(thingsDto);
            var things = assembler.Cache.Values.Where(x => x.IsValueCreated).Select(x => x.Value).ToList();
            return things;
        }

        /// <summary>
        ///     Gets a collection of <see cref="Thing" /> contained into a <see cref="Model" />
        /// </summary>
        /// <param name="projectId">The <see cref="Project" /> id where <see cref="Model" />s are contained</param>
        /// <param name="modelId">The <see cref="Model" /> id</param>
        /// <param name="classKind">The <see cref="ClassKind" /></param>
        /// <returns>A <see cref="Task" /> with the collection of retrieved <see cref="Thing" /></returns>
        public Task<IEnumerable<Thing>> GetThings(Guid projectId, Guid modelId, ClassKind classKind = ClassKind.Iteration)
        {
            return this.GetThings(projectId, new List<Guid> { modelId }, classKind);
        }

        /// <summary>
        ///     Gets a collection of <see cref="Thing" /> that will be needed for the current <see cref="View" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="modelsId">A collection of <see cref="Guid" /> for <see cref="Model" /></param>
        /// <param name="currentView">The current <see cref="View" /></param>
        /// <returns>A collection of <see cref="Thing" /></returns>
        public async Task<IEnumerable<Thing>> GetThingsByView(Guid projectId, IEnumerable<Guid> modelsId, View currentView)
        {
            var things = new List<Thing>();

            switch (currentView)
            {
                case View.RequirementBreakdownStructureView:
                case View.RequirementTraceabilityToRequirementView:
                    things.AddRange(await this.GetThings(projectId, modelsId, ClassKind.RequirementsSpecification));
                    break;
                case View.ProductBreakdownStructureView:
                    things.AddRange(await this.GetThings(projectId, modelsId, ClassKind.ElementDefinition));
                    break;
                case View.RequirementTraceabilityToProductView:
                    things.AddRange(await this.GetThings(projectId, modelsId));
                    break;
            }

            return things;
        }

        /// <summary>
        ///     Computes the <see cref="ServiceBase.MainRoute" /> to include custom <see cref="Guid" />
        /// </summary>
        /// <param name="projectId">The <see cref="Project" /> <see cref="Guid" /></param>
        /// <param name="modelsId">A collection of <see cref="Guid" /> for <see cref="Model" /></param>
        private void ComputeMainRoute(Guid projectId, IEnumerable<Guid> modelsId)
        {
            this.MainRoute = string.Format(this.GetRoute(), projectId, modelsId.ToGuidArray());
        }
    }
}
