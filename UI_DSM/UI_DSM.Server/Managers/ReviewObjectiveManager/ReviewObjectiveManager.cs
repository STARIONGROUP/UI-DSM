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

    using NLog;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Managers.AnnotationManager;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ReviewTaskManager;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="ReviewObjective" />s
    /// </summary>
    public class ReviewObjectiveManager : IReviewObjectiveManager
    {
        /// <summary>
        ///     The <see cref="DatabaseContext" />
        /// </summary>
        private readonly DatabaseContext context;

        /// <summary>
        ///     The <see cref="NLog" /> logger
        /// </summary>
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     The <see cref="IParticipantManager" />
        /// </summary>
        private readonly IParticipantManager participantManager;

        /// <summary>
        ///     The <see cref="IReviewTaskManager" />
        /// </summary>
        private readonly IReviewTaskManager reviewTaskManager;

        /// <summary>
        /// The <see cref="IAnnotationManager"/>
        /// </summary>
        private readonly IAnnotationManager annotationManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewObjectiveManager" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        /// <param name="reviewTaskManager">The <see cref="IReviewTaskManager" /></param>
        /// <param name="annotationManager">The <see cref="IAnnotationManager"/></param>
        public ReviewObjectiveManager(DatabaseContext context, IParticipantManager participantManager, IReviewTaskManager reviewTaskManager,
            IAnnotationManager annotationManager)
        {
            this.context = context;
            this.participantManager = participantManager;
            this.reviewTaskManager = reviewTaskManager;
            this.annotationManager = annotationManager;
        }

        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />s and associated <see cref="Entity" />
        /// </summary>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> as result</returns>
        public async Task<IEnumerable<Entity>> GetEntities(int deepLevel = 0)
        {
            var reviewObjectives = await this.context.ReviewObjectives.ToListAsync();
            return reviewObjectives.SelectMany(x => x.GetAssociatedEntities(deepLevel)).DistinctBy(x => x.Id);
        }

        /// <summary>
        ///     Tries to get a <see cref="Entity" /> based on its <see cref="Guid" /> and its associated <see cref="Entity" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> if found</returns>
        public async Task<IEnumerable<Entity>> GetEntity(Guid entityId, int deepLevel = 0)
        {
            var reviewObjective = await this.FindEntity(entityId);

            return reviewObjective == null ? Enumerable.Empty<Entity>() : reviewObjective.GetAssociatedEntities(deepLevel);
        }

        /// <summary>
        ///     Tries to get a <see cref="ReviewObjective" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="ReviewObjective" /> if found</returns>
        public async Task<ReviewObjective> FindEntity(Guid entityId)
        {
            return await this.context.ReviewObjectives.FindAsync(entityId);
        }

        /// <summary>
        ///     Tries to get all <see cref="ReviewObjective" /> based on their <see cref="Guid" />
        /// </summary>
        /// <param name="entitiesId">A collection of <see cref="Guid" /></param>
        /// <returns>A collection of <see cref="ReviewObjective" /></returns>
        public async Task<IEnumerable<ReviewObjective>> FindEntities(IEnumerable<Guid> entitiesId)
        {
            var entities = new List<ReviewObjective>();

            foreach (var id in entitiesId)
            {
                var entity = await this.FindEntity(id);

                if (entity != null)
                {
                    entities.Add(entity);
                }
            }

            return entities;
        }

        /// <summary>
        ///     Creates a new <see cref="ReviewObjective" /> and store it into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewObjective" /> to create</param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        public async Task<EntityOperationResult<ReviewObjective>> CreateEntity(ReviewObjective entity)
        {
            var validations = this.context.ValidateModel(entity);

            if (validations.Any())
            {
                return EntityOperationResult<ReviewObjective>.Failed(validations.ToArray());
            }

            if (!await this.VerifyContainer(entity))
            {
                return EntityOperationResult<ReviewObjective>.Failed($"Unable to compute the container the current entity with the id {entity.Id}");
            }

            var review = entity.EntityContainer as Review;
            entity.ReviewObjectiveNumber = 0;
            entity.ReviewObjectiveNumber = review!.ReviewObjectives.Max(x => x.ReviewObjectiveNumber) + 1;
            entity.CreatedOn = DateTime.UtcNow;
            entity.Status = StatusKind.Open;

            var operationResult = new EntityOperationResult<ReviewObjective>(this.context.Add(entity), EntityState.Added);

            try
            {
                await this.context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                operationResult.HandleExpection(exception);
                this.logger.Error(exception);
            }

            return operationResult;
        }

        /// <summary>
        ///     Updates a <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewObjective" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public async Task<EntityOperationResult<ReviewObjective>> UpdateEntity(ReviewObjective entity)
        {
            var validations = this.context.ValidateModel(entity);

            if (validations.Any())
            {
                return EntityOperationResult<ReviewObjective>.Failed(validations.ToArray());
            }

            var operationResult = new EntityOperationResult<ReviewObjective>(this.context.Update(entity), EntityState.Modified, EntityState.Unchanged);

            try
            {
                await this.context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                operationResult.HandleExpection(exception);
                this.logger.Error(exception.Message);
            }

            return operationResult;
        }

        /// <summary>
        ///     Deletes a <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewObjective" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the deletion</returns>
        public async Task<EntityOperationResult<ReviewObjective>> DeleteEntity(ReviewObjective entity)
        {
            if (!await this.VerifyContainer(entity))
            {
                return EntityOperationResult<ReviewObjective>.Failed($"Unable to compute the container the current entity with the id {entity.Id}");
            }

            var operationResult = new EntityOperationResult<ReviewObjective>(this.context.Remove(entity), EntityState.Deleted, EntityState.Detached);

            try
            {
                await this.context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                operationResult.HandleExpection(exception);
                this.logger.Error(exception.Message);
            }

            return operationResult;
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewObjective" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task ResolveProperties(ReviewObjective entity, EntityDto dto)
        {
            if (dto is not ReviewObjectiveDto reviewObjectiveDto)
            {
                return;
            }

            var relatedEntities = new Dictionary<Guid, Entity>();
            relatedEntities.InsertEntity(await this.participantManager.FindEntity(reviewObjectiveDto.Author));
            relatedEntities.InsertEntityCollection(await this.reviewTaskManager.FindEntities(reviewObjectiveDto.ReviewTasks));
            relatedEntities.InsertEntityCollection(await this.annotationManager.FindEntities(reviewObjectiveDto.Annotations));
            entity.ResolveProperties(reviewObjectiveDto, relatedEntities);
        }

        /// <summary>
        ///     Finds an <see cref="ReviewObjective" /> and includes his <see cref="Entity" /> container
        /// </summary>
        /// <param name="entityId">The <see cref="Entity" /> id</param>
        /// <returns>A <see cref="Task" /> with the <see cref="ReviewObjective" /></returns>
        public async Task<ReviewObjective> FindEntityWithContainer(Guid entityId)
        {
            return await this.context.ReviewObjectives.Where(x => x.Id == entityId)
                .Include(x => x.EntityContainer).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Gets all <see cref="Entity" /> that are contained by the <see cref="Entity" /> with the Id
        ///     <see cref="containerId" /> and associated <see cref="Entity" />
        /// </summary>
        /// <param name="containerId">The <see cref="Guid" /> of the container</param>
        /// <param name="deepLevel">The deep level</param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /></returns>
        public async Task<IEnumerable<Entity>> GetContainedEntities(Guid containerId, int deepLevel = 0)
        {
            var reviews = await this.context.ReviewObjectives.Where(x => x.EntityContainer != null && x.EntityContainer.Id == containerId).ToListAsync();
            return reviews.SelectMany(x => x.GetAssociatedEntities(deepLevel)).DistinctBy(x => x.Id);
        }

        /// <summary>
        ///     Verifies if the container of the <see cref="Entity" /> has the given <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Entity" /></param>
        /// <param name="containerId">The <see cref="Guid" /> of the container</param>
        /// <returns>A <see cref="Task" /> with the value of the check</returns>
        public async Task<bool> EntityIsContainedBy(Guid entityId, Guid containerId)
        {
            var reviewObjective = await this.FindEntityWithContainer(entityId);
            return reviewObjective != null && reviewObjective.EntityContainer.Id == containerId;
        }

        /// <summary>
        ///     Check if the container of the <see cref="Entity" /> exists inside the database
        /// </summary>
        /// <param name="entity">The <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with the result of the check</returns>
        private async Task<bool> VerifyContainer(Entity entity)
        {
            return entity.EntityContainer != null && await this.context.Reviews.FindAsync(entity.EntityContainer.Id) != null;
        }
    }
}
