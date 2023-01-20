// --------------------------------------------------------------------------------------------------------
// <copyright file="ArtifactDto.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Shared.DTO.Models
{
    using UI_DSM.Shared.Models;

    /// <summary>
    ///    The Data Transfer Object representing the <see cref="Artifact" /> class.
    /// </summary>
    public partial class ArtifactDto
    {
        /// <summary>
        ///     Includes common properties for <see cref="ArtifactDto" /> from an <see cref="Artifact" />
        /// </summary>
        /// <param name="artifact">The <see cref="Artifact" /></param>
        public void IncludeCommonProperties(Artifact artifact)
        {
            this.FileName = artifact.FileName;
        }
    }
}
