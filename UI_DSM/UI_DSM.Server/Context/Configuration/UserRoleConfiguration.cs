// --------------------------------------------------------------------------------------------------------
// <copyright file="UserRoleConfiguration.cs" company="RHEA System S.A.">
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
    /// The <see cref="IEntityTypeConfiguration{TEntity}"/> for the <see cref="IdentityUserRole{TKey}"/> entity
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UserRoleConfiguration: IEntityTypeConfiguration<IdentityUserRole<string>>

    {
        /// <summary>
        ///     Configures the entity of type <see cref="IdentityUserRole{TKey}"/>
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(new IdentityUserRole<string>
            {
                RoleId = RoleConfiguration.AdminRoleId,
                UserId = UserConfiguration.AdminId
            });
        }
    }
}
