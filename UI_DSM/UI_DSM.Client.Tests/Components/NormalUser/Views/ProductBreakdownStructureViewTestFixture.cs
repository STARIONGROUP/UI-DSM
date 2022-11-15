// --------------------------------------------------------------------------------------------------------
// <copyright file="ProductBreakdownStructureViewTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.NormalUser.Views
{
    using Bunit;

    using CDP4Common.DTO;

    using CDP4Dal;

    using Feather.Blazor.Icons;

    using Microsoft.Extensions.DependencyInjection;
   
    using Moq;
   
    using NUnit.Framework;

    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.App.Filter;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ProductBreakdownStructureViewTestFixture
    {
        private IProductBreakdownStructureViewViewModel viewModel;
        private Mock<IReviewItemService> reviewItemService;
        private TestContext context;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.reviewItemService = new Mock<IReviewItemService>();
            this.viewModel = new ProductBreakdownStructureViewViewModel(this.reviewItemService.Object);
            this.context.Services.AddSingleton(this.viewModel);
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifyComponent()
        {
            var productCategory = Guid.NewGuid();
            var equipmentCategory = Guid.NewGuid();
            var functionCategory = Guid.NewGuid();

            var categories = new List<Category>
            {
                new()
                {
                    Iid = productCategory,
                    Name = "products"
                },
                new()
                {
                    Iid = equipmentCategory,
                    Name = "equipment",
                    SuperCategory = new List<Guid>
                    {
                        productCategory
                    }
                },
                new()
                {
                    Iid = functionCategory,
                    Name="functions"
                }
            };

            var envision = new ElementDefinition()
            {
                Iid = Guid.NewGuid(),
                Name = "Envision",
                Category = new List<Guid>
                {
                    productCategory
                }
            };

            var groundSegment = new ElementDefinition()
            {
                Iid = Guid.NewGuid(),
                Name = "Ground Segment"
            };

            var launchSegment = new ElementDefinition()
            {
                Iid = Guid.NewGuid(),
                Name = "Launch Segment"
            };

            var function = new ElementDefinition()
            {
                Iid = Guid.NewGuid(),
                Name = "function",
                Category = new List<Guid> { functionCategory }
            };

            var elementDefinitions = new List<ElementDefinition>
            {
                envision,
                function,
                groundSegment, 
                launchSegment
            };

            var usages = new List<ElementUsage>
            {
                new ()
                {
                    Iid = Guid.NewGuid(),
                    Name = groundSegment.Name,
                    ElementDefinition = groundSegment.Iid,
                    Category = new List<Guid>{equipmentCategory}
                },
                new()
                {
                    Iid = Guid.NewGuid(),
                    Name = launchSegment.Name,
                    ElementDefinition = launchSegment.Iid
                },
                new()
                {
                    Iid = Guid.NewGuid(),
                    Name = function.Name,
                    ElementDefinition = function.Iid
                }
            };

            envision.ContainedElement = usages.Select(x => x.Iid).ToList();

            var iteration = new Iteration()
            {
                Iid = Guid.NewGuid(),
                TopElement = envision.Iid,
                Element = elementDefinitions.Select(x => x.Iid).ToList()
            };

            var assembler = new Assembler(new Uri("http://localhost"));

            var things = new List<Thing>(categories)
            {
                iteration, 
                function,
                groundSegment,
                envision,
                launchSegment
            };

            things.AddRange(usages);

            await assembler.Synchronize(things);
            _ = assembler.Cache.Select(x => x.Value.Value);

            var projectId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();

            this.reviewItemService.Setup(x => x.GetReviewItemsForThings(projectId, reviewId, It.IsAny<IEnumerable<Guid>>(), 0))
                .ReturnsAsync(new List<ReviewItem>
                {
                    new (Guid.NewGuid())
                    {
                        ThingId = usages.Last().Iid,
                        Annotations = { new Comment(Guid.NewGuid()) }
                    },
                    new (Guid.NewGuid())
                    {
                        ThingId =  usages.First().Iid,
                        Annotations = { new Comment(Guid.NewGuid()) }
                    }
                });

            var renderer = this.context.RenderComponent<ProductBreakdownStructureView>();

            var pocos = assembler.Cache.Where(x => x.Value.IsValueCreated)
                .Select(x => x.Value)
                .Select(x => x.Value)
                .ToList();

            await renderer.Instance.InitializeViewModel(pocos, projectId, reviewId);

            Assert.Multiple(() =>
            {
                Assert.That(renderer.Instance.ViewModel.TopElement, Has.Count.EqualTo(1));
                Assert.That(renderer.FindComponents<FeatherMessageCircle>(), Has.Count.EqualTo(1));
            });

            await renderer.InvokeAsync(() => renderer.Instance.Grid.RowSelect.InvokeAsync(renderer.Instance.ViewModel.TopElement.First()));
            Assert.That(this.viewModel.SelectedElement, Is.Not.Null);
        }
    }
}
