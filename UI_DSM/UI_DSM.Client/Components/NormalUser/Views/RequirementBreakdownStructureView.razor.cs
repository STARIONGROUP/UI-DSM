// --------------------------------------------------------------------------------------------------------
// <copyright file="RequirementBreakdownStructureView.razor.cs" company="RHEA System S.A.">
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
    using CDP4Common.CommonData;

    using DevExpress.Blazor;

    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Shared.Enumerator;

    /// <summary>
    ///     Component for the <see cref="View.RequirementBreakdownStructureView" />
    /// </summary>
    public partial class RequirementBreakdownStructureView : GenericBaseView<IRequirementBreakdownStructureViewViewModel>
    {
        /// <summary>
        ///     The <see cref="DxGrid" />
        /// </summary>
        public DxGrid DxGrid { get; set; }

        /// <summary>
        ///     Initialize the correspondant ViewModel for this component
        /// </summary>
        /// <param name="things">The collection of <see cref="Thing" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task InitializeViewModel(IEnumerable<Thing> things)
        {
            this.ViewModel.InitializeProperties(things);
            await this.InvokeAsync(this.StateHasChanged);
        }

        /// <summary>
        ///     Handle the selection of the column chooser
        /// </summary>
        private void OnClick()
        {
            this.DxGrid.ShowColumnChooser(".column-chooser-button");
        }
    }
}
