// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectManagementViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Pages.Administration
{
    using DynamicData;

    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.Components.Administration.ProjectManagement;
    using UI_DSM.Client.Pages.Administration;
    using UI_DSM.Client.Services.Administration.ProjectService;
    using UI_DSM.Client.Services.Administration.UserService;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.Administration.ProjectManagement;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="ProjectManagement" /> page
    /// </summary>
    public class ProjectManagementViewModel : ReactiveObject, IProjectManagementViewModel
    {
        /// <summary>
        ///     The <see cref="IProjectService" />
        /// </summary>
        private readonly IProjectService projectService;

        /// <summary>
        ///     The <see cref="IUserService" />
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        ///     Backing fielf for <see cref="IsAuthorized" />
        /// </summary>
        private bool isAuthorized;

        /// <summary>
        ///     Backing field for <see cref="IsOnCreationMode" />
        /// </summary>
        private bool isOnCreationMode;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectManagementViewModel" /> class.
        /// </summary>
        /// <param name="projectService">The <see cref="IProjectService" /></param>
        /// <param name="navigationManager">The <see cref="NavigationManager" /></param>
        /// <param name="userService">The <see cref="IUserService" /></param>
        public ProjectManagementViewModel(IProjectService projectService, NavigationManager navigationManager, IUserService userService)
        {
            this.NavigationManager = navigationManager;
            this.projectService = projectService;
            this.userService = userService;

            this.ProjectCreationViewModel = new ProjectCreationViewModel
            {
                OnValidSubmit = new EventCallbackFactory().Create(this, this.CreateProject)
            };
        }

        /// <summary>
        ///     Indicates if the current user is allowed to manage project even if he is not the site admin
        /// </summary>
        public bool IsAuthorized
        {
            get => this.isAuthorized;
            set => this.RaiseAndSetIfChanged(ref this.isAuthorized, value);
        }

        /// <summary>
        ///     Gets or sets the <see cref="NavigationManager" />
        /// </summary>
        public NavigationManager NavigationManager { get; set; }

        /// <summary>
        ///     A collection of <see cref="Projects" />
        /// </summary>
        public SourceList<Project> Projects { get; private set; } = new();

        /// <summary>
        ///     Value indicating the user is currently creating a new <see cref="Project" />
        /// </summary>
        public bool IsOnCreationMode
        {
            get => this.isOnCreationMode;
            set => this.RaiseAndSetIfChanged(ref this.isOnCreationMode, value);
        }

        /// <summary>
        ///     The <see cref="IErrorMessageViewModel" />
        /// </summary>
        public IErrorMessageViewModel ErrorMessageViewModel { get; } = new ErrorMessageViewModel();

        /// <summary>
        ///     The <see cref="IProjectCreationViewModel" />
        /// </summary>
        public IProjectCreationViewModel ProjectCreationViewModel { get; private set; }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        public async Task OnInitializedAsync()
        {
            var participants = await this.userService.GetParticipantsForUser();
            this.IsAuthorized = participants.Any(x => x.IsAllowedTo(AccessRight.ProjectManagement));
            this.Projects.AddRange(await this.projectService.GetProjects());
        }

        /// <summary>
        ///     Opens the <see cref="ProjectCreation" /> as a popup
        /// </summary>
        public void OpenCreatePopup()
        {
            this.ProjectCreationViewModel.Project = new Project();
            this.ErrorMessageViewModel.Errors.Clear();
            this.IsOnCreationMode = true;
        }

        /// <summary>
        ///     Navigate to the page dedicated to the given <see cref="Project" />
        /// </summary>
        /// <param name="project">The <see cref="Project" /></param>
        public void GoToProjectPage(Project project)
        {
            this.NavigationManager.NavigateTo($"Administration/Project/{project.Id}");
        }

        /// <summary>
        ///     Tries to create a new <see cref="Project" />
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task CreateProject()
        {
            try
            {
                var creationResult = await this.projectService.CreateProject(this.ProjectCreationViewModel.Project);
                this.ErrorMessageViewModel.Errors.Clear();

                if (creationResult.Errors.Any())
                {
                    this.ErrorMessageViewModel.Errors.AddRange(creationResult.Errors);
                }

                if (creationResult.IsRequestSuccessful)
                {
                    this.Projects.Add(creationResult.Entity);
                }

                this.IsOnCreationMode = !creationResult.IsRequestSuccessful;
            }
            catch (Exception exception)
            {
                this.ErrorMessageViewModel.Errors.Clear();
                this.ErrorMessageViewModel.Errors.Add(exception.Message);
            }
        }
    }
}
