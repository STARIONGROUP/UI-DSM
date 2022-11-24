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

    using ReactiveUI;

    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.Services.Administration.CometService;
    using UI_DSM.Client.Services.Administration.ParticipantService;
    using UI_DSM.Client.Services.ReviewService;
    using UI_DSM.Client.Services.ThingService;
    using UI_DSM.Client.Services.ViewProviderService;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Wrappers;

    /// <summary>
    ///     View model for the <see cref="Client.Pages.NormalUser.ReviewTaskPage.ReviewTaskPage" />
    /// </summary>
    public class ReviewTaskPageViewModel : ReactiveObject, IReviewTaskPageViewModel
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
        ///     Backing field for <see cref="SelectedView" />
        /// </summary>
        private ViewWrapper selectedView;

        /// <summary>
        ///     Backing field for <see cref="ViewSelectorVisible" />
        /// </summary>
        private bool viewSelectorVisible;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewTaskPageViewModel" /> class.
        /// </summary>
        /// <param name="thingService">The <see cref="IThingService" /></param>
        /// <param name="reviewService">The <see cref="IReviewService" /></param>
        /// <param name="viewProviderService">The <see cref="IViewProviderService" /></param>
        /// <param name="participantService">The <see cref="IParticipantService" /></param>
        public ReviewTaskPageViewModel(IThingService thingService, IReviewService reviewService,
            IViewProviderService viewProviderService, IParticipantService participantService)
        {
            this.thingService = thingService;
            this.reviewService = reviewService;
            this.viewProviderService = viewProviderService;
            this.participantService = participantService;
        }

        /// <summary>
        ///     The current <see cref="Participant" />
        /// </summary>
        public Participant Participant { get; set; }

        /// <summary>
        ///     The currently selected <see cref="ViewWrapper" />
        /// </summary>
        public ViewWrapper SelectedView
        {
            get => this.selectedView;
            set => this.RaiseAndSetIfChanged(ref this.selectedView, value);
        }

        /// <summary>
        ///     Value indicating if the baseView needs to be initialized
        /// </summary>
        public bool ShouldInitializeBaseView { get; set; }

        /// <summary>
        ///     The current instance of <see cref="BaseView" />
        /// </summary>
        public BaseView CurrentBaseViewInstance { get; set; }

        /// <summary>
        ///     A collection of all available <see cref="View" />
        /// </summary>
        public List<ViewWrapper> AvailableViews { get; private set; } = new();

        /// <summary>
        ///     Value indicating if the view selector is visible or not
        /// </summary>
        public bool ViewSelectorVisible
        {
            get => this.viewSelectorVisible;
            set => this.RaiseAndSetIfChanged(ref this.viewSelectorVisible, value);
        }

        /// <summary>
        ///     The current <see cref="View" />
        /// </summary>
        public View CurrentView { get; private set; }

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
            this.AvailableViews.Clear();
            var review = await this.reviewService.GetReviewOfProject(projectId, reviewId, 3);
            var reviewObjectiveInsideReview = review?.ReviewObjectives.FirstOrDefault(x => x.Id == reviewObjectiveId);

            var reviewTaskInsideObjective = reviewObjectiveInsideReview?.ReviewTasks.FirstOrDefault(x => x.Id == reviewTaskId);

            if (reviewTaskInsideObjective != null)
            {
                this.Participant = await this.participantService.GetCurrentParticipant(projectId);
                this.ReviewTask = reviewTaskInsideObjective;
                this.Things = await this.thingService.GetThings(projectId, review.Artifacts.OfType<Model>().Select(x => x.Id));

                this.ReviewObjective = reviewObjectiveInsideReview;
                this.AvailableViews.Add(new ViewWrapper(this.ReviewTask.MainView));

                if (this.ReviewTask.AdditionalView != View.None)
                {
                    this.AvailableViews.Add(new ViewWrapper(this.ReviewTask.AdditionalView));
                }

                this.SelectedView = this.AvailableViews[0];
                this.UpdateView(this.SelectedView.View);
            }
        }

        /// <summary>
        ///     Updates the current view
        /// </summary>
        /// <param name="newView">The new view</param>
        /// <param name="shouldUpdateAvailableViews">Indicate if the <see cref="AvailableViews" /> should also be updated</param>
        public void UpdateView(View newView, bool shouldUpdateAvailableViews = false)
        {
            this.CurrentView = newView;
            this.ShouldInitializeBaseView = true;

            if (shouldUpdateAvailableViews)
            {
                this.ComputeAvailableViews();
            }

            this.CurrentBaseView = this.viewProviderService.GetViewType(this.CurrentView);
        }

        /// <summary>
        ///     Gets the main related <see cref="View" />
        /// </summary>
        /// <returns>The main related <see cref="View" /></returns>
        public View GetMainRelatedView()
        {
            return this.CurrentView == this.ReviewTask.MainView || this.CurrentView == this.ReviewTask.AdditionalView
                ? this.ReviewTask.OptionalView
                : this.ReviewTask.MainView;
        }

        /// <summary>
        ///     Gets a collection of related <see cref="View" />s
        /// </summary>
        /// <returns>The collection of related <see cref="View" />s</returns>
        public List<View> GetOtherRelatedViews()
        {
            if (this.ReviewObjective == null)
            {
                return new List<View>();
            }

            var views = new List<View>(this.ReviewObjective.RelatedViews);
            views.RemoveAll(x => x == this.CurrentView);
            return views;
        }

        /// <summary>
        ///     Reset this view model
        /// </summary>
        public void Reset()
        {
            this.ReviewObjective = null;
            this.ReviewTask = null;
            this.Things = Enumerable.Empty<Thing>();
            this.AvailableViews.Clear();
            this.Participant = null;
            this.SelectedView = null;
            this.ShouldInitializeBaseView = false;
            this.CurrentView = View.None;
            this.CurrentBaseView = null;
            this.CurrentBaseViewInstance = null;
        }

        /// <summary>
        ///     Updates the <see cref="AvailableViews" /> collections
        /// </summary>
        private void ComputeAvailableViews()
        {
            this.AvailableViews.Clear();

            if (this.CurrentView == this.ReviewTask.MainView)
            {
                this.AvailableViews.Add(new ViewWrapper(this.ReviewTask.MainView));

                if (this.ReviewTask.AdditionalView != View.None)
                {
                    this.AvailableViews.Add(new ViewWrapper(this.ReviewTask.AdditionalView));
                }
            }
            else
            {
                this.AvailableViews.Add(new ViewWrapper(this.CurrentView));
            }

            this.SelectedView = this.AvailableViews[0];
        }
    }
}
