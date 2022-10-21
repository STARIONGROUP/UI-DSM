// --------------------------------------------------------------------------------------------------------
// <copyright file="Project.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using UI_DSM.Shared.Annotations;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     A <see cref="Project" /> represents a container for reviews
    /// </summary>
    [Table(nameof(Project))]
    public class Project : Entity
    {
        /// <summary>
        ///     Initialize a new <see cref="Project" />
        /// </summary>
        public Project()
        {
            this.InitializeCollections();
        }

        /// <summary>
        ///     Inilializes a new <see cref="Project" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="Project" /></param>
        public Project(Guid id) : base(id)
        {
            this.InitializeCollections();
        }

        /// <summary>
        ///     Gets or sets the name of the <see cref="Project" />
        /// </summary>
        [Required]
        public string ProjectName { get; set; }

        /// <summary>
        ///     Gets or sets the collection of <see cref="Participant" />
        /// </summary>
        [DeepLevel(1)]
        public EntityContainerList<Participant> Participants { get; protected set; }

        /// <summary>
        ///     Gets or sets the collection of <see cref="Review" />
        /// </summary>
        [DeepLevel(1)]
        public EntityContainerList<Review> Reviews { get; protected set; }

        /// <summary>
        ///     A collection of contained <see cref="Annotations" />
        /// </summary>
        [DeepLevel(1)]
        public EntityContainerList<Annotation> Annotations { get; set; }

        /// <summary>
        ///     A collection of contained <see cref="Artifact" />
        /// </summary>
        [DeepLevel(1)]
        public EntityContainerList<Artifact> Artifacts { get; set; }

        /// <summary>
        ///     A collection of given <see cref="ReviewCategory" />
        /// </summary>
        [DeepLevel(0)]
        public List<ReviewCategory> ReviewCategories { get; set; } = new();

        /// <summary>
        ///     The <see cref="DateTime" /> where the <see cref="Project" /> has been created
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        ///     Instantiate a <see cref="EntityDto" /> from a <see cref="Entity" />
        /// </summary>
        /// <returns>A new <see cref="EntityDto" /></returns>
        public override EntityDto ToDto()
        {
            var dto = new ProjectDto(this.Id)
            {
                ProjectName = this.ProjectName,
                CreatedOn = this.CreatedOn,
                Participants = new List<Guid>(this.Participants.Select(x => x.Id)),
                Reviews = new List<Guid>(this.Reviews.Select(x => x.Id)),
                Annotations = new List<Guid>(this.Annotations.Select(x => x.Id)),
                Artifacts = new List<Guid>(this.Artifacts.Select(x => x.Id)),
                ReviewCategories = new List<Guid>(this.ReviewCategories.Select(x => x.Id))
            };

            return dto;
        }

        /// <summary>
        ///     Resolve the properties of the current <see cref="Project" /> from its <see cref="EntityDto" /> counter-part
        /// </summary>
        /// <param name="entityDto">The source <see cref="EntityDto" /></param>
        /// <param name="resolvedEntity">A <see cref="Dictionary{TKey,TValue}" /> of all <see cref="Entity" /></param>
        public override void ResolveProperties(EntityDto entityDto, Dictionary<Guid, Entity> resolvedEntity)
        {
            if (entityDto is not ProjectDto projectDto)
            {
                throw new InvalidOperationException($"The DTO {entityDto.GetType()} does not match with the current Project POCO");
            }

            this.ProjectName = projectDto.ProjectName;
            this.CreatedOn = projectDto.CreatedOn;
            this.Participants.ResolveList(projectDto.Participants, resolvedEntity);
            this.Reviews.ResolveList(projectDto.Reviews, resolvedEntity);
            this.Annotations.ResolveList(projectDto.Annotations, resolvedEntity);
            this.Artifacts.ResolveList(projectDto.Artifacts, resolvedEntity);
            this.ReviewCategories.ResolveList(projectDto.ReviewCategories, resolvedEntity);
        }

        /// <summary>
        ///     Initializes all collections for this <see cref="Entity" />
        /// </summary>
        private void InitializeCollections()
        {
            this.Participants = new EntityContainerList<Participant>(this);
            this.Reviews = new EntityContainerList<Review>(this);
            this.Annotations = new EntityContainerList<Annotation>(this);
            this.Artifacts = new EntityContainerList<Artifact>(this);
            this.ReviewCategories = new List<ReviewCategory>();
        }
    }
}
