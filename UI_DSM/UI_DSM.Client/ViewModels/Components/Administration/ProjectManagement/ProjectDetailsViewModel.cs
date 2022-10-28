// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectDetailsViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.Administration.ProjectManagement
{
    using DevExpress.Blazor;
    using DynamicData;
    using Microsoft.AspNetCore.Components;
    using ReactiveUI;

    using UI_DSM.Client.Components.Administration.ProjectManagement;
    using UI_DSM.Client.Services.Administration.ParticipantService;
    using UI_DSM.Client.Services.Administration.RoleService;
    using UI_DSM.Client.ViewModels.Components.Administration.ParticipantManagement;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="ProjectDetails" /> component
    /// </summary>
    public class ProjectDetailsViewModel : ReactiveObject, IProjectDetailsViewModel
    {
        /// <summary>
        ///     Backing field for <see cref="Project" />
        /// </summary>
        private Project project;

        /// <summary>
        ///     The <see cref="Project" />
        /// </summary>
        public Project Project
        {
            get => this.project;
            set => this.RaiseAndSetIfChanged(ref this.project, value);
        }

        /// <summary>
        ///     The <see cref="IParticipantService" />
        /// </summary>
        private readonly IParticipantService participantService;

        /// <summary>
        ///     The <see cref="IRoleService" />
        /// </summary>
        private readonly IRoleService roleService;

        /// <summary>
        ///     The <see cref="IParticipantDetailsViewModel" />
        /// </summary>
        public IParticipantDetailsViewModel ParticipantDetailsViewModel { get; private set; } = new ParticipantDetailsViewModel();

        /// <summary>
        ///     Backing field for <see cref="IsOnUpdateViewMode" />
        /// </summary>
        private bool isOnUpdateViewMode;

        /// <summary>
        ///     The user that the administrator is going to delete
        /// </summary>
        private Participant participantToDelete;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectDetailsViewModel" /> class.
        /// </summary>
        public ProjectDetailsViewModel(IParticipantService participantService, IRoleService roleService)
        {
            this.participantService = participantService;
            this.roleService = roleService;

            this.ParticipantDetailsViewModel = new ParticipantDetailsViewModel
            {
                OnValidSubmit = new EventCallbackFactory().Create(this, this.UpdateParticipant)
            };

            this.ConfirmCancelPopup = new ConfirmCancelPopupViewModel
            {
                ConfirmRenderStyle = ButtonRenderStyle.Danger,
                OnCancel = new EventCallbackFactory().Create(this, this.OnCancelDelete),
                OnConfirm = new EventCallbackFactory().Create(this, this.OnConfirmDelete),
                ContentText = "Are you sure to want to remove this participant ?",
                HeaderText = "Are you sure ?"
            };
        }

        /// <summary>
        ///     The <see cref="IConfirmCancelPopupViewModel" />
        /// </summary>
        public IConfirmCancelPopupViewModel ConfirmCancelPopup { get; private set; }

        /// <summary>
        ///     Opens a popup to confirm to delete a participant
        /// </summary>
        /// <param name="participant">The <see cref="Participant" /> to remove</param>
        public void AskConfirmDeleteUser(Participant participant)
        {
            this.participantToDelete = participant;
            this.ConfirmCancelPopup.ContentText = $"You are about to delete the participant {this.participantToDelete.User.UserName}.\nAre you sure?";
            this.ConfirmCancelPopup.IsVisible = true;
        }

        /// <summary>
        ///     Opens a popup to provide informations for the given <see cref="Participant" />
        /// </summary>
        /// <param name="participant">The <see cref="Participant" /></param>
        public async Task OpenUpdatePopup(Participant participant)
        {
            this.ParticipantDetailsViewModel.Participant = participant;
            this.ParticipantDetailsViewModel.AvailableRoles = await roleService.GetRoles();
            this.IsOnUpdateViewMode = true;
        }

        /// <summary>
        ///     Value indicating the administrator is currently updating roles of a  <see cref="Participant"/>
        /// </summary>
        public bool IsOnUpdateViewMode
        {
            get => this.isOnUpdateViewMode;
            set => this.RaiseAndSetIfChanged(ref this.isOnUpdateViewMode, value);
        }

        /// <summary>
        ///     Callback used when the confirmation dialog has confirmed the deletion
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task OnConfirmDelete()
        {
            var deleteResult = await this.participantService.DeleteParticipant(this.participantToDelete, this.project.Id);

            if (deleteResult.IsRequestSuccessful)
            {
                this.Project.Participants.Remove(this.participantToDelete);
            }

            this.OnCancelDelete();
        }

        /// <summary>
        ///     Callback used when the confirmation dialog has canceled the deletion
        /// </summary>
        private void OnCancelDelete()
        {
            this.ConfirmCancelPopup.IsVisible = false;
            this.participantToDelete = null;
        }

        /// <summary>
        ///     The <see cref="IErrorMessageViewModel" />
        /// </summary>
        public IErrorMessageViewModel ErrorMessageViewModel { get; } = new ErrorMessageViewModel();

        /// <summary>
        ///     Tries to update a <see cref="Participant" />
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task UpdateParticipant()
        {
            try
            {
                var updateResult = await this.participantService.UpdateParticipant(this.ParticipantDetailsViewModel.Participant, this.project.Id);
                this.ErrorMessageViewModel.Errors.Clear();

                if (updateResult.Errors.Any())
                {
                    this.ErrorMessageViewModel.Errors.AddRange(updateResult.Errors);
                }

                if (updateResult.IsRequestSuccessful)
                {
                    this.ParticipantDetailsViewModel.Participant = updateResult.Entity;
                }
                
                this.IsOnUpdateViewMode = !updateResult.IsRequestSuccessful;
            
            }
            catch (Exception exception)
            {
                this.ErrorMessageViewModel.Errors.Clear();
                this.ErrorMessageViewModel.Errors.Add(exception.Message);
            }
        }
    }
}
