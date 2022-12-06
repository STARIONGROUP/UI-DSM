// --------------------------------------------------------------------------------------------------------
// <copyright file="EntitiesRequestResponses.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.Types
{
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     A <see cref="RequestResponseDto" /> with an <see cref="List{TEntity}" /> collection
    /// </summary>
    /// <typeparam name="TEntity">An <see cref="Entity" /></typeparam>
    public class EntitiesRequestResponses<TEntity> : RequestResponseDto where TEntity : Entity
    {
        /// <summary>
        ///     Gets or sets the <see cref="List{TEntity}" />
        /// </summary>
        public List<TEntity> Entities { get; set; }

        /// <summary>
        ///     Creates a new <see cref="EntitiesRequestResponses{TEntity}" /> in case of success
        /// </summary>
        /// <param name="entities">The <see cref="List{TEntity}" /></param>
        /// <returns>A new <see cref="EntitiesRequestResponses{TEntity}" /></returns>
        public static EntitiesRequestResponses<TEntity> Success(List<TEntity> entities)
        {
            return new EntitiesRequestResponses<TEntity>
            {
                Entities = entities,
                IsRequestSuccessful = true
            };
        }

        /// <summary>
        ///     Creates a new <see cref="EntityRequestResponse{TEntity}" /> in case of failure
        /// </summary>
        /// <param name="errors">A collection of error message</param>
        /// <returns>A new <see cref="EntitiesRequestResponses{TEntity}" /></returns>
        public static EntitiesRequestResponses<TEntity> Fail(List<string> errors)
        {
            return new EntitiesRequestResponses<TEntity>
            {
                Errors = errors,
                IsRequestSuccessful = false
            };
        }
    }
}
