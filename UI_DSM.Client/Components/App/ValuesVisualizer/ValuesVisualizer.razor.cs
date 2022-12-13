// --------------------------------------------------------------------------------------------------------
// <copyright file="ValuesVisualizer.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.ValuesVisualizer
{
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using Microsoft.AspNetCore.Components;

    /// <summary>
    ///     Base component for <see cref="ParameterType" /> values visualizer
    /// </summary>
    public abstract partial class ValuesVisualizer
    {
        /// <summary>
        ///     The <see cref="ParameterOrOverrideBase" />
        /// </summary>
        [Parameter]
        public ParameterOrOverrideBase Parameter { get; set; }

        /// <summary>
        ///     The <see cref="Option" />
        /// </summary>
        [Parameter]
        public Option Option { get; set; }

        /// <summary>
        ///     The <see cref="ActualFiniteState" />
        /// </summary>
        [Parameter]
        public ActualFiniteState State { get; set; }
    }
}
