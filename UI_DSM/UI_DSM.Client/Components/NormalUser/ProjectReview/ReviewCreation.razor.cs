// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewCreation.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.NormalUser.ProjectReview
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This component is used to create new <see cref="Review" />
    /// </summary>
    public partial class ReviewCreation
    {

        /// <summary>
        ///     A collection of <see cref="Model" /> a project artifact
        /// </summary>
        [Parameter]
        public IEnumerable<Artifact> ProjectArtifacts { get; set; }

        /// <summary>
        ///     A collection of <see cref="Model" /> a project model
        /// </summary>
        public IEnumerable<Model> ProjectModels { get; set; } = new List<Model>();

        /// <summary>
        ///     The <see cref="Model" /> of the project
        /// </summary>
        public Model Model { get; set; }

        /// <summary>
        ///     The <see cref="IReviewCreationViewModel" /> for the component
        /// </summary>
        [Parameter]
        public IReviewCreationViewModel ViewModel { get; set; }

        protected override void OnInitialized()
        {
            this.ProjectModels = this.ProjectArtifacts.OfType<Model>().ToList();
        }
    }
}
