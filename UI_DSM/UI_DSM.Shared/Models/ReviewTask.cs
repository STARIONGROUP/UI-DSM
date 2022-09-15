// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewTask.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw
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

    /// <summary>
    ///     A <see cref="ReviewTask" /> is a task to be fulfilled as part of the review objective.
    /// </summary>
    [Table(nameof(ReviewTask))]
    public class ReviewTask : Entity
    {
        /// <summary>
        ///     Initializes a new <see cref="ReviewTask" />
        /// </summary>
        public ReviewTask()
        {
        }

        /// <summary>
        ///     Inilializes a new <see cref="ReviewTask" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="ReviewTask" /></param>
        public ReviewTask(Guid id) : base(id)
        {
        }

        /// <summary>
        ///     The title of the <see cref="ReviewTask" />
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        ///     The description of the <see cref="ReviewTask" />
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        ///     The number of the <see cref="ReviewTask" /> inside the <see cref="ReviewObjective" />
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskNumber { get; set; }

        /// <summary>
        ///     The current <see cref="StatusKind" />
        /// </summary>
        public StatusKind Status { get; set; }

        /// <summary>
        ///     The <see cref="DateTime" /> where the <see cref="ReviewTask" /> has been created
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        ///     The author of this <see cref="ReviewTask" />
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DeepLevel(0)]
        public Participant Author { get; set; }

        /// <summary>
        ///     The <see cref="Participant" /> that have to complete this <see cref="ReviewTask" />
        /// </summary>
        [DeepLevel(0)]
        public Participant IsAssignedTo { get; set; }

        /// <summary>
        ///     Instantiate a <see cref="EntityDto" /> from a <see cref="ReviewTask" />
        /// </summary>
        /// <returns>A new <see cref="EntityDto" /></returns>
        public override EntityDto ToDto()
        {
            return new ReviewTaskDto(this.Id)
            {
                Author = this.Author?.Id ?? Guid.Empty,
                CreatedOn = this.CreatedOn,
                Description = this.Description,
                IsAssignedTo = this.IsAssignedTo?.Id ?? Guid.Empty,
                Status = this.Status,
                TaskNumber = this.TaskNumber,
                Title = this.Title
            };
        }

        /// <summary>
        ///     Resolve the properties of the current <see cref="Entity" /> from its <see cref="EntityDto" /> counter-part
        /// </summary>
        /// <param name="entityDto">The source <see cref="EntityDto" /></param>
        /// <param name="resolvedEntity">A <see cref="Dictionary{TKey,TValue}" /> of all others <see cref="Entity" /></param>
        public override void ResolveProperties(EntityDto entityDto, Dictionary<Guid, Entity> resolvedEntity)
        {
            if (entityDto is not ReviewTaskDto reviewTaskDto)
            {
                throw new InvalidOperationException($"The DTO {entityDto.GetType()} does not match with the current ReviewTask POCO");
            }

            this.Author = this.GetEntity<Participant>(reviewTaskDto.Author, resolvedEntity);
            this.IsAssignedTo = this.GetEntity<Participant>(reviewTaskDto.IsAssignedTo, resolvedEntity);
            this.Title = reviewTaskDto.Title;
            this.Description = reviewTaskDto.Description;
            this.Status = reviewTaskDto.Status;
            this.TaskNumber = reviewTaskDto.TaskNumber;
            this.CreatedOn = reviewTaskDto.CreatedOn;
        }
    }
}
