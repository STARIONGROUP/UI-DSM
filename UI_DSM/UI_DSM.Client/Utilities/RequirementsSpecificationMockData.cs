// --------------------------------------------------------------------------------------------------------
// <copyright file="RequirementsSpecificationMockData.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Utilities
{
    using CDP4Common.EngineeringModelData;

    /// <summary>
    ///     Utility class to get some mocked data for <see cref="RequirementsSpecification" />
    /// </summary>
    public static class RequirementsSpecificationMockData
    {
        /// <summary>
        ///     A <see cref="Random" />
        /// </summary>
        private static readonly Random rng = new();

        /// <summary>
        ///     Gets a <see cref="RequirementsSpecification" /> with some groups and requirements
        /// </summary>
        /// <returns>A <see cref="RequirementsSpecification" /></returns>
        public static RequirementsSpecification GetRequirementsSpecification()
        {
            var requirementsSpecification = new RequirementsSpecification(Guid.NewGuid(), null, null)
            {
                Name = "Requirements Specification",
                ShortName = "RequirementsSpecification"
            };

            requirementsSpecification.Group.AddRange(GetRequirementsGroup(5));
            requirementsSpecification.Requirement.AddRange(GetRequirements(5 * 5));

            foreach (var requirement in requirementsSpecification.Requirement)
            {
                var groupNumber = rng.Next(0, requirementsSpecification.Group.Count);
                requirement.Group = requirementsSpecification.Group[groupNumber];
            }

            return requirementsSpecification;
        }

        /// <summary>
        ///     Creates some <see cref="Requirement" />
        /// </summary>
        /// <returns>A collection of <see cref="Requirement" /></returns>
        private static IEnumerable<Requirement> GetRequirements(int maxAmount)
        {
            var numberOfRequirement = rng.Next(1, maxAmount + 1);
            var requirements = new List<Requirement>();

            for (var groupCount = 0; groupCount < numberOfRequirement; groupCount++)
            {
                requirements.Add(new Requirement(Guid.NewGuid(), null, null)
                {
                    ShortName = $"Requirement{groupCount}",
                    Name = $"Requirements - {groupCount}"
                });
            }

            return requirements;
        }

        /// <summary>
        ///     Creates some <see cref="RequirementsGroup" />
        /// </summary>
        /// <returns>A collection of <see cref="RequirementsGroup" /></returns>
        private static IEnumerable<RequirementsGroup> GetRequirementsGroup(int maxAmount)
        {
            var numberOfGroup = rng.Next(1, maxAmount + 1);
            var groups = new List<RequirementsGroup>();

            for (var groupCount = 0; groupCount < numberOfGroup; groupCount++)
            {
                groups.Add(new RequirementsGroup(Guid.NewGuid(), null, null)
                {
                    ShortName = $"RequirementsGroup{groupCount}",
                    Name = $"Requirements Group - {groupCount}"
                });
            }

            return groups;
        }
    }
}
