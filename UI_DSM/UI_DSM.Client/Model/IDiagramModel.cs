// --------------------------------------------------------------------------------------------------------
// <copyright file="IDiagramModel.cs" company="RHEA System S.A.">
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
    /// <summary>
    /// Interface definition for the <see cref="DiagramNode"/>, <see cref="DiagramPort"/> and <see cref="DiagramLink"/>
    /// </summary>
    public interface IDiagramModel
    {
        /// <summary>
        /// Gets or sets if the model has comments.
        /// </summary>
        public bool HasComments { get; set; }
    }
}
