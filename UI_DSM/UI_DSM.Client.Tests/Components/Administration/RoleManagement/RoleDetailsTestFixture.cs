// --------------------------------------------------------------------------------------------------------
// <copyright file="RoleDetailsTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.Administration.RoleManagement
{
    using Bunit;

    using DevExpress.Blazor;

    using NUnit.Framework;

    using UI_DSM.Client.Components.Administration.RoleManagement;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components.Administration.RoleManagement;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Wrappers;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class RoleDetailsTestFixture
    {
        private TestContext context;
        private IRoleDetailsViewModel viewModel;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.viewModel = new RoleDetailsViewModel();
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public void VerifyComponent()
        {
            this.viewModel.Role = new Role(Guid.NewGuid())
            {
                RoleName = "Project manager",
                AccessRights = new List<AccessRight>()
                {
                    AccessRight.CreateReview,
                    AccessRight.DeleteReviewObjective,
                    AccessRight.ReviewTask
                }
            };

            var renderer = this.context.RenderComponent<RoleDetails>(parameters =>
            {
                parameters.Add(p => p.ModificationEnabled, false);
                parameters.Add(p => p.ViewModel, this.viewModel);
            });

            var listBox = renderer.FindComponent<DxListBox<AccessRightWrapper, AccessRightWrapper>>();
            var textBox = renderer.FindComponent<DxTextBox>();
            Assert.That(listBox.Instance.Values.Count(), Is.EqualTo(this.viewModel.Role.AccessRights.Count));
            Assert.That(listBox.Instance.Enabled, Is.False);
            Assert.That(textBox.Instance.Text, Is.EqualTo(this.viewModel.Role.RoleName));
            Assert.That(textBox.Instance.Enabled, Is.False);
        }
    }
}
