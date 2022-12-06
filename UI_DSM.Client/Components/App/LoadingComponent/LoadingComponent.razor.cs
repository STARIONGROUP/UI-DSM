// --------------------------------------------------------------------------------------------------------
// <copyright file="LoadingComponent.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.LoadingComponent
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This component is used to show that something is loading or processing
    /// </summary>
    public partial class LoadingComponent
    {
        /// <summary>
        ///     The indicator that shows that something is loading in the component
        /// </summary>
        [Parameter]
        public bool IsLoading { get; set; }

        /// <summary>
        ///     The loading text to display
        /// </summary>
        [Parameter]
        public string LoadingText { get; set; } = "Loading...";

        /// <summary>
        ///     The loading template if needed
        /// </summary>
        [Parameter]
        public RenderFragment LoadingTemplate { get; set; }

        /// <summary>
        ///     The child content of the component
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
