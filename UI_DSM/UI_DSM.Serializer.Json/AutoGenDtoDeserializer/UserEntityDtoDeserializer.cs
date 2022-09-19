// --------------------------------------------------------------------------------------------------------
// <copyright file="UserEntityDtoDeserializer.cs" company="RHEA System S.A.">
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
    ///     The purpose of the <see cref="UserEntityDtoDeserializer" /> is to provide deserialization capabilities
    /// </summary>
    internal static class UserEntityDtoDeserializer
    {
        /// <summary>
        ///     Deserializes an instance of <see cref="UserEntityDto" /> using an <see cref="JsonElement"/>
        /// </summary>
        /// <param name="jsonElement">The <see cref="JsonElement"/> that contains the <see cref="UserEntityDto"/> json object</param>
        /// <returns>An instance of <see cref="UserEntityDto"/></returns>
        internal static UserEntityDto Deserialize(JsonElement jsonElement)
        {
            if (!jsonElement.TryGetProperty("@type", out var type))
            {
                throw new InvalidOperationException("The @type property is not available, the UserEntityDtoDeSerializer cannot be used to deserialize this JsonElement");
            }

            if (type.GetString() != "UserEntityDto")
            {
                throw new InvalidOperationException($"The UserEntityDtoDeserializer can only be used to deserialize objects of type UserEntityDto, a  {type.GetString()} was provided");
            }

            var dto = new UserEntityDto();

            if (jsonElement.TryGetProperty("id", out var idProperty))
            {
                var propertyValue = idProperty.GetString();

                if (propertyValue == null)
                {
                    throw new JsonException("The id property is not present, the UserEntityDto cannot be deserialized");
                }

                dto.Id = Guid.Parse(propertyValue);
            }

            if (jsonElement.TryGetProperty("userName", out var userNameProperty))
            {
                dto.UserName = userNameProperty.GetString();
            }

            if (jsonElement.TryGetProperty("isAdmin", out var isAdminProperty))
            {
                dto.IsAdmin = isAdminProperty.GetBoolean();
            }

            return dto;
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------