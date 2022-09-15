// --------------------------------------------------------------------------------------------------------
// <copyright file="AnnotatableItem.cs" company="RHEA System S.A.">
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

    using UI_DSM.Shared.Annotations;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Extensions;

    /// <summary>
    ///     Represents an <see cref="Entity" /> that can be annotated
    /// </summary>
    [Table(nameof(AnnotatableItem))]
    public abstract class AnnotatableItem : Entity
    {
        /// <summary>
        ///     Initializes a new <see cref="AnnotatableItem" />
        /// </summary>
        protected AnnotatableItem()
        {
            this.InitializeCollections();
        }

        /// <summary>
        ///     Inilializes a new <see cref="AnnotatableItem" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="AnnotatableItem" /></param>
        protected AnnotatableItem(Guid id) : base(id)
        {
            this.InitializeCollections();
        }

        /// <summary>
        ///     Gets or sets the author of the <see cref="AnnotatableItem" />
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DeepLevel(0)]
        public Participant Author { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DateTime" /> for the creation of the <see cref="AnnotatableItem" />
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        ///     A collection of <see cref="Annotation" />
        /// </summary>
        [DeepLevel(0)]
        public List<Annotation> Annotations { get; protected set; }

        /// <summary>
        ///     Resolve the properties of the current <see cref="Entity" /> from its <see cref="EntityDto" /> counter-part
        /// </summary>
        /// <param name="entityDto">The source <see cref="EntityDto" /></param>
        /// <param name="resolvedEntity">A <see cref="Dictionary{TKey,TValue}" /> of all others <see cref="Entity" /></param>
        public override void ResolveProperties(EntityDto entityDto, Dictionary<Guid, Entity> resolvedEntity)
        {
            if (entityDto is not AnnotatableItemDto annotatableItemDto)
            {
                throw new InvalidOperationException($"The DTO {entityDto.GetType()} does not match with the current AnnotatableItem POCO");
            }

            this.Author = this.GetEntity<Participant>(annotatableItemDto.Author, resolvedEntity);
            this.CreatedOn = annotatableItemDto.CreatedOn;
            this.Annotations.ResolveList(annotatableItemDto.Annotations, resolvedEntity);
        }

        /// <summary>
        ///     Initializes all collections for this <see cref="EntityDto" />
        /// </summary>
        private void InitializeCollections()
        {
            this.Annotations = new List<Annotation>();
        }
    }
}
