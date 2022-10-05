// --------------------------------------------------------------------------------------------------------
// <copyright file="DbSetMockHelper.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Tests.Helpers
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.EntityFrameworkCore;

    using Moq;

    using UI_DSM.Server.Context;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Helpers class for mocked <see cref="DbSet{TEntity}" />
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class DbSetMockHelper
    {
        /// <summary>
        ///     Creates a mocked <see cref="DbSet{TEntity}" /> based on an empty <see cref="TEntity" /> collection
        /// </summary>
        /// <typeparam name="TEntity">An <see cref="Entity" /></typeparam>
        /// <returns>The created mocked <see cref="DbSet{TEntity}" /></returns>
        public static Mock<DbSet<TEntity>> CreateMock<TEntity>() where TEntity : Entity
        {
            return CreateMock(new List<TEntity>());
        }

        /// <summary>
        ///     Creates a mocked <see cref="DbSet{TEntity}" /> based on an <see cref="TEntity" /> collection
        /// </summary>
        /// <typeparam name="TEntity">An <see cref="Entity" /></typeparam>
        /// <param name="entities">The collection of <see cref="TEntity" /></param>
        /// <returns>The created mocked <see cref="DbSet{TEntity}" /></returns>
        public static Mock<DbSet<TEntity>> CreateMock<TEntity>(List<TEntity> entities) where TEntity : Entity
        {
            var dbSet = new Mock<DbSet<TEntity>>();

            dbSet.UpdateDbSetCollection(entities);
            return dbSet;
        }

        /// <summary>
        ///     Updates the collection the mocked <see cref="DbSet{TEntity}" />
        /// </summary>
        /// <typeparam name="TEntity">An <see cref="Entity" /></typeparam>
        /// <param name="dbSet">The mocked <see cref="DbSet{TEntity}" /></param>
        /// <param name="entities">The new collection</param>
        public static void UpdateDbSetCollection<TEntity>(this Mock<DbSet<TEntity>> dbSet, List<TEntity> entities) where TEntity : Entity
        {
            var data = entities.AsQueryable();

            dbSet.As<IAsyncEnumerable<TEntity>>().Setup(x => x.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumerator<TEntity>(data.GetEnumerator()));

            dbSet.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<TEntity>(data.Provider));
            dbSet.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(data.Expression);
            dbSet.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(data.ElementType);
            dbSet.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            foreach (var entity in entities)
            {
                dbSet.Setup(x => x.Find(entity.Id)).Returns(entity);
                dbSet.Setup(x => x.FindAsync(entity.Id)).ReturnsAsync(entity);
            }
        }

        /// <summary>
        /// Creates and register <see cref="DbSet{TEntity}"/>s for provided types into the <see cref="DatabaseContext"/>
        /// </summary>
        /// <typeparam name="TEntity">An <see cref="Entity"/></typeparam>
        /// <typeparam name="TEntityContainer">An <see cref="Entity"/></typeparam>
        /// <param name="context">The mocked <see cref="DatabaseContext"/></param>
        /// <param name="entityDbSet">The mocked <see cref="DbSet{TEntity}"/> for <see cref="TEntity"/></param>
        /// <param name="entityContainerDbSet">The mocked <see cref="DbSet{TEntity}"/> for <see cref="TEntityContainer"/></param>
        public static void CreateDbSetForContext<TEntity, TEntityContainer>(this Mock<DatabaseContext> context, out Mock<DbSet<TEntity>> entityDbSet,
            out Mock<DbSet<TEntityContainer>> entityContainerDbSet) where TEntity : Entity where TEntityContainer : Entity
        {
            context.CreateDbSetForContext(out entityDbSet);
            entityContainerDbSet = CreateMock<TEntityContainer>();
            context.Setup(x => x.Set<TEntityContainer>()).Returns(entityContainerDbSet.Object);
        }

        /// <summary>
        /// Creates and register <see cref="DbSet{TEntity}"/> for provided type into the <see cref="DatabaseContext"/>
        /// </summary>
        /// <typeparam name="TEntity">An <see cref="Entity"/></typeparam>
        /// <param name="context">The mocked <see cref="DatabaseContext"/></param>
        /// <param name="entityDbSet">The mocked <see cref="DbSet{TEntity}"/> for <see cref="TEntity"/></param>
        public static void CreateDbSetForContext<TEntity>(this Mock<DatabaseContext> context, out Mock<DbSet<TEntity>> entityDbSet) where TEntity : Entity 
        {
            entityDbSet = CreateMock<TEntity>();
            context.Setup(x => x.Set<TEntity>()).Returns(entityDbSet.Object);
        }
    }
}
