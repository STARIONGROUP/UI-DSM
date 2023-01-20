// --------------------------------------------------------------------------------------------------------
// <copyright file="HaveThingRowLinkedBase.cs" company="RHEA System S.A.">
//  Copyright (c) 2023 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Components.App.HaveThingRowLinked
{
    using DevExpress.Blazor;

    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.App.SelectedItemCard;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;

    /// <summary>
    ///     Base component for the <see cref="HaveThingRowLinked{THaveThingRowViewModel}" /> components
    /// </summary>
    public abstract partial class HaveThingRowLinkedBase
    {
        /// <summary>
        ///     A <see cref="Dictionary{TKey,TValue}" /> for the <see cref="DynamicComponent.Parameters" />
        /// </summary>
        protected Dictionary<string, object> Parameters = new();

        /// <summary>
        ///     The <see cref="Type" /> for the <see cref="SelectedItemCard" />
        /// </summary>
        protected Type CardInformationType { get; set; }

        /// <summary>
        ///     The header of the popup
        /// </summary>
        protected string PopupHeader { get; set; }

        /// <summary>
        ///     Label name
        /// </summary>
        [Parameter]
        public string Label { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback{TValue}" /> to handle the double click on a property for navigation
        /// </summary>
        [Parameter]
        public EventCallback<string> OnItemDoubleClick { get; set; }

        /// <summary>
        ///     Reference to the <see cref="DxPopup" />
        /// </summary>
        public DxPopup Popup { get; set; }

        /// <summary>
        ///     Fire the <see cref="OnItemDoubleClick" /> callback
        /// </summary>
        /// <param name="propertyName">The name of the double-clicked property</param>
        /// <returns>A <see cref="Task" /></returns>
        protected async Task OnDoubleClick(string propertyName)
        {
            if (this.OnItemDoubleClick.HasDelegate)
            {
                await this.InvokeAsync(() => this.OnItemDoubleClick.InvokeAsync(propertyName));
            }
        }

        /// <summary>
        ///     Opens the more information popup
        /// </summary>
        /// <param name="item"></param>
        protected async Task OpenPopup(IHaveAnnotatableItemRowViewModel item)
        {
            var itemInformation = SelectedItemCardViewModel.GetCorrespondances(item);
            this.CardInformationType = itemInformation.Item1;
            this.Parameters["SelectedItem"] = item;
            this.PopupHeader = itemInformation.Item2;

            if (this.CardInformationType != null)
            {
                await this.Popup.ShowAsync();
            }
        }
    }
}
