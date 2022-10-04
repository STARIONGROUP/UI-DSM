// --------------------------------------------------------------------------------------------------------
// <copyright file="RoleServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Services.Administration.RoleService
{
    using System.Net;

    using NUnit.Framework;

    using RichardSzalay.MockHttp;

    using UI_DSM.Client.Services;
    using UI_DSM.Client.Services.Administration.RoleService;
    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Serializer.Json;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class RoleServiceTestFixture
    {
        private RoleService service;
        private MockHttpMessageHandler httpMessageHandler;
        private IJsonService jsonService;

        [SetUp]
        public void Setup()
        {
            this.httpMessageHandler = new MockHttpMessageHandler();
            var httpClient = this.httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/api");

            ServiceBase.RegisterService<RoleService>();
            this.jsonService = new JsonService(new JsonDeserializer(), new JsonSerializer());
            this.service = new RoleService(httpClient, this.jsonService);
        }

        [Test]
        public void VerifyGetRoles()
        {
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.Forbidden;

            var request = this.httpMessageHandler.When("/Role");
            request.Respond(_ => httpResponse);
            Assert.That(async () => await this.service.GetRoles(), Throws.Exception);

            httpResponse.StatusCode = HttpStatusCode.OK;

            var rolesDtos = new List<RoleDto>
            {
                new(Guid.NewGuid())
                {
                    RoleName = "Project administrator",
                    AccessRights = new List<AccessRight>()
                    {
                        AccessRight.ManageParticipant
                    }
                }
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(rolesDtos));

            var roles = this.service.GetRoles().Result;
            Assert.That(roles.Count, Is.EqualTo(1));
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.GetRoles(), Throws.Exception);
        }

        [Test]
        public async Task VerifyGetRole()
        {
            var guid = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When(HttpMethod.Get, $"/Role/{guid}");
            request.Respond(_ => httpResponse);
            var role = await this.service.GetRole(guid);

            Assert.That(role, Is.Null);

            httpResponse.StatusCode = HttpStatusCode.OK;

            var roleDto = new RoleDto(guid)
            {
                RoleName = "Project administrator",
                AccessRights = new List<AccessRight>()
                {
                    AccessRight.ManageParticipant
                }
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(new List<EntityDto>(){ roleDto }));
            role = await this.service.GetRole(guid);

            Assert.That(role.Id, Is.EqualTo(guid));

            httpResponse.Content = new StringContent(string.Empty);

            Assert.That(async () => await this.service.GetRole(guid), Throws.Exception);
        }

        [Test]
        public async Task VerifyCreateRole()
        {
            var role = new Role()
            {
                RoleName = "Project administrator",
                AccessRights = new List<AccessRight>()
                {
                    AccessRight.ManageParticipant
                }
            };

            var httpResponse = new HttpResponseMessage();

            var entityRequestResponse = new EntityRequestResponseDto()
            {
                IsRequestSuccessful = false
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));

            var request = this.httpMessageHandler.When(HttpMethod.Post, "/Role/Create");
            request.Respond(_ => httpResponse);
            Assert.That(this.service.CreateRole(role).Result.IsRequestSuccessful, Is.False);

            entityRequestResponse.IsRequestSuccessful = true;

            entityRequestResponse.Entities = new List<EntityDto>()
            {
                role.ToDto()
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));
            var result = await this.service.CreateRole(role);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsRequestSuccessful, Is.True);
                Assert.That(result.Entity, Is.Not.Null);
            });
            
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.CreateRole(role), Throws.Exception);
        }

        [Test]
        public void VerifyDeleteRole()
        {
            var role = new Role(Guid.NewGuid());
            var httpResponse = new HttpResponseMessage();
            var requestResponse = new RequestResponseDto() { IsRequestSuccessful = true };
            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));
            var request = this.httpMessageHandler.When(HttpMethod.Delete, $"/Role/{role.Id}");
            request.Respond(_ => httpResponse);

            Assert.That(this.service.DeleteRole(role).Result.IsRequestSuccessful, Is.True);

            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.DeleteRole(role), Throws.Exception);
        }

        [Test]
        public void VerifyUpdateRole()
        {
            var role = new Role(Guid.NewGuid())
            {
                RoleName = "Project administrator",
                AccessRights = new List<AccessRight>()
                {
                    AccessRight.ManageParticipant
                }
            };

            var httpResponse = new HttpResponseMessage();
            var requestResponse = new EntityRequestResponseDto() { IsRequestSuccessful = false };
            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));

            var request = this.httpMessageHandler.When(HttpMethod.Put, $"/Role/{role.Id}");
            request.Respond(_ => httpResponse);
            Assert.That(this.service.UpdateRole(role).Result.IsRequestSuccessful, Is.False);

            requestResponse.IsRequestSuccessful = true;
            
            requestResponse.Entities = new List<EntityDto>()
            {
                role.ToDto()
            }; 
            
            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));

            Assert.That(this.service.UpdateRole(role).Result.IsRequestSuccessful, Is.True);
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.UpdateRole(role), Throws.Exception);
        }
    }
}
