// --------------------------------------------------------------------------------------------------------
// <copyright file="Artifact.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
//
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
//
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
//
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

// --------------------------------------------------------------------------------------------------------
// ------------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!------------
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Shared.DTO.Models
{
    using UI_DSM.Shared.Models;

    /// <summary>
    ///    The Data Transfer Object representing the <see cref="Artifact" /> class.
    /// </summary>
    public abstract partial class ArtifactDto : EntityDto
    {
        /// <summary>
        ///    Initializes a new <see cref="ArtifactDto" /> class.
        /// </summary>
        protected ArtifactDto()
        {
        }

        /// <summary>
        ///    Initializes a new <see cref="ArtifactDto" /> class.
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the represented <see cref="Entity" /></param>
        protected ArtifactDto(Guid id) : base(id)
        {
        }

        /// <summary>
        ///    Gets or sets the FileName of the Artifact
        /// </summary>
        public string FileName { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------