// --------------------------------------------------------------------------------------------------------
// <copyright file="HaveTechnologyView.razor.cs" company="RHEA System S.A.">
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

    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Component for view that have a Technology View inside the <see cref="TraceabilityTableViewModel"/>
    /// </summary>
    /// <typeparam name="TViewModel">A <see cref="IHaveTechnologyViewViewModel"/></typeparam>
    public abstract partial class HaveTechnologyView<TViewModel> : HaveTraceabilityTableView<TViewModel> where TViewModel: IHaveTechnologyViewViewModel
    {
        /// <summary>
        ///     Initialize the correspondant ViewModel for this component
        /// </summary>
        /// <param name="things">The collection of <see cref="Thing" /></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <param name="reviewTaskId">The <see cref="ReviewTask" /> id</param>
        /// <param name="prefilters">A collection of prefilters</param>
        /// <param name="additionnalColumnsVisibleAtStart">A collection of columns name that can be visible by default at start</param>
        /// <param name="participant">The current <see cref="Participant"/></param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task InitializeViewModel(IEnumerable<Thing> things, Guid projectId, Guid reviewId, Guid reviewTaskId, List<string> prefilters, List<string> additionnalColumnsVisibleAtStart, Participant participant)
        {
            await base.InitializeViewModel(things, projectId, reviewId, reviewTaskId, prefilters, additionnalColumnsVisibleAtStart, participant);
            this.IsLoading = false;

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
