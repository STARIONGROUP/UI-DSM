// --------------------------------------------------------------------------------------------------------
// <copyright file="ProductRowViewModel.cs" company="RHEA System S.A.">
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
    using CDP4Common.EngineeringModelData;

    using UI_DSM.Client.Extensions;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Row view model to display content for a <see cref="ElementDefinition" /> that are Product
    /// </summary>
    public class ProductRowViewModel : HaveThingRowViewModel<ElementDefinition>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="HaveThingRowViewModel{TThing}" /> class.
        /// </summary>
        /// <param name="thing">The <see cref="ElementDefinition" /></param>
        /// <param name="reviewItem">The associated <see cref="HaveThingRowViewModel{TThing}.ReviewItem" /></param>
        public ProductRowViewModel(ElementDefinition thing, ReviewItem reviewItem) : base(thing, reviewItem)
        {
            this.InitializesProperties();
        }

        /// <summary>
        ///     Gets the Id of the current <see cref="IHaveThingRowViewModel" />
        /// </summary>
        public override string Id => this.ComputedId;

        /// <summary>
        ///     The computed Id
        /// </summary>
        public string ComputedId { get; private set; }

        /// <summary>
        ///     Gets the name of the <see cref="ElementDefinition" />
        /// </summary>
        public string Name => this.Thing.Name;

        /// <summary>
        ///     Gets the shortname of the <see cref="ElementDefinition" />
        /// </summary>
        public string ShortName => this.Thing.ShortName;

        /// <summary>
        ///     Gets the owner of the <see cref="ElementDefinition" />
        /// </summary>
        public string Owner => this.Thing.GetOwnerShortName();

        /// <summary>
        ///     The value of the technology parameter
        /// </summary>
        public string TechnologyValue { get; private set; }

        /// <summary>
        ///     If the <see cref="ProductRowViewModel" /> has a valid technology parameter
        /// </summary>
        public bool HasValidTechnology { get; private set; }

        /// <summary>
        ///     Compute the <see cref="ComputedId" />
        /// </summary>
        /// <param name="shouldShowTechnology">If should include the technology</param>
        public void ComputeId(bool shouldShowTechnology)
        {
            if (shouldShowTechnology && this.Thing.ShouldHaveTechnologyParameter())
            {
                this.ComputedId = $"{this.Thing.Name} ({this.TechnologyValue})";
            }
            else
            {
                this.ComputedId = this.Thing.Name;
            }
        }

        /// <summary>
        ///     Initializes this row view model properties
        /// </summary>
        private void InitializesProperties()
        {
            if (!this.Thing.ShouldHaveTechnologyParameter())
            {
                this.HasValidTechnology = true;
            }
            else
            {
                this.HasValidTechnology = this.Thing.TryGetParameterValue("technology", null, null, out var retrievedValue);
                this.TechnologyValue = retrievedValue;
                this.HasValidTechnology &= this.TechnologyValue != "-";
            }

            this.ComputeId(false);
        }
    }
}
