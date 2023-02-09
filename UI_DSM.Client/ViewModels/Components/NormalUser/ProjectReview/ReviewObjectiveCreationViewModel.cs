// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveCreationViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview
{
    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.Components.NormalUser.ProjectReview;
    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.Services.ReviewObjectiveService;
    using UI_DSM.Client.Services.ReviewService;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="ReviewCreation" /> component
    /// </summary>
    public class ReviewObjectiveCreationViewModel : ReactiveObject, IReviewObjectiveCreationViewModel
    {
        /// <summary>
        ///     The <see cref="IReviewService" />
        /// </summary>
        private readonly IReviewObjectiveService reviewObjectiveService;

        /// <summary>
        ///     Backing field for <see cref="ProjectId" />
        /// </summary>
        private Guid projectId;

        /// <summary>
        ///     Backing field for <see cref="ReviewId" />
        /// </summary>
        private Guid reviewId;

        /// <summary>
        ///     Backing field for <see cref="ReviewObjectivesCreationStatus" />
        /// </summary>
        private CreationStatus reviewObjectivesCreationStatus;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewObjectiveCreationViewModel" /> class.
        /// </summary>
        /// <param name="reviewObjectiveService">The <see cref="IReviewObjectiveService" /></param>
        public ReviewObjectiveCreationViewModel(IReviewObjectiveService reviewObjectiveService)
        {
            this.reviewObjectiveService = reviewObjectiveService;
        }

        /// <summary>
        ///     The <see cref="ReviewObjective" /> to create
        /// </summary>
        public ReviewObjective ReviewObjective { get; set; }

        /// <summary>
        ///     The <see cref="Guid" /> of the <see cref="Project" />
        /// </summary>
        public Guid ProjectId
        {
            get => this.projectId;
            set => this.RaiseAndSetIfChanged(ref this.projectId, value);
        }

        /// <summary>
        ///     The <see cref="Guid" /> of the <see cref="Review" />
        /// </summary>
        public Guid ReviewId
        {
            get => this.reviewId;
            set => this.RaiseAndSetIfChanged(ref this.reviewId, value);
        }

        /// <summary>
        ///     The <see cref="EventCallback" /> to call for data submit
        /// </summary>
        public EventCallback OnValidSubmit { get; set; }

        /// <summary>
        ///     A collection of <see cref="ReviewObjectiveCreationDto" /> that has been selected
        /// </summary>
        public IEnumerable<ReviewObjectiveCreationDto> SelectedReviewObjectives { get; set; }

        /// <summary>
        ///     A collection of <see cref="ReviewObjectiveCreationDto" /> of kind PRR that has been selected
        /// </summary>
        public IEnumerable<ReviewObjectiveCreationDto> SelectedReviewObjectivesPrr { get; set; }

        /// <summary>
        ///     A collection of <see cref="ReviewObjectiveCreationDto" /> of kind SRR that has been selected
        /// </summary>
        public IEnumerable<ReviewObjectiveCreationDto> SelectedReviewObjectivesSrr { get; set; }

        /// <summary>
        ///     A collection of <see cref="ReviewObjectiveDto" /> available for a review
        /// </summary>
        public List<ReviewObjectiveCreationDto> AvailableReviewObjectiveCreationDto { get; set; } = new();

        /// <summary>
        ///     Value indicating the current status of the <see cref="List{ReviewObjectives}" /> creation
        /// </summary>
        public CreationStatus ReviewObjectivesCreationStatus
        {
            get => this.reviewObjectivesCreationStatus;
            set => this.RaiseAndSetIfChanged(ref this.reviewObjectivesCreationStatus, value);
        }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        public async Task OnInitializedAsync()
        {
            this.AvailableReviewObjectiveCreationDto = await this.reviewObjectiveService.GetAvailableTemplates(this.projectId, this.reviewId);
            this.ReviewObjectivesCreationStatus = CreationStatus.None;
        }
    }
}
