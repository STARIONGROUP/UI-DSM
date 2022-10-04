// --------------------------------------------------------------------------------------------------------
// <copyright file="ArtifactService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.ArtifactService
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     The <see cref="ArtifactService" /> provide capability to manage <see cref="Artifact" />s inside a
    ///     <see cref="Project" />
    /// </summary>
    [Route("Project/{0}/Artifact")]
    public class ArtifactService : EntityServiceBase<Artifact, ArtifactDto>, IArtifactService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityServiceBase{TEntity, TEntityDto}" /> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="ServiceBase.HttpClient" /></param>
        /// <param name="jsonService">The <see cref="IJsonService" /></param>
        public ArtifactService(HttpClient httpClient, IJsonService jsonService) : base(httpClient, jsonService)
        {
        }

        /// <summary>
        ///     Tries to upload a <see cref="Model" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /> where the <see cref="Model" /> will belongs</param>
        /// <param name="temporaryFileName">The file path</param>
        /// <param name="modelName">The name of the model</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{Model}" /></returns>
        public Task<EntityRequestResponse<Model>> UploadModel(Guid projectId, string temporaryFileName, string modelName)
        {
            var model = new Model
            {
                FileName = temporaryFileName,
                ModelName = modelName
            };

            return this.CreateArtifact(projectId, model);
        }

        /// <summary>
        ///     Uploads a File into the server and creates the corresponding <see cref="Artifact" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /> where the <see cref="Artifact" /> will belongs to</param>
        /// <param name="artifact">The <see cref="TArtifact" /> to create</param>
        /// <returns>A <see cref="Task" /> with the created <see cref="EntityRequestResponse{TArtifact}" /> result</returns>
        private async Task<EntityRequestResponse<TArtifact>> CreateArtifact<TArtifact>(Guid projectId, TArtifact artifact)
            where TArtifact : Artifact
        {
            this.ComputeMainRoute(projectId);
            var createdArtifact = await this.CreateEntity(artifact, 0);

            return new EntityRequestResponse<TArtifact>
            {
                IsRequestSuccessful = createdArtifact.IsRequestSuccessful,
                Errors = createdArtifact.Errors,
                Entity = (TArtifact)createdArtifact.Entity
            };
        }
    }
}
