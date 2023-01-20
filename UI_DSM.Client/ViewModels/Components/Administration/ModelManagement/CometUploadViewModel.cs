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
    using Microsoft.AspNetCore.Components.Forms;

    using ReactiveUI;

    using UI_DSM.Client.Components.Administration.ModelManagement;
    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.Services.Administration.CometService;
    using UI_DSM.Shared.DTO.CometData;
    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    ///     View model for the <see cref="CometUpload" /> component
    /// </summary>
    public class CometUploadViewModel : ReactiveObject, ICometUploadViewModel
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
        ///     A collection of <see cref="EngineeringModelData"/>
        /// </summary>
        public List<EngineeringModelData> AvailableModels { get; set; }

        /// <summary>
        ///     Backing field for <see cref="BrowserFile" />
        /// </summary>
        private IBrowserFile browserFile;

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
        private EngineeringModelData selectedEngineeringModelSetup;

        /// <summary>
        ///     Backing field for <see cref="SelectedIterationSetup" />
        /// </summary>
        private IterationData selectedIterationSetup;

        /// <summary>
        ///     The <see cref="Guid" /> of the session with the API
        /// </summary>
        private Guid sessionId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CometUploadViewModel" /> class.
        /// </summary>
        /// <param name="cometService">The <see cref="ICometService" /></param>
        public CometUploadViewModel(ICometService cometService)
        {
            this.cometService = cometService;
        }

        /// <summary>
        ///     Gets the <see cref="IBrowserFile" /> to upload
        /// </summary>
        public IBrowserFile BrowserFile
        {
            get => this.browserFile;
            private set => this.RaiseAndSetIfChanged(ref this.browserFile, value);
        }

        /// <summary>
        ///     A collection of available <see cref="IterationData" />
        /// </summary>
        public IEnumerable<IterationData> AvailableIterationsSetup { get; set; } = new List<IterationData>();

        /// <summary>
        ///     The currently selected <see cref="EngineeringModelData"/>
        /// </summary>
        public EngineeringModelData SelectedEngineeringModelSetup
        {
            get => this.selectedEngineeringModelSetup;
            set => this.RaiseAndSetIfChanged(ref this.selectedEngineeringModelSetup, value);
        }

        /// <summary>
        ///     The currently selected <see cref="IterationData" />
        /// </summary>
        [Required]
        public IterationData SelectedIterationSetup
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
        public CometAuthenticationData UploadData { get; } = new();

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

            var response = this.UploadData.UploadFromFile 
                ? await this.cometService.UploadAnnexC3File(this.UploadData, this.BrowserFile) 
                : await this.cometService.Login(this.UploadData);

            if (response.IsRequestSuccessful)
            {
                this.sessionId = response.SessionId;
                this.CometConnectionStatus = AuthenticationStatus.Success;
                this.UploadData.Password = string.Empty;
                this.ErrorMessage = string.Empty;
                var models = await this.cometService.GetAvailableEngineeringModels(this.sessionId);
                this.AvailableModels = models.AvailableModels;
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
            this.UploadData.UserName = string.Empty;
            this.UploadData.Password = string.Empty;
            this.ErrorMessage = string.Empty;
            this.UploadData.Url = string.Empty;
            this.SelectedEngineeringModelSetup = null;
            this.SelectedIterationSetup = null;
            this.BrowserFile = null;
            this.UploadData.UploadFromFile = false;
            this.IterationUploadStatus = UploadStatus.None;
            this.AvailableModels = new List<EngineeringModelData>();

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
            return this.cometService.UploadIteration(this.sessionId, this.SelectedEngineeringModelSetup.EngineeringId, this.SelectedIterationSetup.IterationId);
        }

        /// <summary>
        ///     Handle an upload failure
        /// </summary>
        /// <param name="response">The <see cref="RequestResponseDto" /> that contains information about the failure</param>
        public void HandleUploadFailure(RequestResponseDto response)
        {
            this.IterationUploadStatus = UploadStatus.Fail;
            this.ErrorMessage = response.Errors.FirstOrDefault();
        }

        /// <summary>
        ///     Handle the change of the selected file to upload
        /// </summary>
        /// <param name="newBrowserFile">The <see cref="IBrowserFile" /></param>
        public void HandleOnFileSelected(IBrowserFile newBrowserFile)
        {
            this.BrowserFile = newBrowserFile;
            this.UploadData.UploadFromFile = true;
            this.UploadData.Url = string.Empty;
        }

        /// <summary>
        ///     Handle any change on the <see cref="CometAuthenticationData.Url" /> property
        /// </summary>
        /// <param name="newText">The new text</param>
        public void UrlTextHasChanged(string newText)
        {
            this.UploadData.Url = newText;

            if (!string.IsNullOrEmpty(this.UploadData.Url) && this.BrowserFile != null)
            {
                this.UploadData.UploadFromFile = false;
                this.BrowserFile = null;
            }
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

            this.AvailableIterationsSetup = this.SelectedEngineeringModelSetup.Iterations.OrderBy(x => x.IterationName);

            this.SelectedIterationSetup = this.AvailableIterationsSetup.FirstOrDefault();
        }
    }
}
