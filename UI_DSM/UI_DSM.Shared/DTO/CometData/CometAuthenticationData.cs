// --------------------------------------------------------------------------------------------------------
// <copyright file="CometAuthenticationData.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Shared.DTO.CometData
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     Data used to establish a connection to a Comet session
    /// </summary>
    public class CometAuthenticationData
    {
        /// <summary>
        ///     Gets or sets the Url where the Comet instance can be reached
        /// </summary>
        [Required]
        [Url]
        public string Url { get; set; }

        /// <summary>
        ///     Gets or sets the Username for the Comet login
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        ///     Gets or sets the Password for the Comet login
        /// </summary>
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }
    }
}
