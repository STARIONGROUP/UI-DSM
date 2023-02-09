// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveCreation.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.NormalUser.ProjectReview
{
    using Microsoft.AspNetCore.Components;
    using ReactiveUI;
    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This component is used to create new <see cref="ReviewObjective" />(s)
    /// </summary>
    public partial class ReviewObjectiveCreation : IDisposable
    {
        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     The <see cref="ReviewObjectiveCreationDto" />
        /// </summary>
        public ReviewObjectiveCreationDto ReviewObjectiveCreationDto { get; set; }

        /// <summary>
        ///     A collection of <see cref="ReviewObjectiveCreationDto" /> of kind PRR available for a review
        /// </summary>
        List<ReviewObjectiveCreationDto> AvailableReviewObjectiveCreationDtoPrr { get; set; } = new List<ReviewObjectiveCreationDto>();

        /// <summary>
        ///     A collection of <see cref="ReviewObjectiveCreationDto" /> of kind SRR available for a review
        /// </summary>
        List<ReviewObjectiveCreationDto> AvailableReviewObjectiveCreationDtoSrr { get; set; } = new List<ReviewObjectiveCreationDto>();

        /// <summary>
        ///     The text to display inside the creation button
        /// </summary>
        public string CreationText { get; set; }

        /// <summary>
        ///     The <see cref="IReviewObjectiveCreationViewModel" /> for the component
        /// </summary>
        [Parameter]
        public IReviewObjectiveCreationViewModel ViewModel { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.disposables.ForEach(x => x.Dispose());
            this.disposables.Clear();
        }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        protected override async Task OnInitializedAsync()
        {
            await this.ViewModel.OnInitializedAsync();
            this.AvailableReviewObjectiveCreationDtoPrr = this.ViewModel.AvailableReviewObjectiveCreationDto.Where(x => x.Kind == ReviewObjectiveKind.Prr).ToList();
            this.AvailableReviewObjectiveCreationDtoSrr = this.ViewModel.AvailableReviewObjectiveCreationDto.Where(x => x.Kind == ReviewObjectiveKind.Srr).ToList();

            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.ReviewObjectivesCreationStatus)
                .Subscribe(_ => this.OnCreationStatusChanged()));
        }

        /// <summary>
        ///     Sets properties when the <see cref="ReviewObjectivesCreationStatus" /> has changed
        /// </summary>
        private void OnCreationStatusChanged()
        {
            switch (this.ViewModel.ReviewObjectivesCreationStatus)
            {
                case CreationStatus.None:
                    this.CreationText = "Create";
                    break;
                case CreationStatus.Creating:
                    this.CreationText = "Creating";
                    break;
                case CreationStatus.Fail:
                    this.CreationText = "Retry...";
                    break;
                case CreationStatus.Done:
                    break;
                default:
                    throw new InvalidDataException("Unsupported value");
            }

            this.InvokeAsync(this.StateHasChanged);
        }

        /// <summary>
        /// Selects all availables <see cref="ReviewObjectiveCreationDto"/>
        /// </summary>
        private void SelectAll()
        {
	        this.ViewModel.SelectedReviewObjectivesPrr = new List<ReviewObjectiveCreationDto>(this.AvailableReviewObjectiveCreationDtoPrr);
	        this.ViewModel.SelectedReviewObjectivesSrr = new List<ReviewObjectiveCreationDto>(this.AvailableReviewObjectiveCreationDtoSrr);
        }
    }
}
