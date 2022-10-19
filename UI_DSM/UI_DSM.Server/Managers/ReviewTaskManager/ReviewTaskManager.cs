// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewTaskManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.ReviewTaskManager
{
    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="ReviewTask" />s
    /// </summary>
    public class ReviewTaskManager : ContainedEntityManager<ReviewTask, ReviewObjective>, IReviewTaskManager
    {
        /// <summary>
        ///     The <see cref="IParticipantManager" />
        /// </summary>
        private readonly IParticipantManager participantManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewTaskManager" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        public ReviewTaskManager(DatabaseContext context, IParticipantManager participantManager) : base(context)
        {
            this.participantManager = participantManager;
        }

        /// <summary>
        ///     Updates a <see cref="ReviewTask" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewTask" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public override async Task<EntityOperationResult<ReviewTask>> UpdateEntity(ReviewTask entity)
        {
            if (!this.ValidateCurrentEntity(entity, out var entityOperationResult))
            {
                return entityOperationResult;
            }

            var foundEntity = await this.FindEntity(entity.Id);
            entity.AdditionalView = foundEntity.AdditionalView;
            entity.MainView = foundEntity.MainView;
            entity.OptionalView = foundEntity.OptionalView;
            entity.HasPrimaryView = foundEntity.HasPrimaryView;

            return await this.UpdateEntityIntoContext(entity);
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="ReviewTask" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewTask" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task ResolveProperties(ReviewTask entity, EntityDto dto)
        {
            if (dto is not ReviewTaskDto reviewTaskDto)
            {
                return;
            }

            var relatedEntities = new Dictionary<Guid, Entity>();
            relatedEntities.InsertEntity(await this.participantManager.FindEntity(reviewTaskDto.Author));
            relatedEntities.InsertEntity(await this.participantManager.FindEntity(reviewTaskDto.IsAssignedTo));
            entity.ResolveProperties(reviewTaskDto, relatedEntities);
        }

        /// <summary>
        ///     Create <see cref="ReviewTask" />s based on templates
        /// </summary>
        /// <param name="container">The <see cref="ReviewObjective" /> container</param>
        /// <param name="templateReviewTasks">A collection of <see cref="ReviewTask" /></param>
        /// <param name="author">The <see cref="Participant" /> author</param>
        public void CreateEntitiesBasedOnTemplate(ReviewObjective container, List<ReviewTask> templateReviewTasks, Participant author)
        {
            foreach (var templateReviewTask in templateReviewTasks)
            {
                var reviewTask = new ReviewTask(templateReviewTask)
                {
                    Author = author
                };

                this.SetSpecificPropertiesBeforeCreate(reviewTask);

                container.ReviewTasks.Add(reviewTask);
                this.Context.Add(reviewTask);
            }
        }

        /// <summary>
        ///     Sets specific properties before the creation of the <see cref="ReviewTask" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewTask" /></param>
        protected override void SetSpecificPropertiesBeforeCreate(ReviewTask entity)
        {
            entity.CreatedOn = DateTime.UtcNow;
            entity.Status = StatusKind.Open;
        }
    }
}
