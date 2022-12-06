// --------------------------------------------------------------------------------------------------------
// <copyright file="EntityRequestResponseDtoSerializer.cs" company="RHEA System S.A.">
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

    /// <summary>
    ///     The purpose of the <see cref="EntityRequestResponseDtoSerializer" /> is to provide serialization capabilities
    /// </summary>
    internal static class EntityRequestResponseDtoSerializer
    {
        /// <summary>
        ///     Serializes an instance of <see cref="EntityRequestResponseDto" /> using an <see cref="Utf8JsonWriter" />
        /// </summary>
        /// <param name="obj">The <see cref="EntityRequestResponseDto" /> to serialize</param>
        /// <param name="writer"> The target <see cref="Utf8JsonWriter" /></param>
        internal static void Serialize(object obj, Utf8JsonWriter writer)
        {
            if (obj is not EntityRequestResponseDto dto)
            {
                throw new ArgumentException("The object shall be an EntityRequestResponseDto", nameof(obj));
            }

            writer.WriteStartObject();

            writer.WritePropertyName("@type");
            writer.WriteStringValue("EntityRequestResponseDto");

            writer.WritePropertyName("isRequestSuccessful");
            writer.WriteBooleanValue(dto.IsRequestSuccessful);

            writer.WriteStartArray("errors");

            foreach (var dtoError in dto.Errors)
            {
                writer.WriteStringValue(dtoError);
            }

            writer.WriteEndArray();

            writer.WriteStartArray("entities");

            foreach (var entityDto in dto.Entities)
            {
                var serializationAction = SerializationProvider.Provide(entityDto.GetType());
                serializationAction(entityDto, writer);
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}
