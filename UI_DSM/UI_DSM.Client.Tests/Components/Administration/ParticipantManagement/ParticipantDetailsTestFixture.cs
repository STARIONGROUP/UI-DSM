// --------------------------------------------------------------------------------------------------------
// <copyright file="ParticipantDetailsTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.Administration.ParticipantManagement
{
    using Bunit;

    using NUnit.Framework;

    using UI_DSM.Client.Components.Administration.ParticipantManagement;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components.Administration.ParticipantManagement;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ParticipantDetailsTestFixture
    {
        private TestContext context;
        private IParticipantDetailsViewModel viewModel;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.viewModel = new ParticipantDetailsViewModel();
        }

        [TearDown]
        public void TearDown()
        {
            this.context.CleanContext();
        }

        [Test]
        public void VerifyRender()
        {        
            this.viewModel.Participant = new Participant()
            {
                Role = new Role(Guid.NewGuid())
                {
                    RoleName = "Project administrator",
                    AccessRights = new List<AccessRight>()
                    {
                        AccessRight.ManageParticipant
                    }
                },
                User = new UserEntity(Guid.NewGuid())
                {
                    UserName = "user"
                }
            };

            var renderer = this.context.RenderComponent<ParticipantDetails>(parameters =>
                parameters.Add(p => p.ViewModel, this.viewModel));

            var participantName = renderer.Find("#participantName");
            Assert.That(participantName.TextContent, Is.EqualTo(this.viewModel.Participant.User.UserName.ToString()));
        }
    }
}
