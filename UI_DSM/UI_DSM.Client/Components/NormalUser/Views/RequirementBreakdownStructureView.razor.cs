// --------------------------------------------------------------------------------------------------------
// <copyright file="RequirementBreakdownStructureView.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.NormalUser.Views
{
    using System.Reactive.Linq;

    using CDP4Common.CommonData;

    using DevExpress.Blazor;

    using ReactiveUI;

    using UI_DSM.Client.Components.App.Filter;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Component for the <see cref="View.RequirementBreakdownStructureView" />
    /// </summary>
    public partial class RequirementBreakdownStructureView : GenericBaseView<IRequirementBreakdownStructureViewViewModel>, IDisposable
    {
        /// <summary>
        /// A collection of <see cref="IDisposable"/>
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     The <see cref="DxGrid" />
        /// </summary>
        protected DxGrid DxGrid { get; set; }

        /// <summary>
        ///     Reference to the <see cref="Filter" />
        /// </summary>
        public Filter RowFiltering { get; set; }

        /// <summary>
        ///     Initialize the correspondant ViewModel for this component
        /// </summary>
        /// <param name="things">The collection of <see cref="Thing" /></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task InitializeViewModel(IEnumerable<Thing> things, Guid projectId, Guid reviewId)
        {
            await base.InitializeViewModel(things, projectId, reviewId);

            this.RowFiltering.ViewModel.InitializeProperties(this.ViewModel.AvailableRowFilters);

            this.disposables.Add(this.WhenAnyValue(x => x.RowFiltering.ViewModel.IsFilterVisible)
                .Where(x => !x)
                .Subscribe(_ => this.InvokeAsync(this.OnRowFilteringClose)));
        }

        /// <summary>
        ///     Apply the filtering on rows
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private Task OnRowFilteringClose()
        {
            this.ViewModel.FilterRequirementRows(this.RowFiltering.ViewModel.GetSelectedFilters());
            return this.InvokeAsync(this.StateHasChanged);
        }

        /// <summary>
        ///     Handle the selection of the column chooser
        /// </summary>
        protected void OnClick()
        {
            this.DxGrid.ShowColumnChooser("#column-chooser");
        }

        /// <summary>
        ///     Checks if the current <see cref="RequirementRowViewModel" /> has a <see cref="Comment" />
        /// </summary>
        /// <param name="context">The <see cref="GridColumnCellDisplayTemplateContext" /></param>
        /// <returns>The result of the check</returns>
        protected static bool HasComment(GridColumnCellDisplayTemplateContext context)
        {
            return context.DataItem is RequirementRowViewModel { ReviewItem: { } } row &&
                   row.ReviewItem.Annotations.OfType<Comment>().Any();
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.disposables.ForEach(x => x.Dispose());
        }
    }
}
