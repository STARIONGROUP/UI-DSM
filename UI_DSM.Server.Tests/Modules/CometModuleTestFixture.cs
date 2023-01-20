// --------------------------------------------------------------------------------------------------------
// <copyright file="CometModuleTestFixture.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Server.Tests.Modules
{
    using System.Text.Json;

    using Carter;

    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Enumerator;
    using UI_DSM.Server.Modules;
    using UI_DSM.Server.Services.CometService;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Server.Validator;
    using UI_DSM.Shared.DTO.CometData;

    [TestFixture]
    public class CometModuleTestFixture
    {
        private CometModule module;
        private Mock<ICometService> cometService;
        private Mock<HttpContext> context;
        private Mock<HttpResponse> response;
        private Mock<IServiceProvider> serviceProvider;
        private Mock<HttpRequest> request;

        [SetUp]
        public void Setup()
        {
            this.cometService = new Mock<ICometService>();

            ModuleTestHelper.Setup<CometModule, CometAuthenticationData>(new CometAuthenticationDataValidator(), out this.context,
                out this.response, out this.request, out this.serviceProvider);

            this.module = new CometModule(this.cometService.Object);
        }

        [Test]
        public async Task VerifyLogin()
        {
            var authenticationData = new CometAuthenticationData();
            await this.module.Login(authenticationData, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400,Times.Once);
            
            authenticationData.UserName = "user";
            authenticationData.Password = "pass";
            authenticationData.Url = "http://localhost";
            var authenticationReply = new Tuple<AuthenticationStatus, Guid>(AuthenticationStatus.Fail, Guid.Empty);
            this.cometService.Setup(x => x.Login(authenticationData)).ReturnsAsync(authenticationReply);
            await this.module.Login(authenticationData, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            authenticationReply = new Tuple<AuthenticationStatus, Guid>(AuthenticationStatus.ServerFailure, Guid.Empty);
            this.cometService.Setup(x => x.Login(authenticationData)).ReturnsAsync(authenticationReply);
            await this.module.Login(authenticationData, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 500, Times.Once);

            authenticationReply = new Tuple<AuthenticationStatus, Guid>(AuthenticationStatus.Success, Guid.NewGuid());
            this.cometService.Setup(x => x.Login(authenticationData)).ReturnsAsync(authenticationReply);
            await this.module.Login(authenticationData, this.context.Object);
        }

        [Test]
        public async Task VerifyGetAvailableEngineeringModels()
        {
            var sessionId = Guid.NewGuid();

            this.cometService.Setup(x => x.GetAvailableEngineeringModel(sessionId)).Throws<Exception>();
            await this.module.GetAvailableEngineeringModels(sessionId,this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 404, Times.Once);

            var engineeringModels = new List<EngineeringModelSetup>
            {
                new ()
                {
                    Iid = Guid.NewGuid(),
                    Name = "Model",
                    IterationSetup =
                    {
                        new IterationSetup()
                        {
                            IsDeleted = true
                        },
                        new IterationSetup(),
                        new IterationSetup()
                        {
                            FrozenOn = DateTime.UtcNow,
                            IterationNumber = 1,
                            IterationIid = Guid.NewGuid()
                        }
                    }
                }
            };

            this.cometService.Setup(x => x.GetAvailableEngineeringModel(sessionId)).Returns(engineeringModels);
            await this.module.GetAvailableEngineeringModels(sessionId, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 404, Times.Once);
        }

        [Test]
        public async Task VerifyLogout()
        {
            var sessionId = Guid.NewGuid();
            await this.module.Logout(sessionId, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 404, Times.Never);

            this.cometService.Setup(x => x.Close(sessionId)).Throws<Exception>();
            await this.module.Logout(sessionId, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 404, Times.Once);
        }

        [Test]
        public async Task VerifyUploadIteration()
        {
            var validatorService = new Mock<IValidatorLocator>();
            validatorService.Setup(x => x.GetValidator<ModelUploadData>()).Returns(new ModelUploadDataValidator());
            this.serviceProvider.Setup(x => x.GetService(typeof(IValidatorLocator))).Returns(validatorService.Object);

            var sessionId = Guid.NewGuid();
            var modelUpload = new ModelUploadData();
            await this.module.UploadIteration(sessionId, modelUpload, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            modelUpload.ModelId = Guid.NewGuid();
            modelUpload.IterationId = Guid.NewGuid();

            var iterationSetup = new IterationSetup();
            
            this.cometService.Setup(x => x.GetIterationSetup(sessionId, modelUpload.ModelId, modelUpload.IterationId))
                .Returns(iterationSetup);

            var iteration = new Iteration();
            this.cometService.Setup(x => x.ReadIteration(sessionId, iterationSetup)).ReturnsAsync(iteration);
            this.cometService.Setup(x => x.CreateAnnexC3File(iteration)).ReturnsAsync("filename.zip");

            await this.module.UploadIteration(sessionId, modelUpload, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            this.cometService.Setup(x => x.GetIterationSetup(sessionId, modelUpload.ModelId, modelUpload.IterationId))
                .Throws<Exception>();

            await this.module.UploadIteration(sessionId, modelUpload, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(2));
        }

        [Test]
        public async Task VerifyUploadAnnexC3()
        {
            var formColletion = new Mock<IFormCollection>();
            var formFileCollection = new FormFileCollection();
            this.request.Setup(x => x.Form).Returns(formColletion.Object);
            formColletion.Setup(x => x.Files).Returns(formFileCollection);
            await this.module.UploadAnnexC3(this.context.Object); 
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            var formFile = new Mock<IFormFile>();
            formFile.Setup(x => x.ContentType).Returns("application/json");
            formFileCollection.Add(formFile.Object);
            await this.module.UploadAnnexC3(this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(2));
            
            formFile.Setup(x => x.ContentType).Returns("application/x-zip-compressed");
            var stringValues = new StringValues();
            formColletion.Setup(x => x.TryGetValue("authenticationData", out stringValues)).Returns(false);
            await this.module.UploadAnnexC3(this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(3));

            var authenticationData = new CometAuthenticationData()
            {
                UploadFromFile = false,
                UserName = "admin",
                Password = "pass"
            };

            stringValues = new StringValues(new []{JsonSerializer.Serialize(authenticationData)});
            formColletion.Setup(x => x.TryGetValue("authenticationData", out stringValues)).Returns(true);
            this.response.Setup(x => x.StatusCode).Returns(200);

            await this.module.UploadAnnexC3(this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(4));
            authenticationData.UploadFromFile = true;
            stringValues = new StringValues(new[] { JsonSerializer.Serialize(authenticationData) });
            formColletion.Setup(x => x.TryGetValue("authenticationData", out stringValues)).Returns(true);

            this.cometService.Setup(x => x.ReadAnnexC3File(It.IsAny<IFormFile>(), It.IsAny<CometAuthenticationData>()))
                .ReturnsAsync(new Tuple<AuthenticationStatus, Guid>(AuthenticationStatus.Fail, Guid.Empty));

            await this.module.UploadAnnexC3(this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            this.cometService.Setup(x => x.ReadAnnexC3File(It.IsAny<IFormFile>(), It.IsAny<CometAuthenticationData>()))
                .ReturnsAsync(new Tuple<AuthenticationStatus, Guid>(AuthenticationStatus.ServerFailure, Guid.Empty));

            await this.module.UploadAnnexC3(this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 500, Times.Once);

            this.cometService.Setup(x => x.ReadAnnexC3File(It.IsAny<IFormFile>(), It.IsAny<CometAuthenticationData>()))
                .ReturnsAsync(new Tuple<AuthenticationStatus, Guid>(AuthenticationStatus.Success, Guid.NewGuid()));
            
            await this.module.UploadAnnexC3(this.context.Object);
        }
    }
}
