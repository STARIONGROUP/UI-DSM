// --------------------------------------------------------------------------------------------------------
// <copyright file="ConfirmCancelPopup.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components
{
    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.ViewModels.Components;

    /// <summary>
    ///     Popup that ask the user to confirm his last choice
    /// </summary>
    public partial class ConfirmCancelPopup : IDisposable
    {
        /// <summary>
        ///     Value that indicates if buttons are enabled or not
        /// </summary>
        private bool buttonsEnabled = true;

        /// <summary>
        ///     The <see cref="IDisposable" />
        /// </summary>
        private IDisposable disposable;

        /// <summary>
        ///     The <see cref="ConfirmCancelPopupViewModel" />
        /// </summary>
        [Parameter]
        public IConfirmCancelPopupViewModel ViewModel { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.disposable.Dispose();
        }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        protected override Task OnInitializedAsync()
        {
            this.disposable = this.WhenAnyValue(x => x.ViewModel.IsVisible).Subscribe(_ => this.InvokeAsync(this.StateHasChanged));
            return base.OnInitializedAsync();
        }

        /// <summary>
        ///     Performs the OnConfirm <see cref="EventHandler" /> when the Confirm button has been pressed
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private Task OnConfirmClicked()
        {
            this.buttonsEnabled = false;
            return this.ViewModel.OnConfirm.InvokeAsync();
        }
    }
}
