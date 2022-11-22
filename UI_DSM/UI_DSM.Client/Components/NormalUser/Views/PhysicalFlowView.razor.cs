// --------------------------------------------------------------------------------------------------------
// <copyright file="PhysicalFlowView.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.NormalUser.Views
{
    using Blazor.Diagrams.Core;
    using Blazor.Diagrams.Core.Models;
    using Blazor.Diagrams.Core.Models.Base;

    using DynamicData;

    using Microsoft.AspNetCore.Components.Web;

    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Enumerator;

    /// <summary>
    /// Component for the <see cref="View.PhysicalFlowView"/>
    /// </summary>
    public partial class PhysicalFlowView 
    {
        /// <summary>
        /// Gets or sets the diagram component.
        /// </summary>
        private Diagram Diagram { get; set; }

        /// <summary>
        /// The map collection from <see cref="NodeModel"/> ID to <see cref="ProductRowViewModel"/>
        /// </summary>
        private Dictionary<string, ProductRowViewModel> ProductsMap = new();

        /// <summary>
        /// The map collection from <see cref="PortModel"/> ID to <see cref="PortRowViewModel"/>
        /// </summary>
        private Dictionary<string, PortRowViewModel> PortsMap = new();

        /// <summary>
        /// The map collection from <see cref="LinkModel"/> ID to <see cref="InterfaceRowViewModel"/>
        /// </summary>
        private Dictionary<string, InterfaceRowViewModel> InterfacesMap = new();

        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.Diagram = new Diagram();

            this.ViewModel.OnFinishLoad += (s, e) =>
            {
                Random rand = new Random();
                int index = rand.Next(this.ViewModel.Products.Count);
                var firstNode = this.ViewModel.Products[index];
                this.CreateCentralNodeAndNeighbours(firstNode);
            };

            this.Diagram.MouseUp += Diagram_MouseUp;
        }

        /// <summary>
        /// MouseUp event for the diagram component.
        /// </summary>
        /// <param name="model">the model clicked (NodeModel,PortModel or LinkModel)</param>
        /// <param name="args">the args of the event</param>
        private void Diagram_MouseUp(Model model, MouseEventArgs args)
        {
            //Right button
            if(args.Button == 0)
            {
                if (model is NodeModel node)
                {
                    var selectedNode = this.Diagram.Nodes.FirstOrDefault(x => x.Id == node.Id, null);
                    var asociatedProduct = this.ProductsMap[selectedNode.Id];
                    this.CreateCentralNodeAndNeighbours(asociatedProduct);
                    this.ViewModel.SelectedElement = asociatedProduct;
                }
                else if (model is PortModel port)
                {
                    var selectedPort = this.Diagram.Nodes.Select(n => n.Ports.FirstOrDefault(x => x.Id == port.Id, null)).FirstOrDefault();
                    this.ViewModel.SelectedElement = this.PortsMap[selectedPort.Id];
                }
                else if (model is LinkModel link)
                {
                    var selectedLink = this.Diagram.Nodes.Select(n => n.Links.FirstOrDefault(x => x.Id == link.Id, null)).FirstOrDefault();
                    this.ViewModel.SelectedElement = this.InterfacesMap[selectedLink.Id];
                }
            }
        }

        /// <summary>
        /// Creates a central node and his neighbours
        /// </summary>
        /// <param name="centerNode">the center node</param>
        private void CreateCentralNodeAndNeighbours(ProductRowViewModel centerNode)
        {
            this.Diagram.Nodes.Clear();
            this.ProductsMap.Clear();
            this.PortsMap.Clear();
            this.InterfacesMap.Clear();

            var neighbours = this.ViewModel.GetNeighbours(centerNode);

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

            this.StateHasChanged();
        }

        /// <summary>
        /// Creates a new node from a <see cref="ProductRowViewModel"/>. The product is added to the Diagram an the corresponding maps are filled.
        /// </summary>
        /// <param name="product">the product for which the node will be created</param>
        /// <returns>the created <see cref="NodeModel"/></returns>
        private NodeModel CreateNewNodeFromProduct(ProductRowViewModel product)
        {
            var node = new NodeModel();
            node.Title = product.Name;
            
            this.Diagram.Nodes.Add(node);
            this.ProductsMap.Add(node.Id, product);

            if (this.ViewModel.HasChildren(product))
            {
                var ports = this.ViewModel.LoadChildren(product);
                foreach(var port in ports)
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
