// --------------------------------------------------------------------------------------------------------
// <copyright file="ThingManagerTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Tests.Managers
{
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Managers.ThingManager;
    using UI_DSM.Server.Services.CometService;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ThingManagerTestFixture
    {
        private ThingManager thingManager;
        private Mock<ICometService> cometService;
        private Iteration iteration;
        private Model model;

        [SetUp]
        public void Setup()
        {
            this.cometService = new Mock<ICometService>();
            this.thingManager = new ThingManager(this.cometService.Object);
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
                            Category =
                            {
                                new Category(Guid.NewGuid(), null, null)
                                {
                                    IsDeprecated = true
                                }
                            }
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

            this.model = new Model(Guid.NewGuid())
            {
                IterationId = this.iteration.Iid
            };

            this.cometService.Setup(x => x.GetIteration(this.model)).ReturnsAsync(this.iteration);
        }

        [Test]
        public async Task VerifyGetIterations()
        {
            var things = (await this.thingManager.GetIterations(new List<Model> { this.model })).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(things, Is.Not.Empty);
                Assert.That(things.FirstOrDefault(x => x.Iid == this.iteration.Iid), Is.EqualTo(this.iteration));
            });

            things = (await this.thingManager.GetIterations(new List<Model> { new () })).ToList();
            Assert.That(things, Is.Empty);
        }

        [Test]
        public async Task VerifyGetThings()
        {
            var requirementsSpecification = (await this.thingManager.GetThings(new List<Model> { this.model },
                ClassKind.RequirementsSpecification)).ToList();

            var elements = await this.thingManager.GetThings(new List<Model> { this.model, new () }, ClassKind.ElementDefinition);
            var options = await this.thingManager.GetThings(new List<Model> { this.model }, ClassKind.Option);

            Assert.Multiple(() =>
            {
                Assert.That(requirementsSpecification, Is.Not.Empty);
                Assert.That(elements, Is.Not.Empty);
                Assert.That(options, Is.Empty);
                Assert.That(requirementsSpecification.Where(x => x is Requirement {IsDeprecated: true}), Is.Empty);
                Assert.That(requirementsSpecification.Where(x => x is Category {IsDeprecated: true}), Is.Not.Empty);
            });
        }
    }
}
