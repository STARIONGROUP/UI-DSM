// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewCategory.cs" company="RHEA System S.A.">
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
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     A <see cref="ReviewCategory" /> represents a container for reviews
    /// </summary>
    [Table(nameof(ReviewCategory))]
    public class ReviewCategory : Entity
    {
        /// <summary>
        ///     Initialize a new <see cref="ReviewCategory" />
        /// </summary>
        public ReviewCategory()
        {
            this.InitializeCollections();
        }

        /// <summary>
        ///     Inilializes a new <see cref="ReviewCategory" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="ReviewCategory" /></param>
        public ReviewCategory(Guid id) : base(id)
        {
            this.InitializeCollections();
        }

        /// <summary>
        ///     Gets or sets the name of the <see cref="ReviewCategory" />
        /// </summary>
        [Required]
        public string ReviewCategoryName { get; set; }

        /// <summary>
        ///     The description of the <see cref="ReviewCategory" />
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        ///     The tag color of the <see cref="ReviewCategory" />
        /// </summary>
        [Required]
        public string TagColor { get; set; }

        /// <summary>
        ///     A collection of <see cref="Project" /> where this <see cref="ReviewCategory" /> is used
        /// </summary>
        public List<Project> Projects { get; set; }

        /// <summary>
        ///     A collection of <see cref="ReviewObjective" /> where this <see cref="ReviewCategory" /> is used
        /// </summary>
        public List<ReviewObjective> ReviewObjectives { get; set; }

        /// <summary>
        ///     A collection of <see cref="ReviewItem" />
        /// </summary>
        public List<ReviewItem> ReviewItems { get; set; }

        /// <summary>
        ///     Instantiate a <see cref="EntityDto" /> from a <see cref="Entity" />
        /// </summary>
        /// <returns>A new <see cref="EntityDto" /></returns>
        public override EntityDto ToDto()
        {
            var dto = new ReviewCategoryDto(this.Id)
            {
                ReviewCategoryName = this.ReviewCategoryName,
                Description = this.Description,
                TagColor = this.TagColor
            };

            return dto;
        }

        /// <summary>
        ///     Resolve the properties of the current <see cref="Project" /> from its <see cref="EntityDto" /> counter-part
        /// </summary>
        /// <param name="entityDto">The source <see cref="EntityDto" /></param>
        /// <param name="resolvedEntity">A <see cref="Dictionary{TKey,TValue}" /> of all <see cref="Entity" /></param>
        public override void ResolveProperties(EntityDto entityDto, Dictionary<Guid, Entity> resolvedEntity)
        {
            if (entityDto is not ReviewCategoryDto reviewCategoryDto)
            {
                throw new InvalidOperationException($"The DTO {entityDto.GetType()} does not match with the current ReviewCategory POCO");
            }

            this.ReviewCategoryName = reviewCategoryDto.ReviewCategoryName;
            this.Description = reviewCategoryDto.Description;
            this.TagColor = reviewCategoryDto.TagColor;
        }

        /// <summary>
        ///     Initializes all collections for this <see cref="ReviewCategory" />
        /// </summary>
        private void InitializeCollections()
        {
            this.Projects = new List<Project>();
            this.ReviewObjectives = new List<ReviewObjective>();
            this.ReviewItems = new List<ReviewItem>();
        }
    }
}
