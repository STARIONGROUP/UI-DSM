// --------------------------------------------------------------------------------------------------------
// <copyright file="RegistrationDto.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.DTO.UserManagement
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Registration information used to create to the UI-DSM server
    /// </summary>
    public class RegistrationDto : AuthenticationDto
    {
        /// <summary>
        /// Confirm password to be sure that the administrator has correctly typed the password
        /// </summary>
        [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
