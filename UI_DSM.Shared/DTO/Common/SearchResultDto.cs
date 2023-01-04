// --------------------------------------------------------------------------------------------------------
// <copyright file="SearchResultDto.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.DTO.Common
{
    using CDP4Common.EngineeringModelData;

    using UI_DSM.Shared.Enumerator;

    /// <summary>
    ///     Data Transfer Object that represent the result of a search query
    /// </summary>
    public class SearchResultDto
    {
        /// <summary>
        ///     The kind of object
        /// </summary>
        public string ObjectKind { get; set; }

        /// <summary>
        ///     The text to display
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>
        ///     The base url where the object is visible
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        ///     The location of the current ressource
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        ///     A specific category applied to an item
        /// </summary>
        public string SpecificCategory { get; set; }

        /// <summary>
        ///     A collection of <see cref="View" /> where the current item visible
        /// </summary>
        public IEnumerable<View> AvailableViews { get; set; }

        /// <summary>
        ///     The <see cref="Guid" /> of the item
        /// </summary>
        public Guid ItemId { get; set; }

        /// <summary>
        ///     The <see cref="Guid" /> of the containing <see cref="Iteration" />
        /// </summary>
        public Guid IterationId { get; set; }
    }
}
