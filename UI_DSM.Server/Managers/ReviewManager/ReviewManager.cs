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
    using Microsoft.EntityFrameworkCore;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Managers.ArtifactManager;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ReviewItemManager;
    using UI_DSM.Server.Managers.ReviewObjectiveManager;
    using UI_DSM.Shared.DTO.Common;
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
        ///     The <see cref="IReviewItemManager" />
        /// </summary>
        private readonly IReviewItemManager reviewItemManager;

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
        /// <param name="reviewItemManager">The <see cref="IReviewItemManager" /></param>
        public ReviewManager(DatabaseContext context, IParticipantManager participantManager, IReviewObjectiveManager reviewObjectiveManager,
            IArtifactManager artifactManager, IReviewItemManager reviewItemManager) : base(context)
        {
            this.participantManager = participantManager;
            this.reviewObjectiveManager = reviewObjectiveManager;
            this.artifactManager = artifactManager;
            this.reviewItemManager = reviewItemManager;
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
            relatedEntities.InsertEntityCollection(await this.reviewObjectiveManager.FindEntities(reviewDto.ReviewObjectives));
            relatedEntities.InsertEntityCollection(await this.artifactManager.FindEntities(reviewDto.Artifacts));
            relatedEntities.InsertEntityCollection(await this.reviewItemManager.FindEntities(reviewDto.ReviewItems));
            entity.ResolveProperties(reviewDto, relatedEntities);
        }

        /// <summary>
        ///     Gets the number of open <see cref="ReviewTask" /> where the logged user is assigned to
        ///     and <see cref="Comment" /> for each <see cref="Project" />
        /// </summary>
        /// <param name="reviewsId">A collection of <see cref="Guid" /> for <see cref="Review" />s</param>
        /// <param name="projectId">A <see cref="Guid" /> of <see cref="Project" />s</param>
        /// <param name="userName">The name of the current logged user</param>
        /// <returns> A <see cref="Dictionary{Guid,ComputedProjectProperties}" /></returns>
        public async Task<Dictionary<Guid, AdditionalComputedProperties>> GetOpenTasksAndComments(IEnumerable<Guid> reviewsId, Guid projectId, string userName)
        {
            var dictionary = new Dictionary<Guid, AdditionalComputedProperties>();

            foreach (var reviewId in reviewsId)
            {
                var computedProperties = await this.GetOpenTasksAndComments(reviewId, projectId, userName);

                if (computedProperties != null)
                {
                    dictionary[reviewId] = computedProperties;
                }
            }

            return dictionary;
        }

        /// <summary>
        ///     Gets the <see cref="SearchResultDto"/> based on a <see cref="Guid"/>
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <returns>A URL</returns>
        public override async Task<SearchResultDto> GetSearchResult(Guid entityId)
        {
            var review = await this.EntityDbSet.Where(x => x.Id == entityId)
                .Include(x => x.EntityContainer).FirstOrDefaultAsync();

            if (review == null)
            {
                return null;
            }

            var route = $"Project/{review.EntityContainer.Id}/Review/{review.Id}";

            return new SearchResultDto()
            {
                BaseUrl = route, 
                DisplayText = review.Title,
                ObjectKind = nameof(Review),
                Location = ((Project)review.EntityContainer).ProjectName
            };
        }

        /// <summary>
        ///     Gets all <see cref="Entity" /> that needs to be unindexed when the current <see cref="Entity" /> is delete
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the entity</param>
        /// <returns>A collection of <see cref="Entity" /></returns>
        public override async Task<IEnumerable<Entity>> GetExtraEntitiesToUnindex(Guid entityId)
        {
            var review = await this.EntityDbSet.Where(x => x.Id == entityId)
                .Include(x => x.ReviewItems)
                .ThenInclude(x => x.Annotations)
                .Include(x => x.ReviewObjectives)
                .ThenInclude(x => x.ReviewTasks)
                .Include(x => x.ReviewObjectives)
                .ThenInclude(x => x.Annotations)
                .ThenInclude(x => (x as Comment).Replies)
                .Include(x => x.ReviewItems)
                .ThenInclude(x => x.Annotations)
                .ThenInclude(x => (x as Comment).Replies)
                .FirstOrDefaultAsync();

            if (review == null)
            {
                return Enumerable.Empty<Entity>();
            }

            var entities = new List<Entity>(review.ReviewItems);
            entities.AddRange(review.ReviewItems.SelectMany(x => x.Annotations));
            entities.AddRange(review.ReviewItems.SelectMany(x => x.Annotations.OfType<Comment>()).SelectMany(x => x.Replies));
            entities.AddRange(review.ReviewObjectives);
            entities.AddRange(review.ReviewObjectives.SelectMany(x => x.Annotations));
            entities.AddRange(review.ReviewObjectives.SelectMany(x => x.Annotations.OfType<Comment>()).SelectMany(x => x.Replies));
            entities.AddRange(review.ReviewObjectives.SelectMany(x => x.ReviewTasks));
            return entities;
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

        /// <summary>
        ///     Gets the number of open <see cref="ReviewTask" /> where the logged user is assigned to
        ///     and <see cref="Comment" /> for a <see cref="Review" />
        /// </summary>
        /// <param name="reviewId">A <see cref="Guid" /> for <see cref="Review" /></param>
        /// <param name="projectId">A <see cref="Guid" /> of <see cref="Project" />s</param>
        /// <param name="userName">The name of the current logged user</param>
        /// <returns> A <see cref="AdditionalComputedProperties" /></returns>
        private async Task<AdditionalComputedProperties> GetOpenTasksAndComments(Guid reviewId, Guid projectId, string userName)
        {
            if (this.EntityDbSet.All(x => x.Id != reviewId))
            {
                return null;
            }

            var participant = await this.participantManager.GetParticipantForProject(projectId, userName);

            if (participant == null)
            {
                return null;
            }

            var tasks = this.EntityDbSet
                .Where(x => x.Id == reviewId)
                .SelectMany(x => x.ReviewObjectives)
                .SelectMany(x => x.ReviewTasks)
                .Include(x => x.IsAssignedTo)
                .AsEnumerable()
                .Count(x => x.Status == StatusKind.Open && x.IsAssignedTo != null && x.IsAssignedTo.Any(p => p.Id == participant.Id));

            var comments = this.EntityDbSet
                .Where(x => x.Id == reviewId)
                .SelectMany(x => x.ReviewItems)
                .SelectMany(x => x.Annotations)
                .OfType<Comment>()
                .ToList()
                .DistinctBy(x => x.Id)
                .ToList();

            return new AdditionalComputedProperties
            {
                TaskCount = tasks,
                OpenCommentCount = comments.Count(x => x.Status == StatusKind.Open),
                TotalCommentCount = comments.Count
            };
        }
    }
}
