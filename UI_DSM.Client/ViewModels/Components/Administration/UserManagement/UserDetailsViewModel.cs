// --------------------------------------------------------------------------------------------------------
// <copyright file="UserDetailsViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.Administration.UserManagement
{
    using UI_DSM.Client.Components.Administration.UserManagement;
    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     View model for <see cref="UserDetails" /> component
    /// </summary>
    public class UserDetailsViewModel : IUserDetailsViewModel
    {
        /// <summary>
        ///     The <see cref="UserEntityDto" />
        /// </summary>
        public UserEntityDto UserEntity { get; set; }
    }
}
