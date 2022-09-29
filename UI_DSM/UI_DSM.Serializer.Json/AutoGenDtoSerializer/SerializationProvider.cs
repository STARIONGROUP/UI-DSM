// --------------------------------------------------------------------------------------------------------
// <copyright file="SerializationProvider.cs" company="RHEA System S.A.">
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
    ///     Delegate provider for the appropriate serialization method to serialize a <see cref="Type" />
    /// </summary>
    internal static class SerializationProvider
    {
        private static readonly Dictionary<Type, Action<object, Utf8JsonWriter>> SerializerActionMap = new()
        {
            { typeof(CommentDto), CommentDtoSerializer.Serialize },
            { typeof(FeedbackDto), FeedbackDtoSerializer.Serialize },
            { typeof(ModelDto), ModelDtoSerializer.Serialize },
            { typeof(NoteDto), NoteDtoSerializer.Serialize },
            { typeof(ParticipantDto), ParticipantDtoSerializer.Serialize },
            { typeof(ProjectDto), ProjectDtoSerializer.Serialize },
            { typeof(ReplyDto), ReplyDtoSerializer.Serialize },
            { typeof(ReviewDto), ReviewDtoSerializer.Serialize },
            { typeof(ReviewObjectiveDto), ReviewObjectiveDtoSerializer.Serialize },
            { typeof(ReviewTaskDto), ReviewTaskDtoSerializer.Serialize },
            { typeof(RoleDto), RoleDtoSerializer.Serialize },
            { typeof(UserEntityDto), UserEntityDtoSerializer.Serialize },
        };

        /// <summary>
        ///     Provides the delegate <see cref="Action{Object, Utf8JsonWriter}"/> for the
        /// <see cref="Type"/> that is to be serialized
        /// </summary>
        /// <param name="type">The subject <see cref="Type"/> that is to be serialized </param>
        /// <returns>A Delegate of <see cref="Action{Object, Utf8JsonWriter}"/> </returns>
        /// <exception cref="NotSupportedException">
        ///     Thrown when the <see cref="Type"/> is not supported.
        /// </exception>
        internal static Action<object, Utf8JsonWriter> Provide(Type type)
        {
            if (!SerializerActionMap.TryGetValue(type, out var action))
            {
                throw new NotSupportedException($"The {type.Name} is not supported by the SerializationProvider.");
            }

            return action;
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------