// --------------------------------------------------------------------------------------------------------
// <copyright file="FeedbackManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.FeedbackManager
{
    using Microsoft.EntityFrameworkCore;

    using NLog;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Managers.AnnotatableItemManager;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="Feedback" />s
    /// </summary>
    public class FeedbackManager : IFeedbackManager
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
        ///     The <see cref="IAnnotatableItemManager" />
        /// </summary>
        private IAnnotatableItemManager annotatableItemManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FeedbackManager" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        public FeedbackManager(DatabaseContext context, IParticipantManager participantManager)
        {
            this.context = context;
            this.participantManager = participantManager;
        }

        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />s and associated <see cref="Entity" />
        /// </summary>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> as result</returns>
        public async Task<IEnumerable<Entity>> GetEntities(int deepLevel = 0)
        {
            var feedbacks = await this.context.Feedbacks.ToListAsync();
            return feedbacks.SelectMany(x => x.GetAssociatedEntities(deepLevel)).DistinctBy(x => x.Id);
        }

        /// <summary>
        ///     Tries to get a <see cref="Entity" /> based on its <see cref="Guid" /> and its associated <see cref="Entity" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> if found</returns>
        public async Task<IEnumerable<Entity>> GetEntity(Guid entityId, int deepLevel = 0)
        {
            var feedback = await this.FindEntity(entityId);

            return feedback == null ? Enumerable.Empty<Entity>() : feedback.GetAssociatedEntities(deepLevel);
        }

        /// <summary>
        ///     Tries to get a <see cref="Feedback" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="Feedback" /> if found</returns>
        public async Task<Feedback> FindEntity(Guid entityId)
        {
            return await this.context.Feedbacks.FindAsync(entityId);
        }

        /// <summary>
        ///     Tries to get all <see cref="Feedback" /> based on their <see cref="Guid" />
        /// </summary>
        /// <param name="entitiesId">A collection of <see cref="Guid" /></param>
        /// <returns>A collection of <see cref="Feedback" /></returns>
        public async Task<IEnumerable<Feedback>> FindEntities(IEnumerable<Guid> entitiesId)
        {
            var entities = new List<Feedback>();

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
        ///     Creates a new <see cref="Feedback" /> and store it into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="entity">The <see cref="Feedback" /> to create</param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        public async Task<EntityOperationResult<Feedback>> CreateEntity(Feedback entity)
        {
            var validations = this.context.ValidateModel(entity);

            if (validations.Any())
            {
                return EntityOperationResult<Feedback>.Failed(validations.ToArray());
            }

            entity.CreatedOn = DateTime.UtcNow;

            if (!await this.VerifyContainer(entity))
            {
                return EntityOperationResult<Feedback>.Failed($"Unable to compute the container the current entity with the id {entity.Id}");
            }

            var operationResult = new EntityOperationResult<Feedback>(this.context.Add(entity), EntityState.Added);

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
        ///     Updates a <see cref="Feedback" />
        /// </summary>
        /// <param name="entity">The <see cref="Feedback" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public async Task<EntityOperationResult<Feedback>> UpdateEntity(Feedback entity)
        {
            var validations = this.context.ValidateModel(entity);

            if (validations.Any())
            {
                return EntityOperationResult<Feedback>.Failed(validations.ToArray());
            }
            
            if (!await this.VerifyContainer(entity))
            {
                return EntityOperationResult<Feedback>.Failed($"Unable to compute the container the current entity with the id {entity.Id}");
            }

            var operationResult = new EntityOperationResult<Feedback>(this.context.Update(entity), EntityState.Modified, EntityState.Unchanged);

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
        ///     Deletes a <see cref="Feedback" />
        /// </summary>
        /// <param name="entity">The <see cref="Feedback" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the deletion</returns>
        public async Task<EntityOperationResult<Feedback>> DeleteEntity(Feedback entity)
        {
            if (!await this.VerifyContainer(entity))
            {
                return EntityOperationResult<Feedback>.Failed($"Unable to compute the container the current entity with the id {entity.Id}");
            }

            var operationResult = new EntityOperationResult<Feedback>(this.context.Remove(entity), EntityState.Deleted, EntityState.Detached);

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
        ///     Resolve all properties for the <see cref="Feedback" />
        /// </summary>
        /// <param name="entity">The <see cref="Feedback" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task ResolveProperties(Feedback entity, EntityDto dto)
        {
            if (dto is not FeedbackDto feedbackDto)
            {
                return;
            }

            var relatedEntities = new Dictionary<Guid, Entity>();
            relatedEntities.InsertEntity(await this.participantManager.FindEntity(feedbackDto.Author));
            relatedEntities.InsertEntityCollection(await this.annotatableItemManager.FindEntities(feedbackDto.AnnotatableItems));

            entity.ResolveProperties(feedbackDto, relatedEntities);
        }

        /// <summary>
        ///     Finds an <see cref="Feedback" /> and includes his <see cref="Entity" /> container
        /// </summary>
        /// <param name="entityId">The <see cref="Entity" /> id</param>
        /// <returns>A <see cref="Task" /> with the <see cref="Feedback" /></returns>
        public async Task<Feedback> FindEntityWithContainer(Guid entityId)
        {
            return await this.context.Feedbacks.Where(x => x.Id == entityId)
                .Include(x => x.EntityContainer).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Gets all <see cref="Feedback" /> that are contained by the <see cref="Entity" /> with the Id
        ///     <see cref="containerId" /> and associated <see cref="Entity" />
        /// </summary>
        /// <param name="containerId">The <see cref="Guid" /> of the container</param>
        /// <param name="deepLevel">The deep level</param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /></returns>
        public async Task<IEnumerable<Entity>> GetContainedEntities(Guid containerId, int deepLevel = 0)
        {
            var feedbacks = await this.context.Feedbacks.Where(x => x.EntityContainer != null
                                                                  && x.EntityContainer.Id == containerId).ToListAsync();

            return feedbacks.SelectMany(x => x.GetAssociatedEntities(deepLevel)).DistinctBy(x => x.Id);
        }

        /// <summary>
        ///     Verifies if the container of the <see cref="Entity" /> has the given <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Entity" /></param>
        /// <param name="containerId">The <see cref="Guid" /> of the container</param>
        /// <returns>A <see cref="Task" /> with the value of the check</returns>
        public async Task<bool> EntityIsContainedBy(Guid entityId, Guid containerId)
        {
            var feedback = await this.FindEntityWithContainer(entityId);
            return feedback != null && feedback.EntityContainer.Id == containerId;
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
