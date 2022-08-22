// --------------------------------------------------------------------------------------------------------
// <copyright file="IRoleDetailsViewModel.cs" company="RHEA System S.A.">
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
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Wrappers;

    /// <summary>
    ///     Interface definition for <see cref="RoleDetailsViewModel" />
    /// </summary>
    public interface IRoleDetailsViewModel: IDisposable
    {
        /// <summary>
        ///     The <see cref="Role" /> to inspect
        /// </summary>
        Role Role { get; set; }

        /// <summary>
        ///     A collection of <see cref="AccessRightWrapper" /> that the role has
        /// </summary>
        IEnumerable<AccessRightWrapper> CurrentAccessRights { get; set; }

        /// <summary>
        ///     Update this view model properties
        /// </summary>
        void UpdateProperties();
    }
}
