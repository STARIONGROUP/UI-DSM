// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewDtoDeserializer.cs" company="RHEA System S.A.">
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
    ///     The purpose of the <see cref="ReviewDtoDeserializer" /> is to provide deserialization capabilities
    /// </summary>
    internal static class ReviewDtoDeserializer
    {
        /// <summary>
        ///     Deserializes an instance of <see cref="ReviewDto" /> using an <see cref="JsonElement"/>
        /// </summary>
        /// <param name="jsonElement">The <see cref="JsonElement"/> that contains the <see cref="ReviewDto"/> json object</param>
        /// <returns>An instance of <see cref="ReviewDto"/></returns>
        internal static ReviewDto Deserialize(JsonElement jsonElement)
        {
            if (!jsonElement.TryGetProperty("@type", out var type))
            {
                throw new InvalidOperationException("The @type property is not available, the ReviewDtoDeSerializer cannot be used to deserialize this JsonElement");
            }

            if (type.GetString() != "ReviewDto")
            {
                throw new InvalidOperationException($"The ReviewDtoDeserializer can only be used to deserialize objects of type ReviewDto, a  {type.GetString()} was provided");
            }

            var dto = new ReviewDto();

            if (jsonElement.TryGetProperty("id", out var idProperty))
            {
                var propertyValue = idProperty.GetString();

                if (propertyValue == null)
                {
                    throw new JsonException("The id property is not present, the ReviewDto cannot be deserialized");
                }

                dto.Id = Guid.Parse(propertyValue);
            }

            if (jsonElement.TryGetProperty("createdOn", out var createdOnProperty))
            {
                var propertyValue = createdOnProperty.GetString();

                if (propertyValue != null)
                {
                    dto.CreatedOn = DateTime.Parse(propertyValue);
                }
            }

            if (jsonElement.TryGetProperty("author", out var authorProperty))
            {
                var propertyValue = authorProperty.GetString();

                if (propertyValue != null)
                {
                    dto.Author = Guid.Parse(propertyValue);
                }
            }

            if (jsonElement.TryGetProperty("reviewNumber", out var reviewNumberProperty))
            {
                dto.ReviewNumber = reviewNumberProperty.GetInt32();
            }

            if (jsonElement.TryGetProperty("title", out var titleProperty))
            {
                dto.Title = titleProperty.GetString();
            }

            if (jsonElement.TryGetProperty("description", out var descriptionProperty))
            {
                dto.Description = descriptionProperty.GetString();
            }

            if (jsonElement.TryGetProperty("status", out var statusProperty))
            {
                var propertyValue = statusProperty.GetString();

                if (propertyValue != null)
                {
                    dto.Status = StatusKindDeserializer.Deserialize(propertyValue);
                }
            }

            if (jsonElement.TryGetProperty("reviewObjectives", out var reviewObjectivesProperty))
            {
                foreach (var item in reviewObjectivesProperty.EnumerateArray())
                {
                    var propertyValue = item.GetString();

                    if (propertyValue != null)
                    {
                        dto.ReviewObjectives.Add(Guid.Parse(propertyValue));
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