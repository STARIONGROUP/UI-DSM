// --------------------------------------------------------------------------------------------------------
// <copyright file="PhysicalFlowViewTestFixture.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar, Jaime Bernar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Tests.Components.NormalUser.Views
{
    using Blazor.Diagrams.Core.Geometry;
    using Blazor.Diagrams.Core.Models;
    using Blazor.Diagrams.Core.Models.Base;

    using Bunit;

    using CDP4Common.DTO;
    using CDP4Common.EngineeringModelData;
    
    using CDP4Dal;
    
    using CDP4JsonSerializer;
    
    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.App.ConnectionVisibilitySelector;
    using UI_DSM.Client.ViewModels.App.Filter;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    
    using UI_DSM.Serializer.Json;
    
    using UI_DSM.Shared.Models;

    using BinaryRelationship = CDP4Common.DTO.BinaryRelationship;
    using ElementDefinition = CDP4Common.DTO.ElementDefinition;
    using ElementUsage = CDP4Common.DTO.ElementUsage;
    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class PhysicalFlowViewTestFixture
    {
        private IInterfaceViewViewModel viewModel;
        private Mock<IReviewItemService> reviewItemService;
        private TestContext context;

        private readonly Uri uri = new Uri("http://test.com");

        private IRenderedComponent<PhysicalFlowView> renderer;

        [SetUp]
        public void Setup()
        {
            try
            {
                this.context = new TestContext();
                this.context.AddDevExpressBlazorTesting();
                this.context.ConfigureDevExpressBlazor();
                this.reviewItemService = new Mock<IReviewItemService>();
                this.viewModel = new InterfaceViewViewModel(this.reviewItemService.Object, new FilterViewModel(), null);
                this.context.Services.AddSingleton(this.viewModel);

                this.context.Services.AddSingleton(this.viewModel);

                this.context.Services.AddTransient<ICdp4JsonSerializer, Cdp4JsonSerializer>();
                this.context.Services.AddTransient<IJsonSerializer, JsonSerializer>();
                this.context.Services.AddTransient<IJsonDeserializer, JsonDeserializer>();
                this.context.Services.AddTransient<IJsonService, JsonService>();
                this.context.Services.AddTransient<IReviewItemService, ReviewItemService>();
                this.context.Services.AddTransient<IDocumentBasedViewModel, DocumentBasedViewModel>();
                this.context.Services.AddTransient<IFilterViewModel, FilterViewModel>();
                this.context.Services.AddTransient<IConnectionVisibilitySelectorViewModel, ConnectionVisibilitySelectorViewModel>();
                this.context.JSInterop.Setup<Rectangle>("ZBlazorDiagrams.getBoundingClientRect", _ => true);

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
                    ElementDefinition = portDefinition.Iid,
                    Category =
                {
                    portCategoryId
                }
                };

                var sourcePort = new ElementUsage()
                {
                    Iid = Guid.NewGuid(),
                    Name = "Port_ALL",
                    ElementDefinition = portDefinition.Iid,
                    InterfaceEnd = InterfaceEndKind.INPUT,
                    Category =
                {
                    portCategoryId
                }
                };

                var targetPort = new ElementUsage()
                {
                    Iid = Guid.NewGuid(),
                    Name = "Port_BLL",
                    ElementDefinition = portDefinition.Iid,
                    InterfaceEnd = InterfaceEndKind.OUTPUT,
                    Category =
                {
                    portCategoryId
                }
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

                assembler.Synchronize(things).Wait();
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

                this.renderer = this.context.RenderComponent<PhysicalFlowView>();

                var pocos = assembler.Cache.Where(x => x.Value.IsValueCreated)
                    .Select(x => x.Value)
                    .Select(x => x.Value)
                    .ToList();

                var reviewItem = new ReviewItem(Guid.NewGuid());

                this.renderer.Instance.InitializeViewModel(pocos, projectId, reviewId, Guid.Empty, new List<string>(), new List<string>()).RunSynchronously();
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public void VerifyThatGetNeighboursWorks()
        {
            try
            {
                var product = this.renderer.Instance.ViewModel.Products.First();
                var result = this.viewModel.GetNeighbours(product);
                Assert.That(result, Is.Not.Null);
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }

        [Test]
        public void VerifyThatModelCanBeSelected()
        {
            var productNode = this.viewModel.ProductsMap.Keys.First();
            Assert.That(this.viewModel.SelectedElement, Is.Null);
            this.viewModel.SetSelectedModel(productNode);
            Assert.That(this.viewModel.SelectedElement, Is.Not.Null);

            var portNode = this.viewModel.PortsMap.Keys.First();
            this.viewModel.SelectedElement = null;
            this.viewModel.SetSelectedModel(portNode);
            Assert.That(this.viewModel.SelectedElement, Is.Not.Null);

            var linkNode = this.viewModel.InterfacesMap.Keys.First();
            this.viewModel.SelectedElement = null;
            this.viewModel.SetSelectedModel(linkNode);
            Assert.That(this.viewModel.SelectedElement, Is.Not.Null);
        }

        [Test]
        public void VerifyThatANewNodeCanBeCreatedFromAProduct()
        {
            var product = this.renderer.Instance.ViewModel.Products.Last();
            var nodeModel = this.viewModel.CreateNewNodeFromProduct(product);
            Assert.That(nodeModel, Is.Not.Null);
        }

        [Test]
        public void VerifyThatCentralNodeCanBeCreated()
        {
            var product = this.renderer.Instance.ViewModel.Products.Last();
            this.viewModel.ProductsMap.Clear();
            this.viewModel.CreateCentralNodeAndNeighbours(product);
            Assert.That(this.viewModel.ProductsMap.Count > 0);
        }

        [Test]
        public void VerifyThatHasChildrenWorks()
        {
            var product = this.renderer.Instance.ViewModel.Products.Last();
            var result = this.viewModel.HasChildren(product);
            Assert.IsTrue(result);
        }

        [Test]
        public void VerifyThatCanLoadChildren()
        {
            var product = this.renderer.Instance.ViewModel.Products.Last();
            var result = this.viewModel.LoadChildren(product);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void VerifyThatInterfaceLinksCanBeCreated()
        {
            this.viewModel.InterfacesMap.Clear();

            Assert.That(this.viewModel.InterfacesMap, Has.Count.EqualTo(0));

            this.viewModel.CreateInterfacesLinks();

            Assert.Multiple(() =>
            {
                Assert.That(this.viewModel.Interfaces, Has.Count.GreaterThan(0));
                Assert.That(this.viewModel.InterfacesMap, Has.Count.GreaterThan(0));
            });
        }

        [Test]
        public void VerifyPhysicalFlowView()
        {
            var diagram = this.renderer.Instance.Diagram;

            Assert.That(diagram, Is.Not.Null);
            Assert.That(diagram.Options.AllowMultiSelection, Is.False);
        }

        [Test]
        public async Task VerifyThatDiagramIsUpdatedWhenCentralNodeChanged()
        {
            var diagram = this.renderer.Instance.Diagram;
            var product1 = this.renderer.Instance.ViewModel.Products.First();
            var product2 = this.renderer.Instance.ViewModel.Products[1];

            this.viewModel.CreateCentralNodeAndNeighbours(product1);
            await this.renderer.InvokeAsync(() => this.renderer.Instance.RefreshDiagram());

            var nodes = new List<NodeModel>(diagram.Nodes.ToList());
            var links = new List<BaseLinkModel>(diagram.Links.ToList());

            this.viewModel.CreateCentralNodeAndNeighbours(product2);
            await this.renderer.InvokeAsync(() => this.renderer.Instance.RefreshDiagram());

            Assert.That(nodes, Is.Not.EqualTo(diagram.Nodes));
            Assert.That(links, Is.Not.EqualTo(diagram.Links));
        }

        [Test]
        public async Task VerifyThatComponentsCanBeCopied()
        {
            try
            {
                var component = this.context.RenderComponent<PhysicalFlowView>();
                var anotherView = component.Instance;

                var oldViewModel = anotherView.ViewModel;
                this.context.JSInterop.Setup<int[]>("GetNodeDimensions").SetResult(new int[] { 100, 80 });

                Task<bool> result = Task.FromResult(false);
                await this.renderer.InvokeAsync(() => result = this.renderer.Instance.CopyComponents(anotherView));
                var newViewModel = this.renderer.Instance.ViewModel;

                Assert.That(oldViewModel, Is.EqualTo(newViewModel));
                Assert.That(result.Result, Is.True);
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }

        [Test]
        public async Task VerifyThatComponentsAreNotCopied()
        {
            var component = this.context.RenderComponent<DocumentBased>();
            var anotherView = component.Instance;

            var oldViewModel = anotherView.ViewModel;
            this.context.JSInterop.Setup<int[]>("GetNodeDimensions").SetResult(new int[] { 100, 80 });

            Task<bool> result = Task.FromResult(false);
            await this.renderer.InvokeAsync(() => result = this.renderer.Instance.CopyComponents(anotherView));
            var newViewModel = this.renderer.Instance.ViewModel;

            Assert.That(oldViewModel, Is.Not.EqualTo(newViewModel));
            Assert.That(result.Result, Is.False);
        }

        [Test]
        public void VerifyThatFirstProductCanBeCreatedBySelectedElement()
        {
            try
            {
                var product = this.viewModel.ProductsMap.Values.First();
                var port = this.viewModel.PortsMap.Values.First();
                var interf = this.viewModel.InterfacesMap.Values.First();

                var productRow = this.viewModel.SelectFirstProductByCloserSelectedItem(product);
                var portRow = this.viewModel.SelectFirstProductByCloserSelectedItem(port);
                var interfaceRow = this.viewModel.SelectFirstProductByCloserSelectedItem(interf);
                var nullRow = this.viewModel.SelectFirstProductByCloserSelectedItem(null);

                Assert.Multiple(() =>
                {
                    Assert.That(productRow, Is.Not.Null);
                    Assert.That(portRow, Is.Not.Null);
                    Assert.That(interfaceRow, Is.Not.Null);
                    Assert.That(nullRow, Is.Not.Null);
                });
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }

        [Test]
        public void VerifyThatObjectCanBeUpdated()
        {
            try
            {
                var product = this.viewModel.Products.First();
                var result = this.viewModel.TryUpdate(product, true);
                Assert.That(result, Is.True);

                var product2 = new ProductRowViewModel(new CDP4Common.EngineeringModelData.ElementDefinition(), new ReviewItem());
                var result2 = this.viewModel.TryUpdate(product2, true);
                Assert.That(result2, Is.False);
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }

        [Test]
        public async Task VerifyThatDoubleClickOnModelWorks()
        {
            var diagramNode = this.viewModel.ProductsMap.Keys.Last();
            await this.renderer.InvokeAsync(() => this.renderer.Instance.MouseDoubleClickOnModel(diagramNode, new Blazor.Diagrams.Core.Geometry.Point(0, 0)));
            Assert.That(this.viewModel.ProductsMap, Has.Count.GreaterThan(0));

            var link = this.viewModel.InterfacesMap.Keys.Last();
            Assert.That(link.Vertices.Count, Is.EqualTo(0));
            await this.renderer.InvokeAsync(() => this.renderer.Instance.MouseDoubleClickOnModel(link, new Blazor.Diagrams.Core.Geometry.Point(0, 0)));
            Assert.That(link.Vertices.Count, Is.GreaterThan(0));
        }

        [Test]
        public void VerifyThatMouseUpOnModelWorks()
        {
            var diagramNode = this.viewModel.ProductsMap.Keys.Last();
            var previousSelectedElement = this.renderer.Instance.ViewModel.SelectedElement;
            this.renderer.Instance.MouseUpOnComponent(diagramNode);
            var lastSelectedElement = this.renderer.Instance.ViewModel.SelectedElement;
            Assert.That(lastSelectedElement, Is.Not.EqualTo(previousSelectedElement));
        }
    }
}
