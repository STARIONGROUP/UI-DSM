// --------------------------------------------------------------------------------------------------------
// <copyright file="InterfaceViewViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.NormalUser.Views
{
    using Blazor.Diagrams.Core;
    using Blazor.Diagrams.Core.Geometry;
    using Blazor.Diagrams.Core.Models;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using DynamicData;
    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.Components.NormalUser.DiagrammingConfiguration;
    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.Extensions;
    using UI_DSM.Client.Model;
    using UI_DSM.Client.Services.DiagrammingConfigurationService;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.ViewModels.App.ConnectionVisibilitySelector;
    using UI_DSM.Client.ViewModels.App.Filter;
    using UI_DSM.Client.ViewModels.Components.NormalUser.DiagrammingConfiguration;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;

    using Model = Blazor.Diagrams.Core.Models.Base.Model;

    /// <summary>
    ///     View model for the <see cref="Client.Components.NormalUser.Views.InterfaceView" /> component
    /// </summary>
    public class InterfaceViewViewModel : BaseViewViewModel, IInterfaceViewViewModel
    {
        /// <summary>
        ///     The <see cref="IDiagrammingConfigurationService" />
        /// </summary>
        private readonly IDiagrammingConfigurationService diagrammingConfigurationService;

        /// <summary>
        ///     A collection of all <see cref="InterfaceRowViewModel" />
        /// </summary>
        private List<InterfaceRowViewModel> allInterfaces;

        /// <summary>
        ///     The collection of all <see cref="PortRowViewModel" />
        /// </summary>
        private List<PortRowViewModel> allPorts;

        /// <summary>
        ///     A collection of all <see cref="ProductRowViewModel" />
        /// </summary>
        private List<ProductRowViewModel> allProducts;

        /// <summary>
        ///     A collection of filtered <see cref="ProductRowViewModel" />
        /// </summary>
        private List<ProductRowViewModel> filteredProducts;

        /// <summary>
        ///     Backing field for <see cref="IsOnSavingMode" />
        /// </summary>
        private bool isOnSavingMode;

        /// <summary>
        ///     Backing field for <see cref="IsViewSettingsVisible" />
        /// </summary>
        private bool isViewSettingsVisible;

        /// <summary>
        ///     Backing field for <see cref="ShouldShowProducts" />
        /// </summary>
        private bool shouldShowProducts;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseViewViewModel" /> class.
        /// </summary>
        /// <param name="reviewItemService">The <see cref="IReviewItemService" /></param>
        /// <param name="filterViewModel">The <see cref="IFilterViewModel" /></param>
        /// <param name="diagrammingConfigurationService">The <see cref="IDiagrammingConfigurationService"/></param>
        public InterfaceViewViewModel(IReviewItemService reviewItemService, IFilterViewModel filterViewModel, IDiagrammingConfigurationService diagrammingConfigurationService) : base(reviewItemService)
        {
            this.FilterViewModel = filterViewModel;
            this.diagrammingConfigurationService = diagrammingConfigurationService;
        }

        /// <summary>
        ///     Value asserting if products has to been shown
        /// </summary>
        public bool ShouldShowProducts
        {
            get => this.shouldShowProducts;
            private set => this.RaiseAndSetIfChanged(ref this.shouldShowProducts, value);
        }

        /// <summary>
        ///     Value asserting if the settings for the view should be visible
        /// </summary>
        public bool IsViewSettingsVisible
        {
            get => this.isViewSettingsVisible;
            set => this.RaiseAndSetIfChanged(ref this.isViewSettingsVisible, value);
        }

        /// <summary>
        ///     The <see cref="IFilterViewModel" />
        /// </summary>
        public IFilterViewModel FilterViewModel { get; }

        /// <summary>
        ///     A collection of filtered <see cref="InterfaceRowViewModel" />
        /// </summary>
        public List<InterfaceRowViewModel> Interfaces { get; private set; } = new();

        /// <summary>
        ///     The <see cref="IConnectionVisibilitySelectorViewModel" /> for products
        /// </summary>
        public IConnectionVisibilitySelectorViewModel ProductVisibilityState { get; private set; } = new ConnectionVisibilitySelectorViewModel();

        /// <summary>
        ///     A collection <see cref="IBelongsToInterfaceView" />
        /// </summary>
        public IEnumerable<IBelongsToInterfaceView> Data { get; private set; }

        /// <summary>
        ///     A collection of <see cref="ProductRowViewModel" />
        /// </summary>
        public List<ProductRowViewModel> Products { get; private set; } = new();

        /// <summary>
        ///     The <see cref="IConnectionVisibilitySelectorViewModel" /> for ports
        /// </summary>
        public IConnectionVisibilitySelectorViewModel PortVisibilityState { get; private set; } = new ConnectionVisibilitySelectorViewModel();

        /// <summary>
        ///     The map collection from <see cref="NodeModel" /> ID to <see cref="ProductRowViewModel" />
        /// </summary>
        public Dictionary<DiagramNode, ProductRowViewModel> ProductsMap { get; } = new();

        /// <summary>
        ///     The map collection from <see cref="PortModel" /> ID to <see cref="PortRowViewModel" />
        /// </summary>
        public Dictionary<DiagramPort, PortRowViewModel> PortsMap { get; } = new();

        /// <summary>
        ///     The map collection from <see cref="LinkModel" /> ID to <see cref="InterfaceRowViewModel" />
        /// </summary>
        public Dictionary<DiagramLink, InterfaceRowViewModel> InterfacesMap { get; } = new();

        /// <summary>
        /// Gets or sets the center of the diagram.
        /// </summary>
        public Point DiagramCenter { get; set; } = new Point(800, 800);

        /// <summary>
        ///     Filters current rows
        /// </summary>
        /// <param name="selectedFilters">The selected filters</param>
        public void FilterRows(IReadOnlyDictionary<ClassKind, List<FilterRow>> selectedFilters)
        {
            var selectedOwners = selectedFilters[ClassKind.DomainOfExpertise];
            var selectedCategories = selectedFilters[ClassKind.Category];
            var selectedComponents = selectedFilters[ClassKind.ElementDefinition];

            this.filteredProducts = this.allProducts.Where(x => selectedComponents.Any(component => component.DefinedThing.Iid == x.ThingId))
                .OrderBy(x => x.Thing.Name)
                .ToList();

            this.allInterfaces.ForEach(x => x.IsVisible = false);

            this.Interfaces = this.allInterfaces.Where(x => selectedOwners.Any(owner => x.InterfaceOwner.Iid == owner.DefinedThing.Iid)
                                                            && selectedCategories.Any(cat => x.NatureCategory.Iid == cat.DefinedThing.Iid))
                .OrderBy(x => x.Id)
                .ToList();

            this.Interfaces.ForEach(x => x.IsVisible = true);
            this.ApplyVisibility();
        }

        /// <summary>
        ///     Handle the change of visibility
        /// </summary>
        public void OnVisibilityStateChanged()
        {
            this.ApplyVisibility();
        }

        /// <summary>
        ///     Initialize this view model properties
        /// </summary>
        /// <param name="things">A collection of <see cref="Thing" /></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <param name="reviewTaskId">The <see cref="ReviewTask"/> id</param>
        /// <param name="prefilters">A collection of prefilters</param>
        /// <param name="additionnalColumnsVisibleAtStart">A collection of columns name that can be visible by default at start</param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task InitializeProperties(IEnumerable<Thing> things, Guid projectId, Guid reviewId, Guid reviewTaskId, List<string> prefilters, List<string> additionnalColumnsVisibleAtStart)
        {
            await base.InitializeProperties(things, projectId, reviewId, reviewTaskId, prefilters, additionnalColumnsVisibleAtStart);

            var products = this.Things.OfType<ElementDefinition>()
                .Where(x => x.IsProduct())
                .ToList();

            var ports = products.SelectMany(x => x.ContainedElement)
                .Where(x => x.IsPort())
                .ToList();

            var interfaces = ports.SelectMany(x => x.GetInterfacesOfPort())
                .DistinctBy(x => x.Iid)
                .ToList();

            var allThings = new List<Thing>(products);
            allThings.AddRange(ports);
            allThings.AddRange(interfaces);

            var reviewItems = await this.ReviewItemService.GetReviewItemsForThings(this.ProjectId, this.ReviewId,
                allThings.Select(x => x.Iid));

            this.allPorts = new List<PortRowViewModel>(ports
                .Select(x => new PortRowViewModel(x, reviewItems.FirstOrDefault(ri => ri.ThingId == x.Iid)))
                .OrderBy(x => x.Name));

            this.allProducts = new List<ProductRowViewModel>(products
                .Select(x => new ProductRowViewModel(x, reviewItems.FirstOrDefault(ri => ri.ThingId == x.Iid))));

            this.allInterfaces = new List<InterfaceRowViewModel>(interfaces
                .Select(x => new InterfaceRowViewModel(x, reviewItems.FirstOrDefault(ri => ri.ThingId == x.Iid),
                    this.allPorts.First(source => source.ThingId == x.Source.Iid),
                    this.allPorts.First(target => target.ThingId == x.Target.Iid))));

            this.filteredProducts = new List<ProductRowViewModel>(this.allProducts.OrderBy(x => x.Thing.Name));
            this.Interfaces = new List<InterfaceRowViewModel>(this.allInterfaces.OrderBy(x => x.Id));
            this.ApplyVisibility();
            this.InitializesFilter();

            this.InitializeDiagram();

            this.DiagrammingConfigurationPopupViewModel = new DiagrammingConfigurationPopupViewModel
            {
                OnValidSubmit = new EventCallbackFactory().Create(this, this.SaveCurrentDiagramLayout)
            };
        }

        /// <summary>
        ///     Tries to set the <see cref="IBaseViewViewModel.SelectedElement" /> to the previous selected item
        /// </summary>
        /// <param name="selectedItem">The previously selectedItem</param>
        public override void TrySetSelectedItem(object selectedItem)
        {
            if (selectedItem is IHaveThingRowViewModel row)
            {
                var existingRow = this.Interfaces.FirstOrDefault(x => x.ThingId == row.ThingId) as IHaveThingRowViewModel;
                existingRow ??= this.Products.FirstOrDefault(x => x.ThingId == row.ThingId);
                existingRow ??= this.allPorts.FirstOrDefault(x => x.ThingId == row.ThingId);
                this.SelectedElement = existingRow;
            }
        }

        /// <summary>
        ///     Gets a collection of all availables <see cref="IHaveAnnotatableItemRowViewModel" />
        /// </summary>
        /// <returns>The collection of <see cref="IHaveAnnotatableItemRowViewModel" /></returns>
        public override List<IHaveAnnotatableItemRowViewModel> GetAvailablesRows()
        {
            var rows = new List<IHaveAnnotatableItemRowViewModel>();
            rows.AddRange(this.Interfaces);
            rows.AddRange(this.Products);
            rows.AddRange(this.allPorts);
            return rows;
        }

        /// <summary>
        ///     Updates all <see cref="IHaveAnnotatableItemRowViewModel" />
        /// </summary>
        /// <param name="annotatableItems">A collection of <see cref="AnnotatableItem" /></param>
        public override void UpdateAnnotatableRows(List<AnnotatableItem> annotatableItems)
        {
            var reviewItems = annotatableItems.OfType<ReviewItem>();
            this.Interfaces.ForEach(x => x.UpdateReviewItem(reviewItems.FirstOrDefault(ri => ri.ThingId == x.ThingId)));
            this.Products.ForEach(x => x.UpdateReviewItem(reviewItems.FirstOrDefault(ri => ri.ThingId == x.ThingId)));
            this.allPorts.ForEach(x => x.UpdateReviewItem(reviewItems.FirstOrDefault(ri => ri.ThingId == x.ThingId)));
        }

        /// <summary>
        ///     Asserts that a <see cref="ProductRowViewModel" /> has <see cref="PortRowViewModel" /> children
        /// </summary>
        /// <param name="productRow">The <see cref="ProductRowViewModel" /></param>
        /// <returns>True if contains any visible port</returns>
        public bool HasChildren(ProductRowViewModel productRow)
        {
            return this.allPorts.Any(x => x.ElementContainer.Iid == productRow.ThingId && x.IsVisible);
        }

        /// <summary>
        ///     Asserts that a <see cref="PortRowViewModel" /> has <see cref="InterfaceRowViewModel" /> children
        /// </summary>
        /// <param name="portRow">The <see cref="PortRowViewModel" /></param>
        /// <returns>True if contains any visible interfaces</returns>
        public bool HasChildren(PortRowViewModel portRow)
        {
            return this.Interfaces.Any(x => (x.SourceId == portRow.ThingId || x.TargetId == portRow.ThingId) && x.IsVisible);
        }

        /// <summary>
        ///     Loads all <see cref="PortRowViewModel" /> children rows of a <see cref="ProductRowViewModel" />
        /// </summary>
        /// <param name="productRow">The <see cref="ProductRowViewModel" /></param>
        /// <returns>A collection of <see cref="PortRowViewModel" /></returns>
        public IEnumerable<PortRowViewModel> LoadChildren(ProductRowViewModel productRow)
        {
            return this.allPorts.Where(x => x.ElementContainer.Iid == productRow.ThingId && x.IsVisible)
                .OrderBy(x => x.Thing.Name);
        }

        /// <summary>
        ///     Loads all <see cref="InterfaceRowViewModel" /> children rows of a <see cref="PortRowViewModel" />
        /// </summary>
        /// <param name="portRow">The <see cref="PortRowViewModel" /></param>
        /// <returns>A collection of <see cref="InterfaceRowViewModel" /></returns>
        public IEnumerable<InterfaceRowViewModel> LoadChildren(PortRowViewModel portRow)
        {
            return this.Interfaces.Where(x => (x.SourceId == portRow.ThingId || x.TargetId == portRow.ThingId) && x.IsVisible)
                .OrderBy(x => x.Id);
        }

        /// <summary>
        ///     Modifies the datasource to display based on the visibility of products
        /// </summary>
        /// <param name="visibility">The new visibility</param>
        public void SetProductsVisibility(bool visibility)
        {
            this.Data = visibility ? this.Products : this.Interfaces;
            this.ShouldShowProducts = visibility;
        }

        /// <summary>
        ///     Initializes the diagram with the selected element as a center node.
        /// </summary>
        public void InitializeDiagram()
        {
            var firstCenterProduct = this.SelectFirstProductByCloserSelectedItem(this.SelectedElement);
            this.CreateCentralNodeAndNeighbours(firstCenterProduct);
        }

        /// <summary>
        /// Selects the first central product depending on the selected element.
        /// </summary>
        /// <param name="selectedElement">the selected element</param>
        /// <returns>the selected <see cref="ProductRowViewModel"/></returns>
        /// <exception cref="ArgumentException">if the selected element is not null and is not of correct type</exception>
        public ProductRowViewModel SelectFirstProductByCloserSelectedItem(object selectedElement)
        {
            if (selectedElement is ProductRowViewModel productRowViewModel)
            {
                return productRowViewModel;
            }
            else if (selectedElement is PortRowViewModel portRowViewModel)
            {
                return this.Products.FirstOrDefault(x => x.ThingId == portRowViewModel.ContainerId, this.Products.First());
            }
            else if (selectedElement is InterfaceRowViewModel interfaceRowViewModel)
            {
                var source = this.allPorts.FirstOrDefault(x => x.ThingId == interfaceRowViewModel.SourceId, this.allPorts.First());
                return this.Products.FirstOrDefault(x => x.ThingId == source.ContainerId, this.Products.First());
            }
            else if (selectedElement is null)
            {
                return this.Products.First();
            }
            else
            {
                throw new ArgumentException($"the {selectedElement} is not a valid argument");
            }
        }

        /// <summary>
        /// Tries to get all the neighbours of a <see cref="ProductRowViewModel"/>
        /// </summary>
        /// <param name="productRow">the product to get the neighbours from</param>
        /// <returns>A <see cref="IEnumerable{ProductRowViewModel}"/> with the neighbours, or null if the product don't have neighbours</returns>
        /// <exception cref="Exception">if the source and target of a interface it's the same port</exception>
        public IEnumerable<ProductRowViewModel> GetNeighbours(ProductRowViewModel productRow)
        {
            if (this.HasChildren(productRow))
            {
                var ports = this.LoadChildren(productRow);
                var neighbours = new List<ProductRowViewModel>();

                foreach (var port in ports)
                {
                    var sourceInterface = this.Interfaces.FirstOrDefault(x => (x.SourceId == port.ThingId) && x.IsVisible, null);
                    var targetInterface = this.Interfaces.FirstOrDefault(x => (x.TargetId == port.ThingId) && x.IsVisible, null);

                    if (sourceInterface != null && targetInterface != null)
                    {
                        throw new Exception("Source and Target of an interface shall be a different port");
                    }
                    else if (sourceInterface != null)
                    {
                        var targetPort = this.allPorts.FirstOrDefault(p => p.ThingId == sourceInterface.TargetId, null);

                        if (targetPort != null)
                        {
                            var targetPortContainer = this.Products.FirstOrDefault(pr => pr.ThingId == targetPort.ContainerId, null);

                            if (targetPortContainer != null)
                            {
                                neighbours.Add(targetPortContainer);
                            }
                        }
                    }
                    else if (targetInterface != null)
                    {
                        var sourcePort = this.allPorts.FirstOrDefault(p => p.ThingId == targetInterface.SourceId, null);
                        if (sourcePort != null)
                        {
                            var sourcePortContainer = this.Products.FirstOrDefault(pr => pr.ThingId == sourcePort.ContainerId, null);

                            if (sourcePortContainer != null)
                            {
                                neighbours.Add(sourcePortContainer);
                            }
                        }
                    }
                }

                return neighbours.Distinct();
            }

            return null;
        }

        /// <summary>
        ///     Sets the selected model for this <see cref="IInterfaceViewViewModel" />
        /// </summary>
        /// <param name="model">the model to select</param>
        public void SetSelectedModel(Model model)
        {
            if (model is DiagramNode node && this.ProductsMap.ContainsKey(node))
            {
                this.SelectedElement = this.ProductsMap[node];
            }
            else if (model is DiagramPort port && this.PortsMap.ContainsKey(port))
            {
                this.SelectedElement = this.PortsMap[port];
            }
            else if (model is DiagramLink link && this.InterfacesMap.ContainsKey(link))
            {
                this.SelectedElement = this.InterfacesMap[link];
            }
        }

        /// <summary>
        ///     Creates a central node and his neighbours
        /// </summary>
        /// <param name="centerNode">the center node</param>
        public void CreateCentralNodeAndNeighbours(ProductRowViewModel centerProduct)
        {
            this.ProductsMap.Clear();
            this.PortsMap.Clear();
            this.InterfacesMap.Clear();

            var node = CreateNewNodeFromProduct(centerProduct);
            node.SetPosition(this.DiagramCenter.X, this.DiagramCenter.Y);

            this.CreateNeighboursAndPositionAroundProduct(centerProduct);
        }

        /// <summary>
        /// Creates neighbours for a product a place them in circle 
        /// </summary>
        /// <param name="productRowViewModel">the <see cref="ProductRowViewModel"/></param>
        public void CreateNeighboursAndPositionAroundProduct(ProductRowViewModel productRowViewModel)
        {
            if (this.ProductsMap.ContainsValue(productRowViewModel))
            {
                var node = this.ProductsMap.FirstOrDefault(x => x.Value == productRowViewModel).Key;
                node.IsExpanded = true;
                var r = 250.0;

                var neighbours = this.GetNeighbours(productRowViewModel);

                if (neighbours != null && neighbours.Any())
                {
                    var angle = Math.PI / 2.0;
                    var angleIncrement = (2.0 * Math.PI) / neighbours.Count();

                    foreach (var neighbour in neighbours)
                    {
                        var x = node.Position.X - r * Math.Cos(angle);
                        var y = node.Position.Y - r * Math.Sin(angle);

                        var neighbourNode = CreateNewNodeFromProduct(neighbour);
                        neighbourNode.SetPosition(x, y);

                        angle += angleIncrement;
                    }
                }
                this.CreateInterfacesLinks();
            }
        }

        /// <summary>
        ///     Creates a new node from a <see cref="ProductRowViewModel" />. The product is added to the Diagram an the corresponding maps are filled.
        /// </summary>
        /// <param name="product">the product for which the node will be created</param>
        /// <returns>the created <see cref="NodeModel" /></returns>
        public DiagramNode CreateNewNodeFromProduct(ProductRowViewModel product)
        {
            var node = new DiagramNode
            {
                Title = product.Name,
                HasComments = product.HasComment(),
                ThingId = product.ThingId
            };

            var nodeCanBeAdded = !this.ProductsMap.ContainsValue(product);

            if (nodeCanBeAdded)
            {
                this.ProductsMap.Add(node, product);
            }

            if (this.HasChildren(product) && nodeCanBeAdded)
            {
                var ports = this.LoadChildren(product);
                foreach (var port in ports)
                {
                    var index = ports.IndexOf(port);
                    var alignment = GetAproximateAlignment(index, ports.Count());
                    var portNode = new DiagramPort(node, alignment, port.Name);
                    node.AddPort(portNode);
                    portNode.Locked = true;
                    portNode.HasComments = port.HasComment();

                    switch (port.InterfaceEnd)
                    {
                        case "INPUT":
                            portNode.Direction = PortDirection.In;
                            break;
                        case "IN_OUT":
                            portNode.Direction = PortDirection.InOut;
                            break;
                        case "OUT":
                            portNode.Direction = PortDirection.Out;
                            break;
                    }

                    this.PortsMap.TryAdd(portNode, port);
                }
            }        

            return node;
        }

        /// <summary>
        /// Gets the aproximate alignment of the port depending its position in the node
        /// </summary>
        /// <param name="indexOfPort">the index of the port</param>
        /// <param name="totalPortsInNode"></param>
        /// <returns>the aproximate alignment of the port</returns>
        private static PortAlignment GetAproximateAlignment(int indexOfPort, int totalPortsInNode)
        {
            var increment = (360.0) / totalPortsInNode;
            var angle = increment * indexOfPort;

            if (angle >= 0 && angle < 22.5 || angle >= 337.5 && angle < 360.0)
            {
                return PortAlignment.Right;
            }
            else if (angle >= 22.5 && angle < 67.5)
            {
                return PortAlignment.TopRight;
            }
            else if (angle >= 67.5 && angle < 112.5)
            {
                return PortAlignment.Top;
            }
            else if (angle >= 112.5 && angle < 157.5)
            {
                return PortAlignment.TopLeft;
            }
            else if (angle >= 157.5 && angle < 202.5)
            {
                return PortAlignment.Left;
            }
            else if (angle >= 202.5 && angle < 247.5)
            {
                return PortAlignment.BottomLeft;
            }
            else if (angle >= 247.5 && angle < 292.5)
            {
                return PortAlignment.Bottom;
            }
            else if (angle >= 292.5 && angle < 337.5)
            {
                return PortAlignment.BottomRight;
            }

            return PortAlignment.Top;
        }

        /// <summary>
        ///     Creates the interfaces (links) between the ports of the products.
        /// </summary>
        public void CreateInterfacesLinks()
        {
            this.InterfacesMap.Clear();
            foreach (var interf in this.Interfaces)
            {
                var sourcePortRowVM = this.PortsMap.Values.FirstOrDefault(port => port.ThingId == interf.SourceId);
                var targetPortRowVM = this.PortsMap.Values.FirstOrDefault(port => port.ThingId == interf.TargetId);

                var sourcePortID = this.PortsMap.FirstOrDefault(item => item.Value == sourcePortRowVM).Key;
                var targetPortID = this.PortsMap.FirstOrDefault(item => item.Value == targetPortRowVM).Key;

                var source = this.PortsMap.Keys.FirstOrDefault(port => port == sourcePortID);
                var target = this.PortsMap.Keys.FirstOrDefault(port => port == targetPortID);

                if (source != null && target != null)
                {
                    var link = new DiagramLink(source, target);
                    link.Locked = true;
                    link.Router = Routers.Orthogonal;
                    link.PathGenerator = PathGenerators.Smooth;
                    link.SourceMarker = LinkMarker.Square;
                    link.TargetMarker = LinkMarker.Arrow;
                    link.HasComments = interf.HasComment();

                    if (!this.InterfacesMap.Keys.Any(x => x.TargetPort == link.TargetPort && x.SourcePort == link.SourcePort))
                    {
                        this.InterfacesMap.Add(link, interf);
                    }
                }
            }
        }

        /// <summary>
        ///     Upgrades the nodes with the actual data
        /// </summary>
        public bool TryUpdate(object updatedObject, bool hasComments)
        {
            if (updatedObject is ProductRowViewModel productRowViewModel && this.Products.Contains(productRowViewModel))
            {
                var indexOfProduct = this.Products.IndexOf(productRowViewModel);
                var keyValuePair = this.ProductsMap.First(x => x.Value == productRowViewModel);
                var node = this.ProductsMap.Keys.First(x => x == keyValuePair.Key);

                this.Products[indexOfProduct] = productRowViewModel;
                this.ProductsMap[keyValuePair.Key] = productRowViewModel;
                node.HasComments = hasComments;
                return true;
            }
            else if (updatedObject is PortRowViewModel portRowViewModel && this.allPorts.Contains(portRowViewModel))
            {
                var indexOfPort = this.allPorts.IndexOf(portRowViewModel);
                var keyValuePair = this.PortsMap.First(x => x.Value == portRowViewModel);
                var port = this.PortsMap.Keys.First(x => x == keyValuePair.Key);

                this.allPorts[indexOfPort] = portRowViewModel;
                this.PortsMap[keyValuePair.Key] = portRowViewModel;
                port.HasComments = hasComments;
                return true;
            }
            else if (updatedObject is InterfaceRowViewModel interfaceRowViewModel && this.Interfaces.Contains(interfaceRowViewModel))
            {
                var indexOfInterface = this.Interfaces.IndexOf(interfaceRowViewModel);
                var keyValuePair = this.InterfacesMap.First(x => x.Value == interfaceRowViewModel);
                var link = this.InterfacesMap.Keys.First(x => x == keyValuePair.Key);

                this.Interfaces[indexOfInterface] = interfaceRowViewModel;
                this.InterfacesMap[keyValuePair.Key] = interfaceRowViewModel;
                link.HasComments = hasComments;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     The <see cref="IDiagrammingConfigurationPopupViewModel" />
        /// </summary>
        public IDiagrammingConfigurationPopupViewModel DiagrammingConfigurationPopupViewModel { get; private set; }

        /// <summary>
        ///     Value indicating the user is currently saving the diagramming configuration
        /// </summary>
        public bool IsOnSavingMode
        {
            get => this.isOnSavingMode;
            set => this.RaiseAndSetIfChanged(ref this.isOnSavingMode, value);
        }

        /// <summary>
        ///     Opens the <see cref="DiagrammingConfigurationPopup" />
        /// </summary>
        public void OpenSavingPopup()
        {
            this.IsOnSavingMode = true;
        }

        /// <summary>
        ///     Saves current diagram layout
        /// </summary>
        public async void SaveCurrentDiagramLayout()
        {
            var layoutInformationDtos = this.ProductsMap.Keys.Select(x => new DiagramLayoutInformationDto { ThingId = x.ThingId, xPosition = x.Position.X, yPosition = x.Position.Y });
            var response = await this.diagrammingConfigurationService.SaveDiagramLayout(this.ProjectId, this.ReviewTaskId, this.DiagrammingConfigurationPopupViewModel.ConfigurationName, layoutInformationDtos);
            this.IsOnSavingMode = !response;
        }

        /// <summary>
        ///     Verifies that a <see cref="IHaveThingRowViewModel" /> has ports has children
        /// </summary>
        /// <param name="row">The <see cref="IHaveThingRowViewModel" /></param>
        /// <returns>True if has any port</returns>
        private bool HasPort(IHaveThingRowViewModel row)
        {
            return this.allPorts.Any(x => x.ElementContainer.Iid == row.ThingId);
        }

        /// <summary>
        ///     Applies a filter based on the <see cref="ConnectionToVisibilityState" />
        /// </summary>
        private void ApplyVisibility()
        {
            foreach (var portRowViewModel in this.allPorts)
            {
                portRowViewModel.IsVisible = this.PortVisibilityState?.CurrentState switch
                {
                    ConnectionToVisibilityState.NotConnected => !portRowViewModel.IsConnected,
                    ConnectionToVisibilityState.Connected => portRowViewModel.IsConnected,
                    _ => true
                };
            }

            this.Products = this.ProductVisibilityState?.CurrentState switch
            {
                ConnectionToVisibilityState.NotConnected => this.filteredProducts.Where(x => !this.HasPort(x)).ToList(),
                ConnectionToVisibilityState.Connected => this.filteredProducts.Where(this.HasPort).ToList(),
                _ => this.filteredProducts.ToList()
            };

            this.SetProductsVisibility(this.ShouldShowProducts);
        }

        /// <summary>
        ///     Initializes the filters criteria for rows
        /// </summary>
        private void InitializesFilter()
        {
            var availableRowFilters = new List<FilterModel>();

            var availableOwners = new List<DefinedThing>(this.allInterfaces
                .Select(x => x.InterfaceOwner)
                .OrderBy(x => x.ShortName)
                .DistinctBy(x => x.Iid));

            availableRowFilters.Add(new FilterModel
            {
                ClassKind = ClassKind.DomainOfExpertise,
                DisplayName = "Interface Owner",
                UseShortName = true,
                Values = availableOwners
            });

            var availableCategories = new List<DefinedThing>(this.allInterfaces
                .Select(x => x.NatureCategory)
                .OrderBy(x => x.Name)
                .DistinctBy(x => x.Iid));

            availableRowFilters.Add(new FilterModel
            {
                ClassKind = ClassKind.Category,
                DisplayName = "Interface Nature",
                Values = availableCategories
            });

            var availableProducts = new List<DefinedThing>(this.allProducts
                    .Select(x => x.Thing))
                .OrderBy(x => x.Name)
                .ToList();

            availableRowFilters.Add(new FilterModel
            {
                ClassKind = ClassKind.ElementDefinition,
                DisplayName = "Product",
                Values = availableProducts
            });

            this.FilterViewModel.InitializeProperties(availableRowFilters);
        }
    }
}
