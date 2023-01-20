// --------------------------------------------------------------------------------------------------------
// <copyright file="DiagramLinkDto.cs" company="RHEA System S.A.">
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
    /// <summary>
    ///     Common DTO used to save to all the diagram link configuration
    /// </summary>
    public class DiagramLinkDto
    {
        /// <summary>
        ///     The linked <see cref="Thing" /> id
        /// </summary>
        public Guid ThingId { get; set; }

        /// <summary>
        ///     A collection of Vertices
        /// </summary>
        public List<PointDto> Vertices { get; set; }
    }
}
