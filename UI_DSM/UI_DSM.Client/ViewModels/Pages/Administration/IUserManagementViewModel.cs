// --------------------------------------------------------------------------------------------------------
// <copyright file="IUserManagementViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Pages.Administration
{
    using DynamicData;

    using UI_DSM.Client.Components.Administration.UserManagement;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.Administration.UserManagement;
    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     Interface definition for <see cref="UserManagementViewModel" />
    /// </summary>
    public interface IUserManagementViewModel
    {
        /// <summary>
        ///     The <see cref="IUserManagementViewModel" />
        /// </summary>
        IUserRegistrationViewModel UserRegistrationViewModel { get; }

        /// <summary>
        ///     Value indicating if the <see cref="UserRegistration" /> popup should be visible
        /// </summary>
        bool RegistrationPopupVisible { get; set; }

        /// <summary>
        ///     Value indicating if the <see cref="UserDetails" /> popup should be visible
        /// </summary>
        bool UserDetailsPopupVisible { get; set; }

        /// <summary>
        ///     The <see cref="IConfirmCancelPopupViewModel" />
        /// </summary>
        IConfirmCancelPopupViewModel ConfirmCancelPopup { get; }

        /// <summary>
        ///     A collections of <see cref="UserDto" />
        /// </summary>
        SourceList<UserDto> Users { get; }

        /// <summary>
        ///     The <see cref="ErrorMessageViewModel" />
        /// </summary>
        IErrorMessageViewModel ErrorMessageViewModel { get; }

        /// <summary>
        ///     The <see cref="IUserDetailsViewModel" />
        /// </summary>
        IUserDetailsViewModel UserDetailsViewModel { get; }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        Task OnInitializedAsync();

        /// <summary>
        ///     Opens the <see cref="UserRegistration" /> as a popup
        /// </summary>
        void OpenRegisterPopup();

        /// <summary>
        ///     Opens a popup to confirm to delete a registered user
        /// </summary>
        /// <param name="user">The <see cref="UserDto" /> to remove</param>
        void AskConfirmDeleteUser(UserDto user);

        /// <summary>
        ///     Opens a popup to provide informations for the given <see cref="UserDto" />
        /// </summary>
        /// <param name="user">The <see cref="UserDto" /></param>
        void OpenDetailsPopup(UserDto user);
    }
}
