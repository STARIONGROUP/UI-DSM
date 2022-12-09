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
    using UI_DSM.Server.Managers.ReviewObjectiveManager;
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
        ///     The <see cref="IReviewObjectiveManager" />
        /// </summary>
        private IReviewObjectiveManager reviewObjectiveManager;

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
            var selectedParticipants = entity.IsAssignedTo.ToList();
            if (!this.ValidateCurrentEntity(entity, out var entityOperationResult))
            {
                return entityOperationResult;
            }
            var reviewTask = (await this.GetEntity(entity.Id)).OfType<ReviewTask>().First();
            reviewTask.Status = entity.Status;
            var deletedParticipants = reviewTask.IsAssignedTo.Where(x => selectedParticipants.All(p => p.Id != x.Id));
            var addedParticipants = selectedParticipants.Where(x => reviewTask.IsAssignedTo.All(p => p.Id != x.Id));
            foreach (var participant in deletedParticipants.ToList())
            {
                reviewTask.IsAssignedTo.Remove(participant);
            }
            foreach (var addedParticipant in addedParticipants.ToList())
            {
                reviewTask.IsAssignedTo.Add(addedParticipant);
            }
            entity.CreatedOn = entity.CreatedOn.ToUniversalTime();
            var operation = await this.UpdateEntityIntoContext(reviewTask);
            if (this.reviewObjectiveManager != null)
            {
                var foundEntity = await this.FindEntityWithContainer(entity.Id);
                await this.reviewObjectiveManager.UpdateStatus(foundEntity.EntityContainer.Id);
            }
            return operation;
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
            relatedEntities.InsertEntityCollection(await this.participantManager.FindEntities(reviewTaskDto.IsAssignedTo));
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
        ///     Injects the <see cref="IReviewObjectiveManager" /> to avoid circular dependency
        /// </summary>
        /// <param name="manager">The <see cref="IReviewObjectiveManager" /></param>
        public void InjectManager(IReviewObjectiveManager manager)
        {
            this.reviewObjectiveManager = manager;
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
