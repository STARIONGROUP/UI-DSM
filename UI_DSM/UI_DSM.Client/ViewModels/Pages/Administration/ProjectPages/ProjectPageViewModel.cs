// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectPageViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Pages.Administration.ProjectPages
{
    using DevExpress.Blazor;

    using DynamicData;

    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.Components.Administration.ParticipantManagement;
    using UI_DSM.Client.Components.Administration.ProjectManagement;
    using UI_DSM.Client.Pages.Administration.ProjectPages;
    using UI_DSM.Client.Services.Administration.ParticipantService;
    using UI_DSM.Client.Services.Administration.ProjectService;
    using UI_DSM.Client.Services.Administration.RoleService;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.Administration.ParticipantManagement;
    using UI_DSM.Client.ViewModels.Components.Administration.ProjectManagement;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="ProjectPage" /> page
    /// </summary>
    public class ProjectPageViewModel : ReactiveObject, IProjectPageViewModel
    {
        /// <summary>
        ///     The <see cref="IParticipantService" />
        /// </summary>
        private readonly IParticipantService participantService;

        /// <summary>
        ///     The <see cref="IProjectService" />
        /// </summary>
        private readonly IProjectService projectService;

        /// <summary>
        ///     Backing field for <see cref="CreationPopupVisible" />
        /// </summary>
        private bool creationPopupVisible;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectPageViewModel" /> class.
        /// </summary>
        /// <param name="projectService">The <see cref="IProjectService" /></param>
        /// <param name="participantService">The <see cref="IParticipantService" /></param>
        /// <param name="roleService">The <see cref="IRoleService" /></param>
        public ProjectPageViewModel(IProjectService projectService, IParticipantService participantService, IRoleService roleService)
        {
            this.projectService = projectService;
            this.participantService = participantService;

            this.ParticipantCreationViewModel = new ParticipantCreationViewModel(this.participantService, roleService)
            {
                OnValidSubmit = new EventCallbackFactory().Create(this, this.CreateParticipant)
            };
        }

        /// <summary>
        ///     The <see cref="IProjectDetailsViewModel" /> for the <see cref="ProjectDetails" /> component
        /// </summary>
        public IProjectDetailsViewModel ProjectDetailsViewModel { get; } = new ProjectDetailsViewModel();

        /// <summary>
        ///     Value indicating if the <see cref="DxPopup" /> for the creation of <see cref="Participant" /> is visible
        /// </summary>
        public bool CreationPopupVisible
        {
            get => this.creationPopupVisible;
            set => this.RaiseAndSetIfChanged(ref this.creationPopupVisible, value);
        }

        /// <summary>
        ///     Gets the <see cref="IErrorMessageViewModel" />
        /// </summary>
        public IErrorMessageViewModel ErrorMessageViewModel { get; } = new ErrorMessageViewModel();

        /// <summary>
        ///     Gets the <see cref="IParticipantCreationViewModel" />
        /// </summary>
        public IParticipantCreationViewModel ParticipantCreationViewModel { get; set; }

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
            this.ProjectDetailsViewModel.Project = projectResponse;
        }

        /// <summary>
        ///     Opens the <see cref="ParticipantCreation" /> popup
        /// </summary>
        public async Task OpenCreateParticipantPopup()
        {
            await this.ParticipantCreationViewModel.UpdateProperties(this.ProjectDetailsViewModel.Project.Id);
            this.CreationPopupVisible = true;
        }

        /// <summary>
        ///     Create a new <see cref="Participant" /> with the provided data
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task CreateParticipant()
        {
            var requestResponse = await this.participantService.CreateParticipant(this.ProjectDetailsViewModel.Project.Id, this.ParticipantCreationViewModel.Participant);

            if (requestResponse.IsRequestSuccessful)
            {
                this.ProjectDetailsViewModel.Project.Participants.Add(requestResponse.Entity);
                this.CreationPopupVisible = false;
            }
            else
            {
                this.ErrorMessageViewModel.Errors.Clear();
                this.ErrorMessageViewModel.Errors.AddRange(requestResponse.Errors);
            }
        }
    }
}
