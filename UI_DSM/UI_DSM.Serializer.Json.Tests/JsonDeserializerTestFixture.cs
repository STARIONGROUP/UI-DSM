// --------------------------------------------------------------------------------------------------------
// <copyright file="JsonDeserializerTestFixture.cs" company="RHEA System S.A.">
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
    using NUnit.Framework;

    using UI_DSM.Shared.DTO.Models;

    [TestFixture]
    public class JsonDeserializerTestFixture
    {
        private IJsonDeserializer deserializer;

        [SetUp]
        public void Setup()
        {
            this.deserializer = new JsonDeserializer();
        }

        [Test]
        public void VerifyDeserialize()
        {
            var fileName = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Data", "sample.json");

            using var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            var data = this.deserializer.Deserialize(stream).ToList();
            
            Assert.Multiple(() =>
            {
                Assert.That(data, Has.Count.EqualTo(11));
                Assert.That(data.OfType<ProjectDto>(), Is.Not.Empty);
                Assert.That(data.OfType<ProjectDto>().First().Id, Is.EqualTo(Guid.Parse("9417d7ea-ce53-4187-897f-f1a1cc2d104e")));
                Assert.That(data.OfType<ParticipantDto>().FirstOrDefault(x => x.Id == Guid.Parse("3542293d-cdcd-4143-a546-436bc9d9a118")), Is.Not.Null);
            });
        }
    }
}
