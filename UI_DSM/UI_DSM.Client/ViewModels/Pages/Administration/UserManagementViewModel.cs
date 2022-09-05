// --------------------------------------------------------------------------------------------------------
// <copyright file="UserManagementViewModel.cs" company="RHEA System S.A.">
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
    using DevExpress.Blazor;

    using DynamicData;

    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.Components.Administration.UserManagement;
    using UI_DSM.Client.Pages.Administration;
    using UI_DSM.Client.Services.Administration.UserService;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.Administration.UserManagement;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.DTO.UserManagement;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="UserManagement" /> page
    /// </summary>
    public class UserManagementViewModel : ReactiveObject, IUserManagementViewModel
    {
        /// <summary>
        ///     The <see cref="IUserService" />
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        ///     Backing field for <see cref="IsOnCreationMode" />
        /// </summary>
        private bool isOnCreationMode;

        /// <summary>
        ///     Backing field for <see cref="IsOnDetailsViewMode" />
        /// </summary>
        private bool isOnDetailsViewMode;

        /// <summary>
        ///     The user that the administrator is going to delete
        /// </summary>
        private UserDto userToDelete;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserManagementViewModel" /> class.
        /// </summary>
        public UserManagementViewModel(IUserService userService)
        {
            this.userService = userService;

            this.ConfirmCancelPopup = new ConfirmCancelPopupViewModel
            {
                ConfirmRenderStyle = ButtonRenderStyle.Danger,
                OnCancel = new EventCallbackFactory().Create(this, this.OnCancelDelete),
                OnConfirm = new EventCallbackFactory().Create(this, this.OnConfirmDelete),
                HeaderText = "Are you sure ?"
            };

            this.UserRegistrationViewModel = new UserRegistrationViewModel
            {
                OnValidSubmit = new EventCallbackFactory().Create(this, this.RegisterUser)
            };
        }

        /// <summary>
        ///     The <see cref="ErrorMessageViewModel" />
        /// </summary>
        public IErrorMessageViewModel ErrorMessageViewModel { get; private set; } = new ErrorMessageViewModel();

        /// <summary>
        ///     The <see cref="IUserDetailsViewModel" />
        /// </summary>
        public IUserDetailsViewModel UserDetailsViewModel { get; private set; } = new UserDetailsViewModel();

        /// <summary>
        ///     The <see cref="IUserManagementViewModel" />
        /// </summary>
        public IUserRegistrationViewModel UserRegistrationViewModel { get; private set; }

        /// <summary>
        ///     Value indicating the user is currently creating a new <see cref="User"/>
        /// </summary>
        public bool IsOnCreationMode
        {
            get => this.isOnCreationMode;
            set => this.RaiseAndSetIfChanged(ref this.isOnCreationMode, value);
        }

        /// <summary>
        ///     Value indicating the user is currently visualizing details on a  <see cref="UserDto"/>
        /// </summary>
        public bool IsOnDetailsViewMode
        {
            get => this.isOnDetailsViewMode;
            set => this.RaiseAndSetIfChanged(ref this.isOnDetailsViewMode, value);
        }

        /// <summary>
        ///     The <see cref="IConfirmCancelPopupViewModel" />
        /// </summary>
        public IConfirmCancelPopupViewModel ConfirmCancelPopup { get; private set; }

        /// <summary>
        ///     A collections of <see cref="UserDto" />
        /// </summary>
        public SourceList<UserDto> Users { get; private set; } = new();

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        public async Task OnInitializedAsync()
        {
            this.Users.AddRange(await this.userService.GetUsers());
        }

        /// <summary>
        ///     Opens the <see cref="UserRegistration" /> as a popup
        /// </summary>
        public void OpenRegisterPopup()
        {
            this.ErrorMessageViewModel.Errors.Clear();
            this.UserRegistrationViewModel.Registration = new RegistrationDto();
            this.IsOnCreationMode = true;
        }

        /// <summary>
        ///     Opens a popup to confirm to delete a registered user
        /// </summary>
        /// <param name="user">The <see cref="UserDto" /> to remove</param>
        public void AskConfirmDeleteUser(UserDto user)
        {
            this.userToDelete = user;
            this.ConfirmCancelPopup.ContentText = $"You are about to delete the user {this.userToDelete.UserName}.\nAre you sure?";
            this.ConfirmCancelPopup.IsVisible = true;
        }

        /// <summary>
        ///     Opens a popup to provide informations for the given <see cref="UserDto" />
        /// </summary>
        /// <param name="user">The <see cref="UserDto" /></param>
        public void OpenDetailsPopup(UserDto user)
        {
            this.UserDetailsViewModel.User = user;
            this.IsOnDetailsViewMode = true;
        }

        /// <summary>
        ///     Tries to register a new user
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task RegisterUser()
        {
            var registerResult = await this.userService.RegisterUser(this.UserRegistrationViewModel.Registration);
            this.IsOnCreationMode = !registerResult.IsRequestSuccessful;
            this.ErrorMessageViewModel.Errors.Clear();

            if (registerResult.Errors != null)
            {
                this.ErrorMessageViewModel.Errors.AddRange(registerResult.Errors);
            }

            if (registerResult.IsRequestSuccessful)
            {
                this.Users.Add(registerResult.CreatedUser);
            }
        }

        /// <summary>
        ///     Callback used when the confirmation dialog has confirmed the deletion
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task OnConfirmDelete()
        {
            var deleteResult = await this.userService.DeleteUser(this.userToDelete);

            if (deleteResult.IsRequestSuccessful)
            {
                this.Users.Remove(this.userToDelete);
            }

            this.OnCancelDelete();
        }

        /// <summary>
        ///     Callback used when the confirmation dialog has canceled the deletion
        /// </summary>
        private void OnCancelDelete()
        {
            this.ConfirmCancelPopup.IsVisible = false;
            this.userToDelete = null;
        }
    }
}
