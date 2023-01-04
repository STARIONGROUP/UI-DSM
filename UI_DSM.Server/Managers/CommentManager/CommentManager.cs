// --------------------------------------------------------------------------------------------------------
// <copyright file="CommentManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.CommentManager
{
    using Microsoft.EntityFrameworkCore;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Managers.AnnotatableItemManager;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ReplyManager;
    using UI_DSM.Server.Managers.ReviewTaskManager;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="Comment" />s
    /// </summary>
    public class CommentManager : ContainedEntityManager<Comment, Project>, ICommentManager
    {
        /// <summary>
        ///     The <see cref="IParticipantManager" />
        /// </summary>
        private readonly IParticipantManager participantManager;

        /// <summary>
        ///     The <see cref="IReplyManager" />
        /// </summary>
        private readonly IReplyManager replyManager;

        /// <summary>
        ///     The <see cref="IAnnotatableItemManager" />
        /// </summary>
        private IAnnotatableItemManager annotatableItemManager;

        /// <summary>
        ///     The <see cref="IReviewTaskManager" />
        /// </summary>
        private IReviewTaskManager reviewTaskManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommentManager" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        /// <param name="replyManager">The <see cref="IReplyManager" /></param>
        /// <param name="reviewTaskManager">The <see cref="IReviewTaskManager" /></param>
        public CommentManager(DatabaseContext context, IParticipantManager participantManager, IReplyManager replyManager, IReviewTaskManager reviewTaskManager) : base(context)
        {
            this.participantManager = participantManager;
            this.replyManager = replyManager;
            this.reviewTaskManager = reviewTaskManager;
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="Comment" />
        /// </summary>
        /// <param name="entity">The <see cref="Comment" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task ResolveProperties(Comment entity, EntityDto dto)
        {
            if (dto is not CommentDto commentDto)
            {
                return;
            }

            var relatedEntities = new Dictionary<Guid, Entity>();
            relatedEntities.InsertEntity(await this.participantManager.FindEntity(commentDto.Author));
            relatedEntities.InsertEntityCollection(await this.annotatableItemManager.FindEntities(commentDto.AnnotatableItems));
            relatedEntities.InsertEntityCollection(await this.replyManager.FindEntities(commentDto.Replies));
            relatedEntities.InsertEntity(await this.reviewTaskManager.FindEntity(commentDto.CreatedInside));

            entity.ResolveProperties(commentDto, relatedEntities);
        }

        /// <summary>
        ///     Gets the <see cref="SearchResultDto" /> based on a <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Comment" /></param>
        /// <returns>A URL</returns>
        public override async Task<SearchResultDto> GetSearchResult(Guid entityId)
        {
            var comment = await this.EntityDbSet.Where(x => x.Id == entityId)
                .Include(x => x.Author)
                .Include(x => x.CreatedInside)
                .ThenInclude(x => x.EntityContainer)
                .ThenInclude(x => x.EntityContainer)
                .ThenInclude(x => x.EntityContainer).FirstOrDefaultAsync();

            if (comment == null)
            {
                return null;
            }

            var reviewTask = comment.CreatedInside;
            var reviewObjective = (ReviewObjective)reviewTask.EntityContainer;
            var review = (Review)reviewObjective.EntityContainer;
            var project = (Project)review.EntityContainer;

            var route = $"Project/{project.Id}/Review/{review.Id}/ReviewObjective/{reviewObjective.Id}/ReviewTask/{reviewTask.Id}";

            return new SearchResultDto
            {
                ObjectKind = nameof(Comment),
                BaseUrl = route,
                DisplayText = $"{comment.Author.ParticipantName}: {comment.Content}",
                Location = $"{project.ProjectName} > {review.Title} > {reviewObjective.Title} > {reviewTask.Description}"
            };
        }

        /// <summary>
        ///     Gets all <see cref="Entity" /> that needs to be unindexed when the current <see cref="Entity" /> is delete
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the entity</param>
        /// <returns>A collection of <see cref="Entity" /></returns>
        public override async Task<IEnumerable<Entity>> GetExtraEntitiesToUnindex(Guid entityId)
        {
            var comment = await this.EntityDbSet.Where(x => x.Id == entityId)
                .Include(x => x.Replies).FirstOrDefaultAsync();

            return comment == null ? Enumerable.Empty<Entity>() : comment.Replies;
        }

        /// <summary>
        ///     Injects a <see cref="IAnnotatableItemManager" /> to break the circular dependency
        /// </summary>
        /// <param name="manager">The <see cref="IAnnotatableItemManager" /></param>
        public void InjectManager(IAnnotatableItemManager manager)
        {
            this.annotatableItemManager = manager;
        }

        /// <summary>
        ///     Gets all <see cref="Comment" /> linked to a <see cref="AnnotatableItem" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="annotatableItemId">The <see cref="Guid" /> of the <see cref="AnnotatableItem" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /></returns>
        public async Task<IEnumerable<Entity>> GetCommentsOfAnnotatableItem(Guid projectId, Guid annotatableItemId)
        {
            var comments = this.EntityDbSet.Where(x => x.EntityContainer.Id == projectId
                                                       && x.AnnotatableItems.Any(item => item.Id == annotatableItemId))
                .Include(x => x.Author)
                .ThenInclude(x => x.User)
                .Include(x => x.AnnotatableItems)
                .Include(x => x.Replies)
                .ThenInclude(x => x.Author);

            var list = await comments.ToListAsync();

            return list.SelectMany(x => x.GetAssociatedEntities(1)).DistinctBy(x => x.Id);
        }

        /// <summary>
        ///     Updates a <see cref="Comment" />
        /// </summary>
        /// <param name="entity">The <see cref="Comment" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public override async Task<EntityOperationResult<Comment>> UpdateEntity(Comment entity)
        {
            if (!this.ValidateCurrentEntity(entity, out var entityOperationResult))
            {
                return entityOperationResult;
            }

            var foundEntity = await this.FindEntity(entity.Id);

            entity.View = foundEntity.View;
            entity.CreatedOn = foundEntity.CreatedOn.ToUniversalTime();

            var entry = this.Context.Entry(foundEntity);

            if (entry != null)
            {
                entity.CreatedInside = await this.reviewTaskManager.FindEntity(entry.Property<Guid?>("CreatedInsideId")
                    .OriginalValue.GetValueOrDefault());
            }

            return await this.UpdateEntityIntoContext(entity);
        }

        /// <summary>
        ///     Sets specific properties before the creation of the <see cref="Comment" />
        /// </summary>
        /// <param name="entity">The <see cref="Comment" /></param>
        protected override void SetSpecificPropertiesBeforeCreate(Comment entity)
        {
            entity.CreatedOn = DateTime.UtcNow;
            entity.Status = StatusKind.Open;
        }
    }
}
