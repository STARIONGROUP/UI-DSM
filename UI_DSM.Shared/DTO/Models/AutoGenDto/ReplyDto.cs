// --------------------------------------------------------------------------------------------------------
// <copyright file="Reply.cs" company="RHEA System S.A.">
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
    ///    The Data Transfer Object representing the <see cref="Reply" /> class.
    /// </summary>
    [SearchDto(nameof(ReplyDto))]
    public partial class ReplyDto : EntityDto
    {
        /// <summary>
        ///    Initializes a new <see cref="ReplyDto" /> class.
        /// </summary>
        public ReplyDto()
        {
        }

        /// <summary>
        ///    Initializes a new <see cref="ReplyDto" /> class.
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the represented <see cref="Entity" /></param>
        public ReplyDto(Guid id) : base(id)
        {
        }

        /// <summary>
        ///    Gets or sets the Author of the Reply
        /// </summary>
        public Guid Author { get; set; }

        /// <summary>
        ///    Gets or sets the CreatedOn of the Reply
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        ///    Gets or sets the Content of the Reply
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///    Instantiate a <see cref="Entity" /> from a <see cref="EntityDto" />
        /// </summary>
        public override Entity InstantiatePoco()
        {
            return new Reply(this.Id);
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------