// --------------------------------------------------------------------------------------------------------
// <copyright file="DiagramLinkWidget.razor.cs" company="RHEA System S.A.">
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

    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.Model;

    /// <summary>
    /// Partial class for the component <see cref="DiagramLinkWidget"/>
    /// </summary>
    public partial class DiagramLinkWidget
    {
        /// <summary>
        /// Gets or sets the link between ports.
        /// </summary>
        [Parameter]
        public DiagramLink Link { get; set; }

        /// <summary>
        /// Gets or sets the category of the interface
        /// </summary>
        public InterfaceCategory Category { get; set; } = InterfaceCategory.Other;

        /// <summary>
        /// Gets the color of the interface, which depends on the <see cref="InterfaceCategory"/>
        /// </summary>
        public string Color => this.CategoryColorMap[this.Category];

        private readonly Dictionary<InterfaceCategory, string> CategoryColorMap = new Dictionary<InterfaceCategory, string>()
        {
            { InterfaceCategory.Other, "gray" },
            { InterfaceCategory.Power_Interfaces, "red" },
            { InterfaceCategory.Signal_Interfaces, "#00B0F0" },
            { InterfaceCategory.TM_TC_Interfaces, "green" },
            { InterfaceCategory.DataBus_Interfaces, "yellow" },
            { InterfaceCategory.Str_Interfaces, "black" },
            { InterfaceCategory.TC_Interfaces, "#843C0C" },
            { InterfaceCategory.Mechanisms_Interfaces, "#7030A0" },
            { InterfaceCategory.Prop_Interfaces, "#FFC000" },
            { InterfaceCategory.Comms_Interfaces, "#99FFCC" },
        };

        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            this.Link.Color = this.Color;

            if (this.Link.HasComments)
            {
                this.Link.Color = "green";
            }
        }
    }
}
