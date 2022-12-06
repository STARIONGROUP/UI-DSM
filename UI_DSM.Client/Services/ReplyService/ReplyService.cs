// --------------------------------------------------------------------------------------------------------
// <copyright file="ReplyService.cs" company="RHEA System S.A.">
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
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     The <see cref="ReplyService" /> provide capability to manage <see cref="Reply" />s related to a
    ///     <see cref="Comment" />
    /// </summary>
    [Route("Project/{0}/Annotation/{1}/Reply")]
    public class ReplyService : EntityServiceBase<Reply, ReplyDto>, IReplyService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReplyService" /> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="ServiceBase.HttpClient" /></param>
        /// <param name="jsonService">The <see cref="IJsonService" /></param>
        public ReplyService(HttpClient httpClient, IJsonService jsonService) : base(httpClient, jsonService)
        {
        }

        /// <summary>
        ///     Gets all <see cref="Reply" />s contained inside a <see cref="Comment" />
        /// </summary>
        /// <param name="projectId">
        ///     The <see cref="Entity.Id" /> of the <see cref="Project" /> of the <see cref="Comment" />
        /// </param>
        /// <param name="annotationId">The <see cref="Entity.Id" /> of the <see cref="Comment" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="Reply" /></returns>
        public async Task<List<Reply>> GetRepliesOfComment(Guid projectId, Guid annotationId, int deepLevel = 0)
        {
            try
            {
                this.ComputeMainRoute(projectId, annotationId);
                return await this.GetEntities(deepLevel);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Gets a <see cref="Reply" /> contained inside a <see cref="Comment" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="annotationId">The <see cref="Guid" /> of the <see cref="Comment" /></param>
        /// <param name="replyId">The <see cref="Guid" /> of the <see cref="Reply" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="Reply" /></returns>
        public async Task<Reply> GetReplyOfComment(Guid projectId, Guid annotationId, Guid replyId, int deepLevel = 0)
        {
            try
            {
                this.ComputeMainRoute(projectId, annotationId);
                return await this.GetEntity(replyId, deepLevel);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Creates a new <see cref="Reply" /> inside a <see cref="Comment" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="annotationId">The <see cref="Guid" /> of the <see cref="Comment" /></param>
        /// <param name="reply">The <see cref="Reply" />to create</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{Reply}" /></returns>
        public async Task<EntityRequestResponse<Reply>> CreateReply(Guid projectId, Guid annotationId, Reply reply)
        {
            try
            {
                this.ComputeMainRoute(projectId, annotationId);
                return await this.CreateEntity(reply, 0);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Updates a <see cref="Reply" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="commentId">The <see cref="Guid" /> of the <see cref="Comment" /></param>
        /// <param name="reply">The <see cref="Reply" />to update</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{Reply}" /></returns>
        public async Task<EntityRequestResponse<Reply>> UpdateReply(Guid projectId, Guid commentId, Reply reply)
        {
            try
            {
                this.ComputeMainRoute(projectId, commentId);
                return await this.UpdateEntity(reply, 0);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Deletes a <see cref="Reply" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="commentId">The <see cref="Guid" /> of the <see cref="Comment" /></param>
        /// <param name="reply">The <see cref="Reply" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /></returns>
        public async Task<RequestResponseDto> DeleteReply(Guid projectId, Guid commentId, Reply reply)
        {
            try
            {
                this.ComputeMainRoute(projectId, commentId);
                return await this.DeleteEntity(reply);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }
    }
}
