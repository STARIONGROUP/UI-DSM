// --------------------------------------------------------------------------------------------------------
// <copyright file="RouteParserServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Tests.Services
{
    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Managers.ProjectManager;
    using UI_DSM.Server.Managers.ReviewManager;
    using UI_DSM.Server.Managers.ReviewObjectiveManager;
    using UI_DSM.Server.Managers.ReviewTaskManager;
    using UI_DSM.Server.Managers.RoleManager;
    using UI_DSM.Server.Managers.UserManager;
    using UI_DSM.Server.Services.RouteParserService;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class RouteParserServiceTestFixture
    {
        private IRouteParserService service;
        private Mock<IProjectManager> projectManager;
        private Mock<IReviewManager> reviewManager;
        private Mock<IReviewObjectiveManager> reviewObjectiveManager;
        private Mock<IReviewTaskManager> reviewTaskManager;
        private Mock<IUserManager> userManager;
        private Mock<IRoleManager> roleManager;

        [SetUp]
        public void Setup()
        {
            this.projectManager = new Mock<IProjectManager>();
            this.reviewManager = new Mock<IReviewManager>();
            this.reviewObjectiveManager = new Mock<IReviewObjectiveManager>();
            this.reviewTaskManager = new Mock<IReviewTaskManager>();
            this.userManager = new Mock<IUserManager>();
            this.roleManager = new Mock<IRoleManager>();

            this.service = new RouteParserService(this.projectManager.Object, this.reviewManager.Object, this.reviewObjectiveManager.Object,
                this.reviewTaskManager.Object, this.userManager.Object, this.roleManager.Object);
        }

        [Test]
        public async Task VerifyParseAdministrationUrl()
        {
            var entityName = nameof(Project);
            var url = $"/Administration/{entityName}Management/";
            var result = await this.service.ParseUrl(url);
            
            Assert.Multiple(() =>
            {
                Assert.That(result[0].DisplayName, Is.EqualTo($"{entityName} Management"));
                Assert.That(result[0].Url, Is.EqualTo($"/Administration/{entityName}Management"));
            });

            entityName = nameof(User);
            url = $"/Administration/{entityName}Management/";
            result = await this.service.ParseUrl(url);

            Assert.Multiple(() =>
            {
                Assert.That(result[0].DisplayName, Is.EqualTo($"{entityName} Management"));
                Assert.That(result[0].Url, Is.EqualTo($"/Administration/{entityName}Management"));
            });

            var guid = Guid.NewGuid();
            url = $"/Administration/{entityName}/{guid}";
            result = await this.service.ParseUrl(url);

            Assert.Multiple(() =>
            {
                Assert.That(result[0].DisplayName, Is.EqualTo("Home"));
                Assert.That(result[0].Url, Is.EqualTo("/"));
            });

            var guid1 = guid;

            this.userManager.Setup(x => x.FindEntity(guid1)).ReturnsAsync(new UserEntity()
            {
                UserName = "user"
            });

            result = await this.service.ParseUrl(url);

            Assert.Multiple(() =>
            {
                Assert.That(result[0].DisplayName, Is.EqualTo($"{entityName} Management"));
                Assert.That(result[0].Url, Is.EqualTo($"/Administration/{entityName}Management"));
                Assert.That(result[1].DisplayName, Is.EqualTo("user"));
                Assert.That(result[1].Url, Is.EqualTo($"/Administration/{entityName}/{guid}"));
            });

            entityName = nameof(Role);
            url = $"/Administration/{entityName}Management/";
            result = await this.service.ParseUrl(url);

            Assert.Multiple(() =>
            {
                Assert.That(result[0].DisplayName, Is.EqualTo($"{entityName} Management"));
                Assert.That(result[0].Url, Is.EqualTo($"/Administration/{entityName}Management"));
            });

            guid = Guid.NewGuid();
            url = $"/Administration/{entityName}/{guid}";
            result = await this.service.ParseUrl(url);

            Assert.Multiple(() =>
            {
                Assert.That(result[0].DisplayName, Is.EqualTo("Home"));
                Assert.That(result[0].Url, Is.EqualTo("/"));
            });

            this.roleManager.Setup(x => x.FindEntity(guid)).ReturnsAsync(new Role()
            {
                RoleName = "role"
            });

            result = await this.service.ParseUrl(url);

            Assert.Multiple(() =>
            {
                Assert.That(result[0].DisplayName, Is.EqualTo($"{entityName} Management"));
                Assert.That(result[0].Url, Is.EqualTo($"/Administration/{entityName}Management"));
                Assert.That(result[1].DisplayName, Is.EqualTo("role"));
                Assert.That(result[1].Url, Is.EqualTo($"/Administration/{entityName}/{guid}"));
            });

            url = $"/Administration/{entityName}/{guid}eruazh";
            result = await this.service.ParseUrl(url);

            Assert.Multiple(() =>
            {
                Assert.That(result[0].DisplayName, Is.EqualTo("Home"));
                Assert.That(result[0].Url, Is.EqualTo("/"));
            });

            entityName = nameof(ReviewItem);
            url = $"/Administration/{entityName}Management/";
            result = await this.service.ParseUrl(url);

            Assert.Multiple(() =>
            {
                Assert.That(result[0].DisplayName, Is.EqualTo("Home"));
                Assert.That(result[0].Url, Is.EqualTo("/"));
            });
        }

        [Test]
        public async Task VerifyParsePageUrl()
        {
            var projectId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            var reviewObjectiveId = Guid.NewGuid();
            var reviewTaskId = Guid.NewGuid();

            var url = string.Empty;
            var result = await this.service.ParseUrl(url);

            Assert.Multiple(() =>
            {
                Assert.That(result[0].DisplayName, Is.EqualTo("Home"));
                Assert.That(result[0].Url, Is.EqualTo("/"));
            });

            url = "/Project";
            result = await this.service.ParseUrl(url);

            Assert.Multiple(() =>
            {
                Assert.That(result[0].DisplayName, Is.EqualTo("Home"));
                Assert.That(result[0].Url, Is.EqualTo("/"));
            });

            url = $"/Project/{projectId}";
            result = await this.service.ParseUrl(url);

            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(1));
                Assert.That(result[0].DisplayName, Is.EqualTo("Home"));
                Assert.That(result[0].Url, Is.EqualTo("/"));
            });

            this.projectManager.Setup(x => x.FindEntity(projectId)).ReturnsAsync(new Project() { ProjectName = "A project" });

            result = await this.service.ParseUrl(url);

            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(2));
                Assert.That(result[1].DisplayName, Is.EqualTo("A project"));
                Assert.That(result[1].Url, Is.EqualTo(url));
            });

            url += $"/Review/{reviewId}";
            result = await this.service.ParseUrl(url);

            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(1));
                Assert.That(result[0].DisplayName, Is.EqualTo("Home"));
                Assert.That(result[0].Url, Is.EqualTo("/"));
            });

            this.reviewManager.Setup(x => x.FindEntity(reviewId)).ReturnsAsync(new Review() { Title = "Review title" });
            result = await this.service.ParseUrl(url);

            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(3));
                Assert.That(result[2].DisplayName, Is.EqualTo("Review title"));
                Assert.That(result[2].Url, Is.EqualTo(url));
            });

            url += $"/ReviewObjective/{reviewObjectiveId}";
            result = await this.service.ParseUrl(url);

            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(1));
                Assert.That(result[0].DisplayName, Is.EqualTo("Home"));
                Assert.That(result[0].Url, Is.EqualTo("/"));
            });

            this.reviewObjectiveManager.Setup(x => x.FindEntity(reviewObjectiveId)).ReturnsAsync(new ReviewObjective() { ReviewObjectiveKindNumber = 1});
            result = await this.service.ParseUrl(url);

            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(4));
                Assert.That(result[3].DisplayName, Is.EqualTo("PRR - 1"));
                Assert.That(result[3].Url, Is.EqualTo(url));
            });

            url += $"/ReviewTask/{reviewTaskId}";
            result = await this.service.ParseUrl(url);

            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(1));
                Assert.That(result[0].DisplayName, Is.EqualTo("Home"));
                Assert.That(result[0].Url, Is.EqualTo("/"));
            });

            this.reviewTaskManager.Setup(x => x.FindEntity(reviewTaskId)).ReturnsAsync(new ReviewTask() { Description = "A task" });
            result = await this.service.ParseUrl(url);

            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(5));
                Assert.That(result[4].DisplayName, Is.EqualTo("A task"));
                Assert.That(result[4].Url, Is.EqualTo(url));
            });

            url += $"/Project/{projectId}";
            result = await this.service.ParseUrl(url);

            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(1));
                Assert.That(result[0].DisplayName, Is.EqualTo("Home"));
                Assert.That(result[0].Url, Is.EqualTo("/"));
            });
        }
    }
}
