// --------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtension.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.Extensions
{
    using CDP4Common.CommonData;

    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     Extension class for <see cref="IEnumerable{Entity}" />
    /// </summary>
    public static class EnumerableExtension
    {
        /// <summary>
        ///     Convert any <see cref="IEnumerable{Entity}" /> to a <see cref="List{EntityDto}" />
        /// </summary>
        /// <param name="collection">A collection of <see cref="Entity" /></param>
        /// <returns>A <see cref="List{EntityDto}" /></returns>
        public static List<EntityDto> ToDtos(this IEnumerable<Entity> collection)
        {
            return collection.Select(x => x.ToDto()).ToList();
        }

        /// <summary>
        ///     Resolve the <see cref="List{TEntity}" /> from the <see cref="IEnumerable{Guid}" />
        /// </summary>
        /// <param name="collection">The collection of <see cref="TEntity" /></param>
        /// <param name="guids">A collection of <see cref="Guid" /> of <see cref="Entity" /> to find</param>
        /// <param name="resolvedEntity">A <see cref="Dictionary{TKey,TValue}" /> of all <see cref="Entity" /></param>
        /// <typeparam name="TEntity">An <see cref="Entity" /></typeparam>
        public static void ResolveList<TEntity>(this List<TEntity> collection, IEnumerable<Guid> guids, Dictionary<Guid, Entity> resolvedEntity) where TEntity : Entity
        {
            collection.Clear();

            foreach (var guid in guids)
            {
                if (resolvedEntity.TryGetValue(guid, out var entity) && entity is TEntity foundEntity)
                {
                    collection.Add(foundEntity);
                }
            }
        }

        /// <summary>
        ///     Resolve the <see cref="List{TEntity}" /> from the <see cref="IEnumerable{Guid}" />
        /// </summary>
        /// <param name="collection">The <see cref="EntityContainerList{TEntity}"/> of <see cref="TEntity" /></param>
        /// <param name="guids">A collection of <see cref="Guid" /> of <see cref="Entity" /> to find</param>
        /// <param name="resolvedEntity">A <see cref="Dictionary{TKey,TValue}" /> of all <see cref="Entity" /></param>
        /// <typeparam name="TEntity">An <see cref="Entity" /></typeparam>
        public static void ResolveList<TEntity>(this EntityContainerList<TEntity> collection, IEnumerable<Guid> guids, Dictionary<Guid, Entity> resolvedEntity) where TEntity : Entity
        {
            collection.Clear();

            foreach (var guid in guids)
            {
                if (resolvedEntity.TryGetValue(guid, out var entity) && entity is TEntity foundEntity)
                {
                    collection.Add(foundEntity);
                }
            }
        }

        /// <summary>
        ///     Distincts a collection of <see cref="Thing" /> by its <see cref="Thing.Iid" />
        /// </summary>
        /// <param name="things">The collection of <see cref="Thing" /></param>
        /// <returns>The distincted collection</returns>
        public static List<Thing> DistinctById(this IEnumerable<Thing> things)
        {
            return things.DistinctBy(x => x.Iid).ToList();
        }
    }
}
