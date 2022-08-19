// --------------------------------------------------------------------------------------------------------
// <copyright file="UiDsmRoleConfiguration.cs" company="RHEA System S.A.">
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

    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="IEntityTypeConfiguration{TEntity}" /> for the <see cref="Role" /> entity
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UiDsmRoleConfiguration : IEntityTypeConfiguration<Role>
    {
        /// <summary>
        ///     The unique ID for the Project administrator role, to prevent loosing data on migration
        /// </summary>
        public const string ProjectAdministratorRoleId = "FD580A55-9666-4ABE-A02B-3A99478996F7";

        /// <summary>
        ///     The unique ID for the reviewer role, to prevent loosing data on migration
        /// </summary>
        public const string ReviewerRoleId = "28B83519-FB7C-4A9A-8279-194140BFCFBE";

        /// <summary>
        ///     Configures the entity of type <see cref="Role"/>
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasData(new Role(new Guid(ProjectAdministratorRoleId))
            {
                RoleName = "Project Administrator",
                AccessRights = new List<AccessRight>()
                {
                    AccessRight.ManageParticipant,
                    AccessRight.CreateTask,
                    AccessRight.DeleteTask,
                    AccessRight.UpdateTask
                }
            }, new Role(new Guid(ReviewerRoleId))
            {
                RoleName = "Reviewer",
                AccessRights = new List<AccessRight>()
                {
                    AccessRight.ReviewTask
                }
            });
        }
    }
}
