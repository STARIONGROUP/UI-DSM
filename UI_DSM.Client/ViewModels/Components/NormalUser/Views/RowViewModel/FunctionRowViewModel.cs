// --------------------------------------------------------------------------------------------------------
// <copyright file="FunctionRowViewModel.cs" company="RHEA System S.A.">
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

    using UI_DSM.Client.Extensions;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Row view model to display content for a <see cref="ElementBase" /> that are Function
    /// </summary>
    public class FunctionRowViewModel : ElementBaseRowViewModel
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FunctionRowViewModel" /> class.
        /// </summary>
        /// <param name="thing">The <see cref="ElementBase" /></param>
        /// <param name="reviewItem">The associated <see cref="HaveThingRowViewModel{TThing}.ReviewItem" /></param>
        public FunctionRowViewModel(ElementBase thing, ReviewItem reviewItem) : base(thing, reviewItem)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FunctionRowViewModel" /> class.
        /// </summary>
        public FunctionRowViewModel()
        {
        }

        /// <summary>
        ///     Gets the Id of the current <see cref="IHaveThingRowViewModel" />
        /// </summary>
        public override string Id => this.Thing.Name;

        /// <summary>
        ///     Gets the value of Technology of the linked product
        /// </summary>
        public List<(string, string)> LinkedTechnologyValues { get; private set; } = new();

        /// <summary>
        ///     Gets the value of Trl of the linked product
        /// </summary>
        public List<(string, int?)> LinkedTrlValues { get; private set; } = new();

        /// <summary>
        ///     Gets the value of Cost of the linked product
        /// </summary>
        public List<(string, string)> LinkedCostValues { get; private set; } = new();

        /// <summary>
        ///     The collection of <see cref="Requirement" /> that are satisfied by the function
        /// </summary>
        public IEnumerable<Requirement> SatisfiedRequirements => this.Thing.GetRelatedThings<Requirement>(ThingExtension.SatisfyCategoryName, ClassKind.Requirement);

        /// <summary>
        ///     The collection of products that implements the function
        /// </summary>
        public IEnumerable<ElementUsage> ProductsImplement => this.Thing.GetRelatedThings<ElementUsage>(ThingExtension.ImplementCategoryName, ClassKind.ElementUsage,
            ThingExtension.ProductCategoryName, false);

        /// <summary>
        ///     Updates the <see cref="ElementBaseRowViewModel.CurrentOption" /> property
        /// </summary>
        /// <param name="selectedOption">The new <see cref="Option" /></param>
        public override void UpdateOption(Option selectedOption)
        {
            base.UpdateOption(selectedOption);
            this.UpdateCostValues();
        }

        /// <summary>
        ///     Initializes this row view model properties
        /// </summary>
        protected override void InitializesProperties()
        {
            base.InitializesProperties();

            var linkedProducts = this.Thing.GetLinkedProducts();

            foreach (var linkedProduct in linkedProducts.OrderBy(x => x.Name))
            {
                if (linkedProduct.ShouldHaveTechnologyParameter())
                {
                    this.LinkedTechnologyValues.Add(linkedProduct.TryGetParameterValue("technology", null, null, out var technology)
                        ? (linkedProduct.Name, technology)
                        : (linkedProduct.Name, "-"));
                }

                if (linkedProduct.ShouldHaveTrlParameter())
                {
                    if (linkedProduct.TryGetParameterValue("trl", null, null, out var trl)
                        && int.TryParse(trl, out var trlParsed))
                    {
                        this.LinkedTrlValues.Add((linkedProduct.Name, trlParsed));
                    }
                    else
                    {
                        this.LinkedTrlValues.Add((linkedProduct.Name, null));
                    }
                }
            }
        }

        /// <summary>
        ///     Updates the collection of <see cref="LinkedCostValues" /> based on an option
        /// </summary>
        private void UpdateCostValues()
        {
            var linkedProducts = this.Thing.GetLinkedProducts();
            this.LinkedCostValues.Clear();

            foreach (var product in linkedProducts.OrderBy(x => x.Name))
            {
                this.LinkedCostValues.Add(product.TryGetParameterValue("cost", this.CurrentOption, null, out var cost)
                    ? (product.Name, cost)
                    : (product.Name, "-"));
            }
        }
    }
}
