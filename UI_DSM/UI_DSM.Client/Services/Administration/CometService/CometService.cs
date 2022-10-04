// --------------------------------------------------------------------------------------------------------
// <copyright file="CometService.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Services.Administration.CometService
{
    using System.Net.Http.Headers;
    using System.Text;

    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Shared.DTO.CometData;

    /// <summary>
    ///     The <see cref="CometService" /> provide capabilities to interact with a Comet instance
    /// </summary>
    [Route("Comet")]
    public class CometService : ServiceBase, ICometService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CometService" /> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="ServiceBase.HttpClient" /></param>
        /// <param name="jsonService">The <see cref="IJsonService" /></param>
        public CometService(HttpClient httpClient, IJsonService jsonService) : base(httpClient, jsonService)
        {
        }

        /// <summary>
        ///     Tries to login to a Comet <see cref="ISession" />
        /// </summary>
        /// <param name="uploadData">The <see cref="CometAuthenticationData" /></param>
        /// <returns>A <see cref="Task" /> with the result of the connection</returns>
        public async Task<CometAuthenticationResponse> Login(CometAuthenticationData uploadData)
        {
            var content = this.jsonService.Serialize(uploadData);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await this.HttpClient.PostAsync(Path.Combine(this.MainRoute, "Login"), bodyContent);
            return this.jsonService.Deserialize<CometAuthenticationResponse>(await response.Content.ReadAsStreamAsync());
        }

        /// <summary>
        ///     Closes the current <see cref="ISession" />
        /// </summary>
        /// <param name="sessionId">The <see cref="Guid" /> of the session</param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task<bool> Logout(Guid sessionId)
        {
            var response = await this.HttpClient.DeleteAsync($"{this.MainRoute}/{sessionId}");
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        ///     Gets a collection of available <see cref="EngineeringModelSetup" />
        /// </summary>
        /// <param name="sessionId">The <see cref="Guid" /> of the session</param>
        /// <returns>A <see cref="Task" /> with a <see cref="ModelsDataResponse" /></returns>
        public async Task<ModelsDataResponse> GetAvailableEngineeringModels(Guid sessionId)
        {
            var response = await this.HttpClient.GetAsync($"{this.MainRoute}/{sessionId}/Models");

            if (!response.IsSuccessStatusCode)
            {
                throw new ArgumentException("Invalid session id");
            }

            return this.jsonService.Deserialize<ModelsDataResponse>(await response.Content.ReadAsStreamAsync());
        }

        /// <summary>
        ///     Upload an <see cref="Iteration" /> to the server
        /// </summary>
        /// <param name="sessionId">The <see cref="Guid" /> of the session</param>
        /// <param name="modelId">The <see cref="Guid" /> of the <see cref="EngineeringModel" /></param>
        /// <param name="iterationId">The <see cref="Guid" /> of the <see cref="Iteration" /> to upload</param>
        /// <returns>A <see cref="Task" /> with the <see cref="ModelUploadResponse" /></returns>
        public async Task<ModelUploadResponse> UploadIteration(Guid sessionId, Guid modelId, Guid iterationId)
        {
            var modelUpload = new ModelUploadData
            {
                IterationId = iterationId,
                ModelId = modelId
            };

            var content = this.jsonService.Serialize(modelUpload);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await this.HttpClient.PostAsync($"{this.MainRoute}/{sessionId}/Models/Upload", bodyContent);
            return this.jsonService.Deserialize<ModelUploadResponse>(await response.Content.ReadAsStreamAsync());
        }

        /// <summary>
        ///     Uploads an Annex C3 file and tries to opens it
        /// </summary>
        /// <param name="uploadData">The <see cref="CometAuthenticationData" /></param>
        /// <param name="browserFile">The <see cref="IBrowserFile" /></param>
        /// <returns>A <see cref="Task" /> with the result of the upload</returns>
        public async Task<CometAuthenticationResponse> UploadAnnexC3File(CometAuthenticationData uploadData, IBrowserFile browserFile)
        {
            const int limitSize = 512000 * 2;

            if (browserFile.Size > limitSize)
            {
                return new CometAuthenticationResponse()
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
            content.Add(new StringContent(this.jsonService.Serialize(uploadData)), "authenticationData");
            var response = await this.HttpClient.PostAsync(Path.Combine(this.MainRoute, "Upload"), content);
            return this.jsonService.Deserialize<CometAuthenticationResponse>(await response.Content.ReadAsStreamAsync());
        }
    }
}
