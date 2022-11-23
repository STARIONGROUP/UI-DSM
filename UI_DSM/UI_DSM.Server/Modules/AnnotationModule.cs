// --------------------------------------------------------------------------------------------------------
// <copyright file="AnnotationModule.cs" company="RHEA System S.A.">
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
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Managers.AnnotatableItemManager;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="AnnotationModule" />  is a
    ///     <see cref="ContainedEntityModule{TEntity,TEntityDto, TEntityContainer}" /> to manage
    ///     <see cref="Annotation" /> related to a <see cref="Project" />
    /// </summary>
    [Route("api/Project/{projectId:guid}/Annotation")]
    public class AnnotationModule : ContainedEntityModule<Annotation, AnnotationDto, Project>
    {
        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task GetEntities(IEntityManager<Annotation> manager, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, this.ContainerRouteKey);

            if (participant == null)
            {
                return;
            }

            await base.GetEntities(manager, context, deepLevel);
        }

        /// <summary>
        ///     Get a <see cref="AnnotationDto" /> based on its <see cref="Guid" /> with all associated entities
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task GetEntity(IEntityManager<Annotation> manager, Guid entityId, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, this.ContainerRouteKey);

            if (participant == null)
            {
                return;
            }

            await base.GetEntity(manager, entityId, context, deepLevel);
        }

        /// <summary>
        ///     Tries to create a new <see cref="Annotation" /> based on its <see cref="AnnotationDto" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="dto">The <see cref="AnnotationDto" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task CreateEntity(IEntityManager<Annotation> manager, AnnotationDto dto, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, this.ContainerRouteKey);

            if (participant == null)
            {
                return;
            }

            if (!(await IsAllowedTo(context, participant, AccessRight.ReviewTask)))
            {
                return;
            }

            _ = context.RequestServices.GetService<IAnnotatableItemManager>();
            dto.Author = participant.Id;
            await base.CreateEntity(manager, dto, context, deepLevel);
        }

        /// <summary>
        ///     Tries to delete an <see cref="Annotation" /> defined by the given <see cref="Guid" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Annotation" /> to delete</param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /> as result</returns>
        [Authorize]
        public override async Task<RequestResponseDto> DeleteEntity(IEntityManager<Annotation> manager, Guid entityId, HttpContext context)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, this.ContainerRouteKey);

            if (participant == null)
            {
                return new RequestResponseDto();
            }

            var annotation = (await manager.GetEntity(entityId)).OfType<Annotation>().FirstOrDefault();

            if (annotation?.Author.Id != participant.Id)
            {
                context.Response.StatusCode = 403;

                return new RequestResponseDto()
                {
                   Errors = new List<string>{"Unable to delete a Comment from someelse"}
                };
            }

            return await base.DeleteEntity(manager, entityId, context);
        }

        /// <summary>
        ///     Tries to update an existing <see cref="Annotation" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Annotation" /></param>
        /// <param name="dto">The <see cref="AnnotationDto" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" />as result</returns>
        [Authorize]
        public override async Task UpdateEntity(IEntityManager<Annotation> manager, Guid entityId, AnnotationDto dto, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, this.ContainerRouteKey);

            if (participant == null)
            {
                return;
            }

            _ = context.RequestServices.GetService<IAnnotatableItemManager>();
            var annotation = (await manager.GetEntity(entityId)).OfType<Annotation>().FirstOrDefault();

            if (annotation?.Author.Id != participant.Id && annotation?.Content != dto.Content)
            {
                context.Response.StatusCode = 403;
                return;
            }

            await base.UpdateEntity(manager, entityId, dto, context, deepLevel);
        }

        /// <summary>
        ///     Adds the <see cref="Annotation" /> into the corresponding collection of the <see cref="Project" />
        /// </summary>
        /// <param name="entity">The <see cref="Annotation" /></param>
        /// <param name="container">The <see cref="Project" /></param>
        protected override void AddEntityIntoContainerCollection(Annotation entity, Project container)
        {
            container.Annotations.Add(entity);
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
