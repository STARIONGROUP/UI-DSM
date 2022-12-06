// --------------------------------------------------------------------------------------------------------
// <copyright file="Artifact.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     Represents a file artifact that is uploaded to the system and linked to a project.
    /// </summary>
    [Table(nameof(Artifact))]
    public abstract class Artifact : Entity
    {
        /// <summary>
        ///     Initializes a new <see cref="Entity" />
        /// </summary>
        protected Artifact()
        {
            this.Reviews = new List<Review>();
        }

        /// <summary>
        ///     Inilializes a new <see cref="Entity" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="Entity" /></param>
        protected Artifact(Guid id) : base(id)
        {
            this.Reviews = new List<Review>();
        }

        /// <summary>
        ///     The name of the represented file
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string FileName { get; set; }

        /// <summary>
        ///     A collection of <see cref="Review" /> where this <see cref="Artifact" /> is used
        /// </summary>
        public List<Review> Reviews { get; set; }

        /// <summary>
        ///     Resolve the properties of the current <see cref="Entity" /> from its <see cref="EntityDto" /> counter-part
        /// </summary>
        /// <param name="entityDto">The source <see cref="EntityDto" /></param>
        /// <param name="resolvedEntity">A <see cref="Dictionary{TKey,TValue}" /> of all others <see cref="Entity" /></param>
        public override void ResolveProperties(EntityDto entityDto, Dictionary<Guid, Entity> resolvedEntity)
        {
            if (entityDto is not ArtifactDto annotationDto)
            {
                throw new InvalidOperationException($"The DTO {entityDto.GetType()} does not match with the current Artifact POCO");
            }

            this.FileName = annotationDto.FileName;
        }
    }
}
