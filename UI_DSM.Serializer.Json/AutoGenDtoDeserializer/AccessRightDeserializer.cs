// --------------------------------------------------------------------------------------------------------
// <copyright file="AccessRightDeserializer.cs" company="RHEA System S.A.">
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
    using UI_DSM.Shared.Enumerator;

    /// <summary>
    ///     The purpose of the <see cref="AccessRightDeserializer" /> is to provide deserialization capabilities for enum
    /// </summary>
    internal static class AccessRightDeserializer
    {
        /// <summary>
        ///     Deserializes a string value to a <see cref="AccessRight"/>
        /// </summary>
        /// <param name="value">The string representation of the <see cref="AccessRight"/></param>
        /// <returns>The value of the <see cref="AccessRight"/></returns>
        internal static AccessRight Deserialize(string value)
        {
            return value switch
            {
                "REVIEWTASK" => AccessRight.ReviewTask,
                "CREATEREVIEW" => AccessRight.CreateReview,
                "DELETEREVIEW" => AccessRight.DeleteReview,
                "CREATEREVIEWOBJECTIVE" => AccessRight.CreateReviewObjective,
                "DELETEREVIEWOBJECTIVE" => AccessRight.DeleteReviewObjective,
                "ASSIGNTASK" => AccessRight.AssignTask,
                "PROJECTMANAGEMENT" => AccessRight.ProjectManagement,
                _ => throw new ArgumentException($"{value} is not a valid AccessRight", nameof(value))
            };
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------