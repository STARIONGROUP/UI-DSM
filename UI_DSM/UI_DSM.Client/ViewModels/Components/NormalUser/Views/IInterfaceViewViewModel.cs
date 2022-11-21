// --------------------------------------------------------------------------------------------------------
// <copyright file="IInterfaceViewViewModel.cs" company="RHEA System S.A.">
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
    using UI_DSM.Client.ViewModels.App.ConnectionVisibilitySelector;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;

    /// <summary>
    ///     Interface definition for <see cref="InterfaceViewViewModel" />
    /// </summary>
    public interface IInterfaceViewViewModel : IBaseViewViewModel
    {
        /// <summary>
        ///     A collection of available <see cref="FilterModel" /> for rows
        /// </summary>
        List<FilterModel> AvailableRowFilters { get; }

        /// <summary>
        ///     A collection <see cref="IBelongsToInterfaceView" />
        /// </summary>
        IEnumerable<IBelongsToInterfaceView> Data { get; }

        /// <summary>
        ///     A collection of <see cref="ProductRowViewModel" />
        /// </summary>
        List<ProductRowViewModel> Products { get; }

        /// <summary>
        ///     The <see cref="IConnectionVisibilitySelectorViewModel" /> for products
        /// </summary>
        IConnectionVisibilitySelectorViewModel ProductVisibilityState { get; set; }

        /// <summary>
        ///     The <see cref="IConnectionVisibilitySelectorViewModel" /> for ports
        /// </summary>
        IConnectionVisibilitySelectorViewModel PortVisibilityState { get; set; }

        /// <summary>
        ///     A collection of filtered <see cref="InterfaceRowViewModel" />
        /// </summary>
        List<InterfaceRowViewModel> Interfaces { get; }

        /// <summary>
        ///     Value asserting if products has to been shown
        /// </summary>
        bool ShouldShowProducts { get; }

        /// <summary>
        ///     Filters current rows
        /// </summary>
        /// <param name="selectedFilters">The selected filters</param>
        void FilterRows(IReadOnlyDictionary<ClassKind, List<FilterRow>> selectedFilters);

        /// <summary>
        ///     Handle the change of visibility
        /// </summary>
        void OnVisibilityStateChanged();

        /// <summary>
        ///     Asserts that a <see cref="ProductRowViewModel" /> has <see cref="PortRowViewModel" /> children
        /// </summary>
        /// <param name="productRow">The <see cref="ProductRowViewModel" /></param>
        /// <returns>True if contains any visible port</returns>
        bool HasChildren(ProductRowViewModel productRow);

        /// <summary>
        ///     Asserts that a <see cref="PortRowViewModel" /> has <see cref="InterfaceRowViewModel" /> children
        /// </summary>
        /// <param name="portRow">The <see cref="PortRowViewModel" /></param>
        /// <returns>True if contains any visible interfaces</returns>
        bool HasChildren(PortRowViewModel portRow);

        /// <summary>
        ///     Loads all <see cref="PortRowViewModel" /> children rows of a <see cref="ProductRowViewModel" />
        /// </summary>
        /// <param name="productRow">The <see cref="ProductRowViewModel" /></param>
        /// <returns>A collection of <see cref="PortRowViewModel" /></returns>
        IEnumerable<PortRowViewModel> LoadChildren(ProductRowViewModel productRow);
        
        /// <summary>
        ///     Loads all <see cref="InterfaceRowViewModel" /> children rows of a <see cref="PortRowViewModel" />
        /// </summary>
        /// <param name="portRow">The <see cref="PortRowViewModel" /></param>
        /// <returns>A collection of <see cref="InterfaceRowViewModel" /></returns>
        IEnumerable<InterfaceRowViewModel> LoadChildren(PortRowViewModel portRow);

        /// <summary>
        ///     Modifies the datasource to display based on the visibility of products
        /// </summary>
        /// <param name="visibility">The new visibility</param>
        void SetProductsVisibility(bool visibility);
    }
}
