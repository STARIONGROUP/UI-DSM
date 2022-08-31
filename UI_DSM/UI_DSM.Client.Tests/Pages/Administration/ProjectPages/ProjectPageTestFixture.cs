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
    using Bunit;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components.Administration.ProjectManagement;
    using UI_DSM.Client.Pages.Administration.ProjectPages;
    using UI_DSM.Client.Services.Administration.ProjectService;
    using UI_DSM.Client.ViewModels.Pages.Administration.ProjectPages;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ProjectPageTestFixture
    {
        private TestContext context;
        private IProjectPageViewModel viewModel;
        private Mock<IProjectService> projectService;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.projectService = new Mock<IProjectService>();
            this.viewModel = new ProjectPageViewModel(this.projectService.Object);
            this.context.Services.AddSingleton(this.viewModel);
        }

        [TearDown]
        public void Teardown()
        {
            this.context.Dispose();
        }

        [Test]
        public void VerifyRenderer()
        {
            var projectGuid = Guid.NewGuid();
            this.projectService.Setup(x => x.GetProject(projectGuid, 0)).ReturnsAsync((Project)null);

            var renderer = this.context.RenderComponent<ProjectPage>(parameters =>
            {
                parameters.Add(p => p.ProjectId, projectGuid.ToString());
            });

            Assert.That(() => renderer.FindComponent<ProjectDetails>(), Throws.Exception);

            this.projectService.Setup(x => x.GetProject(projectGuid, 1)).ReturnsAsync(new Project(projectGuid)
            {
                ProjectName = "Project"
            });

            this.viewModel.OnInitializedAsync(projectGuid);
            renderer.Render();

            Assert.That(this.viewModel.ProjectDetailsViewModel.Project, Is.Not.Null);
        }
    }
}
