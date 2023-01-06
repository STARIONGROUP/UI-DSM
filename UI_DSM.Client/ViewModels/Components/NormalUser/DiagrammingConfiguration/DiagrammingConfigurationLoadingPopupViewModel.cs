// --------------------------------------------------------------------------------------------------------
// <copyright file="DiagrammingConfigurationLoadingPopupViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Components.NormalUser.ProjectReview;
    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.Services.Administration.ProjectService;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="DiagrammingConfigurationLoadingPopup" /> component
    /// </summary>
    public class DiagrammingConfigurationLoadingPopupViewModel : IDiagrammingConfigurationLoadingPopupViewModel
    {
        /// <summary>
        ///     The list of available configurations
        /// </summary>
        public IEnumerable<string> ConfigurationsName { get; set; }

        /// <summary>
        ///     The name of the selected configuration
        /// </summary>
        public string SelectedConfiguration { get; set; } 

        /// <summary>
        ///     The <see cref="EventCallback" /> to call for data submit
        /// </summary>
        public EventCallback OnValidSubmit { get; set; }

    }
}
