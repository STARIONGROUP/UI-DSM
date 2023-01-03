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
    using Blazor.Diagrams.Core.Geometry;
    using Blazor.Diagrams.Core.Models;
    using Blazor.Diagrams.Core.Models.Base;

    using CDP4Common.CommonData;
    
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.JSInterop;
    
    using UI_DSM.Client.Components.Widgets;
    using UI_DSM.Client.Model;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Shared.Enumerator;

    /// <summary>
    ///     Component for the <see cref="View.PhysicalFlowView" />
    /// </summary>
    public partial class PhysicalFlowView : GenericBaseView<IInterfaceViewViewModel>, IReusableView, IDisposable
    {
        /// <summary>
        ///     Gets or sets the diagram component.
        /// </summary>
        public Diagram Diagram { get; set; }

        /// <summary>
        /// Gets or sets the JSRuntime to invoke JS 
        /// </summary>
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// The reference of the diagram
        /// </summary>
        public ElementReference DiagramReference { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Diagram.MouseDoubleClick -= Diagram_MouseDoubleClick;
            this.Diagram.MouseUp -= this.Diagram_MouseUp;
        }

        /// <summary>
        ///     Tries to copy components from another <see cref="BaseView" />
        /// </summary>
        /// <param name="otherView">The other <see cref="BaseView" /></param>
        /// <returns>Value indicating if it could copy components</returns>
        public async Task<bool> CopyComponents(BaseView otherView)
        {
            if (otherView is not GenericBaseView<IInterfaceViewViewModel> interfaceView)
            {
                return false;
            }

            var dimensions = await this.JSRuntime.InvokeAsync<int[]>("GetNodeDimensions", this.DiagramReference);
            var centerX = Math.Round(dimensions[0] / 2.0);
            var centerY = Math.Round(dimensions[1] / 2.0);
            this.ViewModel.DiagramCenter = new Blazor.Diagrams.Core.Geometry.Point(centerX,centerY);
            this.ViewModel = interfaceView.ViewModel;
            await this.HasChanged();

            this.ViewModel.InitializeDiagram();
            this.RefreshDiagram();
            this.Diagram.ZoomToFit();
            this.IsLoading = false;
            return true;
        }

        /// <summary>
        ///     Initialize the correspondant ViewModel for this component
        /// </summary>
        /// <param name="things">The collection of <see cref="Thing" /></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task InitializeViewModel(IEnumerable<Thing> things, Guid projectId, Guid reviewId)
        {
            await base.InitializeViewModel(things, projectId, reviewId);
            this.ViewModel.InitializeDiagram();
            this.RefreshDiagram();
            this.IsLoading = false;
        }

        /// <summary>
        ///     The comments of the selected object have changed
        /// </summary>
        /// <param name="updatedObject">the object that the objects have changed</param>
        /// <param name="hasComments">if have comments or not</param>
        public void SelectedElementChangedComments(object updatedObject, bool hasComments)
        {
            this.ViewModel.TryUpdate(updatedObject, hasComments);
            this.RefreshDiagram();
        }

        /// <summary>
        ///     Method that handles the on central node changed event.
        /// </summary>
        public void RefreshDiagram()
        {
            this.Diagram.Nodes.Clear();
            this.ViewModel.ProductsMap.Keys.ToList().ForEach(node => { node.RefreshAll(); this.Diagram.Nodes.Add(node); });
            this.ViewModel.InterfacesMap.Keys.ToList().ForEach(link => { link.Refresh();  this.Diagram.Links.Add(link); });
            this.Diagram.Refresh();
            this.StateHasChanged();
        }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        /// </summary>
        protected override void OnInitialized()
        {
            this.IsLoading = true;
            base.OnInitialized();

            this.Diagram = new Diagram
            {
                Options =
                {
                    GridSize = 1,
                    AllowMultiSelection = false,
                    DefaultNodeComponent = typeof(DiagramNodeWidget)
                }
            };

            this.Diagram.RegisterModelComponent<DiagramLink, DiagramLinkWidget>();
            this.Diagram.MouseDoubleClick += Diagram_MouseDoubleClick;
            this.Diagram.MouseUp += this.Diagram_MouseUp;
        }

        /// <summary>
        /// Mouse double click event for the diagram component
        /// </summary>
        /// <param name="model">the model double clicked</param>
        /// <param name="args">the args of the event</param>
        private void Diagram_MouseDoubleClick(Model model, MouseEventArgs args)
        {
            if (args.Button == 0)
            {
                this.MouseDoubleClickOnModel(model, new Point(args.OffsetX, args.OffsetY));
            }            
        }

        /// <summary>
        ///     MouseUp event for the diagram component
        /// </summary>
        /// <param name="model">the model clicked (NodeModel,PortModel or LinkModel)</param>
        /// <param name="args">the args of the event</param>
        private void Diagram_MouseUp(Model model, MouseEventArgs args)
        {
            //Right button
            if (args.Button == 0)
            {
                this.MouseUpOnComponent(model);
            }
        }

        /// <summary>
        /// Mouse double click on a diagram's model
        /// </summary>
        /// <param name="model">the model double clicked</param>
        public void MouseDoubleClickOnModel(Model model, Point point)
        {
            if (model is DiagramNode diagramNode && this.ViewModel.ProductsMap.ContainsKey(diagramNode))
            {
                var productRowViewModel = this.ViewModel.ProductsMap[diagramNode];
                this.ViewModel.CreateNeighboursAndPositionAroundProduct(productRowViewModel);
                this.RefreshDiagram();
            }
            else if (model is DiagramLink diagramLink)
            {
                var vertex = new LinkVertexModel(diagramLink, point);
                diagramLink.Vertices.Add(vertex);
                this.ViewModel.CreateInterfacesLinks();
                this.Diagram.Refresh();
            }
        }

        /// <summary>
        /// Mouse up on a diagram's model
        /// </summary>
        /// <param name="model">the model clicked</param>
        public void MouseUpOnComponent(Model model)
        {
            this.ViewModel.SetSelectedModel(model);
        }
    }
}
