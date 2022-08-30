// --------------------------------------------------------------------------------------------------------
// <copyright file="UserEntityConfiguration.cs" company="RHEA System S.A.">
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

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="IEntityTypeConfiguration{TEntity}" /> for the <see cref="UserEntity" /> entity
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        /// <summary>
        ///     The unique ID for the <see cref="UserEntity" />
        /// </summary>
        public const string EntityId = "3503BF4C-1211-41EB-B369-AAA6BBDF5FF8";

        /// <summary>
        ///     Configures the entity of type <see cref="UserEntity" />
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            var user = new UserEntity(new Guid(EntityId))
            {
                UserId = UserConfiguration.AdminId,
                UserName = "admin",
                IsAdmin = true
            };

            builder.HasData(user);
        }
    }
}
