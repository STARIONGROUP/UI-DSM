// --------------------------------------------------------------------------------------------------------
// <copyright file="AnnotationManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.AnnotationManager
{
    using UI_DSM.Server.Context;
    using UI_DSM.Server.Managers.AnnotatableItemManager;
    using UI_DSM.Server.Managers.CommentManager;
    using UI_DSM.Server.Managers.FeedbackManager;
    using UI_DSM.Server.Managers.NoteManager;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="Annotation" />s
    /// </summary>
    public class AnnotationManager : IAnnotationManager
    {
        /// <summary>
        ///     The <see cref="ICommentManager" />
        /// </summary>
        private readonly ICommentManager commentManager;

        /// <summary>
        ///     The <see cref="IFeedbackManager" />
        /// </summary>
        private readonly IFeedbackManager feedbackManager;

        /// <summary>
        ///     The <see cref="INoteManager" />
        /// </summary>
        private readonly INoteManager noteManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AnnotationManager" /> class.
        /// </summary>
        /// <param name="commentManager">The <see cref="ICommentManager" /></param>
        /// <param name="feedbackManager">The <see cref="IFeedbackManager" />></param>
        /// <param name="noteManager">The <see cref="INoteManager" /></param>
        public AnnotationManager(ICommentManager commentManager, IFeedbackManager feedbackManager, INoteManager noteManager)
        {
            this.commentManager = commentManager;
            this.feedbackManager = feedbackManager;
            this.noteManager = noteManager;
        }

        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />s and associated <see cref="Entity" />
        /// </summary>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> as result</returns>
        public async Task<IEnumerable<Entity>> GetEntities(int deepLevel = 0)
        {
            var annotations = new List<Entity>();
            annotations.AddRange(await this.commentManager.GetEntities(deepLevel));
            annotations.AddRange(await this.feedbackManager.GetEntities(deepLevel));
            annotations.AddRange(await this.noteManager.GetEntities(deepLevel));
            return annotations.DistinctBy(x => x.Id);
        }

        /// <summary>
        ///     Tries to get a <see cref="Entity" /> based on its <see cref="Guid" /> and its associated <see cref="Entity" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> if found</returns>
        public async Task<IEnumerable<Entity>> GetEntity(Guid entityId, int deepLevel = 0)
        {
            var annotation = new List<Entity>();
            annotation.AddRange(await this.commentManager.GetEntity(entityId, deepLevel));

            if (annotation.Any())
            {
                return annotation;
            }

            annotation.AddRange(await this.feedbackManager.GetEntity(entityId, deepLevel));

            if (annotation.Any())
            {
                return annotation;
            }

            annotation.AddRange(await this.noteManager.GetEntity(entityId, deepLevel));
            return annotation;
        }

        /// <summary>
        ///     Tries to get a <see cref="Annotation" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="Annotation" /> if found</returns>
        public async Task<Annotation> FindEntity(Guid entityId)
        {
            return (Annotation)await this.commentManager.FindEntity(entityId)
                   ?? (Annotation)await this.feedbackManager.FindEntity(entityId)
                   ?? await this.noteManager.FindEntity(entityId);
        }

        /// <summary>
        ///     Tries to get all <see cref="Annotation" /> based on their <see cref="Guid" />
        /// </summary>
        /// <param name="entitiesId">A collection of <see cref="Guid" /></param>
        /// <returns>A collection of <see cref="Annotation" /></returns>
        public async Task<IEnumerable<Annotation>> FindEntities(IEnumerable<Guid> entitiesId)
        {
            var entities = new List<Annotation>();

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
        ///     Creates a new <see cref="Annotation" /> and store it into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="entity">The <see cref="Annotation" /> to create</param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        public async Task<EntityOperationResult<Annotation>> CreateEntity(Annotation entity)
        {
            switch (entity)
            {
                case Comment comment:
                    var commentResult = await this.commentManager.CreateEntity(comment);

                    return commentResult.Succeeded
                        ? EntityOperationResult<Annotation>.Success(commentResult.Entity)
                        : EntityOperationResult<Annotation>.Failed(commentResult.Errors.ToArray());
                case Feedback feedback:
                    var feedbackResult = await this.feedbackManager.CreateEntity(feedback);

                    return feedbackResult.Succeeded
                        ? EntityOperationResult<Annotation>.Success(feedbackResult.Entity)
                        : EntityOperationResult<Annotation>.Failed(feedbackResult.Errors.ToArray());
                case Note note:
                    var noteResult = await this.noteManager.CreateEntity(note);

                    return noteResult.Succeeded
                        ? EntityOperationResult<Annotation>.Success(noteResult.Entity)
                        : EntityOperationResult<Annotation>.Failed(noteResult.Errors.ToArray());
            }

            return EntityOperationResult<Annotation>.Failed("Unknowned Annotation subclass");
        }

        /// <summary>
        ///     Updates a <see cref="Annotation" />
        /// </summary>
        /// <param name="entity">The <see cref="Annotation" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public async Task<EntityOperationResult<Annotation>> UpdateEntity(Annotation entity)
        {
            switch (entity)
            {
                case Comment comment:
                    var commentResult = await this.commentManager.UpdateEntity(comment);

                    return commentResult.Succeeded
                        ? EntityOperationResult<Annotation>.Success(commentResult.Entity)
                        : EntityOperationResult<Annotation>.Failed(commentResult.Errors.ToArray());
                case Feedback feedback:
                    var feedbackResult = await this.feedbackManager.UpdateEntity(feedback);

                    return feedbackResult.Succeeded
                        ? EntityOperationResult<Annotation>.Success(feedbackResult.Entity)
                        : EntityOperationResult<Annotation>.Failed(feedbackResult.Errors.ToArray());
                case Note note:
                    var noteResult = await this.noteManager.UpdateEntity(note);

                    return noteResult.Succeeded
                        ? EntityOperationResult<Annotation>.Success(noteResult.Entity)
                        : EntityOperationResult<Annotation>.Failed(noteResult.Errors.ToArray());
            }

            return EntityOperationResult<Annotation>.Failed("Unknowned Annotation subclass");
        }

        /// <summary>
        ///     Deletes a <see cref="Annotation" />
        /// </summary>
        /// <param name="entity">The <see cref="Annotation" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the deletion</returns>
        public async Task<EntityOperationResult<Annotation>> DeleteEntity(Annotation entity)
        {
            switch (entity)
            {
                case Comment comment:
                    var commentResult = await this.commentManager.DeleteEntity(comment);

                    return commentResult.Succeeded
                        ? EntityOperationResult<Annotation>.Success(commentResult.Entity)
                        : EntityOperationResult<Annotation>.Failed(commentResult.Errors.ToArray());
                case Feedback feedback:
                    var feedbackResult = await this.feedbackManager.DeleteEntity(feedback);

                    return feedbackResult.Succeeded
                        ? EntityOperationResult<Annotation>.Success(feedbackResult.Entity)
                        : EntityOperationResult<Annotation>.Failed(feedbackResult.Errors.ToArray());
                case Note note:
                    var noteResult = await this.noteManager.DeleteEntity(note);

                    return noteResult.Succeeded
                        ? EntityOperationResult<Annotation>.Success(noteResult.Entity)
                        : EntityOperationResult<Annotation>.Failed(noteResult.Errors.ToArray());
            }

            return EntityOperationResult<Annotation>.Failed("Unknowned Annotation subclass");
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="Annotation" />
        /// </summary>
        /// <param name="entity">The <see cref="Annotation" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task ResolveProperties(Annotation entity, EntityDto dto)
        {
            switch (entity)
            {
                case Comment comment:
                    await this.commentManager.ResolveProperties(comment, dto);
                    break;
                case Feedback feedback:
                    await this.feedbackManager.ResolveProperties(feedback, dto);
                    break;
                case Note note:
                    await this.noteManager.ResolveProperties(note, dto);
                    break;
            }
        }

        /// <summary>
        ///     Injects a <see cref="IAnnotatableItemManager" /> to break the circular dependency
        /// </summary>
        /// <param name="manager">The <see cref="IAnnotatableItemManager" /></param>
        public void InjectManager(IAnnotatableItemManager manager)
        {
            this.commentManager.InjectManager(manager);
            this.feedbackManager.InjectManager(manager);
            this.noteManager.InjectManager(manager);
        }

        /// <summary>
        ///     Finds an <see cref="Annotation" /> and includes his <see cref="Entity" /> container
        /// </summary>
        /// <param name="entityId">The <see cref="Entity" /> id</param>
        /// <returns>A <see cref="Task" /> with the <see cref="Annotation" /></returns>
        public async Task<Annotation> FindEntityWithContainer(Guid entityId)
        {
            return (Annotation)await this.commentManager.FindEntityWithContainer(entityId)
                   ?? (Annotation)await this.feedbackManager.FindEntityWithContainer(entityId)
                   ?? await this.noteManager.FindEntityWithContainer(entityId);
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
            var annotations = new List<Entity>();
            annotations.AddRange(await this.commentManager.GetContainedEntities(containerId, deepLevel));
            annotations.AddRange(await this.feedbackManager.GetContainedEntities(containerId, deepLevel));
            annotations.AddRange(await this.noteManager.GetContainedEntities(containerId, deepLevel));
            return annotations.DistinctBy(x => x.Id);
        }

        /// <summary>
        ///     Verifies if the container of the <see cref="Entity" /> has the given <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Entity" /></param>
        /// <param name="containerId">The <see cref="Guid" /> of the container</param>
        /// <returns>A <see cref="Task" /> with the value of the check</returns>
        public async Task<bool> EntityIsContainedBy(Guid entityId, Guid containerId)
        {
            var annotation = await this.FindEntityWithContainer(entityId);
            return annotation != null && annotation.EntityContainer.Id == containerId;
        }
    }
}
