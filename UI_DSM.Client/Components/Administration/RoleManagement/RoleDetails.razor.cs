// --------------------------------------------------------------------------------------------------------
// <copyright file="RoleDetails.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.Administration.RoleManagement
{
    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.ViewModels.Components.Administration.RoleManagement;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Component to provide details for a <see cref="Role" />
    /// </summary>
    public partial class RoleDetails : IDisposable
    {
        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     The <see cref="IRoleDetailsViewModel" /> for this component
        /// </summary>
        [Parameter]
        public IRoleDetailsViewModel ViewModel { get; set; }

        /// <summary>
        ///     Value indicating if data component allow modification
        /// </summary>
        [Parameter]
        public bool ModificationEnabled { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.disposables.ForEach(x => x.Dispose());
            this.disposables.Clear();
        }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        /// </summary>
        protected override void OnInitialized()
        {
            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.Role)
                .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));

            this.disposables.Add(this.WhenAnyValue(x => x.ModificationEnabled)
                .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));

            this.disposables.Add(this.ViewModel);
        }
    }
}
