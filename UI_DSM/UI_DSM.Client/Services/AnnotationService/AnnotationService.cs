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

    using UI_DSM.Client.Services.JsonDeserializerProvider;
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
        /// <param name="deserializer">The <see cref="IJsonDeserializerService" /></param>
        public AnnotationService(HttpClient httpClient, IJsonDeserializerService deserializer) : base(httpClient, deserializer)
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
        /// <param name="annotation">The <see cref="Annotation" />to update</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{Annotation}" /></returns>
        public async Task<EntityRequestResponse<Annotation>> UpdateAnnotation(Annotation annotation)
        {
            this.VerifyEntityAndContainer<Project>(annotation);

            try
            {
                this.ComputeMainRoute(annotation.EntityContainer.Id);
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
        /// <param name="annotation">The <see cref="Annotation" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /></returns>
        public async Task<RequestResponseDto> DeleteAnnotation(Annotation annotation)
        {
            this.VerifyEntityAndContainer<Project>(annotation);

            try
            {
                this.ComputeMainRoute(annotation.EntityContainer.Id);
                return await this.DeleteEntity(annotation);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }
    }
}
