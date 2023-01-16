// --------------------------------------------------------------------------------------------------------
// <copyright file="ReportDto.cs" company="RHEA System S.A.">
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
    ///     Data Transfer Object to transfer information related to a single report
    /// </summary>
    public class ReportDto
    {
        /// <summary>
        ///     Gets or sets the report name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the ProjectId
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="IterationId"/>
        /// </summary>
        public Guid IterationId { get; set; }
    }
}
