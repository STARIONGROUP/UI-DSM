// --------------------------------------------------------------------------------------------------------
// <copyright file="RequirementBreakdownStructureViewRowViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel
{
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using UI_DSM.Client.Extensions;

    /// <summary>
    ///     Row view model to display content for a <see cref="Requirement" />
    /// </summary>
    public class RequirementBreakdownStructureViewRowViewModel
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RequirementBreakdownStructureViewRowViewModel" /> class.
        /// </summary>
        public RequirementBreakdownStructureViewRowViewModel(Requirement associatedRequirement)
        {
            this.AssociatedRequirement = associatedRequirement;
        }

        /// <summary>
        ///     The associated <see cref="Requirement" />
        /// </summary>
        public Requirement AssociatedRequirement { get; }

        /// <summary>
        ///     The ShortName of the <see cref="Requirement" />
        /// </summary>
        public string Id => this.AssociatedRequirement.ShortName;

        /// <summary>
        ///     The Name of the <see cref="Requirement" />
        /// </summary>
        public string Name => this.AssociatedRequirement.Name;

        /// <summary>
        ///     The Name of the <see cref="Requirement" /> owner
        /// </summary>
        public string Owner => this.AssociatedRequirement.GetOwnerShortName();

        /// <summary>
        ///     The <see cref="Definition" /> content of the <see cref="Requirement" />
        /// </summary>
        public string Definition => this.AssociatedRequirement.GetFirstDefinition();

        /// <summary>
        ///     The collection of <see cref="Category" /> name
        /// </summary>
        public IEnumerable<string> Categories => this.AssociatedRequirement.GetCategories();

        /// <summary>
        ///     The collection of <see cref="ParametricConstraint" /> expression
        /// </summary>
        public IEnumerable<string> ParametricConstraints => this.AssociatedRequirement.GetParametricConstraints();

        /// <summary>
        ///     The Justificaiton of the <see cref="Requirement" />
        /// </summary>
        public string Justification => this.AssociatedRequirement.GetSimpleParameterValue("justification");

        /// <summary>
        ///     The <see cref="Requirement" /> Type
        /// </summary>
        public string RequirementType => this.AssociatedRequirement.GetSimpleParameterValue("requirement type");

        /// <summary>
        ///     The Verification Method of the <see cref="Requirement" />
        /// </summary>
        public string VerificationMethod => this.AssociatedRequirement.GetSimpleParameterValue("verification method");

        /// <summary>
        ///     The Verification Stage of the <see cref="Requirement" />
        /// </summary>
        public string VerificationStage => this.AssociatedRequirement.GetSimpleParameterValue("verification stage");

        /// <summary>
        ///     The names of the <see cref="Thing" />s from which the <see cref="Requirement" /> derives
        /// </summary>
        public IEnumerable<string> DerivesFrom => this.AssociatedRequirement.GetRelatedThingsName("derives", ClassKind.Requirement, false);

        /// <summary>
        ///     The names of the <see cref="Thing" />s to which the <see cref="Requirement" /> derives
        /// </summary>
        public IEnumerable<string> DerivesTo => this.AssociatedRequirement.GetRelatedThingsName("derives", ClassKind.Requirement);

        /// <summary>
        ///     The names of functions that satisfies the <see cref="Requirement" />
        /// </summary>
        public IEnumerable<string> SatisfyByFunction => this.AssociatedRequirement.GetRelatedThingsName("satisfy", ClassKind.ElementDefinition, "function", false, false);

        /// <summary>
        ///     The names of products that satisfies the <see cref="Requirement" />
        /// </summary>
        public IEnumerable<string> SatisfyByProduct => this.AssociatedRequirement.GetRelatedThingsName("satisfy", ClassKind.ElementDefinition, "product", false, false);
    }
}
