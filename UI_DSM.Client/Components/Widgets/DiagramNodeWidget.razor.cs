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
    using Blazor.Diagrams.Components.Renderers;
    using Blazor.Diagrams.Core.Geometry;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.JSInterop;

    using System.Threading.Tasks;
    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.Model;
    using UI_DSM.Shared.Models;

    /// <summary>
    /// Partial class fot the <see cref="DiagramNodeWidget.razor"/>
    /// </summary>
    public partial class DiagramNodeWidget
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        private ElementReference DivReference { get; set; }

        /// <summary>
        /// Gets or sets the product node
        /// </summary>
        [Parameter]
        public DiagramNode Node { get; set; }

        /// <summary>
        /// Gets or sets the over class of the node.
        /// </summary>
        public string OverClass { get; set; } = string.Empty;

        public string ShapeClass { get; set; } = string.Empty;


        public double Width { get; set; }
        public double Height { get; set; }

        private List<Point> Points = new List<Point>();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                var dimensions = await this.JSRuntime.InvokeAsync<double[]>("GetNodeDimensions", this.DivReference);
                this.Width = dimensions[0];
                this.Height = dimensions[1];
                this.CalculateDistanceBetweenNodes();
            }
        }

        private void CalculateDistanceBetweenNodes()
        {
            this.CalculateEllipticalDistance();
        }

        private void CalculateRectangularDistance()
        {
            this.Points.Clear();
            var perimeter = 2.0 * this.Width + 2.0 * this.Height;
            var totalNodes = this.Node.Ports.Cast<DiagramPort>().Count();
            var distanceBetweenNodes = Math.Round(perimeter / totalNodes);

            var nodesInWidth = Math.Round(this.Width / distanceBetweenNodes);
            var nodesInHeight = Math.Round(this.Height / distanceBetweenNodes);

            var x0 = 0;
            var y0 = 0;

            nodesInWidth = nodesInWidth <= 0 ? 1 : nodesInWidth;
            nodesInHeight = nodesInHeight <= 0 ? 1 : nodesInHeight;

            //TOP
            for (int i = 0; i < nodesInWidth && this.Points.Count < totalNodes; i++)
            {
                this.Points.Add(new Point(x0 + distanceBetweenNodes * i, y0));
            }

            //RIGHT
            for (int i = 0; i < nodesInHeight && this.Points.Count < totalNodes; i++)
            {
                this.Points.Add(new Point(x0 + this.Width, y0 + distanceBetweenNodes * i));
            }

            //BOTTOM
            for (int i = 0; i < nodesInWidth && this.Points.Count < totalNodes; i++)
            {
                this.Points.Add(new Point(x0 + this.Width - distanceBetweenNodes * i, y0 + this.Height));
            }

            //LEFT
            for (int i = 0; i < nodesInHeight && this.Points.Count < totalNodes; i++)
            {
                this.Points.Add(new Point(x0, y0 + this.Height - distanceBetweenNodes * i));
            }
        }

        private void CalculateCircularDistance()
        {
            this.Points.Clear();

            this.ShapeClass = "round-node";

            var radius = Math.Max(this.Height/2.0, this.Width/2.0);

            var x0 = radius;
            var y0 = radius;

            var totalNodes = this.Node.Ports.Cast<DiagramPort>();
            var angle = 0.0;
            var angleIncrement = (Math.PI * 2.0) / totalNodes.Count();

            foreach (var port in this.Node.Ports.Cast<DiagramPort>())
            {
                var x = x0 + radius * Math.Cos(angle);
                var y = y0 + radius * Math.Sin(angle);

                this.Points.Add(new Point(x,y));

                angle += angleIncrement;
            }
        }

        private void CalculateEllipticalDistance()
        {
            this.Points.Clear();

            this.ShapeClass = "elliptical-node";

            var a = this.Width/2.0;
            var b = this.Height/2.0;
            var radius = b;

            var h = a;
            var k = b;

            var totalNodes = this.Node.Ports.Cast<DiagramPort>();
            var angle = 0.0;
            var angleIncrement = (Math.PI * 2.0) / totalNodes.Count();

            foreach (var port in this.Node.Ports.Cast<DiagramPort>())
            {
                var x = h + radius * Math.Cos(angle);
                var y = k + radius * Math.Sin(angle);
                //Use x or y to project
                var xf = Math.Sqrt((1.0 - ((y - k) * (y - k)) / (b * b)) * a * a) + h;
                var yf = Math.Sqrt((1.0 - ((x - h) * (x - h)) / (a * a)) * b * b) + k;

                this.Points.Add(new Point(xf, y));

                angle += angleIncrement;
            }
        }
    }
}
