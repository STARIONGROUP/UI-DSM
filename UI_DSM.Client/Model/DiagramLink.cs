// --------------------------------------------------------------------------------------------------------
// <copyright file="DiagramLink.cs" company="RHEA System S.A.">
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

    using CDP4Common.CommonData;
    using CDP4Common.SiteDirectoryData;

    using UI_DSM.Client.Enumerator;

    /// <summary>
    ///     Derived class for holding data for the <see cref="Components.Widgets.DiagramLinkWidget" />
    /// </summary>
    public class DiagramLink : LinkModel, IDiagramModel
    {
        /// <summary>
        ///     Creates a new instance of type <see cref="DiagramLink" />
        /// </summary>
        /// <param name="sourcePort">the source of the link</param>
        /// <param name="targetPort">the target of the link</param>
        public DiagramLink(PortModel sourcePort, PortModel targetPort = null) : base(sourcePort, targetPort)
        {
        }

        /// <summary>
        ///     Gets or sets the visibility of the link
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        ///     Gets or sets the <see cref="Thing" /> Id
        /// </summary>
        public Guid ThingId { get; set; }

        /// <summary>
        ///     The <see cref="InterfaceCategory" />
        /// </summary>
        public InterfaceCategory InterfaceCategory { get; set; }

        /// <summary>
        ///     Gets or sets if the model has comments.
        /// </summary>
        public bool HasComments { get; set; }
    }
}
