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
    using System.Reactive.Linq;

    using Blazor.Diagrams.Core;
    using Blazor.Diagrams.Core.Geometry;
    using Blazor.Diagrams.Core.Models;

    using CDP4Common.CommonData;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.JSInterop;

    using ReactiveUI;

    using UI_DSM.Client.Components.Widgets;
    using UI_DSM.Client.Model;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    using Model = Blazor.Diagrams.Core.Models.Base.Model;

    /// <summary>
    ///     Component for the <see cref="View.PhysicalFlowView" />
    /// </summary>
    public partial class PhysicalFlowView : GenericBaseView<IInterfaceViewViewModel>, IReusableView, IDisposable
    {
        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     Value indicating if the view is completely initialized or not
        /// </summary>
        private bool isFullyInitialized;

        /// <summary>
        ///     Value indicating that the diagram should center on the first node
        /// </summary>
        private bool shouldCenter;

        /// <summary>
        ///     Gets or sets the diagram component.
        /// </summary>
        public Diagram Diagram { get; set; }

        /// <summary>
        ///     Gets or sets the JSRuntime to invoke JS
        /// </summary>
        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        /// <summary>
        ///     The reference of the diagram
        /// </summary>
        public ElementReference DiagramReference { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Diagram.MouseUp -= this.Diagram_MouseUp;
            this.Diagram.MouseDoubleClick -= this.Diagram_MouseDoubleClick;
            this.disposables.ForEach(x => x.Dispose());
            this.disposables.Clear();
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

            this.ViewModel = interfaceView.ViewModel;
            this.ViewModel.InitializeDiagram();
            this.RefreshDiagram();
            await this.HasChanged();

            this.IsLoading = false;
            return true;
        }

        /// <summary>
        ///     Tries to navigate to a corresponding item
        /// </summary>
        /// <param name="itemName">The name of the item to navigate to</param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task TryNavigateToItem(string itemName)
        {
            if (this.Diagram.Container is not { } rect)
            {
                return;
            }

            Point toFocus = null;
            var center = this.Diagram.GetRelativePoint(rect.Center.X, rect.Center.Y);

            if (this.ViewModel.ProductsMap.Any(x => string.Equals(x.Value.Id, itemName, StringComparison.InvariantCultureIgnoreCase)))
            {
                var node = this.ViewModel
                    .ProductsMap.First(x => string.Equals(x.Value.Id, itemName, StringComparison.InvariantCultureIgnoreCase))
                    .Key;

                toFocus = new Point(node.Position.X - center.X, node.Position.Y - center.Y);
            }
            else if (this.ViewModel.PortsMap.Any(x => string.Equals(x.Value.Id, itemName, StringComparison.InvariantCultureIgnoreCase)))
            {
                var port = this.ViewModel.PortsMap
                    .First(x => string.Equals(x.Value.Id, itemName, StringComparison.InvariantCultureIgnoreCase))
                    .Key;

                toFocus = new Point(port.Position.X - center.X, port.Position.Y - center.Y);
            }
            else if (this.ViewModel.InterfacesMap.Any(x => string.Equals(x.Value.Id, itemName, StringComparison.InvariantCultureIgnoreCase)))
            {
                var link = this.ViewModel.InterfacesMap
                    .First(x => string.Equals(x.Value.Id, itemName, StringComparison.InvariantCultureIgnoreCase))
                    .Key;

                toFocus = new Point(link.SourcePort!.Position.X - center.X, link.SourcePort!.Position.Y - center.Y);
            }

            if (toFocus != null)
            {
                this.Diagram.SetZoom(1);
                this.Diagram.SetPan(-toFocus.X, -toFocus.Y);
                this.Diagram.Refresh();
            }

            await Task.CompletedTask;
        }

        /// <summary>
        ///     Initialize the correspondant ViewModel for this component
        /// </summary>
        /// <param name="things">The collection of <see cref="Thing" /></param>
        /// <param name="projectId">The <see cref="UI_DSM.Shared.Models.Project" /> id</param>
        /// <param name="reviewId">The <see cref="UI_DSM.Shared.Models.Review" /> id</param>
        /// <param name="reviewTaskId">The <see cref="UI_DSM.Shared.Models.ReviewTask" /> id</param>
        /// <param name="prefilters">A collection of prefilters</param>
        /// <param name="additionnalColumnsVisibleAtStart">A collection of columns name that can be visible by default at start</param>
        /// <param name="participant">The current <see cref="Participant" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task InitializeViewModel(IEnumerable<Thing> things, Guid projectId, Guid reviewId, Guid reviewTaskId, List<string> prefilters, List<string> additionnalColumnsVisibleAtStart, Participant participant)
        {
            await base.InitializeViewModel(things, projectId, reviewId, reviewTaskId, prefilters, additionnalColumnsVisibleAtStart, participant);
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
        ///     Method that handles the refresh of the diagram
        /// </summary>
        public void RefreshDiagram()
        {
            this.ViewModel.FilterRowsForDiagram(this.ViewModel.FilterViewModel.GetSelectedFilters());
            this.Diagram.Nodes.Clear();
            this.Diagram.Links.Clear();
            this.ViewModel.CreateInterfacesLinks();

            this.ViewModel.ProductsMap.Keys.ToList().ForEach(node =>
            {
                node.RefreshAll();
                this.Diagram.Nodes.Add(node);
            });

            this.ViewModel.InterfacesMap.Keys.ToList().ForEach(link =>
            {
                link.Refresh();
                this.Diagram.Links.Add(link);
            });

            this.Diagram.Refresh();
            this.StateHasChanged();
        }

        /// <summary>
        ///     Mouse double click on a diagram's model
        /// </summary>
        /// <param name="model">the model double clicked</param>
        /// <param name="point">The <see cref="Point" /></param>
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
                this.Diagram.Refresh();
            }

            this.StateHasChanged();
        }

        /// <summary>
        ///     Mouse up on a diagram's model
        /// </summary>
        /// <param name="model">the model clicked</param>
        public void MouseUpOnComponent(Model model)
        {
            this.ViewModel.SetSelectedModel(model);
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
            this.Diagram.MouseDoubleClick += this.Diagram_MouseDoubleClick;
            this.Diagram.MouseUp += this.Diagram_MouseUp;
        }

        /// <summary>
        ///     Method invoked after each time the component has been rendered. Note that the component does
        ///     not automatically re-render after the completion of any returned <see cref="T:System.Threading.Tasks.Task" />, because
        ///     that would cause an infinite render loop.
        /// </summary>
        /// <param name="firstRender">
        ///     Set to <c>true</c> if this is the first time
        ///     <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> has been invoked
        ///     on this component instance; otherwise <c>false</c>.
        /// </param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
        /// <remarks>
        ///     The <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> and
        ///     <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRenderAsync(System.Boolean)" /> lifecycle methods
        ///     are useful for performing interop, or interacting with values received from <c>@ref</c>.
        ///     Use the <paramref name="firstRender" /> parameter to ensure that initialization work is only performed
        ///     once.
        /// </remarks>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (this.shouldCenter && this.Diagram.Container != null)
            {
                if (this.ViewModel.SelectedElement is IHaveThingRowViewModel thingRow)
                {
                    await this.TryNavigateToItem(thingRow.Id);
                }
                else if (this.Diagram.Nodes.OfType<DiagramNode>().FirstOrDefault(x => x.IsVisible) is { } node)
                {
                    var firstProduct = this.ViewModel.ProductsMap[node];
                    await this.TryNavigateToItem(firstProduct.Id);
                }

                this.shouldCenter = false;
                await this.InvokeAsync(this.StateHasChanged);
            }

            if (!this.IsLoading && !this.isFullyInitialized)
            {
                if (this.DiagramReference.Context == null)
                {
                    await this.InvokeAsync(this.StateHasChanged);
                    return;
                }

                var dimensions = await this.JsRuntime.InvokeAsync<int[]>("GetNodeDimensions", this.DiagramReference);
                var centerX = Math.Round(dimensions[0] / 2.0);
                var centerY = Math.Round(dimensions[1] / 2.0);
                this.ViewModel.DiagramCenter = new Point(centerX, centerY);

                this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.IsOnSavingMode)
                    .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));

                this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.IsOnLoadingMode)
                    .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));

                this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.IsOnLoadingMode)
                    .Subscribe(async x => await this.OnIsOnLoadingModeChanged(x)));

                this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.FilterViewModel.IsFilterVisible)
                    .Where(x => !x)
                    .Subscribe(_ => this.InvokeAsync(this.RefreshDiagram)));

                this.shouldCenter = true;
                this.isFullyInitialized = true;

                await this.InvokeAsync(this.StateHasChanged);
            }
        }

        /// <summary>
        ///     Handle the change of the <see cref="IsOnLoadingMode" /> variable
        /// </summary>
        /// <param name="value">The IsOnLoadingMode</param>
        /// <returns>A <see cref="Task" /></returns>
        private async Task OnIsOnLoadingModeChanged(bool value)
        {
            if (!value && this.ViewModel.HasLoadedConfiguration)
            {
                this.RefreshDiagram();
                this.shouldCenter = true;
                this.ViewModel.HasLoadedConfiguration = false;
            }

            await this.InvokeAsync(this.StateHasChanged);
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
        ///     MouseDoubleClick event for the diagram component
        /// </summary>
        /// <param name="model">the model clicked (NodeModel,PortModel or LinkModel)</param>
        /// <param name="args">the args of the event</param>
        private void Diagram_MouseDoubleClick(Model model, MouseEventArgs args)
        {
            if (args.Button == 0)
            {
                this.MouseDoubleClickOnModel(model, new Point(args.OffsetX, args.OffsetY));
            }
        }
    }
}
