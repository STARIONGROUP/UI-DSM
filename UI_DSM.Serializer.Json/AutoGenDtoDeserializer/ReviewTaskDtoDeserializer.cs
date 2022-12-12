// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewTaskDtoDeserializer.cs" company="RHEA System S.A.">
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
    ///     The purpose of the <see cref="ReviewTaskDtoDeserializer" /> is to provide deserialization capabilities
    /// </summary>
    internal static class ReviewTaskDtoDeserializer
    {
        /// <summary>
        ///     Deserializes an instance of <see cref="ReviewTaskDto" /> using an <see cref="JsonElement"/>
        /// </summary>
        /// <param name="jsonElement">The <see cref="JsonElement"/> that contains the <see cref="ReviewTaskDto"/> json object</param>
        /// <returns>An instance of <see cref="ReviewTaskDto"/></returns>
        internal static ReviewTaskDto Deserialize(JsonElement jsonElement)
        {
            if (!jsonElement.TryGetProperty("@type", out var type))
            {
                throw new InvalidOperationException("The @type property is not available, the ReviewTaskDtoDeSerializer cannot be used to deserialize this JsonElement");
            }

            if (type.GetString() != "ReviewTaskDto")
            {
                throw new InvalidOperationException($"The ReviewTaskDtoDeserializer can only be used to deserialize objects of type ReviewTaskDto, a  {type.GetString()} was provided");
            }

            var dto = new ReviewTaskDto();

            if (jsonElement.TryGetProperty("id", out var idProperty))
            {
                var propertyValue = idProperty.GetString();

                if (propertyValue == null)
                {
                    throw new JsonException("The id property is not present, the ReviewTaskDto cannot be deserialized");
                }

                dto.Id = Guid.Parse(propertyValue);
            }

            if (jsonElement.TryGetProperty("title", out var titleProperty))
            {
                dto.Title = titleProperty.GetString();
            }

            if (jsonElement.TryGetProperty("description", out var descriptionProperty))
            {
                dto.Description = descriptionProperty.GetString();
            }

            if (jsonElement.TryGetProperty("taskNumber", out var taskNumberProperty))
            {
                dto.TaskNumber = taskNumberProperty.GetInt32();
            }

            if (jsonElement.TryGetProperty("mainView", out var mainViewProperty))
            {
                var propertyValue = mainViewProperty.GetString();

                if (propertyValue != null)
                {
                    dto.MainView = ViewDeserializer.Deserialize(propertyValue);
                }
            }

            if (jsonElement.TryGetProperty("optionalView", out var optionalViewProperty))
            {
                var propertyValue = optionalViewProperty.GetString();

                if (propertyValue != null)
                {
                    dto.OptionalView = ViewDeserializer.Deserialize(propertyValue);
                }
            }

            if (jsonElement.TryGetProperty("additionalView", out var additionalViewProperty))
            {
                var propertyValue = additionalViewProperty.GetString();

                if (propertyValue != null)
                {
                    dto.AdditionalView = ViewDeserializer.Deserialize(propertyValue);
                }
            }

            if (jsonElement.TryGetProperty("hasPrimaryView", out var hasPrimaryViewProperty))
            {
                dto.HasPrimaryView = hasPrimaryViewProperty.GetBoolean();
            }

            if (jsonElement.TryGetProperty("status", out var statusProperty))
            {
                var propertyValue = statusProperty.GetString();

                if (propertyValue != null)
                {
                    dto.Status = StatusKindDeserializer.Deserialize(propertyValue);
                }
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

            if (jsonElement.TryGetProperty("isAssignedTo", out var isAssignedToProperty))
            {
                foreach (var item in isAssignedToProperty.EnumerateArray())
                {
                    var propertyValue = item.GetString();

                    if (propertyValue != null)
                    {
                        dto.IsAssignedTo.Add(Guid.Parse(propertyValue));
                    }
                }
            }

            if (jsonElement.TryGetProperty("prefilters", out var prefiltersProperty))
            {
                foreach (var item in prefiltersProperty.EnumerateArray())
                {
                    var propertyValue = item.GetString();

                    if (propertyValue != null)
                    {
                        dto.Prefilters.Add(propertyValue);
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