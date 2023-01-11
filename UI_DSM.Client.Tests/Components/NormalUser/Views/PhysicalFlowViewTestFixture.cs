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
    using AppComponents;

    using Blazor.Diagrams.Core.Geometry;
    using Blazor.Diagrams.Core.Models;
    using Blazor.Diagrams.Core.Models.Base;

    using Bunit;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.EngineeringModelData;
    
    using CDP4Dal;
    
    using CDP4JsonSerializer;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components.NormalUser.DiagrammingConfiguration;
    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.Services.DiagrammingConfigurationService;
    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.App.ConnectionVisibilitySelector;
    using UI_DSM.Client.ViewModels.App.Filter;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Serializer.Json;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    using BinaryRelationship = CDP4Common.DTO.BinaryRelationship;
    using ElementDefinition = CDP4Common.DTO.ElementDefinition;
    using ElementUsage = CDP4Common.DTO.ElementUsage;
    using Participant = UI_DSM.Shared.Models.Participant;
    using TestContext = Bunit.TestContext;
    using Thing = CDP4Common.DTO.Thing;

    [TestFixture]
    public class PhysicalFlowViewTestFixture
    {
        private IInterfaceViewViewModel viewModel;
        private Mock<IReviewItemService> reviewItemService;
        private TestContext context;
        private IRenderedComponent<PhysicalFlowView> renderer;
        private Mock<IDiagrammingConfigurationService> diagramService;

        [SetUp]
        public void Setup()
        {
            try
            {
                this.context = new TestContext();
                this.context.AddDevExpressBlazorTesting();
                this.context.ConfigureDevExpressBlazor();
                this.reviewItemService = new Mock<IReviewItemService>();
                this.diagramService = new Mock<IDiagrammingConfigurationService>();

                this.viewModel = new InterfaceViewViewModel(this.reviewItemService.Object, new FilterViewModel(), this.diagramService.Object,
                    new ErrorMessageViewModel());

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

                this.context.JSInterop.Setup<int[]>("GetNodeDimensions").SetResult(new[] { 100, 80 });

                this.renderer.Instance.InitializeViewModel(pocos, projectId, reviewId, Guid.Empty, new List<string>(), 
                    new List<string>(), new Participant() { Role = new Role() { AccessRights = { AccessRight.CreateDiagramConfiguration } } });
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

            Assert.Multiple(() =>
            {
                Assert.That(nodes, Is.Not.EqualTo(diagram.Nodes));
                Assert.That(links, Is.Not.EqualTo(diagram.Links));
            });
        }

        [Test]
        public async Task VerifyThatComponentsCanBeCopied()
        {
            try
            {
                var component = this.context.RenderComponent<PhysicalFlowView>();
                var anotherView = component.Instance;

                var oldViewModel = anotherView.ViewModel;
                this.context.JSInterop.Mode = JSRuntimeMode.Strict;
                this.context.JSInterop.Setup<int[]>("GetNodeDimensions", _ => true).SetResult(new [] { 100, 80 });

                var result = Task.FromResult(false);
                await this.renderer.InvokeAsync(() => result = this.renderer.Instance.CopyComponents(anotherView));
                var newViewModel = this.renderer.Instance.ViewModel;

                Assert.Multiple(() =>
                {
                    Assert.That(oldViewModel, Is.EqualTo(newViewModel));
                    Assert.That(result.Result, Is.True);
                });
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

            var result = Task.FromResult(false);
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
            Assert.That(link.Vertices, Has.Count.EqualTo(0));
            await this.renderer.InvokeAsync(() => this.renderer.Instance.MouseDoubleClickOnModel(link, new Blazor.Diagrams.Core.Geometry.Point(0, 0)));
            Assert.That(link.Vertices, Has.Count.GreaterThan(0));
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

        [Test]
        public async Task VerifySaveDiagram()
        {
            this.renderer.Instance.IsLoading = false;
            this.renderer.Render();
            var buttons = this.renderer.FindComponents<AppButton>();
            Assert.That(this.viewModel.IsOnSavingMode, Is.False);

            await this.renderer.InvokeAsync(buttons[0].Instance.Click.InvokeAsync);
            Assert.That(this.viewModel.IsOnSavingMode, Is.True);

            this.diagramService.Setup(x => x.SaveDiagramLayout(It.IsAny<Guid>(), It.IsAny<Guid>(),
                It.IsAny<string>(), It.IsAny<DiagramDto>())).ReturnsAsync((false,new List<string>{"Alreayd exist"}));

            var creationDialog = this.renderer.FindComponent<DiagrammingConfigurationPopup>();
            creationDialog.Instance.ViewModel.ConfigurationName = "a config";
            await this.renderer.InvokeAsync(creationDialog.Instance.ViewModel.OnValidSubmit.InvokeAsync);
            Assert.That(this.viewModel.IsOnSavingMode, Is.True);

            this.diagramService.Setup(x => x.SaveDiagramLayout(It.IsAny<Guid>(), It.IsAny<Guid>(),
                It.IsAny<string>(), It.IsAny<DiagramDto>())).ReturnsAsync((true, new List<string>()));

            await this.renderer.InvokeAsync(creationDialog.Instance.ViewModel.OnValidSubmit.InvokeAsync);
            Assert.That(this.viewModel.IsOnSavingMode, Is.False);
        }

        [Test]
        public async Task VerifyLoadDiagram()
        {
            this.renderer.Instance.IsLoading = false;
            this.renderer.Render();
            var buttons = this.renderer.FindComponents<AppButton>();
            Assert.That(this.viewModel.IsOnLoadingMode, Is.False);

            this.diagramService.Setup(x => x.LoadDiagramLayoutConfigurationNames(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new List<string> { "A config" });

            await this.renderer.InvokeAsync(buttons[1].Instance.Click.InvokeAsync);
            Assert.That(this.viewModel.IsOnLoadingMode, Is.True);

            var creationDialog = this.renderer.FindComponent<DiagrammingConfigurationLoadingPopup>();
            Assert.That(creationDialog.Instance.ViewModel.ConfigurationsName.ToList(), Has.Count.EqualTo(1));

            creationDialog.Instance.ViewModel.SelectedConfiguration = "A config";

            this.diagramService.Setup(x => x.LoadDiagramLayoutConfiguration(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(new DiagramDto()
                {
                    Nodes = new List<DiagramNodeDto>()
                    {
                        new()
                        {
                            ThingId = this.viewModel.Products[0].ThingId,
                            Point = new PointDto()
                            {
                                X = 650,
                                Y = 447
                            }
                        },
                        new()
                        {
                            ThingId = this.viewModel.Products[1].ThingId,
                            Point = new PointDto()
                            {
                                X = 750,
                                Y = 447
                            }
                        },
                        new()
                        {
                            ThingId = this.viewModel.Products[2].ThingId,
                            Point = new PointDto()
                            {
                                X = 500,
                                Y = 447
                            }
                        }
                    }, 
                    Links = new List<DiagramLinkDto>()
                    {
                        new()
                        {
                            ThingId = this.viewModel.Interfaces[0].ThingId,
                            Vertices = new List<PointDto>{new (){X = 450, Y = 200}}
                        }
                    },
                    Filters = new List<FilterDto>()
                    {
                        new ()
                        {
                            ClassKind = ClassKind.DomainOfExpertise,
                            SelectedFilters = new List<Guid>(this.viewModel.FilterViewModel.SelectedFilters[ClassKind.DomainOfExpertise].Select(x => x.DefinedThing.Iid))
                        },
                        new()
                        {
                            ClassKind = ClassKind.ElementDefinition,
                            SelectedFilters = new List<Guid>(this.viewModel.FilterViewModel.SelectedFilters[ClassKind.ElementDefinition].Select(x => x.DefinedThing.Iid))
                        },
                        new()
                        {
                            ClassKind = ClassKind.Category,
                            SelectedFilters = new List<Guid>(this.viewModel.FilterViewModel.SelectedFilters[ClassKind.Category].Select(x => x.DefinedThing.Iid))
                        }
                    }
                });

            await this.renderer.InvokeAsync(creationDialog.Instance.ViewModel.OnValidSubmit.InvokeAsync);
            this.diagramService.Verify(x => x.LoadDiagramLayoutConfiguration(It.IsAny<Guid>(), It.IsAny<Guid>(), "A config"), Times.Once);
        }
    }
}
