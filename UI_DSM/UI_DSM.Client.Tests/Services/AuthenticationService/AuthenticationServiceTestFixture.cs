// --------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Services.AuthenticationService
{
    using System.Net;
    using System.Text.Json;

    using Blazored.SessionStorage;

    using Moq;

    using NUnit.Framework;

    using RichardSzalay.MockHttp;

    using UI_DSM.Client.Services.AuthenticationService;
    using UI_DSM.Shared.DTO.UserManagement;

    [TestFixture]
    public class AuthenticationServiceTestFixture
    {
        private AuthenticationService service;
        private MockHttpMessageHandler httpMessageHandler;
        private Mock<AuthenticationProvider> authenticationProvider;
        private Mock<ISessionStorageService> sessionStorage;

        [SetUp]
        public void Setup()
        {
            this.httpMessageHandler = new MockHttpMessageHandler();
            this.sessionStorage = new Mock<ISessionStorageService>();
            this.authenticationProvider = new Mock<AuthenticationProvider>(new HttpClient(), this.sessionStorage.Object);
            var httpClient = this.httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/api");
            this.service = new AuthenticationService(httpClient, this.authenticationProvider.Object, this.sessionStorage.Object);
        }

        [Test]
        public void VerifyLogin()
        {
            var mockRequest = this.httpMessageHandler.When("/User/Login");

            var httpResponseMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = new StringContent(JsonSerializer.Serialize(new AuthenticationResponseDto()
                {
                    Errors = new List<string>()
                    {
                        "Unauthorized User"
                    }
                }))
            };

            mockRequest.Respond(_ => httpResponseMessage);

            var authentication = new AuthenticationDto()
            {
                UserName = "admin",
                Password = "password"
            };

            Assert.That(this.service.Login(authentication).Result.IsRequestSuccessful, Is.False);

            httpResponseMessage.StatusCode = HttpStatusCode.OK;

            httpResponseMessage.Content = new StringContent(JsonSerializer.Serialize(new AuthenticationResponseDto()
            {
                IsRequestSuccessful = true,
                Token = Guid.NewGuid().ToString()
            }));

            Assert.That(this.service.Login(authentication).Result.IsRequestSuccessful, Is.True);
        }

        [Test]
        public void VerifyLogout()
        {
            this.sessionStorage.Setup(x => x.RemoveItemAsync(AuthenticationProvider.SessionStorageKey, null));
            Assert.That(async () => await this.service.Logout(), Throws.Nothing);

            this.sessionStorage.Verify(x => x.RemoveItemAsync(AuthenticationProvider.SessionStorageKey, null), Times.Once);
        }
    }
}
