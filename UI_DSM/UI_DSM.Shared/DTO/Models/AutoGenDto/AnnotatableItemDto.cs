// --------------------------------------------------------------------------------------------------------
// <copyright file="AnnotatableItem.cs" company="RHEA System S.A.">
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
    ///    The Data Transfer Object representing the <see cref="AnnotatableItem" /> class.
    /// </summary>
    public abstract partial class AnnotatableItemDto : EntityDto
    {
        /// <summary>
        ///    Initializes a new <see cref="AnnotatableItemDto" /> class.
        /// </summary>
        protected AnnotatableItemDto()
        {
            this.Annotations = new List<Guid>();
        }

        /// <summary>
        ///    Initializes a new <see cref="AnnotatableItemDto" /> class.
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the represented <see cref="Entity" /></param>
        protected AnnotatableItemDto(Guid id) : base(id)
        {
            this.Annotations = new List<Guid>();
        }

        /// <summary>
        ///    Gets or sets the Annotations of the AnnotatableItem
        /// </summary>
        public List<Guid> Annotations { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------