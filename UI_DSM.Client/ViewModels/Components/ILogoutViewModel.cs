// --------------------------------------------------------------------------------------------------------
// <copyright file="ILogoutViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components
{
    using Microsoft.AspNetCore.Components;

    /// <summary>
    ///     Interface definition for <see cref="LogoutViewModel" />
    /// </summary>
    public interface ILogoutViewModel
    {
        /// <summary>
        ///     Logout from the data source
        /// </summary>
        /// <returns>A <see cref="Task"/></returns>
        Task ExecuteLogout();

        /// <summary>
        ///     The <see cref="Microsoft.AspNetCore.Components.NavigationManager" />
        /// </summary>
        NavigationManager NavigationManager { get; set; }
    }
}
