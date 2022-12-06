// --------------------------------------------------------------------------------------------------------
// <copyright file="IArtifactService.cs" company="RHEA System S.A.">
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
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     Interface definition for <see cref="ArtifactService" />
    /// </summary>
    public interface IArtifactService
    {
        /// <summary>
        ///     Tries to upload a <see cref="Model" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /> where the <see cref="Model" /> will belongs</param>
        /// <param name="temporaryFileName">The file name</param>
        /// <param name="modelName">The name of the model</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{Model}" /> result</returns>
        Task<EntityRequestResponse<Model>> UploadModel(Guid projectId, string temporaryFileName, string modelName);

        /// <summary>
        ///     Gets all <see cref="Artifact" />s contained inside a <see cref="Project" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="Artifact" /></returns>
        Task<List<Artifact>> GetArtifactsOfProject(Guid projectId, int deepLevel = 0);
    }
}
