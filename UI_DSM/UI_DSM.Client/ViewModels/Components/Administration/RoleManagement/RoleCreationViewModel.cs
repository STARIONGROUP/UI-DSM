// --------------------------------------------------------------------------------------------------------
// <copyright file="RoleCreationViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.Administration.RoleManagement
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Components.Administration.RoleManagement;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Wrappers;

    /// <summary>
    ///     View model for the <see cref="RoleCreation" /> Component
    /// </summary>
    public class RoleCreationViewModel : IRoleCreationViewModel
    {
        /// <summary>
        ///     The <see cref="Role" /> to create
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback" /> to call for data submit
        /// </summary>
        public EventCallback OnValidSubmit { get; set; }

        /// <summary>
        ///     A collection of <see cref="AccessRightWrapper" /> that has been selected
        /// </summary>
        public IEnumerable<AccessRightWrapper> SelectedAccessRights { get; set; }
    }
}
