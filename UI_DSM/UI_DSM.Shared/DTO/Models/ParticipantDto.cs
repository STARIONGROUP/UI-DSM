// --------------------------------------------------------------------------------------------------------
// <copyright file="ParticipantDto.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.DTO.Models
{
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The Data Transfer Object representing the <see cref="Participant" /> class.
    /// </summary>
    public class ParticipantDto : EntityDto
    {
        /// <summary>
        ///     Initiazes a new <see cref="ParticipantDto" />
        /// </summary>
        public ParticipantDto()
        {
        }

        /// <summary>
        ///     Initiazes a new <see cref="ParticipantDto" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the represented <see cref="Participant" /></param>
        public ParticipantDto(Guid id) : base(id)
        {
        }

        /// <summary>
        ///     Gets or sets the <see cref="Guid" /> of the represented user
        /// </summary>
        public Guid User { get; set; }

        /// <summary>
        /// Instantiate a <see cref="Participant"/> from a <see cref="ParticipantDto"/>
        /// </summary>
        /// <returns>A new <see cref="Participant"/></returns>
        public override Entity InstantiatePoco()
        {
            return new Participant(this.Id);
        }
    }
}
