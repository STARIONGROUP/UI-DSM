// --------------------------------------------------------------------------------------------------------
// <copyright file="ResolverService.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Server.Services.ResolverService
{
    using GP.SearchService.SDK.Definitions;

    using UI_DSM.Client.Extensions;
    using UI_DSM.Server.Managers.CommentManager;
    using UI_DSM.Server.Managers.FeedbackManager;
    using UI_DSM.Server.Managers.ModelManager;
    using UI_DSM.Server.Managers.NoteManager;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ProjectManager;
    using UI_DSM.Server.Managers.ReplyManager;
    using UI_DSM.Server.Managers.ReviewCategoryManager;
    using UI_DSM.Server.Managers.ReviewManager;
    using UI_DSM.Server.Managers.ReviewObjectiveManager;
    using UI_DSM.Server.Managers.ReviewTaskManager;
    using UI_DSM.Server.Managers.RoleManager;
    using UI_DSM.Server.Managers.ThingManager;
    using UI_DSM.Server.Managers.UserManager;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This service enables to filter and translate <see cref="CommonBaseSearchDto" /> to <see cref="SearchResultDto" />
    /// </summary>
    public class ResolverService : IResolverService
    {
        /// <summary>
        ///     The <see cref="ICommentManager" />
        /// </summary>
        private readonly ICommentManager commentManager;

        /// <summary>
        ///     The <see cref="IFeedbackManager" />
        /// </summary>
        private readonly IFeedbackManager feedbackManager;

        /// <summary>
        ///     The <see cref="IModelManager" />
        /// </summary>
        private readonly IModelManager modelManager;

        /// <summary>
        ///     The <see cref="INoteManager" />
        /// </summary>
        private readonly INoteManager noteManager;

        /// <summary>
        ///     The <see cref="IParticipantManager" />
        /// </summary>
        private readonly IParticipantManager participantManager;

        /// <summary>
        ///     The <see cref="IProjectManager" />
        /// </summary>
        private readonly IProjectManager projectManager;

        /// <summary>
        ///     The <see cref="IReplyManager" />
        /// </summary>
        private readonly IReplyManager replyManager;

        /// <summary>
        ///     The <see cref="IReviewCategoryManager" />
        /// </summary>
        private readonly IReviewCategoryManager reviewCategoryManager;

        /// <summary>
        ///     The <see cref="IReviewManager" />
        /// </summary>
        private readonly IReviewManager reviewManager;

        /// <summary>
        ///     The <see cref="IReviewObjectiveManager" />
        /// </summary>
        private readonly IReviewObjectiveManager reviewObjectiveManager;

        /// <summary>
        ///     The <see cref="IReviewTaskManager" />
        /// </summary>
        private readonly IReviewTaskManager reviewTaskManager;

        /// <summary>
        ///     The <see cref="IRoleManager" />
        /// </summary>
        private readonly IRoleManager roleManager;

        /// <summary>
        ///     The <see cref="IThingManager" />
        /// </summary>
        private readonly IThingManager thingManager;

        /// <summary>
        ///     The <see cref="IUserManager" />
        /// </summary>
        private readonly IUserManager userManager;

        /// <summary>
        ///     Initializes a new <see cref="ResolverService" />
        /// </summary>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        /// <param name="projectManager">The <see cref="IProjectManager" /></param>
        /// <param name="reviewManager">The <see cref="IReviewManager" /></param>
        /// <param name="reviewObjectiveManager">The <see cref="IReviewObjectiveManager" /></param>
        /// <param name="reviewTaskManager">The <see cref="IReviewTaskManager" /></param>
        /// <param name="commentManager">The <see cref="ICommentManager" /></param>
        /// <param name="replyManager">The <see cref="IReplyManager" /></param>
        /// <param name="thingManager">The <see cref="IThingManager" /></param>
        /// <param name="roleManager">The <see cref="IRoleManager" /></param>
        /// <param name="userManager">The <see cref="IUserManager" /></param>
        /// <param name="modelManager">The <see cref="IModelManager" /></param>
        /// <param name="reviewCategoryManager">The <see cref="IReviewCategoryManager" /></param>
        /// <param name="noteManager">The <see cref="INoteManager" /></param>
        /// <param name="feedbackManager">The <see cref="IFeedbackManager" /></param>
        public ResolverService(IParticipantManager participantManager, IProjectManager projectManager, IReviewManager reviewManager,
            IReviewObjectiveManager reviewObjectiveManager, IReviewTaskManager reviewTaskManager,
            ICommentManager commentManager, IReplyManager replyManager, IThingManager thingManager, IRoleManager roleManager, IUserManager userManager,
            IModelManager modelManager, IReviewCategoryManager reviewCategoryManager, INoteManager noteManager, IFeedbackManager feedbackManager)
        {
            this.participantManager = participantManager;
            this.projectManager = projectManager;
            this.reviewManager = reviewManager;
            this.reviewObjectiveManager = reviewObjectiveManager;
            this.reviewTaskManager = reviewTaskManager;
            this.commentManager = commentManager;
            this.replyManager = replyManager;
            this.thingManager = thingManager;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.modelManager = modelManager;
            this.reviewCategoryManager = reviewCategoryManager;
            this.noteManager = noteManager;
            this.feedbackManager = feedbackManager;
        }

        /// <summary>
        ///     Resolves the collection of <see cref="CommonBaseSearchDto" /> into a collection of <see cref="SearchResultDto" />
        /// </summary>
        /// <param name="dtos">The collection of <see cref="SearchResultDto" /></param>
        /// <param name="userName">The name of the current user</param>
        /// <returns>A collection of <see cref="SearchResultDto" /></returns>
        public async Task<List<SearchResultDto>> ResolveSearchResult(List<CommonBaseSearchDto> dtos, string userName)
        {
            var results = new List<SearchResultDto>();

            var projects = (await this.projectManager.GetAvailableProjectsForUser(userName)).ToList();
            var managedProjects = (await this.projectManager.GetProjectsForManagement(userName)).ToList();
            var loggedUser = await this.userManager.GetUserByName(userName);
            var models = new List<Model>();

            foreach (var project in projects)
            {
                models.AddRange((await this.modelManager.GetContainedEntities(project.Id)).OfType<Model>());
            }

            models = models.DistinctBy(x => x.IterationId).ToList();

            foreach (var commonBaseSearchDto in dtos.Where(commonBaseSearchDto => commonBaseSearchDto.Type.EndsWith("Dto")))
            {
                var result = await this.ResolveSearchResult(commonBaseSearchDto, projects, managedProjects, loggedUser);

                if (result != null)
                {
                    results.Add(result);
                }
            }

            foreach (var commonBaseSearchDto in dtos.Where(commonBaseSearchDto => !commonBaseSearchDto.Type.EndsWith("Dto")))
            {
                results.AddRange(await this.ResolveSearchResult(commonBaseSearchDto, models));
            }

            return results;
        }

        /// <summary>
        ///     Resolve a 10-25 <see cref="ISearchDto" />
        /// </summary>
        /// <param name="commonBaseSearchDto">The <see cref="CommonBaseSearchDto" /></param>
        /// <param name="models">A collection of <see cref="Model" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="SearchResultDto" /></returns>
        private async Task<List<SearchResultDto>> ResolveSearchResult(CommonBaseSearchDto commonBaseSearchDto, List<Model> models)
        {
            var results = new List<SearchResultDto>();

            foreach (var model in models)
            {
                if (await this.thingManager.GetThing(commonBaseSearchDto, model) is { } thing)
                {
                    results.Add(new SearchResultDto
                    {
                        BaseUrl = (await this.modelManager.GetSearchResult(model.Id)).BaseUrl,
                        SpecificCategory = thing.GetSpecificCategoryForThing(),
                        ObjectKind = thing.GetType().Name,
                        DisplayText = thing.GetSpecificNameForThing()
                    });
                }
            }

            return results;
        }

        /// <summary>
        ///     Resolve a UI-DSM <see cref="ISearchDto" />
        /// </summary>
        /// <param name="commonBaseSearchDto">The <see cref="ISearchDto" /></param>
        /// <param name="projects">All available <see cref="Project" /> for the current user</param>
        /// <param name="managedProjects">All available <see cref="Project" /> where the user can manage</param>
        /// <param name="loggedUser">The <see cref="UserEntity" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="SearchResultDto" /></returns>
        private async Task<SearchResultDto> ResolveSearchResult(ISearchDto commonBaseSearchDto,
            IEnumerable<Project> projects, IEnumerable<Project> managedProjects, UserEntity loggedUser)
        {
            switch (commonBaseSearchDto.Type.Split(".").Last())
            {
                case nameof(ProjectDto):
                    if (loggedUser.IsAdmin)
                    {
                        return await this.projectManager.GetSearchResult(commonBaseSearchDto.Id);
                    }

                    if (projects.Any(x => x.Id == commonBaseSearchDto.Id))
                    {
                        return await this.projectManager.GetSearchResult(commonBaseSearchDto.Id);
                    }

                    return null;

                case nameof(RoleDto):
                    if (loggedUser.IsAdmin)
                    {
                        return await this.roleManager.GetSearchResult(commonBaseSearchDto.Id);
                    }

                    return null;

                case nameof(UserEntityDto):
                    if (loggedUser.IsAdmin)
                    {
                        return await this.userManager.GetSearchResult(commonBaseSearchDto.Id);
                    }

                    return null;

                case nameof(ParticipantDto):
                    if (loggedUser.IsAdmin)
                    {
                        return await this.participantManager.GetSearchResult(commonBaseSearchDto.Id);
                    }

                    var searchedParticipant = await this.participantManager.GetSearchResult(commonBaseSearchDto.Id);
                    return IsContainedIntoProject(managedProjects, searchedParticipant) ? searchedParticipant : null;

                case nameof(ReviewCategoryDto):
                    if (loggedUser.IsAdmin)
                    {
                        return await this.reviewCategoryManager.GetSearchResult(commonBaseSearchDto.Id);
                    }

                    return null;

                case nameof(ModelDto):
                    var searchedModel = await this.modelManager.GetSearchResult(commonBaseSearchDto.Id);
                    return IsContainedIntoProject(projects, searchedModel) ? searchedModel : null;

                case nameof(ReviewDto):
                    var searchedReview = await this.reviewManager.GetSearchResult(commonBaseSearchDto.Id);
                    return IsContainedIntoProject(projects, searchedReview) ? searchedReview : null;

                case nameof(ReviewObjectiveDto):
                    var searchedReviewObjective = await this.reviewObjectiveManager.GetSearchResult(commonBaseSearchDto.Id);
                    return IsContainedIntoProject(projects, searchedReviewObjective) ? searchedReviewObjective : null;

                case nameof(ReviewTaskDto):
                    var searchedReviewTask = await this.reviewTaskManager.GetSearchResult(commonBaseSearchDto.Id);
                    return IsContainedIntoProject(projects, searchedReviewTask) ? searchedReviewTask : null;

                case nameof(CommentDto):
                    var searchedComment = await this.commentManager.GetSearchResult(commonBaseSearchDto.Id);
                    return IsContainedIntoProject(projects, searchedComment) ? searchedComment : null;

                case nameof(NoteDto):
                    var searchedNote = await this.noteManager.GetSearchResult(commonBaseSearchDto.Id);
                    return IsContainedIntoProject(projects, searchedNote) ? searchedNote : null;

                case nameof(FeedbackDto):
                    var searchedFeedback = await this.feedbackManager.GetSearchResult(commonBaseSearchDto.Id);
                    return IsContainedIntoProject(projects, searchedFeedback) ? searchedFeedback : null;

                case nameof(ReplyDto):
                    var searchedReply = await this.replyManager.GetSearchResult(commonBaseSearchDto.Id);
                    return IsContainedIntoProject(projects, searchedReply) ? searchedReply : null;

                default:
                    return null;
            }
        }

        /// <summary>
        ///     Verifies that a <see cref="SearchResultDto" /> is contained into a <see cref="Project" />
        /// </summary>
        /// <param name="projects">A collection of possible <see cref="Project" /></param>
        /// <param name="searchResult">The <see cref="SearchResultDto" /></param>
        /// <returns>True if contained into one of a <see cref="Project" /></returns>
        private static bool IsContainedIntoProject(IEnumerable<Project> projects, SearchResultDto searchResult)
        {
            if (searchResult == null)
            {
                return false;
            }

            var projectId = Guid.Parse(searchResult.BaseUrl.Split("/")[1]);
            return projects.Any(x => x.Id == projectId);
        }
    }
}
