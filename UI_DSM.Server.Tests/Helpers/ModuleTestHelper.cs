// --------------------------------------------------------------------------------------------------------
// <copyright file="ModuleTestHelper.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Tests.Helpers
{
    using Carter;

    using FluentValidation;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;

    using Moq;

    using UI_DSM.Server.Modules;
    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     Helper class that handles common setup for all <see cref="ModuleBase" /> test class
    /// </summary>
    public static class ModuleTestHelper
    {
        /// <summary>
        ///     Setup all mocks that will be usefull for testing
        /// </summary>
        /// <typeparam name="TModule">A <see cref="ModuleBase" /></typeparam>
        /// <typeparam name="TDto">A <see cref="EntityDto" /></typeparam>
        /// <param name="validator">A <see cref="AbstractValidator{T}" /> instance</param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="response">The <see cref="HttpResponse" /></param>
        /// <param name="request">The <see cref="HttpRequest" /></param>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/></param>
        public static void Setup<TModule, TDto>(AbstractValidator<TDto> validator, out Mock<HttpContext> context, 
            out Mock<HttpResponse> response, out Mock<HttpRequest> request, out Mock<IServiceProvider> serviceProvider) where TModule : ModuleBase where TDto : class
        {
            response = new Mock<HttpResponse>();
            response.SetupSet(response => response.StatusCode = It.IsAny<int>()).Verifiable();
            context = new Mock<HttpContext>();

            request = new Mock<HttpRequest>();
            request.Setup(x => x.HttpContext).Returns(context.Object);

            context.Setup(x => x.Request).Returns(request.Object);
            context.Setup(x => x.Response).Returns(response.Object);
            var headers = new Mock<IHeaderDictionary>();
            request.Setup(x => x.Headers).Returns(headers.Object);
            response.Setup(x => x.HttpContext).Returns(context.Object);

            ModuleBase.RegisterModule<TModule>();
            var mockValidator = new Mock<IValidatorLocator>();
            mockValidator.Setup(x => x.GetValidator<TDto>()).Returns(validator);

            serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(typeof(IValidatorLocator))).Returns(mockValidator.Object);

            var negotiator = new Mock<IResponseNegotiator>();
            negotiator.Setup(x => x.CanHandle(It.IsAny<MediaTypeHeaderValue>())).Returns(true);

            serviceProvider.Setup(x => x.GetService(typeof(IEnumerable<IResponseNegotiator>))).Returns(new List<IResponseNegotiator>()
            {
                negotiator.Object
            });

            context.Setup(x => x.RequestServices).Returns(serviceProvider.Object);
        }
    }
}
