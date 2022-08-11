// --------------------------------------------------------------------------------------------------------
// <copyright file="RoleConfiguration.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Context.Configuration
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    ///     The <see cref="IEntityTypeConfiguration{TEntity}" /> for the <see cref="IdentityRole" /> entity
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        /// <summary>
        ///     The unique ID for the Admin user, to prevent loosing data on migration
        /// </summary>
        public const string AdminRoleId = "AF8956F8-CA85-4DF2-8CB6-C46D0845B987";

        /// <summary>
        ///     Configures the entity of type <see cref="IdentityRole" />.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(new IdentityRole
            {
                Id = AdminRoleId,
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            });
        }
    }
}
