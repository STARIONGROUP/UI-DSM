// --------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationProviderTestFixture.cs" company="RHEA System S.A.">
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
    using Blazored.SessionStorage;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Services.AuthenticationService;

    [TestFixture]
    public class AuthenticationProviderTestFixture
    {
        private AuthenticationProvider provider;
        private Mock<ISessionStorageService> sessionStorageService;

        [SetUp]
        public void Setup()
        {
            this.sessionStorageService = new Mock<ISessionStorageService>();
            this.provider = new AuthenticationProvider(new HttpClient(), this.sessionStorageService.Object);
        }

        [Test]
        public void VerifyGetAuthenticationState()
        {
            this.sessionStorageService.Setup(x => x.GetItemAsync<string>(AuthenticationProvider.SessionStorageKey, null))
                .ReturnsAsync((string)null);

            Assert.That(this.provider.GetAuthenticationStateAsync().Result.User.Claims.Count(), Is.Zero);

            var generatedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9" +
                                 ".eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbmlzdHJhdG9yIiwiaXNzIjoiVUktRFNNLUFwaSIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjcwMzIifQ" +
                                 ".bASataGQs30uqkfBV2-tv6-poYzuBbUj3-ja_PBrnm0";

            this.sessionStorageService.Setup(x => x.GetItemAsync<string>(AuthenticationProvider.SessionStorageKey, null))
                .ReturnsAsync(generatedToken);

            var user = this.provider.GetAuthenticationStateAsync().Result.User;
            Assert.That(user.Claims.Count(), Is.EqualTo(4));
            Assert.That(user.IsInRole("Administrator"), Is.True);
        }
    }
}
