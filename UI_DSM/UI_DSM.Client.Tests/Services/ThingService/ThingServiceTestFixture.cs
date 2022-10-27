// --------------------------------------------------------------------------------------------------------
// <copyright file="ThingServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Services.ThingService
{
    using System.Net;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using NUnit.Framework;

    using RichardSzalay.MockHttp;

    using UI_DSM.Client.Extensions;
    using UI_DSM.Client.Services;
    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Client.Services.ThingService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Extensions;

    [TestFixture]
    public class ThingServiceTestFixture
    {
        private ThingService thingService;
        private MockHttpMessageHandler httpMessageHandler;
        private IJsonService jsonService;
        private Guid projectId;
        private List<Guid> modelsId;
        private Iteration iteration;

        [SetUp]
        public void Setup()
        {
            this.httpMessageHandler = new MockHttpMessageHandler();
            var httpClient = this.httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/api");

            this.projectId = Guid.NewGuid();

            this.modelsId = new List<Guid>
            {
                Guid.NewGuid()
            };

            ServiceBase.RegisterService<ThingService>();
            this.jsonService = JsonSerializerHelper.CreateService();
            this.thingService = new ThingService(httpClient, this.jsonService);

            this.iteration = new Iteration(Guid.NewGuid(), null, null);

            var requirementsSpecification = new List<RequirementsSpecification>
            {
                new(Guid.NewGuid(), null, null)
                {
                    Requirement =
                    {
                        new Requirement(Guid.NewGuid(), null, null)
                        {
                            Definition =
                            {
                                new Definition(Guid.NewGuid(), null, null)
                            }
                        },
                        new Requirement(Guid.NewGuid(), null, null)
                        {
                            Definition =
                            {
                                new Definition(Guid.NewGuid(), null, null)
                            },
                        },
                        new Requirement(Guid.NewGuid(), null, null)
                        {
                            IsDeprecated = true,
                            Definition =
                            {
                                new Definition(Guid.NewGuid(), null, null)
                            }
                        }
                    }
                }
            };

            this.iteration.RequirementsSpecification.AddRange(requirementsSpecification);

            var elements = new List<ElementDefinition>
            {
                new (Guid.NewGuid(), null, null)
                {
                    Parameter =
                    {
                        new Parameter(Guid.NewGuid(), null, null),
                        new Parameter(Guid.NewGuid(), null, null)
                    }
                }
            };

            this.iteration.Element.AddRange(elements);

            this.iteration.Container = new EngineeringModel(Guid.NewGuid(), null, null);
        }

        [Test]
        public async Task VerifyGetThings()
        {
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When($"/Project/{this.projectId}/Model/{this.modelsId.ToGuidArray()}/Thing");
            request.Respond(_ => httpResponse);

            var things = await this.thingService.GetThings(this.projectId, new List<Guid>());
            Assert.That(things, Is.Empty);

            things = await this.thingService.GetThings(this.projectId, this.modelsId.First());
            Assert.That(things, Is.Empty);

            httpResponse.StatusCode = HttpStatusCode.OK;

            var dtos = this.jsonService.Serialize(this.iteration.RequirementsSpecification.GetContainedAndReferencedThings());
            httpResponse.Content = new StringContent(dtos);
            things = await this.thingService.GetThings(this.projectId, this.modelsId.First(), ClassKind.RequirementsSpecification);
            Assert.That(things, Is.Not.Empty);

            dtos = this.jsonService.Serialize(this.iteration.RequirementsSpecification.GetContainedAndReferencedThings());
            httpResponse.Content = new StringContent(dtos);
            things = await this.thingService.GetThingsByView(this.projectId, this.modelsId, View.RequirementBreakdownStructureView);
            Assert.That(things, Is.Not.Empty);

            dtos = this.jsonService.Serialize(this.iteration.RequirementsSpecification.GetContainedAndReferencedThings());
            httpResponse.Content = new StringContent(dtos);
            things = await this.thingService.GetThingsByView(this.projectId, this.modelsId, View.ProductBreakdownStructureView);
            Assert.That(things, Is.Not.Empty);
        }
    }
}
