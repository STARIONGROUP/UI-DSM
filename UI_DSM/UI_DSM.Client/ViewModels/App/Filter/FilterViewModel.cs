// --------------------------------------------------------------------------------------------------------
// <copyright file="FilterViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.App.Filter
{
    using CDP4Common.CommonData;

    using ReactiveUI;

    using UI_DSM.Client.Model;

    /// <summary>
    ///     View model for the <see cref="Client.Components.App.Filter.Filter" /> component
    /// </summary>
    public class FilterViewModel : ReactiveObject, IFilterViewModel
    {
        /// <summary>
        ///     Backing field for <see cref="IsFilterVisible" />
        /// </summary>
        private bool isFilterVisible;

        /// <summary>
        ///     Backing field for <see cref="SelectedFilterModel" />
        /// </summary>
        private FilterModel selectedFilterModel;

        /// <summary>
        ///     A collection of available filters
        /// </summary>
        public List<FilterModel> AvailableFilters { get; private set; } = new();

        /// <summary>
        ///     A <see cref="Dictionary{TKey,TValue}" /> of selected filters
        /// </summary>
        public Dictionary<ClassKind, List<FilterRow>> SelectedFilters { get; private set; } = new();

        /// <summary>
        ///     The currently selected <see cref="FilterModel" />
        /// </summary>
        public FilterModel SelectedFilterModel
        {
            get => this.selectedFilterModel;
            set => this.RaiseAndSetIfChanged(ref this.selectedFilterModel, value);
        }

        /// <summary>
        ///     A value indicating if the component is visible or not
        /// </summary>
        public bool IsFilterVisible
        {
            get => this.isFilterVisible;
            set => this.RaiseAndSetIfChanged(ref this.isFilterVisible, value);
        }

        /// <summary>
        ///     Initializes this view model properties
        /// </summary>
        public void InitializeProperties(List<FilterModel> filters)
        {
            this.AvailableFilters = filters;
            this.SelectedFilters.Clear();

            foreach (var filter in this.AvailableFilters)
            {
                this.SelectedFilters[filter.ClassKind] = new List<FilterRow>();

                foreach (var definedThing in filter.Values)
                {
                    this.SelectedFilters[filter.ClassKind].Add(new FilterRow(definedThing));
                }
            }

            this.SelectedFilterModel = this.AvailableFilters.First();
        }

        /// <summary>
        ///     Selects or deselects all <see cref="FilterRow" />
        /// </summary>
        /// <param name="isSelected">The state to apply</param>
        public void SelectDeselectAll(bool isSelected)
        {
            foreach (var row in this.SelectedFilters[this.SelectedFilterModel.ClassKind])
            {
                row.IsSelected = isSelected;
            }
        }

        /// <summary>
        ///     Modifies the filter value
        /// </summary>
        /// <param name="row">The <see cref="FilterRow" /></param>
        public void OnChangeValue(FilterRow row)
        {
            var filterRow = this.SelectedFilters[this.SelectedFilterModel.ClassKind].FirstOrDefault(x => x == row);

            if (filterRow != null)
            {
                filterRow.IsSelected = !filterRow.IsSelected;
            }
        }

        /// <summary>
        ///     Gets the <see cref="Dictionary{TKey,TValue}"/> of selected filters
        /// </summary>
        public Dictionary<ClassKind, List<FilterRow>> GetSelectedFilters()
        {
            var selectedValues = new Dictionary<ClassKind, List<FilterRow>>();

            foreach (var classKind in this.SelectedFilters.Keys)
            {
                selectedValues[classKind] = this.SelectedFilters[classKind].Where(x => x.IsSelected).ToList();
            }

            return selectedValues;
        }
    }
}
