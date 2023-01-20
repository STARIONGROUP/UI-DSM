// --------------------------------------------------------------------------------------------------------
// <copyright file="RoleDetailsViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.Administration.RoleManagement
{
    using ReactiveUI;

    using UI_DSM.Client.Components.Administration.RoleManagement;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Wrappers;

    /// <summary>
    ///     View model for the <see cref="RoleDetails" /> component
    /// </summary>
    public class RoleDetailsViewModel : ReactiveObject, IRoleDetailsViewModel
    {
        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     Backing field for <see cref="Role" />
        /// </summary>
        private Role role;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RoleDetailsViewModel" /> class.
        /// </summary>
        public RoleDetailsViewModel()
        {
            this.disposables.Add(this.WhenAnyValue(x => x.Role)
                .WhereNotNull()
                .Subscribe(_ => this.UpdateProperties()));
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
        ///     The <see cref="Role" /> to inspect
        /// </summary>
        public Role Role
        {
            get => this.role;
            set => this.RaiseAndSetIfChanged(ref this.role, value);
        }

        /// <summary>
        ///     A collection of <see cref="AccessRightWrapper" /> that the role has
        /// </summary>
        public IEnumerable<AccessRightWrapper> CurrentAccessRights { get; set; }

        /// <summary>
        ///     Update this view model properties
        /// </summary>
        public void UpdateProperties()
        {
            this.CurrentAccessRights = this.Role.AccessRights.Select(x => new AccessRightWrapper(x));
        }
    }
}
