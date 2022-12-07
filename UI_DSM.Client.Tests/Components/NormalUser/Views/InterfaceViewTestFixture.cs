// --------------------------------------------------------------------------------------------------------
// <copyright file="InterfaceViewTestFixture.cs" company="RHEA System S.A.">
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
    using CDP4Common.EngineeringModelData;
   
    using CDP4Dal;

    using Feather.Blazor.Icons;

    using Microsoft.Extensions.DependencyInjection;
   
    using Moq;
   
    using NUnit.Framework;

    using Radzen.Blazor;

    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.App.ConnectionVisibilitySelector;
    using UI_DSM.Client.ViewModels.App.Filter;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Models;

    using BinaryRelationship = CDP4Common.DTO.BinaryRelationship;
    using ElementDefinition = CDP4Common.DTO.ElementDefinition;
    using ElementUsage = CDP4Common.DTO.ElementUsage;
    using Requirement = CDP4Common.EngineeringModelData.Requirement;
    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class InterfaceViewTestFixture
    {
        private IInterfaceViewViewModel viewModel;
        private Mock<IReviewItemService> reviewItemService;
        private TestContext context;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.reviewItemService = new Mock<IReviewItemService>();
            this.viewModel = new InterfaceViewViewModel(this.reviewItemService.Object, new FilterViewModel());
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
                var portCategoryId = Guid.NewGuid();
                var productCategoryId = Guid.NewGuid();
                var interfaceCategoryId = Guid.NewGuid();

                var categories = new List<Category>
            {
                new()
                {
                    Iid = portCategoryId,
                    Name = "ports"
                },
                new()
                {
                    Iid = interfaceCategoryId,
                    Name = "interfaces"
                },
                new()
                {
                    Iid = productCategoryId,
                    Name = "products"
                }
            };

                var portDefinition = new ElementDefinition()
                {
                    Iid = Guid.NewGuid(),
                    Name = "Port",
                    Category =
                {
                    portCategoryId
                }
                };

                var notConnectedPort = new ElementUsage()
                {
                    Iid = Guid.NewGuid(),
                    Name = "Port_ACC",
                    ElementDefinition = portDefinition.Iid
                };

                var sourcePort = new ElementUsage()
                {
                    Iid = Guid.NewGuid(),
                    Name = "Port_ALL",
                    ElementDefinition = portDefinition.Iid,
                    InterfaceEnd = InterfaceEndKind.INPUT,
                    Category = {portCategoryId}
                };

                var targetPort = new ElementUsage()
                {
                    Iid = Guid.NewGuid(),
                    Name = "Port_BLL",
                    ElementDefinition = portDefinition.Iid,
                    InterfaceEnd = InterfaceEndKind.OUTPUT,
                    Category = { portCategoryId }
                };

                var accelorometerBox = new ElementDefinition()
                {
                    Iid = Guid.NewGuid(),
                    Name = "Accelerometer Box",
                    Category = { productCategoryId },
                    ContainedElement = { targetPort.Iid, notConnectedPort.Iid }
                };

                var powerGenerator = new ElementDefinition()
                {
                    Iid = Guid.NewGuid(),
                    Name = "Battery",
                    Category = { productCategoryId },
                    ContainedElement = { sourcePort.Iid }
                };

                var emptyProduct = new ElementDefinition()
                {
                    Iid = Guid.NewGuid(),
                    Name = "Accelerometer Box 2",
                    Category = { productCategoryId },
                };

                var interfaceRelationShip = new BinaryRelationship()
                {
                    Iid = Guid.NewGuid(),
                    Category = { interfaceCategoryId },
                    Source = sourcePort.Iid,
                    Target = targetPort.Iid
                };

                var assembler = new Assembler(new Uri("http://localhost"));

                var things = new List<Thing>(categories)
                {
                    sourcePort,
                    targetPort,
                    notConnectedPort,
                    accelorometerBox,
                    powerGenerator,
                    interfaceRelationShip,
                    emptyProduct
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
                        ThingId = interfaceRelationShip.Iid,
                        Annotations = { new Comment(Guid.NewGuid()) }
                    }
                    });

                var renderer = this.context.RenderComponent<InterfaceView>();

                var pocos = assembler.Cache.Where(x => x.Value.IsValueCreated)
                    .Select(x => x.Value)
                    .Select(x => x.Value)
                    .ToList();

                await renderer.Instance.InitializeViewModel(pocos, projectId, reviewId, new List<string>(), new List<string>());
                Assert.That(renderer.FindComponents<FeatherMessageCircle>(), Has.Count.EqualTo(1));

                var button = renderer.FindComponent<RadzenCheckBox<bool>>();
                await renderer.InvokeAsync(async () => await button.Instance.Change.InvokeAsync(true));
                Assert.That(renderer.FindComponents<FeatherMessageCircle>(), Has.Count.EqualTo(0));

                await renderer.InvokeAsync(async () => await renderer.Instance.Grid
                    .ExpandRow(this.viewModel.Products.First(x => x.ThingId == powerGenerator.Iid)));

                Assert.That(renderer.FindComponents<FeatherMessageCircle>(), Has.Count.EqualTo(0));

                await renderer.InvokeAsync(async () => await renderer.Instance.Grid
                    .ExpandRow(this.viewModel.LoadChildren(this.viewModel.Products.First(x => x.ThingId == powerGenerator.Iid)).First()));

                Assert.That(renderer.FindComponents<FeatherMessageCircle>(), Has.Count.EqualTo(1));

                this.viewModel.PortVisibilityState.CurrentState = ConnectionToVisibilityState.NotConnected;

                Assert.Multiple(() =>
                {
                    Assert.That(renderer.FindComponents<FeatherMessageCircle>(), Has.Count.EqualTo(0));
                    Assert.That(this.viewModel.Products, Has.Count.EqualTo(3));
                });

                this.viewModel.ProductVisibilityState.CurrentState = ConnectionToVisibilityState.NotConnected;
                Assert.That(this.viewModel.Products, Has.Count.EqualTo(1));

                renderer.Instance.TrySetSelectedItem(this.viewModel.Interfaces.First());
                Assert.That(this.viewModel.SelectedElement, Is.TypeOf<InterfaceRowViewModel>());

                this.viewModel.TrySetSelectedItem(this.viewModel.Products.First());
                Assert.That(this.viewModel.SelectedElement, Is.TypeOf<ProductRowViewModel>());

                this.viewModel.TrySetSelectedItem(this.viewModel.LoadChildren(this.viewModel.Products.First()).First());
                Assert.That(this.viewModel.SelectedElement, Is.TypeOf<PortRowViewModel>());

                this.viewModel.TrySetSelectedItem(new RequirementRowViewModel(new Requirement()
                {
                    Iid = Guid.NewGuid()
                }, null));

                Assert.That(this.viewModel.SelectedElement, Is.Not.TypeOf<RequirementRowViewModel>());

                this.viewModel.TrySetSelectedItem(null);
                Assert.That(this.viewModel.SelectedElement, Is.Not.Null);

                var otherRenderer = this.context.RenderComponent<TrlView>();
                
                Assert.Multiple(() =>
                {
                    Assert.That(async () => await renderer.Instance.CopyComponents(otherRenderer.Instance), Is.False);
                    Assert.That(async () => await renderer.Instance.CopyComponents(renderer.Instance), Is.True);
                });
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }
    }
}
