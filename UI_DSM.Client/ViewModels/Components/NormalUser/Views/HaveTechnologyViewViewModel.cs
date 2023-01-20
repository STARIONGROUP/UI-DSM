// --------------------------------------------------------------------------------------------------------
// <copyright file="HaveTechnologyViewViewModel.cs" company="RHEA System S.A.">
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
    using ReactiveUI;

    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.ViewModels.App.Filter;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;

    /// <summary>
    ///     <see cref="HaveTraceabilityTableViewModel" /> for view that has a Technology View
    /// </summary>
    public abstract class HaveTechnologyViewViewModel : HaveTraceabilityTableViewModel, IHaveTechnologyViewViewModel
    {
        /// <summary>
        ///     Backing field for <see cref="IsOnTechnologyView" />
        /// </summary>
        private bool isOnTechnologyView;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseViewViewModel" /> class.
        /// </summary>
        /// <param name="reviewItemService">The <see cref="IReviewItemService" /></param>
        /// <param name="rowsFilter">The <see cref="IFilterViewModel" /> for rows</param>
        /// <param name="columnsFilter">The <see cref="IFilterViewModel" /> for columns</param>
        protected HaveTechnologyViewViewModel(IReviewItemService reviewItemService, IFilterViewModel rowsFilter, IFilterViewModel columnsFilter)
            : base(reviewItemService, rowsFilter, columnsFilter)
        {
        }

        /// <summary>
        ///     Value indicating that the current view should display Technology
        /// </summary>
        public bool IsOnTechnologyView
        {
            get => this.isOnTechnologyView;
            set => this.RaiseAndSetIfChanged(ref this.isOnTechnologyView, value);
        }

        /// <summary>
        ///     Perfoms the switch between view
        /// </summary>
        public void OnTechnologyViewChange()
        {
            foreach (var row in this.TraceabilityTableViewModel.Rows.OfType<ProductRowViewModel>())
            {
                row.ComputeId(this.IsOnTechnologyView);
            }
        }

        /// <summary>
        ///     Verifies that a <see cref="IHaveThingRowViewModel" /> is valid
        /// </summary>
        /// <param name="row">A <see cref="IHaveThingRowViewModel" /></param>
        /// <returns>The result of the verification</returns>
        protected override bool IsValidRow(IHaveThingRowViewModel row)
        {
            return !this.IsOnTechnologyView || ((ProductRowViewModel)row).HasValidTechnology;
        }
    }
}
