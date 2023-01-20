// --------------------------------------------------------------------------------------------------------
// <copyright file="DiagramPort.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Model
{
    using Blazor.Diagrams.Core.Models;
    using UI_DSM.Client.Enumerator;

    /// <summary>
    /// Derived class for holding data for the <see cref="Components.Widgets.DiagramNodeWidget.razor"/>
    /// </summary>
    public class DiagramPort : PortModel, IDiagramModel
    {
        /// <summary>
        /// Gets or sets if the model has comments.
        /// </summary>
        public bool HasComments { get; set; }

        /// <summary>
        /// Gets or sets the text of the port.
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the direction of the port.
        /// </summary>
        public PortDirection Direction { get; set; }

        /// <summary>
        /// Creates a new instance of type <see cref="DiagramPort"/>
        /// </summary>
        /// <param name="parent">the parent of the port</param>
        /// <param name="alignment">the alignment of the port inside the parent</param>
        public DiagramPort(NodeModel parent, PortAlignment alignment) : base(parent, alignment)
        {
            
        }

        /// <summary>
        /// Creates a new instance of type <see cref="DiagramPort"/>
        /// </summary>
        /// <param name="parent">the parent of the port</param>
        /// <param name="alignment">the alignment of the port inside the parent</param>
        /// <param name="text">the text that the node can show</param>
        public DiagramPort(NodeModel parent, PortAlignment alignment, string text) : base(parent, alignment)
        {
            this.Text = text;
        }
    }
}
