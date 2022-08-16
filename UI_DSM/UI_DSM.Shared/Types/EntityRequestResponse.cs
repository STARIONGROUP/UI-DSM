// --------------------------------------------------------------------------------------------------------
// <copyright file="EntityRequestResponse.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.Types
{
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     A <see cref="RequestResponseDto" /> with an <see cref="TEntity" /> object
    /// </summary>
    /// <typeparam name="TEntity">An <see cref="Entity" /></typeparam>
    public class EntityRequestResponse<TEntity> : RequestResponseDto where TEntity : Entity
    {
        /// <summary>
        ///     Gets or sets the <see cref="TEntity" />
        /// </summary>
        public TEntity Entity { get; set; }
    }
}
