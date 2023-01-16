// --------------------------------------------------------------------------------------------------------
// <copyright file="IReportingService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Services.ReportingService
{
    using CDP4Common.EngineeringModelData;

    using DevExpress.XtraReports.UI;

    /// <summary>
    ///     Interface definition for <see cref="ReportingService" />
    /// </summary>
    public interface IReportingService
    {
        /// <summary>
        ///     Retrieves a list of available Reports
        /// </summary>
        /// <param name="modelId">The engineering model's id</param>
        /// <returns>A <see cref="List{T}"/> of type <see cref="string"/> that contains available report locations</returns>
        List<string> GetAvailableReports(Guid modelId);

        /// <summary>
        ///     Retrieves the report information
        /// </summary>
        /// <param name="reportLocation"></param>
        /// <param name="annexC3FileName">The name of the Annex C3 file</param>
        /// <param name="iterationId">The <see cref="Iteration"/>'s Id</param>
        /// <returns>The <see cref="XtraReport" /></returns>
        Task<XtraReport> GetReportDto(string reportLocation, string annexC3FileName, Guid iterationId);
    }
}
