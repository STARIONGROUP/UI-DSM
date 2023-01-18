// --------------------------------------------------------------------------------------------------------
// <copyright file="BudgetTemplateDtoDeserializer.cs" company="RHEA System S.A.">
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

// --------------------------------------------------------------------------------------------------------
// ------------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!------------
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Serializer.Json
{
    using System.Text.Json;

    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     The purpose of the <see cref="BudgetTemplateDtoDeserializer" /> is to provide deserialization capabilities
    /// </summary>
    internal static class BudgetTemplateDtoDeserializer
    {
        /// <summary>
        ///     Deserializes an instance of <see cref="BudgetTemplateDto" /> using an <see cref="JsonElement"/>
        /// </summary>
        /// <param name="jsonElement">The <see cref="JsonElement"/> that contains the <see cref="BudgetTemplateDto"/> json object</param>
        /// <returns>An instance of <see cref="BudgetTemplateDto"/></returns>
        internal static BudgetTemplateDto Deserialize(JsonElement jsonElement)
        {
            if (!jsonElement.TryGetProperty("@type", out var type))
            {
                throw new InvalidOperationException("The @type property is not available, the BudgetTemplateDtoDeSerializer cannot be used to deserialize this JsonElement");
            }

            if (type.GetString() != "BudgetTemplateDto")
            {
                throw new InvalidOperationException($"The BudgetTemplateDtoDeserializer can only be used to deserialize objects of type BudgetTemplateDto, a  {type.GetString()} was provided");
            }

            var dto = new BudgetTemplateDto();

            if (jsonElement.TryGetProperty("id", out var idProperty))
            {
                var propertyValue = idProperty.GetString();

                if (propertyValue == null)
                {
                    throw new JsonException("The id property is not present, the BudgetTemplateDto cannot be deserialized");
                }

                dto.Id = Guid.Parse(propertyValue);
            }

            if (jsonElement.TryGetProperty("budgetName", out var budgetNameProperty))
            {
                dto.BudgetName = budgetNameProperty.GetString();
            }

            if (jsonElement.TryGetProperty("fileName", out var fileNameProperty))
            {
                dto.FileName = fileNameProperty.GetString();
            }

            return dto;
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------