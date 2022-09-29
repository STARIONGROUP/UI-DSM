// --------------------------------------------------------------------------------------------------------
// <copyright file="CometConnectionViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.Administration.ModelManagement
{
    using System.ComponentModel.DataAnnotations;

    using CDP4Common.EngineeringModelData;

    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.Components.Administration.ModelManagement;
    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.Services.Administration.CometService;
    using UI_DSM.Shared.DTO.CometData;
    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    ///     View model for the <see cref="CometConnection" /> component
    /// </summary>
    public class CometConnectionViewModel : ReactiveObject, ICometConnectionViewModel
    {
        /// <summary>
        ///     The <see cref="ICometService" />
        /// </summary>
        private readonly ICometService cometService;

        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     <see cref="Dictionary{TKey,TValue}" /> that stores available models with frozen Iteration
        /// </summary>
        private Dictionary<Guid, List<Tuple<Guid, string>>> availableModels;

        /// <summary>
        ///     Backing field for <see cref="CometConnectionStatus" />
        /// </summary>
        private AuthenticationStatus cometConnectionStatus;

        /// <summary>
        ///     Backing field for <see cref="ErrorMessage" />
        /// </summary>
        private string errorMessage;

        /// <summary>
        ///     Backing field for <see cref="IterationUploadStatus" />
        /// </summary>
        private UploadStatus iterationUploadStatus;

        /// <summary>
        ///     Backing field for <see cref="SelectedEngineeringModelSetup" />
        /// </summary>
        private Tuple<Guid, string> selectedEngineeringModelSetup;

        /// <summary>
        ///     Backing field for <see cref="SelectedIterationSetup" />
        /// </summary>
        private Tuple<Guid, string> selectedIterationSetup;

        /// <summary>
        ///     The <see cref="Guid" /> of the session with the API
        /// </summary>
        private Guid sessionId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CometConnectionViewModel" /> class.
        /// </summary>
        /// <param name="cometService">The <see cref="ICometService" /></param>
        public CometConnectionViewModel(ICometService cometService)
        {
            this.cometService = cometService;
        }

        /// <summary>
        ///     A collection of available <see cref="Tuple{Guid,String}" />
        /// </summary>
        public IEnumerable<Tuple<Guid, string>> AvailableEngineeringModels { get; set; } = new List<Tuple<Guid, string>>();

        /// <summary>
        ///     A collection of available <see cref="Tuple{Guid, String}" />
        /// </summary>
        public IEnumerable<Tuple<Guid, string>> AvailableIterationsSetup { get; set; } = new List<Tuple<Guid, string>>();

        /// <summary>
        ///     The currently selected <see cref="Tuple{Guid, String}" />
        /// </summary>
        public Tuple<Guid, string> SelectedEngineeringModelSetup
        {
            get => this.selectedEngineeringModelSetup;
            set => this.RaiseAndSetIfChanged(ref this.selectedEngineeringModelSetup, value);
        }

        /// <summary>
        ///     The currently selected <see cref="Tuple{Guid, String}" />
        /// </summary>
        [Required]
        public Tuple<Guid, string> SelectedIterationSetup
        {
            get => this.selectedIterationSetup;
            set => this.RaiseAndSetIfChanged(ref this.selectedIterationSetup, value);
        }

        /// <summary>
        ///     Value indicating the current status of the COMET connection
        /// </summary>
        public AuthenticationStatus CometConnectionStatus
        {
            get => this.cometConnectionStatus;
            set => this.RaiseAndSetIfChanged(ref this.cometConnectionStatus, value);
        }

        /// <summary>
        ///     Value indicating the current status of the <see cref="Iteration" /> upload
        /// </summary>
        public UploadStatus IterationUploadStatus
        {
            get => this.iterationUploadStatus;
            private set => this.RaiseAndSetIfChanged(ref this.iterationUploadStatus, value);
        }

        /// <summary>
        ///     The error message
        /// </summary>
        public string ErrorMessage
        {
            get => this.errorMessage;
            private set => this.RaiseAndSetIfChanged(ref this.errorMessage, value);
        }

        /// <summary>
        ///     Gets the <see cref="CometAuthenticationData" />
        /// </summary>
        public CometAuthenticationData AuthenticationData { get; } = new();

        /// <summary>
        ///     <see cref="EventCallback" /> to be called after the <see cref="SelectedIterationSetup" /> has been set
        /// </summary>
        public EventCallback OnEventCallback { get; set; }

        /// <summary>
        ///     Tries to establish a connection to a Comet instance
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        public async Task CometLogin()
        {
            this.CometConnectionStatus = AuthenticationStatus.Authenticating;
            var response = await this.cometService.Login(this.AuthenticationData);

            if (response.IsRequestSuccessful)
            {
                this.sessionId = response.SessionId;
                this.CometConnectionStatus = AuthenticationStatus.Success;
                this.AuthenticationData.Password = string.Empty;
                this.ErrorMessage = string.Empty;
                var models = await this.cometService.GetAvailableEngineeringModels(this.sessionId);
                this.availableModels = models.AvailableModels;

                this.AvailableEngineeringModels = models.ModelNames.Keys
                    .Select(modelName => new Tuple<Guid, string>(modelName, models.ModelNames[modelName])).ToList();
            }
            else
            {
                this.ErrorMessage = response.Errors.FirstOrDefault();
                this.CometConnectionStatus = AuthenticationStatus.Fail;
            }
        }

        /// <summary>
        ///     Closes the connection to the Comet Session
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        public async Task CometLogout()
        {
            if (this.sessionId != Guid.Empty)
            {
                await this.cometService.Logout(this.sessionId);
            }

            this.CometConnectionStatus = AuthenticationStatus.None;
        }

        /// <summary>
        ///     Initializes the view model properties
        /// </summary>
        public void InitializeProperties()
        {
            this.CometConnectionStatus = AuthenticationStatus.None;
            this.AuthenticationData.UserName = string.Empty;
            this.AuthenticationData.Password = string.Empty;
            this.ErrorMessage = string.Empty;
            this.AuthenticationData.Url = string.Empty;
            this.AvailableEngineeringModels = new List<Tuple<Guid, string>>();
            this.SelectedEngineeringModelSetup = null;
            this.SelectedIterationSetup = null;
            this.IterationUploadStatus = UploadStatus.None;
            this.availableModels = new Dictionary<Guid, List<Tuple<Guid, string>>>();

            this.disposables.Add(this.WhenAnyValue(x => x.SelectedEngineeringModelSetup)
                .Subscribe(_ => this.UpdateAvailableIterationsSetup()));
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
        ///     Uploads the selected <see cref="Iteration" />
        /// </summary>
        /// <returns>A <see cref="Task" /> with the <see cref="ModelUploadResponse" /></returns>
        public Task<ModelUploadResponse> UploadSelectedIteration()
        {
            this.IterationUploadStatus = UploadStatus.Uploading;
            return this.cometService.UploadIteration(this.sessionId, this.SelectedEngineeringModelSetup.Item1, this.SelectedIterationSetup.Item1);
        }

        /// <summary>
        ///     Handle an upload failure
        /// </summary>
        /// <param name="response">The <see cref="RequestResponseDto"/> that contains information about the failure</param>
        public void HandleUploadFailure(RequestResponseDto response)
        {
            this.IterationUploadStatus = UploadStatus.Fail;
            this.ErrorMessage = response.Errors.FirstOrDefault();
        }

        /// <summary>
        ///     Fills the <see cref="AvailableIterationsSetup" /> collection
        /// </summary>
        private void UpdateAvailableIterationsSetup()
        {
            if (this.SelectedEngineeringModelSetup == null)
            {
                return;
            }

            this.AvailableIterationsSetup = this.availableModels.TryGetValue(this.SelectedEngineeringModelSetup.Item1, out var iterations) 
                ? iterations 
                : new List<Tuple<Guid, string>>();

            this.SelectedIterationSetup = this.AvailableIterationsSetup?.FirstOrDefault();
        }
    }
}
