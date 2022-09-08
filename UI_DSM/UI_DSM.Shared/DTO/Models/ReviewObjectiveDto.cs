// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveDto.cs" company="RHEA System S.A.">
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
    ///     The Data Transfer Object representing the <see cref="ReviewObjective" /> class.
    /// </summary>
    public class ReviewObjectiveDto : AnnotableItemDto
    {
        /// <summary>
        ///     Initiazes a new <see cref="EntityDto" />
        /// </summary>
        public ReviewObjectiveDto()
        {
        }

        /// <summary>
        ///     Initiazes a new <see cref="EntityDto" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the represented <see cref="Entity" /></param>
        public ReviewObjectiveDto(Guid id) : base(id)
        {
        }

        /// <summary>
        ///     Gets or sets the title of the <see cref="ReviewObjectiveDto" />
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Gets or sets the description of the <see cref="ReviewObjectiveDto" />
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     The number of the <see cref="ReviewObjectiveDto" /> inside the <see cref="Review" />
        /// </summary>
        public int ReviewObjectiveNumber { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="StatusKind" /> of the <see cref="ReviewObjectiveDto" />
        /// </summary>
        public StatusKind Status { get; set; }

        /// <summary>
        ///     Instantiate a <see cref="Entity" /> from a <see cref="EntityDto" />
        /// </summary>
        /// <returns>A new <see cref="Entity" /></returns>
        public override Entity InstantiatePoco()
        {
            return new ReviewObjective(this.Id);
        }
    }
}
