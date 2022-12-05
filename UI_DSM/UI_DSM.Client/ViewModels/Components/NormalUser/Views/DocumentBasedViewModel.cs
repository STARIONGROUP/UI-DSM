// --------------------------------------------------------------------------------------------------------
// <copyright file="DocumentBasedViewModel.cs" company="RHEA System S.A.">
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

    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="Client.Components.NormalUser.Views.DocumentBased" /> component
    /// </summary>
    public class DocumentBasedViewModel : BaseViewViewModel, IDocumentBasedViewModel
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseViewViewModel" /> class.
        /// </summary>
        /// <param name="reviewItemService">The <see cref="IReviewItemService" /></param>
        public DocumentBasedViewModel(IReviewItemService reviewItemService) : base(reviewItemService)
        {
        }

        /// <summary>
        ///     A collection of <see cref="HyperLinkRowViewModel" />
        /// </summary>
        public IEnumerable<HyperLinkRowViewModel> Rows { get; private set; } = new List<HyperLinkRowViewModel>();

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

            var hyperlinks= this.Things.OfType<ElementDefinition>()
                .Where(x => x.HyperLink.Any())
                .SelectMany(x => x.HyperLink)
                .ToList();

            hyperlinks.AddRange(this.Things.OfType<Requirement>()
                .Where(x => x.HyperLink.Any())
                .SelectMany(x => x.HyperLink));

            var reviewItems = await this.ReviewItemService.GetReviewItemsForThings(this.ProjectId, this.ReviewId,
                hyperlinks.Select(x => x.Iid));

            this.Rows = new List<HyperLinkRowViewModel>(hyperlinks.Select(x =>
                new HyperLinkRowViewModel(x, reviewItems.FirstOrDefault(ri => ri.ThingId == x.Iid))));
        }

        /// <summary>
        ///     Tries to set the <see cref="IBaseViewViewModel.SelectedElement" /> to the previous selected item
        /// </summary>
        /// <param name="selectedItem">The previously selectedItem</param>
        public override void TrySetSelectedItem(object selectedItem)
        {
            if (selectedItem is HyperLinkRowViewModel hyperLinkRow)
            {
                this.SelectedElement = this.Rows.FirstOrDefault(x => x.ThingId == hyperLinkRow.ThingId);
            }
        }
    }
}
