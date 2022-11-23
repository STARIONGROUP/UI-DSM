// --------------------------------------------------------------------------------------------------------
// <copyright file="IRequirementBreakdownStructureViewViewModel.cs" company="RHEA System S.A.">
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
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;

    /// <summary>
    ///     Interface definition for <see cref="RequirementBreakdownStructureViewViewModel" />
    /// </summary>
    public interface IRequirementBreakdownStructureViewViewModel: IBaseViewViewModel
    {
        /// <summary>
        ///     A collection of <see cref="RequirementRowViewModel" />
        /// </summary>
        IEnumerable<RequirementRowViewModel> Rows { get; }

        /// <summary>
        ///     The <see cref="IFilterViewModel" />
        /// </summary>
        IFilterViewModel FilterViewModel { get; set; }

        /// <summary>
        ///     Filters current rows
        /// </summary>
        /// <param name="selectedFilters">The selected filters</param>
        void FilterRequirementRows(IReadOnlyDictionary<ClassKind, List<FilterRow>> selectedFilters);
    }
}
