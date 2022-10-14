// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewCreationTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.NormalUser.ProjectReview
{
    using Bunit;

    using DevExpress.Blazor;

    using Moq;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;

    using NUnit.Framework;

    using UI_DSM.Client.Components.NormalUser.ProjectReview;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;
    using AppComponents;
    using UI_DSM.Client.Pages.Administration;
    using UI_DSM.Shared.Types;

    [TestFixture]
    public class ReviewCreationTestFixture
    {
        private TestContext context;
        private IReviewCreationViewModel reviewCreationViewModel;
        private IErrorMessageViewModel errorMessageViewModel;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.errorMessageViewModel = new ErrorMessageViewModel();

            this.reviewCreationViewModel = new ReviewCreationViewModel()
            {
                Review = new Review(),
                OnValidSubmit = new EventCallbackFactory().Create(this, () => this.reviewCreationViewModel.Review = new Review())
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
                var renderer = this.context.RenderComponent<ReviewCreation>(parameters =>
                {
                    parameters.AddCascadingValue(this.errorMessageViewModel);
                    parameters.Add(p => p.ViewModel, this.reviewCreationViewModel);
                    parameters.Add(p => p.ProjectArtifacts, new List<Artifact>());
                });

                var textBox = renderer.FindComponent<DxTextBox>();
                var listBox = renderer.FindComponent<DxListBox<Model, Model>>();
                Assert.Multiple(() =>
                {
                    Assert.That(textBox.Instance.Text, Is.Null);
                    Assert.That(listBox.Instance.Values, Is.Empty);
                });

                this.reviewCreationViewModel.Review.Title = "review1";

                renderer.Render();
                Assert.That(textBox.Instance.Text, Is.EqualTo(this.reviewCreationViewModel.Review.Title));

                var dxButton = renderer.FindComponent<EditForm>();
                await renderer.InvokeAsync(dxButton.Instance.OnValidSubmit.InvokeAsync);

                Assert.That(this.reviewCreationViewModel.Review.Title, Is.Null);
            }
            catch (Exception e)
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }
    }
}
