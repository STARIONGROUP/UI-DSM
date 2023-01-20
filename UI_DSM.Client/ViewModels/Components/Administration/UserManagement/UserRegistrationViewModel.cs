// --------------------------------------------------------------------------------------------------------
// <copyright file="UserRegistrationViewModel.cs" company="RHEA System S.A.">
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
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Components.Administration.UserManagement;
    using UI_DSM.Shared.DTO.UserManagement;

    /// <summary>
    ///     View model for the <see cref="UserRegistration" /> component
    /// </summary>
    public class UserRegistrationViewModel : IUserRegistrationViewModel
    {
        /// <summary>
        ///     The <see cref="EventCallback" /> to call for data submit
        /// </summary>
        public EventCallback OnValidSubmit { get; set; }

        /// <summary>
        ///     The <see cref="RegistrationDto" /> that will be use to register the user
        /// </summary>
        public RegistrationDto Registration { get; set; }
    }
}
