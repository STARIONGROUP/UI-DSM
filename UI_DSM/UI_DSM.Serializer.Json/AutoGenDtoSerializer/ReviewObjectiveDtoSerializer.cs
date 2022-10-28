// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveDtoSerializer.cs" company="RHEA System S.A.">
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
    ///     The purpose of the <see cref="ReviewObjectiveDtoSerializer" /> is to provide serialization capabilities
    /// </summary>
    internal static class ReviewObjectiveDtoSerializer
    {
        /// <summary>
        ///     Serializes an instance of <see cref="ReviewObjectiveDto" /> using an <see cref="Utf8JsonWriter"/>
        /// </summary>
        /// <param name="obj">The <see cref="ReviewObjectiveDto" /> to serialize</param>
        /// <param name="writer"> The target <see cref="Utf8JsonWriter" /></param>
        internal static void Serialize(object obj, Utf8JsonWriter writer)
        {
            if (obj is not ReviewObjectiveDto dto)
            {
                throw new ArgumentException("The object shall be an ReviewObjectiveDto", nameof(obj));
            }

            writer.WriteStartObject();

            writer.WritePropertyName("@type");
            writer.WriteStringValue("ReviewObjectiveDto");

            writer.WritePropertyName("author");
            writer.WriteStringValue(dto.Author);

            writer.WritePropertyName("createdOn");
            writer.WriteStringValue(dto.CreatedOn);

            writer.WritePropertyName("title");
            writer.WriteStringValue(dto.Title);

            writer.WritePropertyName("description");
            writer.WriteStringValue(dto.Description);

            writer.WritePropertyName("reviewObjectiveKind");
            writer.WriteStringValue(dto.ReviewObjectiveKind.ToString().ToUpper());

            writer.WritePropertyName("reviewObjectiveKindNumber");
            writer.WriteNumberValue(dto.ReviewObjectiveKindNumber);

            writer.WritePropertyName("reviewObjectiveNumber");
            writer.WriteNumberValue(dto.ReviewObjectiveNumber);

            writer.WritePropertyName("status");
            writer.WriteStringValue(dto.Status.ToString().ToUpper());

            writer.WriteStartArray("reviewTasks");

            foreach (var item in dto.ReviewTasks)
            {
                writer.WriteStringValue(item);
            }

            writer.WriteEndArray();

            writer.WriteStartArray("relatedViews");

            foreach (var item in dto.RelatedViews)
            {
                writer.WriteStringValue(item.ToString().ToUpper());
            }

            writer.WriteEndArray();

            writer.WriteStartArray("reviewCategories");

            foreach (var item in dto.ReviewCategories)
            {
                writer.WriteStringValue(item);
            }

            writer.WriteEndArray();

            writer.WriteStartArray("annotations");

            foreach (var item in dto.Annotations)
            {
                writer.WriteStringValue(item);
            }

            writer.WriteEndArray();

            writer.WritePropertyName("id");
            writer.WriteStringValue(dto.Id);

            writer.WriteEndObject();
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------