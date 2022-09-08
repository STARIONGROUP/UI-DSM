// --------------------------------------------------------------------------------------------------------
// <copyright file="AnnotableItem.cs" company="RHEA System S.A.">
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

    /// <summary>
    ///     Represents an <see cref="Entity" /> that can be annotated
    /// </summary>
    [Table(nameof(AnnotableItem))]
    public abstract class AnnotableItem : Entity
    {
        /// <summary>
        ///     Initializes a new <see cref="AnnotableItem" />
        /// </summary>
        protected AnnotableItem()
        {
        }

        /// <summary>
        ///     Inilializes a new <see cref="AnnotableItem" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="AnnotableItem" /></param>
        protected AnnotableItem(Guid id) : base(id)
        {
        }

        /// <summary>
        ///     Gets or sets the author of the <see cref="AnnotableItem" />
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DeepLevel(0)]
        public Participant Author { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DateTime" /> for the creation of the <see cref="AnnotableItem" />
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        ///     Resolve the properties of the current <see cref="Entity" /> from its <see cref="EntityDto" /> counter-part
        /// </summary>
        /// <param name="entityDto">The source <see cref="EntityDto" /></param>
        /// <param name="resolvedEntity">A <see cref="Dictionary{TKey,TValue}" /> of all others <see cref="Entity" /></param>
        public override void ResolveProperties(EntityDto entityDto, Dictionary<Guid, Entity> resolvedEntity)
        {
            if (entityDto is not AnnotableItemDto annotableItemDto)
            {
                throw new InvalidOperationException($"The DTO {entityDto.GetType()} does not match with the current AnnotableItem POCO");
            }

            this.Author = this.GetEntity<Participant>(annotableItemDto.Author, resolvedEntity);
            this.CreatedOn = annotableItemDto.CreatedOn;
        }
    }
}
