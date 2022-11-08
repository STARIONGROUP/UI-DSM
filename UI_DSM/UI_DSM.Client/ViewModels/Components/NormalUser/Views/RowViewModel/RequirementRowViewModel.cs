// --------------------------------------------------------------------------------------------------------
// <copyright file="RequirementRowViewModel.cs" company="RHEA System S.A.">
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
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Row view model to display content for a <see cref="Requirement" />
    /// </summary>
    public class RequirementRowViewModel : HaveThingRowViewModel<Requirement>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RequirementRowViewModel" /> class.
        /// </summary>
        /// <param name="thing">The <see cref="Requirement" /></param>
        /// <param name="reviewItem">The <see cref="ReviewItem" /></param>
        public RequirementRowViewModel(Requirement thing, ReviewItem reviewItem) : base(thing, reviewItem)
        {
        }

        /// <summary>
        ///     The ShortName of the <see cref="Requirement" />
        /// </summary>
        public override string Id => this.Thing.ShortName;

        /// <summary>
        ///     The Name of the <see cref="Requirement" />
        /// </summary>
        public string Name => this.Thing.Name;

        /// <summary>
        ///     The Name of the <see cref="Requirement" /> owner
        /// </summary>
        public string Owner => this.Thing.GetOwnerShortName();

        /// <summary>
        ///     The <see cref="Definition" /> content of the <see cref="Requirement" />
        /// </summary>
        public string Definition => this.Thing.GetFirstDefinition();

        /// <summary>
        ///     The collection of <see cref="Category" /> name
        /// </summary>
        public IEnumerable<string> Categories => this.Thing.GetCategories();

        /// <summary>
        ///     The collection of <see cref="ParametricConstraint" /> expression
        /// </summary>
        public IEnumerable<string> ParametricConstraints => this.Thing.GetParametricConstraints();

        /// <summary>
        ///     The Justificaiton of the <see cref="Requirement" />
        /// </summary>
        public string Justification => this.Thing.GetSimpleParameterValue("justification");

        /// <summary>
        ///     The <see cref="Requirement" /> Type
        /// </summary>
        public string RequirementType => this.Thing.GetSimpleParameterValue("requirement type");

        /// <summary>
        ///     The Verification Method of the <see cref="Requirement" />
        /// </summary>
        public string VerificationMethod => this.Thing.GetSimpleParameterValue("verification method");

        /// <summary>
        ///     The Verification Stage of the <see cref="Requirement" />
        /// </summary>
        public string VerificationStage => this.Thing.GetSimpleParameterValue("verification stage");

        /// <summary>
        ///     The names of the <see cref="Thing" />s from which the <see cref="Requirement" /> derives
        /// </summary>
        public IEnumerable<string> DerivesFrom => this.Thing.GetRelatedThingsName("derives", ClassKind.Requirement, false);

        /// <summary>
        ///     The names of the <see cref="Thing" />s to which the <see cref="Requirement" /> derives
        /// </summary>
        public IEnumerable<string> DerivesTo => this.Thing.GetRelatedThingsName("derives", ClassKind.Requirement);

        /// <summary>
        ///     The names of functions that satisfies the <see cref="Requirement" />
        /// </summary>
        public IEnumerable<string> SatisfyByFunction => this.Thing.GetRelatedThingsName("satisfy", ClassKind.ElementDefinition, "function", false, false);

        /// <summary>
        ///     The names of products that satisfies the <see cref="Requirement" />
        /// </summary>
        public IEnumerable<string> SatisfyByProduct => this.Thing.GetRelatedThingsName("satisfy", ClassKind.ElementDefinition, "product", false, false);

        /// <summary>
        ///     The name of the <see cref="RequirementsSpecification" />
        /// </summary>
        public string Specification => this.Thing.GetSpecificationName();

        /// <summary>
        ///     The name of the <see cref="RequirementsGroup" />
        /// </summary>
        public string Group => this.Thing.Group?.ShortName;

        /// <summary>
        ///     A collection of name for <see cref="Requirement" />s that trace this <see cref="Requirement" />
        /// </summary>
        public IEnumerable<string> Traces => this.Thing.GetRelatedThingsName("trace", ClassKind.Requirement);
    }
}
