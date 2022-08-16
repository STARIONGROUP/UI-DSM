// --------------------------------------------------------------------------------------------------------
// <copyright file="RegistrationResponseDto.cs" company="RHEA System S.A.">
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
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     Response used after a registration request
    /// </summary>
    public class RegistrationResponseDto : RequestResponseDto
    {
        /// <summary>
        ///     The created <see cref="UserDto" /> if the registration went successfully
        /// </summary>
        public UserDto CreatedUser { get; set; }
    }
}
