// --------------------------------------------------------------------------------------------------------
// <copyright file="ReportService.cs" company="RHEA System S.A.">
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
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This <see cref="ReportService" /> provides capabilities to query report names for a specific Project
    /// </summary>
    [Route("Reporting/{0}")]
    public class ReportService : ServiceBase, IReportService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReportService" /> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="ServiceBase.HttpClient" /></param>
        /// <param name="jsonService">The <see crf="IJsonService" /></param>
        public ReportService(HttpClient httpClient, IJsonService jsonService) : base(httpClient, jsonService)
        {
        }

        /// <summary>
        ///     Gets a collection of report names that will be needed for the current <see cref="View" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <returns>A collection of report names</returns>
        public async Task<IEnumerable<string>> GetAvailableReports(Guid projectId)
        {
            this.ComputeMainRoute(projectId);
            var uri = this.MainRoute;

            var response = await this.HttpClient.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
            {
                return Enumerable.Empty<string>();
            }

            return this.jsonService.Deserialize<IEnumerable<string>>(await response.Content.ReadAsStreamAsync());
        }
    }
}
