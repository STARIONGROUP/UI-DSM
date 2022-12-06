// --------------------------------------------------------------------------------------------------------
// <copyright file="ModelsDataResponse.cs" company="RHEA System S.A.">
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
    ///     Data used to reply to the get query for getting available EngineeringModel
    /// </summary>
    public class ModelsDataResponse : RequestResponseDto
    {
        /// <summary>
        ///     A collection of <see cref="EngineeringModelData" />
        /// </summary>
        public List<EngineeringModelData> AvailableModels { get; set; } = new();
    }
}
