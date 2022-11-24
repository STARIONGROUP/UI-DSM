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
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.Services.ReviewService;
    using UI_DSM.Client.Services.ThingService;
    using UI_DSM.Client.Services.ViewProviderService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.App.Filter;
    using UI_DSM.Client.ViewModels.App.SelectedItemCard;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Pages.NormalUser.ReviewTaskPage;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ReviewTaskPageTestFixture
    {
        private TestContext context;
        private IReviewTaskPageViewModel viewModel;
        private Mock<IReviewService> reviewService;
        private Mock<IThingService> thingService;
        private Mock<IParticipantService> participantService;
        private IViewProviderService viewProviderService;
        private ISelectedItemCardViewModel selectedItemCard;
        private Guid projectId;
        private Guid reviewId;
        private Guid reviewObjectiveId;
        private Guid reviewTaskId;

        [SetUp]
        public void Setup()
        {
            this.reviewService = new Mock<IReviewService>();
            this.thingService = new Mock<IThingService>();
            this.participantService = new Mock<IParticipantService>();

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
            this.viewModel = new ReviewTaskPageViewModel(this.thingService.Object, this.reviewService.Object, this.viewProviderService, this.participantService.Object);
            this.selectedItemCard = new SelectedItemCardViewModel();
            this.projectId = Guid.NewGuid();
            this.reviewId = Guid.NewGuid();
            this.reviewObjectiveId = Guid.NewGuid();
            this.reviewTaskId = Guid.NewGuid();
            this.context.Services.AddSingleton(this.viewModel);
            this.context.Services.AddSingleton(this.selectedItemCard);
            
            IRequirementBreakdownStructureViewViewModel breakdown = 
                new RequirementBreakdownStructureViewViewModel(new Mock<IReviewItemService>().Object, new FilterViewModel());

            this.context.Services.AddSingleton(breakdown);
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
                Artifacts = { new Model(Guid.NewGuid()) }
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

            this.thingService.Setup(x => x.GetThings(this.projectId, It.IsAny<IEnumerable<Guid>>(),
                ClassKind.Iteration)).ReturnsAsync(new List<Thing>());

            await this.viewModel.OnInitializedAsync(this.projectId, this.reviewId, this.reviewObjectiveId, this.reviewTaskId);
            
            renderer.Render();

            Assert.That(renderer.Instance.BaseView, Is.Null);

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
            await renderer.InvokeAsync(async () => await relatedViews.Instance.OnViewSelect.InvokeAsync(relatedViews.Instance.OtherRelatedViews[0]));
            Assert.That(this.viewModel.CurrentBaseView, Is.EqualTo(typeof(InterfaceView)));
        }
    }
}
