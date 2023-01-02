// --------------------------------------------------------------------------------------------------------
// <copyright file="ResolverServiceTestFixture.cs" company="RHEA System S.A.">
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
    using CDP4Common.EngineeringModelData;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Managers.CommentManager;
    using UI_DSM.Server.Managers.FeedbackManager;
    using UI_DSM.Server.Managers.ModelManager;
    using UI_DSM.Server.Managers.NoteManager;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ProjectManager;
    using UI_DSM.Server.Managers.ReplyManager;
    using UI_DSM.Server.Managers.ReviewCategoryManager;
    using UI_DSM.Server.Managers.ReviewManager;
    using UI_DSM.Server.Managers.ReviewObjectiveManager;
    using UI_DSM.Server.Managers.ReviewTaskManager;
    using UI_DSM.Server.Managers.RoleManager;
    using UI_DSM.Server.Managers.ThingManager;
    using UI_DSM.Server.Managers.UserManager;
    using UI_DSM.Server.Services.ResolverService;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ResolverServiceTestFixture
    {
        private ResolverService service;
        private Mock<IParticipantManager> participantManager;
        private Mock<IProjectManager> projectManager;
        private Mock<IReviewManager> reviewManager;
        private Mock<IReviewObjectiveManager> reviewObjectiveManager;
        private Mock<IReviewTaskManager> reviewTaskManager;
        private Mock<ICommentManager> commentManager;
        private Mock<IReplyManager> replyManager;
        private Mock<IThingManager> thingManager;
        private Mock<IRoleManager> roleManager;
        private Mock<IUserManager> userManager;
        private Mock<IModelManager> modelManager;
        private Mock<IReviewCategoryManager> reviewCategoryManager;
        private Mock<INoteManager> noteManager;
        private Mock<IFeedbackManager> feedbackManager;

        [SetUp]
        public void Setup()
        {
            this.participantManager = new Mock<IParticipantManager>();
            this.projectManager = new Mock<IProjectManager>();
            this.reviewManager = new Mock<IReviewManager>();
            this.reviewObjectiveManager = new Mock<IReviewObjectiveManager>();
            this.reviewTaskManager = new Mock<IReviewTaskManager>();
            this.commentManager = new Mock<ICommentManager>();
            this.replyManager = new Mock<IReplyManager>();
            this.thingManager = new Mock<IThingManager>();
            this.roleManager = new Mock<IRoleManager>();
            this.userManager = new Mock<IUserManager>();
            this.modelManager = new Mock<IModelManager>();
            this.reviewCategoryManager = new Mock<IReviewCategoryManager>();
            this.noteManager = new Mock<INoteManager>();
            this.feedbackManager = new Mock<IFeedbackManager>();

            this.service = new ResolverService(this.participantManager.Object, this.projectManager.Object, this.reviewManager.Object,
                this.reviewObjectiveManager.Object, this.reviewTaskManager.Object, this.commentManager.Object, this.replyManager.Object,
                this.thingManager.Object, this.roleManager.Object, this.userManager.Object, this.modelManager.Object,
                this.reviewCategoryManager.Object, this.noteManager.Object, this.feedbackManager.Object);
        }

        [Test]
        public async Task VerifyResolveSearchResult()
        {
            const string username = "user";

            var model1 = new Model(Guid.NewGuid())
            {
                IterationId = Guid.NewGuid()
            };
            
            var model2 = new Model(Guid.NewGuid())
            {
                IterationId = Guid.NewGuid()
            };

            var project1 = new Project(Guid.NewGuid());
            var project2 = new Project(Guid.NewGuid());

            var availableProjects = new List<Project>
            {
                project1,
                project2
            };

            var managedProjects = new List<Project>
            {
                project1
            };

            this.projectManager.Setup(x => x.GetAvailableProjectsForUser(username)).ReturnsAsync(availableProjects);
            this.projectManager.Setup(x => x.GetProjectsForManagement(username)).ReturnsAsync(managedProjects);
            this.modelManager.Setup(x => x.GetContainedEntities(project1.Id,0)).ReturnsAsync(new List<Entity> { model1 });
            this.modelManager.Setup(x => x.GetContainedEntities(project2.Id,0)).ReturnsAsync(new List<Entity> { model2 });
            
            this.userManager.Setup(x => x.GetUserByName(username)).ReturnsAsync(new UserEntity(Guid.NewGuid())
            {
                IsAdmin = true
            });

            var dtos = new List<CommonBaseSearchDto>
            {
                new ()
                {
                    Type = nameof(ProjectDto),
                    Id = Guid.NewGuid()
                },
                new ()
                {
                    Type = nameof(RoleDto),
                    Id = Guid.NewGuid()
                },
                new ()
                {
                    Type = nameof(UserEntityDto),
                    Id = Guid.NewGuid()
                },
                new ()
                {
                    Type = nameof(ParticipantDto),
                    Id = Guid.NewGuid()
                },
                new ()
                {
                    Type = nameof(ReviewCategoryDto),
                    Id = Guid.NewGuid()
                },
            };

            var resolvedDtos = await this.service.ResolveSearchResult(dtos, username);
            Assert.That(resolvedDtos, Is.Empty);

            this.projectManager.Setup(x => x.GetSearchResult(dtos[0].Id)).ReturnsAsync(new SearchResultDto());

            this.roleManager.Setup(x => x.GetSearchResult(dtos[1].Id)).ReturnsAsync(new SearchResultDto());
            this.userManager.Setup(x => x.GetSearchResult(dtos[2].Id)).ReturnsAsync(new SearchResultDto());
            
            this.participantManager.Setup(x => x.GetSearchResult(dtos[3].Id)).ReturnsAsync(new SearchResultDto()
            {
                BaseUrl = $"Project/{Guid.NewGuid()}"
            });

            this.reviewCategoryManager.Setup(x => x.GetSearchResult(dtos[4].Id)).ReturnsAsync(new SearchResultDto());

            resolvedDtos = await this.service.ResolveSearchResult(dtos, username);
            Assert.That(resolvedDtos, Has.Count.EqualTo(dtos.Count));

            this.userManager.Setup(x => x.GetUserByName(username)).ReturnsAsync(new UserEntity(Guid.NewGuid()));

            resolvedDtos = await this.service.ResolveSearchResult(dtos, username);
            Assert.That(resolvedDtos, Is.Empty);

            var otherDtos = new List<CommonBaseSearchDto>
            {
                new ()
                {
                    Type = nameof(ProjectDto),
                    Id = project1.Id
                },
                new ()
                {
                    Type = nameof(ParticipantDto),
                    Id = Guid.NewGuid()
                },
                new ()
                {
                    Type = nameof(ModelDto),
                    Id = Guid.NewGuid()
                },
                new ()
                {
                    Type = nameof(ReviewDto),
                    Id = Guid.NewGuid()
                },
                new ()
                {
                    Type = nameof(ReviewObjectiveDto),
                    Id = Guid.NewGuid()
                },
                new ()
                {
                    Type = nameof(ReviewTaskDto),
                    Id = Guid.NewGuid()
                },
                new ()
                {
                    Type = nameof(CommentDto),
                    Id = Guid.NewGuid()
                },
                new ()
                {
                    Type = nameof(NoteDto),
                    Id = Guid.NewGuid()
                },
                new ()
                {
                    Type = nameof(FeedbackDto),
                    Id = Guid.NewGuid()
                },
                new ()
                {
                    Type = nameof(ReplyDto),
                    Id = Guid.NewGuid()
                },
            };

            resolvedDtos = await this.service.ResolveSearchResult(otherDtos, username);
            Assert.That(resolvedDtos, Is.Empty);

            this.projectManager.Setup(x => x.GetSearchResult(otherDtos[0].Id)).ReturnsAsync(new SearchResultDto());

            this.participantManager.Setup(x => x.GetSearchResult(otherDtos[1].Id)).ReturnsAsync(new SearchResultDto()
            {
                BaseUrl = $"Project/{project1.Id}/Model/{otherDtos[1].Id}"
            });

            this.modelManager.Setup(x => x.GetSearchResult(otherDtos[2].Id)).ReturnsAsync(new SearchResultDto()
            {
                BaseUrl = $"Project/{project2.Id}/Model/{otherDtos[2].Id}"
            });

            this.reviewManager.Setup(x => x.GetSearchResult(otherDtos[3].Id)).ReturnsAsync(new SearchResultDto()
            {
                BaseUrl = $"Project/{project2.Id}/Review/{otherDtos[3].Id}"
            });

            this.reviewObjectiveManager.Setup(x => x.GetSearchResult(otherDtos[4].Id)).ReturnsAsync(new SearchResultDto()
            {
                BaseUrl = $"Project/{project2.Id}/Review/{otherDtos[3].Id}/ReviewObjective/{otherDtos[4].Id}"
            });

            this.reviewTaskManager.Setup(x => x.GetSearchResult(otherDtos[5].Id)).ReturnsAsync(new SearchResultDto()
            {
                BaseUrl = $"Project/{project2.Id}/Review/{otherDtos[3].Id}/ReviewObjective/{otherDtos[4].Id}/ReviewTask/{otherDtos[5].Id}"
            });

            this.commentManager.Setup(x => x.GetSearchResult(otherDtos[6].Id)).ReturnsAsync(new SearchResultDto()
            {
                BaseUrl = $"Project/{project2.Id}/Comment/{otherDtos[6].Id}"
            });

            this.noteManager.Setup(x => x.GetSearchResult(otherDtos[7].Id)).ReturnsAsync(new SearchResultDto()
            {
                BaseUrl = $"Project/{project2.Id}/Note/{otherDtos[7].Id}"
            });

            this.feedbackManager.Setup(x => x.GetSearchResult(otherDtos[8].Id)).ReturnsAsync(new SearchResultDto()
            {
                BaseUrl = $"Project/{project2.Id}/Feedback/{otherDtos[8].Id}"
            });

            this.replyManager.Setup(x => x.GetSearchResult(otherDtos[9].Id)).ReturnsAsync(new SearchResultDto()
            {
                BaseUrl = $"Project/{project2.Id}/Comment/{otherDtos[6].Id}/Reply/{otherDtos[9].Id}"
            });

            resolvedDtos = await this.service.ResolveSearchResult(otherDtos, username);
            Assert.That(resolvedDtos, Has.Count.EqualTo(otherDtos.Count));

            resolvedDtos = await this.service.ResolveSearchResult(new List<CommonBaseSearchDto> { new() { Type = nameof(Participant) } }, username);
            Assert.That(resolvedDtos, Is.Empty);

            var thingsDtos = new List<CommonBaseSearchDto>
            {
                new ()
                {
                    Type = nameof(ElementDefinition),
                    Id = Guid.NewGuid()
                }
            };

            this.thingManager.Setup(x => x.GetThing(thingsDtos[0], model1))
                .ReturnsAsync(new ElementDefinition()
                {
                    Name = "Element"
                });

            this.modelManager.Setup(x => x.GetSearchResult(model1.Id)).ReturnsAsync(new SearchResultDto()
            {
                BaseUrl = $"Project/{project2.Id}/Model/{model1.Id}"
            });

            resolvedDtos = await this.service.ResolveSearchResult(thingsDtos, username);
            Assert.That(resolvedDtos, Has.Count.EqualTo(thingsDtos.Count));
        }
    }
}
