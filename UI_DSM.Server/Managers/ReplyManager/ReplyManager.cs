// --------------------------------------------------------------------------------------------------------
// <copyright file="ReplyManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.ReplyManager
{
    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="Reply" />s
    /// </summary>
    public class ReplyManager : ContainedEntityManager<Reply, Comment>, IReplyManager
    {
        /// <summary>
        ///     The <see cref="IParticipantManager" />
        /// </summary>
        private readonly IParticipantManager participantManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReplyManager" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        public ReplyManager(DatabaseContext context, IParticipantManager participantManager) : base(context)
        {
            this.participantManager = participantManager;
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="Reply" />
        /// </summary>
        /// <param name="entity">The <see cref="Reply" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task ResolveProperties(Reply entity, EntityDto dto)
        {
            if (dto is not ReplyDto replyDto)
            {
                return;
            }

            var relatedEntities = new Dictionary<Guid, Entity>();
            relatedEntities.InsertEntity(await this.participantManager.FindEntity(replyDto.Author));
            entity.ResolveProperties(replyDto, relatedEntities);
        }

        /// <summary>
        ///     Updates a <see cref="Reply" />
        /// </summary>
        /// <param name="entity">The <see cref="Reply" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public override async Task<EntityOperationResult<Reply>> UpdateEntity(Reply entity)
        {
            if (!this.ValidateCurrentEntity(entity, out var entityOperationResult))
            {
                return entityOperationResult;
            }

            var foundEntity = await this.FindEntity(entity.Id);
            entity.CreatedOn = foundEntity.CreatedOn.ToUniversalTime();

            return await this.UpdateEntityIntoContext(entity);
        }

        /// <summary>
        ///     Sets specific properties before the creation of the <see cref="Reply" />
        /// </summary>
        /// <param name="entity">The <see cref="Reply" /></param>
        protected override void SetSpecificPropertiesBeforeCreate(Reply entity)
        {
            entity.CreatedOn = DateTime.UtcNow;
        }
    }
}
