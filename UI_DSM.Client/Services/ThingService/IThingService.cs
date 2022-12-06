// --------------------------------------------------------------------------------------------------------
// <copyright file="IThingService.cs" company="RHEA System S.A.">
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

    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ThingService" />
    /// </summary>
    public interface IThingService
    {
        /// <summary>
        ///     Gets a collection of <see cref="Thing" /> contained into <see cref="Model" />s
        /// </summary>
        /// <param name="projectId">The <see cref="Project" /> id where <see cref="Model" />s are contained</param>
        /// <param name="modelsId"></param>
        /// <param name="classKind"></param>
        /// <returns></returns>
        Task<IEnumerable<Thing>> GetThings(Guid projectId, IEnumerable<Guid> modelsId, ClassKind classKind = ClassKind.Iteration);

        /// <summary>
        ///     Gets a collection of <see cref="Thing" /> contained into a <see cref="Model" />
        /// </summary>
        /// <param name="projectId">The <see cref="Project" /> id where <see cref="Model" />s are contained</param>
        /// <param name="modelId">The <see cref="Model" /> id</param>
        /// <param name="classKind">The <see cref="ClassKind" /></param>
        /// <returns>A <see cref="Task" /> with the collection of retrieved <see cref="Thing" /></returns>
        Task<IEnumerable<Thing>> GetThings(Guid projectId, Guid modelId, ClassKind classKind = ClassKind.Iteration);

        /// <summary>
        ///     Gets a collection of <see cref="Thing" /> contained into a <see cref="Model" />
        /// </summary>
        /// <param name="projectId">The <see cref="Project" /> id where <see cref="Model" />s are contained</param>
        /// <param name="model">The <see cref="Model" /></param>
        /// <param name="classKind">The <see cref="ClassKind" /></param>
        /// <returns>A <see cref="Task" /> with the collection of retrieved <see cref="Thing" /></returns>
        Task<IEnumerable<Thing>> GetThings(Guid projectId, Model model, ClassKind classKind = ClassKind.Iteration);

        /// <summary>
        ///     Gets a collection of <see cref="Thing" /> that will be needed for the current <see cref="View" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="modelsId">A collection of <see cref="Guid" /> for <see cref="Model" /></param>
        /// <param name="currentView">The current <see cref="View" /></param>
        /// <returns>A collection of <see cref="Thing" /></returns>
        Task<IEnumerable<Thing>> GetThingsByView(Guid projectId, IEnumerable<Guid> modelsId, View currentView);
    }
}
