// --------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationResponseDto.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.DTO
{
    /// <summary>
    /// Authentication response after an authentication request
    /// </summary>
    public class AuthenticationResponseDto
    {
        /// <summary>
        /// Value asserting that the authentication proceeded successfully
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// The message in case of authentication failure
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// The authentication token
        /// </summary>
        public string Token { get; set; }
    }
}
