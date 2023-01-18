// --------------------------------------------------------------------------------------------------------
// <copyright file="BudgetUpload.razor.cs" company="RHEA System S.A.">
//  Copyright (c) 2023 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Components.Administration.BudgetManagement
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;

    using UI_DSM.Client.ViewModels.Components.Administration.BudgetManagement;

    /// <summary>
    ///     Component used to upload a BudgetTemplate
    /// </summary>
    public partial class BudgetUpload
    {
        /// <summary>
        ///     The <see cref="IBudgetUploadViewModel" />
        /// </summary>
        [Parameter]
        public IBudgetUploadViewModel ViewModel { get; set; }

        /// <summary>
        ///     The text for the upload button
        /// </summary>
        public string UploadText { get; set; }

        /// <summary>
        ///     Indicates if the upload button is enabled or not
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.IsEnabled = true;
            this.UploadText = "Upload";
        }

        /// <summary>
        ///     Handle the InputFile event
        /// </summary>
        /// <param name="arg">The <see cref="InputFileChangeEventArgs" /></param>
        /// <returns>A <see cref="Task" /></returns>
        private void OnFileSelected(InputFileChangeEventArgs arg)
        {
            if (arg.FileCount == 1)
            {
                this.ViewModel.BudgetData.BrowserFile = arg.File;
            }
        }

        /// <summary>
        ///     Handle the submit of the <see cref="EditForm" />
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task OnSubmit()
        {
            this.UploadText = "Uploading...";
            this.IsEnabled = false;
            await this.InvokeAsync(this.ViewModel.OnSubmit.InvokeAsync);
            this.UploadText = "Upload";
            this.IsEnabled = true;
        }
    }
}
