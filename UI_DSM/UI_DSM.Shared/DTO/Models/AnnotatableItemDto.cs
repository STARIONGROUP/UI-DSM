// --------------------------------------------------------------------------------------------------------
// <copyright file="AnnotatableItemDto.cs" company="RHEA System S.A.">
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
    ///     The Data Transfer Object representing the <see cref="AnnotatableItem" /> class.
    /// </summary>
    public abstract class AnnotatableItemDto : EntityDto
    {
        /// <summary>
        ///     Initiazes a new <see cref="AnnotatableItemDto" />
        /// </summary>
        protected AnnotatableItemDto()
        {
            this.InitializeCollections();
        }

        /// <summary>
        ///     Initiazes a new <see cref="AnnotatableItemDto" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the represented <see cref="AnnotatableItemDto" /></param>
        protected AnnotatableItemDto(Guid id) : base(id)
        {
            this.InitializeCollections();
        }

        /// <summary>
        ///     Gets or sets the <see cref="Guid" /> of the represented Author
        /// </summary>
        public Guid Author { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DateTime" /> for the creation of the <see cref="AnnotatableItemDto" />
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        ///     A collection of <see cref="Guid" /> that represents <see cref="Annotation" />
        /// </summary>
        public List<Guid> Annotations { get; set; }

        /// <summary>
        ///     Includes common properties for <see cref="AnnotatableItemDto" /> from an <see cref="AnnotatableItem" />
        /// </summary>
        /// <param name="annotatableItem">The <see cref="AnnotatableItem" /></param>
        public void IncludeCommonProperties(AnnotatableItem annotatableItem)
        {
            this.Author = annotatableItem.Author?.Id ?? Guid.Empty;
            this.CreatedOn = annotatableItem.CreatedOn;
            this.Annotations = annotatableItem.Annotations.Select(x => x.Id).ToList();
        }

        /// <summary>
        ///     Initializes all collection of this <see cref="Entity" />
        /// </summary>
        private void InitializeCollections()
        {
            this.Annotations = new List<Guid>();
        }
    }
}
