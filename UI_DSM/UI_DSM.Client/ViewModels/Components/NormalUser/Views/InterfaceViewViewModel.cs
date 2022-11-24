// --------------------------------------------------------------------------------------------------------
// <copyright file="InterfaceViewViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.NormalUser.Views
{
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using ReactiveUI;

    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.Extensions;
    using UI_DSM.Client.Model;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.ViewModels.App.ConnectionVisibilitySelector;
    using UI_DSM.Client.ViewModels.App.Filter;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;

    using System.Linq;
    using DynamicData;
    using Blazor.Diagrams.Core.Models.Base;
    using Blazor.Diagrams.Core.Models;

    /// <summary>
    ///     View model for the <see cref="Client.Components.NormalUser.Views.InterfaceView" /> component
    /// </summary>
    public class InterfaceViewViewModel : BaseViewViewModel, IInterfaceViewViewModel
    {
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
        ///     Backing field for <see cref="ShouldShowProducts" />
        /// </summary>
        private bool shouldShowProducts;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseViewViewModel" /> class.
        /// </summary>
        /// <param name="reviewItemService">The <see cref="IReviewItemService" /></param>
        /// <param name="filterViewModel">The <see cref="IFilterViewModel"/></param>
        public InterfaceViewViewModel(IReviewItemService reviewItemService, IFilterViewModel filterViewModel) : base(reviewItemService)
        {
            this.FilterViewModel = filterViewModel;
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
        public IConnectionVisibilitySelectorViewModel ProductVisibilityState { get; set; }

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
        public IConnectionVisibilitySelectorViewModel PortVisibilityState { get; set; }

        /// <summary>
        ///     A collection of available <see cref="FilterModel" /> for rows
        /// </summary>
        public List<FilterModel> AvailableRowFilters { get; } = new();

        /// <summary>
        /// A list of the nodes in the <see cref="Diagram"/>
        /// </summary>
        public List<NodeModel> ProductNodes { get; } = new();

        /// <summary>
        /// The map collection from <see cref="NodeModel"/> ID to <see cref="ProductRowViewModel"/>
        /// </summary>
        public Dictionary<string, ProductRowViewModel> ProductsMap { get; } = new();

        /// <summary>
        /// The map collection from <see cref="PortModel"/> ID to <see cref="PortRowViewModel"/>
        /// </summary>
        public Dictionary<string, PortRowViewModel> PortsMap { get; } = new();

        /// <summary>
        /// The map collection from <see cref="LinkModel"/> ID to <see cref="InterfaceRowViewModel"/>
        /// </summary>
        public Dictionary<string, InterfaceRowViewModel> InterfacesMap { get; } = new();

        /// <summary>
        /// Event fired when the state of the component needs to change.
        /// </summary>
        public Action OnCentralNodeChanged { get; set; }

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
        /// <returns>A <see cref="Task" /></returns>
        public override async Task InitializeProperties(IEnumerable<Thing> things, Guid projectId, Guid reviewId)
        {
            await base.InitializeProperties(things, projectId, reviewId);

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
                .Select(x => new InterfaceRowViewModel(x, reviewItems.FirstOrDefault(ri => ri.ThingId == x.Iid))));

            this.filteredProducts = new List<ProductRowViewModel>(this.allProducts.OrderBy(x => x.Thing.Name));
            this.Interfaces = new List<InterfaceRowViewModel>(this.allInterfaces.OrderBy(x => x.Id));
            this.ApplyVisibility();
            this.InitializesFilter();

            //TODO: the selection of the product shall not be random. It will be the selected thing.
            var firstNode = this.Products.FirstOrDefault(p =>
            {
                var neighbours = this.GetNeighbours(p);
                if (neighbours is not null && neighbours.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }, this.Products.First());

            this.CreateCentralNodeAndNeighbours(firstNode);
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
                    var sourceInterface = this.Interfaces.FirstOrDefault(i => i.SourceId.ToString() == port.Id, null);

                    var targetInterface = this.Interfaces.FirstOrDefault(i => i.TargetId.ToString() == port.Id, null);

                    if (sourceInterface != null && targetInterface != null)
                    {
                        throw new Exception("Source and Target of an interface shall be a different port");
                    }
                    else if (sourceInterface != null)
                    {
                        var targetPort = this.allPorts.FirstOrDefault(p => p.Id == sourceInterface.TargetId.ToString(), null);
                        if (targetPort != null)
                        {
                            var targetPortContainer = this.Products.FirstOrDefault(pr => pr.Id == targetPort.ContainerId.ToString(), null);

                            if (targetPortContainer != null)
                            {
                                neighbours.Add(targetPortContainer);
                            }
                        }
                    }
                    else if (targetInterface != null)
                    {
                        var sourcePort = this.allPorts.FirstOrDefault(p => p.Id == targetInterface.SourceId.ToString(), null);
                        if (sourcePort != null)
                        {
                            var sourcePortContainer = this.Products.FirstOrDefault(pr => pr.Id == sourcePort.ContainerId.ToString(), null);

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
        /// Sets the selected model for this <see cref="IInterfaceViewViewModel"/>
        /// </summary>
        /// <param name="model">the model to select</param>
        public void SetSelectedModel(Model model)
        {
            if (model is NodeModel node)
            {
                var asociatedProduct = this.ProductsMap[node.Id];
                this.CreateCentralNodeAndNeighbours(asociatedProduct);
                this.SelectedElement = asociatedProduct;
            }
            else if (model is PortModel port)
            {
                this.SelectedElement = this.PortsMap[port.Id];
            }
            else if (model is LinkModel link)
            {
                this.SelectedElement = this.InterfacesMap[link.Id];
            }
        }

        /// <summary>
        /// Creates a central node and his neighbours
        /// </summary>
        /// <param name="centerNode">the center node</param>
        public void CreateCentralNodeAndNeighbours(ProductRowViewModel centerNode)
        {
            this.ProductNodes.Clear();
            this.ProductsMap.Clear();
            this.PortsMap.Clear();
            this.InterfacesMap.Clear();

            var neighbours = this.GetNeighbours(centerNode);

            var cx = 800.0;
            var cy = 500.0;
            var r = 200.0;

            var node = CreateNewNodeFromProduct(centerNode);
            node.SetPosition(cx, cy);

            if (neighbours != null)
            {
                var angle = 0.0;
                var angleIncrement = neighbours.Count() > 0 ? (2.0 * Math.PI) / neighbours.Count() : 0;

                foreach (var neighbour in neighbours)
                {
                    var x = cx + r * Math.Cos(angle);
                    var y = cy + r * Math.Sin(angle);

                    Console.WriteLine($"The node {neighbour.Name} is positioned in {x},{y}");

                    var neighbourNode = CreateNewNodeFromProduct(neighbour);
                    neighbourNode.SetPosition(x, y);

                    angle += angleIncrement;
                }
            }

            
            Task.Run(() => this.OnCentralNodeChanged);
        }

        /// <summary>
        /// Creates a new node from a <see cref="ProductRowViewModel"/>. The product is added to the Diagram an the corresponding maps are filled.
        /// </summary>
        /// <param name="product">the product for which the node will be created</param>
        /// <returns>the created <see cref="NodeModel"/></returns>
        public NodeModel CreateNewNodeFromProduct(ProductRowViewModel product)
        {
            var node = new NodeModel();
            node.Title = product.Name;

            this.ProductNodes.Add(node);
            this.ProductsMap.Add(node.Id, product);

            if (this.HasChildren(product))
            {
                var ports = this.LoadChildren(product);
                foreach (var port in ports)
                {
                    var index = ports.IndexOf(port);
                    var portNode = node.AddPort((PortAlignment)index);
                    portNode.Locked = true;
                    this.PortsMap.Add(portNode.Id, port);
                }
            }

            return node;
        }
    }
}
