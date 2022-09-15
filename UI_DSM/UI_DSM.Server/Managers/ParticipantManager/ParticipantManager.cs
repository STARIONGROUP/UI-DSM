// --------------------------------------------------------------------------------------------------------
// <copyright file="ParticipantManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.ParticipantManager
{
    using Microsoft.EntityFrameworkCore;

    using NLog;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Managers.RoleManager;
    using UI_DSM.Server.Managers.UserManager;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="Participant" />s
    /// </summary>
    public class ParticipantManager : IParticipantManager
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
        ///     The <see cref="IRoleManager" />
        /// </summary>
        private readonly IRoleManager roleManager;

        /// <summary>
        ///     The <see cref="IUserManager" />
        /// </summary>
        private readonly IUserManager userManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ParticipantManager" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        /// <param name="userManager">The <see cref="IUserManager" /></param>
        /// <param name="roleManage">The <see cref="IRoleManager" /></param>
        public ParticipantManager(DatabaseContext context, IUserManager userManager, IRoleManager roleManage)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManage;
        }

        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />s and associated <see cref="Entity" />
        /// </summary>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> as result</returns>
        public async Task<IEnumerable<Entity>> GetEntities(int deepLevel = 0)
        {
            var participants = await this.context.Participants.ToListAsync();
            return participants.SelectMany(x => x.GetAssociatedEntities(deepLevel)).DistinctBy(x => x.Id);
        }

        /// <summary>
        ///     Tries to get a <see cref="Entity" /> based on its <see cref="Guid" /> and its associated <see cref="Entity" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> if found</returns>
        public async Task<IEnumerable<Entity>> GetEntity(Guid entityId, int deepLevel = 0)
        {
            var participant = await this.FindEntity(entityId);

            return participant == null ? Enumerable.Empty<Entity>() : participant.GetAssociatedEntities(deepLevel);
        }

        /// <summary>
        ///     Tries to get a <see cref="Participant" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="Participant" /> if found</returns>
        public async Task<Participant> FindEntity(Guid entityId)
        {
            return await this.context.Participants.FindAsync(entityId);
        }

        /// <summary>
        ///     Tries to get all <see cref="Participant" /> based on their <see cref="Guid" />
        /// </summary>
        /// <param name="entitiesId">A collection of <see cref="Guid" /></param>
        /// <returns>A collection of <see cref="Participant" /></returns>
        public async Task<IEnumerable<Participant>> FindEntities(IEnumerable<Guid> entitiesId)
        {
            var entities = new List<Participant>();

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
        ///     Creates a new <see cref="Participant" /> and store it into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="entity">The <see cref="Participant" /> to create</param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        public async Task<EntityOperationResult<Participant>> CreateEntity(Participant entity)
        {
            var validations = this.context.ValidateModel(entity);

            if (validations.Any())
            {
                return EntityOperationResult<Participant>.Failed(validations.ToArray());
            }

            if (!await this.VerifyContainer(entity))
            {
                return EntityOperationResult<Participant>.Failed($"Unable to compute the container the current entity with the id {entity.Id}");
            }

            var project = entity.EntityContainer as Project;
            var existingParticipant = project!.Participants.FirstOrDefault(x => x.Id != Guid.Empty && x.User.Id == entity.User.Id);
             
            if (existingParticipant != null)
            {
                project.Participants.Remove(entity);
                return EntityOperationResult<Participant>.Failed("This user is already a participant of this project");
            }

            var operationResult = new EntityOperationResult<Participant>(this.context.Add(entity), EntityState.Added);

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
        ///     Updates a <see cref="Participant" />
        /// </summary>
        /// <param name="entity">The <see cref="Participant" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public async Task<EntityOperationResult<Participant>> UpdateEntity(Participant entity)
        {
            var validations = this.context.ValidateModel(entity);

            if (validations.Any())
            {
                return EntityOperationResult<Participant>.Failed(validations.ToArray());
            }

            var operationResult = new EntityOperationResult<Participant>(this.context.Update(entity), EntityState.Modified, EntityState.Unchanged);

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
        ///     Deletes a <see cref="Participant" />
        /// </summary>
        /// <param name="entity">The <see cref="Participant" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the deletion</returns>
        public async Task<EntityOperationResult<Participant>> DeleteEntity(Participant entity)
        {
            if (!await this.VerifyContainer(entity))
            {
                return EntityOperationResult<Participant>.Failed($"Unable to compute the container the current entity with the id {entity.Id}");
            }

            var operationResult = new EntityOperationResult<Participant>(this.context.Remove(entity), EntityState.Deleted, EntityState.Detached);

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
        ///     Resolve all properties for the <see cref="Participant" />
        /// </summary>
        /// <param name="entity">The <see cref="Participant" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="Participant" /> as result</returns>
        public async Task ResolveProperties(Participant entity, EntityDto dto)
        {
            if (dto is not ParticipantDto participantDto)
            {
                return;
            }

            var relatedEntities = new Dictionary<Guid, Entity>();
            relatedEntities.InsertEntity(await this.roleManager.FindEntity(participantDto.Role));
            relatedEntities.InsertEntity(await this.userManager.FindEntity(participantDto.User));
            entity.ResolveProperties(participantDto, relatedEntities);
        }

        /// <summary>
        ///     Finds an <see cref="Participant" /> and includes his <see cref="Entity" /> container
        /// </summary>
        /// <param name="entityId">The <see cref="Entity" /> id</param>
        /// <returns>A <see cref="Task" /> with the <see cref="Participant" /></returns>
        public async Task<Participant> FindEntityWithContainer(Guid entityId)
        {
            return await this.context.Participants.Where(x => x.Id == entityId)
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
            var participants = await this.context.Participants.Where(x => x.EntityContainer != null
                                                                          && x.EntityContainer.Id == containerId).ToListAsync();

            return participants.SelectMany(x => x.GetAssociatedEntities(deepLevel)).DistinctBy(x => x.Id);
        }

        /// <summary>
        ///     Verifies if the container of the <see cref="Entity" /> has the given <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Entity" /></param>
        /// <param name="containerId">The <see cref="Guid" /> of the container</param>
        /// <returns>A <see cref="Task" /> with the value of the check</returns>
        public async Task<bool> EntityIsContainedBy(Guid entityId, Guid containerId)
        {
            var participant = await this.FindEntityWithContainer(entityId);
            return participant != null && participant.EntityContainer.Id == containerId;
        }

        /// <summary>
        ///     Gets all <see cref="Participant" /> where the <see cref="Participant.User" /> is the provided
        ///     <see cref="UserEntity" />
        /// </summary>
        /// <param name="userName">The name of the <see cref="UserEntity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Participant" /></returns>
        public async Task<IEnumerable<Participant>> GetParticipants(string userName)
        {
            var userEntity = await this.userManager.GetUserByName(userName);

            return userEntity == null
                ? Enumerable.Empty<Participant>() 
                : this.context.Participants.Where(x => x.User.Id == userEntity.Id)
                    .Include(x => x.EntityContainer);
        }

        /// <summary>
        ///     Gets all <see cref="UserEntity" /> that are available for the creation of <see cref="Participant" /> inside a certain project
        /// </summary>
        /// <param name="projectId">The <see cref="projectId" /></param>
        /// <returns>A <see cref="Task" /> collection of <see cref="UserEntity" /> as result</returns>
        public async Task<IEnumerable<UserEntity>> GetAvailableUsersForParticipantCreation(Guid projectId)
        {
            var projectParticipant = this.context.Participants.Where(x => x.EntityContainer != null && x.EntityContainer.Id == projectId)
                .Select(x => x.User.Id).ToList();

            var users = await this.userManager.GetUsers(x => projectParticipant.All(userId => userId != x.Id));
            return users;
        }

        /// <summary>
        ///     Gets a <see cref="Participant" /> by his name and his <see cref="Project" /> where he is involed
        /// </summary>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="userName">The name of the <see cref="Participant" /></param>
        /// <returns>A <see cref="Task" />with the <see cref="Participant" /> if found</returns>
        public async Task<Participant> GetParticipantForProject(Guid projectId, string userName)
        {
            var participant = await this.context.Participants.Where(x => x.EntityContainer != null && x.EntityContainer.Id == projectId && x.User.UserName == userName)
                .ToListAsync();

            return participant.FirstOrDefault();
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
