// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewCategory.cs" company="RHEA System S.A.">
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
    ///    The Data Transfer Object representing the <see cref="ReviewCategory" /> class.
    /// </summary>
    public partial class ReviewCategoryDto : EntityDto
    {
        /// <summary>
        ///    Initializes a new <see cref="ReviewCategoryDto" /> class.
        /// </summary>
        public ReviewCategoryDto()
        {
        }

        /// <summary>
        ///    Initializes a new <see cref="ReviewCategoryDto" /> class.
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the represented <see cref="Entity" /></param>
        public ReviewCategoryDto(Guid id) : base(id)
        {
        }

        /// <summary>
        ///    Gets or sets the ReviewCategoryName of the ReviewCategory
        /// </summary>
        public string ReviewCategoryName { get; set; }

        /// <summary>
        ///    Gets or sets the Description of the ReviewCategory
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///    Gets or sets the TagColor of the ReviewCategory
        /// </summary>
        public string TagColor { get; set; }

        /// <summary>
        ///    Instantiate a <see cref="Entity" /> from a <see cref="EntityDto" />
        /// </summary>
        public override Entity InstantiatePoco()
        {
            return new ReviewCategory(this.Id);
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------