// --------------------------------------------------------------------------------------------------------
// <copyright file="DiagramDto.cs" company="RHEA System S.A.">
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
    ///     Data Transfer Object that represents a Diagram
    /// </summary>
    public class DiagramDto
    {
        /// <summary>
        ///     A collection of <see cref="DiagramNodeDto" />
        /// </summary>
        public List<DiagramNodeDto> Nodes { get; set; }

        /// <summary>
        ///     A collection of <see cref="DiagramLinkDto" />
        /// </summary>
        public List<DiagramLinkDto> Links { get; set; }

        /// <summary>
        ///     A collection of <see cref="FilterDto" />
        /// </summary>
        public List<FilterDto> Filters { get; set; }
    }
}
