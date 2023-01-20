// --------------------------------------------------------------------------------------------------------
// <copyright file="RoleCreation.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.Administration.RoleManagement
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.Components.Administration.RoleManagement;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This component is used to create a new <see cref="Role" />
    /// </summary>
    public partial class RoleCreation
    {
        /// <summary>
        ///     The <see cref="IRoleCreationViewModel" /> for the component
        /// </summary>
        [Parameter]
        public IRoleCreationViewModel ViewModel { get; set; }
    }
}
