// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectManagerTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Tests.Managers
{
    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Managers.ProjectManager;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ProjectManagerTestFixture
    {
        [SetUp]
        public void Setup()
        {
            this.context = new Mock<DatabaseContext>();
            this.manager = new ProjectManager(this.context.Object);
        }

        private ProjectManager manager;
        private Mock<DatabaseContext> context;

        [Test]
        public void VerifyGetProjects()
        {
            var data = new List<Project>
            {
                new(Guid.NewGuid()) { ProjectName = "P1" },
                new(Guid.NewGuid()) { ProjectName = "P2" },
                new(Guid.NewGuid()) { ProjectName = "P2" }
            };

            var dbSet = DbSetMockHelper.CreateMock(data);
            
            this.context.Setup(x => x.Projects).Returns(dbSet);
            Assert.That(this.manager.GetProjects().Result.Count(), Is.EqualTo(data.Count));
        }

        [Test]
        public void VerifyCreateProject()
        {
            var data = new List<Project>
            {
                new(Guid.NewGuid()) { ProjectName = "P1" },
                new(Guid.NewGuid()) { ProjectName = "P2" },
                new(Guid.NewGuid()) { ProjectName = "P2" }
            };

            var dbSet = DbSetMockHelper.CreateMock(data);

            this.context.Setup(x => x.Projects).Returns(dbSet);

            var newProject = new Project()
            {
                ProjectName = data.First().ProjectName
            };

            var creationResult = this.manager.CreateProject(newProject).Result;
            Assert.That(creationResult.Succeeded, Is.False);

            newProject.ProjectName = "A new project";
            
            Task.Run(async () => await this.manager.CreateProject(newProject));
            this.context.Verify(x => x.Add(It.IsAny<Project>()), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new InvalidOperationException());

            creationResult = this.manager.CreateProject(newProject).Result;
            Assert.That(creationResult.Succeeded, Is.False);
        }

        [Test]
        public void VerifyUpdateProject()
        {
            var project = new Project();
            _ = this.manager.UpdateProject(project).Result;

            this.context.Verify(x => x.Update(It.IsAny<Project>()), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new InvalidOperationException());

            var creationResult = this.manager.UpdateProject(project).Result;
            Assert.That(creationResult.Succeeded, Is.False);
        }

        [Test]
        public void VerifyDeleteProject()
        {
            var project = new Project();
            _ = this.manager.DeleteProject(project).Result;

            this.context.Verify(x => x.Remove(It.IsAny<Project>()), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new InvalidOperationException());

            var creationResult = this.manager.DeleteProject(project).Result;
            Assert.That(creationResult.Succeeded, Is.False);
        }
    }
}
