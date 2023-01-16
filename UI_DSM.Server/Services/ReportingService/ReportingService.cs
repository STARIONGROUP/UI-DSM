// --------------------------------------------------------------------------------------------------------
// <copyright file="ReportingService.cs" company="RHEA System S.A.">
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
    ///     This service provides information related to Reports
    /// </summary>
    public class ReportingService : IReportingService
    {
        /// <summary>
        /// Holds a reference to the INJECTED <see cref="CometService.ICometService"/>
        /// </summary>
        private readonly CometService.ICometService cometService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReportingService" /> class.
        /// </summary>
        public ReportingService(CometService.ICometService cometService)
        {
            this.cometService = cometService;
        }

        /// <summary>
        ///     Retrieves a list of available Reports
        /// </summary>
        /// <param name="projectId">The project id</param>
        /// <returns>A <see cref="List{T}"/> of type <see cref="string"/> that contains available report locations</returns>
        public List<string> GetAvailableReports(Guid projectId)
        {
            var result = new List<string>();

            foreach (var file in Directory.GetFiles(Path.Combine("Reports", "Generic")))
            {
                result.Add(Path.Combine("Generic", Path.GetFileNameWithoutExtension(file)));
            }

            var directoryPath = Path.Combine("Reports", projectId.ToString());

            if (Directory.Exists(directoryPath))
            {
                foreach (var file in Directory.GetFiles(directoryPath))
                {
                    result.Add(Path.Combine(projectId.ToString(),  Path.GetFileNameWithoutExtension(file)));
                }
            }

            return result;
        }

        /// <summary>
        ///     Retrieves the report information
        /// </summary>
        /// <param name="reportLocation"></param>
        /// <param name="annexC3FileName">The name of the Annex C3 file</param>
        /// <param name="iterationId">The <see cref="Iteration"/>'s Id</param>
        /// <returns>The <see cref="XtraReport" /></returns>
        public async Task<XtraReport> GetReportDto(string reportLocation, string annexC3FileName, Guid iterationId)
        {
            var reportResolver = new ReportResolver();
            var session = await this.cometService.GetSession(annexC3FileName, iterationId);
            var iteration = await this.cometService.GetIteration(annexC3FileName, iterationId);

            var fileName = reportLocation;

            if (Path.GetExtension(fileName) != "rep4")
            {
                fileName += ".rep4";
            }

            var result = reportResolver.Resolve(fileName, iteration, session);

            return result;
        }
    }
}
