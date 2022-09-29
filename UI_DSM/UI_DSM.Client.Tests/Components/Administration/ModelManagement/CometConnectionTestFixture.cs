// --------------------------------------------------------------------------------------------------------
// <copyright file="CometConnectionTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.Administration.ModelManagement
{
    using Bunit;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components.Administration.ModelManagement;
    using UI_DSM.Client.Services.Administration.CometService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components.Administration.ModelManagement;
    using UI_DSM.Shared.DTO.CometData;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class CometConnectionTestFixture
    {
        private TestContext context;
        private ICometConnectionViewModel viewModel;
        private Mock<ICometService> cometService;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.cometService = new Mock<ICometService>();
            this.viewModel = new CometConnectionViewModel(this.cometService.Object);
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
            this.viewModel.Dispose();
        }

        [Test]
        public async Task VerifyComponent()
        {
            var renderer = this.context.RenderComponent<CometConnection>(parameters =>
            {
                parameters.Add(p => p.ViewModel, this.viewModel);
            });

            this.viewModel.InitializeProperties();

            var editForm = renderer.FindComponent<EditForm>();

            this.viewModel.AuthenticationData.Url = "http://localhost";
            this.viewModel.AuthenticationData.UserName = "admin";
            this.viewModel.AuthenticationData.Password = "password";

            this.cometService.Setup(x => x.Login(this.viewModel.AuthenticationData)).ReturnsAsync(new CometAuthenticationResponse()
            {
                IsRequestSuccessful = false
            });

            await renderer.InvokeAsync(editForm.Instance.OnValidSubmit.InvokeAsync);
            Assert.That(renderer.Instance.ConnectButtonText, Is.EqualTo("Retry"));

            var sessionId = Guid.NewGuid();

            this.cometService.Setup(x => x.Login(this.viewModel.AuthenticationData)).ReturnsAsync(new CometAuthenticationResponse()
            {
                IsRequestSuccessful = true,
                SessionId = sessionId
            });

            var modelGuid = Guid.NewGuid();
            var iterationGuid = Guid.NewGuid();

            var modelsData = new ModelsDataResponse()
            {
                ModelNames = new Dictionary<Guid, string>(),
                AvailableModels = new Dictionary<Guid, List<Tuple<Guid, string>>>()
            };

            modelsData.ModelNames[modelGuid] = "LOFT";

            modelsData.AvailableModels[modelGuid] = new List<Tuple<Guid, string>>
            {
                new (iterationGuid, "Iteration 1")
            };

            this.cometService.Setup(x => x.GetAvailableEngineeringModels(It.IsAny<Guid>()))
                .ReturnsAsync(modelsData);

            await renderer.InvokeAsync(editForm.Instance.OnValidSubmit.InvokeAsync);
            Assert.That(this.viewModel.AuthenticationData.Password, Is.Empty);

            this.viewModel.OnEventCallback = new EventCallbackFactory().Create(this.viewModel, async () =>
            {
                var upload = await this.cometService.Object.UploadIteration(sessionId, modelGuid, iterationGuid);
                
                if(!upload.IsRequestSuccessful)
                {
                    this.viewModel.HandleUploadFailure(upload);
                }
                else
                {
                    await this.viewModel.CometLogout();
                }
            });

            this.viewModel.SelectedEngineeringModelSetup = new Tuple<Guid, string>(modelGuid, "LOFT");

            this.cometService.Setup(x => x.UploadIteration(sessionId, modelGuid, iterationGuid))
                .ReturnsAsync(new ModelUploadResponse()
                {
                    IsRequestSuccessful = false,
                    Errors = new List<string>{"Error during uploading"}
                });

            await renderer.InvokeAsync(this.viewModel.OnEventCallback.InvokeAsync);
            Assert.That(renderer.Instance.UploadText, Is.EqualTo("Retry..."));

            this.cometService.Setup(x => x.UploadIteration(sessionId, modelGuid, iterationGuid))
                .ReturnsAsync(new ModelUploadResponse()
                {
                    IsRequestSuccessful = true
                });

            await renderer.InvokeAsync(this.viewModel.OnEventCallback.InvokeAsync);
        }
    }
}
