// --------------------------------------------------------------------------------------------------------
// <copyright file="ParticipantService.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Services.Administration.ParticipantService
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Shared.Assembler;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     The <see cref="ParticipantService" /> provide capability to manage <see cref="Participant" />s inside a
    ///     <see cref="Project" />
    /// </summary>
    [Route("Project/{0}/Participant")]
    public class ParticipantService : EntityServiceBase<Participant, ParticipantDto>, IParticipantService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ParticipantService" /> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="ServiceBase.HttpClient" /></param>
        /// <param name="jsonService">The <see cref="IJsonService" /></param>
        public ParticipantService(HttpClient httpClient, IJsonService jsonService) : base(httpClient, jsonService)
        {
        }

        /// <summary>
        ///     Gets all <see cref="Participant" />s contained inside a <see cref="Project" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="Participant" /></returns>
        public async Task<List<Participant>> GetParticipantsOfProject(Guid projectId, int deepLevel = 0)
        {
            try
            {
                this.ComputeMainRoute(projectId);
                return await this.GetEntities(deepLevel);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Gets a <see cref="Participant" /> contained inside a <see cref="Project" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="participantId">The <see cref="Guid" /> of the <see cref="Participant" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="Participant" /></returns>
        public async Task<Participant> GetParticipantOfProject(Guid projectId, Guid participantId, int deepLevel = 0)
        {
            try
            {
                this.ComputeMainRoute(projectId);
                return await this.GetEntity(participantId, deepLevel);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Creates a new <see cref="Participant" /> inside a <see cref="Project" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="participant">The <see cref="Participant" />to create</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{Participant}" /></returns>
        public async Task<EntityRequestResponse<Participant>> CreateParticipant(Guid projectId, Participant participant)
        {
            try
            {
                this.ComputeMainRoute(projectId);
                return await this.CreateEntity(participant, 0);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Updates a <see cref="Participant" />
        /// </summary>
        /// <param name="participant">The <see cref="Participant" />to update</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{Participant}" /></returns>
        public async Task<EntityRequestResponse<Participant>> UpdateParticipant(Participant participant)
        {
            this.VerifyEntityAndContainer<Project>(participant);

            try
            {
                this.ComputeMainRoute(participant.EntityContainer.Id);
                return await this.UpdateEntity(participant, 0);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Deletes a <see cref="Participant" />
        /// </summary>
        /// <param name="participant">The <see cref="Participant" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /></returns>
        public async Task<RequestResponseDto> DeleteParticipant(Participant participant)
        {
            this.VerifyEntityAndContainer<Project>(participant);

            try
            {
                this.ComputeMainRoute(participant.EntityContainer.Id);
                return await this.DeleteEntity(participant);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Gets a collection of <see cref="UserEntity" /> that can be used for the creation of <see cref="Participant" /> into a
        ///     <see cref="Project" />
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="UserEntity" /></returns>
        public async Task<List<UserEntity>> GetAvailableUsersForCreation(Guid projectId)
        {
            try
            {
                this.ComputeMainRoute(projectId);
                var getResponse = await this.HttpClient.GetAsync(Path.Combine(this.MainRoute, "AvailableUsers"));

                if (!getResponse.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(await getResponse.Content.ReadAsStringAsync());
                }

                var dtos = this.jsonService.Deserialize<IEnumerable<EntityDto>>(await getResponse.Content.ReadAsStreamAsync());
                return Assembler.CreateEntities<UserEntity>(dtos).ToList();
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }
    }
}
