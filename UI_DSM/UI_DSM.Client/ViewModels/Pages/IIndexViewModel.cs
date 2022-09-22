// --------------------------------------------------------------------------------------------------------
// <copyright file="IIndexViewModel.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
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

    using UI_DSM.Shared.Models;

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
        ///     A collection of available <see cref="Project" /> for the user
        /// </summary>
        SourceList<Project> AvailableProject { get; }

        /// <summary>
        ///     Navigate to the page dedicated to the given <see cref="Project" />
        /// </summary>
        /// <param name="project">The <see cref="Project" /></param>
        void GoToProjectPage(Project project);
    }
}
