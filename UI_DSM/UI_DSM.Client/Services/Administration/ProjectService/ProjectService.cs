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
    using System.Text.Json;

    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     The <see cref="ProjectService" /> provide capability to manage <see cref="Project" />s.
    /// </summary>
    public class ProjectService : ServiceBase, IProjectService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectService" /> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="ServiceBase.HttpClient" /></param>
        public ProjectService(HttpClient httpClient) : base(httpClient)
        {
        }

        /// <summary>
        ///     Provide a collection of <see cref="Project" />
        /// </summary>
        /// <returns>A <see cref="Task" /> where the result is a collection of <see cref="Project" /></returns>
        public async Task<List<Project>> GetProjects()
        {
            try
            {
                var response = await this.HttpClient.GetAsync("Project");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(content);
                }

                var projectDtos = JsonSerializer.Deserialize<List<ProjectDto>>(content, this.JsonSerializerOptions);

                return projectDtos!.Select(x =>
                {
                    var project = (Project)x.InstantiatePoco();
                    project.ResolveProperties(x);
                    return project;
                }).ToList();
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
                var content = JsonSerializer.Serialize((ProjectDto)newProject.ToDto());
                var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

                var response = await this.HttpClient.PostAsync("Project/Create", bodyContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                var entityRequestResponse = JsonSerializer.Deserialize<EntityRequestResponseDto<ProjectDto>>(responseContent, this.JsonSerializerOptions);

                if (!entityRequestResponse!.IsRequestSuccessful)
                {
                    return EntityRequestResponse<Project>.Fail(entityRequestResponse.Errors);
                }

                var project = (Project)entityRequestResponse.Entity.InstantiatePoco();
                project.ResolveProperties(entityRequestResponse.Entity);

                return EntityRequestResponse<Project>.Success(project);
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
                var url = Path.Combine("Project", projectToDelete.Id.ToString());
                var deleteResponse = await this.HttpClient.DeleteAsync(url);
                var deleteContent = await deleteResponse.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<RequestResponseDto>(deleteContent, this.JsonSerializerOptions);
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
        /// <returns>A <see cref="Task" /> with the <see cref="Project" /> if found</returns>
        public async Task<Project> GetProject(Guid projectGuid)
        {
            try
            {
                var url = Path.Combine("Project", projectGuid.ToString());
                var getProjectResponse = await this.HttpClient.GetAsync(url);
                var getProjectContent = await getProjectResponse.Content.ReadAsStringAsync();

                if (!getProjectResponse.IsSuccessStatusCode)
                {
                    return null;
                }

                var projectDto = JsonSerializer.Deserialize<ProjectDto>(getProjectContent, this.JsonSerializerOptions);
                var project = (Project)projectDto!.InstantiatePoco();
                project.ResolveProperties(projectDto);
                return project;
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }
    }
}
