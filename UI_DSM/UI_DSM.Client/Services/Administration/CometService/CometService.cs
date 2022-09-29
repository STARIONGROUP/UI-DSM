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
    using System.Text;
    using System.Text.Json;

    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;

    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Services.JsonDeserializerProvider;
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
        /// <param name="authenticationData">The <see cref="CometAuthenticationData" /></param>
        /// <returns>A <see cref="Task" /> with the result of the connection</returns>
        public async Task<CometAuthenticationResponse> Login(CometAuthenticationData authenticationData)
        {
            var content = JsonSerializer.Serialize(authenticationData);
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
            var modelUpload = new ModelUploadData()
            {
                IterationId = iterationId,
                ModelId = modelId
            };

            var content = JsonSerializer.Serialize(modelUpload);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await this.HttpClient.PostAsync($"{this.MainRoute}/{sessionId}/Models/Upload", bodyContent);
            return this.jsonService.Deserialize<ModelUploadResponse>(await response.Content.ReadAsStreamAsync());
        }
    }
}
