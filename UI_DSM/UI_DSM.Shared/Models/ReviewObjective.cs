// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjective.cs" company="RHEA System S.A.">
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
    ///     A <see cref="ReviewObjective" /> represents a review object for a <see cref="Review" />
    /// </summary>
    [Table(nameof(ReviewObjective))]
    public class ReviewObjective : AnnotatableItem
    {
        /// <summary>
        ///     Initializes a new <see cref="ReviewObjective" />
        /// </summary>
        public ReviewObjective()
        {
            this.InitializeCollections();
        }

        /// <summary>
        ///     Inilializes a new <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="Entity" /></param>
        public ReviewObjective(Guid id) : base(id)
        {
            this.InitializeCollections();
        }

        /// <summary>
        ///     Gets or sets the <see cref="ReviewObjective" /> title
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="ReviewObjective" /> description
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        ///     The number of the <see cref="ReviewObjective" /> inside the <see cref="Review" />
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewObjectiveNumber { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="StatusKind" /> of the <see cref="ReviewObjective" />
        /// </summary>
        public StatusKind Status { get; set; }

        /// <summary>
        ///     A collection of <see cref="ReviewTasks" />
        /// </summary>
        [DeepLevel(1)]
        public EntityContainerList<ReviewTask> ReviewTasks { get; protected set; }

        /// <summary>
        ///     Instantiate a <see cref="EntityDto" /> from a <see cref="Entity" />
        /// </summary>
        /// <returns>A new <see cref="EntityDto" /></returns>
        public override EntityDto ToDto()
        {
            var dto = new ReviewObjectiveDto(this.Id)
            {
                Title = this.Title,
                Description = this.Description,
                ReviewObjectiveNumber = this.ReviewObjectiveNumber,
                Status = this.Status,
                ReviewTasks = this.ReviewTasks.Select(x => x.Id).ToList()
            };

            dto.IncludeCommonProperties(this);
            return dto;
        }

        /// <summary>
        ///     Resolve the properties of the current <see cref="Entity" /> from its <see cref="EntityDto" /> counter-part
        /// </summary>
        /// <param name="entityDto">The source <see cref="EntityDto" /></param>
        /// <param name="resolvedEntity">A <see cref="Dictionary{TKey,TValue}" /> of all others <see cref="Entity" /></param>
        public override void ResolveProperties(EntityDto entityDto, Dictionary<Guid, Entity> resolvedEntity)
        {
            base.ResolveProperties(entityDto, resolvedEntity);

            if (entityDto is not ReviewObjectiveDto reviewObjectiveDto)
            {
                throw new InvalidOperationException($"The DTO {entityDto.GetType()} does not match with the current ReviewObjective POCO");
            }

            this.Description = reviewObjectiveDto.Description;
            this.Status = reviewObjectiveDto.Status;
            this.Title = reviewObjectiveDto.Title;
            this.ReviewObjectiveNumber = reviewObjectiveDto.ReviewObjectiveNumber;
            this.ReviewTasks.ResolveList(reviewObjectiveDto.ReviewTasks, resolvedEntity);
        }

        /// <summary>
        ///     Initializes all collections of this <see cref="ReviewObjective" />
        /// </summary>
        private void InitializeCollections()
        {
            this.ReviewTasks = new EntityContainerList<ReviewTask>(this);
        }
    }
}
