// --------------------------------------------------------------------------------------------------------
// <copyright file="Annotation.cs" company="RHEA System S.A.">
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
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using UI_DSM.Shared.Annotations;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Extensions;

    /// <summary>
    ///     An abstract class to annotate review items
    /// </summary>
    [Table(nameof(Annotation))]
    public abstract class Annotation : Entity
    {
        /// <summary>
        ///     Initializes a new <see cref="Annotation" />
        /// </summary>
        protected Annotation()
        {
            this.AnnotatableItems = new List<AnnotatableItem>();
        }

        /// <summary>
        ///     Inilializes a new <see cref="Annotation" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="Annotation" /></param>
        protected Annotation(Guid id) : base(id)
        {
            this.AnnotatableItems = new List<AnnotatableItem>();
        }

        /// <summary>
        ///     Gets or sets the author of this <see cref="Annotation" />
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DeepLevel(0)]
        public Participant Author { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DateTime" /> for the creation of this <see cref="Annotation" />
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        ///     Gets or sets a collection of the <see cref="AnnotatableItem" />
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DeepLevel(0)]
        public List<AnnotatableItem> AnnotatableItems { get; set; }

        /// <summary>
        ///     Gets or sets the content of the <see cref="Annotation" />
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        ///     Resolve the properties of the current <see cref="Entity" /> from its <see cref="EntityDto" /> counter-part
        /// </summary>
        /// <param name="entityDto">The source <see cref="EntityDto" /></param>
        /// <param name="resolvedEntity">A <see cref="Dictionary{TKey,TValue}" /> of all others <see cref="Entity" /></param>
        public override void ResolveProperties(EntityDto entityDto, Dictionary<Guid, Entity> resolvedEntity)
        {
            if (entityDto is not AnnotationDto annotationDto)
            {
                throw new InvalidOperationException($"The DTO {entityDto.GetType()} does not match with the current Annotation POCO");
            }

            this.Author = this.GetEntity<Participant>(annotationDto.Author, resolvedEntity);
            this.CreatedOn = annotationDto.CreatedOn;
            this.Content = annotationDto.Content;
            this.AnnotatableItems.ResolveList(annotationDto.AnnotatableItems, resolvedEntity);
        }
    }
}
