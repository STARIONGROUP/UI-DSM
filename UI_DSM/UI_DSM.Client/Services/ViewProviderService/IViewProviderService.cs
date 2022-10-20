// --------------------------------------------------------------------------------------------------------
// <copyright file="IViewProviderService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.ViewProviderService
{
    using UI_DSM.Shared.Enumerator;

    /// <summary>
    ///     Interface definition for <see cref="ViewProviderService" />
    /// </summary>
    public interface IViewProviderService
    {
        /// <summary>
        ///     Tries to get the correct <see cref="Type" /> for that correspond for the provided <see cref="View" />
        /// </summary>
        /// <param name="view">The <see cref="View" /></param>
        /// <returns>The retrieves <see cref="Type" /></returns>
        Type GetViewType(View view);
    }
}
