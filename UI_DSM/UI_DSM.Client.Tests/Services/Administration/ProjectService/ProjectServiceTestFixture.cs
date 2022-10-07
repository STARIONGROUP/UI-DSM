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
    using System;
    using System.Net;

    using NUnit.Framework;

    using RichardSzalay.MockHttp;

    using UI_DSM.Client.Services;
    using UI_DSM.Client.Services.Administration.ProjectService;
    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Serializer.Json;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ProjectServiceTestFixture
    {
        private ProjectService service;
        private MockHttpMessageHandler httpMessageHandler;
        private IJsonService jsonService;

        [SetUp]
        public void Setup()
        {
            this.httpMessageHandler = new MockHttpMessageHandler();
            var httpClient = this.httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/api");

            ServiceBase.RegisterService<ProjectService>();
            this.jsonService = new JsonService(new JsonDeserializer(), new JsonSerializer());
            this.service = new ProjectService(httpClient, this.jsonService);
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

            httpResponse.Content = new StringContent(this.jsonService.Serialize(projectDtos));

            var projects = this.service.GetProjects().Result;
            Assert.That(projects.Count, Is.EqualTo(1));
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await  this.service.GetProjects(), Throws.Exception);
        }

        [Test]
        public async Task VerifyCreateProject()
        {
            var project = new Project()
            {
                ProjectName = "Project"
            };

            var httpResponse = new HttpResponseMessage();
            
            var entityRequestResponse = new EntityRequestResponseDto()
            {
                IsRequestSuccessful = false
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));

            var request = this.httpMessageHandler.When(HttpMethod.Post, "/Project/Create");
            request.Respond(_ => httpResponse);
            Assert.That(this.service.CreateProject(project).Result.IsRequestSuccessful, Is.False);

            entityRequestResponse.IsRequestSuccessful = true;

            entityRequestResponse.Entities = new List<EntityDto>()
            {
                project.ToDto()
            }; 

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));

            var result = await this.service.CreateProject(project);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsRequestSuccessful, Is.True);
                Assert.That(result.Entity, Is.Not.Null);
            });

            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.CreateProject(project), Throws.Exception);
        }

        [Test]
        public void VerifyDeleteProject()
        {
            var project = new Project(Guid.NewGuid());
            var httpResponse = new HttpResponseMessage();
            var requestResponse = new RequestResponseDto() { IsRequestSuccessful = true };
            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));
            var request = this.httpMessageHandler.When(HttpMethod.Delete, $"/Project/{project.Id}");
            request.Respond(_ => httpResponse);

            Assert.That(this.service.DeleteProject(project).Result.IsRequestSuccessful, Is.True);

            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.DeleteProject(project), Throws.Exception);
        }

        [Test]
        public async Task VerifyGetProject()
        {
            var guid = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When(HttpMethod.Get, $"/Project/{guid}");
            request.Respond(_ => httpResponse);
            var project = await this.service.GetProject(guid);

            Assert.That(project, Is.Null);

            httpResponse.StatusCode = HttpStatusCode.OK;

            var projectDto = new ProjectDto(guid)
            {
                ProjectName = "Project"
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(new List<EntityDto>(){projectDto}));
            project = await this.service.GetProject(guid);
            Assert.That(project.Id, Is.EqualTo(guid));

            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.GetProject(guid), Throws.Exception);
        }

        [Test]
        public async Task VerifyGetUserParticipation()
        {
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When(HttpMethod.Get, "/Project/UserParticipation");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.GetUserParticipation(), Throws.Exception);

            var projects = new List<Project>()
            {
                new(Guid.NewGuid()),
                new(Guid.NewGuid())
            };

            httpResponse.StatusCode = HttpStatusCode.OK;
            httpResponse.Content = new StringContent(this.jsonService.Serialize(projects.ToDtos()));
            var foundProject = await this.service.GetUserParticipation();
            Assert.That(foundProject, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task VerifyGetOpenTaskAndComments()
        {
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.BadRequest;

            var request = this.httpMessageHandler.When(HttpMethod.Get, "/Project/OpenTasksAndComments");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.GetOpenTasksAndComments(), Throws.Exception);

            httpResponse.StatusCode = HttpStatusCode.OK;

            var guids = new List<Guid>
            {
                Guid.NewGuid()
            };

            var requestResults = new Dictionary<Guid, ComputedProjectProperties>
            {
                [guids[0]] = new()
                {
                    CommentCount = 15,
                    TaskCount = 12
                }
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResults));
            Assert.That(await this.service.GetOpenTasksAndComments(), Is.EquivalentTo(requestResults));
        }
    }
}
