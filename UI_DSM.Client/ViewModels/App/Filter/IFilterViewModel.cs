// --------------------------------------------------------------------------------------------------------
// <copyright file="IFilterViewModel.cs" company="RHEA System S.A.">
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

    using UI_DSM.Client.Model;

    /// <summary>
    ///     Interface definition for <see cref="FilterViewModel" />
    /// </summary>
    public interface IFilterViewModel
    {
        /// <summary>
        ///     A collection of available filters
        /// </summary>
        List<FilterModel> AvailableFilters { get; }

        /// <summary>
        ///     A <see cref="Dictionary{TKey,TValue}" /> of selected filters
        /// </summary>
        Dictionary<ClassKind, List<FilterRow>> SelectedFilters { get; }

        /// <summary>
        ///     The currently selected <see cref="FilterModel" />
        /// </summary>
        FilterModel SelectedFilterModel { get; set; }

        /// <summary>
        ///     A value indicating if the component is visible or not
        /// </summary>
        bool IsFilterVisible { get; set; }

        /// <summary>
        ///     Initializes this view model properties
        /// </summary>
        void InitializeProperties(List<FilterModel> filters);

        /// <summary>
        ///     Selects or deselects all <see cref="FilterRow" />
        /// </summary>
        /// <param name="isSelected">The state to apply</param>
        void SelectDeselectAll(bool isSelected);

        /// <summary>
        ///     Modifies the filter value
        /// </summary>
        /// <param name="row">The <see cref="FilterRow" /></param>
        void OnChangeValue(FilterRow row);

        /// <summary>
        ///     Gets the <see cref="Dictionary{TKey,TValue}" /> of selected filters
        /// </summary>
        Dictionary<ClassKind, List<FilterRow>> GetSelectedFilters();

        /// <summary>
        ///     Verifies that all filters are selected or not
        /// </summary>
        /// <returns>True if all are selected, false if all deselected, null otherwise</returns>
        bool? AreAllSelected();
    }
}
