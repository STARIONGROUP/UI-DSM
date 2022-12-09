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

    using UI_DSM.Shared.Annotations;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;

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
            this.InitializeCollections();
        }

        /// <summary>
        ///     Inilializes a new <see cref="Participant" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="Participant" /></param>
        public Participant(Guid id) : base(id)
        {
            this.InitializeCollections();
        }

        /// <summary>
        ///     The represented <see cref="User" />
        /// </summary>
        [Required]
        [DeepLevel(0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public UserEntity User { get; set; }

        /// <summary>
        ///     The <see cref="Participant" /> name
        /// </summary>
        public string ParticipantName => this.User?.UserName;

        /// <summary>
        ///     The current <see cref="Role" /> for the <see cref="User" />
        /// </summary>
        [Required]
        [DeepLevel(0)]
        public Role Role { get; set; }

        /// <summary>
        ///     A collection of <see cref="ReviewTask" /> where this <see cref="Participant" /> is assigned
        /// </summary>
        public List<ReviewTask> AssignedTasks { get; set; }

        /// <summary>
        ///     Instantiate a <see cref="ParticipantDto" /> from a <see cref="Participant" />
        /// </summary>
        /// <returns>A new <see cref="ParticipantDto" /></returns>
        public override EntityDto ToDto()
        {
            var dto = new ParticipantDto(this.Id)
            {
                User = this.User == null ? Guid.Empty : this.User.Id,
                Role = this.Role == null ? Guid.Empty : this.Role.Id,
            };

            return dto;
        }

        /// <summary>
        ///     Resolve the properties of the current <see cref="Participant" /> from its <see cref="EntityDto" /> counter-part
        /// </summary>
        /// <param name="entityDto">The source <see cref="EntityDto" /></param>
        /// <param name="resolvedEntity">A <see cref="Dictionary{TKey,TValue}" /> of all <see cref="Entity" /></param>
        public override void ResolveProperties(EntityDto entityDto, Dictionary<Guid, Entity> resolvedEntity)
        {
            if (entityDto is not ParticipantDto participantDto)
            {
                throw new InvalidOperationException($"The DTO {entityDto.GetType()} does not match with the current Participant POCO");
            }

            this.Role = this.GetEntity<Role>(participantDto.Role, resolvedEntity);
            this.User = this.GetEntity<UserEntity>(participantDto.User, resolvedEntity);
        }

        /// <summary>
        ///     Asserts that the current <see cref="Participant" /> has right to do something
        /// </summary>
        /// <param name="requestedAccessRight">The requested <see cref="AccessRight" /></param>
        /// <returns>The assert</returns>
        public bool IsAllowedTo(AccessRight requestedAccessRight)
        {
            return this.Role != null && this.Role.AccessRights.Any(x => x == requestedAccessRight);
        }

        /// <summary>
        ///     Initializes all collections for this <see cref="Participant" />
        /// </summary>
        private void InitializeCollections()
        {
            this.AssignedTasks = new List<ReviewTask>();
        }
    }
}
