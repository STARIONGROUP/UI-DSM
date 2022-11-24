// --------------------------------------------------------------------------------------------------------
// <copyright file="ParticipantServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Services.Administration.ParticipantService
{
    using System.Net;

    using NUnit.Framework;

    using RichardSzalay.MockHttp;

    using UI_DSM.Client.Services;
    using UI_DSM.Client.Services.Administration.ParticipantService;
    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ParticipantServiceTestFixture
    {
        private ParticipantService service;
        private MockHttpMessageHandler httpMessageHandler;
        private IJsonService jsonService;

        [SetUp]
        public void Setup()
        {
            this.httpMessageHandler = new MockHttpMessageHandler();
            var httpClient = this.httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/api");

            ServiceBase.RegisterService<ParticipantService>();
            this.jsonService = JsonSerializerHelper.CreateService();
            this.service = new ParticipantService(httpClient, this.jsonService);
            EntityHelper.RegisterEntities();
        }

        [Test]
        public async Task VerifyGetParticipants()
        {
            var projectId = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When($"/Project/{projectId}/Participant");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.GetParticipantsOfProject(projectId), Throws.Exception);

            httpResponse.StatusCode = HttpStatusCode.Forbidden;
            Assert.That(async () => await this.service.GetParticipantsOfProject(projectId), Throws.Exception);

            httpResponse.StatusCode = HttpStatusCode.OK;

            var roleGuid1 = Guid.NewGuid();
            var userGuid1 = Guid.NewGuid();
            var roleGuid2 = Guid.NewGuid();
            var userGuid2 = Guid.NewGuid();

            var entitiesDto = new List<EntityDto>
            {
                new ParticipantDto(Guid.NewGuid())
                {
                    Role = roleGuid1,
                    User = userGuid1
                },
                new RoleDto(roleGuid1)
                {
                    RoleName = "Project administrator",
                    AccessRights = new List<AccessRight>()
                    {
                        AccessRight.ReviewTask
                    }
                },
                new UserEntityDto(userGuid1)
                {
                    UserName = "admin", 
                    IsAdmin = true
                }, 
                new ParticipantDto(Guid.NewGuid())
                {
                    Role = roleGuid2,
                    User = userGuid2
                },
                new RoleDto(roleGuid2)
                {
                    RoleName = "Reviewer",
                    AccessRights = new List<AccessRight>()
                    {
                        AccessRight.ReviewTask
                    }
                },
                new UserEntityDto(userGuid2)
                {
                    UserName = "user"
                }
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entitiesDto));

            var participants = await this.service.GetParticipantsOfProject(projectId);
            Assert.That(participants, Has.Count.EqualTo(2));
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.GetParticipantsOfProject(projectId), Throws.Exception);
        }

        [Test]
        public async Task VerifyGetParticipant()
        {
            var projectId = Guid.NewGuid();
            var guid = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When(HttpMethod.Get, $"/Project/{projectId}/Participant/{guid}");
            request.Respond(_ => httpResponse);
            var participant = await this.service.GetParticipantOfProject(projectId, guid);

            Assert.That(participant, Is.Null);

            httpResponse.StatusCode = HttpStatusCode.OK;

            var roleGuid1 = Guid.NewGuid();
            var userGuid1 = Guid.NewGuid();

            var entitiesDto = new List<EntityDto>
            {
                new ParticipantDto(guid)
                {
                    Role = roleGuid1,
                    User = userGuid1
                },
                new RoleDto(roleGuid1)
                {
                    RoleName = "Project administrator",
                    AccessRights = new List<AccessRight>()
                    {
                        AccessRight.ReviewTask
                    }
                },
                new UserEntityDto(userGuid1)
                {
                    UserName = "admin",
                    IsAdmin = true
                }
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entitiesDto));
            participant = await this.service.GetParticipantOfProject(projectId,guid);

            Assert.That(participant.Id, Is.EqualTo(guid));

            httpResponse.Content = new StringContent(string.Empty);

            Assert.That(async () => await this.service.GetParticipantOfProject(projectId, guid), Throws.Exception);
        }

        [Test]
        public async Task VerifyCreateParticipant()
        {
            var participant = new Participant()
            {
                Role = new Role(Guid.NewGuid())
                {
                    RoleName = "Project administrator",
                    AccessRights = new List<AccessRight>()
                    {
                        AccessRight.ReviewTask
                    }
                },
                User = new UserEntity(Guid.NewGuid())
                {
                    UserName = "user"
                }
            };

            var projectId = Guid.NewGuid();

            var httpResponse = new HttpResponseMessage();

            var entityRequestResponse = new EntityRequestResponseDto()
            {
                IsRequestSuccessful = false
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));

            var request = this.httpMessageHandler.When(HttpMethod.Post, $"/Project/{projectId}/Participant/Create");
            request.Respond(_ => httpResponse);

            var requestResponse = await this.service.CreateParticipant(projectId, participant);
            Assert.That(requestResponse.IsRequestSuccessful, Is.False);

            entityRequestResponse.IsRequestSuccessful = true;

            entityRequestResponse.Entities = participant.GetAssociatedEntities().ToDtos();

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));

            requestResponse = await this.service.CreateParticipant(projectId, participant);

            Assert.Multiple(() =>
            {
                Assert.That(requestResponse.IsRequestSuccessful, Is.True);
                Assert.That(requestResponse.Entity, Is.Not.Null);
            });
            
            httpResponse.Content = new StringContent(string.Empty);

            Assert.That(async () => await this.service.CreateParticipant(projectId, participant), Throws.Exception);
        }

        [Test]
        public async Task VerifyDeleteParticipant()
        {
            var participant = new Participant()
            {
                Role = new Role(Guid.NewGuid())
                {
                    RoleName = "Project administrator",
                    AccessRights = new List<AccessRight>()
                    {
                        AccessRight.ReviewTask
                    }
                },
                User = new UserEntity(Guid.NewGuid())
                {
                    UserName = "user"
                }
            };

            var project = new Project(Guid.NewGuid());

            var httpResponse = new HttpResponseMessage();
            var requestResponse = new RequestResponseDto() { IsRequestSuccessful = true };
            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));
            var request = this.httpMessageHandler.When(HttpMethod.Delete, $"/Project/{project.Id}/Participant/{participant.Id}");
            request.Respond(_ => httpResponse);

            project.Participants.Add(participant);
            var result = await this.service.DeleteParticipant(participant, project.Id);
            Assert.That(result.IsRequestSuccessful, Is.True);

            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.DeleteParticipant(participant, project.Id), Throws.Exception);
        }

        [Test]
        public async Task VerifyUpdateParticipant()
        {
            var participant = new Participant()
            {
                Role = new Role(Guid.NewGuid())
                {
                    RoleName = "Project administrator",
                    AccessRights = new List<AccessRight>()
                    {
                        AccessRight.ReviewTask
                    }
                },
                User = new UserEntity(Guid.NewGuid())
                {
                    UserName = "user"
                }
            };

            var project = new Project(Guid.NewGuid());

            var httpResponse = new HttpResponseMessage();
            var requestResponse = new EntityRequestResponseDto() { IsRequestSuccessful = false };
            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));

            var request = this.httpMessageHandler.When(HttpMethod.Put, $"/Project/{project.Id}/Participant/{participant.Id}");
            request.Respond(_ => httpResponse);

            project.Participants.Add(participant);

            var requestResult = await this.service.UpdateParticipant(participant, project.Id);
            Assert.That(requestResult.IsRequestSuccessful, Is.False);

            requestResponse.IsRequestSuccessful = true;

            requestResponse.Entities = participant.GetAssociatedEntities().ToDtos();

            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));

            requestResult = await this.service.UpdateParticipant(participant, project.Id);
            Assert.That(requestResult.IsRequestSuccessful, Is.True);
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.UpdateParticipant(participant, project.Id), Throws.Exception);
        }

        [Test]
        public async Task VerifyGetAvailableUsersForCreation()
        {
            var projectId = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When(HttpMethod.Get, $"/Project/{projectId}/Participant/AvailableUsers");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.GetAvailableUsersForCreation(projectId), Throws.Exception);

            httpResponse.StatusCode = HttpStatusCode.OK;

            var usersEntities = new List<UserEntity>()
            {
                new (Guid.NewGuid())
                {
                    UserName = "admin",
                    IsAdmin = true
                },
                new (Guid.NewGuid())
                {
                    UserName = "user1"
                },
                new (Guid.NewGuid())
                {
                    UserName = "user2"
                }
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(usersEntities.ToDtos()));
            var availableUsers = await this.service.GetAvailableUsersForCreation(projectId);
            Assert.That(availableUsers, Has.Count.EqualTo(3));
        }

        [Test]
        public async Task VerifyGetCurrentParticipant()
        {
            var projectId = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When(HttpMethod.Get, $"/Project/{projectId}/Participant/LoggedUser");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.GetCurrentParticipant(projectId), Throws.Exception);

            httpResponse.StatusCode = HttpStatusCode.OK;

            var entities = new List<Entity>()
            {
                new Participant(Guid.NewGuid())
                {
                    User = new UserEntity(Guid.NewGuid())
                    {
                        UserName = "user1"
                    },
                    Role = new Role(Guid.NewGuid())
                }
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entities.ToDtos()));
            var participant = await this.service.GetCurrentParticipant(projectId);
            Assert.That(participant, Is.Not.Null);
        }
    }
}
