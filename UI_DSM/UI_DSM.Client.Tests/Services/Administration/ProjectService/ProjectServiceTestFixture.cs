// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Services.Administration.ProjectService
{
    using System.Net;
    using System.Text.Json;

    using NUnit.Framework;

    using RichardSzalay.MockHttp;

    using UI_DSM.Client.Services.Administration.ProjectService;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ProjectServiceTestFixture
    {
        private ProjectService service;
        private MockHttpMessageHandler httpMessageHandler;

        [SetUp]
        public void Setup()
        {
            this.httpMessageHandler = new MockHttpMessageHandler();
            var httpClient = this.httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/api");

            this.service = new ProjectService(httpClient);
        }

        [Test]
        public void VerifyGetProjects()
        {
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.Forbidden;

            var request = this.httpMessageHandler.When("/Project");
            request.Respond(_ => httpResponse);
            Assert.That(async () => await this.service.GetProjects(), Throws.Exception);

            httpResponse.StatusCode = HttpStatusCode.OK;
            
            var projectDtos = new List<ProjectDto>
            {
                new(Guid.NewGuid())
                {
                    ProjectName = "Project"
                }
            };

            httpResponse.Content = new StringContent(JsonSerializer.Serialize(projectDtos));

            var projects = this.service.GetProjects().Result;
            Assert.That(projects.Count, Is.EqualTo(1));
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await  this.service.GetProjects(), Throws.Exception);
        }

        [Test]
        public void VerifyCreateProject()
        {
            var project = new Project()
            {
                ProjectName = "Project"
            };

            var httpResponse = new HttpResponseMessage();
            
            var entityRequestResponse = new EntityRequestResponseDto<ProjectDto>()
            {
                IsRequestSuccessful = false
            };

            httpResponse.Content = new StringContent(JsonSerializer.Serialize(entityRequestResponse));

            var request = this.httpMessageHandler.When(HttpMethod.Post, "/Project/Create");
            request.Respond(_ => httpResponse);
            Assert.That(this.service.CreateProject(project).Result.IsRequestSuccessful, Is.False);

            entityRequestResponse.IsRequestSuccessful = true;
            entityRequestResponse.Entity = project.ToDto() as ProjectDto;
            httpResponse.Content = new StringContent(JsonSerializer.Serialize(entityRequestResponse));
            Assert.That(this.service.CreateProject(project).Result.IsRequestSuccessful, Is.True);
            Assert.That(this.service.CreateProject(project).Result.Entity, Is.Not.Null);

            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.CreateProject(project), Throws.Exception);
        }

        [Test]
        public void VerifyDeleteProject()
        {
            var project = new Project(Guid.NewGuid());
            var httpResponse = new HttpResponseMessage();
            var requestResponse = new RequestResponseDto() { IsRequestSuccessful = true };
            httpResponse.Content = new StringContent(JsonSerializer.Serialize(requestResponse));
            var request = this.httpMessageHandler.When(HttpMethod.Delete, $"/Project/{project.Id}");
            request.Respond(_ => httpResponse);

            Assert.That(this.service.DeleteProject(project).Result.IsRequestSuccessful, Is.True);

            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.DeleteProject(project), Throws.Exception);
        }

        [Test]
        public void VerifyGetProject()
        {
            var guid = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When(HttpMethod.Get, $"/Project/{guid}");
            request.Respond(_ => httpResponse);
            Assert.That(this.service.GetProject(guid).Result, Is.Null);

            httpResponse.StatusCode = HttpStatusCode.OK;

            var projectDto = new ProjectDto(guid)
            {
                ProjectName = "Project"
            };

            httpResponse.Content = new StringContent(JsonSerializer.Serialize(projectDto));
            Assert.That(this.service.GetProject(guid).Result.Id, Is.EqualTo(guid));

            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.GetProject(guid), Throws.Exception);
        }
    }
}
