// --------------------------------------------------------------------------------------------------------
// <copyright file="IndexViewModel.cs" company="RHEA System S.A.">
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
        public IndexViewModel(IProjectService projectService, AuthenticationStateProvider authenticationProvider)
        {
            this.projectService = projectService;
            this.authenticationProvider = authenticationProvider;
            authenticationProvider.AuthenticationStateChanged += this.PopulateAvailableProjects;
        }

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
        ///     Populate the <see cref="AvailableProject" /> collection
        /// </summary>
        /// <param name="task">The <see cref="AuthenticationState" /> Task</param>
        private void PopulateAvailableProjects(Task<AuthenticationState> task)
        {
            Task.Run(async () =>
            {
                var state = await task;
                this.AvailableProject.Clear();

                if (state.User.Identity is { IsAuthenticated: true })
                {
                    this.AvailableProject.AddRange(await this.projectService.GetUserParticipation());
                }
            });
        }
    }
}
