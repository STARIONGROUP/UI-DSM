// --------------------------------------------------------------------------------------------------------
// <copyright file="RolePageViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Pages.Administration.RolePages
{
    using System.Reactive.Linq;

    using DynamicData;

    using ReactiveUI;

    using UI_DSM.Client.Components.Administration.RoleManagement;
    using UI_DSM.Client.Pages.Administration.RolePages;
    using UI_DSM.Client.Services.Administration.RoleService;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.Administration.RoleManagement;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="RolePage" /> page
    /// </summary>
    public class RolePageViewModel : ReactiveObject, IRolePageViewModel, IDisposable
    {
        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     The <see cref="IRoleService" />
        /// </summary>
        private readonly IRoleService roleService;

        /// <summary>
        ///     Name of the role without any modification
        /// </summary>
        private string initialRoleName;

        /// <summary>
        ///     Backing field for <see cref="ModificationEnabled" />
        /// </summary>
        private bool modificationEnable;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RolePageViewModel" /> class.
        /// </summary>
        /// <param name="roleService">The <see cref="IRoleService" /></param>
        public RolePageViewModel(IRoleService roleService)
        {
            this.roleService = roleService;

            this.disposables.Add(this.WhenAnyValue(x => x.ModificationEnabled)
                .Where(x => !x)
                .Subscribe(_ => this.ResetRole()));
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.disposables.ForEach(x => x.Dispose());
            this.disposables.Clear();
        }

        /// <summary>
        ///     The <see cref="IRoleDetailsViewModel" /> for the <see cref="RoleDetails" /> component
        /// </summary>
        public IRoleDetailsViewModel RoleDetailsViewModel { get; } = new RoleDetailsViewModel();

        /// <summary>
        ///     Value indicating if <see cref="Role" /> values are editable or not
        /// </summary>
        public bool ModificationEnabled
        {
            get => this.modificationEnable;
            set => this.RaiseAndSetIfChanged(ref this.modificationEnable, value);
        }

        /// <summary>
        ///     The <see cref="IErrorMessageViewModel" />
        /// </summary>
        public IErrorMessageViewModel ErrorMessageViewModel { get; } = new ErrorMessageViewModel();

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <param name="roleGuid">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        public async Task OnInitializedAsync(Guid roleGuid)
        {
            this.RoleDetailsViewModel.Role = await this.roleService.GetRole(roleGuid);

            this.initialRoleName = this.RoleDetailsViewModel.Role?.RoleName;
        }

        /// <summary>
        ///     Update the current <see cref="Role" />
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        public async Task UpdateRole()
        {
            if (this.RoleDetailsViewModel.Role != null)
            {
                try
                {
                    this.RoleDetailsViewModel.Role.AccessRights = this.RoleDetailsViewModel.CurrentAccessRights
                        .Select(x => x.AccessRight).ToList();

                    var updateResponse = await this.roleService.UpdateRole(this.RoleDetailsViewModel.Role);

                    if (updateResponse.IsRequestSuccessful)
                    {
                        this.RoleDetailsViewModel.Role = updateResponse.Entity;
                        this.initialRoleName = this.RoleDetailsViewModel.Role.RoleName;
                    }
                    else
                    {
                        this.ErrorMessageViewModel.Errors.Clear();
                        this.ErrorMessageViewModel.Errors.AddRange(updateResponse.Errors);
                    }

                    this.ModificationEnabled = !updateResponse.IsRequestSuccessful;
                }
                catch (Exception exception)
                {
                    this.ErrorMessageViewModel.Errors.Clear();
                    this.ErrorMessageViewModel.Errors.Add(exception.Message);
                }
            }
        }

        /// <summary>
        ///     Reset all initial data for the current <see cref="IRoleDetailsViewModel.Role" />
        /// </summary>
        private void ResetRole()
        {
            if (this.RoleDetailsViewModel.Role != null)
            {
                this.RoleDetailsViewModel.Role.RoleName = this.initialRoleName;
                this.RoleDetailsViewModel.UpdateProperties();
            }
        }
    }
}
