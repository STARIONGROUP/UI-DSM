// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewTaskPageTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Pages.NormalUser.ReviewTaskPage
{
    using Bunit;

    using CDP4Common.CommonData;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components.App.RelatedViews;
    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.Pages.NormalUser.ReviewTaskPage;
    using UI_DSM.Client.Services.Administration.ParticipantService;
    using UI_DSM.Client.Services.AnnotationService;
    using UI_DSM.Client.Services.ReplyService;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.Services.ReviewService;
    using UI_DSM.Client.Services.ReviewTaskService;
    using UI_DSM.Client.Services.ThingService;
    using UI_DSM.Client.Services.ViewProviderService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.App.AnnotationLinker;
    using UI_DSM.Client.ViewModels.App.Comments;
    using UI_DSM.Client.ViewModels.App.Filter;
    using UI_DSM.Client.ViewModels.App.OptionChooser;
    using UI_DSM.Client.ViewModels.App.SelectedItemCard;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Client.ViewModels.Pages.NormalUser.ReviewTaskPage;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ReviewTaskPageTestFixture
    {
        private TestContext context;
        private IReviewTaskPageViewModel viewModel;
        private Mock<IReviewService> reviewService;
        private Mock<IReviewItemService> reviewItemService;
        private Mock<IThingService> thingService;
        private Mock<IParticipantService> participantService;
        private Mock<IReviewTaskService> reviewTaskService;
        private IAnnotationLinkerViewModel annotationLinker;
        private IViewProviderService viewProviderService;
        private ISelectedItemCardViewModel selectedItemCard;
        private Guid projectId;
        private Guid reviewId;
        private Guid reviewObjectiveId;
        private Guid reviewTaskId;
        private Mock<IAnnotationService> annotationService;
        private Mock<IReplyService> replyService;

        [SetUp]
        public void Setup()
        {
            this.reviewService = new Mock<IReviewService>();
            this.reviewItemService = new Mock<IReviewItemService>();
            this.thingService = new Mock<IThingService>();
            this.participantService = new Mock<IParticipantService>();
            this.annotationService = new Mock<IAnnotationService>();
            this.reviewTaskService = new Mock<IReviewTaskService>();
            this.replyService = new Mock<IReplyService>();

            this.participantService.Setup(x => x.GetCurrentParticipant(It.IsAny<Guid>()))
                .ReturnsAsync(new Participant(Guid.NewGuid())
                {
                    Role = new Role(Guid.NewGuid())
                    {
                        AccessRights = { AccessRight.ReviewTask }
                    }
                });

            this.viewProviderService = new ViewProviderService();
            this.context = new TestContext();
            this.annotationLinker = new AnnotationLinkerViewModel();

            this.viewModel = new ReviewTaskPageViewModel(this.thingService.Object, this.reviewService.Object, this.viewProviderService,
                this.participantService.Object, this.reviewItemService.Object, this.reviewTaskService.Object, this.annotationLinker);

            this.selectedItemCard = new SelectedItemCardViewModel();
            this.projectId = Guid.NewGuid();
            this.reviewId = Guid.NewGuid();
            this.reviewObjectiveId = Guid.NewGuid();
            this.reviewTaskId = Guid.NewGuid();
            this.context.Services.AddSingleton(this.viewModel);
            this.context.Services.AddSingleton(this.selectedItemCard);
            this.context.Services.AddSingleton(this.reviewItemService.Object);
            this.context.Services.AddSingleton(this.annotationService.Object);
            this.context.Services.AddSingleton(this.replyService.Object);
            this.context.Services.AddTransient<ICommentsViewModel, CommentsViewModel>();
            this.context.Services.AddTransient<ISelectedItemCardViewModel, SelectedItemCardViewModel>();

            IRequirementBreakdownStructureViewViewModel breakdown = 
                new RequirementBreakdownStructureViewViewModel(new Mock<IReviewItemService>().Object, new FilterViewModel());

            IProductBreakdownStructureViewViewModel productBreakdown =
                new ProductBreakdownStructureViewViewModel(new Mock<IReviewItemService>().Object, new FilterViewModel(), new OptionChooserViewModel());

            IInterfaceViewViewModel interfaceView =
                new InterfaceViewViewModel(new Mock<IReviewItemService>().Object, new FilterViewModel());

            this.context.Services.AddSingleton(breakdown);
            this.context.Services.AddSingleton(productBreakdown);
            this.context.Services.AddSingleton(interfaceView);
            this.context.ConfigureDevExpressBlazor();
            ViewProviderService.RegisterViews();
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifyInitialization()
        {
            var renderer = this.context.RenderComponent<ReviewTaskPage>(parameters =>
            {
                parameters.Add(p => p.ProjectId, this.projectId.ToString());
                parameters.Add(p => p.ReviewId, this.reviewId.ToString());
                parameters.Add(p => p.ReviewObjectiveId, this.reviewObjectiveId.ToString());
                parameters.Add(p => p.ReviewTaskId, this.reviewTaskId.ToString());
            });

            Assert.That(renderer.Instance.BaseView, Is.Null);

            var review = new Review(this.reviewId)
            {
                ReviewObjectives = { new ReviewObjective(Guid.NewGuid()) },
                Artifacts = 
                {
                    new Model(Guid.NewGuid())
                    {
                        IterationId = Guid.NewGuid(),
                        ModelName = "Envision - Iteration 4"
                    },
                    new Model(Guid.NewGuid())
                    {
                        IterationId = Guid.NewGuid(),
                        ModelName = "Envision - Iteration 5"
                    }
                }
            };

            this.reviewService.Setup(x => x.GetReviewOfProject(this.projectId, this.reviewId, It.IsAny<int>()))
                .ReturnsAsync(review);

            await this.viewModel.OnInitializedAsync(this.projectId, this.reviewId, this.reviewObjectiveId, this.reviewTaskId);
            Assert.That(this.viewModel.ReviewObjective, Is.Null);

            var reviewObjective = new ReviewObjective(this.reviewObjectiveId)
            {
                ReviewTasks = { new ReviewTask(Guid.NewGuid()) },
                RelatedViews =
                {
                    View.InterfaceView
                }
            };

            review.ReviewObjectives.Add(reviewObjective);

            await this.viewModel.OnInitializedAsync(this.projectId, this.reviewId, this.reviewObjectiveId, this.reviewTaskId);
            Assert.That(this.viewModel.ReviewTask, Is.Null);

            var reviewTask = new ReviewTask(this.reviewTaskId)
            {
                MainView = View.BudgetView
            };

            reviewObjective.ReviewTasks.Add(reviewTask);

            this.thingService.Setup(x => x.GetThings(this.projectId, It.IsAny<Model>(),
                ClassKind.Iteration)).ReturnsAsync(new List<Thing>());

            await this.viewModel.OnInitializedAsync(this.projectId, this.reviewId, this.reviewObjectiveId, this.reviewTaskId);
            
            renderer.Render();

            Assert.That(renderer.Instance.BaseView!.Instance, Is.Null);

            reviewTask.MainView = View.RequirementBreakdownStructureView;
            reviewTask.AdditionalView = View.RequirementVerificationControlView;
            reviewTask.OptionalView = View.ProductBreakdownStructureView;
            await this.viewModel.OnInitializedAsync(this.projectId, this.reviewId, this.reviewObjectiveId, this.reviewTaskId);
            renderer.Render();

            Assert.That(renderer.Instance.BaseView, Is.Not.Null);

            this.viewModel.ViewSelectorVisible = true;
            this.viewModel.SelectedView = this.viewModel.AvailableViews[1];
            this.viewModel.ViewSelectorVisible = false;

            var relatedViews = renderer.FindComponent<RelatedViews>();
            await renderer.InvokeAsync(async () => await relatedViews.Instance.OnViewSelect.InvokeAsync(relatedViews.Instance.MainRelatedView));

            Assert.That(renderer.Instance.BaseView.Type, Is.EqualTo(typeof(ProductBreakdownStructureView)));

            this.viewModel.ModelSelectorVisible = true;
            Assert.That(this.viewModel.CurrentModel, Is.EqualTo(review.Artifacts[1]));

            this.viewModel.SelectedModel = this.viewModel.AvailableModels[1];
            Assert.That(this.viewModel.ModelSelectorVisible, Is.True);

            this.viewModel.SelectedModel = this.viewModel.AvailableModels[0];
            
            Assert.Multiple(() =>
            {
                Assert.That(this.viewModel.ModelSelectorVisible, Is.False);
                Assert.That(this.viewModel.CurrentModel, Is.EqualTo(this.viewModel.AvailableModels[0]));
            });

            var reviewItem = new ReviewItem(Guid.NewGuid());

            var thing = new HyperLink()
            {
                Iid = Guid.NewGuid()
            };

            var hyperLink = new HyperLinkRowViewModel(thing, null);

            renderer.Instance.SelectedItem = hyperLink;
            await renderer.InvokeAsync(() => renderer.Instance.SelectedItemCard.MarkAsReviewed.InvokeAsync(hyperLink));
            Assert.That(this.viewModel.ConfirmCancelDialog.IsVisible, Is.True);
            await renderer.InvokeAsync(this.viewModel.ConfirmCancelDialog.OnCancel.InvokeAsync);
            
            Assert.Multiple(() =>
            {
                Assert.That(this.viewModel.ConfirmCancelDialog.IsVisible, Is.False);
                Assert.That(hyperLink.ReviewItem, Is.Null);
            });

            await renderer.InvokeAsync(() => renderer.Instance.SelectedItemCard.MarkAsReviewed.InvokeAsync(hyperLink));

            this.reviewItemService.Setup(x => x.CreateReviewItem(this.projectId, this.reviewId, thing.Iid))
                .ReturnsAsync(EntityRequestResponse<ReviewItem>.Success(reviewItem));

            this.reviewItemService.Setup(x => x.UpdateReviewItem(this.projectId, this.reviewId, reviewItem))
                .ReturnsAsync(EntityRequestResponse<ReviewItem>.Success(new ReviewItem(reviewItem.Id) { IsReviewed = true, ThingId = thing.Iid}));

            await renderer.InvokeAsync(this.viewModel.ConfirmCancelDialog.OnConfirm.InvokeAsync);

            Assert.Multiple(() =>
            {
                Assert.That(this.viewModel.ConfirmCancelDialog.IsVisible, Is.False);
                Assert.That(hyperLink.ReviewItem, Is.Not.Null);
                Assert.That(hyperLink.ReviewItem.IsReviewed, Is.True);
            });

            var doneButton = renderer.Find("#doneButton");
            doneButton.Click();

            Assert.That(this.viewModel.DoneConfirmCancelPopup.IsVisible, Is.True);

            await renderer.InvokeAsync(() => this.viewModel.DoneConfirmCancelPopup.OnConfirm.InvokeAsync()); 
            
            Assert.Multiple(() =>
            {
                Assert.That(this.viewModel.ReviewTask!.Status, Is.EqualTo(StatusKind.Done));
                Assert.That(this.viewModel.DoneConfirmCancelPopup.IsVisible, Is.False);
            });

            var undoneButton = renderer.Find("#undoneButton");
            undoneButton.Click();

            Assert.That(this.viewModel.DoneConfirmCancelPopup.IsVisible, Is.True);

            await renderer.InvokeAsync(() => this.viewModel.DoneConfirmCancelPopup.OnConfirm.InvokeAsync());
            
            Assert.Multiple(() =>
            {
                Assert.That(this.viewModel.ReviewTask!.Status, Is.EqualTo(StatusKind.Open));
                Assert.That(this.viewModel.DoneConfirmCancelPopup.IsVisible, Is.False);
            });

            var comment = new Comment(Guid.NewGuid());
            await renderer.InvokeAsync(() => this.viewModel.OnLinkCallback.InvokeAsync(comment));
            Assert.That(this.viewModel.IsLinkerVisible, Is.True);

            this.reviewItemService.Setup(x => x.LinkItemsToAnnotation(this.projectId, this.reviewId, comment.Id, It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(EntitiesRequestResponses<ReviewItem>.Fail(new List<string>()));

            await renderer.InvokeAsync(this.viewModel.AnnotationLinkerViewModel.OnSubmit.InvokeAsync);
            Assert.That(this.viewModel.IsLinkerVisible, Is.True);

            this.reviewItemService.Setup(x => x.LinkItemsToAnnotation(this.projectId, this.reviewId, comment.Id, It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(EntitiesRequestResponses<ReviewItem>.Success(new List<ReviewItem>()));

            await renderer.InvokeAsync(this.viewModel.AnnotationLinkerViewModel.OnSubmit.InvokeAsync);
            Assert.That(this.viewModel.IsLinkerVisible, Is.False);
        }
    }
}
