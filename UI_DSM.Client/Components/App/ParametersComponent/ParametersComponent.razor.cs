// --------------------------------------------------------------------------------------------------------
// <copyright file="ParametersComponent.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.ParametersComponent
{
    using CDP4Common.EngineeringModelData;

    using Microsoft.AspNetCore.Components;

    /// <summary>
    ///     Component that display all <see cref="ParameterOrOverrideBase" /> contained into an <see cref="ElementBase" />
    /// </summary>
    public partial class ParametersComponent
    {
        /// <summary>
        ///     The collection of <see cref="ParameterOrOverrideBase" />
        /// </summary>
        [Parameter]
        public List<ParameterOrOverrideBase> Parameters { get; set; }

        /// <summary>
        ///     The current <see cref="Option" />
        /// </summary>
        [Parameter]
        public Option CurrentOption { get; set; }
    }
}
