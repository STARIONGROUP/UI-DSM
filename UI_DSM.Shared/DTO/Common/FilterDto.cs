// --------------------------------------------------------------------------------------------------------
// <copyright file="FilterDto.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.DTO.Common
{
    using CDP4Common.CommonData;

    /// <summary>
    ///     Data Transfer Object that represents selected filters
    /// </summary>
    public class FilterDto
    {
        /// <summary>
        ///     The <see cref="ClassKind" />
        /// </summary>
        public ClassKind ClassKind { get; set; }

        /// <summary>
        ///     A collection of <see cref="Guid" /> of filters that has been selected
        /// </summary>
        public List<Guid> SelectedFilters { get; set; }
    }
}
