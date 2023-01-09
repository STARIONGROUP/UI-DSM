// --------------------------------------------------------------------------------------------------------
// <copyright file="IAnnotationManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.AnnotationManager
{
    using UI_DSM.Server.Managers.AnnotatableItemManager;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="AnnotationManager" />
    /// </summary>
    public interface IAnnotationManager : IContainedEntityManager<Annotation>
    {
        /// <summary>
        ///     Injects a <see cref="IAnnotatableItemManager" /> to break the circular dependency
        /// </summary>
        /// <param name="manager">The <see cref="IAnnotatableItemManager" /></param>
        void InjectManager(IAnnotatableItemManager manager);

        /// <summary>
        ///     Gets all <see cref="Annotation"/> linked to a <see cref="AnnotatableItem" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="annotatableItemId">The <see cref="Guid" /> of the <see cref="AnnotatableItem" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /></returns>
        Task<IEnumerable<Entity>> GetAnnotationsOfAnnotatableItem(Guid projectId, Guid annotatableItemId);

        /// <summary>
        ///     Gets all <see cref="Annotation" /> that are linked to a <see cref="ReviewTask" />
        /// </summary>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewTaskId">The <see cref="ReviewTask" /> id</param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /></returns>
        Task<IEnumerable<Entity>> GetAnnotationsForReviewTask(Guid projectId, Guid reviewTaskId);
    }
}
