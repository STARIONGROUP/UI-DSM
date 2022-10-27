// --------------------------------------------------------------------------------------------------------
// <copyright file="QueryableExtension.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Server.Extensions
{
    using System.Collections;
    using System.Reflection;

    using Microsoft.EntityFrameworkCore;

    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Extension class for the <see cref="IQueryable{T}" /> interface
    /// </summary>
    public static class QueryableExtension
    {
        /// <summary>
        ///     Builds the <see cref="IQueryable{T}" /> with all included members for the <see cref="TEntity" />
        /// </summary>
        /// <typeparam name="TEntity">A <see cref="Entity" /></typeparam>
        /// <param name="queryable">The <see cref="IQueryable{T}" /></param>
        /// <param name="deepLevel">The deep level</param>
        public static IQueryable<TEntity> BuildIncludeEntityQueryable<TEntity>(this IQueryable<TEntity> queryable, int deepLevel)
            where TEntity : Entity
        {
            var currentType = typeof(TEntity);
            var containedPropertyInfos = new Dictionary<Type, List<PropertyInfo>>();

            return queryable.BuildIncludeEntityQueryable(deepLevel + 1, currentType, containedPropertyInfos, string.Empty);
        }

        /// <summary>
        ///     Builds the <see cref="IQueryable{T}" /> with all included members for the <see cref="TEntity" />
        /// </summary>
        /// <typeparam name="TEntity">A <see cref="Entity" /></typeparam>
        /// <param name="queryable">The <see cref="IQueryable{T}" /></param>
        /// <param name="deepLevel">The deep level</param>
        /// <param name="entityType">The current <see cref="Entity" /> type</param>
        /// <param name="containedType">A <see cref="Dictionary{TKey,TValue}"/>  to avoid adding duplicates</param>
        /// <param name="currentPath">The current path to add to the <see cref="IQueryable{T}" /></param>
        private static IQueryable<TEntity> BuildIncludeEntityQueryable<TEntity>(this IQueryable<TEntity> queryable, int deepLevel,
            Type entityType, Dictionary<Type, List<PropertyInfo>> containedType, string currentPath)
            where TEntity : Entity
        {
            var scopedProperties = Entity.GetScopedProperties(deepLevel, entityType);

            if (!containedType.ContainsKey(entityType))
            {
                containedType.Add(entityType, new List<PropertyInfo>());
            }

            return scopedProperties.Where(scopedProperty => !containedType[entityType].Contains(scopedProperty))
                .Aggregate(queryable, (current, scopedProperty) => ComputeProperty(current, deepLevel, entityType, containedType, currentPath, scopedProperty));
        }

        /// <summary>
        ///     Adds the property to the <see cref="IQueryable{T}" />
        /// </summary>
        /// <typeparam name="TEntity">An <see cref="Entity" /></typeparam>
        /// <param name="queryable">The <see cref="IQueryable{T}" /></param>
        /// <param name="deepLevel">The deep level</param>
        /// <param name="entityType">The current <see cref="Entity" /> type</param>
        /// <param name="containedType">A <see cref="Dictionary{TKey,TValue}"/>  to avoid adding duplicates</param>
        /// <param name="currentPath">The current pass</param>
        /// <param name="scopedProperty">The current <see cref="PropertyInfo" /></param>
        /// <returns>The <see cref="IQueryable{T}" /></returns>
        private static IQueryable<TEntity> ComputeProperty<TEntity>(IQueryable<TEntity> queryable, int deepLevel, Type entityType, IDictionary<Type, List<PropertyInfo>> containedType, string currentPath, PropertyInfo scopedProperty) where TEntity : Entity
        {
            var propertyType = scopedProperty.PropertyType.GetCorrectTypeToCheck();
            var updatedPath = UpdatePath(currentPath, scopedProperty.Name);

            if (propertyType.IsAbstract)
            {
                containedType[entityType].Add(scopedProperty);
                queryable = queryable.Include(updatedPath);

                foreach (var concreteClass in Entity.GetConcreteClasses(propertyType))
                {
                    queryable = queryable.BuildIncludeEntityQueryable(deepLevel == 0 ? deepLevel: deepLevel - 1, concreteClass, new Dictionary<Type, List<PropertyInfo>>(containedType), updatedPath);
                }
            }
            else
            {
                queryable = queryable.Include(updatedPath);

                if (propertyType.IsAssignableTo(typeof(Entity)))
                {
                    containedType[entityType].Add(scopedProperty);

                    queryable = queryable
                        .BuildIncludeEntityQueryable(deepLevel == 0 ? deepLevel : deepLevel - 1, propertyType, new Dictionary<Type, List<PropertyInfo>>(containedType), updatedPath);
                }
            }

            return queryable;
        }

        /// <summary>
        ///     Updates the current path
        /// </summary>
        /// <param name="currentPath">The current path</param>
        /// <param name="propertyName">The name of the property to add</param>
        /// <returns>The updated path</returns>
        private static string UpdatePath(string currentPath, string propertyName)
        {
            return string.IsNullOrEmpty(currentPath) ? propertyName : $"{currentPath}.{propertyName}";
        }

        /// <summary>
        ///     Verify if the current <see cref="Type" /> is an <see cref="Enumerable" />
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to check</param>
        /// <returns>The result</returns>
        private static bool IsEnumerable(this Type type)
        {
            return type.GetInterface(nameof(IEnumerable)) != null && type.GenericTypeArguments.Any();
        }

        /// <summary>
        ///     Gets the <see cref="Type" /> to check. If the <see cref="Type" /> is a <see cref="Enumerable" />, gets the generic type argument
        /// </summary>
        /// <param name="type">The <see cref="Type" /></param>
        /// <returns>The correct <see cref="Type" /></returns>
        private static Type GetCorrectTypeToCheck(this Type type)
        {
            return type.IsEnumerable() ? type.GenericTypeArguments.First() : type;
        }
    }
}
