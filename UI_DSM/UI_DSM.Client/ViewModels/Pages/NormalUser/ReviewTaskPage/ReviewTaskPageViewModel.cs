// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewTaskPageViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Pages.NormalUser.ReviewTaskPage
{
    using CDP4Common.CommonData;

    using DynamicData;

    using ReactiveUI;

    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.Services.Administration.CometService;
    using UI_DSM.Client.Services.ReviewService;
    using UI_DSM.Client.Services.ThingService;
    using UI_DSM.Client.Services.ViewProviderService;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="ReviewTaskPage" />
    /// </summary>
    public class ReviewTaskPageViewModel : ReactiveObject, IReviewTaskPageViewModel
    {
        /// <summary>
        ///     The <see cref="IReviewService" />
        /// </summary>
        private readonly IReviewService reviewService;

        /// <summary>
        ///     The <see cref="ICometService" />
        /// </summary>
        private readonly IThingService thingService;

        /// <summary>
        ///     The <see cref="IViewProviderService" />
        /// </summary>
        private readonly IViewProviderService viewProviderService;

        /// <summary>
        ///     Backing field for <see cref="CurrentBaseView" />
        /// </summary>
        private Type currentBaseView;

        /// <summary>
        ///     Backing field for <see cref="ReviewObjective" />
        /// </summary>
        private ReviewObjective reviewObjective;

        /// <summary>
        ///     Backing field for <see cref="ReviewTask" />
        /// </summary>
        private ReviewTask reviewTask;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewTaskPageViewModel" /> class.
        /// </summary>
        /// <param name="thingService">The <see cref="IThingService" /></param>
        /// <param name="reviewService">The <see cref="IReviewService" /></param>
        /// <param name="viewProviderService">The <see cref="IViewProviderService" /></param>
        public ReviewTaskPageViewModel(IThingService thingService, IReviewService reviewService,
            IViewProviderService viewProviderService)
        {
            this.thingService = thingService;
            this.reviewService = reviewService;
            this.viewProviderService = viewProviderService;
        }

        /// <summary>
        ///     A collection of <see cref="Comment" />
        /// </summary>
        public SourceList<Comment> Comments { get; set; } = new();

        /// <summary>
        ///     The current <see cref="ReviewObjective" />
        /// </summary>
        public ReviewObjective ReviewObjective
        {
            get => this.reviewObjective;
            set => this.RaiseAndSetIfChanged(ref this.reviewObjective, value);
        }

        /// <summary>
        ///     A collection of <see cref="Thing" /> that are suitable for the current <see cref="ReviewTask" />
        /// </summary>
        public IEnumerable<Thing> Things { get; set; }

        /// <summary>
        ///     The current <see cref="Type" /> for the <see cref="GenericBaseView{TViewModel}" />
        /// </summary>
        public Type CurrentBaseView
        {
            get => this.currentBaseView;
            set => this.RaiseAndSetIfChanged(ref this.currentBaseView, value);
        }

        /// <summary>
        ///     The <see cref="IReviewTaskPageViewModel.ReviewTask" />
        /// </summary>
        public ReviewTask ReviewTask
        {
            get => this.reviewTask;
            set => this.RaiseAndSetIfChanged(ref this.reviewTask, value);
        }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="reviewObjectiveId">The <see cref="Guid" /> of the <see cref="ReviewObjective" /></param>
        /// <param name="reviewTaskId">The <see cref="Guid" /> of the <see cref="IReviewTaskPageViewModel.ReviewTask" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task OnInitializedAsync(Guid projectId, Guid reviewId, Guid reviewObjectiveId, Guid reviewTaskId)
        {
            var review = await this.reviewService.GetReviewOfProject(projectId, reviewId, 3);
            var reviewObjectiveInsideReview = review?.ReviewObjectives.FirstOrDefault(x => x.Id == reviewObjectiveId);

            var reviewTaskInsideObjective = reviewObjectiveInsideReview?.ReviewTasks.FirstOrDefault(x => x.Id == reviewTaskId);

            if (reviewTaskInsideObjective != null)
            {
                this.ReviewTask = reviewTaskInsideObjective;

                this.Things = await this.thingService.GetThingsByView(projectId, review.Artifacts.OfType<Model>().Select(x => x.Id)
                    , this.ReviewTask.MainView);

                this.ReviewObjective = reviewObjectiveInsideReview;
                this.CurrentBaseView = this.viewProviderService.GetViewType(this.ReviewTask.MainView);
            }
        }
    }
}
