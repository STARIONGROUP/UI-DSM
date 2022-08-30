// --------------------------------------------------------------------------------------------------------
// <copyright file="UserConfiguration.cs" company="RHEA System S.A.">
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

    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="IEntityTypeConfiguration{TEntity}" /> for the <see cref="User" /> entity
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        /// <summary>
        ///     The unique ID for the Admin user, to prevent loosing data on migration
        /// </summary>
        public const string AdminId = "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7";
        
        /// <summary>
        ///     Configures the entity of type <see cref="User" />
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<User> builder)
        {
            var passwordHasher = new PasswordHasher<User>();

            var adminUser = new User
            {
                Id = AdminId,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                IsAdmin = true
            };

            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "pass");

            builder.HasData(adminUser);
        }
    }
}
