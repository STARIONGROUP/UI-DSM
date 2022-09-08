// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectManager.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Server.Managers.ProjectManager
{
    using Microsoft.EntityFrameworkCore;

    using NLog;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ReviewManager;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="Project" />s
    /// </summary>
    public class ProjectManager : IProjectManager
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
        ///     The <see cref="IReviewManager" />
        /// </summary>
        private readonly IReviewManager reviewManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectManager" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        /// <param name="reviewManager">The <see cref="IReviewManager"/></param>
        public ProjectManager(DatabaseContext context, IParticipantManager participantManager, IReviewManager reviewManager)
        {
            this.context = context;
            this.participantManager = participantManager;
            this.reviewManager = reviewManager;
        }

        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />s and associated <see cref="Entity" />
        /// </summary>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> as result</returns>
        public async Task<IEnumerable<Entity>> GetEntities(int deepLevel = 0)
        {
            var projects = await this.context.Projects.ToListAsync();
            return projects.SelectMany(x => x.GetAssociatedEntities(deepLevel));
        }

        /// <summary>
        ///     Tries to get all <see cref="Project" /> based on their <see cref="Guid" />
        /// </summary>
        /// <param name="entitiesId">A collection of <see cref="Guid" /></param>
        /// <returns>A collection of <see cref="Project" /></returns>
        public async Task<IEnumerable<Project>> FindEntities(IEnumerable<Guid> entitiesId)
        {
            var entities = new List<Project>();

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
        ///     Creates a new <see cref="Project" /> and store it into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="entity">The <see cref="Project" /> to create</param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        public async Task<EntityOperationResult<Project>> CreateEntity(Project entity)
        {
            var operationResult = new EntityOperationResult<Project>(this.context.Add(entity), EntityState.Added);

            try
            {
                await this.context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                if (ExceptionHelper.IsUniqueConstraintViolation(exception))
                {
                    operationResult.HandleExpection($"The name {entity.ProjectName} is already used");
                }
                else
                {
                    operationResult.HandleExpection(exception);
                    this.logger.Error(exception.Message);
                }
            }

            return operationResult;
        }

        /// <summary>
        ///     Updates a <see cref="Project" />
        /// </summary>
        /// <param name="entity">The <see cref="Project" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public async Task<EntityOperationResult<Project>> UpdateEntity(Project entity)
        {
            var operationResult = new EntityOperationResult<Project>(this.context.Update(entity), EntityState.Modified, EntityState.Unchanged);

            try
            {
                await this.context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                if (ExceptionHelper.IsUniqueConstraintViolation(exception))
                {
                    operationResult.HandleExpection($"The name {entity.ProjectName} is already used");
                }
                else
                {
                    operationResult.HandleExpection(exception);
                    this.logger.Error(exception.Message);
                }
            }

            return operationResult;
        }

        /// <summary>
        ///     Deletes a <see cref="Project" />
        /// </summary>
        /// <param name="entity">The <see cref="Project" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the deletion</returns>
        public async Task<EntityOperationResult<Project>> DeleteEntity(Project entity)
        {
            var operationResult = new EntityOperationResult<Project>(this.context.Remove(entity), EntityState.Deleted, EntityState.Detached);

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
        ///     Tries to get a <see cref="Entity" /> based on its <see cref="Guid" /> and its associated <see cref="Entity" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" />  if found</returns>
        public async Task<IEnumerable<Entity>> GetEntity(Guid entityId, int deepLevel = 0)
        {
            var project = await this.FindEntity(entityId);

            if (project == null)
            {
                return Enumerable.Empty<Entity>();
            }

            return project.GetAssociatedEntities(deepLevel);
        }

        /// <summary>
        ///     Tries to get a <see cref="Project" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="Project" /> if found</returns>
        public async Task<Project> FindEntity(Guid entityId)
        {
            return await this.context.Projects.FindAsync(entityId);
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="Project" />
        /// </summary>
        /// <param name="entity">The <see cref="Project" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task ResolveProperties(Project entity, EntityDto dto)
        {
            if (dto is not ProjectDto projectDto)
            {
                return;
            }

            var relatedEntities = new Dictionary<Guid, Entity>();
            relatedEntities.InsertEntityCollection(await this.participantManager.FindEntities(projectDto.Participants));
            relatedEntities.InsertEntityCollection(await this.reviewManager.FindEntities(projectDto.Reviews));

            entity.ResolveProperties(projectDto, relatedEntities);
        }

        /// <summary>
        ///     Get a collection of <see cref="Project" /> where a <see cref="UserEntity" /> is a <see cref="Participant" />
        /// </summary>
        /// <param name="userName">The name of the <see cref="UserEntity"/></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Project" /></returns>
        public async Task<IEnumerable<Project>> GetAvailableProjectsForUser(string userName)
        {
            var participants = await this.participantManager.GetParticipants(userName);
            return participants.Select(x => x.EntityContainer as Project);
        }
    }
}
