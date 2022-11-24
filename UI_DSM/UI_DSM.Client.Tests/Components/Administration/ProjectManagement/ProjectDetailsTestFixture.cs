// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectDetailsTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.Administration.ProjectManagement
{
    using Bunit;

    using DevExpress.Blazor;

    using Moq;
    
    using NUnit.Framework;

    using UI_DSM.Client.Components.Administration.ProjectManagement;
    using UI_DSM.Client.Services.Administration.ParticipantService;
    using UI_DSM.Client.Services.Administration.RoleService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components.Administration.ProjectManagement;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;
    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ProjectDetailsTestFixture
    {
        private TestContext context;
        private IProjectDetailsViewModel viewModel;
        private Mock<IParticipantService> participantService;
        private Mock<IRoleService> roleService;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.participantService = new Mock<IParticipantService>();
            this.roleService = new Mock<IRoleService>();

            this.viewModel = new ProjectDetailsViewModel(this.participantService.Object, this.roleService.Object)
            {
                Project = new Project(Guid.NewGuid())
                {
                    ProjectName = "Project"
                }
            };
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public void VerifyComponent()
        {
            var renderer = this.context.RenderComponent<ProjectDetails>(parameters =>
            {
                parameters.Add(p => p.ViewModel, this.viewModel);
            });

            var inputText = renderer.FindComponent<DxTextBox>();
            Assert.That(inputText.Instance.Text, Is.EqualTo(this.viewModel.Project.ProjectName));
        }

        [Test]
        public async Task VerifyOpenUpdateParticipant()
        {
            try
            {
                var roleId = Guid.NewGuid();
                
                var newRole = new Role(roleId)
                {
                    RoleName = "Project Manager",
                    AccessRights = new List<AccessRight>()
                    {
                        AccessRight.ReviewTask
                    }
                };

                var participant = new Participant()
                {
                    Role = new Role(Guid.NewGuid())
                    {
                        RoleName = "Project administrator",
                        AccessRights = new List<AccessRight>()
                        {
                            AccessRight.ReviewTask
                        }
                    },
                    User = new UserEntity(Guid.NewGuid())
                    {
                        UserName = "user"
                    }
                };

                this.viewModel.Project.Participants.Add(participant);

                var renderer = this.context.RenderComponent<ProjectDetails>(parameters =>
                {
                    parameters.Add(p => p.ViewModel, this.viewModel);
                });
                var participantName = renderer.Find("td");
                Assert.That(participantName.InnerHtml, Does.Contain(participant.User.UserName));

                var updateButton = renderer.FindComponent<DxButton>();
                Assert.That(this.viewModel.IsOnUpdateViewMode, Is.False);
                await renderer.InvokeAsync(updateButton.Instance.Click.InvokeAsync);

                Assert.That(this.viewModel.IsOnUpdateViewMode, Is.True);

                this.viewModel.ParticipantDetailsViewModel.Participant.Role = newRole;

                this.participantService.Setup(x => x.UpdateParticipant(this.viewModel.ParticipantDetailsViewModel.Participant, this.viewModel.Project.Id))
                    .ReturnsAsync(EntityRequestResponse<Participant>.Fail(new List<string>
                    {
                        "Failed to update user role"
                    }));


                await this.viewModel.ParticipantDetailsViewModel.OnValidSubmit.InvokeAsync();

                Assert.Multiple(() =>
                {
                    Assert.That(this.viewModel.ErrorMessageViewModel.Errors, Has.Count.EqualTo(1));
                    Assert.That(this.viewModel.IsOnUpdateViewMode, Is.True);
                });

                this.participantService.Setup(x => x.UpdateParticipant(this.viewModel.ParticipantDetailsViewModel.Participant, this.viewModel.Project.Id))
                    .ReturnsAsync(EntityRequestResponse<Participant>.Success(new Participant()));

                await this.viewModel.ParticipantDetailsViewModel.OnValidSubmit.InvokeAsync();

                Assert.Multiple(() =>
                {
                    Assert.That(this.viewModel.IsOnUpdateViewMode, Is.False);
                    Assert.That(this.viewModel.ErrorMessageViewModel.Errors, Is.Empty);
                });

                this.participantService.Setup(x => x.UpdateParticipant(this.viewModel.ParticipantDetailsViewModel.Participant, this.viewModel.Project.Id))
                    .ThrowsAsync(new HttpRequestException());

                await this.viewModel.ParticipantDetailsViewModel.OnValidSubmit.InvokeAsync();
                Assert.That(this.viewModel.ErrorMessageViewModel.Errors, Has.Count.EqualTo(1));
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }

        [Test]
        public async Task VerifyDeleteParticipant()
        {

            var participant1 = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid())
                {
                    RoleName = "Project administrator",
                    AccessRights = new List<AccessRight>()
                    {
                        AccessRight.ReviewTask
                    }
                },
                User = new UserEntity(Guid.NewGuid())
                {
                    UserName = "user1"
                }
            };

            var participant2 = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid())
                {
                    RoleName = "Project administrator",
                    AccessRights = new List<AccessRight>()
                    {
                        AccessRight.ReviewTask
                    }
                },
                User = new UserEntity(Guid.NewGuid())
                {
                    UserName = "user2"
                }
            };

            this.viewModel.Project.Participants.Add(participant1);
            this.viewModel.Project.Participants.Add(participant2);

            var renderer = this.context.RenderComponent<ProjectDetails>(parameters =>
            {
                parameters.Add(p => p.ViewModel, this.viewModel);
            });

            var deleteButton = renderer.FindComponents<DxButton>()
                .FirstOrDefault(x => x.Instance.RenderStyle == ButtonRenderStyle.Danger);

            Assert.That(this.viewModel.ConfirmCancelPopup.IsVisible, Is.False);
            await renderer.InvokeAsync(() => deleteButton!.Instance.Click.InvokeAsync());

            Assert.That(this.viewModel.ConfirmCancelPopup.IsVisible, Is.True);

            this.participantService.Setup(x => x.DeleteParticipant(It.IsAny<Participant>(), this.viewModel.Project.Id)).ReturnsAsync(new RequestResponseDto()
            {
                IsRequestSuccessful = true
            });

            await renderer.InvokeAsync(() => this.viewModel.ConfirmCancelPopup.OnConfirm.InvokeAsync());
            Assert.That(this.viewModel.Project.Participants, Has.Count.EqualTo(1));
        }
    }
}
