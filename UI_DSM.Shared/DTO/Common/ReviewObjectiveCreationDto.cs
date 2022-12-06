// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveCreationDto.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Shared.DTO.Common
{
    using System.Xml.Linq;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Data transfer object used to specify the <see cref="ReviewObjective" /> and contained <see cref="ReviewTask" />
    ///     that needs to be created
    /// </summary>
    public class ReviewObjectiveCreationDto
    {
        /// <summary>
        ///     The <see cref="ReviewObjectiveKind" />
        /// </summary>
        public ReviewObjectiveKind Kind { get; set; }

        /// <summary>
        ///     The <see cref="ReviewObjective.ReviewObjectiveKindNumber" />
        /// </summary>
        public int KindNumber { get; set; }

        public override string ToString()
        {
            return Kind.ToString().ToUpper() + " - " + KindNumber;
        }
    }
}
