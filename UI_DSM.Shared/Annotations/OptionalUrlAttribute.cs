// --------------------------------------------------------------------------------------------------------
// <copyright file="OptionalUrlAttribute.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.Annotations
{
    using System.ComponentModel.DataAnnotations;

    using UI_DSM.Shared.DTO.CometData;

    /// <summary>
    ///     The <see cref="OptionalUrlAttribute" /> is used when a property when an URL is optional on a certain case
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OptionalUrlAttribute : ValidationAttribute
    {
        /// <summary>
        ///     The <see cref="UrlAttribute" /> to validate the URL if needed
        /// </summary>
        private readonly UrlAttribute urlAttribute = new();

        /// <summary>Validates the specified value with respect to the current validation attribute.</summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <exception cref="T:System.InvalidOperationException">The current attribute is malformed.</exception>
        /// <exception cref="T:System.NotImplementedException">
        ///     <see
        ///         cref="M:System.ComponentModel.DataAnnotations.ValidationAttribute.IsValid(System.Object,System.ComponentModel.DataAnnotations.ValidationContext)" />
        ///     has not been implemented by a derived class.
        /// </exception>
        /// <returns>An instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult" /> class.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance is CometAuthenticationData uploadData)
            {
                if (!uploadData.UploadFromFile && !string.IsNullOrEmpty(uploadData.Url))
                {
                    return this.urlAttribute.IsValid(uploadData.Url)
                        ? ValidationResult.Success
                        : new ValidationResult(string.Format(this.urlAttribute.ErrorMessage!, "Server address"));
                }

                if (uploadData.UploadFromFile && string.IsNullOrEmpty(uploadData.Url))
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(this.ErrorMessage);
        }
    }
}
