// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewTaskDto.cs" company="RHEA System S.A.">
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
    ///     The Data Transfer Object representing the <see cref="ReviewTask" /> class.
    /// </summary>
    public class ReviewTaskDto : EntityDto
    {
        /// <summary>
        ///     Initiazes a new <see cref="ReviewTaskDto" />
        /// </summary>
        public ReviewTaskDto()
        {
        }

        /// <summary>
        ///     Initiazes a new <see cref="ReviewTaskDto" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the represented <see cref="ReviewTaskDto" /></param>
        public ReviewTaskDto(Guid id) : base(id)
        {
        }

        /// <summary>
        ///     The title of the <see cref="ReviewTaskDto" />
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     The description of the <see cref="ReviewTaskDto" />
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     The number of the <see cref="ReviewTaskDto" /> inside the <see cref="ReviewObjective" />
        /// </summary>
        public int TaskNumber { get; set; }

        /// <summary>
        ///     The current <see cref="StatusKind" />
        /// </summary>
        public StatusKind Status { get; set; }

        /// <summary>
        ///     The <see cref="DateTime" /> where the <see cref="ReviewTaskDto" /> has been created
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        ///     The author of this <see cref="ReviewTaskDto" />
        /// </summary>
        public Guid Author { get; set; }

        /// <summary>
        ///     The <see cref="Participant" /> that have to complete this <see cref="ReviewTaskDto" />
        /// </summary>
        public Guid IsAssignedTo { get; set; }

        /// <summary>
        ///     Instantiate a <see cref="Entity" /> from a <see cref="EntityDto" />
        /// </summary>
        /// <returns>A new <see cref="Entity" /></returns>
        public override Entity InstantiatePoco()
        {
            return new ReviewTask(this.Id);
        }
    }
}
