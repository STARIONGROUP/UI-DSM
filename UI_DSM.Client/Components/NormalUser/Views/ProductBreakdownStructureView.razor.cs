// --------------------------------------------------------------------------------------------------------
// <copyright file="ProductBreakdownStructureView.razor.cs" company="RHEA System S.A.">
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
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Enumerator;

    /// <summary>
    ///     Component for the <see cref="View.ProductBreakdownStructureView" />
    /// </summary>
    public partial class ProductBreakdownStructureView : ElementBreakdownStructureView<IProductBreakdownStructureViewViewModel>, IReusableView
    {
        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// </summary>
        protected override void OnInitialized()
        {
            this.IsLoading = true;
            base.OnInitialized();
        }

        /// <summary>
        ///     Tries to copy components from another <see cref="BaseView" />
        /// </summary>
        /// <param name="otherView">The other <see cref="BaseView" /></param>
        /// <returns>A <see cref="Task" /> with the value indicating if it could copy components</returns>
        public async Task<bool> CopyComponents(BaseView otherView)
        {
            if (otherView is not TrlView trlView)
            {
                return false;
            }

            this.ViewModel = trlView.ViewModel;
            this.IsLoading = false;
            await Task.CompletedTask;
            return true;
        }

        /// <summary>
        ///     Hides columns on start
        /// </summary>
        protected override void HideColumnsAtStart()
        {
            var trlColumn = this.Grid.ColumnsCollection.FirstOrDefault(x => x.Property == nameof(ProductRowViewModel.TrlValue));
            var technologyColumn = this.Grid.ColumnsCollection.FirstOrDefault(x => x.Property == nameof(ProductRowViewModel.TechnologyValue));
            var costColumn = this.Grid.ColumnsCollection.FirstOrDefault(x => x.Property == nameof(ProductRowViewModel.CostValue));

            if (trlColumn != null && !this.ViewModel.AdditionnalColumnsVisibleAtStart.Any(x => trlColumn.Property.Contains(x, StringComparison.InvariantCultureIgnoreCase)))
            {
                this.ColumnChooser.OnChangeValue(trlColumn);
            }

            if (technologyColumn != null && !this.ViewModel.AdditionnalColumnsVisibleAtStart.Any(x => technologyColumn.Property.Contains(x, StringComparison.InvariantCultureIgnoreCase)))
            {
                this.ColumnChooser.OnChangeValue(technologyColumn);
            }

            if (costColumn != null && !this.ViewModel.AdditionnalColumnsVisibleAtStart.Any(x => costColumn.Property.Contains(x, StringComparison.InvariantCultureIgnoreCase)))
            {
                this.ColumnChooser.OnChangeValue(costColumn);
            }
        }
    }
}
