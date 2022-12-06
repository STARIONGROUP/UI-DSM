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
    using UI_DSM.Shared.Extensions;
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
            this.InitializesProperties();
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
        ///     The collection of <see cref="Category" /> separated by a ','
        /// </summary>
        public string CommaCategories { get; private set; }

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
        ///     The collection of the names of the <see cref="Thing" /> from which the <see cref="Requirement" /> derives separated by a ','
        /// </summary>
        public string CommaDerivesFrom { get; private set; }

        /// <summary>
        ///     The names of the <see cref="Thing" />s to which the <see cref="Requirement" /> derives
        /// </summary>
        public IEnumerable<string> DerivesTo => this.Thing.GetRelatedThingsName("derives", ClassKind.Requirement);

        /// <summary>
        ///     The collection of the names of the <see cref="Thing" /> to which the <see cref="Requirement" /> derives separated by a ','
        /// </summary>
        public string CommaDerivesTo { get; private set; }

        /// <summary>
        ///     The names of functions that satisfies the <see cref="Requirement" />
        /// </summary>
        public IEnumerable<string> SatisfyByFunction => this.Thing.GetRelatedThingsName("satisfy", ClassKind.ElementDefinition, "function", false, false);

        /// <summary>
        ///     The collection of the name of functions that satisfies the <see cref="Requirement" /> separated by a ','
        /// </summary>
        public string CommaSatisfyByFunction { get; private set; }

        /// <summary>
        ///     The names of products that satisfies the <see cref="Requirement" />
        /// </summary>
        public IEnumerable<string> SatisfyByProduct => this.Thing.GetRelatedThingsName("satisfy", ClassKind.ElementDefinition, "product", false, false);

        /// <summary>
        ///     The collection of the name of products that satisfies the <see cref="Requirement" /> separated by a ','
        /// </summary>
        public string CommaSatisfyByProduct { get; private set; }

        /// <summary>
        ///     The name of the <see cref="RequirementsSpecification" />
        /// </summary>
        public string Specification => this.Thing.GetSpecificationName();

        /// <summary>
        ///     The name of the <see cref="RequirementsGroup" />
        /// </summary>
        public string Group => this.Thing.Group?.Name;

        /// <summary>
        ///     A collection of name for <see cref="Requirement" />s that trace this <see cref="Requirement" />
        /// </summary>
        public IEnumerable<string> Traces => this.Thing.GetRelatedThingsName("trace", ClassKind.Requirement);

        /// <summary>
        ///     Initializes this row view model properties
        /// </summary>
        private void InitializesProperties()
        {
            this.CommaCategories = this.Categories.AsCommaSeparated();
            this.CommaDerivesFrom = this.DerivesFrom.AsCommaSeparated();
            this.CommaDerivesTo = this.DerivesTo.AsCommaSeparated();
            this.CommaSatisfyByFunction = this.SatisfyByFunction.AsCommaSeparated();
            this.CommaSatisfyByProduct = this.SatisfyByProduct.AsCommaSeparated();
        }
    }
}
