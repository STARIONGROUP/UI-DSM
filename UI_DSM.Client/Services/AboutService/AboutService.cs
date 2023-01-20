// --------------------------------------------------------------------------------------------------------
// <copyright file="AboutService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.AboutService
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    ///     This service gets system and uptime information
    /// </summary>
    [Route("About")]
    public class AboutService : ServiceBase, IAboutService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AboutService" /> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="ServiceBase.HttpClient" /></param>
        /// <param name="jsonService">The <see cref="IJsonService" /></param>
        public AboutService(HttpClient httpClient, IJsonService jsonService) : base(httpClient, jsonService)
        {
        }

        /// <summary>
        ///     Gets information from the server
        /// </summary>
        /// <returns>A <see cref="Task" /> with the <see cref="SystemInformationDto" /> response</returns>
        public async Task<SystemInformationDto> GetSystemInformation()
        {
            try
            {
                var response = await this.HttpClient.GetAsync(this.MainRoute);

                return !response.IsSuccessStatusCode
                    ? new SystemInformationDto()
                    : this.jsonService.Deserialize<SystemInformationDto>(await response.Content.ReadAsStreamAsync());
            }
            catch
            {
                return new SystemInformationDto();
            }
        }
    }
}
