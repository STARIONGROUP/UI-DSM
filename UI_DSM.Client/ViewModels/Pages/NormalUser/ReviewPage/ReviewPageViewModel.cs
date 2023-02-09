// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewPageViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Pages.NormalUser.ReviewPage
{
    using ReactiveUI;

    using UI_DSM.Client.Services.Administration.ParticipantService;
    using UI_DSM.Client.Services.AnnotationService;
    using UI_DSM.Client.Services.ReviewObjectiveService;
    using UI_DSM.Client.Services.ReviewService;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="ReviewPageViewModel" /> page
    /// </summary>
    public class ReviewPageViewModel : ReactiveObject, IReviewPageViewModel
    {
        /// <summary>
        ///     The <see cref="IParticipantService" />
        /// </summary>
        private readonly IParticipantService participantService;

        /// <summary>
        ///     The <see cref="IReviewService" />
        /// </summary>
        private readonly IReviewService reviewService;

        /// <summary>
        ///     The <see cref="IReviewObjectiveService" />
        /// </summary>
        private readonly IReviewObjectiveService reviewObjectiveService;

        /// <summary>
        /// The <see cref="IAnnotationService"/>
        /// </summary>
        private readonly IAnnotationService annotationService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewPageViewModel" /> class.
        /// </summary>
        /// <param name="reviewService">The <see cref="IReviewService" /></param>
        /// <param name="reviewObjectiveViewModel"></param>
        /// <param name="reviewObjectiveCreationViewMode"></param>
        /// <param name="participantService">The <see cref="IParticipantService" /></param>
        /// <param name="reviewObjectiveService">The <see cref="IReviewObjectiveService" /></param>
        /// <param name="annotationService">The <see cref="IAnnotationService"/></param>
        public ReviewPageViewModel(IReviewService reviewService, IReviewObjectiveViewModel reviewObjectiveViewModel,
            IReviewObjectiveCreationViewModel reviewObjectiveCreationViewMode, IParticipantService participantService, 
            IReviewObjectiveService reviewObjectiveService, IAnnotationService annotationService)
        {
            this.reviewService = reviewService;
            this.participantService = participantService;
            this.reviewObjectiveService = reviewObjectiveService;
            this.ReviewObjectiveViewModel = reviewObjectiveViewModel;
            this.ReviewObjectiveCreationViewModel = reviewObjectiveCreationViewMode;
            this.annotationService = annotationService;
        }

        /// <summary>
        ///     The <see cref="IReviewObjectiveCreationViewModel" /> for the <see cref="ReviewObjective" /> component
        /// </summary>
        public IReviewObjectiveCreationViewModel ReviewObjectiveCreationViewModel { get; }

        /// <summary>
        ///     The <see cref="IReviewObjectiveViewModel" /> for the <see cref="ReviewObjective" /> component
        /// </summary>
        public IReviewObjectiveViewModel ReviewObjectiveViewModel { get; }

        /// <summary>
        ///     Gets the <see cref="IErrorMessageViewModel" />
        /// </summary>
        public IErrorMessageViewModel ErrorMessageViewModel { get; } = new ErrorMessageViewModel();

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <param name="projectGuid">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reviewGuid">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        public async Task OnInitializedAsync(Guid projectGuid, Guid reviewGuid)
        {
            var reviewResponse = await this.reviewService.GetReviewOfProject(projectGuid, reviewGuid, 1);

            if (reviewResponse != null)
            {
                this.ReviewObjectiveViewModel.Participant = await this.participantService.GetCurrentParticipant(projectGuid);
                this.ReviewObjectiveViewModel.Review = reviewResponse;
                this.ReviewObjectiveViewModel.Project = new Project { Id = projectGuid };
                this.ReviewObjectiveViewModel.CommentsAndTasks = await this.reviewObjectiveService.GetOpenTasksAndComments(projectGuid, reviewGuid);
                this.ReviewObjectiveViewModel.Annotations = await this.annotationService.GetAnnotationsForReview(projectGuid, reviewGuid);
            }
        }
    }
}
