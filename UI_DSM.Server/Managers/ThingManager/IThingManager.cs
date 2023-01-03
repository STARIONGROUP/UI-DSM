// --------------------------------------------------------------------------------------------------------
// <copyright file="IThingManager.cs" company="RHEA System S.A.">
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

    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ThingManager" />
    /// </summary>
    public interface IThingManager
    {
        /// <summary>
        ///     Gets all <see cref="Iteration" /> will all contained <see cref="Thing" />
        ///     that is contained into mutliple <see cref="Model" />
        /// </summary>
        /// <param name="models">A collection of <see cref="Model" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Thing" /></returns>
        Task<IEnumerable<Thing>> GetIterations(IEnumerable<Model> models);

        /// <summary>
        ///     Gets all <see cref="Thing" />s contained into <see cref="Model" />s from a collection where
        ///     <see cref="Thing.ClassKind" />
        ///     is the provided <see cref="ClassKind" />
        /// </summary>
        /// <param name="models">The collection of <see cref="Model" /></param>
        /// <param name="classKind">The <see cref="ClassKind" /></param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="Thing" /></returns>
        Task<IEnumerable<Thing>> GetThings(IEnumerable<Model> models, ClassKind classKind);

        /// <summary>
        ///     Tries to get a <see cref="Thing" /> based on a <see cref="CommonBaseSearchDto" />
        /// </summary>
        /// <param name="commonBaseSearchDto">The <see cref="CommonBaseSearchDto" /></param>
        /// <param name="model">A <see cref="Model" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="Thing" /></returns>
        Task<Thing> GetThing(CommonBaseSearchDto commonBaseSearchDto, Model model);
    }
}
