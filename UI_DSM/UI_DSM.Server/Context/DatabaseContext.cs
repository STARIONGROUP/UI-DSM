// --------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseContext.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Context
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    using UI_DSM.Server.Context.Configuration;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="DatabaseContext" /> is the Database Context for the Entity Framework
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DatabaseContext : IdentityDbContext<User>
    {
        /// <summary>
        ///     Initializes a new instance of <see cref="DatabaseContext" />.
        /// </summary>
        /// <param name="options">The options to be used by a <see cref="DbContext" />.</param>
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DatabaseContext" /> class.
        /// </summary>
        protected DatabaseContext()
        {
        }

        /// <summary>
        ///     A <see cref="DbSet{TEntity}" /> of <see cref="Project" />
        /// </summary>
        public virtual DbSet<Project> Projects { get; set; }

        /// <summary>
        ///     A <see cref="DbSet{TEntity}" /> of <see cref="Role" />
        /// </summary>
        public virtual DbSet<Role> Roles { get; set; }

        /// <summary>
        ///     Configures the schema needed for the identity framework.
        /// </summary>
        /// <param name="builder">
        ///     The builder being used to construct the model for this context.
        /// </param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration());
            builder.ApplyConfiguration(new UserRoleConfiguration());
            builder.ApplyConfiguration(new UiDsmRoleConfiguration());
        }
    }
}
