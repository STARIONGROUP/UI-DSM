// --------------------------------------------------------------------------------------------------------
// <copyright file="UserControllerTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Tests.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Controllers;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.DTO.UserManagement;

    [TestFixture]
    public class UserControllerTestFixture
    {
        private UserController controller;
        private Mock<IConfigurationSection> configurationSection;
        private Mock<UserManager<User>> userManager;

        [SetUp]
        public void Setup()
        {
            var configuration = new Mock<IConfiguration>();
            this.configurationSection = new Mock<IConfigurationSection>();
            this.userManager = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
            configuration.Setup(x => x.GetSection("JwtSettings")).Returns(this.configurationSection.Object);
            this.controller = new UserController(this.userManager.Object, configuration.Object);
        }

        [Test]
        public void VerifyDeleteUser()
        {
            var userId = Guid.NewGuid().ToString();

            this.userManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((User)null);

            Assert.That(this.controller.DeleteUser(userId).Result.GetType(), Is.EqualTo(typeof(NotFoundObjectResult)));

            var user = new User
            {
                IsAdmin = true,
                Id = userId
            };

            this.userManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            Assert.That(this.controller.DeleteUser(userId).Result.GetType(), Is.EqualTo(typeof(BadRequestObjectResult)));

            user.IsAdmin = false;
            Assert.That(this.controller.DeleteUser(userId).Result.GetType(), Is.EqualTo(typeof(OkObjectResult)));
        }

        [Test]
        public void VerifyLogin()
        {
            var authentication = new AuthenticationDto
            {
                UserName = "admin",
                Password = "password"
            };

            this.userManager.Setup(x => x.FindByNameAsync(authentication.UserName)).ReturnsAsync((User)null);

            Assert.That(this.controller.Login(authentication).Result.GetType(), Is.EqualTo(typeof(UnauthorizedObjectResult)));

            var user = new User
            {
                IsAdmin = false,
                UserName = "userName"
            };

            this.userManager.Setup(x => x.FindByNameAsync(authentication.UserName)).ReturnsAsync(user);
            this.userManager.Setup(x => x.CheckPasswordAsync(user, authentication.Password)).ReturnsAsync(false);

            Assert.That(this.controller.Login(authentication).Result.GetType(), Is.EqualTo(typeof(UnauthorizedObjectResult)));
            this.userManager.Setup(x => x.CheckPasswordAsync(user, authentication.Password)).ReturnsAsync(true);
            this.configurationSection.Setup(x => x["securityKey"]).Returns("securityKey1234567890");
            this.configurationSection.Setup(x => x["validIssuer"]).Returns("issuer");
            this.configurationSection.Setup(x => x["validAudience"]).Returns("audience");

            var result = this.controller.Login(authentication).Result;
            Assert.That(result.GetType(), Is.EqualTo(typeof(OkObjectResult)));

            user.IsAdmin = true;

            var okResponse = this.controller.Login(authentication).Result as OkObjectResult;
            Assert.That(((AuthenticationResponseDto)okResponse!.Value!).IsRequestSuccessful, Is.True);
        }

        [Test]
        public void VerifyRegisterUser()
        {
            var registration = new RegistrationDto
            {
                UserName = "user",
                Password = "aPassword",
                ConfirmPassword = "aPassword"
            };

            this.userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError
                {
                    Description = "User already exist"
                }));

            var registrationResult = this.controller.RegisterUser(registration).Result as BadRequestObjectResult;
            Assert.That(((RegistrationResponseDto)registrationResult!.Value!).IsRequestSuccessful, Is.False);
            Assert.That(((RegistrationResponseDto)registrationResult!.Value!).Errors.Count, Is.EqualTo(1));

            this.userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var user = new User()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = registration.UserName
            };

            this.userManager.Setup(x => x.FindByNameAsync(user.UserName)).ReturnsAsync(user);

            var okRegistrationResult = this.controller.RegisterUser(registration).Result as ObjectResult;
            Assert.That(okRegistrationResult!.StatusCode, Is.EqualTo(201));
        }
    }
}
