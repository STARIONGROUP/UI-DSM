// --------------------------------------------------------------------------------------------------------
// <copyright file="Comment.cs" company="RHEA System S.A.">
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

    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///    The Data Transfer Object representing the <see cref="Comment" /> class.
    /// </summary>
    [SearchDto(nameof(CommentDto))]
    public partial class CommentDto : AnnotationDto
    {
        /// <summary>
        ///    Initializes a new <see cref="CommentDto" /> class.
        /// </summary>
        public CommentDto()
        {
            this.Replies = new List<Guid>();
        }

        /// <summary>
        ///    Initializes a new <see cref="CommentDto" /> class.
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the represented <see cref="Entity" /></param>
        public CommentDto(Guid id) : base(id)
        {
            this.Replies = new List<Guid>();
        }

        /// <summary>
        ///    Gets or sets the Replies of the Comment
        /// </summary>
        public List<Guid> Replies { get; set; }

        /// <summary>
        ///    Gets or sets the CreatedInside of the Comment
        /// </summary>
        public Guid CreatedInside { get; set; }

        /// <summary>
        ///    Gets or sets the View of the Comment
        /// </summary>
        public View View { get; set; }

        /// <summary>
        ///    Gets or sets the Status of the Comment
        /// </summary>
        public StatusKind Status { get; set; }

        /// <summary>
        ///    Instantiate a <see cref="Entity" /> from a <see cref="EntityDto" />
        /// </summary>
        public override Entity InstantiatePoco()
        {
            return new Comment(this.Id);
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------