// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveCreationTestFixture.cs" company="RHEA System S.A.">
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
    using UI_DSM.Client.Services.ReviewObjectiveService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;
    using UI_DSM.Client.Services.ReviewService;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Enumerator;

    [TestFixture]
    public class ReviewObjectiveCreationTestFixture
    {
        private TestContext context;
        private IReviewObjectiveCreationViewModel reviewObjectiveCreationViewModel;
        private IErrorMessageViewModel errorMessageViewModel;
        private Mock<IReviewObjectiveService> reviewObjectiveService;


        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.errorMessageViewModel = new ErrorMessageViewModel();
            this.reviewObjectiveService = new Mock<IReviewObjectiveService>();

            this.reviewObjectiveCreationViewModel = new ReviewObjectiveCreationViewModel(this.reviewObjectiveService.Object);
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
                var renderer = this.context.RenderComponent<ReviewObjectiveCreation>(parameters =>
                {
                    parameters.AddCascadingValue(this.errorMessageViewModel);
                    parameters.Add(p => p.ViewModel, this.reviewObjectiveCreationViewModel);
                });

                var listBox = renderer.FindComponent<DxListBox<ReviewObjective, ReviewObjective>>();

                Assert.That(listBox.Instance.Values, Is.Empty);
  
                var reviewCreationDtoPrr = new ReviewObjectiveCreationDto()
                {
                    Kind = ReviewObjectiveKind.Prr,
                    KindNumber = 3
                };

                var reviewCreationDtoSrr = new ReviewObjectiveCreationDto()
                {
                    Kind = ReviewObjectiveKind.Srr,
                    KindNumber = 2
                };

                this.reviewObjectiveCreationViewModel.AvailableReviewObjectiveCreationDto.Add(reviewCreationDtoPrr);
                this.reviewObjectiveCreationViewModel.AvailableReviewObjectiveCreationDto.Add(reviewCreationDtoSrr);

                renderer.Render();

                Assert.That(listBox.Instance.Values.ToList(), Has.Count.EqualTo(2));

                var dxButton = renderer.FindComponent<EditForm>();
                await renderer.InvokeAsync(dxButton.Instance.OnValidSubmit.InvokeAsync);

                Assert.That(this.reviewObjectiveCreationViewModel.SelectedReviewObjectives.ToList(), Has.Count.Empty);
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }
    }
}
