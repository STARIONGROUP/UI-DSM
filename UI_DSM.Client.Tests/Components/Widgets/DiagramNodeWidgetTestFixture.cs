// --------------------------------------------------------------------------------------------------------
// <copyright file="DiagramNodeWidgetTestFixture.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Jaime Bernar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Tests.Components.Widgets
{
    using AngleSharpWrappers;
    using Bunit;
    using DevExpress.Utils.StructuredStorage.Internal.Writer;
    using NUnit.Framework;
    using UI_DSM.Client.Components.Widgets;
    using UI_DSM.Client.Model;
    using UI_DSM.Client.Tests.Helpers;
    
    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class DiagramNodeWidgetTestFixture
    {
        private TestContext context;
        private DiagramNodeWidget widget;
        private DiagramNode node;

        [SetUp]
        public void SetUp()
        {
            try
            {
                this.context = new TestContext();
                this.context.AddDevExpressBlazorTesting();
                this.context.ConfigureDevExpressBlazor();
                this.context.JSInterop.Mode = JSRuntimeMode.Loose;
                this.context.JSInterop.Setup<int[]>("GetNodeDimensions").SetResult(new int[] { 100, 80 });

                this.node = new DiagramNode();
                this.node.AddPort(new DiagramPort(this.node, Blazor.Diagrams.Core.Models.PortAlignment.Top, "top"));
                this.node.AddPort(new DiagramPort(this.node, Blazor.Diagrams.Core.Models.PortAlignment.Bottom, "bottom"));
                this.node.AddPort(new DiagramPort(this.node, Blazor.Diagrams.Core.Models.PortAlignment.Left, "left"));
                this.node.AddPort(new DiagramPort(this.node, Blazor.Diagrams.Core.Models.PortAlignment.Right, "right"));

                var renderer = this.context.RenderComponent<DiagramNodeWidget>(parameters =>
                {
                    parameters.Add(p => p.Node, node);
                });
                //var renderer = this.context.RenderComponent<DiagramNodeWidget>();
                this.widget = renderer.Instance;
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }

        [Test]
        public void VerifyThatPortsCanBePositioned()
        {
            //var diagramNode = new DiagramNodeWidget();
            //diagramNode.Node = this.node;
            //diagramNode.

            //diagramNode.CalculatePortsPositions();
        }


        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }
    }
}
