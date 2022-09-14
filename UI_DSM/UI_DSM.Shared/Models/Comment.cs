// --------------------------------------------------------------------------------------------------------
// <copyright file="Comment.cs" company="RHEA System S.A.">
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
    using System.ComponentModel.DataAnnotations.Schema;

    using UI_DSM.Shared.Annotations;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     A <see cref="Comment" /> <see cref="Annotation" />
    /// </summary>
    [Table(nameof(Comment))]
    public class Comment : Annotation
    {
        /// <summary>
        ///     Initializes a new <see cref="Comment" />
        /// </summary>
        public Comment()
        {
            this.InitializeCollections();
        }

        /// <summary>
        ///     Inilializes a new <see cref="Comment" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="Comment" /></param>
        public Comment(Guid id) : base(id)
        {
            this.InitializeCollections();
        }

        /// <summary>
        ///     A collection of <see cref="Reply" /> of the current <see cref="Comment" />
        /// </summary>
        [DeepLevel(1)]
        public EntityContainerList<Reply> Replies { get; protected set; }

        /// <summary>
        ///     Instantiate a <see cref="EntityDto" /> from a <see cref="Entity" />
        /// </summary>
        /// <returns>A new <see cref="EntityDto" /></returns>
        public override EntityDto ToDto()
        {
            var dto = new CommentDto(this.Id)
            {
                Replies = this.Replies.Select(x => x.Id).ToList()
            };

            dto.IncludeCommonProperties(this);
            return dto;
        }

        /// <summary>
        ///     Resolve the properties of the current <see cref="Entity" /> from its <see cref="EntityDto" /> counter-part
        /// </summary>
        /// <param name="entityDto">The source <see cref="EntityDto" /></param>
        /// <param name="resolvedEntity">A <see cref="Dictionary{TKey,TValue}" /> of all others <see cref="Entity" /></param>
        public override void ResolveProperties(EntityDto entityDto, Dictionary<Guid, Entity> resolvedEntity)
        {
            base.ResolveProperties(entityDto, resolvedEntity);

            if (entityDto is not CommentDto commentDto)
            {
                throw new InvalidOperationException($"The DTO {entityDto.GetType()} does not match with the current Comment POCO");
            }

            this.Replies.ResolveList(commentDto.Replies, resolvedEntity);
        }

        /// <summary>
        ///     Initializes all collections for this <see cref="EntityDto" />
        /// </summary>
        private void InitializeCollections()
        {
            this.Replies = new EntityContainerList<Reply>(this);
        }
    }
}
