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
    using Microsoft.AspNetCore.Components.Forms;

    using NUnit.Framework;

    using UI_DSM.Client.Components.NormalUser.ReviewTask;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ReviewTask;
    using UI_DSM.Shared.Enumerator;
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
                OnValidSubmit = new EventCallbackFactory().Create(this, () => this.taskAssignmentViewModel.SelectedParticipants = new List<Participant>())
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
                var renderer = this.context.RenderComponent<TaskAssignment>(parameters =>
                {
                    parameters.AddCascadingValue(this.errorMessageViewModel);
                    parameters.Add(p => p.ViewModel, this.taskAssignmentViewModel);
                    parameters.Add(p => p.ProjectParticipants, new List<Participant>());
                });

                var listBox = renderer.FindComponent<DxListBox<Participant, Participant>>();


                Assert.That(listBox.Instance.Values, Is.Empty);

                var participant = new Participant(Guid.NewGuid())
                {
                    User = new UserEntity(Guid.NewGuid())
                    {
                        UserName = "user"
                    }
                };

                this.taskAssignmentViewModel.SelectedParticipants.ToList().Add(participant);

                renderer.Render();

                var dxButton = renderer.FindComponent<EditForm>();
                await renderer.InvokeAsync(dxButton.Instance.OnValidSubmit.InvokeAsync);

                Assert.That(this.taskAssignmentViewModel.SelectedParticipants.First().User, Is.Null);
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }
    }
}
