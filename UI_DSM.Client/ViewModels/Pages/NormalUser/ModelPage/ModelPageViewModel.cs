// --------------------------------------------------------------------------------------------------------
// <copyright file="ModelPageViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Pages.NormalUser.ModelPage
{
    using CDP4Common.CommonData;

    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.Components.App.AnnotationLinker;
    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.Services.Administration.ParticipantService;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.Services.ReviewService;
    using UI_DSM.Client.Services.ThingService;
    using UI_DSM.Client.Services.ViewProviderService;
    using UI_DSM.Client.ViewModels.App.AnnotationLinker;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="Client.Pages.NormalUser.ModelPage.ModelPage" /> page
    /// </summary>
    public class ModelPageViewModel : ReactiveObject, IModelPageViewModel
    {
        /// <summary>
        ///     The <see cref="IParticipantService" />
        /// </summary>
        private readonly IParticipantService participantService;

        /// <summary>
        ///     The <see cref="IReviewItemService" />
        /// </summary>
        private readonly IReviewItemService reviewItemService;

        /// <summary>
        ///     The <see cref="IReviewService" />
        /// </summary>
        private readonly IReviewService reviewService;

        /// <summary>
        ///     The <see cref="IThingService" />
        /// </summary>
        private readonly IThingService thingService;

        /// <summary>
        ///     The <see cref="IViewProviderService" />
        /// </summary>
        private readonly IViewProviderService viewProviderService;

        /// <summary>
        ///     Backing field for <see cref="IsLinkerVisible" />
        /// </summary>
        private bool isLinkerVisible;

        /// <summary>
        ///     The current <see cref="Project" /> id
        /// </summary>
        private Guid projectId;

        /// <summary>
        ///     The <see cref="Guid" /> of the current <see cref="Review" />
        /// </summary>
        private Guid reviewId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ModelPageViewModel" /> class.
        /// </summary>
        /// <param name="thingService">The <see cref="IThingService" /></param>
        /// <param name="viewProviderService">The <see cref="IViewProviderService" /></param>
        /// <param name="reviewService">The <see cref="IReviewService" /></param>
        /// <param name="participantService">The <see cref="IParticipantService" /></param>
        /// <param name="annotationLinkerViewModel">The <see cref="IAnnotationLinkerViewModel" /></param>
        /// <param name="reviewItemService">The <see cref="IReviewItemService" /></param>
        public ModelPageViewModel(IThingService thingService, IViewProviderService viewProviderService, IReviewService reviewService,
            IParticipantService participantService, IAnnotationLinkerViewModel annotationLinkerViewModel, IReviewItemService reviewItemService)
        {
            this.thingService = thingService;
            this.viewProviderService = viewProviderService;
            this.reviewService = reviewService;
            this.participantService = participantService;
            this.AnnotationLinkerViewModel = annotationLinkerViewModel;
            this.reviewItemService = reviewItemService;
            this.AnnotationLinkerViewModel.OnSubmit = new EventCallbackFactory().Create(this, this.LinkAnnotation);
            this.OnLinkCallback = new EventCallbackFactory().Create<Comment>(this, this.OpenLinkComponent);
        }

        /// <summary>
        ///     Value indicating if the <see cref="AnnotationLinker" /> is visible
        /// </summary>
        public bool IsLinkerVisible
        {
            get => this.isLinkerVisible;
            set => this.RaiseAndSetIfChanged(ref this.isLinkerVisible, value);
        }

        /// <summary>
        ///     The <see cref="IAnnotationLinkerViewModel" />
        /// </summary>
        public IAnnotationLinkerViewModel AnnotationLinkerViewModel { get; }

        /// <summary>
        ///     The current <see cref="Participant" />
        /// </summary>
        public Participant Participant { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback{TValue}" /> for linking a <see cref="Comment" /> on other element
        /// </summary>
        public EventCallback<Comment> OnLinkCallback { get; }

        /// <summary>
        ///     The current <see cref="Type" /> for the <see cref="GenericBaseView{TViewModel}" />
        /// </summary>
        public Type CurrentBaseView { get; private set; }

        /// <summary>
        ///     Value indicating if the baseView needs to be initialized
        /// </summary>
        public bool ShouldInitializeBaseView { get; set; }

        /// <summary>
        ///     The current instance of <see cref="BaseView" />
        /// </summary>
        public BaseView CurrentBaseViewInstance { get; set; }

        /// <summary>
        ///     A collection of <see cref="Thing" />
        /// </summary>
        public IEnumerable<Thing> Things { get; private set; }

        /// <summary>
        ///     The current <see cref="View" />
        /// </summary>
        public View CurrentView { get; private set; }

        /// <summary>
        ///     Initializes this view model properties
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <param name="reviewGuid"></param>
        /// <param name="modelGuid"></param>
        /// <param name="selectedView">The selected view</param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task InitializeProperties(Guid projectGuid, Guid reviewGuid, Guid modelGuid, View selectedView)
        {
            this.projectId = projectGuid;
            this.reviewId = reviewGuid;
            var review = await this.reviewService.GetReviewOfProject(projectGuid, reviewGuid);
            var model = review?.Artifacts.OfType<Model>().FirstOrDefault(x => x.Id == modelGuid);
            this.CurrentBaseViewInstance = null;

            if (model != null)
            {
                this.CurrentView = selectedView;
                this.Things = await this.thingService.GetThings(projectGuid, model);
                this.CurrentBaseView = this.viewProviderService.GetViewType(selectedView);
                this.ShouldInitializeBaseView = true;
                this.Participant = await this.participantService.GetCurrentParticipant(projectGuid);
            }
        }

        /// <summary>
        ///     Get all accessible views
        /// </summary>
        /// <returns>A collection of <see cref="View" /></returns>
        public List<View> GetAccessibleViews()
        {
            var allViews = new List<View>
            {
                View.RequirementBreakdownStructureView,
                View.RequirementVerificationControlView,
                View.ProductBreakdownStructureView,
                View.TrlView,
                View.FunctionalBreakdownStructureView,
                View.RequirementTraceabilityToRequirementView,
                View.RequirementTraceabilityToProductView,
                View.RequirementTraceabilityToFunctionView,
                View.FunctionalTraceabilityToProductView,
                View.InterfaceView,
                View.DocumentBased
            };

            allViews.Remove(this.CurrentView);
            return allViews;
        }

        /// <summary>
        ///     Links the <see cref="Annotation" /> to all selected items
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task LinkAnnotation()
        {
            var linkResult = await this.reviewItemService.LinkItemsToAnnotation(this.projectId, this.reviewId,
                this.AnnotationLinkerViewModel.CurrentAnnotation.Id,
                this.AnnotationLinkerViewModel.SelectedItems.OfType<IHaveThingRowViewModel>().Select(x => x.ThingId));

            if (linkResult.IsRequestSuccessful)
            {
                await this.CurrentBaseViewInstance.UpdateAnnotatableRows(linkResult.Entities.ToList<AnnotatableItem>());
                this.IsLinkerVisible = false;
            }
        }

        /// <summary>
        ///     Opens the link component to link the current <see cref="Comment" /> to other element
        /// </summary>
        /// <param name="comment">The <see cref="Comment" /></param>
        private void OpenLinkComponent(Comment comment)
        {
            if (this.CurrentBaseViewInstance != null)
            {
                this.AnnotationLinkerViewModel.InitializesViewModel(this.CurrentBaseViewInstance.GetAvailablesRows(), comment);
                this.IsLinkerVisible = true;
            }
        }
    }
}
