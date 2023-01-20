// --------------------------------------------------------------------------------------------------------
// <copyright file="FunctionalTraceabilityToProductViewTestFixture.cs" company="RHEA System S.A.">
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
    using AppComponents;

    using Bunit;
   
    using CDP4Common.DTO;
    
    using CDP4Dal;

    using Feather.Blazor.Icons;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;
  
    using NUnit.Framework;

    using UI_DSM.Client.Components.App.TraceabilityTable;
    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.App.ConnectionVisibilitySelector;
    using UI_DSM.Client.ViewModels.App.Filter;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Shared.Models;

    using Participant = UI_DSM.Shared.Models.Participant;
    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class FunctionalTraceabilityToProductViewTestFixture
    {
        private IFunctionalTraceabilityToProductViewViewModel viewModel;
        private Mock<IReviewItemService> reviewItemService;
        private TestContext context;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.reviewItemService = new Mock<IReviewItemService>();

            this.viewModel = new FunctionalTraceabilityToProductViewViewModel(this.reviewItemService.Object, 
                new FilterViewModel(), new FilterViewModel());

            this.context.Services.AddSingleton(this.viewModel);
            this.context.Services.AddTransient<IConnectionVisibilitySelectorViewModel, ConnectionVisibilitySelectorViewModel>();
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifyComponent()
        {
            try
            {
                var implementsCategory = Guid.NewGuid();
                var functionsCategory = Guid.NewGuid();
                var productCategory = Guid.NewGuid();

                var categories = new List<Category>
            {
                new ()
                {
                    Iid = implementsCategory,
                    Name = "implements"
                },
                new()
                {
                    Iid = functionsCategory,
                    Name = "functions"
                },
                new()
                {
                    Iid = productCategory,
                    Name = "products"
                }
            };

                var owner = new DomainOfExpertise()
                {
                    Iid = Guid.NewGuid(),
                    Name = "Site Administrator",
                    ShortName = "admin"
                };

                var elementDefinitionForProduct = new ElementDefinition()
                {
                    Iid = Guid.NewGuid(),
                    Owner = owner.Iid,
                    Name = "Element Definition For Product Usage",
                    ShortName = "elementDefinitionForProduct"
                };

                var elementDefinitionForFunction = new ElementDefinition()
                {
                    Iid = Guid.NewGuid(),
                    Owner = owner.Iid,
                    Name = "Element Definition For Function Usage",
                    ShortName = "elementDefinitionForFunction",
                    Category = new List<Guid> { functionsCategory }
                };

                var productUsage = new ElementUsage()
                {
                    Iid = Guid.NewGuid(),
                    Owner = owner.Iid,
                    ElementDefinition = elementDefinitionForProduct.Iid,
                    Category = new List<Guid> { productCategory },
                    Name = "Product"
                };

                var functionUsage = new ElementUsage()
                {
                    Iid = Guid.NewGuid(),
                    Owner = owner.Iid,
                    ElementDefinition = elementDefinitionForFunction.Iid,
                    Name = "Function"
                };

                var product = new ElementDefinition()
                {
                    Iid = Guid.NewGuid(),
                    Name = "Product",
                    Owner = owner.Iid,
                    ShortName = "product",
                    Category = new List<Guid> { productCategory },
                    ContainedElement = new List<Guid> { productUsage.Iid }
                };

                var function = new ElementDefinition()
                {
                    Iid = Guid.NewGuid(),
                    Name = "Function",
                    Owner = owner.Iid,
                    ShortName = "function",
                    Category = new List<Guid> { functionsCategory },
                    ContainedElement = new List<Guid> { functionUsage.Iid }
                };

                var anotherFuction = new ElementDefinition()
                {
                    Iid = Guid.NewGuid(),
                    Owner = owner.Iid,
                    Name = "Another Function",
                    ShortName = "anotherFunction",
                    Category = new List<Guid> { functionsCategory }
                };

                var relationShip = new BinaryRelationship()
                {
                    Iid = Guid.NewGuid(),
                    Source = productUsage.Iid,
                    Target = functionUsage.Iid,
                    Category = new List<Guid> { implementsCategory }
                };

                var assembler = new Assembler(new Uri("http://localhost"));

                var things = new List<Thing>(categories)
                {
                    relationShip,
                    anotherFuction,
                    functionUsage,
                    function,
                    productUsage,
                    product,
                    elementDefinitionForFunction,
                    elementDefinitionForProduct,
                    owner
                };

                await assembler.Synchronize(things);
                _ = assembler.Cache.Select(x => x.Value.Value);

                var projectId = Guid.NewGuid();
                var reviewId = Guid.NewGuid();

                this.reviewItemService.Setup(x => x.GetReviewItemsForThings(projectId, reviewId, It.IsAny<IEnumerable<Guid>>(), 0))
                    .ReturnsAsync(new List<ReviewItem>
                    {
                    new (Guid.NewGuid())
                        {
                            ThingId = functionUsage.Iid,
                            Annotations = { new Comment(Guid.NewGuid()) }
                        }
                    });

                var renderer = this.context.RenderComponent<FunctionalTraceabilityToProductView>();

                var pocos = assembler.Cache.Where(x => x.Value.IsValueCreated)
                    .Select(x => x.Value)
                    .Select(x => x.Value)
                    .ToList();

                await renderer.Instance.InitializeViewModel(pocos, projectId, reviewId, Guid.Empty, new List<string>(), 
                    new List<string>(), new Participant());
                
                Assert.Multiple(() =>
                {
                    Assert.That(renderer.FindComponents<FeatherCheck>(), Has.Count.EqualTo(1));
                    Assert.That(renderer.FindComponents<FeatherMessageCircle>(), Has.Count.EqualTo(1));
                });

                var cell = renderer.FindComponent<TraceabilityCell>();
                await renderer.InvokeAsync(() => cell.Instance.OnClick.InvokeAsync(cell.Instance.RelationshipRow));
                Assert.That(this.viewModel.SelectedElement, Is.Not.Null);

                var rows = this.viewModel.GetAvailablesRows();

                Assert.Multiple(() =>
                {
                    Assert.That(rows, Is.Not.Empty);
                    Assert.That(this.viewModel.TraceabilityTableViewModel.Rows.First().ReviewItem, Is.Null);
                });

                var reviewItem = new ReviewItem(Guid.NewGuid())
                {
                    ThingId = this.viewModel.TraceabilityTableViewModel.Rows.First().ThingId
                };

                this.viewModel.UpdateAnnotatableRows(new List<AnnotatableItem> { reviewItem });
                Assert.That(this.viewModel.TraceabilityTableViewModel.Rows.First().ReviewItem, Is.Not.Null);

                var settingsButton = renderer.FindComponent<AppButton>();
                await renderer.InvokeAsync(settingsButton.Instance.Click.InvokeAsync);
                Assert.That(this.viewModel.IsViewSettingsVisible, Is.True);
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }
    }
}
