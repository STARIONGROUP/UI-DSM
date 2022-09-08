// --------------------------------------------------------------------------------------------------------
// <copyright file="AnnotableItemDto.cs" company="RHEA System S.A.">
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
    ///     The Data Transfer Object representing the <see cref="AnnotableItem" /> class.
    /// </summary>
    public abstract class AnnotableItemDto : EntityDto
    {
        /// <summary>
        ///     Initiazes a new <see cref="AnnotableItemDto" />
        /// </summary>
        protected AnnotableItemDto()
        {
        }

        /// <summary>
        ///     Initiazes a new <see cref="AnnotableItemDto" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the represented <see cref="AnnotableItemDto" /></param>
        protected AnnotableItemDto(Guid id) : base(id)
        {
        }

        /// <summary>
        ///     Gets or sets the author of the <see cref="AnnotableItemDto" />
        /// </summary>
        public Guid Author { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DateTime" /> for the creation of the <see cref="AnnotableItemDto" />
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        ///     Includes common properties for <see cref="AnnotableItemDto" /> from an <see cref="AnnotableItem" />
        /// </summary>
        /// <param name="annotableItem">The <see cref="AnnotableItem" /></param>
        public void IncludeCommonProperties(AnnotableItem annotableItem)
        {
            this.Author = annotableItem.Author?.Id ?? Guid.Empty;
            this.CreatedOn = annotableItem.CreatedOn;
        }
    }
}
