// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.ReviewObjectiveManager
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Managers.AnnotationManager;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ReviewCategoryManager;
    using UI_DSM.Server.Managers.ReviewTaskManager;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="ReviewObjective" />s
    /// </summary>
    public class ReviewObjectiveManager : ContainedEntityManager<ReviewObjective, Review>, IReviewObjectiveManager
    {
        /// <summary>
        ///     The <see cref="IAnnotationManager" />
        /// </summary>
        private readonly IAnnotationManager annotationManager;

        /// <summary>
        ///     The <see cref="IParticipantManager" />
        /// </summary>
        private readonly IParticipantManager participantManager;

        /// <summary>
        ///     The <see cref="IReviewCategoryManager" />
        /// </summary>
        private readonly IReviewCategoryManager reviewCategoryManager;

        /// <summary>
        ///     The <see cref="IReviewTaskManager" />
        /// </summary>
        private readonly IReviewTaskManager reviewTaskManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewObjectiveManager" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        /// <param name="reviewTaskManager">The <see cref="IReviewTaskManager" /></param>
        /// <param name="annotationManager">The <see cref="IAnnotationManager" /></param>
        /// <param name="reviewCategoryManager">The <see cref="IReviewCategoryManager" /></param>
        public ReviewObjectiveManager(DatabaseContext context, IParticipantManager participantManager, IReviewTaskManager reviewTaskManager,
            IAnnotationManager annotationManager, IReviewCategoryManager reviewCategoryManager) : base(context)
        {
            this.participantManager = participantManager;
            this.reviewTaskManager = reviewTaskManager;
            this.annotationManager = annotationManager;
            this.reviewCategoryManager = reviewCategoryManager;
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewObjective" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task ResolveProperties(ReviewObjective entity, EntityDto dto)
        {
            if (dto is not ReviewObjectiveDto reviewObjectiveDto)
            {
                return;
            }

            var relatedEntities = new Dictionary<Guid, Entity>();
            relatedEntities.InsertEntityCollection(await this.reviewTaskManager.FindEntities(reviewObjectiveDto.ReviewTasks));
            relatedEntities.InsertEntityCollection(await this.annotationManager.FindEntities(reviewObjectiveDto.Annotations));
            relatedEntities.InsertEntityCollection(await this.reviewCategoryManager.FindEntities(reviewObjectiveDto.ReviewCategories));
            entity.ResolveProperties(reviewObjectiveDto, relatedEntities);
        }

        /// <summary>
        ///     Updates a <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewObjective" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public override async Task<EntityOperationResult<ReviewObjective>> UpdateEntity(ReviewObjective entity)
        {
            if (!this.ValidateCurrentEntity(entity, out var entityOperationResult))
            {
                return entityOperationResult;
            }

            var foundEntity = await this.FindEntity(entity.Id);
            entity.ReviewObjectiveKind = foundEntity.ReviewObjectiveKind;

            return await this.UpdateEntityIntoContext(entity);
        }

        /// <summary>
        ///     Creates an <see cref="ReviewObjective" /> based on a template
        /// </summary>
        /// <param name="template">The <see cref="ReviewObjective" /> template</param>
        /// <param name="container">The <see cref="Review" /> container</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityOperationResult{TEntity}" /></returns>
        public async Task<EntityOperationResult<ReviewObjective>> CreateEntityBasedOnTemplate(ReviewObjective template, Review container)
        {
            if (container.ReviewObjectives.Any(x => x.ReviewObjectiveKind == template.ReviewObjectiveKind
                                                    && x.ReviewObjectiveKindNumber == template.ReviewObjectiveKindNumber))
            {
                return EntityOperationResult<ReviewObjective>.Failed($"A ReviewObjective {template.Title} already exists inside the Review");
            }

            var reviewObjective = new ReviewObjective(template);

            container.ReviewObjectives.Add(reviewObjective);
            this.SetSpecificPropertiesBeforeCreate(reviewObjective);
            var operationResult = new EntityOperationResult<ReviewObjective>(this.Context.Add(reviewObjective), EntityState.Added);

            if (operationResult.Entity != null)
            {
                this.reviewTaskManager.CreateEntitiesBasedOnTemplate(operationResult.Entity, template.ReviewTasks);
            }

            try
            {
                await this.Context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                this.HandleException(exception, operationResult);
            }

            return operationResult;
        }

        /// <summary>
        ///     Creates an <see cref="ReviewObjective" /> based on a template
        /// </summary>
        /// <param name="templates">The <see cref="IEnumerable{ReviewObjective}" /> template</param>
        /// <param name="container">The <see cref="Review" /> container</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityOperationResult{TEntity}" /></returns>
        public async Task<EntityOperationResult<ReviewObjective>> CreateEntityBasedOnTemplates(IEnumerable<ReviewObjective> templates, Review container)
        {
            var reviewObjectives = new List<ReviewObjective>();
            var entityEntries = new List<EntityEntry<ReviewObjective>>();
            var reviewTaskForReviewObjectiveTemplate = new Dictionary<Guid, List<ReviewTask>>();

            foreach (var template in templates)
            {
                if (container.ReviewObjectives.Any(x => x.ReviewObjectiveKind == template.ReviewObjectiveKind
                                                        && x.ReviewObjectiveKindNumber == template.ReviewObjectiveKindNumber))
                {
                    return EntityOperationResult<ReviewObjective>.Failed($"A ReviewObjective {template.Title} already exists inside the Review");
                }

                var reviewObjective = new ReviewObjective(template)
                {
                    Id = Guid.NewGuid()
                };

                container.ReviewObjectives.Add(reviewObjective);
                this.SetSpecificPropertiesBeforeCreate(reviewObjective);
                reviewObjectives.Add(reviewObjective);

                reviewTaskForReviewObjectiveTemplate.Add(reviewObjective.Id, template.ReviewTasks);
            }

            foreach (var reviewObjective in reviewObjectives)
            {
                entityEntries.Add(this.Context.Add(reviewObjective));
            }

            var operationResult = new EntityOperationResult<ReviewObjective>(entityEntries, EntityState.Added);

            if (operationResult.Entities != null || !operationResult.Entities.Any())
            {
                foreach (var reviewObjective in operationResult.Entities)
                {
                    this.reviewTaskManager.CreateEntitiesBasedOnTemplate(reviewObjective, reviewTaskForReviewObjectiveTemplate[reviewObjective.Id]);
                }
            }

            try
            {
                await this.Context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                this.HandleException(exception, operationResult);
            }

            return operationResult;
        }

        /// <summary>
        ///     Get a collection of existing <see cref="ReviewObjectiveCreationDto" /> of a <see cref="Review" />
        /// </summary>
        /// <param name="reviewId">The id of the <see cref="Review" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="ReviewObjective" /></returns>
        public IEnumerable<ReviewObjectiveCreationDto> GetReviewObjectiveCreationForReview(Guid reviewId)
        {
            var reviewObjectives = this.EntityDbSet.Where(x => x.EntityContainer.Id == reviewId).Select(x => new ReviewObjectiveCreationDto { Kind = x.ReviewObjectiveKind, KindNumber = x.ReviewObjectiveKindNumber });
            return reviewObjectives;
        }

        /// <summary>
        ///     Gets the number of open <see cref="ReviewTask" /> where the logged user is assigned to
        ///     and <see cref="Comment" /> for each <see cref="Project" />
        /// </summary>
        /// <param name="reviewObjectivesId">A collection of <see cref="Guid" /> for <see cref="ReviewObjective" />s</param>
        /// <param name="projectId">A <see cref="Guid" /> of <see cref="Project" />s</param>
        /// <param name="userName">The name of the current logged user</param>
        /// <returns> A <see cref="Dictionary{Guid,ComputedProjectProperties}" /></returns>
        public async Task<Dictionary<Guid, ComputedProjectProperties>> GetOpenTasksAndComments(IEnumerable<Guid> reviewObjectivesId, Guid projectId, string userName)
        {
            var dictionary = new Dictionary<Guid, ComputedProjectProperties>();

            foreach (var reviewObjectiveId in reviewObjectivesId)
            {
                var computedProperties = await this.GetOpenTasksAndComments(reviewObjectiveId, projectId, userName);

                if (computedProperties != null)
                {
                    dictionary[reviewObjectiveId] = computedProperties;
                }
            }

            return dictionary;
        }

        /// <summary>
        ///     Update the status of the <see cref="ReviewObjective" /> if necessary.
        /// </summary>
        /// <param name="reviewObjectiveId">The <see cref="Guid" /> of the <see cref="ReviewObjective" /></param>
        public async Task UpdateStatus(Guid reviewObjectiveId)
        {
            var reviewObjective = (await this.GetEntity(reviewObjectiveId)).OfType<ReviewObjective>().FirstOrDefault();

            if (reviewObjective == null)
            {
                return;
            }

            var currentStatus = reviewObjective.ReviewTasks.All(x => x.Status == StatusKind.Done) ? StatusKind.Done : StatusKind.Open;

            if (reviewObjective.Status != currentStatus)
            {
                reviewObjective.Status = currentStatus;
                await this.UpdateEntity(reviewObjective);
            }
        }

        /// <summary>
        ///    Gets the <see cref="SearchResultDto"/> based on a <see cref="Guid"/>
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="ReviewObjective" /></param>
        /// <returns>A URL</returns>
        public override async Task<SearchResultDto> GetSearchResult(Guid entityId)
        {
            var reviewObjective = await this.EntityDbSet.Where(x => x.Id == entityId)
                .Include(x => x.EntityContainer)
                .ThenInclude(x => x.EntityContainer).FirstOrDefaultAsync();

            if (reviewObjective == null)
            {
                return null;
            }

            var route = $"Project/{reviewObjective.EntityContainer.EntityContainer.Id}/Review/{reviewObjective.EntityContainer.Id}/ReviewObjective/{reviewObjective.Id}";

            return new SearchResultDto()
            {
                BaseUrl = route,
                ObjectKind = nameof(ReviewObjective),
                DisplayText = reviewObjective.Title,
                Location = $"{((Project)reviewObjective.EntityContainer.EntityContainer).ProjectName} > {((Review)reviewObjective.EntityContainer).Title}"
            };
        }

        /// <summary>
        ///     Gets all <see cref="Entity" /> that needs to be unindexed when the current <see cref="Entity" /> is delete
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the entity</param>
        /// <returns>A collection of <see cref="Entity" /></returns>
        public override async Task<IEnumerable<Entity>> GetExtraEntitiesToUnindex(Guid entityId)
        {
            var reviewObjective = await this.EntityDbSet.Where(x => x.Id == entityId)
                .Include(x => x.Annotations)
                .ThenInclude(x => (x as Comment).Replies)
                .Include(x => x.ReviewTasks)
                .FirstOrDefaultAsync();

            if (reviewObjective == null)
            {
                return Enumerable.Empty<Entity>();
            }

            var entities = new List<Entity>(reviewObjective.Annotations);
            entities.AddRange(reviewObjective.Annotations.OfType<Comment>().SelectMany(x => x.Replies));
            entities.AddRange(reviewObjective.ReviewTasks);
            return entities;
        }

        /// <summary>
        ///     Sets specific properties before the creation of the <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewObjective" /></param>
        protected override void SetSpecificPropertiesBeforeCreate(ReviewObjective entity)
        {
            var review = entity.EntityContainer as Review;
            entity.ReviewObjectiveNumber = 0;
            entity.ReviewObjectiveNumber = review!.ReviewObjectives.Max(x => x.ReviewObjectiveNumber) + 1;
            entity.CreatedOn = DateTime.UtcNow;
            entity.Status = StatusKind.Open;
        }

        /// <summary>
        ///     Gets the number of open <see cref="ReviewTask" /> where the logged user is assigned to
        ///     and <see cref="Comment" /> for a <see cref="Review" />
        /// </summary>
        /// <param name="reviewObjectiveId">A <see cref="Guid" /> for <see cref="ReviewObjective" /></param>
        /// <param name="projectId">A <see cref="Guid" /> of <see cref="Project" />s</param>
        /// <param name="userName">The name of the current logged user</param>
        /// <returns> A <see cref="ComputedProjectProperties" /></returns>
        private async Task<ComputedProjectProperties> GetOpenTasksAndComments(Guid reviewObjectiveId, Guid projectId, string userName)
        {
            if (this.EntityDbSet.All(x => x.Id != reviewObjectiveId))
            {
                return null;
            }

            var participant = await this.participantManager.GetParticipantForProject(projectId, userName);

            if (participant == null)
            {
                return null;
            }

            var tasks = this.EntityDbSet
                .Where(x => x.Id == reviewObjectiveId)
                .SelectMany(x => x.ReviewTasks)
                .Include(x => x.IsAssignedTo)
                .AsEnumerable()
                .Count(x => x.Status == StatusKind.Open && x.IsAssignedTo != null && x.IsAssignedTo.Any(p => p.Id == participant.Id));

            var comments = this.EntityDbSet
                .Where(x => x.Id == reviewObjectiveId)
                .SelectMany(x => x.Annotations)
                .OfType<Comment>()
                .Count();

            return new ComputedProjectProperties
            {
                TaskCount = tasks,
                CommentCount = comments
            };
        }
    }
}
