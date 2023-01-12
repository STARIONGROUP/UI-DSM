﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="IDiagrammingConfigurationDeletionPopupViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.NormalUser.DiagrammingConfiguration
{
    using Microsoft.AspNetCore.Components;

    /// <summary>
    ///     Interface definition for <see cref="DiagrammingConfigurationDeletionPopupViewModel" />
    /// </summary>
    public interface IDiagrammingConfigurationDeletionPopupViewModel
    {
        /// <summary>
        ///     The list of available configurations
        /// </summary>
        IEnumerable<string> ConfigurationsName { get; set; }

        /// <summary>
        ///     The name of the selected configuration
        /// </summary>
        string SelectedConfiguration { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback" /> to call for data submit
        /// </summary>
        EventCallback OnValidSubmit { get; set; }
    }
}