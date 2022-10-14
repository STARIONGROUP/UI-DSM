// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewCreationViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Components.NormalUser.ProjectReview;
    using UI_DSM.Client.Services.Administration.ProjectService;
    using UI_DSM.Client.Services.ReviewService;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Wrappers;

    /// <summary>
    ///     View model for the <see cref="ReviewCreation" /> component
    /// </summary>
    public class ReviewCreationViewModel : IReviewCreationViewModel
    {
        /// <summary>
        ///     The <see cref="IProjectService" />
        /// </summary>
        private readonly IProjectService projectService;


        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectPageViewModel" /> class.
        /// </summary>
        /// <param name="projectService">The <see cref="IProjectService" /></param>
        public ReviewCreationViewModel(IProjectService projectService)
        {
            this.projectService = projectService;
        }

        /// <summary>
        ///     The <see cref="Review" /> to create
        /// </summary>
        public Review Review { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback" /> to call for data submit
        /// </summary>
        public EventCallback OnValidSubmit { get; set; }

        /// <summary>
        ///     A collection of <see cref="Model" /> that has been selected
        /// </summary>
        public IEnumerable<Model> SelectedModels { get; set; }

        /// <summary>
        ///     A collection of <see cref="Model" /> a project model
        /// </summary>
        public IEnumerable<Model> ProjectModels { get; set; }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <param name="projectGuid">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        public async Task OnInitializedAsync(Guid projectGuid)
        {
            var projectResponse = await this.projectService.GetProject(projectGuid, 1);
            this.ProjectModels = projectResponse.Artifacts.OfType<Model>().ToList();
        }
    }
}
