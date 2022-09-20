// --------------------------------------------------------------------------------------------------------
// <copyright file="EntityRequestResponseDto.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.DTO.Common
{
    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     Common DTO used to respond to all basic operation (creation/update/delete) request on <see cref="EntityDto" />
    /// </summary>
    public class EntityRequestResponseDto: RequestResponseDto
    {
        /// <summary>
        ///     Gets or sets the collection of <see cref="EntityDto" />
        /// </summary>
        public IEnumerable<EntityDto> Entities { get; set; } = new List<EntityDto>();
    }
}
