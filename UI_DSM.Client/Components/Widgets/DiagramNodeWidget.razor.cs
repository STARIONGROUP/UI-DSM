// --------------------------------------------------------------------------------------------------------
// <copyright file="DiagramNodeWidget.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.Widgets
{
    using Blazor.Diagrams.Core.Geometry;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    using UI_DSM.Client.Model;

    /// <summary>
    ///     Partial class fot the <see cref="DiagramNodeWidget" />
    /// </summary>
    public partial class DiagramNodeWidget
    {
        /// <summary>
        ///     Gets the points used to position the ports in this node.
        /// </summary>
        private readonly List<Point> points = new();

        /// <summary>
        ///     Gets or sets the JSRuntime to invoke JS
        /// </summary>
        [Inject]
        private IJSRuntime JsRuntime { get; set; }

        /// <summary>
        ///     Gets or sets the reference of the div that contains the node
        /// </summary>
        private ElementReference DivReference { get; set; }

        /// <summary>
        ///     Gets or sets the product node
        /// </summary>
        [Parameter]
        public DiagramNode Node { get; set; }

        /// <summary>
        ///     Gets or sets the over class of the node.
        /// </summary>
        public string OverClass { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the width of the node
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        ///     Gets or sets the height of the node
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        ///     Gets the points used to position the ports in this node.
        /// </summary>
        public IReadOnlyList<Point> PortsCoordinates => this.points.AsReadOnly();

        /// <summary>
        ///     Calculates the positions of the corresponding ports
        /// </summary>
        public void CalculatePortsPositions()
        {
            this.points.Clear();
            this.CalculateRectangularDistance();
            this.Node.ReinitializePorts();
            this.Node.RefreshAll();
            this.Node.RefreshLinks();
            this.StateHasChanged();
        }

        /// <summary>
        ///     Method invoked after each time the component has been rendered. Note that the component does
        ///     not automatically re-render after the completion of any returned <see cref="Task" />, because
        ///     that would cause an infinite render loop.
        /// </summary>
        /// <param name="firstRender">
        ///     Set to <c>true</c> if this is the first time <see cref="OnAfterRender(bool)" /> has been invoked
        ///     on this component instance; otherwise <c>false</c>.
        /// </param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        /// <remarks>
        ///     The <see cref="OnAfterRender(bool)" /> and <see cref="OnAfterRenderAsync(bool)" /> lifecycle methods
        ///     are useful for performing interop, or interacting with values received from <c>@ref</c>.
        ///     Use the <paramref name="firstRender" /> parameter to ensure that initialization work is only performed
        ///     once.
        /// </remarks>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                var dimensions = await this.JsRuntime.InvokeAsync<int[]>("GetNodeDimensions", this.DivReference);

                this.Width = dimensions != null ? dimensions[0] : 100;
                this.Height = dimensions != null ? dimensions[1] : 80;
                this.CalculatePortsPositions();
            }
        }

        /// <summary>
        ///     Calculates the coordinates of the ports around the rectangle node
        /// </summary>
        private void CalculateRectangularDistance()
        {
            var p = this.Width / 2.0;
            var q = this.Height / 2.0;

            var totalNodes = this.Node.Ports.Cast<DiagramPort>().ToList();
            var t = 0.0;
            var tDelta = 2.0 * Math.PI / totalNodes.Count;

            foreach (var port in totalNodes)
            {
                var cos = Math.Cos(t);
                var sin = Math.Sin(t);

                var x = p + p * (Math.Abs(cos) * cos + Math.Abs(sin) * sin);
                var y = q + q * (Math.Abs(cos) * cos - Math.Abs(sin) * sin);

                this.points.Add(new Point(x, y));

                t += tDelta;
            }
        }
    }
}
