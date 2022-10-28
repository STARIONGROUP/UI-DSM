// --------------------------------------------------------------------------------------------------------
// <copyright file="AnnotationServiceTestFixture.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to annotation an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Tests.Services.AnnotationService
{
    using System.Net;

    using NUnit.Framework;
    
    using RichardSzalay.MockHttp;

    using UI_DSM.Client.Services;
    using UI_DSM.Client.Services.AnnotationService;
    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class AnnotationServiceTestFixture
    {
        private AnnotationService service;
        private MockHttpMessageHandler httpMessageHandler;
        private List<EntityDto> entitiesDto;
        private IJsonService jsonService;

        [SetUp]
        public void Setup()
        {
            EntityHelper.RegisterEntities();
            this.httpMessageHandler = new MockHttpMessageHandler();
            var httpClient = this.httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/api");

            var participantId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var annotatableItemId = Guid.NewGuid();
            var commentId = Guid.NewGuid();
            var feedbackId = Guid.NewGuid();
            var noteId = Guid.NewGuid();
            
            this.entitiesDto = new List<EntityDto>
            {
                new CommentDto(commentId)
                {
                    Author = participantId,
                    CreatedOn = DateTime.UtcNow,
                    Content = "Comment content",
                    AnnotatableItems = new List<Guid>
                    {
                        annotatableItemId
                    }
                },
                new FeedbackDto(feedbackId)
                {
                    Author = participantId,
                    CreatedOn = DateTime.UtcNow,
                    Content = "Comment content",
                    AnnotatableItems = new List<Guid>
                    {
                        annotatableItemId
                    }
                }, 
                new NoteDto(noteId)
                {
                    Author = participantId,
                    CreatedOn = DateTime.UtcNow,
                    Content = "Comment content",
                    AnnotatableItems = new List<Guid>
                    {
                        annotatableItemId
                    }
                },
                new ParticipantDto(participantId)
                {
                    Role = roleId,
                    User = userId
                },
                new UserEntityDto(userId)
                {
                    UserName = "User"
                },
                new RoleDto(roleId)
                {
                    RoleName = "Annotationer",
                    AccessRights = new List<AccessRight>
                    {
                        AccessRight.ReviewTask
                    }
                },
                new ReviewObjectiveDto(annotatableItemId)
                {
                    Annotations = new List<Guid>
                    {
                        noteId, commentId, feedbackId
                    },
                    Author = participantId,
                    Description = "A description"
                }
            };

            ServiceBase.RegisterService<AnnotationService>();

            this.jsonService = JsonSerializerHelper.CreateService();
            this.service = new AnnotationService(httpClient, this.jsonService);
        }

        [Test]
        public async Task VerifyGetAnnotations()
        {
            var projectId = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When($"/Project/{projectId}/Annotation");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.GetAnnotationsOfProject(projectId), Throws.Exception);
            httpResponse.StatusCode = HttpStatusCode.OK;

            httpResponse.Content = new StringContent(this.jsonService.Serialize(this.entitiesDto));
            var annotations = await this.service.GetAnnotationsOfProject(projectId);
            Assert.That(annotations, Has.Count.EqualTo(3));
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.GetAnnotationsOfProject(projectId), Throws.Exception);
        }

        [Test]
        public async Task VerifyGetAnnotation()
        {
            var projectId = Guid.NewGuid();
            var guid = this.entitiesDto.First().Id;
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When(HttpMethod.Get, $"/Project/{projectId}/Annotation/{guid}");
            request.Respond(_ => httpResponse);
            var annotation = await this.service.GetAnnotation(projectId, guid);

            Assert.That(annotation, Is.Null);

            httpResponse.StatusCode = HttpStatusCode.OK;

            httpResponse.Content = new StringContent(this.jsonService.Serialize(this.entitiesDto));
            annotation = await this.service.GetAnnotation(projectId, guid);
            
            Assert.Multiple(() =>
            {
                Assert.That(annotation.Id, Is.EqualTo(guid));
                Assert.That(annotation, Is.TypeOf<Comment>());
            });

            httpResponse.Content = new StringContent(string.Empty);

            Assert.That(async () => await this.service.GetAnnotation(projectId, guid), Throws.Exception);
        }

        [Test]
        public async Task VerifyCreateAnnotation()
        {
            var annotation = new Comment()
            {
                Content = "Annotation description"
            };

            var projectId = Guid.NewGuid();

            var httpResponse = new HttpResponseMessage();

            var entityRequestResponse = new EntityRequestResponseDto()
            {
                IsRequestSuccessful = false
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));
            var request = this.httpMessageHandler.When(HttpMethod.Post, $"/Project/{projectId}/Annotation/Create");
            request.Respond(_ => httpResponse);

            var requestResponse = await this.service.CreateAnnotation(projectId, annotation);
            Assert.That(requestResponse.IsRequestSuccessful, Is.False);

            entityRequestResponse.IsRequestSuccessful = true;

            entityRequestResponse.Entities = annotation.GetAssociatedEntities().ToDtos();

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));

            requestResponse = await this.service.CreateAnnotation(projectId, annotation);

            Assert.Multiple(() =>
            {
                Assert.That(requestResponse.IsRequestSuccessful, Is.True);
                Assert.That(requestResponse.Entity, Is.Not.Null);
            });

            httpResponse.Content = new StringContent(string.Empty);

            Assert.That(async () => await this.service.CreateAnnotation(projectId, annotation), Throws.Exception);
        }

        [Test]
        public async Task VerifyDeleteAnnotation()
        {
            var annotation = new Note(Guid.NewGuid())
            {
                Author = new Participant(Guid.NewGuid())
            };

            var project = new Project(Guid.NewGuid());

            var httpResponse = new HttpResponseMessage();
            var requestResponse = new RequestResponseDto() { IsRequestSuccessful = true };
            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));
            var request = this.httpMessageHandler.When(HttpMethod.Delete, $"/Project/{project.Id}/Annotation/{annotation.Id}");
            request.Respond(_ => httpResponse);
            project.Annotations.Add(annotation);

            var result = await this.service.DeleteAnnotation(project.Id, annotation);
            Assert.That(result.IsRequestSuccessful, Is.True);

            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.DeleteAnnotation(project.Id, annotation), Throws.Exception);
        }

        [Test]
        public async Task VerifyUpdateAnnotation()
        {
            var annotation = new Feedback(Guid.NewGuid())
            {
                Author = new Participant(Guid.NewGuid())
                {
                    Role = new Role(Guid.NewGuid()),
                    User = new UserEntity(Guid.NewGuid())
                }
            };

            var project = new Project(Guid.NewGuid());

            var httpResponse = new HttpResponseMessage();
            var requestResponse = new EntityRequestResponseDto() { IsRequestSuccessful = false };
            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));

            var request = this.httpMessageHandler.When(HttpMethod.Put, $"/Project/{project.Id}/Annotation/{annotation.Id}");
            request.Respond(_ => httpResponse);
            project.Annotations.Add(annotation);

            var requestResult = await this.service.UpdateAnnotation(project.Id, annotation);
            Assert.That(requestResult.IsRequestSuccessful, Is.False);

            requestResponse.IsRequestSuccessful = true;

            requestResponse.Entities = annotation.GetAssociatedEntities().ToDtos();

            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));

            requestResult = await this.service.UpdateAnnotation(project.Id, annotation);
            Assert.That(requestResult.IsRequestSuccessful, Is.True);
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.UpdateAnnotation(project.Id, annotation), Throws.Exception);
        }
    }
}
