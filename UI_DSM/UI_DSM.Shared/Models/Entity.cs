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

    using UI_DSM.Shared.Annotations;
    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     Top level abstract superclass from which all domain concept classes in the model inherit
    /// </summary>
    public abstract class Entity
    {
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
            entities.AddRange(this.GetAssociatedProperties(deepLevel));

            return entities;
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
        ///     Get a collection of all <see cref="Entity" /> that are associated to this <see cref="Entity" /> in the range of the
        ///     <see cref="deepLevel" />.
        /// </summary>
        /// <param name="deepLevel">The deep level</param>
        /// <returns>The collection of <see cref="Entity" /></returns>
        protected IEnumerable<Entity> GetAssociatedProperties(int deepLevel)
        {
            if (deepLevel < 0)
            {
                return Enumerable.Empty<Entity>();
            }

            var entities = new List<Entity>();

            var properties = this.GetType().GetProperties()
                .Where(prop => prop.IsDefined(typeof(DeepLevelAttribute), false))
                .Where(x => x.GetCustomAttributes(typeof(DeepLevelAttribute), false).Cast<DeepLevelAttribute>().First().Level <= deepLevel);

            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.GetValue(this) is Entity entity)
                {
                    entities.Add(entity);
                    entities.AddRange(entity.GetAssociatedProperties(deepLevel - 1));
                }
                else if (propertyInfo.GetValue(this) is IEnumerable<Entity> entityCollection)
                {
                    var entityList = entityCollection.ToList();
                    entities.AddRange(entityList);

                    foreach (var containedEntity in entityList)
                    {
                        entities.AddRange(containedEntity.GetAssociatedProperties(deepLevel - 1));
                    }
                }
            }

            return entities;
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
    }
}
