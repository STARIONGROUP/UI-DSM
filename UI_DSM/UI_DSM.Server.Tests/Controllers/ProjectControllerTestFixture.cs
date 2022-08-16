// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectControllerTestFixture.cs" company="RHEA System S.A.">
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
    using Microsoft.AspNetCore.Mvc;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Controllers;
    using UI_DSM.Server.Managers.ProjectManager;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ProjectControllerTestFixture
    {
        private ProjectController controller;
        private Mock<IProjectManager> projectManager;

        [SetUp]
        public void Setup()
        {
            this.projectManager = new Mock<IProjectManager>();
            this.controller = new ProjectController(this.projectManager.Object);
        }

        [Test]
        public void VerifyGetProjects()
        {
            var projectsCollection = new List<Project>();
            this.projectManager.Setup(x => x.GetProjects()).ReturnsAsync(projectsCollection);
            var getProjectsReponse = this.controller.GetProjects().Result as OkObjectResult;
            Assert.That(getProjectsReponse, Is.Not.Null);
            var retrievedProjects = (IEnumerable<ProjectDto>)getProjectsReponse.Value;
            Assert.That(retrievedProjects, Is.Empty);
            
            projectsCollection.Add(new Project(Guid.NewGuid())
            {
                ProjectName = "Project 1"
            });

            projectsCollection.Add(new Project(Guid.NewGuid())
            {
                ProjectName = "Project 2"
            });

            getProjectsReponse = this.controller.GetProjects().Result as OkObjectResult;
            retrievedProjects = (IEnumerable<ProjectDto>)getProjectsReponse!.Value;
            Assert.That(retrievedProjects!.Count(), Is.EqualTo(2));

            var unknownGuid = Guid.NewGuid();
            this.projectManager.Setup(x => x.GetProject(projectsCollection[0].Id)).ReturnsAsync(projectsCollection[0]);
            this.projectManager.Setup(x => x.GetProject(unknownGuid)).ReturnsAsync((Project)null);

            var unknownProjectResult = this.controller.GetProject(unknownGuid).Result as NotFoundResult;
            Assert.That(unknownProjectResult, Is.Not.Null);
            Assert.That(unknownProjectResult.StatusCode, Is.EqualTo(404));

            var knownProjectResult = this.controller.GetProject(projectsCollection[0].Id).Result as OkObjectResult;
            Assert.That(knownProjectResult, Is.Not.Null);
            var projectDto = (ProjectDto)knownProjectResult.Value;
            Assert.That(projectDto!.Id, Is.EqualTo(projectsCollection[0].Id));
        }

        [Test]
        public void VerifyCreateProject()
        {
            var project = new ProjectDto(Guid.NewGuid());
            var badRequest = this.controller.CreateProject(project).Result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null);

            project.Id = Guid.Empty;

            this.projectManager.Setup(x => x.CreateProject(It.IsAny<Project>()))
                .ReturnsAsync(EntityOperationResult<Project>.Failed("Already exist"));

            badRequest = this.controller.CreateProject(project).Result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null);

            var createdProject = new Project(Guid.NewGuid())
            {
                ProjectName = "project"
            };

            this.projectManager.Setup(x => x.CreateProject(It.IsAny<Project>()))
                .ReturnsAsync(EntityOperationResult<Project>.Success(createdProject));

            var okRequest = this.controller.CreateProject(project).Result as OkObjectResult;
            Assert.That(okRequest, Is.Not.Null);
            var projectDto = (EntityRequestResponseDto<ProjectDto>)okRequest.Value;
            Assert.That(projectDto, Is.Not.Null);
            Assert.That(projectDto.Entity.Id, Is.EqualTo(createdProject.Id));
            Assert.That(projectDto.Entity.ProjectName, Is.EqualTo(createdProject.ProjectName));
        }

        [Test]
        public void VerifyDeleteProject()
        {
            var project = new Project(Guid.NewGuid());
            this.projectManager.Setup(x => x.GetProject(project.Id)).ReturnsAsync((Project)null);

            var notFound = this.controller.DeleteProject(project.Id).Result as NotFoundObjectResult;
            Assert.That(notFound, Is.Not.Null);

            this.projectManager.Setup(x => x.GetProject(project.Id)).ReturnsAsync(project);
            this.projectManager.Setup(x => x.DeleteProject(project)).ReturnsAsync(EntityOperationResult<Project>.Failed());
            
            var internalError = this.controller.DeleteProject(project.Id).Result as ObjectResult;
            Assert.That(internalError, Is.Not.Null);
            Assert.That(internalError.StatusCode, Is.EqualTo(500));

            this.projectManager.Setup(x => x.DeleteProject(project)).ReturnsAsync(EntityOperationResult<Project>.Success(null));
            var okResult = this.controller.DeleteProject(project.Id).Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
        }
    }
}
