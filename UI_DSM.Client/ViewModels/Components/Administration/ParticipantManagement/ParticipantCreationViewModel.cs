// --------------------------------------------------------------------------------------------------------
// <copyright file="ParticipantCreationViewModel.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.ViewModels.Components.Administration.ParticipantManagement
{
    using CDP4Common.CommonData;
    using CDP4Common.SiteDirectoryData;

    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Components.Administration.ParticipantManagement;
    using UI_DSM.Client.Services.Administration.ParticipantService;
    using UI_DSM.Client.Services.Administration.RoleService;
    using UI_DSM.Client.Services.ThingService;
    using UI_DSM.Shared.Models;

    using Participant = UI_DSM.Shared.Models.Participant;

    /// <summary>
    ///     View model for the <see cref="ParticipantCreation" /> component
    /// </summary>
    public class ParticipantCreationViewModel : IParticipantCreationViewModel
    {
        /// <summary>
        ///     The <see cref="IParticipantService" />
        /// </summary>
        private readonly IParticipantService participantService;

        /// <summary>
        ///     The <see cref="IRoleService" />
        /// </summary>
        private readonly IRoleService roleService;

        /// <summary>
        ///     The <see cref="IThingService" />
        /// </summary>
        private readonly IThingService thingService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ParticipantCreationViewModel" /> class.
        /// </summary>
        /// <param name="participantService">The <see cref="IParticipantService" /></param>
        /// <param name="roleService">The <see cref="IRoleService" /></param>
        /// <param name="thingService">The <see cref="IThingService" /></param>
        public ParticipantCreationViewModel(IParticipantService participantService, IRoleService roleService, IThingService thingService)
        {
            this.participantService = participantService;
            this.roleService = roleService;
            this.thingService = thingService;
        }

        /// <summary>
        ///     A collection of available <see cref="Role" />
        /// </summary>
        public IEnumerable<Role> AvailableRoles { get; set; } = new List<Role>();

        /// <summary>
        ///     A collection of <see cref="AvailableUsers" />
        /// </summary>
        public IEnumerable<UserEntity> AvailableUsers { get; set; } = new List<UserEntity>();

        /// <summary>
        ///     A collection of available <see cref="DomainOfExpertise" /> names
        /// </summary>
        public IEnumerable<string> AvailableDomains { get; set; } = new List<string>();

        /// <summary>
        ///     An <see cref="EventCallback" /> to invoke on form submit
        /// </summary>
        public EventCallback OnValidSubmit { get; set; }

        /// <summary>
        ///     The <see cref="IParticipantCreationViewModel.Participant" /> to create
        /// </summary>
        public Participant Participant { get; set; } = new();

        /// <summary>
        ///     The collection of selected <see cref="CDP4Common.SiteDirectoryData.DomainOfExpertise" />
        /// </summary>
        public IEnumerable<string> SelectedDomains { get; set; } = new List<string>();

        /// <summary>
        ///     Updates this view model properties
        /// </summary>
        /// <param name="project">The <see cref="Project" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task UpdateProperties(Project project)
        {
            this.AvailableRoles = await this.roleService.GetRoles();
            this.AvailableUsers = await this.participantService.GetAvailableUsersForCreation(project.Id);
            this.SelectedDomains = new List<string>();

            this.Participant = new Participant
            {
                User = this.AvailableUsers.FirstOrDefault(),
                Role = this.AvailableRoles.FirstOrDefault()
            };

            var domains= (await this.thingService.GetThings(project.Id, project.Artifacts.OfType<Model>().Select(x => x.Id), ClassKind.DomainOfExpertise))
                .OfType<DomainOfExpertise>().Select(x => x.ShortName).Distinct().ToList();
            
            domains.Sort();
            this.AvailableDomains = domains;
        }
    }
}
