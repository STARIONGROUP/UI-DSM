// --------------------------------------------------------------------------------------------------------
// <copyright file="DeserializationProvider.cs" company="RHEA System S.A.">
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
    using System;
    using System.Collections.Generic;
    using System.Text.Json;

    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     Delegate provider for the appropriate deserialization method to serialize a <see cref="Type" />
    /// </summary>
    internal static class DeserializationProvider
    {
        private static readonly Dictionary<string, Func<JsonElement, EntityDto>> DeserializerActionMap = new()
        {
            { "CommentDto", CommentDtoDeserializer.Deserialize },
            { "FeedbackDto", FeedbackDtoDeserializer.Deserialize },
            { "ModelDto", ModelDtoDeserializer.Deserialize },
            { "NoteDto", NoteDtoDeserializer.Deserialize },
            { "ParticipantDto", ParticipantDtoDeserializer.Deserialize },
            { "ProjectDto", ProjectDtoDeserializer.Deserialize },
            { "ReplyDto", ReplyDtoDeserializer.Deserialize },
            { "ReviewCategoryDto", ReviewCategoryDtoDeserializer.Deserialize },
            { "ReviewDto", ReviewDtoDeserializer.Deserialize },
            { "ReviewObjectiveDto", ReviewObjectiveDtoDeserializer.Deserialize },
            { "ReviewTaskDto", ReviewTaskDtoDeserializer.Deserialize },
            { "RoleDto", RoleDtoDeserializer.Deserialize },
            { "UserEntityDto", UserEntityDtoDeserializer.Deserialize },
        };

        /// <summary>
        ///     Provides the delegate <see cref="Func{JsonElement, EntityDto}"/> for the provided typeName that is to be serialized
        /// </summary>
        /// <param name="typeName">The subject <see cref="Type"/> name that is to be serialized </param>
        /// <returns>A Delegate of <see cref="Func{JsonElement, EntityDto}"/> </returns>
        /// <exception cref="NotSupportedException">
        ///     Thrown when the <see cref="Type"/> name is not supported.
        /// </exception>
        internal static Func<JsonElement, EntityDto> Provide(string typeName)
        {
            if (!DeserializerActionMap.TryGetValue(typeName, out var func))
            {
                throw new NotSupportedException($"The {typeName} is not supported by the DeserializationProvider.");
            }

            return func;
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------