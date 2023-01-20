// --------------------------------------------------------------------------------------------------------
// <copyright file="DocumentBased.razor.cs" company="RHEA System S.A.">
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
    using UI_DSM.Client.Components.App.HyperLinkCard;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Enumerator;

    /// <summary>
    ///     Component for the <see cref="View.DocumentBased" />
    /// </summary>
    public partial class DocumentBased : GenericBaseView<IDocumentBasedViewModel>
    {
        /// <summary>
        ///     Handle the OnClick event on a <see cref="HyperLinkCard" />
        /// </summary>
        /// <param name="row">The new selected <see cref="HyperLinkRowViewModel" /></param>
        private Task HandleOnClick(HyperLinkRowViewModel row)
        {
            this.ViewModel.SelectedElement = row;
            return this.HasChanged();
        }

        /// <summary>
        ///     Tries to navigate to a corresponding item
        /// </summary>
        /// <param name="itemName">The name of the item to navigate to</param>
        /// <returns>A <see cref="Task" /></returns>
        public override Task TryNavigateToItem(string itemName)
        {
            return Task.CompletedTask;
        }
    }
}
