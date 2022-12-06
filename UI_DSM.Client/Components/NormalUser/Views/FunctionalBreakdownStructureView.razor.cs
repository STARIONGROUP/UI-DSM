// --------------------------------------------------------------------------------------------------------
// <copyright file="FunctionalBreakdownStructureView.razor.cs" company="RHEA System S.A.">
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
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Enumerator;

    /// <summary>
    ///     Component for the <see cref="View.FunctionalBreakdownStructureView" />
    /// </summary>
    public partial class FunctionalBreakdownStructureView: ElementBreakdownStructureView<IFunctionalBreakdownStructureViewViewModel>
    {
        /// <summary>
        ///     Hides columns on start
        /// </summary>
        protected override void HideColumnsAtStart()
        {
            var trlColumn = this.Grid.ColumnsCollection.FirstOrDefault(x => x.Property == nameof(FunctionRowViewModel.LinkedTrlValues));
            var techonologyColumn = this.Grid.ColumnsCollection.FirstOrDefault(x => x.Property == nameof(FunctionRowViewModel.LinkedTechnologyValues));

            if (trlColumn != null)
            {
                this.ColumnChooser.OnChangeValue(trlColumn);
            }

            if (techonologyColumn != null)
            {
                this.ColumnChooser.OnChangeValue(techonologyColumn);
            }
        }
    }
}
