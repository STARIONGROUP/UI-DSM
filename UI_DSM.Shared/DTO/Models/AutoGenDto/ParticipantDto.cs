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

// --------------------------------------------------------------------------------------------------------
// ------------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!------------
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Shared.DTO.Models
{
    using GP.SearchService.SDK.Definitions;

    using UI_DSM.Shared.Models;

    /// <summary>
    ///    The Data Transfer Object representing the <see cref="Participant" /> class.
    /// </summary>
    [SearchDto(nameof(ParticipantDto))]
    public partial class ParticipantDto : EntityDto
    {
        /// <summary>
        ///    Initializes a new <see cref="ParticipantDto" /> class.
        /// </summary>
        public ParticipantDto()
        {
            this.DomainsOfExpertise = new List<string>();
        }

        /// <summary>
        ///    Initializes a new <see cref="ParticipantDto" /> class.
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the represented <see cref="Entity" /></param>
        public ParticipantDto(Guid id) : base(id)
        {
            this.DomainsOfExpertise = new List<string>();
        }

        /// <summary>
        ///    Gets or sets the User of the Participant
        /// </summary>
        public Guid User { get; set; }

        /// <summary>
        ///    Gets or sets the ParticipantName of the Participant
        /// </summary>
        public string ParticipantName { get; set; }

        /// <summary>
        ///    Gets or sets the Role of the Participant
        /// </summary>
        public Guid Role { get; set; }

        /// <summary>
        ///    Gets or sets the DomainsOfExpertise of the Participant
        /// </summary>
        public List<string> DomainsOfExpertise { get; set; }

        /// <summary>
        ///    Instantiate a <see cref="Entity" /> from a <see cref="EntityDto" />
        /// </summary>
        public override Entity InstantiatePoco()
        {
            return new Participant(this.Id);
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------