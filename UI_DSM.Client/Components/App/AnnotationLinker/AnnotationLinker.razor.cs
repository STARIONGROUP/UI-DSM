// --------------------------------------------------------------------------------------------------------
// <copyright file="AnnotationLinker.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.AnnotationLinker
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.ViewModels.App.AnnotationLinker;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Component that will provide the capability to link a <see cref="Annotation" /> to other elements
    /// </summary>
    public partial class AnnotationLinker
    {
        /// <summary>
        ///     The <see cref="IAnnotationLinkerViewModel" />
        /// </summary>
        [Parameter]
        public IAnnotationLinkerViewModel ViewModel { get; set; }

        /// <summary>
        ///     The text of the submit button
        /// </summary>
        public string SubmitText => this.GetSubmitText();

        /// <summary>
        ///     Computes the text of the submit button
        /// </summary>
        /// <returns>The text</returns>
        private string GetSubmitText()
        {
            return this.ViewModel.CreationStatus switch
            {
                CreationStatus.Creating => "Linking",
                CreationStatus.Fail => "Retry...",
                _ => "Link"
            };
        }

        /// <summary>
        ///     Handle the click on the subimt button
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private Task OnSubmit()
        {
            return this.InvokeAsync(this.ViewModel.OnSubmit.InvokeAsync);
        }
    }
}
