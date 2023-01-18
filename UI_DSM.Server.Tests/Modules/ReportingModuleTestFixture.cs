// --------------------------------------------------------------------------------------------------------
// <copyright file="ReportingModuleTestFixture.cs" company="RHEA System S.A.">
//  Copyright (c) 2023 RHEA System S.A.
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
    using Microsoft.AspNetCore.Http;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Modules;
    using UI_DSM.Server.Services.FileService;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Server.Validator;
    using UI_DSM.Shared.DTO.CometData;

    [TestFixture]
    public class ReportingModuleTestFixture
    {
        private ReportingModule module;
        private Mock<HttpContext> context;
        private Mock<HttpResponse> response;
        private Mock<IServiceProvider> serviceProvider;
        private Mock<HttpRequest> request;
        private Mock<IFileService> fileService;

        [SetUp]
        public void Setup()
        {
            this.fileService = new Mock<IFileService>();

            ModuleTestHelper.Setup<ReportingModule, CometAuthenticationData>(new CometAuthenticationDataValidator(), out this.context,
                out this.response, out this.request, out this.serviceProvider);

            this.serviceProvider.Setup(x => x.GetService(typeof(IFileService))).Returns(this.fileService.Object);

            this.module = new ReportingModule();
        }

        [Test]
        public async Task VerifyUploadReport()
        {
            var form = new Mock<IFormCollection>();
            var formFileCollection = new FormFileCollection();
            form.Setup(x => x.Files).Returns(formFileCollection);
            this.request.Setup(x => x.Form).Returns(form.Object);
            await this.module.UploadReport(this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);
            var formFile = new Mock<IFormFile>();
            formFile.Setup(x => x.ContentType).Returns("application/json");
            formFileCollection.Add(formFile.Object);
            await this.module.UploadReport(this.context.Object);

            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(2));
            formFile.Setup(x => x.ContentType).Returns("application/x-zip-compressed");
            await this.module.UploadReport(this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(2));
        }
    }
}
