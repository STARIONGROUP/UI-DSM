// --------------------------------------------------------------------------------------------------------
// <copyright file="DiagrammingConfigurationPopup.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.NormalUser.DiagrammingConfiguration
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview;

    /// <summary>
    ///     This component is used to save a diagramming configuration
    /// </summary>
    public partial class DiagrammingConfigurationPopup
    {
        /// <summary>
        ///     The <see cref="IDiagrammingConfigurationPopupViewModel" /> for the component
        /// </summary>
        [Parameter]
        public IDiagrammingConfigurationPopupViewModel ViewModel { get; set; }
    }
}
