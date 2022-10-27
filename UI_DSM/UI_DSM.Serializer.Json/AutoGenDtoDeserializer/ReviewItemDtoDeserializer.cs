// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewItemDtoDeserializer.cs" company="RHEA System S.A.">
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
    ///     The purpose of the <see cref="ReviewItemDtoDeserializer" /> is to provide deserialization capabilities
    /// </summary>
    internal static class ReviewItemDtoDeserializer
    {
        /// <summary>
        ///     Deserializes an instance of <see cref="ReviewItemDto" /> using an <see cref="JsonElement"/>
        /// </summary>
        /// <param name="jsonElement">The <see cref="JsonElement"/> that contains the <see cref="ReviewItemDto"/> json object</param>
        /// <returns>An instance of <see cref="ReviewItemDto"/></returns>
        internal static ReviewItemDto Deserialize(JsonElement jsonElement)
        {
            if (!jsonElement.TryGetProperty("@type", out var type))
            {
                throw new InvalidOperationException("The @type property is not available, the ReviewItemDtoDeSerializer cannot be used to deserialize this JsonElement");
            }

            if (type.GetString() != "ReviewItemDto")
            {
                throw new InvalidOperationException($"The ReviewItemDtoDeserializer can only be used to deserialize objects of type ReviewItemDto, a  {type.GetString()} was provided");
            }

            var dto = new ReviewItemDto();

            if (jsonElement.TryGetProperty("id", out var idProperty))
            {
                var propertyValue = idProperty.GetString();

                if (propertyValue == null)
                {
                    throw new JsonException("The id property is not present, the ReviewItemDto cannot be deserialized");
                }

                dto.Id = Guid.Parse(propertyValue);
            }

            if (jsonElement.TryGetProperty("reviewCategories", out var reviewCategoriesProperty))
            {
                foreach (var item in reviewCategoriesProperty.EnumerateArray())
                {
                    var propertyValue = item.GetString();

                    if (propertyValue != null)
                    {
                        dto.ReviewCategories.Add(Guid.Parse(propertyValue));
                    }
                }
            }

            if (jsonElement.TryGetProperty("thingId", out var thingIdProperty))
            {
                var propertyValue = thingIdProperty.GetString();

                if (propertyValue != null)
                {
                    dto.ThingId = Guid.Parse(propertyValue);
                }
            }

            if (jsonElement.TryGetProperty("annotations", out var annotationsProperty))
            {
                foreach (var item in annotationsProperty.EnumerateArray())
                {
                    var propertyValue = item.GetString();

                    if (propertyValue != null)
                    {
                        dto.Annotations.Add(Guid.Parse(propertyValue));
                    }
                }
            }

            return dto;
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------