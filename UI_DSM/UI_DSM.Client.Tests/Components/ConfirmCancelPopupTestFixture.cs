// --------------------------------------------------------------------------------------------------------
// <copyright file="ConfirmCancelPopupTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components
{
    using Bunit;

    using DevExpress.Blazor;

    using Microsoft.AspNetCore.Components;

    using NUnit.Framework;

    using UI_DSM.Client.Components;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ConfirmCancelPopupTestFixture
    {
        private TestContext context;
        private int callbackCallCount;
        private ConfirmCancelPopupViewModel viewModel;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();

            this.viewModel = new ConfirmCancelPopupViewModel()
            {
                CancelRenderStyle = ButtonRenderStyle.Info,
                ConfirmRenderStyle = ButtonRenderStyle.Danger,
                ContentText = "Content",
                HeaderText = "Header",
                OnCancel = new EventCallbackFactory().Create(this, () => this.callbackCallCount--),
                OnConfirm = new EventCallbackFactory().Create(this, () => this.callbackCallCount++),
                IsVisible = true
            };
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifyComponentsParameter()
        {
            var renderer = this.context.RenderComponent<ConfirmCancelPopup>(
                ComponentParameter.CreateParameter("ViewModel", this.viewModel));

            await renderer.InvokeAsync(this.viewModel.OnCancel.InvokeAsync);
            Assert.That(this.callbackCallCount, Is.EqualTo(-1));
            await renderer.InvokeAsync(this.viewModel.OnConfirm.InvokeAsync);
            Assert.That(this.callbackCallCount, Is.EqualTo(0));
        }
    }
}
