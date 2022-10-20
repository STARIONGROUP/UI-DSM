// --------------------------------------------------------------------------------------------------------
// <copyright file="SelectedItemBase.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.SelectedItemCard.SelectedItemCardContent
{
    using Microsoft.AspNetCore.Components;

    /// <summary>
    ///     Base component that provide information for related to a selected item
    /// </summary>
    /// <typeparam name="TSelectedItem">A class</typeparam>
    public abstract partial class SelectedItemBase<TSelectedItem> where TSelectedItem : class
    {
        /// <summary>
        ///     The <see cref="TSelectedItem" />
        /// </summary>
        protected TSelectedItem RowViewModel { get; set; }

        /// <summary>
        ///     The <see cref="SelectedItem" />
        /// </summary>
        [Parameter]
        public object SelectedItem { get; set; }

        /// <summary>
        ///     Method invoked when the component has received parameters from its parent in
        ///     the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        protected override void OnParametersSet()
        {
            this.RowViewModel = this.SelectedItem as TSelectedItem;
            base.OnParametersSet();
        }
    }
}
