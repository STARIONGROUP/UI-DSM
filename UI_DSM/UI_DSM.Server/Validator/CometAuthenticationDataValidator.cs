// --------------------------------------------------------------------------------------------------------
// <copyright file="CometAuthenticationDataValidator.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
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

    using UI_DSM.Shared.DTO.CometData;

    /// <summary>
    ///     <see cref="AbstractValidator{T}" /> for <see cref="CometAuthenticationData" />
    /// </summary>
    public class CometAuthenticationDataValidator : AbstractValidator<CometAuthenticationData>
    {
        /// <summary>
        ///     Initializes a new <see cref="CometAuthenticationDataValidator" />
        /// </summary>
        public CometAuthenticationDataValidator()
        {
            this.RuleFor(x => x.Url).NotEmpty();
            this.RuleFor(x => x.UserName).NotEmpty();
            this.RuleFor(x => x.Password).NotEmpty();
        }
    }
}
