// --------------------------------------------------------------------------------------------------------
// <copyright file="ICometUploadViewModel.cs" company="RHEA System S.A.">
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
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;

    using UI_DSM.Client.Enumerator;
    using UI_DSM.Shared.DTO.CometData;
    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    ///     Interface definition for <see cref="CometUploadViewModel" />
    /// </summary>
    public interface ICometUploadViewModel : IDisposable
    {
        /// <summary>
        ///     A collection of available <see cref="IterationData" />
        /// </summary>
        IEnumerable<IterationData> AvailableIterationsSetup { get; set; }

        /// <summary>
        ///     The currently selected <see cref="EngineeringModelData" />
        /// </summary>
        EngineeringModelData SelectedEngineeringModelSetup { get; set; }

        /// <summary>
        ///     The currently selected <see cref="IterationData" />
        /// </summary>
        IterationData SelectedIterationSetup { get; set; }

        /// <summary>
        ///     <see cref="EventCallback" /> to invoke after selecting the <see cref="IterationSetup" />
        /// </summary>
        EventCallback OnEventCallback { get; set; }

        /// <summary>
        ///     Value indicating the current status of the COMET connection
        /// </summary>
        AuthenticationStatus CometConnectionStatus { get; set; }

        /// <summary>
        ///     Value indicating the current status of the <see cref="Iteration" /> upload
        /// </summary>
        UploadStatus IterationUploadStatus { get; }

        /// <summary>
        ///     Gets the <see cref="CometAuthenticationData" />
        /// </summary>
        CometAuthenticationData UploadData { get; }

        /// <summary>
        ///     The error message
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        ///     Gets the <see cref="IBrowserFile" /> to upload
        /// </summary>
        IBrowserFile BrowserFile { get; }

        /// <summary>
        ///     A collection of <see cref="EngineeringModelData"/>
        /// </summary>
        List<EngineeringModelData> AvailableModels { get; set; }

        /// <summary>
        ///     Tries to establish a connection to a Comet instance
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        Task CometLogin();

        /// <summary>
        ///     Closes the connection to the Comet Session
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        Task CometLogout();

        /// <summary>
        ///     Initializes the view model properties
        /// </summary>
        void InitializeProperties();

        /// <summary>
        ///     Creates an Annex C file for the provided iteration
        /// </summary>
        /// <returns>A <see cref="Task" /> with the <see cref="ModelUploadResponse" /></returns>
        Task<ModelUploadResponse> UploadSelectedIteration();

        /// <summary>
        ///     Handle an upload failure
        /// </summary>
        /// <param name="response">The <see cref="RequestResponseDto" /> that contains information about the failure</param>
        void HandleUploadFailure(RequestResponseDto response);

        /// <summary>
        ///     Handle the change of the selected file to upload
        /// </summary>
        /// <param name="newBrowserFile">The <see cref="IBrowserFile" /></param>
        void HandleOnFileSelected(IBrowserFile newBrowserFile);

        /// <summary>
        ///     Handle any change on the <see cref="CometAuthenticationData.Url" /> property
        /// </summary>
        /// <param name="newText">The new text</param>
        void UrlTextHasChanged(string newText);
    }
}
