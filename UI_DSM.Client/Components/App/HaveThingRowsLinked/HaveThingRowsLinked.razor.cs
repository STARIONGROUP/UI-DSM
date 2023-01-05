// --------------------------------------------------------------------------------------------------------
// <copyright file="HaveThingRowsLinked.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.HaveThingRowsLinked
{
    using CDP4Common.CommonData;

    using DevExpress.Blazor;

    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.App.SelectedItemCard;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;

    /// <summary>
    ///     Component used to represent, inside the context panel, that multiple element are linked to a
    ///     <see cref="IHaveThingRowViewModel" />
    /// </summary>
    public partial class HaveThingRowsLinked<THaveThingRowViewModel> where THaveThingRowViewModel : IHaveThingRowViewModel, new()
    {
        /// <summary>
        ///     A collection of <see cref="THaveThingRowViewModel" />
        /// </summary>
        private List<THaveThingRowViewModel> rows = new();

        /// <summary>
        ///     A collection of <see cref="Thing" />
        /// </summary>
        [Parameter]
        public IEnumerable<Thing> Items { get; set; }

        /// <summary>
        ///     Value indicating if the panel is open
        /// </summary>
        public bool IsPanelOpen { get; set; }

        /// <summary>
        ///     Method invoked when the component has received parameters from its parent in
        ///     the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            this.rows = new List<THaveThingRowViewModel>();

            foreach (var item in this.Items)
            {
                var row = new THaveThingRowViewModel();

                if (row.UpdateThing(item))
                {
                    this.rows.Add(row);
                }
            }
        }
    }
}
