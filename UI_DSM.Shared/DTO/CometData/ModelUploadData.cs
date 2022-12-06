// --------------------------------------------------------------------------------------------------------
// <copyright file="ModelSelectionData.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.DTO.CometData
{
    /// <summary>
    ///     Data used to upload a specific iteration from a specific Engineering model
    /// </summary>
    public class ModelUploadData
    {
        /// <summary>
        ///     The <see cref="Guid" /> of the Engineering Model
        /// </summary>
        public Guid ModelId { get; set; }

        /// <summary>
        ///     The <see cref="Guid" /> of the frozen Iteration
        /// </summary>
        public Guid IterationId { get; set; }
    }
}
