// --------------------------------------------------------------------------------------------------------
// <copyright file="EntityRequestResponseDtoDeserializer.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Serializer.Json
{
    using System.Text.Json;

    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     The purpose of the <see cref="EntityRequestResponseDtoDeserializer" /> is to provide deserialization capabilities
    /// </summary>
    public static class EntityRequestResponseDtoDeserializer
    {
        /// <summary>
        ///     Deserializes an instance of <see cref="EntityRequestResponseDto" /> using an <see cref="JsonElement" />
        /// </summary>
        /// <param name="jsonElement">The <see cref="JsonElement" /> that contains the <see cref="EntityRequestResponseDto" /> json object</param>
        /// <returns>An instance of <see cref="EntityRequestResponseDto" /></returns>
        internal static EntityRequestResponseDto Deserialize(JsonElement jsonElement)
        {
            if (!jsonElement.TryGetProperty("@type", out var type))
            {
                throw new InvalidOperationException("The @type property is not available, the EntityRequestResponseDtoDeserializer cannot be used to deserialize this JsonElement");
            }

            if (type.GetString() != "EntityRequestResponseDto")
            {
                throw new InvalidOperationException($"The EntityRequestResponseDtoDeserializer can only be used to deserialize objects of type EntityRequestResponseDto, a  {type.GetString()} was provided");
            }

            var dto = new EntityRequestResponseDto();

            if (jsonElement.TryGetProperty("isRequestSuccessful", out var isRequestSuccessfulProperty))
            {
                dto.IsRequestSuccessful = isRequestSuccessfulProperty.GetBoolean();
            }

            if (jsonElement.TryGetProperty("errors", out var repliesProperty))
            {
                foreach (var item in repliesProperty.EnumerateArray())
                {
                    var propertyValue = item.GetString();

                    if (propertyValue != null)
                    {
                        dto.Errors.Add(propertyValue);
                    }
                }
            }

            if (jsonElement.TryGetProperty("entities", out var entitiesProperty))
            {
                var entities = new List<EntityDto>();

                foreach (var entity in entitiesProperty.EnumerateArray())
                {
                    if (!entity.TryGetProperty("@type", out var entityType))
                    {
                        throw new InvalidOperationException("The @type property is not available, the EntityRequestResponseDtoDeserializer cannot be used to deserialize this JsonElement");
                    }

                    var typeName = entityType.GetString();
                    var func = DeserializationProvider.Provide(typeName);
                    entities.Add(func(entity));
                }

                dto.Entities = entities;
            }

            return dto;
        }
    }
}
