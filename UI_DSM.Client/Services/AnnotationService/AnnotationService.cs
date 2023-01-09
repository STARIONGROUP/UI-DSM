// --------------------------------------------------------------------------------------------------------
// <copyright file="AnnotationService.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to annotation an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Services.AnnotationService
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Shared.Assembler;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     The <see cref="AnnotationService" /> provide capability to manage <see cref="Annotation" />s inside a
    ///     <see cref="Project" />
    /// </summary>
    [Route("Project/{0}/Annotation")]
    public class AnnotationService : EntityServiceBase<Annotation, AnnotationDto>, IAnnotationService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AnnotationService" /> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="ServiceBase.HttpClient" /></param>
        /// <param name="jsonService">The <see cref="IJsonService" /></param>
        public AnnotationService(HttpClient httpClient, IJsonService jsonService) : base(httpClient, jsonService)
        {
        }

        /// <summary>
        ///     Gets all <see cref="Annotation" />s contained inside a <see cref="Project" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="Annotation" /></returns>
        public async Task<List<Annotation>> GetAnnotationsOfProject(Guid projectId, int deepLevel = 0)
        {
            try
            {
                this.ComputeMainRoute(projectId);
                return await this.GetEntities(deepLevel);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Gets a <see cref="Annotation" /> contained inside a <see cref="Project" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="annotationId">The <see cref="Guid" /> of the <see cref="Annotation" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="Annotation" /></returns>
        public async Task<Annotation> GetAnnotation(Guid projectId, Guid annotationId, int deepLevel = 0)
        {
            try
            {
                this.ComputeMainRoute(projectId);
                return await this.GetEntity(annotationId, deepLevel);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Creates a new <see cref="Annotation" /> inside a <see cref="Project" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="annotation">The <see cref="Annotation" />to create</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{Annotation}" /></returns>
        public async Task<EntityRequestResponse<Annotation>> CreateAnnotation(Guid projectId, Annotation annotation)
        {
            try
            {
                this.ComputeMainRoute(projectId);
                return await this.CreateEntity(annotation, 0);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Updates a <see cref="Annotation" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="annotation">The <see cref="Annotation" />to update</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{Annotation}" /></returns>
        public async Task<EntityRequestResponse<Annotation>> UpdateAnnotation(Guid projectId, Annotation annotation)
        {
            try
            {
                this.ComputeMainRoute(projectId);
                return await this.UpdateEntity(annotation, 0);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Deletes a <see cref="Annotation" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="annotation">The <see cref="Annotation" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /></returns>
        public async Task<RequestResponseDto> DeleteAnnotation(Guid projectId, Annotation annotation)
        {
            try
            {
                this.ComputeMainRoute(projectId);
                return await this.DeleteEntity(annotation);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Gets a collection of <see cref="Annotation" /> that are linked to a <see cref="AnnotatableItem" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="annotatableItemId">The <see cref="Guid" /> of the <see cref="AnnotatableItem" /></param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="Annotation" /></returns>
        public async Task<List<Annotation>> GetAnnotationsOfAnnotatableItem(Guid projectId, Guid annotatableItemId)
        {
            try
            {
                this.ComputeMainRoute(projectId);
                var response = await this.HttpClient.GetAsync($"{this.MainRoute}/AnnotatableItem/{annotatableItemId}");

                if (!response.IsSuccessStatusCode)
                {
                    return new List<Annotation>();
                }

                var dtos= this.jsonService.Deserialize<IEnumerable<EntityDto>>(await response.Content.ReadAsStreamAsync()).ToList();
                return Assembler.CreateEntities<Annotation>(dtos).ToList();
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Gets all <see cref="Annotation" /> that are linked to a <see cref="ReviewTask" />
        /// </summary>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewTaskId">The <see cref="ReviewTask" /> id</param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /></returns>
        public async Task<List<Annotation>> GetAnnotationsForReviewTask(Guid projectId, Guid reviewTaskId)
        {
            try
            {
                this.ComputeMainRoute(projectId);
                var response = await this.HttpClient.GetAsync($"{this.MainRoute}/ReviewTask/{reviewTaskId}");

                if (!response.IsSuccessStatusCode)
                {
                    return new List<Annotation>();
                }

                var dtos = this.jsonService.Deserialize<IEnumerable<EntityDto>>(await response.Content.ReadAsStreamAsync()).ToList();
                return Assembler.CreateEntities<Annotation>(dtos).ToList();
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }
    }
}
