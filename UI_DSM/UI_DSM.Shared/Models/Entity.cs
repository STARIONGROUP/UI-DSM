// --------------------------------------------------------------------------------------------------------
// <copyright file="Entity.cs" company="RHEA System S.A.">
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
    using System.Reflection;

    using Microsoft.EntityFrameworkCore;

    using UI_DSM.Shared.Annotations;
    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     Top level abstract superclass from which all domain concept classes in the model inherit
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        ///     A <see cref="Dictionary{TKey,TValue}" /> that maps an <see cref="Entity" /> Type with all associated
        ///     <see cref="PropertyInfo" />
        ///     with a deepLevel
        /// </summary>
        private static readonly Dictionary<Type, Dictionary<int, List<PropertyInfo>>> EntityAssociatedProperties = new();

        /// <summary>
        ///     A <see cref="Dictionary{TKey,TValue}" /> that maps an abstract <see cref="Entity" /> with all concretes onces
        /// </summary>
        private static readonly Dictionary<Type, List<Type>> AbstractEntityWithConcreteOnces = new();

        /// <summary>
        ///     Initializes a new <see cref="Entity" />
        /// </summary>
        protected Entity()
        {
        }

        /// <summary>
        ///     Inilializes a new <see cref="Entity" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="Entity" /></param>
        protected Entity(Guid id)
        {
            this.Id = id;
        }

        /// <summary>
        ///     Gets or sets the Universally Unique Identifier (UUID) that uniquely identifies an instance of <see cref="Entity" />
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="Entity" /> container of the current <see cref="Entity" />
        /// </summary>
        public Entity EntityContainer { get; set; }

        /// <summary>
        ///     Register an <see cref="Entity" /> with all associated <see cref="PropertyInfo" /> that has a
        ///     <see cref="DeepLevelAttribute" />
        /// </summary>
        /// <param name="type">The <see cref="Type" /> of the <see cref="Entity" /></param>
        public static void RegisterEntityProperties(Type type)
        {
            if (!type.IsSubclassOf(typeof(Entity)))
            {
                return;
            }

            var deepProperties = new Dictionary<int, List<PropertyInfo>>();

            var properties = type.GetProperties()
                .Where(prop => prop.IsDefined(typeof(DeepLevelAttribute), false));

            foreach (var propertyInfo in properties)
            {
                var deepLevel = propertyInfo.GetCustomAttributes(typeof(DeepLevelAttribute), false)
                    .Cast<DeepLevelAttribute>().First().Level;

                if (!deepProperties.ContainsKey(deepLevel))
                {
                    deepProperties[deepLevel] = new List<PropertyInfo>();
                }

                deepProperties[deepLevel].Add(propertyInfo);
            }

            EntityAssociatedProperties[type] = deepProperties;
        }

        /// <summary>
        ///     Instantiate a <see cref="EntityDto" /> from a <see cref="Entity" />
        /// </summary>
        /// <returns>A new <see cref="EntityDto" /></returns>
        public abstract EntityDto ToDto();

        /// <summary>
        ///     Resolve the properties of the current <see cref="Entity" /> from its <see cref="EntityDto" /> counter-part
        /// </summary>
        /// <param name="entityDto">The source <see cref="EntityDto" /></param>
        /// <param name="resolvedEntity">A <see cref="Dictionary{TKey,TValue}" /> of all others <see cref="Entity" /></param>
        public abstract void ResolveProperties(EntityDto entityDto, Dictionary<Guid, Entity> resolvedEntity);

        /// <summary>
        ///     Get a collection of all <see cref="Entity" /> that are associated to this <see cref="Entity" /> in the range of the
        ///     <see cref="deepLevel" />. The collection contains the current <see cref="Entity" />
        /// </summary>
        /// <param name="deepLevel">The deep level</param>
        /// <returns>The collection of <see cref="Entity" /></returns>
        public IEnumerable<Entity> GetAssociatedEntities(int deepLevel = 0)
        {
            if (deepLevel < 0)
            {
                return Enumerable.Empty<Entity>();
            }

            var entities = new List<Entity> { this };
            this.GetAssociatedProperties(deepLevel, entities);
            return entities.DistinctBy(x => x.Id).ToList();
        }

        public IQueryable<TEntity> BuildIncludeEntityQueryable<TEntity>(IQueryable<TEntity> queryable, int deepLevel, Type entityType)
            where TEntity : Entity
        {
            var scopedProperties = GetScopedProperties(deepLevel, entityType);

            foreach (var scopedProperty in scopedProperties)
            {
                queryable = queryable.Include(x => scopedProperty);
                queryable = this.BuildIncludeEntityQueryable(queryable, deepLevel == 0 ? 0 : deepLevel - 1, scopedProperty.PropertyType);
            }

            return queryable;
        }

        /// <summary>
        ///     Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified object  is equal to the current object; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Entity userEntity)
            {
                return this.Id == userEntity.Id;
            }

            return false;
        }

        /// <summary>
        ///     Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        /// <summary>
        ///     Gets a collection of <see cref="PropertyInfo" /> that are in the scoped of the <paramref name="deepLevel" />
        /// </summary>
        /// <param name="deepLevel">The deep level</param>
        /// <param name="entityType">The <see cref="Type" /></param>
        /// <returns>A collection of <see cref="PropertyInfo" /></returns>
        public static List<PropertyInfo> GetScopedProperties(int deepLevel, Type entityType)
        {
            var scopedProperties = new List<PropertyInfo>();

            var associatedProperties = new List<Dictionary<int, List<PropertyInfo>>>();

            if (entityType.IsAbstract)
            {
                associatedProperties.AddRange(AbstractEntityWithConcreteOnces[entityType].Select(concreteType => EntityAssociatedProperties[concreteType]));
            }
            else
            {
                associatedProperties.Add(EntityAssociatedProperties[entityType]);
            }

            foreach (var associatedProperty in associatedProperties)
            {
                for (var deepLevelIndex = deepLevel; deepLevelIndex >= 0; deepLevelIndex--)
                {
                    if (associatedProperty.TryGetValue(deepLevelIndex, out var properties))
                    {
                        scopedProperties.AddRange(properties);
                    }
                }
            }

            return scopedProperties;
        }

        /// <summary>
        ///     Register all abstract <see cref="Entity" /> abstract <see cref="Type" />
        /// </summary>
        /// <param name="entityTypes">A collection of all <see cref="Entity" /> <see cref="Type" /></param>
        public static void RegisterAbstractEntity(List<Type> entityTypes)
        {
            var abstractEntities = entityTypes.Where(x => x.IsAbstract).ToList();

            foreach (var abstractEntity in abstractEntities)
            {
                AbstractEntityWithConcreteOnces[abstractEntity] = entityTypes.Where(x => x.IsSubclassOf(abstractEntity)).ToList();
            }
        }

        /// <summary>
        ///     Get all classes that are a subclass of the provided <see cref="Type" />
        /// </summary>
        /// <param name="propertyType">The <see cref="Type" /></param>
        /// <returns>A collection of <see cref="Type" /></returns>
        public static List<Type> GetConcreteClasses(Type propertyType)
        {
            return AbstractEntityWithConcreteOnces.ContainsKey(propertyType) ? AbstractEntityWithConcreteOnces[propertyType] : new List<Type>();
        }

        /// <summary>
        ///     Get a collection of all <see cref="Entity" /> that are associated to this <see cref="Entity" /> in the range of the
        ///     <see cref="deepLevel" />.
        /// </summary>
        /// <param name="deepLevel">The deep level</param>
        /// <param name="relatedEntities">A collection of related <see cref="Entity" /></param>
        protected void GetAssociatedProperties(int deepLevel, List<Entity> relatedEntities)
        {
            if (deepLevel < 0)
            {
                return;
            }

            var scopedProperties = GetScopedProperties(deepLevel, this.GetType());

            foreach (var propertyInfo in scopedProperties)
            {
                if (propertyInfo.GetValue(this) is Entity entity)
                {
                    TryToAddAssociatedEntity(entity, relatedEntities, deepLevel);
                }
                else if (propertyInfo.GetValue(this) is IEnumerable<Entity> entityCollection)
                {
                    var entityList = entityCollection.ToList();

                    foreach (var containedEntity in entityList)
                    {
                        TryToAddAssociatedEntity(containedEntity, relatedEntities, deepLevel);
                    }
                }
            }
        }

        /// <summary>
        ///     Gets a <see cref="TEntity" /> from the <see cref="resolvedEntity" />
        /// </summary>
        /// <typeparam name="TEntity">A <see cref="Entity" /></typeparam>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="TEntity" /> to get</param>
        /// <param name="resolvedEntity">A <see cref="Dictionary{TKey,TValue}" /> of all <see cref="Entity" /></param>
        /// <returns>The retrieved <see cref="TEntity" /></returns>
        protected TEntity GetEntity<TEntity>(Guid entityId, Dictionary<Guid, Entity> resolvedEntity) where TEntity : Entity
        {
            if (resolvedEntity.TryGetValue(entityId, out var entity) && entity is TEntity castedEntity)
            {
                return castedEntity;
            }

            return null;
        }

        /// <summary>
        ///     Tries to add an associated entity to the current collection
        /// </summary>
        /// <param name="entity">The <see cref="Entity" /> to add</param>
        /// <param name="relatedEntities">The collection of related <see cref="Entity" /></param>
        /// <param name="deepLevel">The deep level</param>
        private static void TryToAddAssociatedEntity(Entity entity, List<Entity> relatedEntities, int deepLevel)
        {
            if (relatedEntities.All(x => x.Id != entity.Id))
            {
                relatedEntities.Add(entity);
                entity.GetAssociatedProperties(deepLevel == 0 ? 0 : deepLevel - 1, relatedEntities);
            }
        }
    }
}
