// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.Administration.ProjectService
{
    using System.Text;

    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Shared.Assembler;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     The <see cref="ProjectService" /> provide capability to manage <see cref="Project" />s.
    /// </summary>
    [Route("Project")]
    public class ProjectService : EntityServiceBase<Project, ProjectDto>, IProjectService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectService" /> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="ServiceBase.HttpClient" /></param>
        /// <param name="jsonService">The <see cref="IJsonService" /></param>
        public ProjectService(HttpClient httpClient, IJsonService jsonService) : base(httpClient, jsonService)
        {
        }

        /// <summary>
        ///     Provide a collection of <see cref="Project" />
        /// </summary>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> where the result is a collection of <see cref="Project" /></returns>
        public async Task<List<Project>> GetProjects(int deepLevel = 0)
        {
            try
            {
                return await this.GetEntities(deepLevel);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Creates a new <see cref="Project" />
        /// </summary>
        /// <param name="newProject">The <see cref="Project" /> to create</param>
        /// <returns>A <see cref="Task" /> with the <see cref="Project" /> if created correctly</returns>
        public async Task<EntityRequestResponse<Project>> CreateProject(Project newProject)
        {
            try
            {
                return await this.CreateEntity(newProject, 0);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Delete a <see cref="Project" />
        /// </summary>
        /// <param name="projectToDelete">The project to delete</param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /></returns>
        public async Task<RequestResponseDto> DeleteProject(Project projectToDelete)
        {
            try
            {
                return await this.DeleteEntity(projectToDelete);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Gets a <see cref="Project" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="projectGuid">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the <see cref="Project" /> if found</returns>
        public async Task<Project> GetProject(Guid projectGuid, int deepLevel = 0)
        {
            try
            {
                return await this.GetEntity(projectGuid, deepLevel);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Gets a collection of all <see cref="Project" /> where the current user is participating
        /// </summary>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Project" /> as result</returns>
        public async Task<List<Project>> GetUserParticipation()
        {
            try
            {
                var response = await this.HttpClient.GetAsync(Path.Combine(this.MainRoute, "UserParticipation"));

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(await response.Content.ReadAsStringAsync());
                }

                var dtos = this.jsonService.Deserialize<IEnumerable<EntityDto>>(await response.Content.ReadAsStreamAsync());
                return Assembler.CreateEntities<Project>(dtos).ToList();
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Gets, for all <see cref="Project" />, the number of open <see cref="ReviewTask" /> and <see cref="Comment" />
        ///     related to the <see cref="Project" />
        /// </summary>
        /// <returns>A <see cref="Task" /> with a <see cref="Dictionary{Guid, ComputedProjectProperties}" /></returns>
        public async Task<Dictionary<Guid, ComputedProjectProperties>> GetOpenTasksAndComments()
        {
            var response = await this.HttpClient.GetAsync($"{this.MainRoute}/OpenTasksAndComments");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }
            return this.jsonService.Deserialize<Dictionary<Guid, ComputedProjectProperties>>(await response.Content.ReadAsStreamAsync());
        }
    }
}
