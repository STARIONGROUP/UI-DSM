// --------------------------------------------------------------------------------------------------------
// <copyright file="CommonBaseSearchDto.cs" company="RHEA System S.A.">
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
    using GP.SearchService.SDK.Definitions;

    /// <summary>
    ///     A <see cref="ISearchDto" />
    /// </summary>
    public class CommonBaseSearchDto : ISearchDto
    {
        /// <summary>
        ///     The id of the dto
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     The type
        /// </summary>
        public string Type { get; set; }
    }
}
