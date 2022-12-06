// --------------------------------------------------------------------------------------------------------
// <copyright file="UserService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.Administration.UserService
{
    using System.Text;
    using System.Text.Json;

    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Shared.Assembler;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.DTO.UserManagement;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="UserService" /> provide capability to manage users.
    /// </summary>
    [Route("User")]
    public class UserService : ServiceBase, IUserService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UserService" /> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient" /></param>
        /// <param name="jsonService">The <see cref="IJsonService" /></param>
        public UserService(HttpClient httpClient, IJsonService jsonService) : base(httpClient, jsonService)
        {
        }

        /// <summary>
        ///     Provide a collection of registered <see cref="UserEntityDto" />
        /// </summary>
        /// <returns>A task where the result is a collection of <see cref="UserEntityDto" /></returns>
        public async Task<List<UserEntityDto>> GetUsers()
        {
            var response = await this.HttpClient.GetAsync(this.MainRoute);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(await response.Content.ReadAsStringAsync());
            }

            var entities = this.jsonService.Deserialize<IEnumerable<EntityDto>>(await response.Content.ReadAsStreamAsync());
            return entities.Where(x => x.GetType() == typeof(UserEntityDto)).Cast<UserEntityDto>().ToList();
        }

        /// <summary>
        ///     Register a new user based on <see cref="RegistrationDto" />
        /// </summary>
        /// <param name="newUser">The <see cref="RegistrationDto" /></param>
        /// <returns>A <see cref="Task" /> with the result of the request</returns>
        public async Task<RegistrationResponseDto> RegisterUser(RegistrationDto newUser)
        {
            var content = JsonSerializer.Serialize(newUser);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            var registerResponse = await this.HttpClient.PostAsync(Path.Combine(this.MainRoute, "Register"), bodyContent);
            return this.jsonService.Deserialize<RegistrationResponseDto>(await registerResponse.Content.ReadAsStreamAsync());
        }

        /// <summary>
        ///     Delete a registered user
        /// </summary>
        /// <param name="userEntityToDelete">The user to delete</param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /></returns>
        public async Task<RequestResponseDto> DeleteUser(UserEntityDto userEntityToDelete)
        {
            if (!userEntityToDelete.IsAdmin)
            {
                var url = Path.Combine(this.MainRoute, userEntityToDelete.Id.ToString());
                var deleteResponse = await this.HttpClient.DeleteAsync(url);

                return this.jsonService.Deserialize<RequestResponseDto>(await deleteResponse.Content.ReadAsStreamAsync());
            }

            return new RequestResponseDto
            {
                Errors = new List<string>
                {
                    "This action is forbidden"
                }
            };
        }

        /// <summary>
        ///     Gets all participants linked to the current user
        /// </summary>
        /// <returns>A collection of <see cref="Participant" /></returns>
        public async Task<List<Participant>> GetParticipantsForUser()
        {
            var response = await this.HttpClient.GetAsync(Path.Combine(this.MainRoute, "LinkedParticipants"));

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(await response.Content.ReadAsStringAsync());
            }

            var dtos = this.jsonService.Deserialize<IEnumerable<EntityDto>>(await response.Content.ReadAsStreamAsync()).ToList();
            return Assembler.CreateEntities<Participant>(dtos).ToList();
        }
    }
}
