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

    using UI_DSM.Shared.Models;

    [ExcludeFromCodeCoverage]
    public static class DbSetMockHelper
    {
        public static Mock<DbSet<TEntity>> CreateMock<TEntity>(List<TEntity> entities) where TEntity : class
        {
            var dbSet = new Mock<DbSet<TEntity>>();

            var data = entities.AsQueryable();

            dbSet.As<IAsyncEnumerable<TEntity>>().Setup(x => x.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumerator<TEntity>(data.GetEnumerator()));

            dbSet.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Project>(data.Provider));
            dbSet.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(data.Expression);
            dbSet.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(data.ElementType);
            dbSet.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return dbSet;
        }
    }
}
