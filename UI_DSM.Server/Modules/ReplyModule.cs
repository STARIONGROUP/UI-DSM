// --------------------------------------------------------------------------------------------------------
// <copyright file="ReplyModule.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Modules
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Services.SearchService;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="ReplyModule" />  is a
    ///     <see cref="ContainedEntityModule{TEntity,TEntityDto, TEntityContainer}" /> to manage
    ///     <see cref="Reply" /> related to a <see cref="Comment" />
    /// </summary>
    [Route("api/Project/{projectId:guid}/Annotation/{annotationId:guid}/Reply")]
    public class ReplyModule : ContainedEntityModule<Reply, ReplyDto, Annotation>
    {
        /// <summary>
        ///     The route key for the ProjectId
        /// </summary>
        private const string ProjectKey = "projectId";

        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task GetEntities(IEntityManager<Reply> manager, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }

            await base.GetEntities(manager, context, deepLevel);
        }

        /// <summary>
        ///     Get a <see cref="ReplyDto" /> based on its <see cref="Guid" /> with all associated entities
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task GetEntity(IEntityManager<Reply> manager, Guid entityId, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }

            await base.GetEntity(manager, entityId, context, deepLevel);
        }

        /// <summary>
        ///     Tries to create a new <see cref="Reply" /> based on its <see cref="ReplyDto" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="dto">The <see cref="ReplyDto" /></param>
        /// <param name="searchService"></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task CreateEntity(IEntityManager<Reply> manager, ReplyDto dto, ISearchService searchService, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }

            dto.Author = participant.Id;
            await base.CreateEntity(manager, dto, searchService, context, deepLevel);
        }

        /// <summary>
        ///     Tries to delete an <see cref="Reply" /> defined by the given <see cref="Guid" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Reply" /> to delete</param>
        /// <param name="searchService">The <see cref="ISearchService"/></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /> as result</returns>
        [Authorize]
        public override async Task<RequestResponseDto> DeleteEntity(IEntityManager<Reply> manager, Guid entityId, ISearchService searchService, HttpContext context)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return new RequestResponseDto();
            }

            var reply = (await manager.GetEntity(entityId)).OfType<Reply>().FirstOrDefault();

            if (reply?.Author.Id != participant.Id)
            {
                context.Response.StatusCode = 403;

                return new RequestResponseDto()
                {
                    Errors = new List<string> { "Unable to delete a Comment from someelse" }
                };
            }

            return await base.DeleteEntity(manager, entityId, searchService, context);
        }

        /// <summary>
        ///     Tries to update an existing <see cref="Reply" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Reply" /></param>
        /// <param name="dto">The <see cref="ReplyDto" /></param>
        /// <param name="searchService">The <see cref="ISearchService"/></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" />as result</returns>
        [Authorize]
        public override async Task UpdateEntity(IEntityManager<Reply> manager, Guid entityId, ReplyDto dto, ISearchService searchService,
            HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }

            var reply = (await manager.GetEntity(entityId)).OfType<Reply>().FirstOrDefault();

            if (reply?.Author.Id != participant.Id)
            {
                context.Response.StatusCode = 403;
                return;
            }

            await base.UpdateEntity(manager, entityId, dto, searchService, context, deepLevel);
        }

        /// <summary>
        ///     Adds the <see cref="Reply" /> into the corresponding collection of the <see cref="Comment" />
        /// </summary>
        /// <param name="entity">The <see cref="Reply" /></param>
        /// <param name="container">The <see cref="Comment" /></param>
        protected override void AddEntityIntoContainerCollection(Reply entity, Annotation container)
        {
            if (container is not Comment comment)
            {
                throw new InvalidOperationException("The container of a Reply has to be a Comment");
            }

            comment.Replies.Add(entity);
        }

        /// <summary>
        ///     Verifies if the provided routes is correctly formatted with all containment
        /// </summary>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the result of the check</returns>
        protected override async Task<bool> AdditionalRouteValidation(HttpContext context)
        {
            var annotationManagar = context.RequestServices.GetService<IContainedEntityManager<Annotation>>();

            if (annotationManagar == null)
            {
                context.Response.StatusCode = 500;
                return false;
            }

            var annotationId = this.GetAdditionalRouteId(context.Request, this.ContainerRouteKey);
            var projectId = this.GetAdditionalRouteId(context.Request, ProjectKey);

            var comment = await annotationManagar.FindEntity(annotationId);

            if (comment is not Comment)
            {
                context.Response.StatusCode = 400;
                return false;
            }

            if (!await annotationManagar.EntityIsContainedBy(annotationId, projectId))
            {
                context.Response.StatusCode = 400;
                return false;
            }

            return true;
        }
    }
}
