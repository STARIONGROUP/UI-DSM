// --------------------------------------------------------------------------------------------------------
// <copyright file="BudgetViewTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.NormalUser.Views
{
    using Bunit;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.Extensions;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.Services.ReviewService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Models;

    using Participant = UI_DSM.Shared.Models.Participant;
    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class BudgetViewTestFixture
    {
        private IBudgetViewViewModel viewModel;
        private Mock<IReviewItemService> reviewItemService;
        private TestContext context;
        private Mock<IReviewService> reviewService;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.reviewItemService = new Mock<IReviewItemService>();
            this.reviewService = new Mock<IReviewService>();
            this.viewModel = new BudgetViewViewModel(this.reviewItemService.Object, JsonSerializerHelper.CreateService(), this.reviewService.Object);
            this.context.Services.AddSingleton(this.viewModel);
            this.context.JSInterop.SetupVoid("window.ReportingViewerCustomization.setObjectRef", _ => true);
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifyComponent()
        {
            var iteration = new Iteration()
            {
                Iid = Guid.NewGuid()
            };

            var product = new ElementDefinition()
            {
                Iid = Guid.NewGuid(),
                Category =
                {
                    new Category()
                    {
                        Iid = Guid.NewGuid(),
                        Name = ThingExtension.ProductCategoryName
                    }
                }
            };

            var function = new ElementDefinition()
            {
                Iid = Guid.NewGuid(),
                Category =
                {
                    new Category()
                    {
                        Iid = Guid.NewGuid(),
                        Name = ThingExtension.FunctionCategoryName
                    }
                }
            };

            var port = new ElementUsage()
            {
                Iid = Guid.NewGuid(),
                Category =
                {
                    new Category()
                    {
                        Iid = Guid.NewGuid(),
                        Name = ThingExtension.PortCategoryName
                    }
                },
                ElementDefinition = new ElementDefinition(){ Iid = Guid.NewGuid()}
            };

            var elementDefintion = new ElementDefinition(){ Iid = Guid.NewGuid() };

            var things = new List<Thing>()
            {
                iteration, product, function, port, elementDefintion
            };

            var renderer = this.context.RenderComponent<BudgetView>();

            this.reviewService.Setup(x => x.GetReviewOfProject(It.IsAny<Guid>(), It.IsAny<Guid>(),0))
                .ReturnsAsync(new Review()
                {
                    Artifacts =
                    {
                        new BudgetTemplate()
                        {
                            BudgetName = "A name",
                            FileName = "A file.zip"
                        }
                    }
                });

            await this.viewModel.InitializeProperties(things, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                new List<string>(), new List<string>(), new Participant());

            Assert.That(this.viewModel.ReportDtoAsString, Is.Empty);
            this.viewModel.SelectedBudgetTemplate = this.viewModel.AvailableBudgets.First();
            Assert.That(this.viewModel.ReportDtoAsString, Is.Not.Empty);

            renderer.Instance.SetSelectedElement(elementDefintion.Iid.ToString());
            Assert.That(this.viewModel.SelectedElement, Is.TypeOf<ElementBaseRowViewModel>());

            renderer.Instance.SetSelectedElement(port.Iid.ToString());
            Assert.That(this.viewModel.SelectedElement, Is.TypeOf<PortRowViewModel>());

            renderer.Instance.SetSelectedElement(function.Iid.ToString());
            Assert.That(this.viewModel.SelectedElement, Is.TypeOf<FunctionRowViewModel>());

            renderer.Instance.SetSelectedElement(product.Iid.ToString());
            Assert.That(this.viewModel.SelectedElement, Is.TypeOf<ProductRowViewModel>());
        }
    }
}
