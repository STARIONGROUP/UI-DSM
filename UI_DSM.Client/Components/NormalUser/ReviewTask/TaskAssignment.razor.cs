// --------------------------------------------------------------------------------------------------------
// <copyright file="TaskAssignement.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.NormalUser.ReviewTask
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ReviewTask;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This component is used to assign a <see cref="Participant" /> to a <see cref="ReviewTask" />
    /// </summary>
    public partial class TaskAssignment
    {
        /// <summary>
        ///     The <see cref="Dictionary{TKey,TValue}" /> for the grouped <see cref="Participant" />
        /// </summary>
        private Dictionary<string, IEnumerable<Participant>> groupedParticipants = new();

        /// <summary>
        ///     The <see cref="Dictionary{TKey,TValue}" /> the opened panel
        /// </summary>
        private Dictionary<string, bool> openedPanels = new();

        /// <summary>
        ///     The <see cref="Dictionary{TKey,TValue}" /> for the selected <see cref="Participant" />
        /// </summary>
        private Dictionary<string, IEnumerable<Participant>> selectedParticipants = new();

        /// <summary>
        ///     A collection of <see cref="Participant" />s of project
        /// </summary>
        [Parameter]
        public IEnumerable<Participant> ProjectParticipants { get; set; }

        /// <summary>
        ///     The <see cref="IReviewCreationViewModel" /> for the component
        /// </summary>
        [Parameter]
        public ITaskAssignmentViewModel ViewModel { get; set; }

        /// <summary>
        ///     Method invoked when the component has received parameters from its parent in
        ///     the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            this.groupedParticipants = this.ProjectParticipants.GroupByDomainOfExpertise();

            foreach (var group in this.groupedParticipants.Keys)
            {
                this.openedPanels[group] = false;
                
                this.selectedParticipants[group] = new List<Participant>(this.ViewModel.SelectedParticipants
                    .Where(x => x.DomainsOfExpertise.Contains(group)));
            }
        }

        /// <summary>
        ///     Handle the change of selected <see cref="Participant" />
        /// </summary>
        /// <param name="participants">The new selected <see cref="Participant" /></param>
        /// <param name="group">The name of the modified group</param>
        private async Task OnValuesChanged(IEnumerable<Participant> participants, string group)
        {
            participants = participants.ToList();
            var availableParticipants = this.groupedParticipants[group];
            var previouslySelectedParticipant = this.selectedParticipants[group];

            var addedParticipants = participants.Where(x => previouslySelectedParticipant.All(p => p.Id != x.Id))
                .ToList();

            var removedParticipants = availableParticipants.Where(x => participants.All(p => p.Id != x.Id))
                .ToList();

            this.ViewModel.SelectedParticipants.AddRange(addedParticipants.Where(x => this.ViewModel.SelectedParticipants.All(p => p.Id != x.Id)));
            this.ViewModel.SelectedParticipants.RemoveAll(x => removedParticipants.Any(p => x.Id == p.Id));

            foreach (var key in this.groupedParticipants.Keys)
            {
                this.selectedParticipants[key] = new List<Participant>(this.ViewModel.SelectedParticipants.Where(x => x.DomainsOfExpertise.Contains(key)));
            }

            await this.InvokeAsync(this.StateHasChanged);
        }
    }
}
