// --------------------------------------------------------------------------------------------------------
// <copyright file="CometAuthenticationResponse.cs" company="RHEA System S.A.">
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

using UI_DSM.Shared.DTO.Common;

namespace UI_DSM.Shared.DTO.CometData
{
    /// <summary>
    ///     A <see cref="RequestResponseDto" /> for a to use after tries to log to Comet
    /// </summary>
    public class CometAuthenticationResponse : RequestResponseDto
    {
        /// <summary>
        ///     The <see cref="Guid" /> of the current session to Comet
        /// </summary>
        public Guid SessionId { get; set; }
    }
}
