// --------------------------------------------------------------------------------------------------------
// <copyright file="ArtifactModule.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Modules
{
    using Carter.Response;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Services.FileService;
    using UI_DSM.Server.Services.SearchService;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="ArtifactModule" />  is a
    ///     <see cref="ContainedEntityModule{TEntity,TEntityDto,TEntityContainer}" /> to manage
    ///     <see cref="Artifact" /> related to a <see cref="Project" />
    /// </summary>
    [Route("api/Project/{projectId:guid}/Artifact")]
    public class ArtifactModule : ContainedEntityModule<Artifact, ArtifactDto, Project>
    {
        /// <summary>
        ///     The <see cref="IFileService" />
        /// </summary>
        private readonly IFileService fileService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ArtifactModule" /> class.
        /// </summary>
        /// <param name="fileService">The <see cref="IFileService" /></param>
        public ArtifactModule(IFileService fileService)
        {
            this.fileService = fileService;
        }

        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override Task GetEntities(IEntityManager<Artifact> manager, HttpContext context, int deepLevel = 0)
        {
            return base.GetEntities(manager, context, deepLevel);
        }

        /// <summary>
        ///     Get a <see cref="ArtifactDto" /> based on its <see cref="Guid" /> with all associated entities
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override Task GetEntity(IEntityManager<Artifact> manager, Guid entityId, HttpContext context, int deepLevel = 0)
        {
            return base.GetEntity(manager, entityId, context, deepLevel);
        }

        /// <summary>
        ///     Tries to create a new <see cref="Artifact" /> based on its <see cref="ArtifactDto" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="dto">The <see cref="ArtifactDto" /></param>
        /// <param name="searchService">The <see cref="ISearchService" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task CreateEntity(IEntityManager<Artifact> manager, ArtifactDto dto, ISearchService searchService, HttpContext context, int deepLevel = 0)
        {
            var requestResponse = new EntityRequestResponseDto();

            if (!this.fileService.TempFileExists(dto.FileName))
            {
                context.Response.StatusCode = 404;

                requestResponse.Errors = new List<string>
                {
                    "Invalid artifact fileName"
                };

                await context.Response.Negotiate(requestResponse);
                return;
            }

            var projectId = this.GetAdditionalRouteId(context.Request, this.ContainerRouteKey);

            if (this.fileService.Exists(projectId.ToString(), dto.FileName))
            {
                context.Response.StatusCode = 400;

                requestResponse.Errors = new List<string>
                {
                    "The artifact already exists !"
                };

                this.fileService.DeleteTemporaryFile(dto.FileName);
                await context.Response.Negotiate(requestResponse);
                return;
            }

            if (dto is ModelDto && !await this.fileService.IsAnnexC3File(dto.FileName))
            {
                context.Response.StatusCode = 400;

                requestResponse.Errors = new List<string>
                {
                    "The artifact is not a valid Annex C3 file"
                };

                this.fileService.DeleteTemporaryFile(dto.FileName);
                await context.Response.Negotiate(requestResponse);
                return;
            }

            if (dto is ModelDto modelDto)
            {
                modelDto.IterationId = Guid.Parse(dto.FileName.Split('.')[0]);
            }

            dto.FileName = this.fileService.MoveFile(dto.FileName, projectId.ToString());
            await base.CreateEntity(manager, dto, searchService, context, deepLevel);
        }

        /// <summary>
        ///     Tries to delete an <see cref="Artifact" /> defined by the given <see cref="Guid" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Artifact" /> to delete</param>
        /// <param name="searchService">The <see cref="ISearchService" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /> as result</returns>
        [Authorize]
        public override async Task<RequestResponseDto> DeleteEntity(IEntityManager<Artifact> manager, Guid entityId, ISearchService searchService, HttpContext context)
        {
            context.Response.StatusCode = 403;
            await Task.CompletedTask;
            return new RequestResponseDto { Errors = { "Forbidden to delete an artifact" } };
        }

        /// <summary>
        ///     Tries to update an existing <see cref="Artifact" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Artifact" /></param>
        /// <param name="dto">The <see cref="ArtifactDto" /></param>
        /// <param name="searchService">The <see cref="ISearchService" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" />as result</returns>
        [Authorize]
        public override Task UpdateEntity(IEntityManager<Artifact> manager, Guid entityId, ArtifactDto dto, ISearchService searchService, HttpContext context, int deepLevel = 0)
        {
            context.Response.StatusCode = 403;
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Adds the <see cref="Artifact" /> into the corresponding collection of the <see cref="Project" />
        /// </summary>
        /// <param name="entity">The <see cref="Artifact" /></param>
        /// <param name="container">The <see cref="Project" /></param>
        protected override void AddEntityIntoContainerCollection(Artifact entity, Project container)
        {
            container.Artifacts.Add(entity);
        }

        /// <summary>
        ///     Verifies if the provided routes is correctly formatted with all containment
        /// </summary>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the result of the check</returns>
        protected override async Task<bool> AdditionalRouteValidation(HttpContext context)
        {
            await Task.CompletedTask;
            return true;
        }
    }
}
