// --------------------------------------------------------------------------------------------------------
// <copyright file="UserModuleTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Tests.Modules
{
    using Carter;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Net.Http.Headers;

    using Moq;
    using NUnit.Framework;

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Managers.UserManager;
    using UI_DSM.Server.Modules;
    using UI_DSM.Server.Services.SearchService;
    using UI_DSM.Server.Types;
    using UI_DSM.Server.Validator;
    using UI_DSM.Shared.DTO.UserManagement;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class UserModuleTestFixture
    {
        private UserModule module;
        private Mock<IEntityManager<UserEntity>> userManager;
        private Mock<IUserManager> iUserManager;
        private Mock<HttpContext> httpContext;
        private Mock<HttpResponse> httpResponse;
        private Mock<UserManager<User>> authenticationUserManager;
        private Mock<ISearchService> searchService;

        [SetUp]
        public void Setup()
        {
            var configuration = new Mock<IConfiguration>();
            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection.Setup(x => x["securityKey"]).Returns("securityKey1234567890");
            configurationSection.Setup(x => x["validIssuer"]).Returns("issuer");
            configurationSection.Setup(x => x["validAudience"]).Returns("audience");

            this.userManager = new Mock<IEntityManager<UserEntity>>();
            this.searchService = new Mock<ISearchService>();
            configuration.Setup(x => x.GetSection("JwtSettings")).Returns(configurationSection.Object);

            this.httpResponse = new Mock<HttpResponse>();
            this.httpResponse.SetupSet(response => response.StatusCode = It.IsAny<int>()).Verifiable();
            this.httpContext = new Mock<HttpContext>();

            var request = new Mock<HttpRequest>();
            request.Setup(x => x.HttpContext).Returns(this.httpContext.Object);

            this.httpContext.Setup(x => x.Request).Returns(request.Object);
            this.httpContext.Setup(x => x.Response).Returns(this.httpResponse.Object);
            this.httpResponse.Setup(x => x.HttpContext).Returns(this.httpContext.Object);

            var mockValidator = new Mock<IValidatorLocator>();
            mockValidator.Setup(x => x.GetValidator<AuthenticationDto>()).Returns(new AuthenticationDtoValidator());
            mockValidator.Setup(x => x.GetValidator<RegistrationDto>()).Returns(new RegistrationDtoValidator());

            var servicesMock = new Mock<IServiceProvider>();
            servicesMock.Setup(x => x.GetService(typeof(IValidatorLocator))).Returns(mockValidator.Object);
            this.httpContext.Setup(x => x.RequestServices).Returns(servicesMock.Object);

            var negotiator = new Mock<IResponseNegotiator>();
            negotiator.Setup(x => x.CanHandle(It.IsAny<MediaTypeHeaderValue>())).Returns(true);

            servicesMock.Setup(x => x.GetService(typeof(IEnumerable<IResponseNegotiator>))).Returns(new List<IResponseNegotiator>()
            {
                negotiator.Object
            });

            var headers = new Mock<IHeaderDictionary>();
            request.Setup(x => x.Headers).Returns(headers.Object);

            ModuleBase.RegisterModule<UserModule>();
            this.module = new UserModule(configuration.Object);

            this.iUserManager = new Mock<IUserManager>();
            this.authenticationUserManager = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
        }

        [Test]
        public async Task VerifyDeleteRole()
        {
            var guid = Guid.NewGuid();
            this.userManager.Setup(x => x.FindEntity(guid)).ReturnsAsync((UserEntity)null);

            var response = await this.module.DeleteEntity(this.userManager.Object, guid, this.searchService.Object, this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.False);
            this.httpResponse.VerifySet(x => x.StatusCode = 404, Times.Once);

            var user = new UserEntity(guid)
            {
                UserName = "user"
            };

            this.userManager.Setup(x => x.FindEntity(guid)).ReturnsAsync(user);
            this.userManager.Setup(x => x.DeleteEntity(user)).ReturnsAsync(EntityOperationResult<UserEntity>.Failed());
            response = await this.module.DeleteEntity(this.userManager.Object, guid, this.searchService.Object, this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.False);
            this.httpResponse.VerifySet(x => x.StatusCode = 500, Times.Once);

            this.userManager.Setup(x => x.DeleteEntity(user)).ReturnsAsync(EntityOperationResult<UserEntity>.Success(user));
            response = await this.module.DeleteEntity(this.userManager.Object, guid, this.searchService.Object, this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.True);
        }

        [Test]
        public async Task VerifyLogin()
        {
            var authentication = new AuthenticationDto
            {
                UserName = "admin",
                Password = ""
            };

            this.authenticationUserManager.Setup(x => x.FindByNameAsync(authentication.UserName)).ReturnsAsync((User)null);
            var response = await this.module.Login(this.authenticationUserManager.Object, authentication, this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.False);
            this.httpResponse.VerifySet(x => x.StatusCode = 422, Times.Once);

            authentication.Password = "pass";

            response = await this.module.Login(this.authenticationUserManager.Object, authentication, this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.False);
            this.httpResponse.VerifySet(x => x.StatusCode = 401, Times.Once);

            var user = new User
            {
                IsAdmin = false,
                UserName = "userName"
            };

            this.authenticationUserManager.Setup(x => x.FindByNameAsync(authentication.UserName)).ReturnsAsync(user);
            this.authenticationUserManager.Setup(x => x.CheckPasswordAsync(user, authentication.Password)).ReturnsAsync(false);

            response = await this.module.Login(this.authenticationUserManager.Object, authentication, this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.False);
            this.httpResponse.VerifySet(x => x.StatusCode = 401, Times.Exactly(2));

            this.authenticationUserManager.Setup(x => x.CheckPasswordAsync(user, authentication.Password)).ReturnsAsync(true);
            response = await this.module.Login(this.authenticationUserManager.Object, authentication, this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.True);
        }

        [Test]
        public async Task VerifyRegisterUser()
        {
            var registration = new RegistrationDto
            {
                UserName = "user",
                Password = "aPassword",
                ConfirmPassword = "aPasswor"
            };

            var response = await this.module.RegisterUser(this.iUserManager.Object, registration, this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.False);
            this.httpResponse.VerifySet(x => x.StatusCode = 422, Times.Once);

            registration.ConfirmPassword = registration.Password;

            this.iUserManager.Setup(x => x.RegisterUser(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(EntityOperationResult<UserEntity>.Failed("User already exist"));

            response = await this.module.RegisterUser(this.iUserManager.Object, registration, this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.False);
            this.httpResponse.VerifySet(x => x.StatusCode = 422, Times.Exactly(2));
            
            var user = new UserEntity()
            {
                Id = Guid.NewGuid(),
                UserName = registration.UserName
            };

            this.iUserManager.Setup(x => x.RegisterUser(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(EntityOperationResult<UserEntity>.Success(user));

            response = await this.module.RegisterUser(this.iUserManager.Object, registration, this.httpContext.Object);

            Assert.That(response.IsRequestSuccessful, Is.True);
            this.httpResponse.VerifySet(x => x.StatusCode = 201, Times.Once);
        }
    }
}
