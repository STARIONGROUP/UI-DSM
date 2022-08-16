// --------------------------------------------------------------------------------------------------------
// <copyright file="Participant.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
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
    ///     Represents a <see cref="User" /> that is linked to a <see cref="Project" />
    /// </summary>
    [Table(nameof(Participant))]
    public class Participant : Entity
    {
        /// <summary>
        ///     Initializes a new <see cref="Participant" />
        /// </summary>
        public Participant()
        {
        }

        /// <summary>
        ///     Inilializes a new <see cref="Participant" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="Participant" /></param>
        public Participant(Guid id) : base(id)
        {
        }

        /// <summary>
        ///     The represented <see cref="User" />
        /// </summary>
        [Required]
        public User User { get; set; }

        /// <summary>
        ///     Instantiate a <see cref="ParticipantDto" /> from a <see cref="Participant" />
        /// </summary>
        /// <returns>A new <see cref="ParticipantDto" /></returns>
        public override EntityDto ToDto()
        {
            var dto = new ParticipantDto(this.Id)
            {
                User = new Guid(this.User.Id)
            };

            return dto;
        }
    }
}
