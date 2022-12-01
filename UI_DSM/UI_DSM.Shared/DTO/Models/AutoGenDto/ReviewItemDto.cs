// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewItem.cs" company="RHEA System S.A.">
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
    ///    The Data Transfer Object representing the <see cref="ReviewItem" /> class.
    /// </summary>
    public partial class ReviewItemDto : AnnotatableItemDto
    {
        /// <summary>
        ///    Initializes a new <see cref="ReviewItemDto" /> class.
        /// </summary>
        public ReviewItemDto()
        {
            this.ReviewCategories = new List<Guid>();
        }

        /// <summary>
        ///    Initializes a new <see cref="ReviewItemDto" /> class.
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the represented <see cref="Entity" /></param>
        public ReviewItemDto(Guid id) : base(id)
        {
            this.ReviewCategories = new List<Guid>();
        }

        /// <summary>
        ///    Gets or sets the IsReviewed of the ReviewItem
        /// </summary>
        public bool IsReviewed { get; set; }

        /// <summary>
        ///    Gets or sets the ReviewCategories of the ReviewItem
        /// </summary>
        public List<Guid> ReviewCategories { get; set; }

        /// <summary>
        ///    Gets or sets the ThingId of the ReviewItem
        /// </summary>
        public Guid ThingId { get; set; }

        /// <summary>
        ///    Instantiate a <see cref="Entity" /> from a <see cref="EntityDto" />
        /// </summary>
        public override Entity InstantiatePoco()
        {
            return new ReviewItem(this.Id);
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------