// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewItem.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
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

    using CDP4Common.CommonData;

    using UI_DSM.Shared.Annotations;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Extensions;

    /// <summary>
    ///     A singular review item that is linked to anything the user wishes to mark as reviewed or annotate.
    /// </summary>
    [Table(nameof(ReviewItem))]
    public class ReviewItem : AnnotatableItem
    {
        /// <summary>
        ///     Initializes a new <see cref="ReviewItem" />
        /// </summary>
        public ReviewItem()
        {
            this.InitializeCollections();
        }

        /// <summary>
        ///     Inilializes a new <see cref="ReviewItem" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="ReviewItem" /></param>
        public ReviewItem(Guid id) : base(id)
        {
            this.InitializeCollections();
        }

        /// <summary>
        /// A collection of <see cref="ReviewCategories"/>
        /// </summary>
        [DeepLevel(0)]
        public List<ReviewCategory> ReviewCategories { get; set; }

        /// <summary>
        ///     Resolve the properties of the current <see cref="Entity" /> from its <see cref="EntityDto" /> counter-part
        /// </summary>
        /// <param name="entityDto">The source <see cref="EntityDto" /></param>
        /// <param name="resolvedEntity">A <see cref="Dictionary{TKey,TValue}" /> of all others <see cref="Entity" /></param>
        public override void ResolveProperties(EntityDto entityDto, Dictionary<Guid, Entity> resolvedEntity)
        {
            base.ResolveProperties(entityDto, resolvedEntity);

            if (entityDto is not ReviewItemDto reviewItemDto)
            {
                throw new InvalidOperationException($"The DTO {entityDto.GetType()} does not match with the current ReviewItem POCO");
            }

            this.ThingId = reviewItemDto.ThingId;
            this.ReviewCategories.ResolveList(reviewItemDto.ReviewCategories, resolvedEntity);
        }

        /// <summary>
        ///     The <see cref="Guid" /> of the related <see cref="Thing" />
        /// </summary>
        public Guid ThingId { get; set; }

        /// <summary>
        ///     Instantiate a <see cref="EntityDto" /> from a <see cref="Entity" />
        /// </summary>
        /// <returns>A new <see cref="EntityDto" /></returns>
        public override EntityDto ToDto()
        {
            var dto = new ReviewItemDto(this.Id);
            dto.IncludeCommonProperties(this);
            dto.ReviewCategories = this.ReviewCategories.Select(x => x.Id).ToList();
            dto.ThingId = this.ThingId;
            return dto;
        }

        /// <summary>
        ///     Initializes all collections for this <see cref="EntityDto" />
        /// </summary>
        private void InitializeCollections()
        {
            this.ReviewCategories = new List<ReviewCategory>();
        }
    }
}
