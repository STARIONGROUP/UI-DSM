// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.ReviewObjectiveManager
{
    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Managers.AnnotationManager;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ReviewTaskManager;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="ReviewObjective" />s
    /// </summary>
    public class ReviewObjectiveManager : ContainedEntityManager<ReviewObjective, Review>, IReviewObjectiveManager
    {
        /// <summary>
        ///     The <see cref="IAnnotationManager" />
        /// </summary>
        private readonly IAnnotationManager annotationManager;

        /// <summary>
        ///     The <see cref="IParticipantManager" />
        /// </summary>
        private readonly IParticipantManager participantManager;

        /// <summary>
        ///     The <see cref="IReviewTaskManager" />
        /// </summary>
        private readonly IReviewTaskManager reviewTaskManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewObjectiveManager" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        /// <param name="reviewTaskManager">The <see cref="IReviewTaskManager" /></param>
        /// <param name="annotationManager">The <see cref="IAnnotationManager" /></param>
        public ReviewObjectiveManager(DatabaseContext context, IParticipantManager participantManager, IReviewTaskManager reviewTaskManager,
            IAnnotationManager annotationManager) : base(context)
        {
            this.participantManager = participantManager;
            this.reviewTaskManager = reviewTaskManager;
            this.annotationManager = annotationManager;
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewObjective" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task ResolveProperties(ReviewObjective entity, EntityDto dto)
        {
            if (dto is not ReviewObjectiveDto reviewObjectiveDto)
            {
                return;
            }

            var relatedEntities = new Dictionary<Guid, Entity>();
            relatedEntities.InsertEntity(await this.participantManager.FindEntity(reviewObjectiveDto.Author));
            relatedEntities.InsertEntityCollection(await this.reviewTaskManager.FindEntities(reviewObjectiveDto.ReviewTasks));
            relatedEntities.InsertEntityCollection(await this.annotationManager.FindEntities(reviewObjectiveDto.Annotations));
            entity.ResolveProperties(reviewObjectiveDto, relatedEntities);
        }

        /// <summary>
        ///     Sets specific properties before the creation of the <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewObjective" /></param>
        protected override void SetSpecificPropertiesBeforeCreate(ReviewObjective entity)
        {
            var review = entity.EntityContainer as Review;
            entity.ReviewObjectiveNumber = 0;
            entity.ReviewObjectiveNumber = review!.ReviewObjectives.Max(x => x.ReviewObjectiveNumber) + 1;
            entity.CreatedOn = DateTime.UtcNow;
            entity.Status = StatusKind.Open;
        }
    }
}
