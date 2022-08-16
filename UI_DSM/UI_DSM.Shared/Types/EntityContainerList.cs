// --------------------------------------------------------------------------------------------------------
// <copyright file="EntityContainerList.cs" company="RHEA System S.A.">
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
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Collection used to automatically link an <see cref="Entity" /> with its <see cref="Entity.EntityContainer" />
    /// </summary>
    /// <typeparam name="TEntity">An <see cref="Entity" /></typeparam>
    public class EntityContainerList<TEntity> : List<TEntity> where TEntity : Entity
    {
        /// <summary>
        ///     Backing field for the container of this <see cref="EntityContainerList{TEntity}" />
        /// </summary>
        private readonly Entity entityContainer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityContainerList{TEntity}" />
        /// </summary>
        /// <param name="entityContainer">The <see cref="Entity" /> container</param>
        public EntityContainerList(Entity entityContainer)
        {
            this.entityContainer = entityContainer;
        }

        /// <summary>
        ///     Gets or sets the value of the <see cref="TEntity" /> associated with the specified index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The <see cref="TEntity" /> with the specified index (only for get).</returns>
        public new TEntity this[int index]
        {
            get => index >= 0 && index < this.Count ? base[index] : throw new ArgumentOutOfRangeException(nameof(index), $"index is {index}, valid range is 0 to {this.Count - 1}");
            set
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"index is {index}, valid range is 0 to {this.Count - 1}");
                }

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (this.Contains(value) && base[index] != value)
                {
                    throw new InvalidOperationException($"The added item already exists {value.Id}.");
                }

                value.EntityContainer = this.entityContainer;
                base[index] = value;
            }
        }

        /// <summary>
        ///     Adds a new <see cref="Entity" /> into the collections if not already presents and sets the
        ///     <see cref="Entity.EntityContainer" /> to the added <see cref="Entity" />
        /// </summary>
        /// <param name="entity">The <see cref="Entity" /> to add</param>
        public new void Add(TEntity entity)
        {
            entity.EntityContainer = this.entityContainer;

            if (this.Any(x => x.Id == entity.Id))
            {
                throw new InvalidOperationException($"An item with the id {entity.Id} is already contained");
            }

            base.Add(entity);
        }

        /// <summary>
        ///     Adds a collection of <see cref="TEntity" /> in the <see cref="List{T}" /> and sets their
        ///     <see cref="Entity.EntityContainer" /> property
        /// </summary>
        /// <param name="entities">The collection of <see cref="TEntity" />s to add</param>
        public new void AddRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                this.Add(entity);
            }
        }
    }
}
