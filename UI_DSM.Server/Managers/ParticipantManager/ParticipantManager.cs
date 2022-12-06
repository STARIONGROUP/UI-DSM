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
    public class ParticipantManager : ContainedEntityManager<Participant, Project>, IParticipantManager
    {
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
        public ParticipantManager(DatabaseContext context, IUserManager userManager, IRoleManager roleManage) : base(context)
        {
            this.userManager = userManager;
            this.roleManager = roleManage;
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="Participant" />
        /// </summary>
        /// <param name="entity">The <see cref="Participant" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="Participant" /> as result</returns>
        public override async Task ResolveProperties(Participant entity, EntityDto dto)
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
                : this.EntityDbSet.Where(x => x.User.Id == userEntity.Id)
                    .BuildIncludeEntityQueryable(0)
                    .Include(x => x.EntityContainer);
        }

        /// <summary>
        ///     Gets all <see cref="UserEntity" /> that are available for the creation of <see cref="Participant" /> inside a certain project
        /// </summary>
        /// <param name="projectId">The <see cref="projectId" /></param>
        /// <returns>A <see cref="Task" /> collection of <see cref="UserEntity" /> as result</returns>
        public async Task<IEnumerable<UserEntity>> GetAvailableUsersForParticipantCreation(Guid projectId)
        {
            var projectParticipant = this.EntityDbSet.Where(x => x.EntityContainer != null && x.EntityContainer.Id == projectId)
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
            var participant = await this.EntityDbSet.Where(x => x.EntityContainer != null
                                                                && x.EntityContainer.Id == projectId && x.User.UserName == userName)
                .BuildIncludeEntityQueryable(0)
                .ToListAsync();

            return participant.FirstOrDefault();
        }

        /// <summary>
        ///     Creates a new <see cref="Participant" /> and store it into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="entity">The <see cref="Participant" /> to create</param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        public override async Task<EntityOperationResult<Participant>> CreateEntity(Participant entity)
        {
            if (!this.ValidateCurrentEntity(entity, out var entityOperationResult))
            {
                return entityOperationResult;
            }

            var project = entity.EntityContainer as Project;
            var existingParticipant = project!.Participants.FirstOrDefault(x => x.Id != Guid.Empty && x.User.Id == entity.User.Id);

            if (existingParticipant != null)
            {
                project.Participants.Remove(entity);
                return EntityOperationResult<Participant>.Failed("This user is already a participant of this project");
            }

            return await this.AddEntityToContext(entity);
        }
    }
}
