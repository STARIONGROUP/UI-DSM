// --------------------------------------------------------------------------------------------------------
// <copyright file="IInterfaceViewViewModel.cs" company="RHEA System S.A.">
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
    using Blazor.Diagrams.Core;
    using Blazor.Diagrams.Core.Models;

    using CDP4Common.CommonData;

    using UI_DSM.Client.Model;
    using UI_DSM.Client.ViewModels.App.ConnectionVisibilitySelector;
    using UI_DSM.Client.ViewModels.App.Filter;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;

    /// <summary>
    ///     Interface definition for <see cref="InterfaceViewViewModel" />
    /// </summary>
    public interface IInterfaceViewViewModel : IBaseViewViewModel
    {
        /// <summary>
        ///     A collection <see cref="IBelongsToInterfaceView" />
        /// </summary>
        IEnumerable<IBelongsToInterfaceView> Data { get; }

        /// <summary>
        ///     A collection of <see cref="ProductRowViewModel" />
        /// </summary>
        List<ProductRowViewModel> Products { get; }

        /// <summary>
        ///     The <see cref="IConnectionVisibilitySelectorViewModel" /> for products
        /// </summary>
        IConnectionVisibilitySelectorViewModel ProductVisibilityState { get; set; }

        /// <summary>
        ///     The <see cref="IConnectionVisibilitySelectorViewModel" /> for ports
        /// </summary>
        IConnectionVisibilitySelectorViewModel PortVisibilityState { get; set; }

        /// <summary>
        ///     A collection of filtered <see cref="InterfaceRowViewModel" />
        /// </summary>
        List<InterfaceRowViewModel> Interfaces { get; }

        /// <summary>
        ///     Value asserting if products has to been shown
        /// </summary>
        bool ShouldShowProducts { get; }

        /// <summary>
        /// A list of the <see cref="NodeModel"/> in the <see cref="Diagram"/>
        /// </summary>
        List<NodeModel> ProductNodes { get; } 

        /// <summary>
        /// A list of the <see cref="PortModel"/> in the <see cref="Diagram"/>
        /// </summary>
        List<PortModel> PortNodes { get; } 

        /// <summary>
        /// A list of the <see cref="LinkModel"/> in the <see cref="Diagram"/>
        /// </summary>
        List<LinkModel> LinkNodes { get; } 

        /// <summary>
        /// The map collection from <see cref="NodeModel"/> ID to <see cref="ProductRowViewModel"/>
        /// </summary>
        Dictionary<string, ProductRowViewModel> ProductsMap { get; }

        /// <summary>
        /// The map collection from <see cref="PortModel"/> ID to <see cref="PortRowViewModel"/>
        /// </summary>
        Dictionary<string, PortRowViewModel> PortsMap { get; }

        /// <summary>
        /// The map collection from <see cref="LinkModel"/> ID to <see cref="InterfaceRowViewModel"/>
        /// </summary>
        Dictionary<string, InterfaceRowViewModel> InterfacesMap { get; }

        /// <summary>
        ///     The <see cref="IFilterViewModel" />
        /// </summary>
        IFilterViewModel FilterViewModel { get; }
        
        /// <summary>
        /// Indicates if the diagram should be updated or not
        /// </summary>
        public bool ShouldUpdateDiagram { get; set; }

        /// <summary>
        /// Gets the central node of the <see cref="IInterfaceViewViewModel"/>
        /// </summary>
        public NodeModel CentralNode { get; }

        /// <summary>
        ///     Filters current rows
        /// </summary>
        /// <param name="selectedFilters">The selected filters</param>
        void FilterRows(IReadOnlyDictionary<ClassKind, List<FilterRow>> selectedFilters);

        /// <summary>
        ///     Handle the change of visibility
        /// </summary>
        void OnVisibilityStateChanged();

        /// <summary>
        ///     Asserts that a <see cref="ProductRowViewModel" /> has <see cref="PortRowViewModel" /> children
        /// </summary>
        /// <param name="productRow">The <see cref="ProductRowViewModel" /></param>
        /// <returns>True if contains any visible port</returns>
        bool HasChildren(ProductRowViewModel productRow);

        /// <summary>
        ///     Asserts that a <see cref="PortRowViewModel" /> has <see cref="InterfaceRowViewModel" /> children
        /// </summary>
        /// <param name="portRow">The <see cref="PortRowViewModel" /></param>
        /// <returns>True if contains any visible interfaces</returns>
        bool HasChildren(PortRowViewModel portRow);

        /// <summary>
        ///     Loads all <see cref="PortRowViewModel" /> children rows of a <see cref="ProductRowViewModel" />
        /// </summary>
        /// <param name="productRow">The <see cref="ProductRowViewModel" /></param>
        /// <returns>A collection of <see cref="PortRowViewModel" /></returns>
        IEnumerable<PortRowViewModel> LoadChildren(ProductRowViewModel productRow);

        /// <summary>
        ///     Loads all <see cref="InterfaceRowViewModel" /> children rows of a <see cref="PortRowViewModel" />
        /// </summary>
        /// <param name="portRow">The <see cref="PortRowViewModel" /></param>
        /// <returns>A collection of <see cref="InterfaceRowViewModel" /></returns>
        IEnumerable<InterfaceRowViewModel> LoadChildren(PortRowViewModel portRow);

        /// <summary>
        ///     Modifies the datasource to display based on the visibility of products
        /// </summary>
        /// <param name="visibility">The new visibility</param>
        void SetProductsVisibility(bool visibility);

        /// <summary>
        /// Initializes the diagram with the selected element as a center node.
        /// </summary>
        void InitializeDiagram();

        /// <summary>
        /// Selects the first central product depending on the selected element.
        /// </summary>
        /// <param name="selectedElement">the selected element</param>
        /// <returns>the selected <see cref="ProductRowViewModel"/></returns>
        /// <exception cref="ArgumentException">if the selected element is not null and is not of type <see cref="ElementBaseRowViewModel"/></exception>
        ProductRowViewModel SelectedFirstProductByCloserSelectedItem(object selectedElement);

        /// <summary>
        /// Tries to get all the neighbours of a <see cref="ProductRowViewModel"/>
        /// </summary>
        /// <param name="productRow">the product to get the neighbours from</param>
        /// <returns>A <see cref="IEnumerable{ProductRowViewModel}"/> with the neighbours, or null if the product don't have neighbours</returns>
        /// <exception cref="Exception">if the source and target of a interface it's the same port</exception>
        IEnumerable<ProductRowViewModel> GetNeighbours(ProductRowViewModel productRow);

        /// <summary>
        /// Creates a central node and his neighbours
        /// </summary>
        /// <param name="centerNode">the center node</param>
        void CreateCentralNodeAndNeighbours(ProductRowViewModel centerNode);

        /// <summary>
        /// Creates a new node from a <see cref="ProductRowViewModel"/>. The product is added to the Diagram an the corresponding maps are filled.
        /// </summary>
        /// <param name="product">the product for which the node will be created</param>
        /// <returns>the created <see cref="NodeModel"/></returns>
        DiagramNode CreateNewNodeFromProduct(ProductRowViewModel product);

        /// <summary>
        /// Creates the links for the <see cref="InterfaceRowViewModel"/>
        /// </summary>
        void CreateInterfacesLinks();

        /// <summary>
        /// Sets the selected model for this <see cref="IInterfaceViewViewModel"/>
        /// </summary>
        /// <param name="model">the model to select</param>
        void SetSelectedModel(Blazor.Diagrams.Core.Models.Base.Model model);

        /// <summary>
        /// Sets the new central node for this <see cref="IInterfaceViewViewModel"/>
        /// </summary>
        /// <param name="nodeModel">the new central node</param>
        void SetCentralNodeModel(NodeModel nodeModel);

        /// <summary>
        /// Upgrades the nodes with the actual data
        /// </summary>
        void UpdateNodesData();

        /// <summary>
        /// Event fired when the state of the component needs to change.
        /// </summary>
        Action OnCentralNodeChanged { get; set; }
    }
}
