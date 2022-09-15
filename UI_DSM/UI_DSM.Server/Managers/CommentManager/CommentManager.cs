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

    using NLog;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Managers.AnnotatableItemManager;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ReplyManager;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="Comment" />s
    /// </summary>
    public class CommentManager : ICommentManager
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
        ///     The <see cref="IReplyManager" />
        /// </summary>
        private readonly IReplyManager replyManager;

        /// <summary>
        ///     The <see cref="IAnnotatableItemManager" />
        /// </summary>
        private IAnnotatableItemManager annotatableItemManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommentManager" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        /// <param name="replyManager">The <see cref="IReplyManager" /></param>
        public CommentManager(DatabaseContext context, IParticipantManager participantManager, IReplyManager replyManager)
        {
            this.context = context;
            this.participantManager = participantManager;
            this.replyManager = replyManager;
        }

        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />s and associated <see cref="Entity" />
        /// </summary>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> as result</returns>
        public async Task<IEnumerable<Entity>> GetEntities(int deepLevel = 0)
        {
            var comments = await this.context.Comments.ToListAsync();
            return comments.SelectMany(x => x.GetAssociatedEntities(deepLevel)).DistinctBy(x => x.Id);
        }

        /// <summary>
        ///     Tries to get a <see cref="Entity" /> based on its <see cref="Guid" /> and its associated <see cref="Entity" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> if found</returns>
        public async Task<IEnumerable<Entity>> GetEntity(Guid entityId, int deepLevel = 0)
        {
            var comment = await this.FindEntity(entityId);

            return comment == null ? Enumerable.Empty<Entity>() : comment.GetAssociatedEntities(deepLevel);
        }

        /// <summary>
        ///     Tries to get a <see cref="Comment" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="Comment" /> if found</returns>
        public async Task<Comment> FindEntity(Guid entityId)
        {
            return await this.context.Comments.FindAsync(entityId);
        }

        /// <summary>
        ///     Tries to get all <see cref="Comment" /> based on their <see cref="Guid" />
        /// </summary>
        /// <param name="entitiesId">A collection of <see cref="Guid" /></param>
        /// <returns>A collection of <see cref="Comment" /></returns>
        public async Task<IEnumerable<Comment>> FindEntities(IEnumerable<Guid> entitiesId)
        {
            var entities = new List<Comment>();

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
        ///     Creates a new <see cref="Comment" /> and store it into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="entity">The <see cref="Comment" /> to create</param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        public async Task<EntityOperationResult<Comment>> CreateEntity(Comment entity)
        {
            var validations = this.context.ValidateModel(entity);

            if (validations.Any())
            {
                return EntityOperationResult<Comment>.Failed(validations.ToArray());
            }

            entity.CreatedOn = DateTime.UtcNow;

            if (!await this.VerifyContainer(entity))
            {
                return EntityOperationResult<Comment>.Failed($"Unable to compute the container the current entity with the id {entity.Id}");
            }

            var operationResult = new EntityOperationResult<Comment>(this.context.Add(entity), EntityState.Added);

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
        ///     Updates a <see cref="Comment" />
        /// </summary>
        /// <param name="entity">The <see cref="Comment" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public async Task<EntityOperationResult<Comment>> UpdateEntity(Comment entity)
        {
            var validations = this.context.ValidateModel(entity);

            if (validations.Any())
            {
                return EntityOperationResult<Comment>.Failed(validations.ToArray());
            }

            if (!await this.VerifyContainer(entity))
            {
                return EntityOperationResult<Comment>.Failed($"Unable to compute the container the current entity with the id {entity.Id}");
            }

            var operationResult = new EntityOperationResult<Comment>(this.context.Update(entity), EntityState.Modified, EntityState.Unchanged);

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
        ///     Deletes a <see cref="Comment" />
        /// </summary>
        /// <param name="entity">The <see cref="Comment" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the deletion</returns>
        public async Task<EntityOperationResult<Comment>> DeleteEntity(Comment entity)
        {
            if (!await this.VerifyContainer(entity))
            {
                return EntityOperationResult<Comment>.Failed($"Unable to compute the container the current entity with the id {entity.Id}");
            }

            var operationResult = new EntityOperationResult<Comment>(this.context.Remove(entity), EntityState.Deleted, EntityState.Detached);

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
        ///     Resolve all properties for the <see cref="Comment" />
        /// </summary>
        /// <param name="entity">The <see cref="Comment" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task ResolveProperties(Comment entity, EntityDto dto)
        {
            if (dto is not CommentDto commentDto)
            {
                return;
            }

            var relatedEntities = new Dictionary<Guid, Entity>();
            relatedEntities.InsertEntity(await this.participantManager.FindEntity(commentDto.Author));
            relatedEntities.InsertEntityCollection(await this.annotatableItemManager.FindEntities(commentDto.AnnotatableItems));
            relatedEntities.InsertEntityCollection(await this.replyManager.FindEntities(commentDto.Replies));

            entity.ResolveProperties(commentDto, relatedEntities);
        }

        /// <summary>
        ///     Finds an <see cref="Comment" /> and includes his <see cref="Entity" /> container
        /// </summary>
        /// <param name="entityId">The <see cref="Entity" /> id</param>
        /// <returns>A <see cref="Task" /> with the <see cref="Comment" /></returns>
        public async Task<Comment> FindEntityWithContainer(Guid entityId)
        {
            return await this.context.Comments.Where(x => x.Id == entityId)
                .Include(x => x.EntityContainer).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Gets all <see cref="Comment" /> that are contained by the <see cref="Entity" /> with the Id
        ///     <see cref="containerId" /> and associated <see cref="Entity" />
        /// </summary>
        /// <param name="containerId">The <see cref="Guid" /> of the container</param>
        /// <param name="deepLevel">The deep level</param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /></returns>
        public async Task<IEnumerable<Entity>> GetContainedEntities(Guid containerId, int deepLevel = 0)
        {
            var comments = await this.context.Comments.Where(x => x.EntityContainer != null
                                                                  && x.EntityContainer.Id == containerId).ToListAsync();

            return comments.SelectMany(x => x.GetAssociatedEntities(deepLevel)).DistinctBy(x => x.Id);
        }

        /// <summary>
        ///     Verifies if the container of the <see cref="Entity" /> has the given <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Entity" /></param>
        /// <param name="containerId">The <see cref="Guid" /> of the container</param>
        /// <returns>A <see cref="Task" /> with the value of the check</returns>
        public async Task<bool> EntityIsContainedBy(Guid entityId, Guid containerId)
        {
            var comment = await this.FindEntityWithContainer(entityId);
            return comment != null && comment.EntityContainer.Id == containerId;
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
