// --------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryExtension.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Extensions
{
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Extension methods the <see cref="Dictionary{TKey,TValue}" /> class
    /// </summary>
    public static class DictionaryExtension
    {
        /// <summary>
        ///     Fill the <see cref="Dictionary{Guid,Entity}" /> with the provided collection of <see cref="Entity" />
        /// </summary>
        /// <param name="dictionary">The <see cref="Dictionary{Guid,Entity}" /> to fill</param>
        /// <param name="entities">A collection of <see cref="Entity" /></param>
        public static void InsertEntityCollection<TEntity>(this Dictionary<Guid, Entity> dictionary, IEnumerable<TEntity> entities) where TEntity : Entity
        {
            foreach (var entity in entities)
            {
                dictionary[entity.Id] = entity;
            }
        }

        /// <summary>
        ///     Adds a <see cref="TEntity" /> into the <see cref="Dictionary{Guid,Entity}" />
        /// </summary>
        /// <typeparam name="TEntity">An <see cref="Entity" /></typeparam>
        /// <param name="dictionary">The <see cref="Dictionary{Guid,Entity}" /></param>
        /// <param name="entity">The <see cref="TEntity" /> to add</param>
        public static void InsertEntity<TEntity>(this Dictionary<Guid, Entity> dictionary, TEntity entity) where TEntity : Entity
        {
            if (entity != null)
            {
                dictionary[entity.Id] = entity;
            }
        }
    }
}
