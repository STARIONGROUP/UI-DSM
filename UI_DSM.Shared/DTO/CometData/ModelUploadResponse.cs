// --------------------------------------------------------------------------------------------------------
// <copyright file="ModelUploadResponse.cs" company="RHEA System S.A.">
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
    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    ///     Data used to has response to an upload of a Comet model
    /// </summary>
    public class ModelUploadResponse : RequestResponseDto
    {
        /// <summary>
        ///     The Path of the uploaded file
        /// </summary>
        public string UploadedFilePath { get; set; }
    }
}
