// --------------------------------------------------------------------------------------------------------
// <copyright file="UserServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Services.Administration.UserService
{
    using System.Net;

    using NUnit.Framework;

    using RichardSzalay.MockHttp;

    using UI_DSM.Client.Services;
    using UI_DSM.Client.Services.Administration.UserService;
    using UI_DSM.Client.Services.JsonDeserializerProvider;
    using UI_DSM.Serializer.Json;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.DTO.UserManagement;

    [TestFixture]
    public class UserServiceTestFixture
    {
        private UserService service;
        private MockHttpMessageHandler httpMessageHandler;
        private IJsonService jsonService;

        [SetUp]
        public void Setup()
        {
            this.httpMessageHandler = new MockHttpMessageHandler();
            var httpClient = this.httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/api");

            ServiceBase.RegisterService<UserService>();
            this.jsonService = new JsonService(new JsonDeserializer(), new JsonSerializer());
            this.service = new UserService(httpClient, this.jsonService);
        }

        [Test]
        public void VerifyGetUsers()
        {
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.Forbidden;

            var request = this.httpMessageHandler.When("/User");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.GetUsers(), Throws.Exception);

            httpResponse.StatusCode = HttpStatusCode.OK;
            
            var users = new List<UserEntityDto>
            {
                new ()
                {
                    UserName = "user1",
                    Id = Guid.NewGuid()
                },
                new ()
                {
                    UserName = "user2",
                    Id = Guid.NewGuid()
                }
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(users));
            var retrievedUsers = this.service.GetUsers().Result;

            Assert.That(retrievedUsers.Count, Is.EqualTo(users.Count));
        }

        [Test]
        public void VerifyRegisterUser()
        {
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.BadRequest;

            var request = this.httpMessageHandler.When("/User/Register");
            request.Respond(_ => httpResponse);

            var registrationResponse = new RegistrationResponseDto()
            {
                IsRequestSuccessful = false
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(registrationResponse));

            var registrationResult = this.service.RegisterUser(new RegistrationDto()).Result;
            Assert.That(registrationResult.IsRequestSuccessful, Is.False);

            httpResponse.StatusCode = HttpStatusCode.Created;

            registrationResponse.IsRequestSuccessful = true;

            registrationResponse.CreatedUserEntity = new UserEntityDto()
            {
                UserName = "user",
                IsAdmin = false,
                Id = Guid.NewGuid()
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(registrationResponse));
            registrationResult = this.service.RegisterUser(new RegistrationDto()).Result;
            Assert.That(registrationResult.IsRequestSuccessful, Is.True);
            Assert.That(registrationResult.CreatedUserEntity.Id, Is.EqualTo(registrationResponse.CreatedUserEntity.Id));
        }

        [Test]
        public void VerifyDeleteUser()
        {
            var userToDelete = new UserEntityDto()
            {
                UserName = "user",
                IsAdmin = true,
                Id = Guid.NewGuid()
            };

            var deleteResponse = this.service.DeleteUser(userToDelete).Result;
            Assert.That(deleteResponse.IsRequestSuccessful, Is.False);
            Assert.That(deleteResponse.Errors, Is.Not.Empty);

            userToDelete.IsAdmin = false;
            var request = this.httpMessageHandler.When(HttpMethod.Delete, $"/User/{userToDelete.Id}");

            var httpResponse = new HttpResponseMessage();
            request.Respond(_ => httpResponse);

            httpResponse.StatusCode = HttpStatusCode.BadRequest;
            var requestResponse = new RequestResponseDto();

            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));

            deleteResponse = this.service.DeleteUser(userToDelete).Result;
            Assert.That(deleteResponse.IsRequestSuccessful, Is.False);

            requestResponse.IsRequestSuccessful = true;

            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));

            deleteResponse = this.service.DeleteUser(userToDelete).Result;
            Assert.That(deleteResponse.IsRequestSuccessful, Is.True);
        }
    }
}
