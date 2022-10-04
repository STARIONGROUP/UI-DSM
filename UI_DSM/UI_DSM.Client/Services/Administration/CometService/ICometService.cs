// --------------------------------------------------------------------------------------------------------
// <copyright file="ICometService.cs" company="RHEA System S.A.">
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
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;

    using Microsoft.AspNetCore.Components.Forms;

    using UI_DSM.Shared.DTO.CometData;

    /// <summary>
    ///     Interface definition for <see cref="CometService" />
    /// </summary>
    public interface ICometService
    {
        /// <summary>
        ///     Tries to login to a Comet <see cref="ISession" />
        /// </summary>
        /// <param name="uploadData">The <see cref="CometAuthenticationData" /></param>
        /// <returns>A <see cref="Task" /> with the result of the connection</returns>
        Task<CometAuthenticationResponse> Login(CometAuthenticationData uploadData);

        /// <summary>
        ///     Closes the current <see cref="ISession" />
        /// </summary>
        /// <param name="sessionId">The <see cref="Guid" /> of the session</param>
        /// <returns>A <see cref="Task" /></returns>
        Task<bool> Logout(Guid sessionId);

        /// <summary>
        ///     Gets a collection of available <see cref="EngineeringModelSetup" />
        /// </summary>
        /// <param name="sessionId">The <see cref="Guid" /> of the session</param>
        /// <returns>A <see cref="Task" /> with a <see cref="ModelsDataResponse" /></returns>
        Task<ModelsDataResponse> GetAvailableEngineeringModels(Guid sessionId);

        /// <summary>
        ///     Upload an <see cref="Iteration" /> to the server
        /// </summary>
        /// <param name="sessionId">The <see cref="Guid" /> of the session</param>
        /// <param name="modelId">The <see cref="Guid" /> of the <see cref="EngineeringModel" /></param>
        /// <param name="iterationId">The <see cref="Guid" /> of the <see cref="Iteration" /> to upload</param>
        /// <returns>A <see cref="Task" /> with the <see cref="ModelUploadResponse" /></returns>
        Task<ModelUploadResponse> UploadIteration(Guid sessionId, Guid modelId, Guid iterationId);

        /// <summary>
        ///     Uploads an Annex C3 file and tries to opens it
        /// </summary>
        /// <param name="uploadData">The <see cref="CometAuthenticationData" /></param>
        /// <param name="browserFile">The <see cref="IBrowserFile" /></param>
        /// <returns>A <see cref="Task" /> with the result of the upload</returns>
        Task<CometAuthenticationResponse> UploadAnnexC3File(CometAuthenticationData uploadData, IBrowserFile browserFile);
    }
}
