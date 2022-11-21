// --------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionVisibilitySelector.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.ConnectionVisibilitySelector
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.ViewModels.App.ConnectionVisibilitySelector;

    /// <summary>
    ///     Component that provide capability to select <see cref="ConnectionToVisibilityState" />
    /// </summary>
    public partial class ConnectionVisibilitySelector
    {
        /// <summary>
        ///     The text to display for the state <see cref="ConnectionToVisibilityState.Connected" />
        /// </summary>
        [Parameter]
        public string ConnectedStateText { get; set; } = "Show with trace";

        /// <summary>
        ///     The text to display for the state <see cref="ConnectionToVisibilityState.NotConnected" />
        /// </summary>
        [Parameter]
        public string NotConnectedStateText { get; set; } = "Show without trace";

        /// <summary>
        ///     The <see cref="IConnectionVisibilitySelectorViewModel" />
        /// </summary>
        [Inject]
        public IConnectionVisibilitySelectorViewModel ViewModel { get; set; }
    }
}
