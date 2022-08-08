// --------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationDto.cs" company="RHEA System S.A.">
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
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Authentication information used to login to the UI-DSM server
    /// </summary>
    public class AuthenticationDto
    {
        /// <summary>
        /// Gets or sets the username to use to authenticate
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password to use to authenticate
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
