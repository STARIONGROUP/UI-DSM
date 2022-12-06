// --------------------------------------------------------------------------------------------------------
// <copyright file="Reply.cs" company="RHEA System S.A.">
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

    /// <summary>
    ///     A <see cref="Reply" /> to a <see cref="Comment" />
    /// </summary>
    [Table(nameof(Reply))]
    public class Reply : Entity
    {
        /// <summary>
        ///     Initializes a new <see cref="Reply" />
        /// </summary>
        public Reply()
        {
        }

        /// <summary>
        ///     Inilializes a new <see cref="Reply" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="Entity" /></param>
        public Reply(Guid id) : base(id)
        {
        }

        /// <summary>
        ///     Gets or sets the author of this <see cref="Reply" />
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DeepLevel(0)]
        public Participant Author { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DateTime" /> for the creation of this <see cref="Reply" />
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        ///     Gets or sets the content of the <see cref="Reply" />
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        ///     Instantiate a <see cref="EntityDto" /> from a <see cref="Entity" />
        /// </summary>
        /// <returns>A new <see cref="EntityDto" /></returns>
        public override EntityDto ToDto()
        {
            return new ReplyDto(this.Id)
            {
                Author = this.Author?.Id ?? Guid.Empty,
                Content = this.Content,
                CreatedOn = this.CreatedOn
            };
        }

        /// <summary>
        ///     Resolve the properties of the current <see cref="Entity" /> from its <see cref="EntityDto" /> counter-part
        /// </summary>
        /// <param name="entityDto">The source <see cref="EntityDto" /></param>
        /// <param name="resolvedEntity">A <see cref="Dictionary{TKey,TValue}" /> of all others <see cref="Entity" /></param>
        public override void ResolveProperties(EntityDto entityDto, Dictionary<Guid, Entity> resolvedEntity)
        {
            if (entityDto is not ReplyDto replyDto)
            {
                throw new InvalidOperationException($"The DTO {entityDto.GetType()} does not match with the current Reply POCO");
            }

            this.Author = this.GetEntity<Participant>(replyDto.Author, resolvedEntity);
            this.Content = replyDto.Content;
            this.CreatedOn = replyDto.CreatedOn;
        }
    }
}
