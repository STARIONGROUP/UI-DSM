// --------------------------------------------------------------------------------------------------------
// <copyright file="ElementBreakdownStructureViewViewModel.cs" company="RHEA System S.A.">
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
    using CDP4Common.EngineeringModelData;

    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;

    /// <summary>
    ///     Base view model for all breakdown Structure view for <see cref="ElementBase" />
    /// </summary>
    public abstract class ElementBreakdownStructureViewViewModel : BaseViewViewModel, IElementBreakdownStructureViewViewModel
    {
        /// <summary>
        ///     A collection of all available <see cref="ElementBaseRowViewModel" />
        /// </summary>
        protected readonly List<ElementBaseRowViewModel> AllAvailableRows = new();

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseViewViewModel" /> class.
        /// </summary>
        /// <param name="reviewItemService">The <see cref="IReviewItemService" /></param>
        protected ElementBreakdownStructureViewViewModel(IReviewItemService reviewItemService) : base(reviewItemService)
        {
        }

        /// <summary>
        ///     A collection of <see cref="ElementBaseRowViewModel" /> for the top element
        /// </summary>
        public List<ElementBaseRowViewModel> TopElement { get; private set; } = new();

        /// <summary>
        ///     Loads all children <see cref="ElementBaseRowViewModel" /> of the <see cref="ElementBaseRowViewModel" /> parent
        /// </summary>
        /// <param name="parent">The <see cref="ElementBaseRowViewModel" />parent</param>
        /// <returns>A collection of <see cref="ElementBaseRowViewModel" /> children</returns>
        public IEnumerable<ElementBaseRowViewModel> LoadChildren(ElementBaseRowViewModel parent)
        {
            return this.AllAvailableRows.Where(x => x.ContainerId == parent.ElementDefinitionId)
                .OrderBy(x => x.Name).ToList();
        }

        /// <summary>
        ///     Verifies if a <see cref="ElementBaseRowViewModel" /> has children
        /// </summary>
        /// <param name="rowData">The <see cref="ElementBaseRowViewModel" /></param>
        /// <returns>The assert</returns>
        public bool HasChildren(ElementBaseRowViewModel rowData)
        {
            return this.AllAvailableRows.Any(x => x.ContainerId == rowData.ElementDefinitionId);
        }
    }
}
