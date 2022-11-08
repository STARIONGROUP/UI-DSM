// --------------------------------------------------------------------------------------------------------
// <copyright file="RequirementTraceabilityToProductView.razor.cs" company="RHEA System S.A.">
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

    using ReactiveUI;

    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Component for the <see cref="View.RequirementTraceabilityToProductView" />
    /// </summary>
    public partial class RequirementTraceabilityToProductView
    {
        /// <summary>
        ///     Initialize the correspondant ViewModel for this component
        /// </summary>
        /// <param name="things">The collection of <see cref="Thing" /></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task InitializeViewModel(IEnumerable<Thing> things, Guid projectId, Guid reviewId)
        {
            await base.InitializeViewModel(things, projectId, reviewId);

            this.Disposables.Add(this.WhenAnyValue(x => x.ViewModel.IsOnTechnologyView)
                .Subscribe(async _ => await this.SwitchView()));
        }

        /// <summary>
        ///     Switch view between Technology and non-technology one
        /// </summary>
        private async Task SwitchView()
        {
            this.ViewModel.OnTechnologyViewChange();
            await this.Table.Update();
        }
    }
}
