// --------------------------------------------------------------------------------------------------------
// <copyright file="CometUploadTestFixture.cs" company="RHEA System S.A.">
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
    public class CometUploadTestFixture
    {
        private TestContext context;
        private ICometUploadViewModel viewModel;
        private Mock<ICometService> cometService;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.cometService = new Mock<ICometService>();
            this.viewModel = new CometUploadViewModel(this.cometService.Object);
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
            var renderer = this.context.RenderComponent<CometUpload>(parameters =>
            {
                parameters.Add(p => p.ViewModel, this.viewModel);
            });

            this.viewModel.InitializeProperties();

            var editForm = renderer.FindComponent<EditForm>();

            const string url = "http://localhost";
            this.viewModel.UrlTextHasChanged(url);
            Assert.That(this.viewModel.UploadData.Url, Is.EqualTo(url));
            this.viewModel.UploadData.UserName = "admin";
            this.viewModel.UploadData.Password = "password";

            this.cometService.Setup(x => x.Login(this.viewModel.UploadData)).ReturnsAsync(new CometAuthenticationResponse()
            {
                IsRequestSuccessful = false
            });

            await renderer.InvokeAsync(editForm.Instance.OnValidSubmit.InvokeAsync);
            Assert.That(renderer.Instance.ConnectButtonText, Is.EqualTo("Retry"));

            var sessionId = Guid.NewGuid();

            this.cometService.Setup(x => x.Login(this.viewModel.UploadData)).ReturnsAsync(new CometAuthenticationResponse()
            {
                IsRequestSuccessful = true,
                SessionId = sessionId
            });

            var modelGuid = Guid.NewGuid();
            var iterationGuid = Guid.NewGuid();

            var modelsData = new ModelsDataResponse()
            {
                AvailableModels =
                {
                    new EngineeringModelData()
                    {
                        EngineeringModelName = "LOFT",
                        EngineeringId = modelGuid,
                        Iterations = new List<IterationData>
                        {
                            new ()
                            {
                                IterationId = iterationGuid,
                                IterationName = "Iteration 1"
                            }
                        }
                    },
                    new EngineeringModelData()
                    {
                    EngineeringModelName = "LOFT",
                    EngineeringId = Guid.NewGuid(),
                    Iterations = new List<IterationData>
                    {
                        new ()
                        {
                            IterationId = Guid.NewGuid(),
                            IterationName = "Iteration 1"
                        }
                    }
                }
                }
            };

            Assert.Multiple(() =>
            {
                Assert.That(modelsData.AvailableModels.First(), Is.EqualTo(new EngineeringModelData()
                {
                    EngineeringId = modelGuid,
                    EngineeringModelName = "LOFT"
                }));

                Assert.That(modelsData.AvailableModels.First(), Is.Not.EqualTo(modelsData.AvailableModels.Last()));

                Assert.That(modelsData.AvailableModels.First().Iterations.First(), Is.EqualTo(new IterationData()
                {
                    IterationId = iterationGuid,
                    IterationName = "Iteration 1"
                }));

                Assert.That(modelsData.AvailableModels.First().Iterations.First(), Is.Not.EqualTo(modelsData.AvailableModels.Last()
                    .Iterations.First()));

                Assert.That(modelsData.AvailableModels.First(), Is.Not.EqualTo(null));
                Assert.That(modelsData.AvailableModels.First().Iterations.First(), Is.Not.EqualTo(null));
                Assert.That(modelsData.AvailableModels.GetHashCode(), Is.Not.EqualTo(0));
                Assert.That(modelsData.AvailableModels.First().Iterations.GetHashCode(), Is.Not.EqualTo(0));
            });

            this.cometService.Setup(x => x.GetAvailableEngineeringModels(It.IsAny<Guid>()))
                .ReturnsAsync(modelsData);

            await renderer.InvokeAsync(editForm.Instance.OnValidSubmit.InvokeAsync);
            Assert.That(this.viewModel.UploadData.Password, Is.Empty);

            this.viewModel.OnEventCallback = new EventCallbackFactory().Create(this.viewModel, async () =>
            {
                var upload = await this.cometService.Object.UploadIteration(sessionId, modelGuid, iterationGuid);
                
                if(!upload.IsRequestSuccessful)
                {
                    this.viewModel.HandleUploadFailure(upload);
                }
                else
                {
                    await this.viewModel.UploadSelectedIteration();
                    await this.viewModel.CometLogout();
                }
            });

            this.viewModel.SelectedEngineeringModelSetup = modelsData.AvailableModels.First();

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

        [Test]
        public async Task VerifyFileUploadBehavior()
        {
            var renderer = this.context.RenderComponent<CometUpload>(parameters =>
            {
                parameters.Add(p => p.ViewModel, this.viewModel);
            });

            this.viewModel.InitializeProperties();

            this.viewModel.UploadData.Url = "http://localhost";
            this.viewModel.UploadData.UserName = "admin";
            this.viewModel.UploadData.Password = "password";

            var inputFile = renderer.FindComponent<InputFile>();

            var browserFiles = new List<IBrowserFile>();
            var eventArgs = new InputFileChangeEventArgs(browserFiles);
            await renderer.InvokeAsync(async () => await inputFile.Instance.OnChange.InvokeAsync(eventArgs));

            Assert.That(this.viewModel.UploadData.Url, Is.Not.Empty);

            var browserFile = new Mock<IBrowserFile>();
            browserFiles.Add(browserFile.Object);

            eventArgs = new InputFileChangeEventArgs(browserFiles);
            await renderer.InvokeAsync(async () => await inputFile.Instance.OnChange.InvokeAsync(eventArgs));

            Assert.That(this.viewModel.UploadData.Url, Is.Empty);

            this.viewModel.UrlTextHasChanged("http://localhost");
            Assert.That(this.viewModel.BrowserFile, Is.Null);
        }
    }
}
