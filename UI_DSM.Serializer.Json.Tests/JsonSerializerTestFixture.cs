// --------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializerTestFixture.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Serializer.Json.Tests
{
    using System.Text;
    using System.Text.Json;

    using NUnit.Framework;

    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;

    using JsonSerializer = UI_DSM.Serializer.Json.JsonSerializer;

    [TestFixture]
    public class JsonSerializerTestFixture
    {
        private IJsonSerializer serializer;

        [SetUp]
        public void Setup()
        {
            this.serializer = new JsonSerializer();
        }

        [Test]
        public void VerifyCollectionSerialization()
        {
            var role = new RoleDto(Guid.NewGuid())
            {
                AccessRights = new List<AccessRight>
                {
                    AccessRight.CreateReview,
                    AccessRight.ReviewTask
                },
                RoleName = "A role"
            };

            var user = new UserEntityDto(Guid.NewGuid())
            {
                IsAdmin = true,
                UserName = "admin"
            };

            var participant = new ParticipantDto(Guid.NewGuid())
            {
                User = user.Id,
                Role = role.Id
            };

            List<Guid> participants = new()
            {
                participant.Id
            };

            var reviewTask = new ReviewTaskDto(Guid.NewGuid())
            {
                Description = "review objective description",
                Title = "review objective Title",
                CreatedOn = DateTime.UtcNow,
                TaskNumber = 2,
                IsAssignedTo = participants,
                Status = StatusKind.Open
            };

            var reviewObjectiveGuid = Guid.NewGuid();
            var commentGuid = Guid.NewGuid();
            var noteGuid = Guid.NewGuid();
            var feedbackGuid = Guid.NewGuid();

            var reviewCategory = new ReviewCategoryDto(Guid.NewGuid())
            {
                ReviewCategoryName = "ReviewCategory",
                Description = "ReviewCategory description",
                TagColor = "#CB7777"
            };

            var reviewObjective = new ReviewObjectiveDto(reviewObjectiveGuid)
            {
                Description = "review objective description",
                Title = "review objective Title",
                CreatedOn = DateTime.UtcNow,
                ReviewObjectiveNumber = 2,
                Status = StatusKind.Open,
                Annotations = new List<Guid>{commentGuid, feedbackGuid, noteGuid},
                ReviewTasks = new List<Guid>{reviewTask.Id},
                ReviewCategories = new List<Guid> { reviewCategory.Id }
            };

            var reply = new ReplyDto(Guid.NewGuid())
            {
                Content = "Reply content",
                CreatedOn = DateTime.UtcNow,
                Author = participant.Id
            };

            var comment = new CommentDto(commentGuid)
            {
                AnnotatableItems = new List<Guid> { reviewObjectiveGuid },
                Author = participant.Id,
                Content = "Comment content",
                CreatedOn = DateTime.UtcNow,
                Replies = new List<Guid> { reply.Id }
            };

            var feedback = new FeedbackDto(feedbackGuid)
            {
                AnnotatableItems = new List<Guid> { reviewObjectiveGuid },
                Author = participant.Id,
                Content = "feedback content",
                CreatedOn = DateTime.UtcNow
            };

            var note = new NoteDto(noteGuid)
            {
                AnnotatableItems = new List<Guid> { reviewObjectiveGuid },
                Author = participant.Id,
                Content = "note content",
                CreatedOn = DateTime.UtcNow
            };

            var reviewItem = new ReviewItemDto(Guid.NewGuid())
            {
                ThingId = Guid.NewGuid()
            };

            var review = new ReviewDto(Guid.NewGuid())
            {
                Description = "review description",
                Title = "review Title",
                CreatedOn = DateTime.UtcNow,
                ReviewNumber = 2,
                Status = StatusKind.Open,
                ReviewObjectives = new List<Guid>{reviewObjective.Id},
                ReviewItems = new List<Guid>{reviewItem.Id}
            };

            var model = new ModelDto(Guid.NewGuid())
            {
                FileName = "161A0A63-DFFE-4DFA-8A33-D80B31DEC9FE.zip",
                ModelName = "Envision - Iteration 1"
            };

            var project = new ProjectDto(Guid.NewGuid())
            {
                ProjectName = "Project",
                Participants = new List<Guid>{participant.Id},
                Reviews = new List<Guid>{review.Id},
                Annotations = new List<Guid>{commentGuid, noteGuid, feedbackGuid},
                Artifacts = new List<Guid>{model.Id},
                ReviewCategories = new List<Guid> { reviewCategory.Id }
            };

            var dtos = new List<EntityDto>
            {
                project, participant, user, role, reply, comment, feedback, note, review, reviewObjective, reviewTask,
                 model, reviewItem
            };

            var stream = new MemoryStream();
            var jsonOptions = new JsonWriterOptions { Indented = true };
            Assert.That(() => this.serializer.Serialize(dtos, stream, jsonOptions), Throws.Nothing);

            var jsonOutput = Encoding.UTF8.GetString(stream.ToArray());
            Console.WriteLine(jsonOutput);
        }

        [Test]
        public void VerifyObjectSerialization()
        {
            var role = new RoleDto(Guid.NewGuid())
            {
                AccessRights = new List<AccessRight>
                {
                    AccessRight.CreateReview,
                    AccessRight.ReviewTask
                },
                RoleName = "A role"
            };

            var user = new UserEntityDto(Guid.NewGuid())
            {
                IsAdmin = true,
                UserName = "admin"
            };

            var participant = new ParticipantDto(Guid.NewGuid())
            {
                User = user.Id,
                Role = role.Id
            };

            var reviewCategory = new ReviewCategoryDto(Guid.NewGuid())
            {
                ReviewCategoryName = "ReviewCategory",
                Description = "ReviewCategory description",
                TagColor = "#CB7777"
            };

            var stream = new MemoryStream();
            var jsonOptions = new JsonWriterOptions { Indented = true };

            Assert.Multiple(() =>
            {
                Assert.That(() => this.serializer.Serialize(participant, stream, jsonOptions), Throws.Nothing);
                Assert.That(() => this.serializer.Serialize(reviewCategory, stream, jsonOptions), Throws.Nothing);
            });
            
            var jsonOutput = Encoding.UTF8.GetString(stream.ToArray());
            Console.WriteLine(jsonOutput);
        }
    }
}
