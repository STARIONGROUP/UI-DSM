// --------------------------------------------------------------------------------------------------------
// <copyright file="DeepLevelAnnotation.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.Annotations
{
    /// <summary>
    ///     <see cref="Attribute" /> to specify the deep level for a Property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DeepLevelAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DeepLevelAttribute" /> class.
        /// </summary>
        public DeepLevelAttribute(int level)
        {
            if (level < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(level), "The deep level should be at least equal to 0");
            }

            this.Level = level;
        }

        /// <summary>
        ///     The deep level
        /// </summary>
        public int Level { get; }
    }
}
