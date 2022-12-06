// --------------------------------------------------------------------------------------------------------
// <copyright file="IHaveTraceabilityTableViewModel.cs" company="RHEA System S.A.">
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

    using UI_DSM.Client.Model;
    using UI_DSM.Client.ViewModels.App.Filter;

    /// <summary>
    ///     Interface definition for <see cref="HaveTraceabilityTableViewModel" />
    /// </summary>
    public interface IHaveTraceabilityTableViewModel : IBaseViewViewModel, IDisposable
    {
        /// <summary>
        ///     The <see cref="IFilterViewModel" /> for rows
        /// </summary>
        IFilterViewModel RowsFilterViewModel { get; }

        /// <summary>
        ///     The <see cref="IFilterViewModel" /> for columns
        /// </summary>
        IFilterViewModel ColumnsFilterViewModel { get; }

        /// <summary>
        ///     The <see cref="ITraceabilityTableViewModel" />
        /// </summary>
        ITraceabilityTableViewModel TraceabilityTableViewModel { get; }

        /// <summary>
        ///     Apply a filtering on rows
        /// </summary>
        /// <param name="selectedFilters">The selected filters</param>
        void FilterRows(Dictionary<ClassKind, List<FilterRow>> selectedFilters);

        /// <summary>
        ///     Apply a filtering on columns
        /// </summary>
        /// <param name="selectedFilters">The selected filters</param>
        void FilterColumns(Dictionary<ClassKind, List<FilterRow>> selectedFilters);
    }
}
