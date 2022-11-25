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
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.Model;

    /// <summary>
    /// Partial class fot the <see cref="DiagramNodeWidget.razor"/>
    /// </summary>
    public partial class DiagramNodeWidget
    {
        /// <summary>
        /// Gets or sets the product node
        /// </summary>
        [Parameter]
        public DiagramNode Node { get; set; }

        /// <summary>
        /// Gets or sets the class of the node.
        /// </summary>
        public string NodeClass { get; set; } = string.Empty;

        /// <summary>
        /// OnMouseOver event for the node
        /// </summary>
        /// <param name="e">the args of the event</param>
        public void OnMouseOver(MouseEventArgs e)
        {
            this.NodeClass = "over";
            this.StateHasChanged();
        }

        /// <summary>
        /// OnMouseOut event for the node
        /// </summary>
        /// <param name="e">the args of the event</param>
        public void OnMouseOut(MouseEventArgs e)
        {
            this.NodeClass = string.Empty;
            this.StateHasChanged();
        }

    }
}
