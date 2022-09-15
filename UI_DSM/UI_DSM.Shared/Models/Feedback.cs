// --------------------------------------------------------------------------------------------------------
// <copyright file="Feedback.cs" company="RHEA System S.A.">
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

    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     A <see cref="Feedback" /> <see cref="Annotation" />
    /// </summary>
    [Table(nameof(Feedback))]
    public class Feedback : Annotation
    {
        /// <summary>
        ///     Initializes a new <see cref="Feedback" />
        /// </summary>
        public Feedback()
        {
        }

        /// <summary>
        ///     Inilializes a new <see cref="Feedback" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="Feedback" /></param>
        public Feedback(Guid id) : base(id)
        {
        }

        /// <summary>
        ///     Instantiate a <see cref="EntityDto" /> from a <see cref="Entity" />
        /// </summary>
        /// <returns>A new <see cref="EntityDto" /></returns>
        public override EntityDto ToDto()
        {
            var dto = new FeedbackDto(this.Id);
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

            if (entityDto is not FeedbackDto)
            {
                throw new InvalidOperationException($"The DTO {entityDto.GetType()} does not match with the current Feedback POCO");
            }
        }
    }
}
