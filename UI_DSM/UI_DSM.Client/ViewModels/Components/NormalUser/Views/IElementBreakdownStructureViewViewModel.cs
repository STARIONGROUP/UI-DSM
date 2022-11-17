// --------------------------------------------------------------------------------------------------------
// <copyright file="IElementBreakdownStructureViewViewModel.cs" company="RHEA System S.A.">
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
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;

    /// <summary>
    ///     Interface definition for <see cref="ElementBreakdownStructureViewViewModel" />
    /// </summary>
    public interface IElementBreakdownStructureViewViewModel: IBaseViewViewModel
    {
        /// <summary>
        ///     A collection of <see cref="ElementBaseRowViewModel" /> for the top element
        /// </summary>
        List<ElementBaseRowViewModel> TopElement { get; }

        /// <summary>
        ///     Loads all children <see cref="ElementBaseRowViewModel" /> of the <see cref="ElementBaseRowViewModel" /> parent
        /// </summary>
        /// <param name="parent">The <see cref="ElementBaseRowViewModel" />parent</param>
        /// <returns>A collection of <see cref="ElementBaseRowViewModel" /> children</returns>
        IEnumerable<ElementBaseRowViewModel> LoadChildren(ElementBaseRowViewModel parent);

        /// <summary>
        ///     Verifies if a <see cref="ElementBaseRowViewModel" /> has children
        /// </summary>
        /// <param name="rowData">The <see cref="ElementBaseRowViewModel" /></param>
        /// <returns>The assert</returns>
        bool HasChildren(ElementBaseRowViewModel rowData);
    }
}
