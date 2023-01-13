// --------------------------------------------------------------------------------------------------------
// <copyright file="AnnotationLinkDto.cs" company="RHEA System S.A.">
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
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Data Transfer Object that represents the link of a <see cref="Annotation" /> to multiple <see cref="ReviewItem" />
    /// </summary>
    public class AnnotationLinkDto
    {
        /// <summary>
        ///     The <see cref="Guid" /> of the exisitng <see cref="Annotation" />
        /// </summary>
        public Guid AnnotationId { get; set; }

        /// <summary>
        ///     The collection of <see cref="Guid" /> of <see cref="Thing" />s
        /// </summary>
        public IEnumerable<Guid> ThingsId { get; set; }
    }
}
