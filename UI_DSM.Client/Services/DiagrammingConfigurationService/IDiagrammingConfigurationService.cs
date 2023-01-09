// --------------------------------------------------------------------------------------------------------
// <copyright file="IDiagrammingConfigurationService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.DiagrammingConfigurationService
{
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="DiagrammingConfigurationService" />
    /// </summary>
    public interface IDiagrammingConfigurationService
    {
        /// <summary>
        ///     Saves <see cref="ReviewTask" /> diagram configuration
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewTaskId">The <see cref="Entity.Id" /> of the <see cref="ReviewTask" /></param>
        /// <param name="configurationName">The name of the configuration</param>
        /// <param name="diagramLayoutInformation">The <see cref="IEnumerable{DiagramNode}" />to create</param>
        /// <returns>A <see cref="Task" /></returns>
        Task<(bool result, List<string> errors)> SaveDiagramLayout(Guid projectId, Guid reviewTaskId, string configurationName, IEnumerable<DiagramLayoutInformationDto> diagramLayoutInformation);

        /// <summary>
        ///     Loads diagram configurations name
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewTaskId">The <see cref="Entity.Id" /> of the <see cref="ReviewTask" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="List{T}" /></returns>
        Task<List<string>> LoadDiagramLayoutConfigurationNames(Guid projectId, Guid reviewTaskId);

        /// <summary>
        ///     Loads <see cref="ReviewTask" /> diagram configuration
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewTaskId">The <see cref="Entity.Id" /> of the <see cref="ReviewTask" /></param>
        /// <param name="configurationName">The name of the selected configuration</param>
        /// <returns>A <see cref="Task" /> with the <see cref="List{DiagramLayoutInformationDto}" /></returns>
        Task<List<DiagramLayoutInformationDto>> LoadDiagramLayoutConfiguration(Guid projectId, Guid reviewTaskId, string configurationName);
    }
}
