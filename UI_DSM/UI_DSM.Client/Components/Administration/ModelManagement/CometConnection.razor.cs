// --------------------------------------------------------------------------------------------------------
// <copyright file="CometConnection.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.Administration.ModelManagement
{
    using CDP4Common.SiteDirectoryData;

    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.ViewModels.Components.Administration.ModelManagement;

    /// <summary>
    ///     Component that gave the possibility to connect to a COMET instance
    /// </summary>
    public partial class CometConnection : IDisposable
    {
        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        private List<IDisposable> disposables = new();

        /// <summary>
        ///     The <see cref="ICometConnectionViewModel" />
        /// </summary>
        [Parameter]
        public ICometConnectionViewModel ViewModel { get; set; }

        /// <summary>
        ///     The text to display inside the connection button
        /// </summary>
        public string ConnectButtonText { get; private set; }

        /// <summary>
        /// The text to display inside the upload button
        /// </summary>
        public string UploadText { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.disposables.ForEach(x => x.Dispose());
            this.disposables.Clear();
        }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        protected override Task OnInitializedAsync()
        {
            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.CometConnectionStatus)
                .Subscribe(_ => this.OnCometConnectionStatusChanged()));

            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.IterationUploadStatus)
                .Subscribe(_ => this.OnIterationUploadStatusChanged()));
                
            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.ErrorMessage)
                .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));

            return base.OnInitializedAsync();
        }

        /// <summary>
        ///     Sets properties when the <see cref="CometConnectionViewModel.IterationUploadStatus" /> has changed
        /// </summary>
        private void OnIterationUploadStatusChanged()
        {
            switch (this.ViewModel.IterationUploadStatus)
            {
                case UploadStatus.None:
                    this.UploadText = "Upload";
                    break;
                case UploadStatus.Uploading:
                    this.UploadText = "Uploading";
                    break;
                case UploadStatus.Fail:
                    this.UploadText = "Retry...";
                    break;
                case UploadStatus.Done:
                    break;
                default:
                    throw new InvalidDataException("Unsupported value");
            }

            this.InvokeAsync(this.StateHasChanged);
        }

        /// <summary>
        ///     Sets properties when the <see cref="CometConnectionViewModel.CometConnectionStatus" /> has changed
        /// </summary>
        private void OnCometConnectionStatusChanged()
        {
            switch (this.ViewModel.CometConnectionStatus)
            {
                case AuthenticationStatus.None:
                    this.ConnectButtonText = "Login to COMET";
                    break;
                case AuthenticationStatus.Authenticating:
                    this.ConnectButtonText = "Authenticating...";
                    break;
                case AuthenticationStatus.Fail:
                case AuthenticationStatus.ServerFailure:
                    this.ConnectButtonText = "Retry";
                    break;
                case AuthenticationStatus.Success:
                    break;
                default:
                    throw new InvalidDataException("Unsupported value");
            }

            this.InvokeAsync(this.StateHasChanged);
        }
    }
}
