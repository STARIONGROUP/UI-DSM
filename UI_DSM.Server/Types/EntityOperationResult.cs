// --------------------------------------------------------------------------------------------------------
// <copyright file="EntityOperationResult.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Types
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Represents the result of an operation on an <see cref="Entity" />
    /// </summary>
    /// <typeparam name="TEntity">An <see cref="Entity" /></typeparam>
    public class EntityOperationResult<TEntity> where TEntity : Entity
    {
        /// <summary>
        ///     Initializes a new <see cref="EntityOperationResult{TEntity}" />
        /// </summary>
        /// <param name="entityEntry">The <see cref="EntityEntry{TEntity}" /> for the operation</param>
        /// <param name="expectedState">A collection of <see cref="EntityState" /></param>
        public EntityOperationResult(EntityEntry<TEntity> entityEntry, params EntityState[] expectedState)
        {
            if (entityEntry == null)
            {
                this.Succeeded = false;
                return;
            }

            this.Succeeded = expectedState.Any(x => x == entityEntry.State);

            if (this.Succeeded)
            {
                this.Entity = entityEntry.Entity;
            }
        }

        /// <summary>
        ///     Initializes a new <see cref="EntityOperationResult{TEntity}" />
        /// </summary>
        /// <param name="entityEntries">The <see cref="List{EntityEntry{TEntity}}" /> for the operation</param>
        /// <param name="expectedState">A collection of <see cref="EntityState" /></param>
        public EntityOperationResult(List<EntityEntry<TEntity>> entityEntries, params EntityState[] expectedState)
        {
            if (entityEntries == null)
            {
                this.Succeeded = false;
                return;
            }

            this.Succeeded = entityEntries.All(x => expectedState.Any(y => x?.State == y));

            if (this.Succeeded)
            {
                this.Entities = entityEntries.Select(x => x?.Entity).ToList();
            }
            else
            {
                this.Entities = new List<TEntity>();
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityOperationResult{TEntity}" /> class.
        /// </summary>
        protected EntityOperationResult()
        {
        }

        /// <summary>
        ///     The <see cref="Entities" /> for the operation
        /// </summary>
        public List<TEntity> Entities { get; set; }

        /// <summary>
        ///     Value asserting the result of the operation
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        ///     The <see cref="Entity" /> for the operation
        /// </summary>
        public TEntity Entity { get; set; }

        /// <summary>
        ///     A collection of errors if any
        /// </summary>
        public List<string> Errors { get; } = new();

        /// <summary>
        ///     Creates a new <see cref="EntityOperationResult{TEntity}" /> with a failure status and some errors message
        /// </summary>
        public static EntityOperationResult<TEntity> Failed(params string[] errors)
        {
            var result = new EntityOperationResult<TEntity>
            {
                Succeeded = false
            };

            result.Errors.AddRange(errors);
            return result;
        }

        /// <summary>
        ///     Creates a new <see cref="EntityOperationResult{TEntity}" /> with a success status and sets the
        ///     <see cref="Entity" />
        /// </summary>
        public static EntityOperationResult<TEntity> Success(TEntity entity)
        {
            return new EntityOperationResult<TEntity>
            {
                Succeeded = true,
                Entity = entity
            };
        }

        /// <summary>
        ///     Creates a new <see cref="EntityOperationResult{TEntity}" /> with a success status and sets the
        ///     <see cref="Entities" />
        /// </summary>
        public static EntityOperationResult<TEntity> AllSuccess(List<TEntity> entities)
        {
            return new EntityOperationResult<TEntity>
            {
                Succeeded = true,
                Entities = entities
            };
        }

        /// <summary>
        ///     Adds the <see cref="Exception.Message" /> to the errors list and unset <see cref="Succeeded" /> value
        /// </summary>
        /// <param name="exception">The <see cref="Exception" /></param>
        public void HandleExpection(Exception exception)
        {
            this.Errors.Add(exception.Message);
            this.Succeeded = false;
        }

        /// <summary>
        ///     Adds the <see cref="Exception.Message" /> to the errors list and unset <see cref="Succeeded" /> value
        /// </summary>
        /// <param name="errorMessage">The error message</param>
        public void HandleExpection(string errorMessage)
        {
            this.Errors.Add(errorMessage);
            this.Succeeded = false;
        }
    }
}
