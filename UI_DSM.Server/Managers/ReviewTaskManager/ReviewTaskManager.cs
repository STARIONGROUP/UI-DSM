// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewTaskManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.ReviewTaskManager
{
    using DynamicData;

    using Microsoft.EntityFrameworkCore;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ReviewObjectiveManager;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="ReviewTask" />s
    /// </summary>
    public class ReviewTaskManager : ContainedEntityManager<ReviewTask, ReviewObjective>, IReviewTaskManager
    {
        /// <summary>
        ///     The <see cref="IParticipantManager" />
        /// </summary>
        private readonly IParticipantManager participantManager;

        /// <summary>
        ///     The <see cref="IReviewObjectiveManager" />
        /// </summary>
        private IReviewObjectiveManager reviewObjectiveManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewTaskManager" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        public ReviewTaskManager(DatabaseContext context, IParticipantManager participantManager) : base(context)
        {
            this.participantManager = participantManager;
        }

        /// <summary>
        ///     Updates a <see cref="ReviewTask" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewTask" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public override async Task<EntityOperationResult<ReviewTask>> UpdateEntity(ReviewTask entity)
        {
            var selectedParticipants = entity.IsAssignedTo.ToList();

            if (!this.ValidateCurrentEntity(entity, out var entityOperationResult))
            {
                return entityOperationResult;
            }

            var reviewTask = (await this.GetEntity(entity.Id)).OfType<ReviewTask>().First();
            reviewTask.Status = entity.Status;
            var deletedParticipants = reviewTask.IsAssignedTo.Where(x => selectedParticipants.All(p => p.Id != x.Id));
            var addedParticipants = selectedParticipants.Where(x => reviewTask.IsAssignedTo.All(p => p.Id != x.Id));

            foreach (var participant in deletedParticipants.ToList())
            {
                reviewTask.IsAssignedTo.Remove(participant);
            }

            foreach (var addedParticipant in addedParticipants.ToList())
            {
                reviewTask.IsAssignedTo.Add(addedParticipant);
            }

            entity.CreatedOn = entity.CreatedOn.ToUniversalTime();
            var operation = await this.UpdateEntityIntoContext(reviewTask);

            if (this.reviewObjectiveManager != null)
            {
                var foundEntity = await this.FindEntityWithContainer(entity.Id);
                await this.reviewObjectiveManager.UpdateStatus(foundEntity.EntityContainer.Id);
            }

            return operation;
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="ReviewTask" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewTask" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task ResolveProperties(ReviewTask entity, EntityDto dto)
        {
            if (dto is not ReviewTaskDto reviewTaskDto)
            {
                return;
            }

            var relatedEntities = new Dictionary<Guid, Entity>();
            relatedEntities.InsertEntityCollection(await this.participantManager.FindEntities(reviewTaskDto.IsAssignedTo));
            entity.ResolveProperties(reviewTaskDto, relatedEntities);
        }

        /// <summary>
        ///     Create <see cref="ReviewTask" />s based on templates
        /// </summary>
        /// <param name="container">The <see cref="ReviewObjective" /> container</param>
        /// <param name="templateReviewTasks">A collection of <see cref="ReviewTask" /></param>
        public void CreateEntitiesBasedOnTemplate(ReviewObjective container, List<ReviewTask> templateReviewTasks)
        {
            foreach (var templateReviewTask in templateReviewTasks)
            {
                var reviewTask = new ReviewTask(templateReviewTask);

                this.SetSpecificPropertiesBeforeCreate(reviewTask);

                container.ReviewTasks.Add(reviewTask);
                this.Context.Add(reviewTask);
            }
        }

        /// <summary>
        ///     Injects the <see cref="IReviewObjectiveManager" /> to avoid circular dependency
        /// </summary>
        /// <param name="manager">The <see cref="IReviewObjectiveManager" /></param>
        public void InjectManager(IReviewObjectiveManager manager)
        {
            this.reviewObjectiveManager = manager;
        }

        /// <summary>
        ///     Gets the <see cref="SearchResultDto" /> based on a <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="ReviewTask" /></param>
        /// <returns>A URL</returns>
        public override async Task<SearchResultDto> GetSearchResult(Guid entityId)
        {
            var reviewTask = await this.EntityDbSet.Where(x => x.Id == entityId)
                .Include(x => x.EntityContainer)
                .ThenInclude(x => x.EntityContainer)
                .ThenInclude(x => x.EntityContainer)
                .FirstOrDefaultAsync();

            if (reviewTask == null)
            {
                return null;
            }

            var route = $"Project/{reviewTask.EntityContainer.EntityContainer.EntityContainer.Id}/Review/{reviewTask.EntityContainer.EntityContainer.Id}/ReviewObjective/{reviewTask.EntityContainer.Id}/ReviewTask/{reviewTask.Id}";

            return new SearchResultDto
            {
                BaseUrl = route,
                ObjectKind = nameof(ReviewTask),
                DisplayText = reviewTask.Description,
                Location = $"{((Project)reviewTask.EntityContainer.EntityContainer.EntityContainer).ProjectName} > " +
                           $"{((Review)reviewTask.EntityContainer.EntityContainer).Title} > " +
                           $"{((ReviewObjective)reviewTask.EntityContainer).Title}"
            };
        }

        /// <summary>
        ///     Gets all <see cref="Entity" /> that needs to be unindexed when the current <see cref="Entity" /> is delete
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the entity</param>
        /// <returns>A collection of <see cref="Entity" /></returns>
        public override async Task<IEnumerable<Entity>> GetExtraEntitiesToUnindex(Guid entityId)
        {
            await Task.CompletedTask;
            return Enumerable.Empty<Entity>();
        }

        /// <summary>
        ///     Gets the number of comments that are associated to <see cref="ReviewTask" />s
        /// </summary>
        /// <param name="reviewTasksId">A collection of <see cref="Guid" /> for <see cref="ReviewTask" /></param>
        /// <returns>A <see cref="Dictionary{Guid, AdditionalComputedProperties}" /></returns>
        public async Task<Dictionary<Guid, AdditionalComputedProperties>> GetCommentsCount(IEnumerable<Guid> reviewTasksId)
        {
            var dictionary = new Dictionary<Guid, AdditionalComputedProperties>();

            foreach (var reviewTaskId in reviewTasksId)
            {
                var computedProperties = await this.GetCommentsCount(reviewTaskId);

                if (computedProperties != null)
                {
                    dictionary[reviewTaskId] = computedProperties;
                }
            }

            return dictionary;
        }

        /// <summary>
        ///     Gets all <see cref="ReviewTask" /> with related <see cref="Entity" /> and Container that are contained inside a
        ///     <see cref="Review" /> and could access to a <see cref="View" />
        /// </summary>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <param name="view">the <see cref="View" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /></returns>
        public async Task<IEnumerable<Entity>> GetReviewTasksForView(Guid projectId, Guid reviewId, View view)
        {
            var reviewTasks = await this.EntityDbSet.Where(x => (x.EntityContainer.EntityContainer.Id == reviewId && x.EntityContainer.EntityContainer.EntityContainer.Id==projectId) &&
                                                                (x.MainView == view
                                                                || x.AdditionalView == view
                                                                || x.OptionalView == view || ((ReviewObjective)x.EntityContainer).RelatedViews.Contains(view)))
                .Include(x => x.EntityContainer)
                .BuildIncludeEntityQueryable(0)
                .ToListAsync();

            var entities = reviewTasks.SelectMany(x => x.GetAssociatedEntities()).DistinctBy(x => x.Id).ToList();
            entities.Add(reviewTasks.Select(x => x.EntityContainer).DistinctBy(x => x.Id));
            return entities;
        }

        /// <summary>
        ///     Sets specific properties before the creation of the <see cref="ReviewTask" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewTask" /></param>
        protected override void SetSpecificPropertiesBeforeCreate(ReviewTask entity)
        {
            entity.CreatedOn = DateTime.UtcNow;
            entity.Status = StatusKind.Open;
        }

        /// <summary>
        ///     Gets the number of comments that are associated to a <see cref="ReviewTask" />
        /// </summary>
        /// <param name="reviewTasksId">The <see cref="Guid" /> for <see cref="ReviewTask" /></param>
        /// <returns>A <see cref="Dictionary{Guid, AdditionalComputedProperties}" /></returns>
        private async Task<AdditionalComputedProperties> GetCommentsCount(Guid reviewTasksId)
        {
            if (this.EntityDbSet.All(x => x.Id != reviewTasksId))
            {
                return null;
            }

            var comments = await this.Context.Comments.Where(x => x.CreatedInside.Id == reviewTasksId)
                .ToListAsync();

            return new AdditionalComputedProperties
            {
                TaskCount = 0,
                OpenCommentCount = comments.Count(x => x.Status == StatusKind.Open),
                TotalCommentCount = comments.Count
            };
        }
    }
}
