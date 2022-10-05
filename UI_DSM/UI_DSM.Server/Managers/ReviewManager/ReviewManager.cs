// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.ReviewManager
{
    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Managers.ArtifactManager;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ReviewObjectiveManager;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="Review" />s
    /// </summary>
    public class ReviewManager : ContainedEntityManager<Review, Project>, IReviewManager
    {
        /// <summary>
        ///     The <see cref="IArtifactManager" />
        /// </summary>
        private readonly IArtifactManager artifactManager;

        /// <summary>
        ///     The <see cref="IParticipantManager" />
        /// </summary>
        private readonly IParticipantManager participantManager;

        /// <summary>
        ///     The <see cref="IReviewObjectiveManager" />
        /// </summary>
        private readonly IReviewObjectiveManager reviewObjectiveManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewManager" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        /// <param name="reviewObjectiveManager">The <see cref="IReviewObjectiveManager" /></param>
        /// <param name="artifactManager">The <see cref="IArtifactManager" /></param>
        public ReviewManager(DatabaseContext context, IParticipantManager participantManager, IReviewObjectiveManager reviewObjectiveManager,
            IArtifactManager artifactManager) : base(context)
        {
            this.participantManager = participantManager;
            this.reviewObjectiveManager = reviewObjectiveManager;
            this.artifactManager = artifactManager;
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="Review" />
        /// </summary>
        /// <param name="entity">The <see cref="Review" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task ResolveProperties(Review entity, EntityDto dto)
        {
            if (dto is not ReviewDto reviewDto)
            {
                return;
            }

            var relatedEntities = new Dictionary<Guid, Entity>();
            relatedEntities.InsertEntity(await this.participantManager.FindEntity(reviewDto.Author));
            relatedEntities.InsertEntityCollection(await this.reviewObjectiveManager.FindEntities(reviewDto.ReviewObjectives));
            relatedEntities.InsertEntityCollection(await this.artifactManager.FindEntities(reviewDto.Artifacts));
            entity.ResolveProperties(reviewDto, relatedEntities);
        }

        /// <summary>
        ///     Sets specific properties before the creation of the <see cref="Review" />
        /// </summary>
        /// <param name="entity">The <see cref="Review" /></param>
        protected override void SetSpecificPropertiesBeforeCreate(Review entity)
        {
            var project = entity.EntityContainer as Project;
            entity.ReviewNumber = 0;
            entity.ReviewNumber = project!.Reviews.Max(x => x.ReviewNumber) + 1;
            entity.CreatedOn = DateTime.UtcNow;
            entity.Status = StatusKind.Open;
        }
    }
}
