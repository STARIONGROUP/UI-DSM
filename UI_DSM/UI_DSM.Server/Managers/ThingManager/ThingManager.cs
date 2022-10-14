// --------------------------------------------------------------------------------------------------------
// <copyright file="ThingManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.ThingManager
{
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using UI_DSM.Server.Services.CometService;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Manager to get <see cref="Thing" /> for a Comet model
    /// </summary>
    public class ThingManager : IThingManager
    {
        /// <summary>
        ///     The <see cref="ICometService" />
        /// </summary>
        private readonly ICometService cometService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ThingManager" /> class.
        /// </summary>
        /// <param name="cometService">The <see cref="ICometService" /></param>
        public ThingManager(ICometService cometService)
        {
            this.cometService = cometService;
        }

        /// <summary>
        ///     Gets all <see cref="Iteration" /> will all contained <see cref="Thing" />
        ///     that is contained into mutliple <see cref="Model" />
        /// </summary>
        /// <param name="models">A collection of <see cref="Model" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Thing" /></returns>
        public async Task<IEnumerable<Thing>> GetIterations(IEnumerable<Model> models)
        {
            var things = new List<Thing>();

            foreach (var model in models)
            {
                things.AddRange(await this.GetIteration(model));
            }

            return things.DistinctById();
        }

        /// <summary>
        ///     Gets all <see cref="Thing" />s contained into <see cref="Model" />s from a collection where
        ///     <see cref="Thing.ClassKind" />
        ///     is the provided <see cref="ClassKind" />
        /// </summary>
        /// <param name="models">The collection of <see cref="Model" /></param>
        /// <param name="classKind">The <see cref="ClassKind" /></param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="Thing" /></returns>
        public async Task<IEnumerable<Thing>> GetThings(IEnumerable<Model> models, ClassKind classKind)
        {
            var things = new List<Thing>();

            foreach (var model in models)
            {
                things.AddRange(await this.GetThings(model, classKind));
            }

            return things.DistinctById();
        }

        /// <summary>
        ///     Gets all <see cref="Thing" />s contained into a <see cref="Model" /> from a collection where
        ///     <see cref="Thing.ClassKind" /> is the provided <see cref="ClassKind" />
        /// </summary>
        /// <param name="model">A <see cref="Model" /></param>
        /// <param name="classKind">The <see cref="ClassKind" /></param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="Thing" /></returns>
        /// <remarks>The <see cref="Iteration" /> is also included into the retrieved collection</remarks>
        private async Task<IEnumerable<Thing>> GetThings(Model model, ClassKind classKind)
        {
            var iteration = await this.cometService.GetIteration(model);

            if (iteration == null)
            {
                return Enumerable.Empty<Thing>();
            }

            if (!iteration.TryGetCollectionOfType(classKind, out var thingCollection))
            {
                return Enumerable.Empty<Thing>();
            }

            var things = thingCollection.GetContainedAndReferencedThings().ToList();
            things.Add(iteration);
            return things;
        }

        /// <summary>
        ///     Gets all <see cref="Iteration" /> will all contained <see cref="Thing" />
        ///     that is contained into a <see cref="Model" />
        /// </summary>
        /// <param name="model">The <see cref="Model" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Thing" /></returns>
        private async Task<IEnumerable<Thing>> GetIteration(Model model)
        {
            var iteration = await this.cometService.GetIteration(model);
            
            return iteration == null ? Enumerable.Empty<Thing>() : iteration.GetContainedAndReferencedThings();
        }
    }
}
