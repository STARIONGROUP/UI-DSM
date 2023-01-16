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
    using System.Reactive.Linq;

    using CDP4Common.EngineeringModelData;

    using DynamicData;

    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.Components.Administration.ModelManagement;
    using UI_DSM.Client.Components.Administration.ParticipantManagement;
    using UI_DSM.Client.Components.Administration.ProjectManagement;
    using UI_DSM.Client.Pages.Administration.ProjectPages;
    using UI_DSM.Client.Services.Administration.ParticipantService;
    using UI_DSM.Client.Services.Administration.ProjectService;
    using UI_DSM.Client.Services.Administration.RoleService;
    using UI_DSM.Client.Services.ArtifactService;
    using UI_DSM.Client.Services.ThingService;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.Administration.ModelManagement;
    using UI_DSM.Client.ViewModels.Components.Administration.ParticipantManagement;
    using UI_DSM.Client.ViewModels.Components.Administration.ProjectManagement;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="ProjectPage" /> page
    /// </summary>
    public class ProjectPageViewModel : ReactiveObject, IProjectPageViewModel
    {
        /// <summary>
        ///     The <see cref="IArtifactService" />
        /// </summary>
        private readonly IArtifactService artifactService;

        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     The <see cref="IParticipantService" />
        /// </summary>
        private readonly IParticipantService participantService;

        /// <summary>
        ///     The <see cref="IProjectService" />
        /// </summary>
        private readonly IProjectService projectService;

        /// <summary>
        ///     Backing field for <see cref="IsOnCometConnectionMode" />
        /// </summary>
        private bool isOnCometConnectionMode;

        /// <summary>
        ///     Backing field for <see cref="IsOnCreationMode" />
        /// </summary>
        private bool isOnCreationMode;

        /// <summary>
        /// The <see cref="IThingService"/>
        /// </summary>
        private readonly IThingService thingService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectPageViewModel" /> class.
        /// </summary>
        /// <param name="projectService">The <see cref="IProjectService" /></param>
        /// <param name="participantService">The <see cref="IParticipantService" /></param>
        /// <param name="roleService">The <see cref="IRoleService" /></param>
        /// <param name="cometUploadViewModel">The <see cref="ICometUploadViewModel" /></param>
        /// <param name="artifactService">The <see cref="IArtifactService" /></param>
        /// <param name="thingService">The <see cref="IThingService"/></param>
        public ProjectPageViewModel(IProjectService projectService, IParticipantService participantService, IRoleService roleService,
            ICometUploadViewModel cometUploadViewModel, IArtifactService artifactService, IThingService thingService)
        {
            this.projectService = projectService;
            this.participantService = participantService;
            this.artifactService = artifactService;
            this.thingService = thingService;

            this.ParticipantCreationViewModel = new ParticipantCreationViewModel(this.participantService, roleService, thingService)
            {
                OnValidSubmit = new EventCallbackFactory().Create(this, this.CreateParticipant)
            };

            this.ProjectDetailsViewModel = new ProjectDetailsViewModel(this.participantService, roleService, thingService);

            this.CometUploadViewModel = cometUploadViewModel;
            this.CometUploadViewModel.OnEventCallback = new EventCallbackFactory().Create(this, _ => this.UploadIteration());

            this.disposables.Add(this.WhenAnyValue(x => x.IsOnCometConnectionMode)
                .Where(x => !x).Subscribe(async _ => await this.CometUploadViewModel.CometLogout()));

            this.disposables.Add(this.WhenAnyValue(x => x.IsOnCometConnectionMode)
                .Where(x => x).Subscribe(_ => this.CometUploadViewModel.InitializeProperties()));
        }

        /// <summary>
        ///     The <see cref="ICometUploadViewModel" /> for the <see cref="CometUpload" /> component
        /// </summary>
        public ICometUploadViewModel CometUploadViewModel { get; }

        /// <summary>
        ///     The <see cref="IProjectDetailsViewModel" /> for the <see cref="ProjectDetails" /> component
        /// </summary>
        public IProjectDetailsViewModel ProjectDetailsViewModel { get; } 

        /// <summary>
        ///     Value indicating the user is currently creating a new <see cref="Participant" />
        /// </summary>
        public bool IsOnCreationMode
        {
            get => this.isOnCreationMode;
            set => this.RaiseAndSetIfChanged(ref this.isOnCreationMode, value);
        }

        /// <summary>
        ///     Value indicating whether the user is currently trying to establish a connection to COMET
        /// </summary>
        public bool IsOnCometConnectionMode
        {
            get => this.isOnCometConnectionMode;
            set => this.RaiseAndSetIfChanged(ref this.isOnCometConnectionMode, value);
        }

        /// <summary>
        ///     Value indicating whether the user is authorized to access the page or not
        /// </summary>
        public bool IsAuthorized { get; set; }

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
            var projectResponse = await this.projectService.GetProject(projectGuid);

            if (projectResponse != null)
            {
                projectResponse.Participants.AddRange(await this.participantService.GetParticipantsOfProject(projectGuid));
                projectResponse.Artifacts.AddRange(await this.artifactService.GetArtifactsOfProject(projectGuid));
                var participant = await this.participantService.GetCurrentParticipant(projectGuid);

                if (participant != null)
                {
                    this.IsAuthorized = participant.IsAllowedTo(AccessRight.ProjectManagement);
                }

                this.ProjectDetailsViewModel.Project = projectResponse;
            }
        }

        /// <summary>
        ///     Opens the <see cref="ParticipantCreation" /> popup
        /// </summary>
        public async Task OpenCreateParticipantPopup()
        {
            await this.ParticipantCreationViewModel.UpdateProperties(this.ProjectDetailsViewModel.Project);
            this.IsOnCreationMode = true;
        }

        /// <summary>
        ///     Opens the <see cref="CometUpload" /> popup
        /// </summary>
        public void OpenCometConnectionPopup()
        {
            this.IsOnCometConnectionMode = true;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.disposables.ForEach(x => x.Dispose());
            this.disposables.Clear();
        }

        /// <summary>
        ///     Uploads an <see cref="Iteration" /> to the server
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task UploadIteration()
        {
            var uploadResponse = await this.CometUploadViewModel.UploadSelectedIteration();

            if (uploadResponse.IsRequestSuccessful)
            {
                var uploadModelResponse = await this.artifactService.UploadModel(this.ProjectDetailsViewModel.Project.Id, uploadResponse.UploadedFilePath,
                    $"{this.CometUploadViewModel.SelectedEngineeringModelSetup.EngineeringModelName} - {this.CometUploadViewModel.SelectedIterationSetup.IterationName}");

                if (uploadModelResponse.IsRequestSuccessful)
                {
                    this.ProjectDetailsViewModel.Project.Artifacts.Add(uploadModelResponse.Entity);
                    await this.thingService.IndexIteration(this.ProjectDetailsViewModel.Project.Id, new List<Guid>{uploadModelResponse.Entity.Id});
                    this.IsOnCometConnectionMode = false;
                }
                else
                {
                    this.CometUploadViewModel.HandleUploadFailure(uploadModelResponse);
                }
            }
            else
            {
                this.CometUploadViewModel.HandleUploadFailure(uploadResponse);
            }
        }

        /// <summary>
        ///     Create a new <see cref="Participant" /> with the provided data
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task CreateParticipant()
        {
            this.ParticipantCreationViewModel.Participant.DomainsOfExpertise = this.ParticipantCreationViewModel.SelectedDomains.ToList();
            var requestResponse = await this.participantService.CreateParticipant(this.ProjectDetailsViewModel.Project.Id, this.ParticipantCreationViewModel.Participant);

            if (requestResponse.IsRequestSuccessful)
            {
                this.ProjectDetailsViewModel.Project.Participants.Add(requestResponse.Entity);
                this.IsOnCreationMode = false;
            }
            else
            {
                this.ErrorMessageViewModel.Errors.Clear();
                this.ErrorMessageViewModel.Errors.AddRange(requestResponse.Errors);
            }
        }
    }
}
