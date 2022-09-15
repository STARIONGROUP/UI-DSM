// --------------------------------------------------------------------------------------------------------
// <copyright file="IReplyService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.ReplyService
{
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     Interface definition for <see cref="ReplyService" />
    /// </summary>
    public interface IReplyService
    {
        /// <summary>
        ///     Gets all <see cref="Reply" />s contained inside a <see cref="Comment" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /> of the <see cref="Comment" /></param>
        /// <param name="annotationId">The <see cref="Entity.Id" /> of the <see cref="Comment" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="Reply" /></returns>
        Task<List<Reply>> GetRepliesOfComment(Guid projectId, Guid annotationId, int deepLevel = 0);

        /// <summary>
        ///     Gets a <see cref="Reply" /> contained inside a <see cref="Comment" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="annotationId">The <see cref="Guid" /> of the <see cref="Comment" /></param>
        /// <param name="replyId">The <see cref="Guid" /> of the <see cref="Reply" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="Reply" /></returns>
        Task<Reply> GetReplyOfComment(Guid projectId, Guid annotationId, Guid replyId, int deepLevel = 0);

        /// <summary>
        ///     Creates a new <see cref="Reply" /> inside a <see cref="Comment" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="annotationId">The <see cref="Guid" /> of the <see cref="Comment" /></param>
        /// <param name="reply">The <see cref="Reply" />to create</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{Reply}" /></returns>
        Task<EntityRequestResponse<Reply>> CreateReply(Guid projectId, Guid annotationId, Reply reply);

        /// <summary>
        ///     Updates a <see cref="Reply" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reply">The <see cref="Reply" />to update</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{Reply}" /></returns>
        Task<EntityRequestResponse<Reply>> UpdateReply(Guid projectId, Reply reply);

        /// <summary>
        ///     Deletes a <see cref="Reply" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reply">The <see cref="Reply" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /></returns>
        Task<RequestResponseDto> DeleteReply(Guid projectId, Reply reply);
    }
}
