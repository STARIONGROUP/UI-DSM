// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveModule.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Modules
{
    using System.Diagnostics.CodeAnalysis;

    using Carter.Response;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Managers.ReviewManager;
    using UI_DSM.Server.Managers.ReviewObjectiveManager;
    using UI_DSM.Server.Managers.ReviewTaskManager;
    using UI_DSM.Server.Services.SearchService;
    using UI_DSM.Shared.Assembler;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="ReviewObjectiveModule" /> is a
    ///     <see cref="ContainedEntityModule{TEntity,TEntityDto, TEntityContainer}" /> to manage
    ///     <see cref="ReviewObjective" /> related to a <see cref="Review" />
    /// </summary>
    [Route("api/Project/{projectId:guid}/Review/{reviewId:guid}/ReviewObjective")]
    public class ReviewObjectiveModule : ContainedEntityModule<ReviewObjective, ReviewObjectiveDto, Review>
    {
        /// <summary>
        ///     The route key for the ProjectId
        /// </summary>
        private const string ProjectKey = "projectId";

        /// <summary>
        ///     A collection of <see cref="ReviewObjective" /> to use for the creation of <see cref="ReviewObjective" /> and
        ///     <see cref="ReviewTask" />
        /// </summary>
        private readonly List<ReviewObjective> reviewObjectivesTemplates;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewObjectiveModule" /> class.
        /// </summary>
        /// <param name="jsonService">The <see cref="IJsonService" /></param>
        public ReviewObjectiveModule(IJsonService jsonService)
        {
            var dtos = jsonService.Deserialize<List<EntityDto>>(File.OpenRead(Path.Combine("Data", "ReviewObjectives.json")));
            this.reviewObjectivesTemplates = Assembler.CreateEntities<ReviewObjective>(dtos).ToList();
        }

        /// <summary>
        ///     Adds routes to the <see cref="IEndpointRouteBuilder" />
        /// </summary>
        /// <param name="app">The <see cref="IEndpointRouteBuilder" /></param>
        [ExcludeFromCodeCoverage]
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            base.AddRoutes(app);

            app.MapPost($"{this.MainRoute}/CreateTemplate", this.CreateEntityTemplate)
                .Accepts<ReviewObjectiveCreationDto>("application/json")
                .Produces<EntityRequestResponseDto>(201)
                .Produces<EntityRequestResponseDto>(422)
                .Produces<EntityRequestResponseDto>(500)
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/CreateTemplate");

            app.MapGet($"{this.MainRoute}/GetAvailableTemplates", this.GetAvailableTemplates)
                .Produces<List<ReviewObjectiveCreationDto>>()
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/GetAvailableTemplates");

            app.MapPost($"{this.MainRoute}/CreateTemplates", this.CreateEntityTemplates)
                .Accepts<List<ReviewObjectiveCreationDto>>("application/json")
                .Produces<EntityRequestResponseDto>(201)
                .Produces<EntityRequestResponseDto>(422)
                .Produces<EntityRequestResponseDto>(500)
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/CreateTemplates");

            app.MapGet($"{this.MainRoute}/OpenTasksAndComments", this.GetOpenTasksAndComments)
                .Produces<Dictionary<Guid, ComputedProjectProperties>>()
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/GetOpenTasksAndComments");
        }

        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task GetEntities(IEntityManager<ReviewObjective> manager, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }

            await base.GetEntities(manager, context, deepLevel);
        }

        /// <summary>
        ///     Get a <see cref="ReviewObjectiveDto" /> based on its <see cref="Guid" /> with all associated entities
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task GetEntity(IEntityManager<ReviewObjective> manager, Guid entityId, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }

            await base.GetEntity(manager, entityId, context, deepLevel);
        }

        /// <summary>
        ///     Tries to create a new <see cref="ReviewObjective" /> based on its <see cref="ReviewObjectiveDto" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="dto">The <see cref="ReviewObjectiveDto" /></param>
        /// <param name="searchService">The <see cref="ISearchService"/></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override Task CreateEntity(IEntityManager<ReviewObjective> manager, ReviewObjectiveDto dto, ISearchService searchService, HttpContext context, int deepLevel = 0)
        {
            context.Response.StatusCode = 405;
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Tries to create a new <see cref="ReviewObjective" /> based on the <see cref="ReviewObjectiveKind" />
        /// </summary>
        /// <param name="reviewManager">The <see cref="IReviewManager" /></param>
        /// <param name="manager">The <see cref="IReviewObjectiveManager" /></param>
        /// <param name="reviewTaskManager">The <see cref="IReviewTaskManager" /></param>
        /// <param name="searchService">The <see cref="ISearchService"/></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="dto">The <see cref="ReviewObjectiveCreationDto" /></param>
        /// <param name="deepLevel">The optional deepLevel</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task CreateEntityTemplate(IReviewManager reviewManager, IReviewObjectiveManager manager, IReviewTaskManager reviewTaskManager, 
            ISearchService searchService, HttpContext context,
            ReviewObjectiveCreationDto dto, int deepLevel = 0)
        {
            var template = this.reviewObjectivesTemplates.SingleOrDefault(x => x.ReviewObjectiveKind == dto.Kind
                                                                               && x.ReviewObjectiveKindNumber == dto.KindNumber);

            if (template == null)
            {
                context.Response.StatusCode = 400;
                return;
            }

            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }

            if (!await IsAllowedTo(context, participant, AccessRight.CreateReviewObjective))
            {
                return;
            }

            var requestResponse = new EntityRequestResponseDto();

            if (!await this.AdditionalRouteValidation(context))
            {
                requestResponse.Errors = new List<string>
                {
                    "Route validation failure"
                };

                await context.Response.Negotiate(requestResponse);
                return;
            }

            var container = (await reviewManager.GetEntity(this.GetAdditionalRouteId(context.Request, this.ContainerRouteKey)))
                .FirstOrDefault(x => x is Review);

            if (container is not Review review)
            {
                context.Response.StatusCode = 400;

                requestResponse.Errors = new List<string>
                {
                    "Unable to find the given container"
                };

                await context.Response.Negotiate(requestResponse);
                return;
            }

            var identityResult = await manager.CreateEntityBasedOnTemplate(template, review);
            this.HandleOperationResult(requestResponse, context.Response, identityResult, 201, deepLevel);
            await context.Response.Negotiate(requestResponse);

            if (identityResult.Succeeded)
            {
                var searchTask = identityResult.Entity.ReviewTasks.Select(x => (ReviewTaskDto)x.ToDto());
                await searchService.IndexData((ReviewObjectiveDto)identityResult.Entity.ToDto());
                await searchService.IndexData(searchTask);
            }
        }

        /// <summary>
        ///     Tries to create a new <see cref="ReviewObjective" /> based on the <see cref="ReviewObjectiveKind" />
        /// </summary>
        /// <param name="reviewManager">The <see cref="IReviewManager" /></param>
        /// <param name="manager">The <see cref="IReviewObjectiveManager" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="searchService">The <see cref="ISearchService"/></param>
        /// <param name="dtos">The <see cref="List{ReviewObjectiveCreationDto}" /></param>
        /// <param name="deepLevel">The optional deepLevel</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task CreateEntityTemplates(IReviewManager reviewManager, IReviewObjectiveManager manager, HttpContext context, ISearchService searchService,
            List<ReviewObjectiveCreationDto> dtos, int deepLevel = 0)
        {
            var templates = this.reviewObjectivesTemplates.Where(x => dtos.Any(y => y.Kind == x.ReviewObjectiveKind
                                                                                    && y.KindNumber == x.ReviewObjectiveKindNumber));

            if (templates == null || !templates.Any())
            {
                context.Response.StatusCode = 400;
                return;
            }

            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }

            if (!await IsAllowedTo(context, participant, AccessRight.CreateReviewObjective))
            {
                return;
            }

            var requestResponse = new EntityRequestResponseDto();

            if (!await this.AdditionalRouteValidation(context))
            {
                requestResponse.Errors = new List<string>
                {
                    "Route validation failure"
                };

                await context.Response.Negotiate(requestResponse);
                return;
            }

            var container = (await reviewManager.GetEntity(this.GetAdditionalRouteId(context.Request, this.ContainerRouteKey)))
                .FirstOrDefault(x => x is Review);

            if (container is not Review review)
            {
                context.Response.StatusCode = 400;

                requestResponse.Errors = new List<string>
                {
                    "Unable to find the given container"
                };

                await context.Response.Negotiate(requestResponse);
                return;
            }

            var identityResults = await manager.CreateEntityBasedOnTemplates(templates, review);
            this.HandleOperationResult(requestResponse, context.Response, identityResults, 201, deepLevel);
            await context.Response.Negotiate(requestResponse);

            if (identityResults.Succeeded)
            {
                var searchObjective = identityResults.Entities.Select(x => (ReviewObjectiveDto)x.ToDto());
                var searchTask = identityResults.Entities.SelectMany(x => x.ReviewTasks).Select(x => (ReviewTaskDto)x.ToDto());

                await searchService.IndexData(searchObjective);
                await searchService.IndexData(searchTask);
            }
        }

        /// <summary>
        ///     Tries to delete an <see cref="ReviewObjective" /> defined by the given <see cref="Guid" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="ReviewObjective" /> to delete</param>
        /// <param name="searchService">The <see cref="ISearchService"/></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /> as result</returns>
        [Authorize]
        public override async Task<RequestResponseDto> DeleteEntity(IEntityManager<ReviewObjective> manager, Guid entityId, ISearchService searchService, HttpContext context)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return new RequestResponseDto();
            }

            if (!participant.IsAllowedTo(AccessRight.DeleteReviewObjective))
            {
                context.Response.StatusCode = 403;

                return new RequestResponseDto
                {
                    Errors = new List<string> { "You don't have requested access right" }
                };
            }

            return await base.DeleteEntity(manager, entityId, searchService, context);
        }

        /// <summary>
        ///     Tries to update an existing <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="ReviewObjective" /></param>
        /// <param name="dto">The <see cref="ReviewObjectiveDto" /></param>
        /// <param name="searchService">The <see cref="ISearchService"/></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" />as result</returns>
        [Authorize]
        public override async Task UpdateEntity(IEntityManager<ReviewObjective> manager, Guid entityId, ReviewObjectiveDto dto, ISearchService searchService,
            HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }

            await base.UpdateEntity(manager, entityId, dto, searchService, context, deepLevel);
        }

        /// <summary>
        ///     Gets, all <see cref="ReviewObjectiveDto" />s from the json file
        ///     and filters them based on the given <see cref="ReviewObjectiveKind" />
        /// </summary>
        /// <param name="reviewObjectiveManager">The <see cref="IReviewObjectiveManager" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task GetAvailableTemplates(IReviewObjectiveManager reviewObjectiveManager, HttpContext context)
        {
            var reviewId = this.GetAdditionalRouteId(context.Request, this.ContainerRouteKey);
            var existingReviewObjectives = reviewObjectiveManager.GetReviewObjectiveCreationForReview(reviewId);
            var reviewObjectiveCreationTemplates = this.reviewObjectivesTemplates.Select(x => new ReviewObjectiveCreationDto { Kind = x.ReviewObjectiveKind, KindNumber = x.ReviewObjectiveKindNumber }).ToList();
            reviewObjectiveCreationTemplates.RemoveAll(x => existingReviewObjectives.Any(y => y.Kind == x.Kind && y.KindNumber == x.KindNumber));
            await context.Response.Negotiate(reviewObjectiveCreationTemplates);
        }

        /// <summary>
        ///     Adds the <see cref="ReviewObjective" /> into the corresponding collection of the <see cref="Review" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewObjective" /></param>
        /// <param name="container">The <see cref="Review" /></param>
        protected override void AddEntityIntoContainerCollection(ReviewObjective entity, Review container)
        {
            container.ReviewObjectives.Add(entity);
        }

        /// <summary>
        ///     Verifies if the provided routes is correctly formatted with all containment
        /// </summary>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the result of the check</returns>
        protected override async Task<bool> AdditionalRouteValidation(HttpContext context)
        {
            var reviewManager = context.RequestServices.GetService<IContainedEntityManager<Review>>();

            if (reviewManager == null)
            {
                context.Response.StatusCode = 500;
                return false;
            }

            var reviewId = this.GetAdditionalRouteId(context.Request, this.ContainerRouteKey);
            var projectId = this.GetAdditionalRouteId(context.Request, ProjectKey);

            if (!await reviewManager.EntityIsContainedBy(reviewId, projectId))
            {
                context.Response.StatusCode = 400;
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Gets, for all <see cref="Review" />, the number of open <see cref="ReviewTask" /> and <see cref="Comment" />
        ///     related to the <see cref="Review" />
        /// </summary>
        /// <param name="reviewObjectiveManager">The <see cref="IReviewManager"/></param>
        /// <param name="reviewId"> The <see cref="Guid"/> of the <see cref="Review" /></param>
        /// <param name="projectId"> The <see cref="Guid"/> of the <see cref="Project" /></param>
        /// <param name="context">The <see cref="HttpContext"/></param>
        /// <returns>A <see cref="Task"/></returns>
        [Authorize]
        public async Task GetOpenTasksAndComments(IReviewObjectiveManager reviewObjectiveManager, Guid reviewId, Guid projectId, HttpContext context)
        {
            var reviewObjectivesId = (await reviewObjectiveManager.GetContainedEntities(reviewId)).Select(x => x.Id);

            var computedProperties = await reviewObjectiveManager.GetOpenTasksAndComments(reviewObjectivesId, projectId, context.User.Identity?.Name);
            await context.Response.Negotiate(computedProperties);
        }
    }
}
