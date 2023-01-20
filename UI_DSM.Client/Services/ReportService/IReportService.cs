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
    using Microsoft.AspNetCore.Components.Forms;

    using UI_DSM.Shared.DTO.CometData;

    /// <summary>
    ///     Interface definition for <see cref="ReportService" />
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        ///     Uploads a report
        /// </summary>
        /// <param name="budgetName">The name of the report</param>
        /// <param name="browserFile">The <see cref="IBrowserFile" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="CometAuthenticationResponse" /></returns>
        Task<CometAuthenticationResponse> UploadReport(string budgetName, IBrowserFile browserFile);
    }
}
