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
    using UI_DSM.Shared.Models;

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
        ///     Value indicating the user is currently creating a new <see cref="User"/>
        /// </summary>
        bool IsOnCreationMode { get; set; }

        /// <summary>
        ///     Value indicating if the <see cref="UserDetails" /> popup should be visible
        /// </summary>
        bool IsOnDetailsViewMode { get; set; }

        /// <summary>
        ///     The <see cref="IConfirmCancelPopupViewModel" />
        /// </summary>
        IConfirmCancelPopupViewModel ConfirmCancelPopup { get; }

        /// <summary>
        ///     A collections of <see cref="UserEntityDto" />
        /// </summary>
        SourceList<UserEntityDto> Users { get; }

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
        /// <param name="userEntity">The <see cref="UserEntityDto" /> to remove</param>
        void AskConfirmDeleteUser(UserEntityDto userEntity);

        /// <summary>
        ///     Opens a popup to provide informations for the given <see cref="UserEntityDto" />
        /// </summary>
        /// <param name="userEntity">The <see cref="UserEntityDto" /></param>
        void OpenDetailsPopup(UserEntityDto userEntity);
    }
}
