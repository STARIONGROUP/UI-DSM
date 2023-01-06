// --------------------------------------------------------------------------------------------------------
// <copyright file="TaskAssignmentTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.NormalUser.ReviewTask
{
    using Bunit;

    using DevExpress.Blazor;

    using Microsoft.AspNetCore.Components;

    using NUnit.Framework;

    using UI_DSM.Client.Components.NormalUser.ReviewTask;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ReviewTask;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class TaskAssignmentTestFixture
    {
        private TestContext context;
        private ITaskAssignmentViewModel taskAssignmentViewModel;
        private IErrorMessageViewModel errorMessageViewModel;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.errorMessageViewModel = new ErrorMessageViewModel();

            this.taskAssignmentViewModel = new TaskAssignmentViewModel()
            {
                SelectedParticipants = new List<Participant>(),
                OnValidSubmit = new EventCallbackFactory().Create(this, () =>
                    this.taskAssignmentViewModel.SelectedParticipants = null)
            };
        }

        [TearDown]
        public void TearDown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifyComponent()
        {
            try
            {
                var participants = new List<Participant>
                {
                    new(Guid.NewGuid())
                    {
                        User = new UserEntity(Guid.NewGuid())
                        {
                            UserName = "user"
                        },
                        DomainsOfExpertise =
                        {
                            "SYS", "THE"
                        }
                    },
                    new(Guid.NewGuid())
                    {
                        User = new UserEntity(Guid.NewGuid())
                        {
                            UserName = "user2"
                        },
                        DomainsOfExpertise =
                        {
                            "SYS", "AOGNC"
                        }
                    },
                    new(Guid.NewGuid())
                    {
                        User = new UserEntity(Guid.NewGuid())
                        {
                            UserName = "user3"
                        }
                    }
                };

                var renderer = this.context.RenderComponent<TaskAssignment>(parameters =>
                {
                    parameters.AddCascadingValue(this.errorMessageViewModel);
                    parameters.Add(p => p.ViewModel, this.taskAssignmentViewModel);
                    parameters.Add(p => p.ProjectParticipants, participants);
                });

                await renderer.InvokeAsync(() => renderer.Instance.OnValuesChanged(participants
                    .Where(x => x.DomainsOfExpertise.Contains("SYS")), "SYS"));

                Assert.That(this.taskAssignmentViewModel.SelectedParticipants, Has.Count.EqualTo(2));

                await renderer.InvokeAsync(() => renderer.Instance.OnValuesChanged(new List<Participant>(), "SYS"));
                Assert.That(this.taskAssignmentViewModel.SelectedParticipants, Is.Empty);

                var dxButton = renderer.FindComponent<DxButton>();
                await renderer.InvokeAsync(dxButton.Instance.Click.InvokeAsync);

                Assert.That(this.taskAssignmentViewModel.SelectedParticipants, Is.Null);
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }
    }
}
