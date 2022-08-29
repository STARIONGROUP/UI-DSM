// --------------------------------------------------------------------------------------------------------
// <copyright file="ILoginViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components
{
    using Microsoft.AspNetCore.Components.Forms;

    using UI_DSM.Client.Enumerator;
    using UI_DSM.Shared.DTO.UserManagement;

    /// <summary>
    ///     Interface definition for <see cref="LoginViewModel" />
    /// </summary>
    public interface ILoginViewModel
    {
        /// <summary>
        ///     The <see cref="AuthenticationDto" /> used for the <see cref="EditForm" />
        /// </summary>
        AuthenticationDto Authentication { get; }

        /// <summary>
        ///     Gets or sets the <see cref="Enumerator.AuthenticationStatus" />
        /// </summary>
        AuthenticationStatus AuthenticationStatus { get; set; }

        /// <summary>
        ///     Gets or sets the login error message
        /// </summary>
        string ErrorMessage { get; set; }

        /// <summary>
        ///     Tries to authenticate to the server
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        Task ExecuteLogin();

        void NavigateIfLoggedIn();
    }
}
