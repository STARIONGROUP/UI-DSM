// --------------------------------------------------------------------------------------------------------
// <copyright file="RoleManagementViewModel.cs" company="RHEA System S.A.">
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

    using UI_DSM.Client.Components.Administration.RoleManagement;
    using UI_DSM.Client.Pages.Administration;
    using UI_DSM.Client.Services.Administration.RoleService;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.Administration.RoleManagement;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Wrappers;

    /// <summary>
    ///     View model for the <see cref="RoleManagement" /> page
    /// </summary>
    public class RoleManagementViewModel : ReactiveObject, IRoleManagementViewModel
    {
        /// <summary>
        ///     The <see cref="IRoleService" />
        /// </summary>
        private readonly IRoleService roleService;

        /// <summary>
        ///     Backing field for <see cref="IsCreationPopupVisible" />
        /// </summary>
        private bool isCreationPopupVisible;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RoleManagementViewModel" /> class.
        /// </summary>
        /// <param name="roleService">The <see cref="IRoleService" /></param>
        /// <param name="navigationManager">The <see cref="NavigationManager"/></param>
        public RoleManagementViewModel(IRoleService roleService , NavigationManager navigationManager)
        {
            this.roleService = roleService;

            this.RoleCreationViewModel = new RoleCreationViewModel
            {
                OnValidSubmit = new EventCallbackFactory().Create(this, this.CreateRole)
            };

            this.NavigationManager = navigationManager;
        }

        /// <summary>
        ///     Gets or sets the <see cref="NavigationManager" />
        /// </summary>
        public NavigationManager NavigationManager { get; set; }

        /// <summary>
        ///     The <see cref="IErrorMessageViewModel" />
        /// </summary>
        public IErrorMessageViewModel ErrorMessageViewModel { get; } = new ErrorMessageViewModel();

        /// <summary>
        ///     Value indicating if the <see cref="DxPopup" /> for the <see cref="RoleCreation" /> component is visible
        /// </summary>
        public bool IsCreationPopupVisible
        {
            get => this.isCreationPopupVisible;
            set => this.RaiseAndSetIfChanged(ref this.isCreationPopupVisible, value);
        }

        /// <summary>
        ///     Gets the <see cref="IRoleCreationViewModel" />
        /// </summary>
        public IRoleCreationViewModel RoleCreationViewModel { get; private set; }

        /// <summary>
        ///     A collection of <see cref="Role" />
        /// </summary>
        public SourceList<Role> Roles { get; private set; } = new();

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        public async Task OnInitializedAsync()
        {
            this.Roles.AddRange(await this.roleService.GetRoles());
        }

        /// <summary>
        ///     Opens the <see cref="RoleCreation" /> component as a <see cref="DxPopup" />
        /// </summary>
        public void OpenCreationPopup()
        {
            this.RoleCreationViewModel.Role = new Role();
            this.RoleCreationViewModel.SelectedAccessRights = new List<AccessRightWrapper>();
            this.IsCreationPopupVisible = true;
        }

        /// <summary>
        ///     Navigate to the page dedicated to the given <see cref="Role" />
        /// </summary>
        /// <param name="role">The <see cref="Role" /></param>
        public void GoToRolePage(Role role)
        {
            this.NavigationManager.NavigateTo($"Administration/Role/{role.Id}");
        }

        /// <summary>
        ///     Tries to create a new <see cref="Role" />
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task CreateRole()
        {
            try
            {
                this.RoleCreationViewModel.Role.AccessRights = this.RoleCreationViewModel.SelectedAccessRights
                    .Select(x => x.AccessRight).ToList();

                var creationResult = await this.roleService.CreateRole(this.RoleCreationViewModel.Role);

                this.ErrorMessageViewModel.Errors.Clear();

                if (creationResult.Errors.Any())
                {
                    this.ErrorMessageViewModel.Errors.AddRange(creationResult.Errors);
                }

                if (creationResult.IsRequestSuccessful)
                {
                    this.Roles.Add(creationResult.Entity);
                }

                this.IsCreationPopupVisible = !creationResult.IsRequestSuccessful;
            }
            catch (Exception exception)
            {
                this.ErrorMessageViewModel.Errors.Clear();
                this.ErrorMessageViewModel.Errors.Add(exception.Message);
            }
        }
    }
}
