// --------------------------------------------------------------------------------------------------------
// <copyright file="Review.cs" company="RHEA System S.A.">
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
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     A <see cref="Review" /> represents a review for a model into a <see cref="Project" />
    /// </summary>
    [Table(nameof(Review))]
    public class Review : Entity
    {
        /// <summary>
        ///     Initializes a new <see cref="Review" />
        /// </summary>
        public Review()
        {
            this.InitializeCollections();
        }

        /// <summary>
        ///     Inilializes a new <see cref="Review" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="Review" /></param>
        public Review(Guid id) : base(id)
        {
            this.InitializeCollections();
        }

        /// <summary>
        ///     The <see cref="DateTime" /> when the <see cref="Review" /> has been created
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        ///     The number of the <see cref="Review" /> inside the <see cref="Project" />
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewNumber { get; set; }

        /// <summary>
        ///     The name of the <see cref="Review" />
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        ///     The description of the <see cref="Review" />
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="StatusKind" /> of the <see cref="Review" />
        /// </summary>
        public StatusKind Status { get; set; }

        /// <summary>
        ///     A collection of contained <see cref="ReviewObjective" />
        /// </summary>
        [DeepLevel(1)]
        public EntityContainerList<ReviewObjective> ReviewObjectives { get; protected set; }

        /// <summary>
        ///     A collection of contained <see cref="ReviewItem" />
        /// </summary>
        [DeepLevel(1)]
        public EntityContainerList<ReviewItem> ReviewItems { get; protected set; }

        /// <summary>
        ///     A collection of <see cref="Artifacts" />
        /// </summary>
        [DeepLevel(0)]
        public List<Artifact> Artifacts { get; protected set; }

        /// <summary>
        ///     Instantiate a <see cref="EntityDto" /> from a <see cref="Entity" />
        /// </summary>
        /// <returns>A new <see cref="EntityDto" /></returns>
        public override EntityDto ToDto()
        {
            return new ReviewDto(this.Id)
            {
                CreatedOn = this.CreatedOn,
                ReviewNumber = this.ReviewNumber,
                Title = this.Title,
                Description = this.Description,
                Status = this.Status,
                ReviewObjectives = this.ReviewObjectives.Select(x => x.Id).ToList(),
                Artifacts = this.Artifacts.Select(x => x.Id).ToList(),
                ReviewItems = this.ReviewItems.Select(x => x.Id).ToList()
            };
        }

        /// <summary>
        ///     Resolve the properties of the current <see cref="Entity" /> from its <see cref="EntityDto" /> counter-part
        /// </summary>
        /// <param name="entityDto">The source <see cref="EntityDto" /></param>
        /// <param name="resolvedEntity">A <see cref="Dictionary{TKey,TValue}" /> of all others <see cref="Entity" /></param>
        public override void ResolveProperties(EntityDto entityDto, Dictionary<Guid, Entity> resolvedEntity)
        {
            if (entityDto is not ReviewDto reviewDto)
            {
                throw new InvalidOperationException($"The DTO {entityDto.GetType()} does not match with the current Review POCO");
            }

            this.CreatedOn = reviewDto.CreatedOn;
            this.ReviewNumber = reviewDto.ReviewNumber;
            this.Title = reviewDto.Title;
            this.Description = reviewDto.Description;
            this.Status = reviewDto.Status;
            this.ReviewObjectives.ResolveList(reviewDto.ReviewObjectives, resolvedEntity);
            this.Artifacts.ResolveList(reviewDto.Artifacts, resolvedEntity);
            this.ReviewItems.ResolveList(reviewDto.ReviewItems, resolvedEntity);
        }

        /// <summary>
        ///     Initializes all collections for this <see cref="Entity" />
        /// </summary>
        private void InitializeCollections()
        {
            this.ReviewObjectives = new EntityContainerList<ReviewObjective>(this);
            this.Artifacts = new List<Artifact>();
            this.ReviewItems = new EntityContainerList<ReviewItem>(this);
        }
    }
}
