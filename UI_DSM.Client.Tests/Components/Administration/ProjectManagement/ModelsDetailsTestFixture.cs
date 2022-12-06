// --------------------------------------------------------------------------------------------------------
// <copyright file="ModelsDetailsTestFixture.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar, Jaime Bernar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Tests.Components.Administration.ProjectManagement
{
    using System;
    using System.Collections.Generic;

    using Bunit;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components.Administration.ProjectManagement;
    using UI_DSM.Client.Services.Administration.ParticipantService;
    using UI_DSM.Client.Services.Administration.RoleService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components.Administration.ProjectManagement;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    internal class ModelsDetailsTestFixture
    {
        private TestContext context;
        private IProjectDetailsViewModel viewModel;
        private Mock<IParticipantService> participantService;
        private Mock<IRoleService> roleService;
        private IRenderedComponent<ModelsDetails> renderer;

        [SetUp]
        public void Setup()
        {
            try
            {
                this.context = new TestContext();
                this.context.ConfigureDevExpressBlazor();
                this.participantService = new Mock<IParticipantService>();
                this.roleService = new Mock<IRoleService>();

                var project = new Project(Guid.NewGuid())
                {
                    ProjectName = "Project",
                };

                var model1 = new UI_DSM.Shared.Models.Model(Guid.NewGuid()) { ModelName = "Model1" };
                var model2 = new UI_DSM.Shared.Models.Model(Guid.NewGuid()) { ModelName = "Model2" };

                project.Artifacts.Add(model1);
                project.Artifacts.Add(model2);

                this.viewModel = new ProjectDetailsViewModel(this.participantService.Object, this.roleService.Object)
                {
                    Project = project,
                };

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

                this.renderer = this.context.RenderComponent<ModelsDetails>(parameters =>
                {
                    parameters.Add(p => p.ViewModel, this.viewModel);
                });
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public void VerifyOpenUpdateParticipant()
        {
            var modelName = this.renderer.Find("td");
            Assert.That(modelName.InnerHtml, Does.Contain("Model1"));
        }

        [Test]
        public void VerifyNumberOfModels()
        {
            Assert.That(this.renderer.Instance.ProjectModels, Has.Count.EqualTo(2));
        }
    }
}
