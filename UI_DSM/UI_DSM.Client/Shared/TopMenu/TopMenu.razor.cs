// --------------------------------------------------------------------------------------------------------
// <copyright file="TopMenu.razor.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Shared.TopMenu
{
    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.ViewModels.Shared.TopMenu;

    /// <summary>
    ///     The TopMenu component
    /// </summary>
    public partial class TopMenu: IDisposable
    {
        /// <summary>
        /// A collection of <see cref="IDisposable"/>
        /// </summary>
        private List<IDisposable> disposables = new();

        /// <summary>
        ///     The <see cref="ITopMenuViewModel" />
        /// </summary>
        [Inject]
        public ITopMenuViewModel ViewModel { get; set; }

        /// <summary>
        ///     Bool to keep track of the open state of the user dropdown menu
        /// </summary>
        private bool IsUserMenuOpen { get; set; } = false;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.disposables.ForEach(x => x.Dispose());
        }

        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// Override this method if you will perform an asynchronous operation and
        /// want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
        protected override Task OnInitializedAsync()
        {
            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.UserName)
                .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));

            return this.ViewModel.InitializesViewModel();
        }
    }
}
