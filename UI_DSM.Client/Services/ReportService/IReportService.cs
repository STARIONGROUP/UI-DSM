// --------------------------------------------------------------------------------------------------------
// <copyright file="IReportService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.ReportService
{
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ReportService" />
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        ///     Gets a collection of report names that will be needed for the current <see cref="View" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <returns>A collection of report names</returns>
        Task<IEnumerable<string>> GetAvailableReports(Guid projectId);
    }
}
