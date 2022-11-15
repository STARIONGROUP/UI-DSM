// --------------------------------------------------------------------------------------------------------
// <copyright file="ProductBreakdownStructureViewViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.NormalUser.Views
{
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using UI_DSM.Client.Extensions;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="Client.Components.NormalUser.Views.ProductBreakdownStructureView" /> component
    /// </summary>
    public class ProductBreakdownStructureViewViewModel : ElementBreakdownStructureViewViewModel, IProductBreakdownStructureViewViewModel
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ProductBreakdownStructureViewViewModel" /> class.
        /// </summary>
        /// <param name="reviewItemService">The <see cref="IReviewItemService" /></param>
        public ProductBreakdownStructureViewViewModel(IReviewItemService reviewItemService) : base(reviewItemService)
        {
        }

        /// <summary>
        ///     Initialize this view model properties
        /// </summary>
        /// <param name="things">A collection of <see cref="Thing" /></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task InitializeProperties(IEnumerable<Thing> things, Guid projectId, Guid reviewId)
        {
            await base.InitializeProperties(things, projectId, reviewId);

            var topElement = this.Things.OfType<Iteration>()
                .Single()
                .TopElement;

            if (topElement == null)
            {
                return;
            }

            this.TopElement.Clear();
            this.AllAvailableRows.Clear();

            var elementUsages = this.Things.OfType<ElementUsage>()
                .Where(x => !x.IsFunction())
                .ToList();

            var filteredThings = new List<ElementBase> { topElement };
            filteredThings.AddRange(elementUsages);

            var reviewItems = await this.ReviewItemService.GetReviewItemsForThings(this.ProjectId, this.ReviewId,
                filteredThings.Select(x => x.Iid));

            this.TopElement.Add(topElement.IsProduct()
                ? new ProductRowViewModel(topElement, reviewItems.FirstOrDefault(x => x.ThingId == topElement.Iid))
                : new ElementBaseRowViewModel(topElement, reviewItems.FirstOrDefault(x => x.ThingId == topElement.Iid)));

            this.AllAvailableRows.AddRange(elementUsages.Where(x => x.IsProduct())
                .Select(x => new ProductRowViewModel(x, reviewItems.FirstOrDefault(ri => ri.ThingId == x.Iid))));

            this.AllAvailableRows.AddRange(elementUsages.Where(x => !x.IsProduct())
                .Select(x => new ElementBaseRowViewModel(x, reviewItems.FirstOrDefault(ri => ri.ThingId == x.Iid))));
        }
    }
}
