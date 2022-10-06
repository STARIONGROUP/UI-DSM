// --------------------------------------------------------------------------------------------------------
// <copyright file="IndexViewModel.cs" company="RHEA System S.A.">
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
    
    using UI_DSM.Client.Pages;
    using UI_DSM.Client.Services.Administration.ProjectService;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="Index" /> page
    /// </summary>
    public class IndexViewModel : IIndexViewModel
    {
        /// <summary>
        ///     The <see cref="AuthenticationStateProvider" />
        /// </summary>
        private readonly AuthenticationStateProvider authenticationProvider;

        /// <summary>
        ///     The <see cref="IProjectService" />
        /// </summary>
        private readonly IProjectService projectService;


        /// <summary>
        ///     Initializes a new instance of the <see cref="IndexViewModel" /> class.
        /// </summary>
        /// <param name="projectService">The <see cref="IProjectService" /></param>
        /// <param name="authenticationProvider">The <see cref="AuthenticationStateProvider" /></param>
        /// <param name="navigationManager">The <see cref="NavigationManager" /></param>
        public IndexViewModel(IProjectService projectService, AuthenticationStateProvider authenticationProvider, NavigationManager navigationManager)
        {
            this.projectService = projectService;
            this.authenticationProvider = authenticationProvider;
            authenticationProvider.AuthenticationStateChanged += this.PopulateAvailableProjects;
            this.NavigationManager = navigationManager;
        }

        /// <summary>
        ///     Gets or sets the <see cref="NavigationManager" />
        /// </summary>
        public NavigationManager NavigationManager { get; set; }

        /// <summary>
        ///     A collection of available <see cref="Project" /> for the user
        /// </summary>
        public SourceList<Project> AvailableProject { get; } = new();

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.authenticationProvider.AuthenticationStateChanged -= this.PopulateAvailableProjects;
        }

        /// <summary>
        ///     Populate the <see cref="AvailableProject" /> collection
        /// </summary>
        public void PopulateAvailableProjects()
        {
            this.PopulateAvailableProjects(this.authenticationProvider.GetAuthenticationStateAsync());
        }

        /// <summary>
        ///     Populates the <see cref="AvailableProject" /> collection based on a <see cref="AuthenticationState" />
        /// </summary>
        /// <param name="state">The <see cref="AuthenticationState" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task PopulateAvailableProjects(AuthenticationState state)
        {
            this.AvailableProject.Clear();
            if (state.User.Identity is { IsAuthenticated: true })
            {
                this.AvailableProject.AddRange(await this.projectService.GetUserParticipation());
            }
        }
        
        /// <summary>
        ///     Populate the <see cref="AvailableProject" /> collection
        /// </summary>
        /// <param name="task">The <see cref="AuthenticationState" /> Task</param>
        private void PopulateAvailableProjects(Task<AuthenticationState> task)
        {
            Task.Run(async () =>
            {
                var state = await task;
                await this.PopulateAvailableProjects(state);
            });
        }

        /// <summary>
        ///     Navigate to the page dedicated to the given <see cref="Project" />
        /// </summary>
        /// <param name="project">The <see cref="Project" /></param>
        public void GoToProjectPage(Project project)
        {
            this.NavigationManager.NavigateTo($"Project/{project.Id}");
        }
    }
}
