// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.ReviewManager
{
    using Microsoft.EntityFrameworkCore;

    using NLog;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Managers.ArtifactManager;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ReviewObjectiveManager;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="Review" />s
    /// </summary>
    public class ReviewManager : IReviewManager
    {
        /// <summary>
        ///     The <see cref="IArtifactManager" />
        /// </summary>
        private readonly IArtifactManager artifactManager;

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
        ///     The <see cref="IReviewObjectiveManager" />
        /// </summary>
        private readonly IReviewObjectiveManager reviewObjectiveManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewManager" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        /// <param name="reviewObjectiveManager">The <see cref="IReviewObjectiveManager" /></param>
        /// <param name="artifactManager">The <see cref="IArtifactManager" /></param>
        public ReviewManager(DatabaseContext context, IParticipantManager participantManager, IReviewObjectiveManager reviewObjectiveManager,
            IArtifactManager artifactManager)
        {
            this.context = context;
            this.participantManager = participantManager;
            this.reviewObjectiveManager = reviewObjectiveManager;
            this.artifactManager = artifactManager;
        }

        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />s and associated <see cref="Entity" />
        /// </summary>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> as result</returns>
        public async Task<IEnumerable<Entity>> GetEntities(int deepLevel = 0)
        {
            var reviews = await this.context.Reviews.ToListAsync();
            return reviews.SelectMany(x => x.GetAssociatedEntities(deepLevel)).DistinctBy(x => x.Id);
        }

        /// <summary>
        ///     Tries to get a <see cref="Entity" /> based on its <see cref="Guid" /> and its associated <see cref="Entity" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> if found</returns>
        public async Task<IEnumerable<Entity>> GetEntity(Guid entityId, int deepLevel = 0)
        {
            var review = await this.FindEntity(entityId);

            return review == null ? Enumerable.Empty<Entity>() : review.GetAssociatedEntities(deepLevel);
        }

        /// <summary>
        ///     Tries to get a <see cref="Review" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="Review" /> if found</returns>
        public async Task<Review> FindEntity(Guid entityId)
        {
            return await this.context.Reviews.FindAsync(entityId);
        }

        /// <summary>
        ///     Tries to get all <see cref="Review" /> based on their <see cref="Guid" />
        /// </summary>
        /// <param name="entitiesId">A collection of <see cref="Guid" /></param>
        /// <returns>A collection of <see cref="Review" /></returns>
        public async Task<IEnumerable<Review>> FindEntities(IEnumerable<Guid> entitiesId)
        {
            var entities = new List<Review>();

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
        ///     Creates a new <see cref="Review" /> and store it into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="entity">The <see cref="Review" /> to create</param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        public async Task<EntityOperationResult<Review>> CreateEntity(Review entity)
        {
            var validations = this.context.ValidateModel(entity);

            if (validations.Any())
            {
                return EntityOperationResult<Review>.Failed(validations.ToArray());
            }

            if (!await this.VerifyContainer(entity))
            {
                return EntityOperationResult<Review>.Failed($"Unable to compute the container the current entity with the id {entity.Id}");
            }

            var project = entity.EntityContainer as Project;
            entity.ReviewNumber = 0;
            entity.ReviewNumber = project!.Reviews.Max(x => x.ReviewNumber) + 1;
            entity.CreatedOn = DateTime.UtcNow;
            entity.Status = StatusKind.Open;

            var operationResult = new EntityOperationResult<Review>(this.context.Add(entity), EntityState.Added);

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
        ///     Updates a <see cref="Review" />
        /// </summary>
        /// <param name="entity">The <see cref="Review" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public async Task<EntityOperationResult<Review>> UpdateEntity(Review entity)
        {
            var validations = this.context.ValidateModel(entity);

            if (validations.Any())
            {
                return EntityOperationResult<Review>.Failed(validations.ToArray());
            }

            var operationResult = new EntityOperationResult<Review>(this.context.Update(entity), EntityState.Modified, EntityState.Unchanged);

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
        ///     Deletes a <see cref="Review" />
        /// </summary>
        /// <param name="entity">The <see cref="Review" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the deletion</returns>
        public async Task<EntityOperationResult<Review>> DeleteEntity(Review entity)
        {
            if (!await this.VerifyContainer(entity))
            {
                return EntityOperationResult<Review>.Failed($"Unable to compute the container the current entity with the id {entity.Id}");
            }

            var operationResult = new EntityOperationResult<Review>(this.context.Remove(entity), EntityState.Deleted, EntityState.Detached);

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
        ///     Resolve all properties for the <see cref="Review" />
        /// </summary>
        /// <param name="entity">The <see cref="Review" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task ResolveProperties(Review entity, EntityDto dto)
        {
            if (dto is not ReviewDto reviewDto)
            {
                return;
            }

            var relatedEntities = new Dictionary<Guid, Entity>();
            relatedEntities.InsertEntity(await this.participantManager.FindEntity(reviewDto.Author));
            relatedEntities.InsertEntityCollection(await this.reviewObjectiveManager.FindEntities(reviewDto.ReviewObjectives));
            relatedEntities.InsertEntityCollection(await this.artifactManager.FindEntities(reviewDto.Artifacts));
            entity.ResolveProperties(reviewDto, relatedEntities);
        }

        /// <summary>
        ///     Gets all <see cref="Review" /> that are contained by the <see cref="Entity" /> with the Id
        ///     <see cref="containerId" /> and associated <see cref="Entity" />
        /// </summary>
        /// <param name="containerId">The <see cref="Guid" /> of the container</param>
        /// <param name="deepLevel">The deep level</param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /></returns>
        public async Task<IEnumerable<Entity>> GetContainedEntities(Guid containerId, int deepLevel = 0)
        {
            var reviews = await this.context.Reviews.Where(x => x.EntityContainer != null && x.EntityContainer.Id == containerId).ToListAsync();
            return reviews.SelectMany(x => x.GetAssociatedEntities(deepLevel)).DistinctBy(x => x.Id);
        }

        /// <summary>
        ///     Finds an <see cref="Review" /> and includes his <see cref="Entity" /> container
        /// </summary>
        /// <param name="entityId">The <see cref="Entity" /> id</param>
        /// <returns>A <see cref="Task" /> with the <see cref="Review" /></returns>
        public async Task<Review> FindEntityWithContainer(Guid entityId)
        {
            return await this.context.Reviews.Where(x => x.Id == entityId)
                .Include(x => x.EntityContainer).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Verifies if the container of the <see cref="Entity" /> has the given <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Entity" /></param>
        /// <param name="containerId">The <see cref="Guid" /> of the container</param>
        /// <returns>A <see cref="Task" /> with the value of the check</returns>
        public async Task<bool> EntityIsContainedBy(Guid entityId, Guid containerId)
        {
            var review = await this.FindEntityWithContainer(entityId);
            return review != null && review.EntityContainer.Id == containerId;
        }

        /// <summary>
        ///     Check if the container of the <see cref="Entity" /> exists inside the database
        /// </summary>
        /// <param name="entity">The <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with the result of the check</returns>
        private async Task<bool> VerifyContainer(Entity entity)
        {
            return entity.EntityContainer != null && await this.context.Projects.FindAsync(entity.EntityContainer.Id) != null;
        }
    }
}
