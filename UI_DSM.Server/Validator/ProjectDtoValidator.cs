// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectDtoValidator.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Validator
{
    using FluentValidation;

    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     <see cref="AbstractValidator{T}" /> for the <see cref="ProjectDto" />
    /// </summary>
    public class ProjectDtoValidator : AbstractValidator<ProjectDto>
    {
        /// <summary>
        ///     Initializes a new <see cref="ProjectDtoValidator" />
        /// </summary>
        public ProjectDtoValidator()
        {
            this.RuleFor(x => x.ProjectName).NotEmpty();
        }
    }
}
