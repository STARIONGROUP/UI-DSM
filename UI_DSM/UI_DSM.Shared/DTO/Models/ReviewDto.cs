// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewDto.cs" company="RHEA System S.A.">
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
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The Data Transfer Object representing the <see cref="Review" /> class.
    /// </summary>
    public class ReviewDto : EntityDto
    {
        /// <summary>
        ///     Initiazes a new <see cref="ReviewDto" />
        /// </summary>
        public ReviewDto()
        {
            this.InitializeCollections();
        }

        /// <summary>
        ///     Initiazes a new <see cref="ReviewDto" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the represented <see cref="ReviewDto" /></param>
        public ReviewDto(Guid id) : base(id)
        {
            this.InitializeCollections();
        }

        /// <summary>
        ///     The <see cref="DateTime" /> when the <see cref="Review" /> has been created
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        ///     The author of the <see cref="Review" />
        /// </summary>
        public Guid Author { get; set; }

        /// <summary>
        ///     The number of the <see cref="Review" /> inside the <see cref="Project" />
        /// </summary>
        public int ReviewNumber { get; set; }

        /// <summary>
        ///     The name of the <see cref="Review" />
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     The description of the <see cref="Review" />
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="StatusKind" /> of the <see cref="Review" />
        /// </summary>
        public StatusKind Status { get; set; }

        /// <summary>
        ///     A collection of <see cref="Guid" /> that represents <see cref="ReviewObjective" />
        /// </summary>
        public List<Guid> ReviewObjectives { get; set; }

        /// <summary>
        ///     Instantiate a <see cref="Entity" /> from a <see cref="EntityDto" />
        /// </summary>
        /// <returns>A new <see cref="Entity" /></returns>
        public override Entity InstantiatePoco()
        {
            return new Review(this.Id);
        }

        /// <summary>
        ///     Initializes all collections for this <see cref="EntityDto" />
        /// </summary>
        private void InitializeCollections()
        {
            this.ReviewObjectives = new List<Guid>();
        }
    }
}
