// --------------------------------------------------------------------------------------------------------
// <copyright file="IIndexViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Pages
{
    using DynamicData;
    
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Authorization;
  
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    ///     Interface definition for <see cref="IndexViewModel" />
    /// </summary>
    public interface IIndexViewModel: IDisposable
    {
        /// <summary>
        ///     Populate the <see cref="AvailableProject" /> collection
        /// </summary>
        void PopulateAvailableProjects();

        /// <summary>
        ///     Populates the <see cref="IndexViewModel.AvailableProject" /> collection based on a <see cref="AuthenticationState" />
        /// </summary>
        /// <param name="state">The <see cref="AuthenticationState" /></param>
        /// <returns>A <see cref="Task" /></returns>
        Task PopulateAvailableProjects(AuthenticationState state);

        /// <summary>
        ///     A collection of available <see cref="Project" /> for the user
        /// </summary>
        SourceList<Project> AvailableProject { get; }

        /// <summary>
        ///     A collection of comments and tasks <see cref="Project" /> for the user
        /// </summary>
        public Dictionary<Guid, AdditionalComputedProperties> CommentsAndTasks { get; set; }

        /// <summary>
        ///     Navigate to the page dedicated to the given <see cref="Project" />
        /// </summary>
        /// <param name="project">The <see cref="Project" /></param>
        void GoToProjectPage(Project project);

        /// <summary>
        ///     Gets or sets the <see cref="NavigationManager" />
        /// </summary>
        NavigationManager NavigationManager { get; set; }
    }
}
