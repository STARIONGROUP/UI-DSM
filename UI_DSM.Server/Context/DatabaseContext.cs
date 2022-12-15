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
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    using UI_DSM.Server.Context.Configuration;
    using UI_DSM.Shared.DTO.Common;
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
            //this.EnsureCreated();
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
        ///     A <see cref="DbSet{TEntity}" /> of <see cref="Participants" />
        /// </summary>
        public virtual DbSet<Participant> Participants { get; set; }

        /// <summary>
        ///     A <see cref="DbSet{TEntity}" /> of <see cref="Role" />
        /// </summary>
        public virtual DbSet<Role> UiDsmRoles { get; set; }

        /// <summary>
        ///     A <see cref="DbSet{TEntity}" /> of <see cref="UserEntity" />
        /// </summary>
        public virtual DbSet<UserEntity> UsersEntities { get; set; }

        /// <summary>
        ///     A <see cref="DbSet{TEntity}" /> of <see cref="Review" />
        /// </summary>
        public virtual DbSet<Review> Reviews { get; set; }

        /// <summary>
        ///     A <see cref="DbSet{TEntity}" /> of <see cref="ReviewObjective" />
        /// </summary>
        public virtual DbSet<ReviewObjective> ReviewObjectives { get; set; }

        /// <summary>
        ///     A <see cref="DbSet{TEntity}" /> of <see cref="ReviewTask" />
        /// </summary>
        public virtual DbSet<ReviewTask> ReviewTasks { get; set; }

        /// <summary>
        ///     A <see cref="DbSet{TEntity}" /> of <see cref="Comment" />
        /// </summary>
        public virtual DbSet<Comment> Comments { get; set; }

        /// <summary>
        ///     A <see cref="DbSet{TEntity}" /> of <see cref="Reply" />
        /// </summary>
        public virtual DbSet<Reply> Replies { get; set; }

        /// <summary>
        ///     A <see cref="DbSet{TEntity}" /> of <see cref="Feedback" />
        /// </summary>
        public virtual DbSet<Feedback> Feedbacks { get; set; }

        /// <summary>
        ///     A <see cref="DbSet{TEntity}" /> of <see cref="Note" />
        /// </summary>
        public virtual DbSet<Note> Notes { get; set; }

        /// <summary>
        ///     A <see cref="DbSet{TEntity}" /> of <see cref="Model" />
        /// </summary>
        public virtual DbSet<Model> Models { get; set; }

        /// <summary>
        ///     A <see cref="DbSet{TEntity}" /> of <see cref="ReviewItems" />
        /// </summary>
        public virtual DbSet<ReviewItem> ReviewItems { get; set; }

        /// <summary>
        ///     A <see cref="DbSet{TEntity}" /> of <see cref="ReviewCategory" />
        /// </summary>
        public virtual DbSet<ReviewCategory> ReviewCategories { get; set; }

        /// <summary>
        ///     Tries to validate an object
        /// </summary>
        /// <param name="instance">The <see cref="object" /> to validate</param>
        /// <returns>A collection of errors if any</returns>
        public List<string> ValidateModel(object instance)
        {
            var errors = new List<ValidationResult>();
            var validationContext = new ValidationContext(instance);
            Validator.TryValidateObject(instance, validationContext, errors);

            return errors.Select(x => x.ErrorMessage).ToList();
        }

        /// <summary>
        ///     Retrieves information related to the database
        /// </summary>
        /// <returns>The <see cref="DatabaseInformationDto" /></returns>
        public virtual DatabaseInformationDto GetDatabaseInformation()
        {
            var connection = this.Database.GetDbConnection();
            var startTime = DateTime.MinValue;

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT pg_postmaster_start_time()";
                this.Database.OpenConnection();
                var result = command.ExecuteScalar();

                if (result is DateTime retrievedDateTime)
                {
                    startTime = retrievedDateTime;
                }
            }

            var information = new DatabaseInformationDto
            {
                State = connection.State,
                DatabaseVersion = connection.ServerVersion,
                StartTime = startTime
            };

            return information;
        }

        /// <summary>
        ///     Configures the schema needed for the identity framework.
        /// </summary>
        /// <param name="builder">
        ///     The builder being used to construct the model for this context.
        /// </param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Project>().HasIndex(x => x.ProjectName).IsUnique();
            builder.Entity<Role>().HasIndex(x => x.RoleName).IsUnique();
            builder.Entity<Model>().HasIndex(x => x.FileName).IsUnique();
            builder.Entity<ReviewCategory>().HasIndex(x => x.ReviewCategoryName).IsUnique();

            builder.Entity<Project>().HasMany(p => p.Participants)
                .WithOne(p => (Project)p.EntityContainer)
                .HasForeignKey("EntityContainerId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Project>().HasMany(p => p.Reviews)
                .WithOne(p => (Project)p.EntityContainer)
                .HasForeignKey("EntityContainerId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Project>().HasMany(p => p.Annotations)
                .WithOne(p => (Project)p.EntityContainer)
                .HasForeignKey("EntityContainerId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Project>().HasMany(p => p.Artifacts)
                .WithOne(p => (Project)p.EntityContainer)
                .HasForeignKey("EntityContainerId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Review>().HasMany(x => x.Artifacts)
                .WithMany(x => x.Reviews);

            builder.Entity<Review>().HasMany(x => x.ReviewItems)
                .WithOne(ri => (Review)ri.EntityContainer)
                .HasForeignKey("EntityContainerId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Review>().HasMany(x => x.ReviewObjectives)
                .WithOne(ro => (Review)ro.EntityContainer)
                .HasForeignKey("EntityContainerId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Participant>().HasMany(x => x.AssignedTasks)
                .WithMany(x => x.IsAssignedTo);

            builder.Entity<Participant>().HasMany(x => x.Annotations)
                .WithOne(a => a.Author)
                .HasForeignKey("AuthorId")
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<Participant>().HasMany(x => x.Replies)
                .WithOne(r => r.Author)
                .HasForeignKey("AuthorId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ReviewTask>().HasMany(x => x.IsAssignedTo)
                .WithMany(x => x.AssignedTasks);

            builder.Entity<ReviewObjective>().HasMany(x => x.ReviewTasks)
                .WithOne(ro => (ReviewObjective)ro.EntityContainer)
                .HasForeignKey("EntityContainerId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Annotation>().HasMany(a => a.AnnotatableItems)
                .WithMany(x => x.Annotations);

            builder.Entity<AnnotatableItem>().HasMany(a => a.Annotations)
                .WithMany(x => x.AnnotatableItems);

            builder.Entity<Comment>().HasMany(x => x.Replies)
                .WithOne(reply => (Comment)reply.EntityContainer)
                .HasForeignKey("EntityContainerId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new UserEntityConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration());
            builder.ApplyConfiguration(new UserRoleConfiguration());
            builder.ApplyConfiguration(new UiDsmRoleConfiguration());
        }

        /// <summary>
        ///     Ensures that the database is fully created
        /// </summary>
        private void EnsureCreated()
        {
            if (this.Database.GetPendingMigrations().Any())
            {
                this.Database.Migrate();
            }
        }
    }
}
