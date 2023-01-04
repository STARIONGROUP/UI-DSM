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

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Managers.AnnotationManager;
    using UI_DSM.Server.Managers.ArtifactManager;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ReviewCategoryManager;
    using UI_DSM.Server.Managers.ReviewManager;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="Project" />s
    /// </summary>
    public class ProjectManager : EntityManager<Project>, IProjectManager
    {
        /// <summary>
        ///     The <see cref="IAnnotationManager" />
        /// </summary>
        private readonly IAnnotationManager annotationManager;

        /// <summary>
        ///     The <see cref="IArtifactManager" />
        /// </summary>
        private readonly IArtifactManager artifactManager;

        /// <summary>
        ///     The <see cref="IParticipantManager" />
        /// </summary>
        private readonly IParticipantManager participantManager;

        /// <summary>
        ///     The <see cref="IReviewCategoryManager" />
        /// </summary>
        private readonly IReviewCategoryManager reviewCategoryManager;

        /// <summary>
        ///     The <see cref="IReviewManager" />
        /// </summary>
        private readonly IReviewManager reviewManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectManager" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        /// <param name="reviewManager">The <see cref="IReviewManager" /></param>
        /// <param name="annotationManager">The <see cref="IAnnotationManager" /></param>
        /// <param name="artifactManager">The <see cref="IArtifactManager" /></param>
        /// <param name="reviewCategoryManager">The <see cref="IReviewCategoryManager" /></param>
        public ProjectManager(DatabaseContext context, IParticipantManager participantManager, IReviewManager reviewManager,
            IAnnotationManager annotationManager, IArtifactManager artifactManager, IReviewCategoryManager reviewCategoryManager) : base(context)
        {
            this.participantManager = participantManager;
            this.reviewManager = reviewManager;
            this.annotationManager = annotationManager;
            this.artifactManager = artifactManager;
            this.reviewCategoryManager = reviewCategoryManager;
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="Project" />
        /// </summary>
        /// <param name="entity">The <see cref="Project" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task ResolveProperties(Project entity, EntityDto dto)
        {
            if (dto is not ProjectDto projectDto)
            {
                return;
            }

            var relatedEntities = new Dictionary<Guid, Entity>();
            relatedEntities.InsertEntityCollection(await this.participantManager.FindEntities(projectDto.Participants));
            relatedEntities.InsertEntityCollection(await this.reviewManager.FindEntities(projectDto.Reviews));
            relatedEntities.InsertEntityCollection(await this.annotationManager.FindEntities(projectDto.Annotations));
            relatedEntities.InsertEntityCollection(await this.artifactManager.FindEntities(projectDto.Artifacts));
            relatedEntities.InsertEntityCollection(await this.reviewCategoryManager.FindEntities(projectDto.ReviewCategories));
            entity.ResolveProperties(projectDto, relatedEntities);
        }

        /// <summary>
        ///     Gets the <see cref="SearchResultDto"/> based on a <see cref="Guid"/>
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <returns>A URL</returns>
        public override async Task<SearchResultDto> GetSearchResult(Guid entityId)
        {
            var project = await this.FindEntity(entityId);

            if (project == null)
            {
                return null;
            }

            return new SearchResultDto()
            {
                ObjectKind = nameof(Project),
                DisplayText = project.ProjectName,
                BaseUrl = $"Project/{project.Id}"
            };
        }

        /// <summary>
        ///     Gets all <see cref="Entity" /> that needs to be unindexed when the current <see cref="Entity" /> is delete
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the entity</param>
        /// <returns>A collection of <see cref="Entity" /></returns>
        public override async Task<IEnumerable<Entity>> GetExtraEntitiesToUnindex(Guid entityId)
        {
            var project = await this.EntityDbSet.Where(x => x.Id == entityId)
                .Include(x => x.Reviews)
                .ThenInclude(x => x.ReviewObjectives)
                .ThenInclude(x => x.ReviewTasks)
                .Include(x => x.Participants)
                .Include(x => x.Annotations)
                .ThenInclude(x => (x as Comment).Replies)
                .Include(x => x.Reviews)
                .ThenInclude(x => x.ReviewItems)
                .FirstOrDefaultAsync();

            if (project == null)
            {
                return Enumerable.Empty<Entity>();
            }

            var entities = new List<Entity>(project.Participants);
            entities.AddRange(project.Reviews);
            entities.AddRange(project.Reviews.SelectMany(x => x.ReviewItems));
            entities.AddRange(project.Reviews.SelectMany(x => x.ReviewObjectives));
            entities.AddRange(project.Reviews.SelectMany(x => x.ReviewObjectives).SelectMany(x => x.ReviewTasks));
            entities.AddRange(project.Annotations);
            entities.AddRange(project.Annotations.OfType<Comment>().SelectMany(x => x.Replies));
            return entities;
        }

        /// <summary>
        ///     Get a collection of <see cref="Project" /> where a <see cref="UserEntity" /> is a <see cref="Participant" />
        /// </summary>
        /// <param name="userName">The name of the <see cref="UserEntity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Project" /></returns>
        public async Task<IEnumerable<Project>> GetAvailableProjectsForUser(string userName)
        {
            var participants = await this.participantManager.GetParticipants(userName);
            return participants.Select(x => x.EntityContainer as Project).OrderBy(x => x!.CreatedOn);
        }

        /// <summary>
        ///     Gets the number of open <see cref="ReviewTask" /> where the logged user is assigned to
        ///     and <see cref="Comment" /> for each <see cref="Project" />
        /// </summary>
        /// <param name="projectsId">A collection of <see cref="Guid" /> for <see cref="Project" />s</param>
        /// <param name="userName">The name of the current logged user</param>
        /// <returns>A <see cref="Task" /> with the <see cref="Dictionary{Guid,ComputedProjectProperties}" /></returns>
        public async Task<Dictionary<Guid, ComputedProjectProperties>> GetOpenTasksAndComments(IEnumerable<Guid> projectsId, string userName)
        {
            var dictionary = new Dictionary<Guid, ComputedProjectProperties>();

            foreach (var projectId in projectsId)
            {
                var computedProperties = await this.GetOpenTasksAndComments(projectId, userName);

                if (computedProperties != null)
                {
                    dictionary[projectId] = computedProperties;
                }
            }

            return dictionary;
        }

        /// <summary>
        ///     Get a collection of <see cref="Project" /> where a <see cref="UserEntity" /> is a <see cref="Participant" />
        ///     and is allowed to manage those projects
        /// </summary>
        /// <param name="userName">The name of the <see cref="UserEntity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Project" /></returns>
        public async Task<IEnumerable<Project>> GetProjectsForManagement(string userName)
        {
            var participants = (await this.participantManager.GetParticipants(userName))
                .Where(x => x.IsAllowedTo(AccessRight.ProjectManagement));

            return participants.Select(x => x.EntityContainer as Project).OrderBy(x => x!.CreatedOn);
        }

        /// <summary>
        ///     Sets specific properties before the creation of the <see cref="Project" />
        /// </summary>
        /// <param name="entity">The <see cref="Project" /></param>
        protected override void SetSpecificPropertiesBeforeCreate(Project entity)
        {
            entity.CreatedOn = DateTime.UtcNow;
        }

        /// <summary>
        ///     Gets the number of open <see cref="ReviewTask" /> where the logged user is assigned to
        ///     and <see cref="Comment" /> for a <see cref="Project" />
        /// </summary>
        /// <param name="projectId">A <see cref="Guid" /> for <see cref="Project" /></param>
        /// <param name="userName">The name of the current logged user</param>
        /// <returns>A <see cref="Task" />with the <see cref="ComputedProjectProperties" /></returns>
        private async Task<ComputedProjectProperties> GetOpenTasksAndComments(Guid projectId, string userName)
        {
            if (this.EntityDbSet.All(x => x.Id != projectId))
            {
                return null;
            }

            var participant = await this.participantManager.GetParticipantForProject(projectId, userName);

            if (participant == null)
            {
                return null;
            }

            var tasks = this.EntityDbSet
                .Where(x => x.Id == projectId)
                .SelectMany(x => x.Reviews)
                .SelectMany(x => x.ReviewObjectives)
                .SelectMany(x => x.ReviewTasks)
                .Include(x => x.IsAssignedTo)
                .AsEnumerable()
                .Count(x => x.Status == StatusKind.Open && x.IsAssignedTo != null && x.IsAssignedTo.Any(p => p.Id == participant.Id));

            var comments = this.EntityDbSet
                .Where(x => x.Id == projectId)
                .SelectMany(x => x.Annotations)
                .OfType<Comment>()
                .Count();

            return new ComputedProjectProperties
            {
                TaskCount = tasks,
                CommentCount = comments
            };
        }
    }
}
