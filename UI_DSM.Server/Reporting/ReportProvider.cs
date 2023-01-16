// --------------------------------------------------------------------------------------------------------
// <copyright file="ReportProvider.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Reporting 
{
    using System.Text;

    using DevExpress.XtraReports.Services;
    using DevExpress.XtraReports.UI;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Server.Services.ReportingService;
    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    /// The implementation of <see cref="IReportProvider"/>
    /// </summary>
    public class ReportProvider : IReportProvider 
    {
        /// <summary>
        /// Gets the INJECTED <see cref="IReportingService"/>
        /// </summary>
        public IReportingService ReportingService { get; }

        /// <summary>
        /// GETS the INJECTED <see cref="IJsonService" />
        /// </summary>
        private IJsonService jsonService { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="ReportProvider"/> class
        /// </summary>
        /// <param name="reportingService">The <see cref="IReportingService"/></param>
        /// <param name="jsonService">The <see cref="IJsonService"/></param>
        public ReportProvider(IReportingService reportingService, IJsonService jsonService)
        {
            this.ReportingService = reportingService;
            this.jsonService = jsonService;
        }

        /// <summary>
        /// Retrieves an <see cref="XtraReport"/> based on the serialized <see cref="ReportDto"/> in the reportDtoAsString parameter
        /// </summary>
        /// <param name="reportDtoAsString">The serialized <see cref="ReportDto"/> </param>
        /// <param name="context">The <see cref="ReportProviderContext"/></param>
        /// <returns></returns>
        XtraReport IReportProvider.GetReport(string reportDtoAsString, ReportProviderContext context)
        {
            if (!string.IsNullOrWhiteSpace(reportDtoAsString))
            {
                var byteArray = Encoding.UTF8.GetBytes(reportDtoAsString);

                using (var stream = new MemoryStream(byteArray))
                {
                    var reportDto = this.jsonService.Deserialize<ReportDto>(stream);

                    if (string.IsNullOrWhiteSpace(reportDto?.Name))
                    {
                        return null;
                    }

                    var result = this.ReportingService.GetReportDto($"Reports\\{reportDto.Name}", $"{reportDto.ProjectId}\\{reportDto.IterationId}.zip", reportDto.IterationId).Result;
                    return result;
                }
            }

            return new XtraReport();
        }
    }
}
