// --------------------------------------------------------------------------------------------------------
// <copyright file="Tooltip.cs" company="RHEA System S.A.">
//  Copyright (c) 2023 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Components.App.Tooltip
{
    using Microsoft.AspNetCore.Components;

    /// <summary>
    ///     Component used to display a tooltip
    /// </summary>
    public partial class Tooltip
    {
        /// <summary>
        ///     The <see cref="RenderFragment" /> child
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        ///     The tooltip text
        /// </summary>
        [Parameter]
        public string Text { get; set; }
    }
}
