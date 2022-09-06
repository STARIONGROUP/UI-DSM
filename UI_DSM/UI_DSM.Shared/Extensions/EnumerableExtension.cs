// --------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtension.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Shared.Extensions
{
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Extension class for <see cref="IEnumerable{Entity}" />
    /// </summary>
    public static class EnumerableExtension
    {
        /// <summary>
        ///     Convert any <see cref="IEnumerable{Entity}" /> to a <see cref="List{EntityDto}" />
        /// </summary>
        /// <param name="collection">A collection of <see cref="Entity" /></param>
        /// <returns>A <see cref="List{EntityDto}" /></returns>
        public static List<EntityDto> ToDtos(this IEnumerable<Entity> collection)
        {
            return collection.Select(x => x.ToDto()).ToList();
        }
    }
}
