// --------------------------------------------------------------------------------------------------------
// <copyright file="ModelPageTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Pages.NormalUser.ModelPage
{
    using Bunit;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components.App.RelatedViews;
    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.Pages.NormalUser.ModelPage;
    using UI_DSM.Client.Services.ArtifactService;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.Services.ThingService;
    using UI_DSM.Client.Services.ViewProviderService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.App.Filter;
    using UI_DSM.Client.ViewModels.App.SelectedItemCard;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Pages.NormalUser.ModelPage;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ModelPageTestFixture
    {
        private TestContext context;
        private IModelPageViewModel viewModel;
        private Mock<IThingService> thingService;
        private IViewProviderService viewProviderService;
        private Mock<IArtifactService> artifactService;
        private Mock<IReviewItemService> reviewItemService;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.thingService = new Mock<IThingService>();
            this.viewProviderService = new ViewProviderService();
            this.reviewItemService = new Mock<IReviewItemService>();
            this.artifactService = new Mock<IArtifactService>();
            this.viewModel = new ModelPageViewModel(this.thingService.Object, this.viewProviderService, this.artifactService.Object);
            this.context.Services.AddSingleton(this.viewModel);
            this.context.Services.AddSingleton(this.reviewItemService.Object);
            this.context.Services.AddTransient<ISelectedItemCardViewModel, SelectedItemCardViewModel>();

            IRequirementTraceabilityToFunctionViewViewModel traceability =
                new RequirementTraceabilityToFunctionViewViewModel(this.reviewItemService.Object, new FilterViewModel(), new FilterViewModel());

            this.context.Services.AddSingleton(traceability);
            this.context.ConfigureDevExpressBlazor();
            ViewProviderService.RegisterViews();
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifyPage()
        {
            var projectId = Guid.NewGuid();
            var model = new Model(Guid.NewGuid());

            this.artifactService.Setup(x => x.GetArtifactsOfProject(projectId, 0)).ReturnsAsync(new List<Artifact>
            {
                model
            });

            var requirement = new Requirement()
            {
                Name = "Req",
                ShortName = "req",
                Iid = Guid.NewGuid(),
                Owner = new DomainOfExpertise()
                {
                    Iid = Guid.NewGuid(),
                    ShortName = "SYS"
                }
            };

            var specification = new RequirementsSpecification()
            {
                Name = "spec",
                ShortName = "spec",
                Requirement = { requirement }
            };

            this.thingService.Setup(x => x.GetThings(projectId, model, ClassKind.Iteration))
                .ReturnsAsync(new List<Thing> { specification });

            this.reviewItemService.Setup(x => x.GetReviewItemsForThings(It.IsAny<Guid>(), It.IsAny<Guid>(),
                    It.IsAny<IEnumerable<Guid>>(), 0))
                .ReturnsAsync(new List<ReviewItem>());

            var renderer = this.context.RenderComponent<ModelPage>(parameters =>
            {
                parameters.Add(p => p.ProjectId, projectId.ToString());
                parameters.Add(p => p.ModelId, model.Id.ToString());
                parameters.Add(p => p.SelectedView, View.RequirementTraceabilityToFunctionView.ToString());
                parameters.Add(p => p.Id, requirement.Iid.ToString());
            });

            Assert.That(this.viewModel.CurrentBaseViewInstance, Is.TypeOf<RequirementTraceabilityToFunctionView>());
            var relatedViews = renderer.FindComponent<RelatedViews>();
            await renderer.InvokeAsync(() => relatedViews.Instance.OnViewSelect.InvokeAsync(relatedViews.Instance.OtherRelatedViews.First()));
            var navigationManager = this.context.Services.GetService<NavigationManager>();
            Assert.That(navigationManager!.Uri, Contains.Substring("RequirementBreakdown"));
        }
    }
}
