// --------------------------------------------------------------------------------------------------------
// <copyright file="HaveThingRowLinked.cs" company="RHEA System S.A.">
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
    using CDP4Common.CommonData;

    using DevExpress.Blazor;

    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Components.App.AppKeyValue;
    using UI_DSM.Client.ViewModels.App.SelectedItemCard;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;

    /// <summary>
    ///     Component used to represent, inside the context panel, that an element are linked to a
    ///     <see cref="IHaveThingRowViewModel" />
    /// </summary>
    public partial class HaveThingRowLinked<THaveThingRowViewModel> where THaveThingRowViewModel : IHaveThingRowViewModel, new()
    {
        /// <summary>
        ///     The <see cref="THaveThingRowViewModel" />
        /// </summary>
        private THaveThingRowViewModel row;

        /// <summary>
        ///     Gets or sets the <see cref="AppKeyValue.VariantValue" />
        /// </summary>
        [Parameter]
        public AppKeyValue.VariantValue Variant { get; set; } = AppKeyValue.VariantValue.Horizontal;

        /// <summary>
        ///     String value conversion based on <see cref="AppKeyValue.VariantValue" />
        /// </summary>
        private string componentVariant;

        /// <summary>
        ///     A <see cref="Thing" />
        /// </summary>
        [Parameter]
        public Thing Item { get; set; }

        /// <summary>
        ///     Method invoked when the component has received parameters from its parent in
        ///     the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            this.componentVariant = this.Variant switch
            {
                AppKeyValue.VariantValue.Horizontal => "app-key-value--horizontal",
                AppKeyValue.VariantValue.Vertical => "app-key-value--vertical",
                _ => ""
            };

            this.row = new THaveThingRowViewModel();

            if (!this.row.UpdateThing(this.Item))
            {
                this.row = default;
            }
        }
    }
}
