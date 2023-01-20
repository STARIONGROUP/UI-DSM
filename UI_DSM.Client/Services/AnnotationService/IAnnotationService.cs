// --------------------------------------------------------------------------------------------------------
// <copyright file="IAnnotationService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.AnnotationService
{
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     Interface definition for <see cref="AnnotationService" />
    /// </summary>
    public interface IAnnotationService
    {
        /// <summary>
        ///     Gets all <see cref="Annotation" />s contained inside a <see cref="Project" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="Annotation" /></returns>
        Task<List<Annotation>> GetAnnotationsOfProject(Guid projectId, int deepLevel = 0);

        /// <summary>
        ///     Gets a <see cref="Annotation" /> contained inside a <see cref="Project" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="annotationId">The <see cref="Guid" /> of the <see cref="Annotation" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="Annotation" /></returns>
        Task<Annotation> GetAnnotation(Guid projectId, Guid annotationId, int deepLevel = 0);

        /// <summary>
        ///     Creates a new <see cref="Annotation" /> inside a <see cref="Project" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="annotation">The <see cref="Annotation" />to create</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{Annotation}" /></returns>
        Task<EntityRequestResponse<Annotation>> CreateAnnotation(Guid projectId, Annotation annotation);

        /// <summary>
        ///     Updates a <see cref="Annotation" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="annotation">The <see cref="Annotation" />to update</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{Annotation}" /></returns>
        Task<EntityRequestResponse<Annotation>> UpdateAnnotation(Guid projectId, Annotation annotation);

        /// <summary>
        ///     Deletes a <see cref="Annotation" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="annotation">The <see cref="Annotation" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /></returns>
        Task<RequestResponseDto> DeleteAnnotation(Guid projectId, Annotation annotation);

        /// <summary>
        ///     Gets a collection of <see cref="Annotation" /> that are linked to a <see cref="AnnotatableItem" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="annotatableItemId">The <see cref="Guid" /> of the <see cref="AnnotatableItem" /></param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="Annotation" /></returns>
        Task<List<Annotation>> GetAnnotationsOfAnnotatableItem(Guid projectId, Guid annotatableItemId);

        /// <summary>
        ///     Gets all <see cref="Annotation" /> that are linked to a <see cref="ReviewTask" />
        /// </summary>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewTaskId">The <see cref="ReviewTask" /> id</param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /></returns>
        Task<List<Annotation>> GetAnnotationsForReviewTask(Guid projectId, Guid reviewTaskId);
    }
}
