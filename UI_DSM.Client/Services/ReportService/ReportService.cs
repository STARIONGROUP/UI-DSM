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
    using System.Net.Http.Headers;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Shared.DTO.CometData;

    /// <summary>
    ///     This <see cref="ReportService" /> provides capabilities to query report names for a specific Project
    /// </summary>
    [Route("Reporting")]
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
        ///     Uploads a report
        /// </summary>
        /// <param name="budgetName">The name of the report</param>
        /// <param name="browserFile">The <see cref="IBrowserFile" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="CometAuthenticationResponse" /></returns>
        public async Task<CometAuthenticationResponse> UploadReport(string budgetName, IBrowserFile browserFile)
        {
            const int limitSize = 512000 * 2;

            if (browserFile.Size > limitSize)
            {
                return new CometAuthenticationResponse
                {
                    Errors = new List<string>
                    {
                        $"The file size must not exceed {limitSize / 100}Kb"
                    }
                };
            }

            var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(browserFile.OpenReadStream(limitSize));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-zip-compressed");
            content.Add(fileContent, "file", browserFile.Name);
            var uri = $"{this.MainRoute}/Upload";
            var response = await this.HttpClient.PostAsync(uri, content);
            return this.jsonService.Deserialize<CometAuthenticationResponse>(await response.Content.ReadAsStreamAsync());
        }
    }
}
