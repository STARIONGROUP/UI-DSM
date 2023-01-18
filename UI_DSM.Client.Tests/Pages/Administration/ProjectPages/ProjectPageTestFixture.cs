// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectPageTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Pages.Administration.ProjectPages
{
    using AppComponents;

    using Bunit;
    using Bunit.TestDoubles;

    using CDP4Common.CommonData;
    using CDP4Common.SiteDirectoryData;

    using Microsoft.AspNetCore.Components.Forms;
    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components.Administration.ProjectManagement;
    using UI_DSM.Client.Pages.Administration.ProjectPages;
    using UI_DSM.Client.Services.Administration.ParticipantService;
    using UI_DSM.Client.Services.Administration.ProjectService;
    using UI_DSM.Client.Services.Administration.RoleService;
    using UI_DSM.Client.Services.ArtifactService;
    using UI_DSM.Client.Services.ReportService;
    using UI_DSM.Client.Services.ThingService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components.Administration.ModelManagement;
    using UI_DSM.Client.ViewModels.Pages.Administration.ProjectPages;
    using UI_DSM.Shared.DTO.CometData;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    using Participant = UI_DSM.Shared.Models.Participant;
    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ProjectPageTestFixture
    {
        private TestContext context;
        private IProjectPageViewModel viewModel;
        private Mock<IProjectService> projectService;
        private Mock<IParticipantService> participantService;
        private Mock<IRoleService> roleService;
        private Mock<IArtifactService> artifactService;
        private Mock<ICometUploadViewModel> cometConnexionViewModel;
        private Mock<IThingService> thingService;
        private Mock<IReportService> reportService;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.projectService = new Mock<IProjectService>();
            this.participantService = new Mock<IParticipantService>();
            this.roleService = new Mock<IRoleService>();
            this.cometConnexionViewModel = new Mock<ICometUploadViewModel>();
            this.artifactService = new Mock<IArtifactService>();
            this.thingService = new Mock<IThingService>();
            this.reportService = new Mock<IReportService>();

            this.viewModel = new ProjectPageViewModel(this.projectService.Object, this.participantService.Object, this.roleService.Object, 
                this.cometConnexionViewModel.Object, this.artifactService.Object, this.thingService.Object, this.reportService.Object);

            this.context.AddTestAuthorization();
            this.context.Services.AddSingleton(this.viewModel);
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifyRenderer()
        {
            var projectGuid = Guid.NewGuid();
            this.projectService.Setup(x => x.GetProject(projectGuid, 0)).ReturnsAsync((Project)null);

            this.thingService.Setup(x => x.GetThings(It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>(), ClassKind.DomainOfExpertise))
                .ReturnsAsync(new List<Thing>
                {
                        new DomainOfExpertise(){Iid = Guid.NewGuid(), ShortName = "AOGNC"},
                        new DomainOfExpertise(){Iid = Guid.NewGuid(), ShortName = "THE"},
                        new DomainOfExpertise(){Iid = Guid.NewGuid(), ShortName = "SYS"}
                });

            var renderer = this.context.RenderComponent<ProjectPage>(parameters => { parameters.Add(p => p.ProjectId, projectGuid.ToString()); });

            Assert.That(() => renderer.FindComponent<ProjectDetails>(), Throws.Exception);

            var project = new Project(projectGuid)
            {
                ProjectName = "Project"
            };

            var participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid())
                {
                    AccessRights = { AccessRight.ProjectManagement }
                }
            };

            this.projectService.Setup(x => x.GetProject(projectGuid, 0)).ReturnsAsync(project);
            this.participantService.Setup(x => x.GetParticipantsOfProject(projectGuid, 0)).ReturnsAsync(new List<Participant> { participant });
            this.participantService.Setup(x => x.GetCurrentParticipant(projectGuid)).ReturnsAsync(participant);
            this.artifactService.Setup(x => x.GetArtifactsOfProject(projectGuid, 0)).ReturnsAsync(new List<Artifact>());

            await this.viewModel.OnInitializedAsync(projectGuid);
            renderer.Render();

            Assert.That(this.viewModel.ProjectDetailsViewModel.Project, Is.Not.Null);

            this.roleService.Setup(x => x.GetRoles(0)).ReturnsAsync(new List<Role>()
            {
                new(Guid.NewGuid())
                {
                    RoleName = "Reviewer"
                }
            });

            this.participantService.Setup(x => x.GetAvailableUsersForCreation(projectGuid)).ReturnsAsync(new List<UserEntity>()
            {
                new(Guid.NewGuid())
                {
                    UserName = "user"
                }
            });

            await renderer.InvokeAsync(async () => await this.viewModel.OpenCreateParticipantPopup());
            Assert.That(this.viewModel.IsOnCreationMode, Is.True);

            this.participantService.Setup(x => x.CreateParticipant(projectGuid, It.IsAny<Participant>()))
                .ReturnsAsync(EntityRequestResponse<Participant>.Fail(new List<string>()));

            await renderer.InvokeAsync(() => this.viewModel.ParticipantCreationViewModel.OnValidSubmit.InvokeAsync());

            Assert.That(this.viewModel.IsOnCreationMode, Is.True);

            this.participantService.Setup(x => x.CreateParticipant(projectGuid, It.IsAny<Participant>()))
                .ReturnsAsync(EntityRequestResponse<Participant>.Success(new Participant()));

            await renderer.InvokeAsync(() => this.viewModel.ParticipantCreationViewModel.OnValidSubmit.InvokeAsync());

            Assert.Multiple(() =>
            {
                Assert.That(this.viewModel.IsOnCreationMode, Is.False);
                Assert.That(project.Participants, Has.Count.EqualTo(2));
            });

            var uploadBudgetButton = renderer.FindComponents<AppButton>().Last();
            await renderer.InvokeAsync(uploadBudgetButton.Instance.Click.InvokeAsync);
            Assert.That(this.viewModel.IsOnBudgetUploadMode, Is.True);

            this.reportService.Setup(x => x.UploadReport(It.IsAny<string>(), It.IsAny<IBrowserFile>()))
                .ReturnsAsync(new CometAuthenticationResponse() { Errors = { "Failed to upload" } });

            await renderer.InvokeAsync(this.viewModel.BudgetUploadViewModel.OnSubmit.InvokeAsync);
            
            Assert.Multiple(() =>
            {
                Assert.That(this.viewModel.IsOnBudgetUploadMode, Is.True);
                Assert.That(this.viewModel.ErrorMessageViewModel.Errors, Has.Count.EqualTo(1));
            });

            this.artifactService.Setup(x => x.UploadBudget(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
                .ReturnsAsync(EntityRequestResponse<BudgetTemplate>.Fail(new List<string> { "Already exist" }));

            this.reportService.Setup(x => x.UploadReport(It.IsAny<string>(), It.IsAny<IBrowserFile>()))
                .ReturnsAsync(new CometAuthenticationResponse() { IsRequestSuccessful = true});

            await renderer.InvokeAsync(this.viewModel.BudgetUploadViewModel.OnSubmit.InvokeAsync);

            Assert.Multiple(() =>
            {
                Assert.That(this.viewModel.IsOnBudgetUploadMode, Is.True);
                Assert.That(this.viewModel.ErrorMessageViewModel.Errors, Has.Count.EqualTo(1));
            });

            this.artifactService.Setup(x => x.UploadBudget(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
                .ReturnsAsync(EntityRequestResponse<BudgetTemplate>.Success(new BudgetTemplate()));

            await renderer.InvokeAsync(this.viewModel.BudgetUploadViewModel.OnSubmit.InvokeAsync);
            
            Assert.Multiple(() =>
            {
                Assert.That(this.viewModel.IsOnBudgetUploadMode, Is.False);
                Assert.That(this.viewModel.ProjectDetailsViewModel.Project.Artifacts.OfType<BudgetTemplate>().ToList(), Has.Count.EqualTo(1));
            });
        }
    }
}
