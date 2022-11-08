// --------------------------------------------------------------------------------------------------------
// <copyright file="TraceabilityVisibilitySelector.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.TraceabilityVisibilitySelector
{
    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;

    /// <summary>
    ///     Component that provide capability to select <see cref="TraceabilityToVisibilityState" />
    /// </summary>
    public partial class TraceabilityVisibilitySelector
    {
        /// <summary>
        ///     The associated <see cref="ITraceabilityTableViewModel" />
        /// </summary>
        public ITraceabilityTableViewModel ViewModel { get; private set; }

        /// <summary>
        ///     Updates the <see cref="ITraceabilityTableViewModel" /> viewmodel
        /// </summary>
        /// <param name="viewModel">The <see cref="ITraceabilityTableViewModel" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task UpdateViewModel(ITraceabilityTableViewModel viewModel)
        {
            this.ViewModel = viewModel;
            await this.InvokeAsync(this.StateHasChanged);
        }
    }
}
